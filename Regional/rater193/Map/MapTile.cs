using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Regional.rater193.Entities;

namespace symmnet.regional.rater193.Map
{
    class MapTile
    {
		public int tileID = 0;
		public TileEntityBase tileEntity;
		
		public MapTile(int newMapTileID = 0)
		{
			tileID = newMapTileID;
		}
	}
}
