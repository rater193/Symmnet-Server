using System;
using System.Collections.Generic;
using System.Text;

namespace symmnet.regional.rater193
{
    class Messages
	{
		//Server to client
		public const byte
			ClientLoadMapChunk = 0,
			ClientSetTile = 1,
			ClientReceiveTile = 2,
			ClientDeleteTile = 5,
			ClientDeleteColumn = 6,
			ClientReceiveTileEntityList = 10,
			ClientUnloadChunk = 11
		;

		//Client to server
		public const byte
			ServerGetChunk = 0,
			ServerSetTile = 1,
			ServerDeleteTile = 5,
			ServerDeleteColumn = 6
		;
	}
}
