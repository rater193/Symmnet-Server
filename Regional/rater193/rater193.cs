using Lidgren.Network;
using Regional.rater193.Entities;
using Regional.rater193.Entities.TileEntity;
using symmnet.regional.rater193.Map;
using symmnet.regional.rater193.NetworkMessages;
using symmnet.regional.rater193.NetworkMessages.Messages.rater193;
using symmnet.regional.rater193.NetworkMessages.Player;
using symmnet.shared.rater193;
using System;
using System.Collections.Generic;
using System.Text;

namespace symmnet.regional.rater193
{
	class Rater193
    {


		public class StaticRooms
		{
			public static MapManager world;
			public static MapManager test;
		};

		public static Action onStartServer;
		public static Action<Client> onPlayerJoined;
		public static Action<Client> onPlayerDisconnect;
		public static Action onUpdate;
		public static Debug dbgServerChat = new Debug("Server Chat");
		public static NetworkMessageController msgController = new NetworkMessageController();

		public static DateTime previousUpdateTime;

		


		public static void Init(string[] args)
		{
			Debug.Log(ConsoleColor.Cyan, "Initializing!");
			#region TileEntityRegistration
			RegisterTileEntities();
			#endregion

			#region network message registration
			msgController.register(Messages.ServerGetChunk, new NetworkMessageServerGetChunk());
			msgController.register(Messages.ServerSetTile, new NetworkMessageServerSetTile());
			msgController.register(Messages.ServerDeleteTile, new NetworkMessageServerDeleteTile());
			msgController.register(Messages.ServerDeleteColumn, new NetworkMessageServerDeleteColumn());
			#endregion

			#region Map creator

			StaticRooms.world = new MapManager("World");
			StaticRooms.test = new MapManager();

			#endregion

			#region Registering actions
			onStartServer += OnStartServer;
			onPlayerJoined += OnPlayerJoined;
			onPlayerDisconnect += OnPlayerDisconnect;
			#endregion

			onUpdate += NetworkPlayer.Update;
		}

		public static void RegisterTileEntities()
		{
			//Just dicking around with ores :P
			new TileEntityOre(99).renderID = 100;
			new TileEntityOre(100).renderID = 101;
			new TileEntityOre(101).renderID = 102;
			new TileEntityOre(102).renderID = 103;
			new TileEntityOre(103).renderID = 104;
			new TileEntityOre(104).renderID = 105;
			new TileEntityOre(105).renderID = 106;
			new TileEntityOre(106).renderID = 107;
			new TileEntityOre(107).renderID = 108;
			new TileEntityOre(108).renderID = 109;
			new TileEntityOre(109).renderID = 110;
			new TileEntityOre(110).renderID = 111;
			new TileEntityOre(111).renderID = 112;
			new TileEntityOre(112).renderID = 113;

			new TileEntityOre(200).renderID = 4000;
			new TileEntityOre(201).renderID = 4001;
			new TileEntityOre(202).renderID = 4002;
			new TileEntityOre(203).renderID = 4003;
			new TileEntityOre(204).renderID = 4004;
			new TileEntityOre(205).renderID = 4005;
			new TileEntityOre(206).renderID = 4006;
		}

		public static void Shutdown()
		{

		}

		public static void Update()
		{
			if(previousUpdateTime==null)
			{
				previousUpdateTime = DateTime.Now;
			}

			TimeSpan span = DateTime.Now - previousUpdateTime;
			double ms = span.TotalMilliseconds;
			if (ms >= 1000 / 20)
			{
				onUpdate.Invoke();
				previousUpdateTime = DateTime.Now;
			}
		}

		public static void OnStartServer()
		{
			Debug.Log(ConsoleColor.Red, "Loading rooms!");
			StaticRooms.world.SaveMap();
			StaticRooms.test.SaveMap();
			System.Threading.Thread.Sleep(2000);
		}

		public static void OnPlayerJoined(Client client)
		{
			NetworkPlayer.OnClientConnect(client);
			if (RegionalServer.IsMapEditor())
			{
				//Send the tile entity data here

				NetOutgoingMessage msg = RegionalServer.server.CreateMessage();
				msg.Write((byte)1); //WORLD
				msg.Write((byte)255); //Rater193's header message group
				msg.Write((byte)symmnet.regional.rater193.Messages.ClientReceiveTileEntityList);

				//Writing the ammount of tile entities to receive
				msg.Write((int)TileEntityBase.registeredEntities.Count);

				foreach(int key in TileEntityBase.registeredEntities.Keys)
				{
					//Here we write the tile entity id, and the render id to use for it
					msg.Write((int)key);
					msg.Write((int)TileEntityBase.GetTileEntity(key).renderID);
				}
				client.connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 2);
			}
		}

		public static void OnPlayerDisconnect(Client client)
		{
			NetworkPlayer.OnClientDisconnect(client);
		}

		internal static void HandleConsole()
		{
			bool running = true;

			//Console.Clear();
			while(running)
			{
				string msg = Console.ReadLine();
				if (msg[0] == '/')
				{
					string[] cmds = msg.Split(' ');
					switch(cmds[0])
					{
						case "/stop":
							running = false;
							Debug.Log("Stopping server...");
							System.Threading.Thread.Sleep(500);
							break;

						case "/help":

							Console.WriteLine();
							Console.ForegroundColor = ConsoleColor.Green;
							Console.WriteLine("========================================================================");
							Console.WriteLine();

							Console.WriteLine();
							Console.ForegroundColor = ConsoleColor.Green;
							Console.Write("/help");
							Console.WriteLine();
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("     - General help command");


							Console.WriteLine();
							Console.ForegroundColor = ConsoleColor.Green;
							Console.Write("/stop");
							Console.WriteLine();
							Console.ForegroundColor = ConsoleColor.Gray;
							Console.Write("     - Shuts down the regional server!");

							Console.WriteLine();
							Console.WriteLine();
							Console.ForegroundColor = ConsoleColor.Green;
							Console.WriteLine("========================================================================");
							Console.WriteLine();

							System.Threading.Thread.Sleep(500);
							break;

						default:
							Debug.Log("Unknown command, " + cmds[0]);
							break;
					}
				}
				else
				{
					//TODO: Broadcast message when sending message!
					//broadcastMessage(msg);
					dbgServerChat.log(msg);
				}
			}
		}

		public static void broadcastMessage(params object[] msgParts)
		{
			string message = "";
			foreach (object frag in msgParts)
			{
				message += " " + frag;
			}
			
		}

		internal static void HandleData(Client client, NetIncomingMessage data)
		{
			if (client != null && client.connection != null && client.mapEditor == true)
			{
				msgController.execute(data.ReadByte(), client, data);
			}
		}
	}
}
