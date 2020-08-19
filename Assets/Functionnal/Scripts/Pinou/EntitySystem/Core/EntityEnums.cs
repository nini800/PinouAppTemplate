namespace Pinou.EntitySystem
{
    public enum EntityTeam
    {
        Players,
        Allies,
        Enemies
    }

    [System.Flags]
    public enum ControllerState
    {
        None = 0,
        Empty = 1,
        Controlled = 2,
        Overriden = 4
    }
    public enum EnemyBaseBehaviour
    {
        BrainlessRush,
        RotateAround
    }

    [System.Flags]
    public enum BeingState
    {
        None = 0,
        Alive = 1,
        Stunned = 2,
        Dead = 4
    }
    [System.Flags]
    public enum AbilityState
    {
        None = 0,
        Casting = 1,
        Performing = 2,
        HardRecovering = 4,
        SoftRecovering = 8,
    }
    [System.Flags]
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
    [System.Flags]
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

    [System.Flags]
    public enum EntityMovementsStats
    {
        None = 0,
        Acceleration = 1,
        MaxSpeed = 2,
        BrakeForce = 4,
        KnockbackTakenFactor = 8,
        RecoilFactor = 16,
    }
    [System.Flags]
    public enum EntityAbilitiesStats
    {
        None = 0,
        Damages = 1,
        Power = 2,
        AttackSpeed = 4,
        KnockbackInflictedFactor = 8,
    }
    [System.Flags]
    public enum EntityVisualStats
    {
        None = 0,
        Visual_RotationSpeed = 1,
    }

}