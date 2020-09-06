using UnityEngine;
using System.Collections.Generic;
using Pinou.Editor;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou.InputSystem
{
	public enum FocusReceiveMode
	{
		Absolute,
		Player
	}
	public partial class PinouInputReceiver : PinouBehaviour
	{
		#region Data Class
		public struct FocusReceiveParameters
		{
			public FocusReceiveParameters(FocusReceiveMode focusMode, int playerID)
			{
				FocusMode = focusMode;
				PlayerID = playerID;
			}
			public FocusReceiveMode FocusMode;
			public int PlayerID;
		}
		#endregion

		#region Statics
		private static List<PinouInputReceiver> _absoluteFocused = new List<PinouInputReceiver>();
		private static List<PinouInputReceiver> _playerFocused = new List<PinouInputReceiver>();

		private static Dictionary<PinouInputReceiver, FocusReceiveParameters> _storedInputReceivers = new Dictionary<PinouInputReceiver, FocusReceiveParameters>();
        #endregion

        #region Attributes, Accessors & Mutators
        [SerializeField] private bool _absoluteFocusByDefault = false;
		[SerializeField, HideInInspector] private FocusMode _absoluteFocusMode = FocusMode.Additive;
        [SerializeField, HideInInspector] private int _playerFocusByDefault = -1;
		[SerializeField, HideInInspector] private FocusMode _playerFocusMode = FocusMode.Additive;

		private FocusReceiveMode _focusReceiveMode = FocusReceiveMode.Absolute;

		public FocusReceiveMode FocusReceiveMode => _focusReceiveMode;

		public bool AbsoluteFocus => _focusReceiveMode == FocusReceiveMode.Absolute;
		public bool PlayerFocus => _focusReceiveMode == FocusReceiveMode.Player;
		public bool AbsoluteFocused => _absoluteFocused.Contains(this);

		private PinouPlayer _focusingPlayer = null;
		public PinouPlayer FocusingPlayer => _focusingPlayer;

		public bool IsFocused => PlayerFocus ? _focusingPlayer != null : AbsoluteFocused == true;
		#endregion

		#region Behaviour
		protected override void OnSafeStart()
		{
			if (_absoluteFocusByDefault == true)
			{
				ReceiveAbsoluteFocus(_absoluteFocusMode);
			}
			else if (_playerFocusByDefault > -1)
			{
				PinouPlayer player = PinouApp.Player.GetPlayerByID(_playerFocusByDefault);
				if (player != null)
				{
					player.Focus(this, _playerFocusMode);
				}
			}
		}
		#endregion

		#region Utilities
		public void ReceiveAbsoluteFocus(FocusMode mode = FocusMode.Additive)
		{
			if (mode == FocusMode.Exclusive)
			{
				_absoluteFocused.Clear();
			}
			_focusReceiveMode = FocusReceiveMode.Absolute;
			if (_absoluteFocused.Contains(this) == false)
			{
				_absoluteFocused.Add(this);
				OnReceiveAbsoluteFocus.Invoke(this);
			}
		}
		public void LooseAbsoluteFocus()
		{
			if (_absoluteFocused.Contains(this) == true)
			{
				_absoluteFocused.Remove(this);
				OnLooseAbsoluteFocus.Invoke(this);
			}
		}
		public void ReceiveFocus(PinouPlayer player)
		{
			_focusReceiveMode = FocusReceiveMode.Player;
			if (_focusingPlayer != null)
			{
				_focusingPlayer.RemoveFocus(this);
			}

			if (_focusingPlayer != player)
			{
				_focusingPlayer = player;
				OnReceivePlayerFocus.Invoke(this, player);
			}

			if (_playerFocused.Contains(this) == false)
			{
				_playerFocused.Add(this);
			}
		}
		public void LooseFocus(PinouPlayer player)
		{
			if (_focusingPlayer == player)
			{
				_focusingPlayer = null;
				OnLoosePlayerFocus.Invoke(this, player);
			}

			if (player.IsFocused(this) == true)
			{
				player.RemoveFocus(this);
			}

			if (_playerFocused.Contains(this) == true)
			{
				_playerFocused.Remove(this);
			}
		}

		public void StoreCurrentInputReceivers()
		{
			for (int i = _absoluteFocused.Count - 1; i >= 0; i--)
			{
				_storedInputReceivers.Add(_absoluteFocused[i], new FocusReceiveParameters(FocusReceiveMode.Absolute, -1));
				_absoluteFocused[i].LooseAbsoluteFocus();
			}

			for (int i = _playerFocused.Count - 1; i >= 0; i--)
			{
				_storedInputReceivers.Add(_playerFocused[i], new FocusReceiveParameters(FocusReceiveMode.Player, _playerFocused[i].FocusingPlayer.PlayerID));
				_playerFocused[i].LooseFocus(_playerFocused[i].FocusingPlayer);
			}
		}

		public void RetrieveStoredInputReceivers()
		{
			foreach (PinouInputReceiver ir in _storedInputReceivers.Keys)
			{
				FocusReceiveParameters frp = _storedInputReceivers[ir];
				switch (frp.FocusMode)
				{
					case FocusReceiveMode.Absolute:
						ir.ReceiveAbsoluteFocus();
						break;
					case FocusReceiveMode.Player:
						ir.ReceiveFocus(PinouApp.Player.GetPlayerByID(frp.PlayerID));
						break;
				}
			}

			_storedInputReceivers.Clear();
		}
		#endregion

		#region Events
		public PinouUtils.Delegate.Action<PinouInputReceiver> OnReceiveAbsoluteFocus { get; private set; } = new PinouUtils.Delegate.Action<PinouInputReceiver>();
		public PinouUtils.Delegate.Action<PinouInputReceiver> OnLooseAbsoluteFocus { get; private set; } = new PinouUtils.Delegate.Action<PinouInputReceiver>();
		public PinouUtils.Delegate.Action<PinouInputReceiver, PinouPlayer> OnReceivePlayerFocus { get; private set; } = new PinouUtils.Delegate.Action<PinouInputReceiver, PinouPlayer>();
		public PinouUtils.Delegate.Action<PinouInputReceiver, PinouPlayer> OnLoosePlayerFocus { get; private set; } = new PinouUtils.Delegate.Action<PinouInputReceiver, PinouPlayer>();
		#endregion

		#region Editor
#if UNITY_EDITOR
		[CustomEditor(typeof(PinouInputReceiver), true)]
		public class PinouInputReceiverEditor : PinouEditor
		{
			protected override void InspectorGUI()
			{
				base.InspectorGUI();
				if (((PinouInputReceiver)target)._absoluteFocusByDefault == false)
				{
					PropField("_playerFocusByDefault");
					PropField("_playerFocusMode");
				}
				else
				{
					PropField("_absoluteFocusMode");
				}
				GUILayout.Space(10f);
				GUI.enabled = false;
				EditorGUILayout.EnumPopup("Focus Receive Mode", ((PinouInputReceiver)target).FocusReceiveMode);
				EditorGUILayout.Toggle("Is Focused", ((PinouInputReceiver)target).IsFocused);
				GUI.enabled = true;
			}
		}

#endif
		#endregion
	}


}