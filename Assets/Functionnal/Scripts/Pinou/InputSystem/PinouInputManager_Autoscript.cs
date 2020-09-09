#pragma warning disable 1522
namespace Pinou.InputSystem
{
	public enum PinouInput
	{
		Game_ShowPauseMenu,
		Game_Shoot,
		Game_ShootHeld,
		Game_Pause,
		Game_Dash,
		Game_Sprint_Toggle,
		Game_Sprint_Held,
		Editor_CameraLook,
		Editor_CameraSprint,
		Editor_CameraUp,
		Editor_CameraDown,
		Menu_GoRight,
		Menu_GoLeft,
		Menu_GoUp,
		Menu_GoDown,
		Menu_Accept,
		Menu_Return,
		Menu_PlayerJoin,
		Menu_PlayerLeave,
		Menu_PlayerReadyStart,
		Menu_PlayerNotReadyReturn
	}
	public enum PinouAxis
	{
		Game_Zoom = 1,

	}
	public enum Pinou2DAxis
	{
		Game_Move = 0,
		Editor_CameraMove = 2,
		Editor_CameraAim = 3
	}
/*	public partial class PinouInputManager
	{
		public static int GetInputParametersIndex(PinouInput input)
		{
			switch (input)
			{
|SWITCHINPUT|
			}
			throw new System.NotImplementedException();
		}
		public static int GetAxisParametersIndex(PinouAxis axis)
		{
			switch (axis)
			{
|SWITCHAXIS|
			}
			throw new System.NotImplementedException();
		}
		public static int Get2DAxisParametersIndex(Pinou2DAxis axis)
		{
			switch (axis)
			{
|SWITCHAXIS2D|
			}
			throw new System.NotImplementedException();
		}
	}*/
}