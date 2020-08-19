#pragma warning disable 0649
using UnityEngine;

namespace Pinou
{
    public class CollisionEventsTransmitter : MonoBehaviour
    {
        [SerializeField] private Transform _toTransmit;
        private void OnCollisionEnter(Collision collision)
        {
            _toTransmit.SendMessage("OnCollisionEnter", collision, SendMessageOptions.DontRequireReceiver);
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            _toTransmit.SendMessage("OnCollisionEnter2D", collision, SendMessageOptions.DontRequireReceiver);

        }
        private void OnCollisionExit(Collision collision)
        {
            _toTransmit.SendMessage("OnCollisionExit", collision, SendMessageOptions.DontRequireReceiver);

        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            _toTransmit.SendMessage("OnCollisionExit2D", collision, SendMessageOptions.DontRequireReceiver);

        }
        private void OnCollisionStay(Collision collision)
        {
            _toTransmit.SendMessage("OnCollisionStay", collision, SendMessageOptions.DontRequireReceiver);

        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            _toTransmit.SendMessage("OnCollisionStay2D", collision, SendMessageOptions.DontRequireReceiver);

        }
        private void OnTriggerEnter(Collider other)
        {
            _toTransmit.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _toTransmit.SendMessage("OnTriggerEnter2D", collision, SendMessageOptions.DontRequireReceiver);
        }
        private void OnTriggerExit(Collider other)
        {
            _toTransmit.SendMessage("OnTriggerExit", other, SendMessageOptions.DontRequireReceiver);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            _toTransmit.SendMessage("OnTriggerExit2D", collision, SendMessageOptions.DontRequireReceiver);
        }
        private void OnTriggerStay(Collider other)
        {
            _toTransmit.SendMessage("OnTriggerStay", other, SendMessageOptions.DontRequireReceiver);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            _toTransmit.SendMessage("OnTriggerStay2D", collision, SendMessageOptions.DontRequireReceiver);
        }

    }
}