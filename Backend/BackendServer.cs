using System;
using Lidgren.Network;
using symmnet.shared.rater193;
using System.Collections.Generic;
using System.Threading;

namespace symmnet.backend
{
    class BackendServer
	{
		public static NetServer server;
		public static List<Client> clientList = new List<Client>();
		public static List<Server> serverList = new List<Server>();
		public static List<object[]> logList = new List<object[]>();

		public static ConsoleColor logColour = ConsoleColor.Cyan;

		static void Main(string[] args)
		{
			StartServer();
        }

		#region Setup the server
		private static void StartServer()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("MMO");
			config.Port = 8002;
			config.MaximumConnections = 100;
			config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
			server = new NetServer(config);
			server.Start();

			Console.ForegroundColor = logColour;
			Console.Write("NA REGION: ");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("Offline");
			Console.WriteLine("");
			Console.ForegroundColor = logColour;
			Console.Write("EU REGION: ");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("Offline");
			Console.WriteLine("");
			Console.ForegroundColor = logColour;
			Console.Write("OCE REGION: ");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("Offline");
			Console.WriteLine("");
			Console.ForegroundColor = logColour;
			Console.Write("CLIENTS CONNECTED: " + clientList.Count);
			Console.WriteLine("");
			Console.ForegroundColor = ConsoleColor.White;

			while(true)
				Listen();
		}
		#endregion

		#region Listen for incoming messages
		private static void Listen()
		{
			NetIncomingMessage data;
			Server server_;
			Client client;

			if((data = server.ReadMessage()) != null)
			{
				switch(data.MessageType)
				{
					case NetIncomingMessageType.ConnectionApproval:
						switch(data.ReadByte())
						{
							case (byte)1: //SERVER
								data.SenderConnection.Approve();
								server_ = new Server(data.SenderConnection);
								serverList.Add(server_);
								UpdateConsole();
								break;
							case (byte)2: //CLIENT
								data.SenderConnection.Approve();
								client = new Client(data.SenderConnection);
								clientList.Add(client);

								bool canSetId = true;
								if(clientList.Count > 1)
								{
									for(int i = 1; i < (clientList.Count + 1); i++)
									{
										canSetId = true;

										for(int i_ = 0; i_ < clientList.Count; i_++)
										{
											if(clientList[i_].clientId == i)
												canSetId = false;
										}

										if(canSetId == true)
											client.clientId = i;
									}
								}
								else
									client.clientId = 1;

								logList.Add(new object[] { logColour, "Client " + client.connection + " connected" });
								UpdateConsole();
								break;
						}
						break;
					case NetIncomingMessageType.Data:
						server_ = GetServer(data.SenderConnection);
						if(server_ != null)
							DataHandle.HandleServer(server_, data);
						client = GetClient(data.SenderConnection);
						if(client != null)
							DataHandle.HandleClient(client, data);
						break;
					case NetIncomingMessageType.StatusChanged:
						if(data.SenderConnection.Status == NetConnectionStatus.Disconnected || data.SenderConnection.Status == NetConnectionStatus.Disconnecting)
						{
							server_ = GetServer(data.SenderConnection);
							RemoveServer(server_);
							client = GetClient(data.SenderConnection);
							RemoveClient(client);
						}
						break;
				}
			}

			Thread.Sleep(1);
		}
		#endregion

		#region Get a server from the server list
		public static Server GetServer(NetConnection connection)
		{
			Server serverReturn = null;

			for(var i = 0; i < serverList.Count; i++)
			{
				if(serverList[i].connection == connection)
					serverReturn = serverList[i];
			}

			return serverReturn;
		}
		public static Server GetServer(int serverId)
		{
			Server serverReturn = null;

			for(var i = 0; i < serverList.Count; i++)
			{
				if(serverList[i].serverId == serverId)
					serverReturn = serverList[i];
			}

			return serverReturn;
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

		#region Remove a server from the server list
		public static void RemoveServer(Server server)
		{
			if(server != null)
			{
				server.timeoutTimer.Stop();
				serverList.Remove(server);
				UpdateConsole();
			}
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
			}
		}
		#endregion

		#region Update the console log
		public static void UpdateConsole()
		{

			Console.Clear();

			Console.ForegroundColor = logColour;
			if(GetServer(0) != null)
			{
				Console.Write("MAP EDITOR: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Online");
				Console.WriteLine("");
			}
			else
			{
				Console.Write("MAP EDITOR: ");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("Offline");
				Console.WriteLine("");
			}
			Console.ForegroundColor = logColour;
			if(GetServer(1) != null)
			{
				Console.Write("NORTH AMERICA: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Online");
				Console.WriteLine("");
			}
			else
			{
				Console.Write("NORTH AMERICA: ");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("Offline");
				Console.WriteLine("");
			}
			Console.ForegroundColor = logColour;
			if(GetServer(2) != null)
			{
				Console.Write("EUROPE: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Online");
				Console.WriteLine("");
			}
			else
			{
				Console.Write("EUROPE: ");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("Offline");
				Console.WriteLine("");
			}
			Console.ForegroundColor = logColour;
			if(GetServer(3) != null)
			{
				Console.Write("OCEANIA: ");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("Online");
				Console.WriteLine("");
			}
			else
			{
				Console.Write("OCEANIA: ");
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("Offline");
				Console.WriteLine("");
			}
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
	}
}
