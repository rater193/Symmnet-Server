using Lidgren.Network;
using symmnet.regional;
using System;
using System.Collections.Generic;
using System.Text;

namespace Regional.rater193.Entities
{
    class TileEntityBase
	{
		#region local instance variables/methods
		public int
			renderID = 100,
			entityID = -1
			;

		public TileEntityBase(int tileEntityID)
		{
			if (!AddEntity(this, tileEntityID)) return;
			entityID = tileEntityID;
			//Continue code here
		}

		public virtual void OnInteract(Client player) { }
		public virtual void OnHit(LivingEntityBase triggeringEntity, int damage) { }
		public virtual void OnDelete(int x, int y) { }
		public void AddToStream(NetOutgoingMessage msg, int x, int y)
		{
			msg.Write((int)x);
			msg.Write((int)y);
			msg.Write((int)renderID);
		}
		#endregion

		#region static variables/methods
		public static Dictionary<int, TileEntityBase> registeredEntities = new Dictionary<int, TileEntityBase>();
		private static Dictionary<string, TileEntityBase> tileEntityGrid = new Dictionary<string, TileEntityBase>();


		private static bool AddEntity(TileEntityBase tileEntity, int entityID)
		{
			if (registeredEntities.ContainsKey(entityID)) return false;
			registeredEntities.Add(entityID, tileEntity);
			return true;
		}
		public static void SetTileEntity(int x, int y, int tileEntityID)
		{
			//exit setting code if the tile entity or tile entity id was not specified
			if (tileEntityID == -1) return;
			SetTileEntity(x, y, GetTileEntity(tileEntityID));

		}
		public static TileEntityBase GetTileEntity(int tileEntityID)
		{
			TileEntityBase ret;
			registeredEntities.TryGetValue(tileEntityID, out ret);
			return ret;
		}
		private static string GetTileEntityID(int x, int y)
		{
			return x + "," + y;
		}
		public static TileEntityBase GetTileEntity(int x, int y)
		{
			TileEntityBase ret;
			tileEntityGrid.TryGetValue(GetTileEntityID(x, y), out ret);
			return ret;
		}
		public static void SetTileEntity(int x, int y, TileEntityBase tileEntity)
		{
			//exit setting code if the tile entity or tile entity id was not specified
			if (tileEntity == null) return;
			string tileEntityGridID = GetTileEntityID(x, y);
			DeleteTile(x, y);

			SetTileEntity(x, y, tileEntity.entityID);
		}
		public static void DeleteTile(int x, int y)
		{
			if (tileEntityGrid.ContainsKey(GetTileEntityID(x, y)))
			{
				GetTileEntity(x, y).OnDelete(x, y);
				tileEntityGrid.Remove(GetTileEntityID(x, y));
			}
		}
		public static bool TileEntityExists(int x, int y)
		{
			return tileEntityGrid.ContainsKey(GetTileEntityID(x, y));
		}
		public static bool TileEntityExists(int entityID)
		{
			return registeredEntities.ContainsKey(entityID);
		}

		#endregion
	}
}
