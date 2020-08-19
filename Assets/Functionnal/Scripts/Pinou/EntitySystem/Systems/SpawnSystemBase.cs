#pragma warning disable 0649, 0414
using Pinou.EntitySystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
	public class SpawnSystemBase : PinouBehaviour
	{
		[Header("References")]
		[Space]
		[SerializeField, ValidateInput("ValidateCreditFactor", "Every behaviour must be a ICreditFactorSystem")] protected PinouBehaviour[] creditFactorSystems;

		private bool ValidateCreditFactor(PinouBehaviour[] behaviours)
		{
			for (int i = 0; i < behaviours.Length; i++)
			{
				if (behaviours[i] as ICreditFactorSystem == null)
				{
					return false;
				}
			}

			return true;
		}

		public float TotalCreditFactor(Entity player)
		{
			float fac = 1f;
			for (int i = 0; i < creditFactorSystems.Length; i++)
			{
				fac *= ((ICreditFactorSystem)creditFactorSystems[i]).CreditFactor(player);
			}
			return fac;
		}
	}
}