#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Pinou.UI
{

    public class UI_ScrollableEventTrigger : EventTrigger
    {
        [SerializeField] private float _maxDragDistanceForPointerDown = 10f;
        private Vector3 _dragStartPos;

        public override void OnBeginDrag(PointerEventData PED)
        {
            _dragStartPos = UnityEngine.Input.mousePosition;
            transform.GetComponentInParent<ScrollRect>().OnBeginDrag(PED);
        }
        public override void OnDrag(PointerEventData PED)
        {
            transform.GetComponentInParent<ScrollRect>().OnDrag(PED);
        }
        public override void OnEndDrag(PointerEventData PED)
        {
            transform.GetComponentInParent<ScrollRect>().OnEndDrag(PED);
        }

        public override void OnPointerDown(PointerEventData PED)
        {
            float dist = (_dragStartPos - UnityEngine.Input.mousePosition).sqrMagnitude;

            if (dist > _maxDragDistanceForPointerDown * _maxDragDistanceForPointerDown)
            {
                return;
            }

            int num = 0;
            int count = triggers.Count;
            while (num < count)
            {
                Entry entry = triggers[num];
                if ((entry.eventID == EventTriggerType.PointerDown) && (entry.callback != null))
                {
                    entry.callback.Invoke(PED);
                }
                num++;
            }
        }
    }
}
