namespace Pinou.EntitySystem
{
	public abstract class FX_Base : PinouBehaviour
	{
		public abstract void StartFX(AbilityCastData castData);
		public abstract void StartFX(AbilityCastResult result);
	}
}