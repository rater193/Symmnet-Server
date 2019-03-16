using Regional.rater193.Entities;
using symmnet.shared.rater193;
using System;
using System.Collections.Generic;
using System.Text;

namespace symmnet.regional.rater193.Map.Chunks
{
    class MapLayerChunk
    {
		public Dictionary<string, MapTile> tiles = new Dictionary<string, MapTile>();
		private string GetTileKey(int x, int y)
		{
			return x + "," + y;
		}

		public MapTile GetTile(int localPosX, int localPosY)
		{
			//The return value
			MapTile ret = null;

			//Keeping the values within the chunk size, JUST incase :P
			localPosX = localPosX % MapManager.Settings.chunkSize;
			localPosY = localPosY % MapManager.Settings.chunkSize;

			if (localPosX < 0)
			{
				localPosX += MapManager.Settings.chunkSize;
				localPosX = (int)MathF.Abs(-localPosX);
			}
			if (localPosY < 0)
			{
				localPosY += MapManager.Settings.chunkSize;
				localPosY = (int)MathF.Abs(-localPosY);
			}

			//Trying to get the map tile
			string tileKey = GetTileKey(localPosX, localPosY);
			tiles.TryGetValue(tileKey, out ret);

			return ret;
		}

		public MapTile SetTile(int localPosX, int localPosY, int tileID)
		{

			//Keeping the values within the chunk size, JUST incase :P
			localPosX = localPosX % MapManager.Settings.chunkSize;
			localPosY = localPosY % MapManager.Settings.chunkSize;

			if (localPosX < 0)
			{
				localPosX += MapManager.Settings.chunkSize;
				localPosX = (int)MathF.Abs (-localPosX);
			}
			if (localPosY < 0)
			{
				localPosY += MapManager.Settings.chunkSize;
				localPosY = (int)MathF.Abs(-localPosY);
			}
			

			MapTile t = null;

			//Trying to set the map tile
			string tileKey = GetTileKey(localPosX, localPosY);
			if (!tiles.ContainsKey(tileKey))
			{
				t = new MapTile(tileID);
				tiles.Add(tileKey, t);
			}
			else
			{
				tiles.TryGetValue(tileKey, out t);
				t.tileID = tileID;
				
			}
			return GetTile(localPosX, localPosY);
		}

		internal void DeleteTile(int localPosX, int localPosY)
		{
			localPosX = localPosX % MapManager.Settings.chunkSize;
			localPosY = localPosY % MapManager.Settings.chunkSize;

			if (localPosX < 0)
			{
				localPosX += MapManager.Settings.chunkSize;
				localPosX = (int)MathF.Abs(-localPosX);
			}
			if (localPosY < 0)
			{
				localPosY += MapManager.Settings.chunkSize;
				localPosY = (int)MathF.Abs(-localPosY);
			}

			string tileKey = GetTileKey(localPosX, localPosY);
			tiles.Remove(tileKey);
		}

		internal MapTile SetTileEntity(int localPosX, int localPosY, int tileEntityID)
		{

			//Keeping the values within the chunk size, JUST incase :P
			localPosX = localPosX % MapManager.Settings.chunkSize;
			localPosY = localPosY % MapManager.Settings.chunkSize;

			if (localPosX < 0)
			{
				localPosX += MapManager.Settings.chunkSize;
				localPosX = (int)MathF.Abs(-localPosX);
			}
			if (localPosY < 0)
			{
				localPosY += MapManager.Settings.chunkSize;
				localPosY = (int)MathF.Abs(-localPosY);
			}


			MapTile t = null;

			//Trying to set the map tile
			string tileKey = GetTileKey(localPosX, localPosY);
			if (!tiles.ContainsKey(tileKey))
			{
				t = new MapTile(-1);
				tiles.Add(tileKey, t);
			}
			else
			{
				tiles.TryGetValue(tileKey, out t);
				t.tileID = -1;

			}
			//t.tileEntity = TileEntityBase.GetTileEntity(tileEntityID);
			t.tileEntity = TileEntityBase.GetTileEntity(tileEntityID);
			//RegionalServer.logList.Add(new object[] { ConsoleColor.White, "Setting tile entity?" });
			//RegionalServer.UpdateConsole();
			return GetTile(localPosX, localPosY);
		}
	}
}
