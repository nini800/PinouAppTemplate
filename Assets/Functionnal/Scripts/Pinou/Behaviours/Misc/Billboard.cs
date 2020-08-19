#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
    public class Billboard : MonoBehaviour
    {
        private static bool _subscribed = false;
        private static Quaternion _oldRot;
        private static List<Billboard> _billboards = new List<Billboard>();
        private static Transform _cam = null;

        [SerializeField] private bool _reverse = false;

        private void Start()
        {
            _billboards.Add(this);
            if (_cam != null)
            {
                transform.rotation = Quaternion.LookRotation(_reverse ? _cam.forward : -_cam.forward);
            }

            if (_subscribed == false)
            {
                _subscribed = true;
                PinouUtils.MonoBehaviour.Update_Subscribe(OnUpdate);
            }
        }

        private void OnUpdate()
        {
            if (_cam == null)
            {
                _cam = FindCamera();
            }
            else
            {
                if (_cam.rotation != _oldRot)
                {
                    _oldRot = _cam.rotation;

                    for (int i = _billboards.Count - 1; i >= 0; i--)
                    {
                        if (_billboards[i] == null)
                        {
                            _billboards.RemoveAt(i);
                        }
                        else
                        {
                            _billboards[i].transform.rotation = Quaternion.LookRotation(_billboards[i]._reverse ? _cam.forward : -_cam.forward);
                        }
                    }
                }
            }
        }

        protected virtual Transform FindCamera()
        {
            return Camera.main.transform;
        }
    }
}
