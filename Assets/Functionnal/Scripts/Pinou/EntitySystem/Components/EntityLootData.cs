#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
    #region Data Classes
    [System.Serializable]
    public class LootBundle
	{
        [System.Serializable]
        public class LootParameters
        {
            [SerializeField, InlineEditor] private EntityDropData _drop;
            [SerializeField, Min(1)] private int _minDropCount = 1;
            [SerializeField, MinValue("@_minDropCount")] private float _maxDropCount = 1f;
            [SerializeField, PropertyRange(0f, "@_maxLootChances")] private float _lootChances;
            [SerializeField, ReadOnly] private float _maxLootChances = 100f;

            public EntityDropData Drop => _drop;
            public float LootChances => _lootChances;

            public EntityDropData[] SuccessLoot()
			{
                float addOneChances = (_maxDropCount - _minDropCount) - Mathf.Floor(_maxDropCount - _minDropCount);
                bool addOne = Random.value <= addOneChances;

                EntityDropData[] drops = new EntityDropData[Mathf.FloorToInt(Random.Range(_minDropCount, _maxDropCount)) + (addOne ? 1 : 0)];
				for (int i = 0; i < drops.Length; i++)
				{
                    drops[i] = _drop;
				}

                return drops;
			}
            public EntityDropData[] AttemptLoot()
			{
                return Random.Range(0f, 100f) <= _lootChances ? SuccessLoot() : new EntityDropData[] { };
			}
        }
        [System.Serializable]
        public class LootGroup
		{
            [SerializeField, Min(1)] private int _groupLootTries = 1;
            [SerializeField, ReadOnly] private float _groupTotalChances = 0f;

            public int MaxGroupLoot => _groupLootTries;
            public float GroupTotalChances => _groupTotalChances;
            public LootParameters[] LootParameters => _lootParameters;

            public EntityDropData[] AttemptGroupLoot()
			{
                List<EntityDropData> drops = new List<EntityDropData>();
				for (int i = 0; i < _groupLootTries; i++)
				{
                    float rand = Random.Range(0f, _groupTotalChances);
                    float chanceCounter = 0f;
					for (int l = 0; l < _lootParameters.Length; l++)
					{
                        chanceCounter += _lootParameters[l].LootChances;
                        if (rand < chanceCounter)
						{
                            drops.AddRange(_lootParameters[i].SuccessLoot());
                            break;
						}
					}
				}

                return drops.ToArray();
			}
            [SerializeField, ValidateInput("EnsureCorrectMaxLootChances", "")] private LootParameters[] _lootParameters;

            private bool EnsureCorrectMaxLootChances(LootParameters[] loots)
			{
#if !UNITY_EDITOR
                return true;
#else
                float totalChances = 0f;
				for (int i = 0; i < loots.Length; i++)
				{
                    totalChances += loots[i].LootChances;
				}
                _groupTotalChances = totalChances;
                float remainingChances = 100f - _groupTotalChances;
                for (int i = 0; i < loots.Length; i++)
                {
                    loots[i].GetType().GetField("_maxLootChances", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(loots[i], loots[i].LootChances + remainingChances);
                }

                return true;
#endif  
            }
        }

        [SerializeField] private LootParameters[] _ungroupedLoots;
        [SerializeField] private LootGroup[] _groupedLoots;

        public LootParameters[] UngroupedLoots => _ungroupedLoots;
        public LootGroup[] GroupedLoots => _groupedLoots;

        public EntityDropData[] ComputeLoot()
		{
            List<EntityDropData> drops = new List<EntityDropData>();
			for (int i = 0; i < _ungroupedLoots.Length; i++)
			{
                drops.AddRange(_ungroupedLoots[i].AttemptLoot());
			}
			for (int i = 0; i < _groupedLoots.Length; i++)
			{
                drops.AddRange(_groupedLoots[i].AttemptGroupLoot());
			}

            return drops.ToArray();
		}
    }

    #endregion

    public class EntityLootData : EntityComponentData
    {
        #region Vars, Fields, Getters
        [Header("Loot Parameters")]
        [Space]
        [SerializeField] private LootBundle _lootBundle;

        [Header("Pickup Drops Parameters")]
        [Space]
        [SerializeField] private bool _canPickupDrops = false;
        [SerializeField, ShowIf(nameof(_canPickupDrops))] private float _dropAwakeDistance = 30f;
        [SerializeField, ShowIf(nameof(_canPickupDrops))] private float _pickupRange = 0.25f;

        public bool CanPickupDrops => _canPickupDrops;
        public float DropAwakeDistance => _dropAwakeDistance;
        public float PickupRange => _pickupRange;
        #endregion

        #region Component Core
        public override EntityComponent ConstructComponent(Entity master, EntityReferences references)
        {
            return ConstructComponent<EntityLoot, EntityLootData>(master, references, this);
        }
        #endregion

        public class EntityLoot : EntityComponent
        {
            #region OnConstruct
            /// <summary>
            /// Need base
            /// </summary>
            protected override void OnConstruct()
            {
                _data = (EntityLootData)((EntityComponent)this).Data;
            }

            private EntityLootData _data = null;
            protected new EntityLootData Data => _data;
            #endregion

            #region Utilities
            public EntityDropData[] ComputeLoot() => _data._lootBundle.ComputeLoot();
			#endregion

			#region Behaviour
            /// <summary>
            /// need base
            /// </summary>
			public override void SlaveUpdate()
			{
                HandlePickupDrops();
			}
			protected virtual void HandlePickupDrops()
			{
                if (Data._canPickupDrops == false) { return; }

                Collider[] colls = Physics.OverlapSphere(Position, _data.DropAwakeDistance, PinouApp.Resources.Data.Layers.EntityDrops);
				foreach (Collider coll in colls)
				{
                    EntityDrop drop;
                    if (drop = coll.GetComponentInParent<EntityDrop>())
					{
                        if ((drop.transform.position - Position).sqrMagnitude <= _data._pickupRange.Squared())
						{
                            drop.Collect(master);
                        }
                        else
						{
                            PinouApp.Loot.RegisterDropIfNeeded(drop);
						}
                    }
				}
			}
			#endregion
		}
	}
}