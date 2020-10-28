#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou;
using Pinou.EntitySystem;
using System;
using Sirenix.OdinInspector;

namespace Pinou.UI
{
	public class UI_DamageTextsManager : UI_PinouBehaviour
	{
		#region Data Classes, Enums
		public enum EntityType
		{
			None = 0,
			LocalPlayer = 1,
			OtherPlayer = 2,
			Ally = 3,
			Enemy = 4,
		}
		[Serializable]
		public class TextModelParameters
		{
			[Serializable]
			public class TextModelEntityParameters
			{
				[Serializable]
				public class TextModelEntityResourcesParameters
				{
					public TextModelEntityResourcesParameters(EntityBeingResourceType type)
					{
						_resourceType = type;
					}

					[SerializeField, ReadOnly] private EntityBeingResourceType _resourceType;
					[Space(3f)]
					[SerializeField] private bool _shouldDisplayNegativeText = false;
					[SerializeField, ShowIf("_shouldDisplayNegativeText")] private GameObject _negativeReceivedTextModel;
					[Space(3f)]
					[SerializeField] private bool _shouldDisplayPositiveText = false;
					[SerializeField, ShowIf("_shouldDisplayPositiveText")] private GameObject _positiveReceivedTextModel;

					public EntityBeingResourceType ResourceType => _resourceType;
					public bool ShouldDisplayPositiveText => _shouldDisplayPositiveText;
					public bool ShouldDisplayNegativeText => _shouldDisplayNegativeText;
					public GameObject NegativeReceivedTextModel => _negativeReceivedTextModel;
					public GameObject PositiveReceivedTextModel => _positiveReceivedTextModel;
				}

				public TextModelEntityParameters(EntityType type)
				{
					_entityType = type;
				}
				[SerializeField, ReadOnly] private EntityType _entityType;
				[SerializeField, ValidateInput("ValidateModelByResources")] private TextModelEntityResourcesParameters[] _modelPerResource;

				public EntityType EntityType => _entityType;
				public TextModelEntityResourcesParameters[] ModelsPerResource => _modelPerResource;
				private bool ValidateModelByResources(TextModelEntityResourcesParameters[] parameters)
				{
					if (_modelPerResource == null) { _modelPerResource = new TextModelEntityResourcesParameters[0]; }

					Array enumValues = Enum.GetValues(typeof(EntityBeingResourceType));
					bool valide = true;
					if (_modelPerResource.Length != enumValues.Length)
					{
						valide = false;
					}
					else
					{
						for (int i = 0; i < _modelPerResource.Length; i++)
						{
							if (_modelPerResource[i].ResourceType != (EntityBeingResourceType)Mathf.Pow(2f, i))
							{
								valide = false;
								break;
							}
						}
					}

					if (valide == false)
					{
						_modelPerResource = new TextModelEntityResourcesParameters[enumValues.Length];
						for (int i = 0; i < _modelPerResource.Length; i++)
						{
							_modelPerResource[i] = new TextModelEntityResourcesParameters((EntityBeingResourceType)Mathf.Pow(2f, i));
						}
					}

					return true;
				}

				public GameObject GetModel(EntityBeingResourceType resource, bool positive)
				{
					TextModelEntityResourcesParameters modelByResource = _modelPerResource[PinouUtils.Maths.Pow2toIndex((int)resource)];
					return positive ?
						(modelByResource.ShouldDisplayPositiveText ? modelByResource.PositiveReceivedTextModel : null) :
						(modelByResource.ShouldDisplayNegativeText ? modelByResource.NegativeReceivedTextModel : null);
				}
			}

			public TextModelParameters(EntityType type)
			{
				_entityType = type;
			}

			[SerializeField, ReadOnly] private EntityType _entityType;
			[SerializeField, ValidateInput("ValidateTextModels")] private TextModelEntityParameters[] _modelPerEntityType;

			public EntityType EntityType => _entityType;
			public TextModelEntityParameters[] ModelPerEntityType => _modelPerEntityType;

			public GameObject GetModel(EntityBeingResourceType resource, EntityType inflictingEntity, bool positive)
			{
				return _modelPerEntityType[((int)inflictingEntity - 1)].GetModel(resource, positive);
			}

			private bool ValidateTextModels(TextModelEntityParameters[] modelParameters)
			{
				Array enumValues = Enum.GetValues(typeof(EntityType));
				bool valide = true;
				if (modelParameters.Length != enumValues.Length - 1)
				{
					valide = false;
				}
				else
				{
					for (int i = 1; i <= modelParameters.Length; i++)
					{
						if (modelParameters[i-1].EntityType != (EntityType)(i))
						{
							valide = false;
							break;
						}
					}
				}

				if (valide == false)
				{
					_modelPerEntityType = new TextModelEntityParameters[enumValues.Length - 1];
					for (int i = 1; i <= modelParameters.Length; i++)
					{
						_modelPerEntityType[i-1] = new TextModelEntityParameters((EntityType)(i));
					}
				}

				return true;
			}
		}
		#endregion

		[Header("Model Parameters")]
		[Space]
		[SerializeField] private TextModelParameters[] _modelParameters;

		private List<UI_DamageText> _damageTexts = new List<UI_DamageText>();

		protected override void OnUIEnabled()
		{
			PinouApp.Entity.OnEntityAbilityHitResult.SafeSubscribe(OnEntityAbilityHitResult);
		}

		private void Update()
		{
			float ratioFactor = 1920f / Screen.width;
			for (int i = _damageTexts.Count - 1; i >= 0; i--)
			{
				if (_damageTexts[i].UpdatePosition(ref ratioFactor))
				{
					PinouApp.Pooler.Store(_damageTexts[i].gameObject);
					_damageTexts.RemoveAt(i);
				}
			}
		}

		private EntityType GetEntityType(Entity ent)
		{
			switch (ent.Team)
			{
				case EntityTeam.Players:
					if (ent != PinouApp.Entity.LocalPlayer)
					{
						return EntityType.OtherPlayer;
					}
					else
					{
						return EntityType.LocalPlayer;
					}
				case EntityTeam.Allies:
					return EntityType.Ally;
				case EntityTeam.Enemies:
					return EntityType.Enemy;
			}

			return EntityType.None;
		}

		private void OnEntityAbilityHitResult(Entity ent, AbilityCastResult result)
		{
			EntityType receivingEntityType = GetEntityType(ent);
			EntityType inflictingEntityType = GetEntityType(result.CastData.Caster);

			AbilityResourceImpactData[] resourcesChanges = result.ResourcesChanges;
			for (int i = 0; i < resourcesChanges.Length; i++)
			{
				bool positive = resourcesChanges[i].ResourceChange >= 0;
				if (ShouldMakeText(resourcesChanges[i].ResourceType, receivingEntityType, inflictingEntityType, positive))
				{
					GameObject model = GetModel(resourcesChanges[i].ResourceType, receivingEntityType, inflictingEntityType, positive);
					MakeText(model, result, resourcesChanges[i]);
				}
			}
		}

		private bool ShouldMakeText(EntityBeingResourceType resourceType, EntityType receivingType, EntityType inflictingType, bool positive)
		{
			return positive ? 
				_modelParameters[((int)receivingType) - 1].ModelPerEntityType[((int)inflictingType) - 1].ModelsPerResource[PinouUtils.Maths.Pow2toIndex((int)resourceType)].ShouldDisplayPositiveText:
				_modelParameters[((int)receivingType) - 1].ModelPerEntityType[((int)inflictingType) - 1].ModelsPerResource[PinouUtils.Maths.Pow2toIndex((int)resourceType)].ShouldDisplayNegativeText;
		}

		private GameObject GetModel(EntityBeingResourceType resourceType, EntityType receivingType, EntityType inflictingType, bool positive)
		{
			return positive ?
				_modelParameters[((int)receivingType) - 1].ModelPerEntityType[((int)inflictingType) - 1].ModelsPerResource[PinouUtils.Maths.Pow2toIndex((int)resourceType)].PositiveReceivedTextModel:
				_modelParameters[((int)receivingType) - 1].ModelPerEntityType[((int)inflictingType) - 1].ModelsPerResource[PinouUtils.Maths.Pow2toIndex((int)resourceType)].NegativeReceivedTextModel;
		}
		private void MakeText(GameObject model, AbilityCastResult result, AbilityResourceImpactData impactData)
		{
			GameObject textGo = PinouApp.Pooler.Retrieve(model, transform);
			UI_DamageText dmgTxt = textGo.GetComponent<UI_DamageText>();
			dmgTxt.Build(result, impactData);
			_damageTexts.Add(dmgTxt);
		}

#if UNITY_EDITOR
		protected override void E_OnValidate()
		{
			Array enumValues = Enum.GetValues(typeof(EntityType));
			bool valide = true;
			if (_modelParameters.Length != enumValues.Length - 1)
			{
				valide = false;
			}
			else
			{
				for (int i = 1; i <= _modelParameters.Length; i++)
				{
					if (_modelParameters[i - 1].EntityType != (EntityType)(i))
					{
						valide = false;
						break;
					}
				}
			}

			if (valide == false)
			{
				_modelParameters = new TextModelParameters[enumValues.Length - 1];
				for (int i = 1; i <= _modelParameters.Length; i++)
				{
					_modelParameters[i - 1] = new TextModelParameters((EntityType)(i));
				}
			}
		}
#endif
	}
}