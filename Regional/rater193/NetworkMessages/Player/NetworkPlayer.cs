using Lidgren.Network;
using symmnet.regional.rater193.Map;
using symmnet.regional.rater193.Map.Chunks;
using symmnet.shared.rater193;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace symmnet.regional.rater193.NetworkMessages.Player
{
    class NetworkPlayer
    {
		private static Dictionary<Client, NetworkPlayer> players = new Dictionary<Client, NetworkPlayer>();

		private ChunkSubscriptionManager subscribedChunks = new ChunkSubscriptionManager();

		public MapManager currentWorld;
		public Client client;
		public int posX = 0;
		public int posY = 0;

		public void OnSpawn() { }
		public void OnLogIn() { }
		public void OnLogOut() { }
		public void OnUpdate() {

			#region Setting tile coordinates
			posX = (int)MathF.Floor((client.playerPosX * 100f) / 48f);
			posY = (int)MathF.Floor((client.playerPosY * 100f) / 48f);
			#endregion

			#region Chunk unscribtion
			Vector2 playerPos = new Vector2(posX, posY) / 16f;
			for(int index = 0; index < subscribedChunks.chunkCoords.Count; index++)
			{
				Vector2 chunkCoords = subscribedChunks.chunkCoords[index];
				float dist = Vector2.Distance(playerPos, chunkCoords);
				if(dist>=4f)
				{
					subscribedChunks.Unsubscribe(chunkCoords);

					NetOutgoingMessage msg = RegionalServer.server.CreateMessage();
					msg.Write((byte)1); //WORLD
					msg.Write((byte)255); //Rater193's header message group
					msg.Write((byte)symmnet.regional.rater193.Messages.ClientUnloadChunk);

					msg.Write((int)chunkCoords.X);
					msg.Write((int)chunkCoords.Y);
					client.connection.SendMessage(msg, NetDeliveryMethod.ReliableOrdered, 12);
				}

			}
			#endregion
		}

		public void OnLoadChunk(Vector2 chunk)
		{
			#region Subscribing to chunks
			subscribedChunks.Subscribe(chunk);
			int chunkX = ((int)chunk.X);
			int chunkY = ((int)chunk.Y);
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, -1);//TILE ENTITIY LAYER
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, 0);
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, 1);
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, 2);
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, 3);
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, 4);
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, 5);
			Rater193.StaticRooms.world.SendChunkToClient(client, chunkX, chunkY, 6);
			#endregion
		}
		public void OnUnloadchunk(Vector2 chunk)
		{
			subscribedChunks.Unsubscribe(chunk);
		}

		#region static methods
		public static void OnClientConnect(Client client) {

			//Creating the new player
			NetworkPlayer newPlayer = new NetworkPlayer();
			newPlayer.currentWorld = Rater193.StaticRooms.world;
			newPlayer.client = client;
			newPlayer.OnLogIn();
			newPlayer.OnSpawn();

			client.playerEntity = newPlayer;

			players.Add(client, newPlayer);

		}
		public static void OnClientDisconnect(Client client) {
			//Removing the player
			GetPlayer(client).OnLogOut();
			players.Remove(client);
		}
		public static NetworkPlayer GetPlayer(Client client)
		{
			NetworkPlayer ret = null;
			players.TryGetValue(client, out ret);
			return ret;
		}
		public static void Update() {
			foreach(var key in players)
			{
				key.Value.OnUpdate();
			}
		}
		#endregion
	}
}
