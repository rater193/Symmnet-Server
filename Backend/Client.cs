using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Lidgren.Network;

namespace symmnet.backend
{
    public class Client
    {
		public NetConnection connection;
		public Timer timeoutTimer;

		public bool loggedIn = false;
		public int clientId;
		public string username;
		public int authority;
		public bool mapEditor = false;
		public int level;
		public float xp;
		public string skinColour;
		public string hairColour;
		public string clothingColour;
		public string mainhand;
		public string offhand;
		public byte[] avatar = new byte[1] { 0x1 };

		public Client(NetConnection connection)
		{
			this.connection = connection;

			timeoutTimer = new Timer();
			timeoutTimer.Elapsed += new ElapsedEventHandler(Timeout);
			timeoutTimer.Interval = 10000;
			timeoutTimer.Start();
		}

		private void Timeout(object source, ElapsedEventArgs elapsed)
		{
			BackendServer.RemoveClient(this);
		}
	}
}
