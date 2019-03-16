using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Lidgren.Network;
using symmnet.regional;
using symmnet.regional.rater193;
using symmnet.regional.rater193.NetworkMessages.Player;

namespace symmnet.regional.rater193.NetworkMessages.Messages.rater193
{
    class NetworkMessageServerGetChunk : NetworkMessage
    {
		public override void execute(Client client, NetIncomingMessage data)
		{
			int chunkX = data.ReadInt32();
			int chunkY = data.ReadInt32();


			NetworkPlayer.GetPlayer(client).OnLoadChunk(new Vector2(chunkX, chunkY));
		}
	}
}
