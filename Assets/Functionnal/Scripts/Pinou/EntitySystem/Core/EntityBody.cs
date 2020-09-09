#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.EntitySystem
{
	public partial class EntityBody : PinouBehaviour
	{
		#region Vars, Fields, Getters
		private Entity _master;
		private Dictionary<EntityEquipable, GameObject[]> _equippedVisuals = new Dictionary<EntityEquipable, GameObject[]>();
		#endregion

		#region Behaviour
		protected override void OnAwake()
		{
			_master = GetComponentInParent<Entity>();
		}

		protected override void OnEnabled()
		{
			if (_master == null) { return; }

			if (_master.HasEquipment == true)
			{
				_master.Equipment.OnEquip.SafeSubscribe(OnEquip);
				_master.Equipment.OnUnequip.SafeSubscribe(OnUnequip);
			}
		}

		protected override void OnDisabled()
		{
			if (_master == null) { return; }

			if (_master.HasEquipment == true)
			{
				_master.Equipment.OnEquip.Unsubscribe(OnEquip);
				_master.Equipment.OnUnequip.Unsubscribe(OnUnequip);
			}
		}
		#endregion

		#region Utilities
		private void EquipVisual(EntityEquipable eq)
		{
			if (eq == null || eq.HasVisual == false) { return; }
			if (_equippedVisuals.ContainsKey(eq)) { UnequipVisual(eq); }

			GameObject[] eqVisuals = new GameObject[eq.Visual.Parts.Length];
			for (int i = 0; i < eqVisuals.Length; i++)
			{
				eqVisuals[i] = PinouApp.Pooler.Retrieve(eq.Visual.Parts[i].Model, GetSocket(eq.Visual.Parts[i].Socket)).gameObject;
				eqVisuals[i].transform.localPosition = Vector3.zero;
				eqVisuals[i].transform.localRotation = Quaternion.Euler(0,0,0);
				eqVisuals[i].transform.localScale = Vector3.one;
			}

			_equippedVisuals.Add(eq, eqVisuals);
		}
		private void UnequipVisual(EntityEquipable eq)
		{
			if (eq == null || eq.HasVisual == false) { return; }
			if (_equippedVisuals.ContainsKey(eq) == false) { return; }

			GameObject[] eqVisuals = _equippedVisuals[eq];
			for (int i = 0; i < eqVisuals.Length; i++)
			{
				PinouApp.Pooler.Store(eqVisuals[i]);
			}

			_equippedVisuals.Remove(eq);
		}
		#endregion

		#region Events
		private void OnEquip(Entity ent, EntityEquipable eq)
		{
			EquipVisual(eq);
		}
		private void OnUnequip(Entity ent, EntityEquipable eq)
		{
			UnequipVisual(eq);
		}
		#endregion
	}
}