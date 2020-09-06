#pragma warning disable 0649, 0414
using Mirror;
using Pinou.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinou.Networking
{
	public class Net_InputReceiver : NetworkBehaviour
	{
		[SerializeField] private PinouInputReceiver _ir;
		[SerializeField] private bool _receiveFocusOnAuthorityStart = true;
		[SerializeField] private bool _receiveFocusOnEnableIfAuthority = true;
		[SerializeField] private bool _looseFocusOnDisableIfAuthority = true;

		public override void OnStartAuthority()
		{
			if (_receiveFocusOnAuthorityStart == true)
			{
				_ir?.ReceiveFocus(PinouApp.Player.GetPlayerByID(0));
			}
		}

		public override void OnStopAuthority()
		{
			if (_receiveFocusOnAuthorityStart == true)
			{
				_ir?.LooseFocus(PinouApp.Player.GetPlayerByID(0));
			}
		}

		private void OnEnable()
		{
			if (_receiveFocusOnEnableIfAuthority == true)
			{
				_ir?.ReceiveFocus(PinouApp.Player.GetPlayerByID(0));
			}
		}
		private void OnDisable()
		{
			if (_looseFocusOnDisableIfAuthority == true)
			{
				_ir?.LooseFocus(PinouApp.Player.GetPlayerByID(0));
			}
		}

		private void OnValidate()
		{
			if (_ir == null) { _ir = GetComponent<PinouInputReceiver>(); }
		}
	}
}