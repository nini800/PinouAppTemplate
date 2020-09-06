using UnityEngine.Rendering;
using UnityEngine;
using UnityEditor;

namespace Pinou.Editor
{
    public static class Editor_SpriteRendererExtender
    {
		[MenuItem("CONTEXT/Renderer/OrderInLayer/Background")]
		public static void SetBackgroundLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, -100);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/Background")]
		public static void SetBackgroundLayer_PS(MenuCommand command) => UpdateLayerOrder(command, -100);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/Background")]
		public static void SetBackgroundLayer_SG(MenuCommand command) => UpdateLayerOrder(command, -100);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/ExperienceCircle")]
		public static void SetExperienceCircleLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 0);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/ExperienceCircle")]
		public static void SetExperienceCircleLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 0);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/ExperienceCircle")]
		public static void SetExperienceCircleLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 0);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/MovementsTrailsNParticles")]
		public static void SetMovementsTrailsNParticlesLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 10);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/MovementsTrailsNParticles")]
		public static void SetMovementsTrailsNParticlesLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 10);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/MovementsTrailsNParticles")]
		public static void SetMovementsTrailsNParticlesLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 10);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/Drops")]
		public static void SetDropsLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 15);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/Drops")]
		public static void SetDropsLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 15);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/Drops")]
		public static void SetDropsLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 15);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/Entity")]
		public static void SetEntityLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 20);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/Entity")]
		public static void SetEntityLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 20);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/Entity")]
		public static void SetEntityLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 20);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/UnderPlayer_FXs")]
		public static void SetUnderPlayer_FXsLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 25);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/UnderPlayer_FXs")]
		public static void SetUnderPlayer_FXsLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 25);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/UnderPlayer_FXs")]
		public static void SetUnderPlayer_FXsLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 25);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/Entity_Player")]
		public static void SetEntity_PlayerLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 30);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/Entity_Player")]
		public static void SetEntity_PlayerLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 30);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/Entity_Player")]
		public static void SetEntity_PlayerLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 30);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/HealthCircle")]
		public static void SetHealthCircleLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 100);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/HealthCircle")]
		public static void SetHealthCircleLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 100);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/HealthCircle")]
		public static void SetHealthCircleLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 100);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/FXs")]
		public static void SetFXsLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 125);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/FXs")]
		public static void SetFXsLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 125);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/FXs")]
		public static void SetFXsLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 125);

		[MenuItem("CONTEXT/Renderer/OrderInLayer/Abilities")]
		public static void SetAbilitiesLayer_Rend(MenuCommand command) => UpdateLayerOrder(command, 150);
		[MenuItem("CONTEXT/ParticleSystem/OrderInLayer/Abilities")]
		public static void SetAbilitiesLayer_PS(MenuCommand command) => UpdateLayerOrder(command, 150);
		[MenuItem("CONTEXT/SortingGroup/OrderInLayer/Abilities")]
		public static void SetAbilitiesLayer_SG(MenuCommand command) => UpdateLayerOrder(command, 150);



        private static void UpdateLayerOrder(MenuCommand command, int order)
        {
			if (command.context is Renderer)
			{
				Renderer rend = (Renderer)command.context;
				rend.sortingOrder = order;
			}
			else if (command.context is ParticleSystem)
			{
				ParticleSystem ps = (ParticleSystem)command.context;
				ParticleSystemRenderer rend = ps.GetComponent<ParticleSystemRenderer>();
				rend.sortingOrder = order;
			}
			else if (command.context is SortingGroup)
			{
				SortingGroup sg = (SortingGroup)command.context;
				sg.sortingOrder = order;
			}
        }
    }
}
