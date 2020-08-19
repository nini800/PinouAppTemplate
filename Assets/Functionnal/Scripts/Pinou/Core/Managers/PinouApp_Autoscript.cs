namespace Pinou
{
	public partial class PinouApp : PinouSingleton<PinouApp>
	{
		private static Pinou.PinouResourcesData.PinouResources _resources;
		public static Pinou.PinouResourcesData.PinouResources Resources=> _resources;
		private static Pinou.InputSystem.PinouPlayerManagerData.PinouPlayerManager _player;
		public static Pinou.InputSystem.PinouPlayerManagerData.PinouPlayerManager Player=> _player;
		private static Pinou.InputSystem.PinouInputManagerData.PinouInputManager _input;
		public static Pinou.InputSystem.PinouInputManagerData.PinouInputManager Input=> _input;
		private static Pinou.PinouSceneManagerData.PinouSceneManager _scene;
		public static Pinou.PinouSceneManagerData.PinouSceneManager Scene=> _scene;
		private static Pinou.PinouTimeManagerData.PinouTimeManager _time;
		public static Pinou.PinouTimeManagerData.PinouTimeManager Time=> _time;
		private static Pinou.Networking.PinouEntityManagerNetData.PinouEntityManagerNet _entity;
		public static Pinou.Networking.PinouEntityManagerNetData.PinouEntityManagerNet Entity=> _entity;
		private static Pinou.EntitySystem.PinouCheatsManagerData.PinouCheatsManager _cheats;
		public static Pinou.EntitySystem.PinouCheatsManagerData.PinouCheatsManager Cheats=> _cheats;
	}
}