using Lidgren.Network;
using symmnet.regional;
using System;
using System.Collections.Generic;
using System.Text;

namespace symmnet.regional.rater193.NetworkMessages
{
    class NetworkMessageController
    {
		private static Dictionary<int, NetworkMessage> networkMessages = new Dictionary<int, NetworkMessage>();

		public void register(int messageID, NetworkMessage msg)
		{
			networkMessages.Add(messageID, msg);
		}

		public NetworkMessage GetNetworkMessage(int msgID)
		{
			return networkMessages.GetValueOrDefault(msgID, null);
		}

		public void execute(int messageID, Client client, NetIncomingMessage data)
		{
			NetworkMessage msg = GetNetworkMessage(messageID);
			msg.execute(client, data);
		}
    }
}
