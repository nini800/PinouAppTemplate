using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public enum AbilityType
	{
		Instant,
		Projectile,
		Melee
	}

	public enum DirectionMethod
    {
		AimBased,
		CharacterForward
    }
	public enum RecoilCondition
	{
		OnCast,
		MustHit
	}
	public enum KnockbackMethod
	{
		AttackBased,
		AttackEntityAverage,
		VictimEntityBased,
		AttackImpactToEntity,
	}
	public enum AbilityCastOrigin
    {
		Caster,
		AimAbsolute
    }
	public enum AbilityCastOriginTiming
	{
		CastEntrance,
		PerformEntrance
	}
	public enum AbilityImpactMethod
    {
		VictimCenter,
		CastOrigin,
		OriginVictimCenterAverage,
		RayCastTowardVictim
    }

	[System.Flags]
	public enum AbilityTargets
    {
		Self = 1,
		Allies = 2,
		Enemies = 4
    }

	public enum AbilityCastResultType
    {
		Succeeded,
		Failed,
		Missed,
		Dodged,
		Parried,
		Blocked,
    }

	public enum HitboxType
    {
		Sphere,
		Box
    }
}