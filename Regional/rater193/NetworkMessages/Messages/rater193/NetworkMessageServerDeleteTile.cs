﻿using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using symmnet.regional;
using symmnet.regional.rater193;

namespace symmnet.regional.rater193.NetworkMessages.Messages.rater193
{
    class NetworkMessageServerDeleteTile : NetworkMessage
	{
		public override void execute(Client client, NetIncomingMessage data)
		{

			int
				_size = data.ReadInt32(),
				_x = data.ReadInt32(),
				_y = data.ReadInt32(),
				_layer = data.ReadInt32();


			for (int __x = -_size; __x <= _size; __x++)
			{
				for (int __y = -_size; __y <= _size; __y++)
				{
					Rater193.StaticRooms.world.DeleteTile(_x + __x, _y + __y, _layer);
				}
			}

			foreach (Client c in RegionalServer.clientList)
			{
				if (c != null && c.connection != null)
				{
					NetOutgoingMessage msg = RegionalServer.server.CreateMessage();
					msg.Write((byte)1); //WORLD
					msg.Write((byte)255); //Rater193's header message group
					msg.Write((byte)symmnet.regional.rater193.Messages.ClientDeleteTile);

					msg.Write((int)_size);
					msg.Write((int)_x);
					msg.Write((int)_y);
					msg.Write((int)_layer);

					RegionalServer.server.SendMessage(msg, c.connection, NetDeliveryMethod.ReliableOrdered, 16);
				}
				//StaticRooms.hub.SendChunkToClient(c, (int)MathF.Floor((float)_x / (float)MapManager.Settings.chunkSize), (int)MathF.Floor((float)_y / (float)MapManager.Settings.chunkSize));
			}
		}
	}
}
