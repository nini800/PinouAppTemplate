using UnityEngine;

namespace Pinou
{
    [CreateAssetMenu(fileName = "TimeData", menuName = "Pinou/TimeData", order = 1000)]
	public class PinouTimeManagerData : PinouManagerData
	{
		public override PinouManager BuildManagerInstance()
		{
			return new PinouTimeManager(this);
		}
		public class PinouTimeManager : PinouManager
		{
			public PinouTimeManager(PinouTimeManagerData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouTimeManagerData Data { get; private set; }

			private bool _gamePaused = false;
			public bool GamePaused => _gamePaused;
			public float CurrentTimeScale
			{
				get
				{
					if (_gamePaused == true)
					{
						return 0f;
					}

					return 1f;
				}
			}

			public override void SlaveUpdate()
			{
				Time.timeScale = CurrentTimeScale;
			}

			public void PauseGame()
			{
				_gamePaused = true;
			}
			public void ResumeGame()
			{
				_gamePaused = false;
			}
		}
	}
}