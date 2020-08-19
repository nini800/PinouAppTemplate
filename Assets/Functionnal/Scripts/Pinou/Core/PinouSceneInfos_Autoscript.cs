#pragma warning disable 1522
using UnityEngine;

namespace Pinou
{
	public enum SceneHolder
	{
		Level,
		Statics,
		Interactables,
		Systems,
		PostProcessing,
		Lights,
		Entities,
		FXs,
	}
	
	public partial class PinouSceneInfos
	{
		[Header("Scene Holders")]
		[Space]
		[SerializeField] private Transform _levelHolder;
		public Transform LevelHolder => _levelHolder;
		[SerializeField] private Transform _staticsHolder;
		public Transform StaticsHolder => _staticsHolder;
		[SerializeField] private Transform _interactablesHolder;
		public Transform InteractablesHolder => _interactablesHolder;
		[SerializeField] private Transform _systemsHolder;
		public Transform SystemsHolder => _systemsHolder;
		[SerializeField] private Transform _postprocessingHolder;
		public Transform PostProcessingHolder => _postprocessingHolder;
		[SerializeField] private Transform _lightsHolder;
		public Transform LightsHolder => _lightsHolder;
		[SerializeField] private Transform _entitiesHolder;
		public Transform EntitiesHolder => _entitiesHolder;
		[SerializeField] private Transform _fxsHolder;
		public Transform FXsHolder => _fxsHolder;
	
		public Transform GetHolder(SceneHolder holder)
		{
			switch(holder)
			{
				case SceneHolder.Level:
					return _levelHolder;
				case SceneHolder.Statics:
					return _staticsHolder;
				case SceneHolder.Interactables:
					return _interactablesHolder;
				case SceneHolder.Systems:
					return _systemsHolder;
				case SceneHolder.PostProcessing:
					return _postprocessingHolder;
				case SceneHolder.Lights:
					return _lightsHolder;
				case SceneHolder.Entities:
					return _entitiesHolder;
				case SceneHolder.FXs:
					return _fxsHolder;
			}
			return null;
		}
    }
}