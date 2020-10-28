#pragma warning disable 0649
using System;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou.ItemSystem
{
	[Serializable]
	public class EntityEquipableVisual
	{
		public EntityEquipableVisual(Sprite icon, VisualPart[] parts)
		{
			_icon = icon;
			_parts = parts;
		}

		[Serializable]
		public class VisualPart
		{
			public VisualPart(GameObject model, EntityBodySocket socket)
			{
				_model = model;
				_socket = socket;
			}
			[SerializeField] private GameObject _model;
			[SerializeField] private EntityBodySocket _socket;

			public GameObject Model => _model;
			public EntityBodySocket Socket => _socket;
		}

		[SerializeField] private Sprite _icon;
		[SerializeField] private VisualPart[] _parts;

		public Sprite Icon => _icon;
		public VisualPart[] Parts => _parts;
	}
}
