using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
	[CreateAssetMenu(fileName = "OptionsData", menuName = "Pinou/Managers/OptionsData", order = 1000)]
	public class PinouOptionsManagerData : PinouManagerData
	{
		public override PinouManager BuildManagerInstance()
		{
			return new PinouOptionsManager(this);
		}
		public class PinouOptionsManager : PinouManager
		{
			public PinouOptionsManager(PinouOptionsManagerData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouOptionsManagerData Data { get; private set; }

			private Dictionary<string, float> _floatOptions = new Dictionary<string, float>();

			public void SetFloatOption(string optionName, float value)
			{
				if (_floatOptions.ContainsKey(optionName))
				{
					_floatOptions[optionName] = value;
				}
				else
				{
					_floatOptions.Add(optionName, value);
				}

				OnFloatOptionUpdate.Invoke(optionName, value);
			}

			public float GetFloatOption(string optionName, float defaultValue)
			{
				if (_floatOptions.ContainsKey(optionName) == false)
				{
					return defaultValue;
				}
				else
				{
					return _floatOptions[optionName];
				}
			}

			public PinouUtils.Delegate.Action<string, float> OnFloatOptionUpdate { get; private set; } = new PinouUtils.Delegate.Action<string, float>();
		}
	}
}