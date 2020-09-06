using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem	
{
	public partial class EntityBeingData : EntityComponentData
	{
		[FoldoutGroup("Being Resources")]
        [Header("Health")]
        [FoldoutGroup("Being Resources"), SerializeField] protected float maxHealth;
        [FoldoutGroup("Being Resources"), SerializeField] protected float healthRegen;
        [FoldoutGroup("Being Resources"), SerializeField] protected float healthReceiveFactor = 1f;
        [FoldoutGroup("Being Resources"), SerializeField] protected bool startNotAtMaxHealth;
        [FoldoutGroup("Being Resources"), SerializeField, Min(0f), PropertyRange(0f, "@maxHealth"), ShowIf("startNotAtMaxHealth")] protected float startHealth;

        [Header("Power")]
        [FoldoutGroup("Being Resources"), SerializeField] protected float maxPower;
        [FoldoutGroup("Being Resources"), SerializeField] protected float powerRegen;
        [FoldoutGroup("Being Resources"), SerializeField] protected float powerReceiveFactor = 1f;
        [FoldoutGroup("Being Resources"), SerializeField] protected bool startNotAtMaxPower;
        [FoldoutGroup("Being Resources"), SerializeField, Min(0f), PropertyRange(0f, "@maxPower"), ShowIf("startNotAtMaxPower")] protected float startPower;

        [Header("Stamina")]
        [FoldoutGroup("Being Resources"), SerializeField] protected float maxStamina;
        [FoldoutGroup("Being Resources"), SerializeField] protected float staminaRegen;
        [FoldoutGroup("Being Resources"), SerializeField] protected float staminaReceiveFactor = 1f;
        [FoldoutGroup("Being Resources"), SerializeField] protected bool startNotAtMaxStamina;
        [FoldoutGroup("Being Resources"), SerializeField, Min(0f), PropertyRange(0f, "@maxStamina"), ShowIf("startNotAtMaxStamina")] protected float startStamina;

        [Header("Mana")]
        [FoldoutGroup("Being Resources"), SerializeField] protected float maxMana;
        [FoldoutGroup("Being Resources"), SerializeField] protected float manaRegen;
        [FoldoutGroup("Being Resources"), SerializeField] protected float manaReceiveFactor = 1f;
        [FoldoutGroup("Being Resources"), SerializeField] protected bool startNotAtMaxMana;
        [FoldoutGroup("Being Resources"), SerializeField, Min(0f), PropertyRange(0f, "@maxMana"), ShowIf("startNotAtMaxMana")] protected float startMana;


		public float GetMaxResource(EntityBeingResourceType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourceType.Health:
					return maxHealth;
				case EntityBeingResourceType.Power:
					return maxPower;
				case EntityBeingResourceType.Stamina:
					return maxStamina;
				case EntityBeingResourceType.Mana:
					return maxMana;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		public float GetResourceRegen(EntityBeingResourceType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourceType.Health:
					return healthRegen;
				case EntityBeingResourceType.Power:
					return powerRegen;
				case EntityBeingResourceType.Stamina:
					return staminaRegen;
				case EntityBeingResourceType.Mana:
					return manaRegen;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		public float GetResourceReceivedFactor(EntityBeingResourceType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourceType.Health:
					return healthReceiveFactor;
				case EntityBeingResourceType.Power:
					return powerReceiveFactor;
				case EntityBeingResourceType.Stamina:
					return staminaReceiveFactor;
				case EntityBeingResourceType.Mana:
					return manaReceiveFactor;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		
		public bool GetResourceStartNotAtMax(EntityBeingResourceType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourceType.Health:
					return startNotAtMaxHealth;
				case EntityBeingResourceType.Power:
					return startNotAtMaxPower;
				case EntityBeingResourceType.Stamina:
					return startNotAtMaxStamina;
				case EntityBeingResourceType.Mana:
					return startNotAtMaxMana;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		
		public float GetResourceStartAmount(EntityBeingResourceType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourceType.Health:
					return startHealth;
				case EntityBeingResourceType.Power:
					return startPower;
				case EntityBeingResourceType.Stamina:
					return startStamina;
				case EntityBeingResourceType.Mana:
					return startMana;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}

        public partial class EntityBeing : EntityComponent
        {
			//Health
			protected float currentHealth;

			public float CurrentHealth => currentHealth;
			public float HealthReceiveFactor => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Health, EntityBeingResourceStat.ResourceReceivedFactor, _data.healthReceiveFactor) : _data.healthReceiveFactor;
			public float MaxHealth => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Health, EntityBeingResourceStat.MaxResource, _data.maxHealth) : _data.maxHealth;
			public float HealthProgress => currentHealth / MaxHealth;

			public void SetHealth(float value)
			{
				currentHealth = Mathf.Clamp(value, 0f, MaxHealth);
			}

			public void ModifyHealth(float amount)
			{
				currentHealth += amount;
				currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
			}

			//Power
			protected float currentPower;

			public float CurrentPower => currentPower;
			public float PowerReceiveFactor => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Power, EntityBeingResourceStat.ResourceReceivedFactor, _data.powerReceiveFactor) : _data.powerReceiveFactor;
			public float MaxPower => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Power, EntityBeingResourceStat.MaxResource, _data.maxPower) : _data.maxPower;
			public float PowerProgress => currentPower / MaxPower;

			public void SetPower(float value)
			{
				currentPower = Mathf.Clamp(value, 0f, MaxPower);
			}

			public void ModifyPower(float amount)
			{
				currentPower += amount;
				currentPower = Mathf.Clamp(currentPower, 0f, MaxPower);
			}

			//Stamina
			protected float currentStamina;

			public float CurrentStamina => currentStamina;
			public float StaminaReceiveFactor => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Stamina, EntityBeingResourceStat.ResourceReceivedFactor, _data.staminaReceiveFactor) : _data.staminaReceiveFactor;
			public float MaxStamina => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Stamina, EntityBeingResourceStat.MaxResource, _data.maxStamina) : _data.maxStamina;
			public float StaminaProgress => currentStamina / MaxStamina;

			public void SetStamina(float value)
			{
				currentStamina = Mathf.Clamp(value, 0f, MaxStamina);
			}

			public void ModifyStamina(float amount)
			{
				currentStamina += amount;
				currentStamina = Mathf.Clamp(currentStamina, 0f, MaxStamina);
			}

			//Mana
			protected float currentMana;

			public float CurrentMana => currentMana;
			public float ManaReceiveFactor => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Mana, EntityBeingResourceStat.ResourceReceivedFactor, _data.manaReceiveFactor) : _data.manaReceiveFactor;
			public float MaxMana => HasStats ? Stats.EvaluateBeingResourceStat(EntityBeingResourceType.Mana, EntityBeingResourceStat.MaxResource, _data.maxMana) : _data.maxMana;
			public float ManaProgress => currentMana / MaxMana;

			public void SetMana(float value)
			{
				currentMana = Mathf.Clamp(value, 0f, MaxMana);
			}

			public void ModifyMana(float amount)
			{
				currentMana += amount;
				currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
			}
			
			//Generics
			public float GetCurrentResource(EntityBeingResourceType resourceType)
			{
				switch(resourceType)
				{
                    case EntityBeingResourceType.Health:
                    	return currentHealth;
                    case EntityBeingResourceType.Power:
                    	return currentPower;
                    case EntityBeingResourceType.Stamina:
                    	return currentStamina;
                    case EntityBeingResourceType.Mana:
                    	return currentMana;
				}
				
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public void SetCurrentResource(EntityBeingResourceType resourceType, float value)
			{
				switch(resourceType)
				{
                    case EntityBeingResourceType.Health:
                    	currentHealth = value;
                    	currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
                    	break;
                    case EntityBeingResourceType.Power:
                    	currentPower = value;
                    	currentPower = Mathf.Clamp(currentPower, 0f, MaxPower);
                    	break;
                    case EntityBeingResourceType.Stamina:
                    	currentStamina = value;
                    	currentStamina = Mathf.Clamp(currentStamina, 0f, MaxStamina);
                    	break;
                    case EntityBeingResourceType.Mana:
                    	currentMana = value;
                    	currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
                    	break;
				}
			}
			public void ModifyCurrentResource(EntityBeingResourceType resourceType, float amount)
			{
				switch(resourceType)
				{
                    case EntityBeingResourceType.Health:
                    	currentHealth += amount;
                    	currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
                    	break;
                    case EntityBeingResourceType.Power:
                    	currentPower += amount;
                    	currentPower = Mathf.Clamp(currentPower, 0f, MaxPower);
                    	break;
                    case EntityBeingResourceType.Stamina:
                    	currentStamina += amount;
                    	currentStamina = Mathf.Clamp(currentStamina, 0f, MaxStamina);
                    	break;
                    case EntityBeingResourceType.Mana:
                    	currentMana += amount;
                    	currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
                    	break;
				}
			}
			public float GetMaxResource(EntityBeingResourceType resourceType)
			{
				switch(resourceType)
				{
					case EntityBeingResourceType.Health:
						return MaxHealth;
					case EntityBeingResourceType.Power:
						return MaxPower;
					case EntityBeingResourceType.Stamina:
						return MaxStamina;
					case EntityBeingResourceType.Mana:
						return MaxMana;
				}
				
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public float GetResourceProgress(EntityBeingResourceType resourceType)
			{
				switch(resourceType)
				{
                    case EntityBeingResourceType.Health:
                    	return HealthProgress;
                    case EntityBeingResourceType.Power:
                    	return PowerProgress;
                    case EntityBeingResourceType.Stamina:
                    	return StaminaProgress;
                    case EntityBeingResourceType.Mana:
                    	return ManaProgress;
				}
				
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public float GetResourceRegen(EntityBeingResourceType resourceType)
			{
				switch(resourceType)
				{



default: return 0f;
				}
			
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public float GetResourceReceivedFactor(EntityBeingResourceType resourceType)
			{
				switch(resourceType)
				{
					case EntityBeingResourceType.Health:
						return HealthReceiveFactor;
					case EntityBeingResourceType.Power:
						return PowerReceiveFactor;
					case EntityBeingResourceType.Stamina:
						return StaminaReceiveFactor;
					case EntityBeingResourceType.Mana:
						return ManaReceiveFactor;
				}
			
				throw new System.Exception("No " + resourceType + " resource found.");
			}
        }
    }
}