#pragma warning disable 0649, 0414
using Pinou.ItemSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pinou.UI
{
	public class UI_SlotsInventory : PinouBehaviour
	{
		[Header("References")]
		[Space]
		[SerializeField] private List<InventorySlot> _slots = new List<InventorySlot>();

		[Header("Editor Builder")]
		[Space]
		[SerializeField, Min(1)] private int E_SlotWidthCount = 1;
		[SerializeField, Min(1)] private int E_SlotHeightCount = 1;
		[SerializeField, Min(1f)] private float E_SlotWidth = 20f;
		[SerializeField, Min(1f)] private float E_SlotHeight = 20f;
		[SerializeField, Min(0f)] private float E_SlotHorizontalMargin = 2f;
		[SerializeField, Min(0f)] private float E_SlotVerticalMargin = 2f;
		[Space]
		[SerializeField] private Color E_SlotColor = Color.white;
		[SerializeField] private Sprite E_SlotSprite = null;

		[Button("Build Inventory")]
		private void E_BuildInventory()
		{
			//Destroy all existing slots
			InventorySlot[] slots = GetComponentsInChildren<InventorySlot>(true);
			for (int i = slots.Length - 1; i >= 0; i--)
			{
				DestroyImmediate(slots[i].gameObject);
			}
			_slots = new List<InventorySlot>();

			//UI Size of the inventory
			RectTransform.sizeDelta = new Vector2(
				E_SlotWidth * E_SlotWidthCount + E_SlotHorizontalMargin * (E_SlotWidthCount + 1),
				E_SlotHeight * E_SlotHeightCount + E_SlotVerticalMargin * (E_SlotHeightCount + 1));

			for (int y = 0; y < E_SlotHeightCount; y++)
			{
				for (int x = 0; x < E_SlotWidthCount; x++)
				{
					Vector2 slotAnchPos = new Vector2(
						E_SlotWidth * x + E_SlotHorizontalMargin * (x + 1) + E_SlotWidth * 0.5f,
						-(E_SlotHeight * y + E_SlotVerticalMargin * (y + 1) + E_SlotHeight * 0.5f));

					int slotID = (y * E_SlotWidthCount) + x + 1;
					GameObject slot = new GameObject("Slot_" + slotID, typeof(RectTransform));
					RectTransform slotRect = slot.GetComponent<RectTransform>();
					slotRect.SetParent(transform);
					slotRect.anchorMin = new Vector2(0, 1);
					slotRect.anchorMax = new Vector2(0, 1);
					slotRect.anchoredPosition = slotAnchPos;
					slotRect.sizeDelta = new Vector2(E_SlotWidth, E_SlotHeight);
					slotRect.localScale = Vector3.one;

					Image slotImg = slot.AddComponent<Image>();
					slotImg.color = E_SlotColor;
					slotImg.sprite = E_SlotSprite;
					_slots.Add(slot.AddComponent<InventorySlot>());
				}
			}
		}
	}
}