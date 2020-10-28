#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.InputSystem
{
	public class PinouLinkedInputReceiver : PinouBehaviour
	{
		public enum LinkingMethod
		{
			HardReference,
			PlayerInputReceiver
		}
		[Header("Linking Parameters")]
		[Space]
		[SerializeField] private LinkingMethod _linkingMethod;
		[SerializeField, ShowIf("_linkingMethod", LinkingMethod.HardReference)] private PinouInputReceiver _hardReference;
		[SerializeField, ShowIf("_linkingMethod", LinkingMethod.PlayerInputReceiver)] private int _playerID;

		private PinouInputReceiver _currentReceiver = null;

		public PinouInputReceiver InputReceiver
		{
			get
			{
				switch (_linkingMethod)
				{
					case LinkingMethod.HardReference:
						return _hardReference;
					case LinkingMethod.PlayerInputReceiver:
						return _currentReceiver;
				}
				return null;
			}
		}
		public bool IsLinked => InputReceiver != null;
		public bool IsFocused => IsLinked ? InputReceiver.IsFocused : false;

		private void Update()
		{
			if (IsLinked == false)
			{
				switch (_linkingMethod)
				{
					case LinkingMethod.PlayerInputReceiver:
						PinouPlayer player = PinouApp.Player.GetPlayerByID(_playerID);
						if (player != null && player.Focuses.Length > 0)
						{
							_currentReceiver = player.Focuses[0];
						}
						break;
				}
			}
		}
	}
}