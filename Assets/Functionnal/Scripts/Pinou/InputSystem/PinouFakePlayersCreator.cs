#pragma warning disable 0649
using UnityEngine;

namespace Pinou
{
	public class PinouFakePlayersCreator : PinouBehaviour
	{
		[System.Serializable]
		public class FakePlayerParams
		{
			public int ControllerID = 0;
			public string Name = "Player";
		}

		[SerializeField] private bool _logCreation = false;
		[SerializeField] private FakePlayerParams[] _fakePlayersToCreate;

		protected override void OnAwake()
		{
			if (enabled == false)
			{
				return;
			}
			for (int i = 0; i < _fakePlayersToCreate.Length; i++)
			{
				PinouApp.Player.CreatePlayer(_fakePlayersToCreate[i].Name, _fakePlayersToCreate[i].ControllerID);
			}
#if UNITY_EDITOR
			if (_logCreation == true)
            {
				Debug.Log("Fake players created.");
			}
#endif
		}

	}
}