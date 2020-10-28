#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.UI
{
	public class UI_OptionSlider : UI_PinouBehaviour
	{
		[SerializeField] private string _gameOptionName;

		public void UpdateFloatGameOption(float value)
		{
			PinouApp.Options.SetFloatOption(_gameOptionName, value);
		}
	}
}