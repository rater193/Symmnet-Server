using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Timers;
using symmnet.regional.rater193.NetworkMessages.Player;

namespace symmnet.regional
{
	//test
	public class Client
	{
		public NetConnection connection;
		public Timer timeoutTimer;

		public int clientId;
		public string username;
		public bool mapEditor = false;
		public int authority;
		public int level;
		public float xp;
		public string skinColour;
		public string hairColour;
		public string clothingColour;
		public string mainhand;
		public string offhand;
		public byte[] avatar = new byte[1] { 0x1 };

		public List<float[]> posHistory = new List<float[]> { new float[2] { 0, 0 }, new float[2] { 0, 0 } };
		public float playerSize = 16;
		public float playerStepMargin = 0.1f;
		public float playerPosX;
		public float playerPosY;
		internal NetworkPlayer playerEntity;

		public Client(int clientId)
		{
			this.clientId = clientId;

			timeoutTimer = new Timer();
			timeoutTimer.Elapsed += new ElapsedEventHandler(Timeout);
			timeoutTimer.Interval = 10000;
			timeoutTimer.Start();
		}

		private void Timeout(object source, ElapsedEventArgs elapsed)
		{
			RegionalServer.RemoveClient(this);
		}
	}
}
