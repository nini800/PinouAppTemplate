using UnityEngine;

namespace Pinou.InputSystem
{
	public partial class PinouInputReceiver : PinouBehaviour
	{
		public bool Game_ShowPauseMenu => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_ShowPauseMenu) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_ShowPauseMenu)) : false;
		public bool Game_Shoot => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_Shoot) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_Shoot)) : false;
		public bool Editor_CameraLook => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Editor_CameraLook) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Editor_CameraLook)) : false;
		public bool Editor_CameraSprint => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Editor_CameraSprint) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Editor_CameraSprint)) : false;
		public bool Editor_CameraUp => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Editor_CameraUp) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Editor_CameraUp)) : false;
		public bool Editor_CameraDown => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Editor_CameraDown) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Editor_CameraDown)) : false;
		public bool Menu_GoRight => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_GoRight) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_GoRight)) : false;
		public bool Menu_GoLeft => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_GoLeft) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_GoLeft)) : false;
		public bool Menu_GoUp => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_GoUp) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_GoUp)) : false;
		public bool Menu_GoDown => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_GoDown) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_GoDown)) : false;
		public bool Menu_Accept => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_Accept) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_Accept)) : false;
		public bool Menu_Return => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_Return) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_Return)) : false;
		public bool Menu_PlayerJoin => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_PlayerJoin) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_PlayerJoin)) : false;
		public bool Menu_PlayerLeave => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_PlayerLeave) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_PlayerLeave)) : false;
		public bool Menu_PlayerReadyStart => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_PlayerReadyStart) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_PlayerReadyStart)) : false;
		public bool Menu_PlayerNotReadyReturn => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Menu_PlayerNotReadyReturn) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Menu_PlayerNotReadyReturn)) : false;
		public float Game_Zoom => IsFocused ? (AbsoluteFocus ? 0f : PinouApp.Input.GameAxisValue(_focusingPlayer.ControllerId, PinouAxis.Game_Zoom)) : 0f;
		public Vector2 Game_Move => IsFocused ? (AbsoluteFocus ? Vector2.zero : PinouApp.Input.GameAxis2DValue(_focusingPlayer.ControllerId, Pinou2DAxis.Game_Move)) : Vector2.zero;
		public Vector2 Editor_CameraMove => IsFocused ? (AbsoluteFocus ? Vector2.zero : PinouApp.Input.GameAxis2DValue(_focusingPlayer.ControllerId, Pinou2DAxis.Editor_CameraMove)) : Vector2.zero;
		public Vector2 Editor_CameraAim => IsFocused ? (AbsoluteFocus ? Vector2.zero : PinouApp.Input.GameAxis2DValue(_focusingPlayer.ControllerId, Pinou2DAxis.Editor_CameraAim)) : Vector2.zero;
	}
}