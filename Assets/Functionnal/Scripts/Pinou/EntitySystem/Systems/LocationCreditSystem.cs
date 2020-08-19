#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public class LocationCreditSystem : PinouBehaviour, ICreditFactorSystem
	{
		[Header("Paramters")]
		[Space]
		[SerializeField] private PinouUtils.Maths.Formula _creditFactorPerMeterFormula;

		public float CreditFactor(Entity player) => _creditFactorPerMeterFormula.Evaluate(player.Position.magnitude);
	}
}