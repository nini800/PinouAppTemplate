#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou.UI
{
    public class UI_SelectorManager : PinouBehaviour
    {
        [SerializeField] private RectTransform _selectionBorder;
        private static RectTransform S_selectionBorder;
        private PinouInputReceiver _ir;

        public static void ApplySelectionBorder(RectTransform rect)
        {
            S_selectionBorder.localScale = Vector3.one;
            S_selectionBorder.SetParent(rect.transform);
            S_selectionBorder.sizeDelta = rect.sizeDelta + new Vector2(5, 5);
            S_selectionBorder.position = rect.position;
        }

        protected override void OnAwake()
        {
            S_selectionBorder = _selectionBorder;
            _ir = GetComponent<PinouInputReceiver>();
        }
        private void Update()
        {
            HandleInputs();
            HandleSelectionBorderDisplay();
        }

        private void HandleInputs()
        {
            if (UI_SelectableElement.CurrentSelectedElement != null && UI_SelectableElement.CurrentSelectedElement.isActiveAndEnabled == true)
            {
                if (_ir.Menu_GoUp == true)
                {
                    UI_SelectableElement.CurrentSelectedElement.GetNeighbour(Direction.Up)?.Select();
                }
                else if (_ir.Menu_GoDown == true)
                {
                    UI_SelectableElement.CurrentSelectedElement.GetNeighbour(Direction.Down)?.Select();
                }
                else if (_ir.Menu_GoRight == true)
                {
                    UI_SelectableElement.CurrentSelectedElement.GetNeighbour(Direction.Right)?.Select();
                }
                else if (_ir.Menu_GoLeft == true)
                {
                    UI_SelectableElement.CurrentSelectedElement.GetNeighbour(Direction.Left)?.Select();
                }

                if (_ir.Menu_Accept == true)
                {
                    UI_SelectableElement.CurrentSelectedElement.Invoke();
                }
            }
        }
        private void HandleSelectionBorderDisplay()
        {
            if (UI_SelectableElement.CurrentSelectedElement == null || UI_SelectableElement.CurrentSelectedElement.isActiveAndEnabled == false)
            {
                S_selectionBorder.gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < UI_SelectableElement.CurrentSelectedElement.SelectEffects.Length; i++)
                {
                    if (UI_SelectableElement.CurrentSelectedElement.SelectEffects[i].EffectType == UI_SelectableElement.SelectEffect.SelectEffectType.SelectionBorder)
                    {
                        S_selectionBorder.gameObject.SetActive(true);
                        return;
                    }
                }

                S_selectionBorder.gameObject.SetActive(false);
            }
        }
    }
}