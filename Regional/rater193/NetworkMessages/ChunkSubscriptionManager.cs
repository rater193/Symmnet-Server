using symmnet.regional.rater193.Map.Chunks;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace symmnet.regional.rater193.NetworkMessages
{
    class ChunkSubscriptionManager
	{
		public List<Vector2> chunkCoords = new List<Vector2>();

		public void Subscribe(Vector2 chunk)
		{
			chunkCoords.Add(chunk);
		}

		public void Unsubscribe(Vector2 chunk)
		{
			chunkCoords.Remove(chunk);
		}
	}
}
