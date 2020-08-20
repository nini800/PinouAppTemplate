#pragma warning disable 0649
using UnityEngine;
using System.Collections.Generic;

namespace Pinou.EntitySystem
{
	[CreateAssetMenu(fileName = "CheatsData", menuName = "Pinou/Managers/CheatsData", order = 1000)]
	public class PinouCheatsManagerData : PinouManagerData
	{
		[Header("Cheats")]
		[Space]
		[SerializeField] private PinouCheat[] _cheats;

		public override PinouManager BuildManagerInstance()
		{
			return new PinouCheatsManager(this);
		}


		public class PinouCheatsManager : PinouManager
		{
			public PinouCheatsManager(PinouCheatsManagerData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouCheatsManagerData Data { get; private set; }

            public override void SlaveUpdate()
            {
                for (int i = 0; i < Data._cheats.Length; i++)
                {
					Data._cheats[i].CheckCheat();
                }
            }
        }
	}
}