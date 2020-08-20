namespace Pinou.EntitySystem
{
    [System.Flags]
    public enum EntityBeingStat
    {
        None = 0,	

        MaxHealth = 1,
        HealthRegen = 2,
        HealthReceivedFactor = 4,
        HealthDoneFactor = 8,

        MaxPower = 16,
        PowerRegen = 32,
        PowerReceivedFactor = 64,
        PowerDoneFactor = 128,

        MaxStamina = 256,
        StaminaRegen = 512,
        StaminaReceivedFactor = 1024,
        StaminaDoneFactor = 2048,

        MaxMana = 4096,
        ManaRegen = 8192,
        ManaReceivedFactor = 16384,
        ManaDoneFactor = 32768
    }
	
	[System.Flags]
    public enum EntityBeingResourceType
    {
        Health = 1,
        Power = 2,
        Stamina = 4,
        Mana = 8
    }
}