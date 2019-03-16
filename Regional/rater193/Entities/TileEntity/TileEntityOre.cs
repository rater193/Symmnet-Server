using symmnet.regional;
using System;
using System.Collections.Generic;
using System.Text;

namespace Regional.rater193.Entities.TileEntity
{
    class TileEntityOre : TileEntityBase
	{
		public TileEntityOre(int tileEntityID) : base(tileEntityID)
		{

			renderID = 101 ;

		}

		public override void OnHit(LivingEntityBase triggeringEntity, int damage)
		{
			base.OnHit(triggeringEntity, damage);
		}

		public override void OnInteract(Client player)
		{
			base.OnInteract(player);
		}
    }
}
