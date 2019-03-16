using symmnet.regional.rater193;
using System;
using Lidgren.Network;
using symmnet.shared.rater193;
using System.Collections.Generic;
using System.Threading;

namespace symmnet.regional
{
    class RegionalServer
	{
		private static bool shutdown = false;
		private static int serverId = 0;
		public static NetServer server;
		public static NetClient serverClient;
		public static List<Client> clientList = new List<Client>();
		public static List<object[]> logList = new List<object[]>();

		public static ConsoleColor logColour = ConsoleColor.Cyan;

		static void Main(string[] args)
		{
			////////////////////////////////////
			//rater193-edit: Event hook!
			Rater193.Init(args);
			////////////////////////////////////
			StartServer();
		}

		public static bool IsMapEditor()
		{
			return serverId <= 0;
		}

		#region Setup the server
		private static void StartServer()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("MMO");
			config.Port = 8001;
			config.MaximumConnections = 100;
			config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
			server = new NetServer(config);
			server.Start();

			Console.ForegroundColor = logColour;
			Console.Write("CLIENTS CONNECTED: " + clientList.Count);
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.White;

			config = new NetPeerConfiguration("MMO");
			serverClient = new NetClient(config);
			serverClient.Start();
			NetOutgoingMessage message = serverClient.CreateMessage();
			message.Write((byte)1); //CONNECT
			serverClient.Connect("127.0.0.1", 8002, message);

			Thread thread = new Thread(ConsoleCommand);
			thread.Start();

			var startTimeSpan = TimeSpan.Zero;
			var periodTimeSpan = TimeSpan.FromSeconds(5);
			var timer = new Timer((e) => { Ping(); }, null, startTimeSpan, periodTimeSpan);

			while (shutdown == false)
			{
				Rater193.Update();
				Listen();
			}
			Rater193.Shutdown();

			serverClient.Disconnect("Quit");
			Thread.Sleep(250);
			Environment.Exit(0);
		}
		#endregion

		#region Listen for incoming messages
		private static void Listen()
		{
			NetIncomingMessage data;

			if((data = serverClient.ReadMessage()) != null) //BES
			{
				switch(data.MessageType)
				{
					case NetIncomingMessageType.Data:
						DataHandle.HandleBES(data);
						break;
					case NetIncomingMessageType.StatusChanged:
						if((NetConnectionStatus)data.ReadByte() == NetConnectionStatus.Connected)
						{
							NetOutgoingMessage message = serverClient.CreateMessage();
							message.Write((byte)2); //SERVER ID
							message.Write(serverId);
							serverClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
						}
						break;
				}
			}

			if((data = server.ReadMessage()) != null) //CLIENT
			{
				Client client;
				switch(data.MessageType)
				{
					case NetIncomingMessageType.ConnectionApproval:
						int clientId = data.ReadInt32();
						for(int i = 0; i < clientList.Count; i++)
						{
							client = clientList[i];
							if(client.clientId == clientId && client.connection == null)
							{
								data.SenderConnection.Approve();
								client.connection = data.SenderConnection;
								logList.Add(new object[] { logColour, "Client " + client.connection + " connected" });
								UpdateConsole();
							}
						}
						break;
					case NetIncomingMessageType.Data:
						client = GetClient(data.SenderConnection);
						if(client != null)
							DataHandle.HandleClient(client, data);
						break;
					case NetIncomingMessageType.StatusChanged:
						if(data.SenderConnection.Status == NetConnectionStatus.Disconnected || data.SenderConnection.Status == NetConnectionStatus.Disconnecting)
						{
							client = GetClient(data.SenderConnection);
							RemoveClient(client);
						}
						break;
				}
			}

			Thread.Sleep(1);
		}
		#endregion

		#region Send a ping to the backend server
		private static void Ping()
		{
			NetOutgoingMessage message = serverClient.CreateMessage(1);
			message.Write((byte)1); //PING
			serverClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
		}
		#endregion

		#region Get a client from the client list
		public static Client GetClient(NetConnection connection)
		{
			Client clientReturn = null;

			for(var i = 0; i < clientList.Count; i++)
			{
				if(clientList[i].connection == connection)
					clientReturn = clientList[i];
			}

			return clientReturn;
		}
		public static Client GetClient(int clientId)
		{
			Client clientReturn = null;

			for(var i = 0; i < clientList.Count; i++)
			{
				if(clientList[i].clientId == clientId)
					clientReturn = clientList[i];
			}

			return clientReturn;
		}
		#endregion

		#region Remove a client from the client list
		public static void RemoveClient(Client client)
		{
			if(client != null)
			{
				client.timeoutTimer.Stop();
				clientList.Remove(client);
				logList.Add(new object[] { logColour, "Client " + client.connection + " disconnected" });
				UpdateConsole();

				for(int i = 0; i < clientList.Count; i++)
				{
					NetOutgoingMessage message = server.CreateMessage();
					message.Write((byte)1); //WORLD
					message.Write((byte)1); //SPAWNING
					message.Write((byte)3); //REMOVE OTHER
					message.Write(client.clientId);
					server.SendMessage(message, clientList[i].connection, NetDeliveryMethod.ReliableOrdered);
				}
				Rater193.onPlayerDisconnect.Invoke(client);
			}
		}
		#endregion

		#region Update the console log
		public static void UpdateConsole()
		{
			
			Console.Clear();			
			Console.ForegroundColor = logColour;
			Console.Write("CLIENTS CONNECTED: " + clientList.Count);
			Console.WriteLine("\n");

			if(logList.Count > 10)
				logList.RemoveAt(0);

			for(int i = 0; i < logList.Count; i++)
			{
				object[] log = logList[i];
				Debug.Log((ConsoleColor)log[0], log[1].ToString());
			}

			Console.ForegroundColor = ConsoleColor.White;
			
		}
		#endregion

		#region Check console commands
		private static void ConsoleCommand()
		{
			if(Console.ReadLine() == "/stop")				
				shutdown = true;
		}
		#endregion
	}
}
