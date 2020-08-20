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

		[SerializeField] protected bool hasPowerLevel;
		[ShowIf("@hasPowerLevel"), SerializeField] protected LevelData powerLevel;
		public LevelData PowerLevel => powerLevel;
		public bool HasPowerLevel => hasPowerLevel;


		public bool GetHasLevelData(EntityStatsLevelType levelType)
		{
			switch(levelType)
			{
				case EntityStatsLevelType.Main:
					return hasMainLevel;
				case EntityStatsLevelType.Power:
					return hasPowerLevel;
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
				case EntityStatsLevelType.Power:
					return powerLevel;	
			}
			
			throw new System.Exception("No " + levelType + " level found.");
		}

        public partial class EntityStats : EntityComponent
        {
			//Main
			protected LevelExperienceData mainExperience;
			public LevelExperienceData MainExperience => mainExperience;
			public bool HasMainLevel => _data.HasMainLevel;


			//Power
			protected LevelExperienceData powerExperience;
			public LevelExperienceData PowerExperience => powerExperience;
			public bool HasPowerLevel => _data.HasPowerLevel;

			
			//Generics
			public LevelExperienceData GetLevelExperienceData(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return null; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience;
					case EntityStatsLevelType.Power:
						return powerExperience;
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
					case EntityStatsLevelType.Power:
						powerExperience = experienceData;
						break;
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
			public int GetCurrentLevel(EntityStatsLevelType levelType)
			{
				if (_data.GetHasLevelData(levelType) == false) { return 0; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						return mainExperience.Level;
					case EntityStatsLevelType.Power:
						return powerExperience.Level;
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
					case EntityStatsLevelType.Power:
						powerExperience.SetLevel(value);
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
					case EntityStatsLevelType.Power:
						powerExperience.ModifyLevel(amount);
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
					case EntityStatsLevelType.Power:
						return powerExperience.MaxLevel;
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
					case EntityStatsLevelType.Power:
						return powerExperience.ExperienceProgress;
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
					case EntityStatsLevelType.Power:
						return powerExperience.Experience;	
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

					case EntityStatsLevelType.Power:
						powerExperience.SetExperience(experience);
						break;
	
				}
			}
			public void ModifyLevelExperience(EntityStatsLevelType levelType, float experience)
			{
				if (_data.GetHasLevelData(levelType) == false) { return; }
				
				switch(levelType)
				{
					case EntityStatsLevelType.Main:
						mainExperience.ModifyExperience(experience);
						break;

					case EntityStatsLevelType.Power:
						powerExperience.ModifyExperience(experience);
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

					case EntityStatsLevelType.Power:
						powerExperience.SetExperiencePct(experience);
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

					case EntityStatsLevelType.Power:
						powerExperience.ModifyExperiencePct(experience);
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

					case EntityStatsLevelType.Power:
						return powerExperience.TotalExperienceForNextLevel;
	
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

					case EntityStatsLevelType.Power:
						return powerExperience.RemainingExperienceForNextLevel;
	
				}
				
				throw new System.Exception("No " + levelType + " level found.");
			}
        }
    }
}