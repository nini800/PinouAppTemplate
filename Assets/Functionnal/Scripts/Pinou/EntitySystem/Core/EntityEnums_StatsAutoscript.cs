namespace Pinou.EntitySystem
{		
    [System.Flags]
    public enum EntityStatsStat
    {
        None = 0,
        GlobalExperienceFactor = 1,

		ExperienceReceivedFactor = 2,
		PowerExperienceReceivedFactor = 4
    }
	
	[System.Flags]
    public enum EntityStatsLevelType
    {
        Main = 1,
        Power = 2
    }
}