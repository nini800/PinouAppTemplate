#pragma warning disable 0649
using UnityEngine;
using UnityEngine.SceneManagement;
using Pinou.EntitySystem;

namespace Pinou
{
    [RequireComponent(typeof(RectTransform))]
    public class UI_PinouBehaviour : PinouBehaviour
    {
        public Vector2 AnchoredPosition
        {
            get { return RectTransform.anchoredPosition; }
            set { RectTransform.anchoredPosition = value; }
        }

        public Vector2 SizeDelta
        {
            get { return RectTransform.sizeDelta; }
            set { RectTransform.sizeDelta = value; }
        }

        public new float Rotation
        {
            get { return RectTransform.eulerAngles.z; }
            set { RectTransform.eulerAngles = RectTransform.eulerAngles.SetZ(value); }
        }
        public new float LocalRotation
        {
            get { return RectTransform.localEulerAngles.z; }
            set { RectTransform.eulerAngles = RectTransform.localEulerAngles.SetZ(value); }
        }

        /// <summary>
        /// Need base.
        /// </summary>
        protected override void OnEnabled()
        {
            PinouApp.Scene.OnBeforeGameSceneUnload.SafeSubscribe(OnBeforeGameSceneUnload);
            PinouApp.Scene.OnSceneLoadComplete.SafeSubscribe(OnSceneLoadComplete);
            PinouApp.Entity.OnPlayerCreated.SafeSubscribe(OnPlayerCreated);

            if (PinouApp.Scene.GameSceneLoaded == true)
			{
                OnSceneLoadComplete(PinouApp.Scene.ActiveSceneInfos.gameObject.scene);
			}
            if (PinouApp.Entity.Player != null)
			{
                OnPlayerCreated(PinouApp.Entity.Player);
			}
        }
		protected override void OnDisabled()
		{
            PinouApp.Scene.OnBeforeGameSceneUnload.Unsubscribe(OnBeforeGameSceneUnload);
            PinouApp.Scene.OnSceneLoadComplete.Unsubscribe(OnSceneLoadComplete);
            PinouApp.Entity.OnPlayerCreated.Unsubscribe(OnPlayerCreated);
        }

		/// <summary>
		/// Need base
		/// </summary>
		/// <param name="obj"></param>
		protected virtual void OnSceneLoadComplete(Scene obj)
        {
            OnUIEnabled();
        }
        /// <summary>
        /// Need base
        /// </summary>
        /// <param name="obj"></param>
        protected virtual void OnBeforeGameSceneUnload(Scene obj)
        {
            OnUIDisabled();
        }
        /// <summary>
        /// Dont need base
        /// </summary>
        /// <param name="player"></param>
        protected virtual void OnPlayerCreated(Entity player)
		{

		}


        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void OnUIEnabled()
        {

        }

        /// <summary>
        /// Do not need base.
        /// </summary>
        protected virtual void OnUIDisabled()
        {

        }
    }
}