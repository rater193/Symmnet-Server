using Lidgren.Network;
using symmnet.shared.rater193;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace symmnet.backend
{
    public static class DataHandle
    {
		public static void HandleServer(Server server, NetIncomingMessage data)
		{
			switch(data.ReadByte())
			{
				case 1: //PING
					server.timeoutTimer.Stop();
					server.timeoutTimer.Start();
					break;
				case 2: //SERVER ID
					server.serverId = data.ReadInt32();
					BackendServer.UpdateConsole();
					break;
				case 3: //ADDED CLIENT
					Client client = BackendServer.GetClient(data.ReadInt32());
					BackendServer.logList.Add(new object[] { BackendServer.logColour, "Regional server added client: " + client.connection });
					BackendServer.UpdateConsole();
					NetOutgoingMessage message = BackendServer.server.CreateMessage();
					message.Write((byte)2); //ACCOUNT
					message.Write((byte)1); //LOGIN
					message.Write(client.username);
					message.Write(client.authority);
					message.Write(client.mapEditor);
					message.Write(client.level);
					message.Write(client.xp);
					message.Write(client.skinColour);
					message.Write(client.hairColour);
					message.Write(client.clothingColour);
					message.Write(client.avatar.Length);
					message.Write(client.avatar);
					message.Write(server.serverId);
					BackendServer.server.SendMessage(message, client.connection, NetDeliveryMethod.ReliableOrdered, 0);
					break;
			}
		}

		public static void HandleClient(Client client, NetIncomingMessage data)
		{
			switch(data.ReadByte())
			{
				case 1: //PING
					BackendServer.logList.Add(new object[] { ConsoleColor.Green, "Received client ping: " + client.connection });
					BackendServer.UpdateConsole();
					client.timeoutTimer.Stop();
					client.timeoutTimer.Start();
					break;
				case 2: //CLIENT ID
					NetOutgoingMessage message = BackendServer.server.CreateMessage();
					message.Write((byte)1);
					message.Write(client.clientId);
					BackendServer.server.SendMessage(message, client.connection, NetDeliveryMethod.ReliableOrdered, 0);
					break;
				#region Account data
				case 3: //ACCOUNT
					switch(data.ReadByte())
					{
						case 1: //LOGIN
							BackendServer.logList.Add(new object[] { BackendServer.logColour, "Client " + client.connection + " login: " + client.loggedIn });
							BackendServer.UpdateConsole();
							if(client.loggedIn == false)
							{
								string username = data.ReadString();
								string path = Path.Combine(Path.GetFullPath(Directory.GetCurrentDirectory()) + "/PlayerDatabase", username);
								string filePath = Path.Combine(path, "Info.txt");

								if(Directory.Exists(path) == false)
								{
									Directory.CreateDirectory(path);

									using(var writer = new StreamWriter(filePath))
									{
										writer.WriteLine("[MAPEDITOR]FALSE");
										writer.WriteLine("[AUTHORITY]1");
										writer.WriteLine("[LEVEL]1");
										writer.WriteLine("[XP]0");
									}
									filePath = Path.Combine(path, "Friends.txt");
									var file = File.Create(filePath);
									file.Close();
									filePath = Path.Combine(path, "Equipment.txt");
									using(var writer = new StreamWriter(filePath))
									{
										writer.WriteLine("[SKINCOL]#ffffff");
										writer.WriteLine("[HAIRCOL]#ffffff");
										writer.WriteLine("[CLOTHCOL]#ffffff");
										writer.WriteLine("[MAINHAND]empty");
										writer.WriteLine("[OFFHAND]empty");
									}
								}

								client.username = username;
								filePath = Path.Combine(path, "Info.txt");
								using(var reader = new StreamReader(filePath))
								{
									string mapEditor = reader.ReadLine();
									mapEditor = mapEditor.Replace("[MAPEDITOR]", "");
									string authority = reader.ReadLine();
									authority = authority.Replace("[AUTHORITY]", "");
									string level = reader.ReadLine();
									level = level.Replace("[LEVEL]", "");
									string xp = reader.ReadLine();
									xp = xp.Replace("[XP]", "");

									client.mapEditor = bool.Parse(mapEditor);
									client.authority = int.Parse(authority);
									client.level = int.Parse(level);
									client.xp = float.Parse(xp);
								}
								filePath = Path.Combine(path, "Equipment.txt");
								using(var reader = new StreamReader(filePath))
								{
									string skinColour = reader.ReadLine();
									skinColour = skinColour.Replace("[SKINCOL]", "");
									string hairColour = reader.ReadLine();
									hairColour = hairColour.Replace("[HAIRCOL]", "");
									string clothingColour = reader.ReadLine();
									clothingColour = clothingColour.Replace("[CLOTHCOL]", "");

									client.skinColour = skinColour;
									client.hairColour = hairColour;
									client.clothingColour = clothingColour;
								}

								if(File.Exists(Path.Combine(path, "Avatar.png")))
								{
									filePath = Path.Combine(path, "Avatar.png");
									byte[] buffer = File.ReadAllBytes(filePath);
									MemoryStream stream = new MemoryStream(buffer);
									client.avatar = new byte[stream.Length];
									client.avatar = stream.ToArray();
								}

								int serverId = data.ReadInt32();
								Server server = BackendServer.GetServer(serverId);
								if(server != null)
								{
									if(serverId == 0)
									{
										if(client.mapEditor == true)
										{
											message = BackendServer.server.CreateMessage();
											message.Write((byte)1); //PENDING CLIENT
											message.Write(client.clientId);
											message.Write(client.username);
											message.Write(client.authority);
											message.Write(client.mapEditor);
											message.Write(client.level);
											message.Write(client.xp);
											message.Write(client.skinColour);
											message.Write(client.hairColour);
											message.Write(client.clothingColour);
											message.Write(client.avatar.Length);
											message.Write(client.avatar);
											BackendServer.server.SendMessage(message, server.connection, NetDeliveryMethod.ReliableOrdered, 0);
										}
									}
									else
									{
										message = BackendServer.server.CreateMessage();
										message.Write((byte)1); //PENDING CLIENT
										message.Write(client.clientId);
										message.Write(client.username);
										message.Write(client.authority);
										message.Write(client.mapEditor);
										message.Write(client.level);
										message.Write(client.xp);
										message.Write(client.skinColour);
										message.Write(client.hairColour);
										message.Write(client.clothingColour);
										message.Write(client.avatar.Length);
										message.Write(client.avatar);
										BackendServer.server.SendMessage(message, server.connection, NetDeliveryMethod.ReliableOrdered, 0);
									}

									client.loggedIn = true;
								}
							}
							break;
					}
					break;
				#endregion
			}
		}
	}
}
