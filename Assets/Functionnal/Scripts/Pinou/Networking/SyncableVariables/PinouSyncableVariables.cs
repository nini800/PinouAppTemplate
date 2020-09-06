#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pinou.Networking
{
	public enum SyncableVariable : long
	{
		Position = 1,
		Rotation = 2,
		Velocity = 4,
		MovementsParticlesEmission = 8,
		MovementsParticlesAngle = 16,
		EntityAbilityCast = 32,
		EntityAbilityHit = 64,
		EntityAbilityHitboxDestroyed = 128,
		EntityDeath = 256,
		EntityHealth = 512,
	}

	public static class PinouSyncableVariablesExtender
	{
		private static Array s_SyncableVariableValues = Enum.GetValues(typeof(SyncableVariable));
		
		public static int GetIndex(this SyncableVariable var)
		{
			for (int i = 0; i < s_SyncableVariableValues.Length; i++)
			{
				if (var == (SyncableVariable)s_SyncableVariableValues.GetValue(i))
				{
					return i;
				}
			}

			return -1;
		}
		
		public static int GetChannel(this SyncableVariable var)
		{
			switch (var)
			{
				case SyncableVariable.Position:
					return 2;
				case SyncableVariable.Rotation:
					return 2;
				case SyncableVariable.Velocity:
					return 1;
				case SyncableVariable.MovementsParticlesEmission:
					return 2;
				case SyncableVariable.MovementsParticlesAngle:
					return 2;
				case SyncableVariable.EntityAbilityCast:
					return 1;
				case SyncableVariable.EntityAbilityHit:
					return 1;
				case SyncableVariable.EntityAbilityHitboxDestroyed:
					return 1;
				case SyncableVariable.EntityDeath:
					return 1;
				case SyncableVariable.EntityHealth:
					return 1;
			}

			return 0;
		}
	}
}