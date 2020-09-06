#pragma warning disable 0649
using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
    [CreateAssetMenu(fileName = "SceneInfosData", menuName = "Pinou/AutoScript Configs/SceneInfosData", order = 1000)]
	public class PinouSceneInfosData : ScriptableObject
	{
		public interface ISubHolder
		{
			string HolderName { get; }
			ICollection<ISubHolder> SubHolders { get; }
		}
		[System.Serializable]
		public class SubSubSubSubSceneHolderParams : ISubHolder
		{
			[SerializeField] private string _holderName;
			public string HolderName => _holderName;
			public ICollection<ISubHolder> SubHolders => null;
		}
		[System.Serializable]
		public class SubSubSubSceneHolderParams : ISubHolder
		{
			[SerializeField] private string _holderName;
			public string HolderName => _holderName;
			[SerializeField] private SubSubSubSubSceneHolderParams[] _subHolders;
			public ICollection<ISubHolder> SubHolders => _subHolders;
		}
		[System.Serializable]
		public class SubSubSceneHolderParams : ISubHolder
		{
			[SerializeField] private string _holderName;
			public string HolderName => _holderName;
			[SerializeField] private SubSubSubSceneHolderParams[] _subHolders;
			public ICollection<ISubHolder> SubHolders => _subHolders;
		}
		[System.Serializable]
		public class SubSceneHolderParams : ISubHolder
		{
			[SerializeField] private string _holderName;
			public string HolderName => _holderName;
			[SerializeField] private SubSubSceneHolderParams[] _subHolders;
			public ICollection<ISubHolder> SubHolders => _subHolders;
		}
		[System.Serializable]
		public class SceneHolderParams : ISubHolder
		{
			[SerializeField] private string _holderName;
			public string HolderName => _holderName;
			[SerializeField] private SubSceneHolderParams[] _subHolders;
			public ICollection<ISubHolder> SubHolders => _subHolders;
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