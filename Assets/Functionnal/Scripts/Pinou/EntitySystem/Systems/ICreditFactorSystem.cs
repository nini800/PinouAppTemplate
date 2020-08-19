#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public interface ICreditFactorSystem
	{
		float CreditFactor(Entity player);
	}
}