using symmnet.regional.rater193.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace symmnet.regional.rater193
{
    class Tiles
    {
		public static MapTile
			//empty map tile
			empty		= null,

			//paths
			pathGrass	= new MapTile(),
			pathStone	= new MapTile(),
			pathDirt	= new MapTile(),
			pathCave	= new MapTile(),

			//walls
			wallStone	= new MapTile(),
			wallDirt	= new MapTile(),
			wallCave	= new MapTile()
		;
	}
}
