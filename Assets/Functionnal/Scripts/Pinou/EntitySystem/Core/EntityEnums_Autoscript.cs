namespace Pinou.EntitySystem
{
    [System.Flags]
    public enum EntityBeingStats
    {
        None = 0,	

        MaxHealth = 1,
        HealthRegen = 2,
        HealthReceivedFactor = 4,

        MaxPower = 8,
        PowerRegen = 16,
        PowerReceivedFactor = 32,

        MaxStamina = 64,
        StaminaRegen = 128,
        StaminaReceivedFactor = 256,

        MaxMana = 512,
        ManaRegen = 1024,
        ManaReceivedFactor = 2048
    }
	
	[System.Flags]
    public enum EntityBeingResourcesType
    {
        Health = 1,
        Power = 2,
        Stamina = 4,
        Mana = 8
    }
}