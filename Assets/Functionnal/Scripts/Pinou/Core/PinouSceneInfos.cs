#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;
using Pinou.Editor;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{

	public partial class PinouSceneInfos : PinouBehaviour
	{
		[ValidateInput("E_ValidateLoadingParts", "Must be a ISceneLoadingPart.", InfoMessageType.Error)] 
		[SerializeField] private PinouBehaviour[] _loadingParts;

		private bool _loadingComplete = false;
		public bool LoadingComplete => _loadingComplete;

		protected override void OnStart()
		{
			for (int i = 0; i < _loadingParts.Length; i++)
			{
				((ISceneLoadingPart)_loadingParts[i]).PerformLoadPart();
			}
			_loadingComplete = true;
			OnLoadingComplete.Invoke();
		}
		public PinouUtils.Delegate.Action OnLoadingComplete { get; private set; } = new PinouUtils.Delegate.Action();

        #region Editor
        private bool E_ValidateLoadingParts(PinouBehaviour[] behaviours)
		{
			bool toReturn = true;
			for (int i = 0; i < behaviours.Length; i++)
			{
				if (behaviours[i] as ISceneLoadingPart == null)
				{
					behaviours[i] = null;
					toReturn = false;
				}
			}
			return toReturn;
		}
#if UNITY_EDITOR


		#region Scene Structure
		[Button("Update Scene Structure")]
		private void E_UpdateSceneStructure()
		{
			PinouSceneInfosData data = Resources.Load<PinouSceneInfosData>("PinouSceneInfosData");
			Transform world = null;
			GameObject[] gobs = gameObject.scene.GetRootGameObjects();
			for (int i = 0; i < gobs.Length; i++)
			{
				if (gobs[i].name == "World")
				{
					world = gobs[i].transform;
					break;
				}
			}
			E_CreateIfNull(ref world, null, "World");

			for (int i = 0; i < data.SceneHolders.Length; i++)
			{
				E_StructureAllHolders(world, data.SceneHolders[i]);
			}
		}
		private void E_StructureAllHolders(Transform parent, PinouSceneInfosData.SceneHolderParams holder)
		{
			Transform newParent = E_StructureHolder(parent, holder);
			for (int i = 0; i < holder.SubHolders.Length; i++)
			{
				E_StructureAllHolders(newParent, holder.SubHolders[i]);
			}
		}
		private Transform E_StructureHolder(Transform parent, PinouSceneInfosData.SceneHolderParams holder)
		{
			Transform t = parent.Find(holder.HolderName);
			E_CreateIfNull(ref t, parent, holder.HolderName);

			string serializedRef = "_" + holder.HolderName.ToLower() + "Holder";
			var holderField = System.Type.GetType("Pinou.PinouSceneInfos").GetField(serializedRef, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.FlattenHierarchy);
			holderField.SetValue(this, t);

			return t;
		}

		private void E_CreateIfNull(ref Transform t, Transform parent, string name)
		{
			if (t == null)
			{
				GameObject newGo = new GameObject();
				newGo.name = name;
				newGo.transform.SetParent(parent);
				t = newGo.transform;
			}
		}
        #endregion
#endif
        #endregion
    }
}