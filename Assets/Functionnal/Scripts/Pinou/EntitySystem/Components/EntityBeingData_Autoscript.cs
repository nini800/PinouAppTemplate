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
                    	currentHealth = Mathf.Clamp(currentHealth, 0f, _data.maxHealth);
                    	break;
                    case EntityBeingResourceType.Power:
                    	currentPower = value;
                    	currentPower = Mathf.Clamp(currentPower, 0f, _data.maxPower);
                    	break;
                    case EntityBeingResourceType.Stamina:
                    	currentStamina = value;
                    	currentStamina = Mathf.Clamp(currentStamina, 0f, _data.maxStamina);
                    	break;
                    case EntityBeingResourceType.Mana:
                    	currentMana = value;
                    	currentMana = Mathf.Clamp(currentMana, 0f, _data.maxMana);
                    	break;
				}
			}
			public void ModifyCurrentResource(EntityBeingResourceType resourceType, float amount)
			{
				switch(resourceType)
				{
                    case EntityBeingResourceType.Health:
                    	currentHealth += amount;
                    	currentHealth = Mathf.Clamp(currentHealth, 0f, _data.maxHealth);
                    	break;
                    case EntityBeingResourceType.Power:
                    	currentPower += amount;
                    	currentPower = Mathf.Clamp(currentPower, 0f, _data.maxPower);
                    	break;
                    case EntityBeingResourceType.Stamina:
                    	currentStamina += amount;
                    	currentStamina = Mathf.Clamp(currentStamina, 0f, _data.maxStamina);
                    	break;
                    case EntityBeingResourceType.Mana:
                    	currentMana += amount;
                    	currentMana = Mathf.Clamp(currentMana, 0f, _data.maxMana);
                    	break;
				}
			}
			public float GetMaxResource(EntityBeingResourceType resourceType)
			{
				switch(resourceType)
				{
					case EntityBeingResourceType.Health:
						return _data.maxHealth;
					case EntityBeingResourceType.Power:
						return _data.maxPower;
					case EntityBeingResourceType.Stamina:
						return _data.maxStamina;
					case EntityBeingResourceType.Mana:
						return _data.maxMana;
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
					case EntityBeingResourceType.Health:
						return _data.healthRegen;
					case EntityBeingResourceType.Power:
						return _data.powerRegen;
					case EntityBeingResourceType.Stamina:
						return _data.staminaRegen;
					case EntityBeingResourceType.Mana:
						return _data.manaRegen;
				}
			
				throw new System.Exception("No " + resourceType + " resource found.");
			}
			public float GetResourceReceivedFactor(EntityBeingResourceType resourceType)
			{
				switch(resourceType)
				{
					case EntityBeingResourceType.Health:
						return _data.healthReceiveFactor;
					case EntityBeingResourceType.Power:
						return _data.powerReceiveFactor;
					case EntityBeingResourceType.Stamina:
						return _data.staminaReceiveFactor;
					case EntityBeingResourceType.Mana:
						return _data.manaReceiveFactor;
				}
			
				throw new System.Exception("No " + resourceType + " resource found.");
			}
        }
    }
}