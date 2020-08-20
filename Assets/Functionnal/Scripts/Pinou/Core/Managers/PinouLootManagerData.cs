#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou
{
	[CreateAssetMenu(fileName = "LootData", menuName = "Pinou/Managers/LootData", order = 1000)]
	public class PinouLootManagerData : PinouManagerData
	{
		public override PinouManager BuildManagerInstance()
		{
			return new PinouLootManager(this);
		}
		public class PinouLootManager : PinouManager
		{
			public PinouLootManager(PinouLootManagerData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouLootManagerData Data { get; private set; }



		}
	}
}