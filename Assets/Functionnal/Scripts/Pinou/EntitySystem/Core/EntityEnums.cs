using System;

namespace Pinou.EntitySystem
{
    public enum EntityTeam
    {
        Players,
        Allies,
        Enemies
    }

    [Flags]
    public enum ControllerState
    {
        None = 0,
        Empty = 1,
        Controlled = 2,
        Overriden = 4
    }
    public enum EnemyNoTargetBehaviour
    {
        StayStill,
        MoveRandomly,
        MoveRandomlyAroundSpawn,
        MoveForward
    }
    public enum EnemyHasTargetBehaviour
    {
        BrainlessRush,
        RotateAround
    }

    [Flags]
    public enum BeingState
    {
        None = 0,
        Alive = 1,
        Stunned = 2,
        Dead = 4
    }
    [Flags]
    public enum AbilityState
    {
        None = 0,
        Casting = 1,
        Performing = 2,
        HardRecovering = 4,
        SoftRecovering = 8,
    }
    [Flags]
    public enum InteractionState
	{
        None = 0,
        Interacting = 1,
	}
    [Flags]
    public enum MovementState
    {
        None = 0,
        Idle = 1,
        Moving = 2,
        Sprinting = 4,
        Dashing = 8,
        InAir = 16,
        Overriden = 32
    }
    [Flags]
    public enum MovementDirection
    {
        None = 0,
        Forward = 1,
        Right = 2,
        Left = 4,
        Backward = 8,
        Upward = 16,
        Downward = 32
    }

    [Flags]
    public enum EntityStatsStat
    {
        None = 0,
        ExperienceGainedGlobalFactor = 1,
    }
    [Flags]
    public enum EntityStatsLevelStat
    {
        None = 0,
        ExperienceGainedFactor = 1,
    }

    [Flags]
    public enum EntityBeingStat
    {
        None = 0,
    }
    [Flags]
    public enum EntityBeingResourceStat
    {
        None = 0,
        MaxResource = 1,
        ResourceRegen_Global = 2,
        ResourceRegen_OutsideCombat = 4,
        ResourceRegen_InsideCombat = 8,
        ResourceReceivedFactor = 16,
    }
    [Flags]
    public enum EntityAbilityResourceStat
	{
        None = 0,
        GlobalFactor = 1,
        NegativeFactor = 2,
        PositiveFactor = 4
	}

    [Flags]
    public enum EntityMovementsStat
    {
        None = 0,
        Acceleration = 1,
        MaxSpeed = 2,
        BrakeForce = 4,
        KnockbackTakenFactor = 8,
        RecoilFactor = 16,
        DashCharges = 32,
        DashCooldown = 64,
    }
    [Flags]
    public enum EntityAbilitiesStat
    {
        None = 0,
        AttackSpeed = 1,
        KnockbackInflictedFactor = 2,
    }
    [Flags]
    public enum EntityVisualStat
    {
        None = 0,
        Visual_RotationSpeed = 1,
    }

    public enum AbilityTriggerMethod
	{
        None = 0,
        Single = 1,
        Burst = 2,
        Automatic = 3
	}

    public enum EntityLinkedInfoType
	{
        CurrentResource,
        ResourceProgress,
        MaxResource,
        LevelProgress,
        Level,
        Experience,
        DashCooldownProgress
	}
}