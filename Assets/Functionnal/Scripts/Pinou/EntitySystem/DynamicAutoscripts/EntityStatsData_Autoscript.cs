using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem	
{
	public partial class EntityStatsData : EntityComponentData
	{
		[Header("Stats Levels")]
		[SerializeField] protected bool hasMainLevel;
		[ShowIf("@hasMainLevel"), SerializeField] protected LevelData mainLevel;
		public LevelData MainLevel => mainLevel;
		public bool HasMainLevel => hasMainLevel;


		public bool GetHasLevelData(EntityStatsLevelType levelType)
		{
			switch(levelType)
			{
				case EntityStatsLevelType.Main:
					return hasMainLevel;
			}
			
			throw new System.Exception("No " + levelType + " level found.");
		}
		public LevelData GetLevelData(EntityStatsLevelType levelType)
		{
			if (GetHasLevelData(levelType) == false) { return null; }
			
			switch(levelType)
			{
				case EntityStatsLevelType.Main:
					return mainLevel;	
			}
			
			throw new System.Exception("No " + levelType + " level found.");
		}

        public partial class EntityStats : EntityComponent
        {
			//Main
			protected LevelExperienceData mainExperience;
			public LevelExperienceData MainExperience => mainExperience;
			public bool HasMainLevel => _data.HasMainLevel;

			
			//Generics
			public LevelExperienceData GetLevelExperienceData(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return null; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience;
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
			public void SetLevelExperienceData(EntityStatsLevelType levelType, LevelExperienceData experienceData)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						mainExperience = experienceData;
						break;
				}
			}
			public int GetCurrentLevel(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return 0; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience.Level;
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
			public void SetCurrentLevel(EntityStatsLevelType levelType, int value)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						mainExperience.SetLevel(value);
						break;
				}
			}
			public void ModifyCurrentLevel(EntityStatsLevelType levelType, int amount)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						mainExperience.ModifyLevel(amount);
						break;
				}
			}
			public int GetMaxLevel(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return 0; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience.MaxLevel;
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
			
			public float GetLevelProgress(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return 0f; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience.ExperienceProgress;
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
			
			public float GetLevelExperience(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return 0f; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience.Experience;	
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
			public void SetLevelExperience(EntityStatsLevelType levelType, float experience)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						mainExperience.SetExperience(experience);
						break;
	
				}
			}
			public void ModifyLevelExperience(EntityStatsLevelType levelType, float experience, bool useStatsInfluence = true)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						if (useStatsInfluence == true)
							MainExperience.ModifyExperience(EvaluateStatsStat(EntityStatsStat.ExperienceGainedGlobalFactor, EvaluateStatsLevelStat(EntityStatsLevelType.Main, EntityStatsLevelStat.ExperienceGainedFactor, experience)));
						else
							mainExperience.ModifyExperience(experience);
						break;	
				}
			}
			public void SetLevelExperiencePct(EntityStatsLevelType levelType, float experience)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						mainExperience.SetExperiencePct(experience);
						break;
	
				}
			}
			public void ModifyLevelExperiencePct(EntityStatsLevelType levelType, float experience)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						mainExperience.ModifyExperiencePct(experience);
						break;
	
				}
			}
			public float GetTotalExperienceForNextLevel(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return 0f; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience.TotalExperienceForNextLevel;
	
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
			public float GetRemainingExperienceForNextLevel(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return 0f; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience.RemainingExperienceForNextLevel;
	
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
        }
    }
}