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
        Main = 1
    }
	
    public enum EntityEquipableType
    {
		None = 0,
        Shell = 1,
        Weapon = 2,
        Aura = 3,
        Reactor = 4
    }
	
    public enum EntityBodySocket
    {
		None = 0,
        Shell = 1,
        Weapon = 2,
        Aura = 3,
        Reactor = 4
    }
}