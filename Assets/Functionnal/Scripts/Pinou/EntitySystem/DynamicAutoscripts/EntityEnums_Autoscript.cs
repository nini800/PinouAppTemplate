using System;

namespace Pinou.EntitySystem
{		
	[Flags]
    public enum EntityBeingResourceType
    {
        Health = 1,
        Power = 2,
        Stamina = 4,
        Mana = 8
    }
	
	[Flags]
    public enum EntityStatsLevelType
    {
        Main = 1,
        Power = 2
    }
	
	[Flags]
    public enum EntityEquipableType
    {
        Chest = 0,
        Pants = 1,
        Boots = 2,
        Gloves = 3
    }
	
	[Flags]
    public enum EntityBodySocket
    {
        Body = 0,
        Weapon = 1,
        Aura = 2
    }
}