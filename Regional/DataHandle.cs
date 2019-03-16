using Lidgren.Network;
using symmnet.regional.rater193;
using symmnet.shared.rater193;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace symmnet.regional
{
	public static class DataHandle
	{
		public static void HandleBES(NetIncomingMessage data)
		{
			switch(data.ReadByte())
			{
				case (byte)1: //PENDING CLIENT
					RegionalServer.logList.Add(new object[] { RegionalServer.logColour, "Pending client connection" });
					RegionalServer.UpdateConsole();
					Client client = new Client(data.ReadInt32());
					RegionalServer.clientList.Add(client);
					client.username = data.ReadString();
					client.authority = data.ReadInt32();
					client.mapEditor = data.ReadBoolean();
					client.level = data.ReadInt32();
					client.xp = data.ReadFloat();
					client.skinColour = data.ReadString();
					client.hairColour = data.ReadString();
					client.clothingColour = data.ReadString();
					int size = data.ReadInt32();
					if(size > 1)
						client.avatar = data.ReadBytes(size);

					NetOutgoingMessage message = RegionalServer.serverClient.CreateMessage();
					message.Write((byte)3); //ADDED CLIENT
					message.Write(client.clientId);
					RegionalServer.serverClient.SendMessage(message, NetDeliveryMethod.ReliableOrdered, 0);
					break;
			}
		}

		public static void HandleClient(Client client, NetIncomingMessage data)
		{
			switch (data.ReadByte())
			{
				case 1: //PING
					client.timeoutTimer.Stop();
					client.timeoutTimer.Start();
					break;
					#region World data
				case 2: //WORLD
					switch (data.ReadByte())
					{
						case 1: //JOINED WORLD
							NetOutgoingMessage message = RegionalServer.server.CreateMessage();
							message.Write((byte)1); //WORLD
							message.Write((byte)1); //SPAWNING
							message.Write((byte)1); //SPAWN SELF
							RegionalServer.server.SendMessage(message, client.connection, NetDeliveryMethod.ReliableOrdered, 0);

							for(int i = 0; i < RegionalServer.clientList.Count; i++)
							{
								Client client_ = RegionalServer.clientList[i];
								if(client_ != client)
								{
									//Sending self to other clients
									message = RegionalServer.server.CreateMessage();
									message.Write((byte)1); //WORLD
									message.Write((byte)1); //SPAWNING
									message.Write((byte)2); //SPAWN OTHER
									message.Write(client.clientId);
									message.Write(client.username);
									message.Write(client.authority);
									message.Write(client.level);
									message.Write(client.skinColour);
									message.Write(client.hairColour);
									message.Write(client.clothingColour);
									message.Write(client.playerPosX);
									message.Write(client.playerPosY);
									RegionalServer.server.SendMessage(message, client_.connection, NetDeliveryMethod.ReliableOrdered, 0);

									//sending other clients to self
									message = RegionalServer.server.CreateMessage();
									message.Write((byte)1); //WORLD
									message.Write((byte)1); //SPAWNING
									message.Write((byte)2); //SPAWN OTHER
									message.Write(client_.clientId);
									message.Write(client_.username);
									message.Write(client_.authority);
									message.Write(client_.level);
									message.Write(client_.skinColour);
									message.Write(client_.hairColour);
									message.Write(client_.clothingColour);
									message.Write(client_.playerPosX);
									message.Write(client_.playerPosY);
									RegionalServer.server.SendMessage(message, client.connection, NetDeliveryMethod.ReliableOrdered, 0);
								}

							}
							Debug.Log("Send map");
							//Triggering rater193's event to handle maps!
							Rater193.onPlayerJoined.Invoke(client);
							break;

						case 2: //PLAYER
							switch(data.ReadByte())
							{
								#region Player position
								case 1: //POSITION
									bool illegalPos = false;
									float[] position = new float[2] { data.ReadFloat(), data.ReadFloat() };
									client.posHistory.RemoveAt(0);
									client.posHistory.Add(position);
									float[] pos1 = client.posHistory[0];
									float[] pos2 = client.posHistory[1];
									float posX1 = pos1[0];
									float posY1 = pos1[1];
									float posX2 = pos2[0];
									float posY2 = pos2[1];

									double distance = Math.Sqrt(Math.Pow(posX2 - posX1, 2) + Math.Pow(posY2 - posY1, 2));
									if(distance <= client.playerStepMargin)
									{
										client.playerPosX = posX2;
										client.playerPosY = posY2;
									}
									else
									{
										client.playerPosX = posX1;
										client.playerPosY = posY1;
										client.posHistory[0] = new float[2] { posX1, posY1 };
										client.posHistory[1] = new float[2] { posX1, posY1 };
										illegalPos = true;
									}

									for(int i = 0; i < RegionalServer.clientList.Count; i++)
									{
										Client client_ = RegionalServer.clientList[i];
										if(client_.connection != null)
										{
											message = RegionalServer.server.CreateMessage();
											message.Write((byte)1); //WORLD
											message.Write((byte)2); //PLAYER
											message.Write((byte)1); //POSITION
											message.Write(client.clientId);
											message.Write(illegalPos);
											message.Write(client.playerPosX);
											message.Write(client.playerPosY);
											RegionalServer.server.SendMessage(message, client_.connection, NetDeliveryMethod.Unreliable, 0);
										}
									}
									break;
								#endregion
								#region Player looking
								case 2: //ROTATION
									for(int i = 0; i < RegionalServer.clientList.Count; i++)
									{
										Client client_ = RegionalServer.clientList[i];
										if(client_ != client && client_.connection != null)
										{
											message = RegionalServer.server.CreateMessage();
											message.Write((byte)1); //WORLD
											message.Write((byte)2); //PLAYER
											message.Write((byte)2); //POSITION
											message.Write(client.clientId);
											message.Write(data.ReadFloat());
											message.Write(data.ReadBoolean());
											RegionalServer.server.SendMessage(message, client_.connection, NetDeliveryMethod.Unreliable, 0);
										}
									}
									break;
									#endregion
							}
							break;						
						case 9: //CHAT
							for(int i = 0; i < RegionalServer.clientList.Count; i++)
							{
								byte mode = data.ReadByte();
								switch(mode)
								{
									case 1: //ALL
										message = RegionalServer.server.CreateMessage();
										message.Write((byte)1); //WORLD
										message.Write((byte)9); //CHAT
										message.Write(mode);
										message.Write(client.authority);
										message.Write(client.username);
										message.Write(data.ReadString());
										RegionalServer.server.SendMessage(message, RegionalServer.clientList[i].connection, NetDeliveryMethod.Unreliable, 0);
										break;
									case 2: //GLOBAL
										message = RegionalServer.server.CreateMessage();
										message.Write((byte)1); //WORLD
										message.Write((byte)9); //CHAT
										message.Write(mode);
										message.Write(client.authority);
										message.Write(client.username);
										message.Write(data.ReadString());
										RegionalServer.server.SendMessage(message, RegionalServer.clientList[i].connection, NetDeliveryMethod.Unreliable, 0);
										break;
								}
							}
							break;
						#region rater193's handler
						case 255:
							Rater193.HandleData(client, data);
							break;
						#endregion
					}
					break;
				#endregion
			}
		}
	}
}
