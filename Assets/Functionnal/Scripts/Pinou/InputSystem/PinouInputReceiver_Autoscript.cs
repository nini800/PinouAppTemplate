using UnityEngine;

namespace Pinou.InputSystem
{
	public partial class PinouInputReceiver : PinouBehaviour
	{
		public bool Game_Shoot => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_Shoot) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_Shoot)) : false;
		public bool Game_ShootHeld => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_ShootHeld) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_ShootHeld)) : false;
		public bool Game_Pause => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_Pause) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_Pause)) : false;
		public bool Game_Dash => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_Dash) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_Dash)) : false;
		public bool Game_Sprint_Toggle => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_Sprint_Toggle) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_Sprint_Toggle)) : false;
		public bool Game_Sprint_Held => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_Sprint_Held) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_Sprint_Held)) : false;
		public bool Game_ShowPauseMenu => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_ShowPauseMenu) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_ShowPauseMenu)) : false;
		public bool Game_OpenInventory => IsFocused ? (AbsoluteFocus ? PinouApp.Input.GameInputPressed(PinouInput.Game_OpenInventory) : PinouApp.Input.GameInputPressed(_focusingPlayer.ControllerId, PinouInput.Game_OpenInventory)) : false;
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
		public Vector2 Game_Move => IsFocused ? (AbsoluteFocus ? Vector2.zero : PinouApp.Input.GameAxis2DValue(_focusingPlayer.ControllerId, Pinou2DAxis.Game_Move)) : Vector2.zero;
		public float Game_Zoom => IsFocused ? (AbsoluteFocus ? 0f : PinouApp.Input.GameAxisValue(_focusingPlayer.ControllerId, PinouAxis.Game_Zoom)) : 0f;
		public Vector2 Editor_CameraMove => IsFocused ? (AbsoluteFocus ? Vector2.zero : PinouApp.Input.GameAxis2DValue(_focusingPlayer.ControllerId, Pinou2DAxis.Editor_CameraMove)) : Vector2.zero;
		public Vector2 Editor_CameraAim => IsFocused ? (AbsoluteFocus ? Vector2.zero : PinouApp.Input.GameAxis2DValue(_focusingPlayer.ControllerId, Pinou2DAxis.Editor_CameraAim)) : Vector2.zero;
	
		
		public bool GetPinouInput(PinouInput input)
		{
			switch(input)
			{
				case PinouInput.Game_Shoot:
					return Game_Shoot;
				case PinouInput.Game_ShootHeld:
					return Game_ShootHeld;
				case PinouInput.Game_Pause:
					return Game_Pause;
				case PinouInput.Game_Dash:
					return Game_Dash;
				case PinouInput.Game_Sprint_Toggle:
					return Game_Sprint_Toggle;
				case PinouInput.Game_Sprint_Held:
					return Game_Sprint_Held;
				case PinouInput.Game_ShowPauseMenu:
					return Game_ShowPauseMenu;
				case PinouInput.Game_OpenInventory:
					return Game_OpenInventory;
				case PinouInput.Editor_CameraLook:
					return Editor_CameraLook;
				case PinouInput.Editor_CameraSprint:
					return Editor_CameraSprint;
				case PinouInput.Editor_CameraUp:
					return Editor_CameraUp;
				case PinouInput.Editor_CameraDown:
					return Editor_CameraDown;
				case PinouInput.Menu_GoRight:
					return Menu_GoRight;
				case PinouInput.Menu_GoLeft:
					return Menu_GoLeft;
				case PinouInput.Menu_GoUp:
					return Menu_GoUp;
				case PinouInput.Menu_GoDown:
					return Menu_GoDown;
				case PinouInput.Menu_Accept:
					return Menu_Accept;
				case PinouInput.Menu_Return:
					return Menu_Return;
				case PinouInput.Menu_PlayerJoin:
					return Menu_PlayerJoin;
				case PinouInput.Menu_PlayerLeave:
					return Menu_PlayerLeave;
				case PinouInput.Menu_PlayerReadyStart:
					return Menu_PlayerReadyStart;
				case PinouInput.Menu_PlayerNotReadyReturn:
					return Menu_PlayerNotReadyReturn;
			}
			
			throw new System.Exception("No Input " + input + " found.");
		}
		
		public float GetPinouAxis(PinouAxis axis)
		{
			switch(axis)
			{
				case PinouAxis.Game_Zoom:
					return Game_Zoom;

			}
			
			throw new System.Exception("No Axis " + axis + " found.");
		}
		
		public Vector2 GetPinouAxis2D(Pinou2DAxis axis)
		{
			switch(axis)
			{
				case Pinou2DAxis.Game_Move:
					return Game_Move;
				case Pinou2DAxis.Editor_CameraMove:
					return Editor_CameraMove;
				case Pinou2DAxis.Editor_CameraAim:
					return Editor_CameraAim;
			}
			
			throw new System.Exception("No Axis " + axis + " found.");
		}
	}
}