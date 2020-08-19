#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Pinou.UI
{
    public partial class UI_ScrollableButton : Button
    {
        [SerializeField] private float _maxDragDistanceForPointerDown = 10f;
        private Vector3 _dragStartPos;

        public override void OnPointerDown(PointerEventData eventData)
        {
            _dragStartPos = UnityEngine.Input.mousePosition;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            float dist = (_dragStartPos - UnityEngine.Input.mousePosition).sqrMagnitude;
            if (dist > _maxDragDistanceForPointerDown * _maxDragDistanceForPointerDown)
            {
                return;
            }

            base.OnPointerClick(eventData);
        }
    }
}