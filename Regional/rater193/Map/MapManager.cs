using Lidgren.Network;
using Regional.rater193.Entities;
using symmnet.regional.rater193.Map.Chunks;
using symmnet.shared.rater193;
using System;
using System.Collections.Generic;
using System.Text;

namespace symmnet.regional.rater193.Map
{
	class MapManager
	{

		private Debug debug;
		/// <summary>
		/// Settings for the MapManager
		/// </summary>
		public class Settings
		{
			#region Chunk settings
			public static readonly int
				chunkSize			= 16,
				chunkRenderDistance	= 3
				;
			#endregion
		}

		//Used for handling chunks
		private Dictionary<string, MapLayerChunk> layerchunks = new Dictionary<string, MapLayerChunk>();


		//These next 2 variables are used only with saving/loading the map/
		//Check the constructor on handling those 2 variables!
		public string mapName = "";
		public bool writeData;

		public MapManager(string mapName = null)
		{

			//Used to tell weather or not to save a map to the disk
			writeData = mapName == null ? false : true;

			//Setting the mapname to be used for reading/writing
			this.mapName = mapName;

			//Creating the debugger for this map
			debug = new Debug(writeData == true ? "MapManager-" + mapName : "MapManager");

			//Now loading from disk, if we havnt already
			if (writeData) { LoadMap(); }


		}

		/// <summary>
		/// Loads the map from disk
		/// </summary>
		private void LoadMap()
		{
			debug.log("Map loading not yet implemented, tell rater to get off his ass and add it in! Class: symmnet.regional.rater193.Map.MapManager");
			if (writeData)
			{
				debug.log("Loading map, " + mapName + "!");
			}
			else
			{
				debug.log("Map tried to load that wasnt created with a mapName!");
			}
		}

		internal void SendChunkToClient(Client client, int x, int y, int layer)
		{
			MapLayerChunk c = GetChunk(x, y, layer);

			MapTile t = null;

			if (c != null)
			{
				Debug.Log("Sending chunk, " + x + ", " + y + ", " + layer);
				//Creating the message
				NetOutgoingMessage msg = RegionalServer.server.CreateMessage();

				//Creating the message header
				msg.Write((byte)1); //WORLD
				msg.Write((byte)255); //Rater193's header message group
				msg.Write((byte)Messages.ClientLoadMapChunk); //Load map chunk header id

				//Writing map data
				//Chunk coords
				msg.Write((int)x);//X
				msg.Write((int)y);//Y
				msg.Write((int)layer);//LAYER
				msg.Write((byte)MapManager.Settings.chunkSize);//ChunkSize
				
				//Chunk data
				for (int _x = 0; _x < MapManager.Settings.chunkSize; _x++)
				{
					for (int _y = 0; _y < MapManager.Settings.chunkSize; _y++)
					{
						t = c.GetTile(_x, _y);
						if (layer >= 0)
						{
							if (t == null)
							{
								msg.Write((int)0);
							}
							else
							{
								msg.Write((int)t.tileID);
							}
						}
						else
						{
							//Sending the tile entity instead of the tile, to save on bandwidth performance
							if (t!=null)
							{
								if(t.tileEntity!=null)
								{
									Debug.Log("Sending tile entity!");
									Debug.Log("t.tileEntity: " + t.tileEntity);
									Debug.Log("t.tileEntity.renderID: " + t.tileEntity.renderID);
									msg.Write((int)t.tileEntity.renderID);
								}
								else
								{
									Debug.Log("Target tile has no tile entity?");
									msg.Write((int)-1);
								}
							}
							else
							{
								msg.Write((int)-1);
							}
						}
					}
				}

				RegionalServer.server.SendMessage(msg, client.connection, NetDeliveryMethod.ReliableOrdered, 10);
				//Server.server.SendMessage(msg, client.connection, NetDeliveryMethod.Unreliable);
			}
		}

		/// <summary>
		/// Converts x and y to a string to use with the chunk dictionary
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		private string GetChunkKey(int x, int y, int layer)
		{
			return x + "," + y + "," + layer;
		}

		/// <summary>
		/// Saves the map to the disk
		/// </summary>
		public void SaveMap()
		{
			debug.log("Map saving not yet implemented, tell rater to get off his ass and add it in! Class: symmnet.regional.rater193.Map.MapManager");
			if (writeData)
			{
				debug.log("Saving map, " + mapName + ", to disk!");
			}
			else
			{
				debug.log("Map tried to save that wasnt created with a mapName!");
			}
		}

		/// <summary>
		/// Returns if a chunk exists, they get created automatically when setting tiles.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool ChunkExists(int x, int y, int layer)
		{
			return layerchunks.ContainsKey(GetChunkKey(x, y, layer));
		}

		/// <summary>
		/// Get a chunk by chunk coodinates
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public MapLayerChunk GetChunk(int x, int y, int layer)
		{
			MapLayerChunk chunk = null;
			layerchunks.TryGetValue(GetChunkKey(x, y, layer), out chunk);
			return chunk;
		}

		/// <summary>
		/// Creates a chunk at the given chunk coordinates, eg: 1, 2; 2, 2; 2, 1; 10,22; etc.....
		/// </summary>
		/// <param name="posX">Chunk posX</param>
		/// <param name="posY">Chunk PosY</param>
		/// <returns></returns>
		public MapLayerChunk CreateChunk(int x, int y, int layer)
		{
			MapLayerChunk c = GetChunk(x, y, layer);
			if (c == null)
			{
				c = new MapLayerChunk();
				layerchunks.Add(GetChunkKey(x, y, layer), c);
			}
			return c;
		}

		public MapLayerChunk GetOrCreateChunk(int x, int y, int layer)
		{
			bool cExists = ChunkExists(x, y, layer);
			MapLayerChunk c = CreateChunk(x, y, layer);
			return c;
			//return ChunkExists(x, y) ? CreateChunk(x, y) : CreateChunk(x, y);
		}

		public MapTile SetTile(int x, int y, int layer, int tileID)
		{
			//(int)MathF.Floor((float)_x / (float)MapManager.Settings.chunkSize)
			int chunkX = (int)MathF.Floor((float)x / (float)Settings.chunkSize);
			int chunkY = (int)MathF.Floor((float)y / (float)Settings.chunkSize);
			
			MapLayerChunk c = GetOrCreateChunk(chunkX, chunkY, layer);

			//RegionalServer.logList.Add(new object[] { ConsoleColor.White, "1" });
			//RegionalServer.UpdateConsole();
			if (layer < 0) return c.SetTileEntity(x, y, tileID); // Returning the tile entity tile
			//RegionalServer.logList.Add(new object[] { ConsoleColor.White, "2" });
			//RegionalServer.UpdateConsole();
			return c.SetTile(x, y, tileID);//or the tile itself
		}

		internal void DeleteTile(int x, int y, int layer)
		{
			//(int)MathF.Floor((float)_x / (float)MapManager.Settings.chunkSize)
			int chunkX = (int)MathF.Floor((float)x / (float)Settings.chunkSize);
			int chunkY = (int)MathF.Floor((float)y / (float)Settings.chunkSize);

			MapLayerChunk c = GetOrCreateChunk(chunkX, chunkY, layer);

			c.DeleteTile(x, y);
		}
	}
}
