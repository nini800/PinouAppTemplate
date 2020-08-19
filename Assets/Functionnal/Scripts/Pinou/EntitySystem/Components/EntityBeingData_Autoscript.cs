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


		public float GetMaxResource(EntityBeingResourcesType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourcesType.Health:
					return maxHealth;
				case EntityBeingResourcesType.Power:
					return maxPower;
				case EntityBeingResourcesType.Stamina:
					return maxStamina;
				case EntityBeingResourcesType.Mana:
					return maxMana;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		public float GetResourceRegen(EntityBeingResourcesType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourcesType.Health:
					return healthRegen;
				case EntityBeingResourcesType.Power:
					return powerRegen;
				case EntityBeingResourcesType.Stamina:
					return staminaRegen;
				case EntityBeingResourcesType.Mana:
					return manaRegen;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		public float GetResourceReceivedFactor(EntityBeingResourcesType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourcesType.Health:
					return healthReceiveFactor;
				case EntityBeingResourcesType.Power:
					return powerReceiveFactor;
				case EntityBeingResourcesType.Stamina:
					return staminaReceiveFactor;
				case EntityBeingResourcesType.Mana:
					return manaReceiveFactor;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		
		public bool GetResourceStartNotAtMax(EntityBeingResourcesType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourcesType.Health:
					return startNotAtMaxHealth;
				case EntityBeingResourcesType.Power:
					return startNotAtMaxPower;
				case EntityBeingResourcesType.Stamina:
					return startNotAtMaxStamina;
				case EntityBeingResourcesType.Mana:
					return startNotAtMaxMana;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}
		
		public float GetResourceStartAmount(EntityBeingResourcesType resourceType)
		{
			switch(resourceType)
			{
				case EntityBeingResourcesType.Health:
					return startHealth;
				case EntityBeingResourcesType.Power:
					return startPower;
				case EntityBeingResourcesType.Stamina:
					return startStamina;
				case EntityBeingResourcesType.Mana:
					return startMana;
			}
			
			throw new System.Exception("No " + resourceType + " resource found.");
		}

        public partial class EntityBeing : EntityComponent
        {
			//Health
			protected float currentHealth;

			public float CurrentHealth => currentHealth;
			public float HealthRegen => _data.healthRegen;
			public float HealthReceiveFactor => _data.healthReceiveFactor;
			public float MaxHealth => _data.maxHealth;
			public float HealthProgress => currentHealth / _data.maxHealth;

			public void SetHealth(float value)
			{
				currentHealth = Mathf.Clamp(value, 0f, _data.maxHealth);
			}

			public void ModifyHealth(float amount)
			{
				currentHealth += amount;
				currentHealth = Mathf.Clamp(currentHealth, 0f, _data.maxHealth);
			}

			//Power
			protected float currentPower;

			public float CurrentPower => currentPower;
			public float PowerRegen => _data.powerRegen;
			public float PowerReceiveFactor => _data.powerReceiveFactor;
			public float MaxPower => _data.maxPower;
			public float PowerProgress => currentPower / _data.maxPower;

			public void SetPower(float value)
			{
				currentPower = Mathf.Clamp(value, 0f, _data.maxPower);
			}

			public void ModifyPower(float amount)
			{
				currentPower += amount;
				currentPower = Mathf.Clamp(currentPower, 0f, _data.maxPower);
			}

			//Stamina
			protected float currentStamina;

			public float CurrentStamina => currentStamina;
			public float StaminaRegen => _data.staminaRegen;
			public float StaminaReceiveFactor => _data.staminaReceiveFactor;
			public float MaxStamina => _data.maxStamina;
			public float StaminaProgress => currentStamina / _data.maxStamina;

			public void SetStamina(float value)
			{
				currentStamina = Mathf.Clamp(value, 0f, _data.maxStamina);
			}

			public void ModifyStamina(float amount)
			{
				currentStamina += amount;
				currentStamina = Mathf.Clamp(currentStamina, 0f, _data.maxStamina);
			}

			//Mana
			protected float currentMana;

			public float CurrentMana => currentMana;
			public float ManaRegen => _data.manaRegen;
			public float ManaReceiveFactor => _data.manaReceiveFactor;
			public float MaxMana => _data.maxMana;
			public float ManaProgress => currentMana / _data.maxMana;

			public void SetMana(float value)
			{
				currentMana = Mathf.Clamp(value, 0f, _data.maxMana);
			}

			public void ModifyMana(float amount)
			{
				currentMana += amount;
				currentMana = Mathf.Clamp(currentMana, 0f, _data.maxMana);
			}
			
			//Generics
			public float GetCurrentResource(EntityBeingResourcesType resourceType)
			{
				switch(resourceType)
				{
                    case EntityBeingResourcesType.Health:
                    	return currentHealth;
                    case EntityBeingResourcesType.Power:
                    	return currentPower;
                    case EntityBeingResourcesType.Stamina:
                    	return currentStamina;
                    case EntityBeingResourcesType.Mana:
                    	return currentMana;
				}
				
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public void SetCurrentResource(EntityBeingResourcesType resourceType, float value)
			{
				switch(resourceType)
				{
                    case EntityBeingResourcesType.Health:
                    	currentHealth = value;
                    	currentHealth = Mathf.Clamp(currentHealth, 0f, _data.maxHealth);
                    	break;
                    case EntityBeingResourcesType.Power:
                    	currentPower = value;
                    	currentPower = Mathf.Clamp(currentPower, 0f, _data.maxPower);
                    	break;
                    case EntityBeingResourcesType.Stamina:
                    	currentStamina = value;
                    	currentStamina = Mathf.Clamp(currentStamina, 0f, _data.maxStamina);
                    	break;
                    case EntityBeingResourcesType.Mana:
                    	currentMana = value;
                    	currentMana = Mathf.Clamp(currentMana, 0f, _data.maxMana);
                    	break;
				}
			}
			public void ModifyCurrentResource(EntityBeingResourcesType resourceType, float amount)
			{
				switch(resourceType)
				{
                    case EntityBeingResourcesType.Health:
                    	currentHealth += amount;
                    	currentHealth = Mathf.Clamp(currentHealth, 0f, _data.maxHealth);
                    	break;
                    case EntityBeingResourcesType.Power:
                    	currentPower += amount;
                    	currentPower = Mathf.Clamp(currentPower, 0f, _data.maxPower);
                    	break;
                    case EntityBeingResourcesType.Stamina:
                    	currentStamina += amount;
                    	currentStamina = Mathf.Clamp(currentStamina, 0f, _data.maxStamina);
                    	break;
                    case EntityBeingResourcesType.Mana:
                    	currentMana += amount;
                    	currentMana = Mathf.Clamp(currentMana, 0f, _data.maxMana);
                    	break;
				}
			}
			public float GetMaxResource(EntityBeingResourcesType resourceType)
			{
				switch(resourceType)
				{
					case EntityBeingResourcesType.Health:
						return _data.maxHealth;
					case EntityBeingResourcesType.Power:
						return _data.maxPower;
					case EntityBeingResourcesType.Stamina:
						return _data.maxStamina;
					case EntityBeingResourcesType.Mana:
						return _data.maxMana;
				}
				
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public float GetResourceRegen(EntityBeingResourcesType resourceType)
			{
				switch(resourceType)
				{
					case EntityBeingResourcesType.Health:
						return _data.healthRegen;
					case EntityBeingResourcesType.Power:
						return _data.powerRegen;
					case EntityBeingResourcesType.Stamina:
						return _data.staminaRegen;
					case EntityBeingResourcesType.Mana:
						return _data.manaRegen;
				}
			
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public float GetResourceReceivedFactor(EntityBeingResourcesType resourceType)
			{
				switch(resourceType)
				{
					case EntityBeingResourcesType.Health:
						return _data.healthReceiveFactor;
					case EntityBeingResourcesType.Power:
						return _data.powerReceiveFactor;
					case EntityBeingResourcesType.Stamina:
						return _data.staminaReceiveFactor;
					case EntityBeingResourcesType.Mana:
						return _data.manaReceiveFactor;
				}
			
				throw new System.Exception("No " + resourceType + " resource found.");
			}
        }
    }
}