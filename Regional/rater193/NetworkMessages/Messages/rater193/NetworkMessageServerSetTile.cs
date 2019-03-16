using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;
using Regional.rater193.Entities;
using symmnet.regional;
using symmnet.regional.rater193;
using symmnet.regional.rater193.Map;

namespace symmnet.regional.rater193.NetworkMessages.Messages.rater193
{
    class NetworkMessageServerSetTile : NetworkMessage
    {
		public override void execute(Client client, NetIncomingMessage data)
		{

			int
				_size = data.ReadInt32(),
				_x = data.ReadInt32(),
				_y = data.ReadInt32(),
				_layer = data.ReadInt32(),
				_id = data.ReadInt32();
			if (_layer < 0)
			{
				//_teID = data.ReadInt32();
			}

			//Setting the tile on the server
			for (int __x = -_size; __x <= _size; __x++)
			{
				for (int __y = -_size; __y <= _size; __y++)
				{
					MapTile t = Rater193.StaticRooms.world.SetTile(__x + _x, __y + _y, _layer, _id);
					///t.tileEntity = TileEntityBase.GetTileEntity(0);
				}
			}

			//Sending the message created tile to the connected clients
			foreach (Client c in RegionalServer.clientList)
			{
				if (c != null && c.connection != null)
				{
					NetOutgoingMessage msg = RegionalServer.server.CreateMessage();
					msg.Write((byte)1); //WORLD
					msg.Write((byte)255); //Rater193's header message group
					msg.Write((byte)symmnet.regional.rater193.Messages.ClientReceiveTile);

					msg.Write((int)_size);
					msg.Write((int)_x);
					msg.Write((int)_y);
					msg.Write((int)_layer);
					msg.Write((int)_layer >= 0 ? _id : TileEntityBase.GetTileEntity(_id).renderID);//Replace the 100 with the tile entity id!
					if (_layer < 0)
					{
						//RegionalServer.logList.Add( new object[] {ConsoleColor.White, "id: " + _id });
						//RegionalServer.UpdateConsole();
					}

					RegionalServer.server.SendMessage(msg, c.connection, NetDeliveryMethod.ReliableOrdered, 16);
				}

				//StaticRooms.hub.SendChunkToClient(c, (int)MathF.Floor((float)_x / (float)MapManager.Settings.chunkSize), (int)MathF.Floor((float)_y / (float)MapManager.Settings.chunkSize));
			}
			//TODO: Send the specific tile update to all other clients inside the room. (NOT THE WHOLE CHUNK!)

		}
	}
}
