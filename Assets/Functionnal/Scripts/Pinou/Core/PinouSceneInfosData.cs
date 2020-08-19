#pragma warning disable 0649
using UnityEngine;

namespace Pinou
{
    [CreateAssetMenu(fileName = "SceneInfosData", menuName = "Pinou/SceneInfosData", order = 1000)]
	public class PinouSceneInfosData : ScriptableObject
	{
		[System.Serializable]
		public class SceneHolderParams
		{
			public string HolderName;
			public SceneHolderParams[] SubHolders;
		}
        [SerializeField] private string _autoScriptPath = "Assets\\Functionnal\\Scripts\\Pinou\\Core\\";
		[SerializeField] private SceneHolderParams[] _sceneHolders;
		public SceneHolderParams[] SceneHolders => _sceneHolders;

		private void OnValidate()
		{
			PinouAutoscript.UpdateSceneInfosAutoScript(_autoScriptPath, _sceneHolders);
		}
	}
}