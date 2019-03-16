using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Lidgren.Network;

namespace symmnet.backend
{
	public class Server
	{
		public NetConnection connection;
		public Timer timeoutTimer;
		public int serverId;

		public Server(NetConnection connection)
		{
			this.connection = connection;

			timeoutTimer = new Timer();
			timeoutTimer.Elapsed += new ElapsedEventHandler(Timeout);
			timeoutTimer.Interval = 10000;
			timeoutTimer.Start();
		}

		private void Timeout(object source, ElapsedEventArgs elapsed)
		{
			BackendServer.RemoveServer(this);
		}
	}
}
