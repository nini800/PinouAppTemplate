using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using Mirror;
using Pinou.Networking;
using System.Threading.Tasks;

namespace Pinou.EntitySystem
{
	public enum EntityCreationMethod
	{
		AroundPlayer,
		AroundPlayer2D_XYPlane
	}
	[CreateAssetMenu(fileName = "EntityData", menuName = "Pinou/Managers/EntityData", order = 1000)]
	public class PinouEntityManagerData : PinouManagerData
	{
		[Header("Main Parameters")]
		[Space]
		[SerializeField] private bool _Mode2D = false;

		[Header("Spawn Method Parameters")]
		[Space]
		[SerializeField] protected EntityCreationMethod entityCreationMethod = EntityCreationMethod.AroundPlayer;
		[SerializeField, ShowIf("entityCreationMethod", EntityCreationMethod.AroundPlayer2D_XYPlane)] protected Vector2 spawnDistanceRange2D = new Vector2(10f, 15f);

		[Header("Despawn Parameters")]
		[Space]
		[SerializeField] protected float enemyDespawnDistance = 100f;

		public EntityCreationMethod EntityCreationMethod => entityCreationMethod;

		public override PinouManager BuildManagerInstance()
		{
			return new PinouEntityManager(this);
		}

		public class PinouEntityManager : PinouManager
		{
			#region OnConstruct
			public PinouEntityManager(PinouEntityManagerData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouEntityManagerData Data { get; private set; }
			#endregion

			#region Vars, Getters
			protected List<Vector3> recentPlayerDeathPoses = new List<Vector3>();

			protected List<Entity> players = new List<Entity>();
			protected List<Entity> allies = new List<Entity>();
			protected List<Entity> enemies = new List<Entity>();

			public virtual Entity Player => players.Count > 0 ? players[0] : null;
			public Entity[] Players => players.ToArray();
			public Entity[] Allies => allies.ToArray();
			public Entity[] Enemies => enemies.ToArray();

			public bool Mode2D => Data._Mode2D;
			#endregion

			#region Behaviour
			public override void SlaveStart()
            {
				PinouApp.Scene.OnBeforeGameSceneUnload.Subscribe(OnBeforeGameSceneUnload);
				PinouApp.Scene.OnSceneLoadComplete.Subscribe(OnSceneLoadComplete);
				CheckForExistingEntities();
			}

			private void OnBeforeGameSceneUnload(Scene obj)
			{
				ClearEntities();
			}
			private void OnSceneLoadComplete(Scene obj)
			{
				CheckForExistingEntities();
			}

			private void CheckForExistingEntities()
            {
				if (PinouApp.Scene.GameSceneLoaded == false) { return; }

				GameObject[] _rootGobs = PinouApp.Scene.ActiveSceneInfos.gameObject.scene.GetRootGameObjects();
                for (int i = 0; i < _rootGobs.Length; i++)
                {
                    foreach (Entity entity in _rootGobs[i].GetComponentsInChildren<Entity>())
                    {
                        switch (entity.Team)
                        {
                            case EntityTeam.Players:
								players.Add(entity);
                                break;
							case EntityTeam.Allies:
								allies.Add(entity);
                                break;
							case EntityTeam.Enemies:
								enemies.Add(entity);
                                break;
						}

						SubscribeToEntityEvents(entity);
					}
				}
            }

			public override void SlaveUpdate()
			{
				HandleDespawnEnemies();
			}
			private void HandleDespawnEnemies()
			{
				if (PinouNetworkManager.MainBehaviour.isServer == false) { return; }
				for (int i = enemies.Count - 1; i >= 0; i--)
				{
					if (enemies[i] == null)
					{
						enemies.RemoveAt(i);
						continue;
					}
					bool shouldKill = true;
					//Check around all players
					for (int j = players.Count - 1; j >= 0; j--)
					{
						if (players[j] == null)
						{
							players.RemoveAt(j);
							continue;
						}
						if (Vector3.SqrMagnitude(players[j].Position - enemies[i].Position) <= Data.enemyDespawnDistance * Data.enemyDespawnDistance)
						{
							shouldKill = false;
							break;
						}
					}
					//Prevent removing monsters right after player death
					for (int j = 0; j < recentPlayerDeathPoses.Count; j++)
					{
						if (Vector3.SqrMagnitude(recentPlayerDeathPoses[j] - enemies[i].Position) <= Data.enemyDespawnDistance * Data.enemyDespawnDistance)
						{
							shouldKill = false;
							break;
						}
					}
					if (shouldKill == true)
					{
						enemies[i].OnRemovedByManager.Invoke(enemies[i]);
						Destroy(enemies[i].gameObject);
						enemies.RemoveAt(i);
					}
				}
			}
			#endregion

			#region Utilities
			private Vector3 GetAroundPlayerSpawnPosition(Entity player)
			{
				if (Data.entityCreationMethod == EntityCreationMethod.AroundPlayer2D_XYPlane)
				{
					float randAngle = Random.Range(0f, Mathf.PI * 2f);
					Vector2 offset = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle)) *
						Random.Range(Data.spawnDistanceRange2D.x, Data.spawnDistanceRange2D.y);

					return offset +player.Position2D;
				}
				else
				{
					throw new System.Exception("Not Implemented.");
				}
			}
			public Entity[] CreateEntityGroupAroundPlayer(PinouResourcesEntities.SpawnGroup group, Entity player)
			{
				return CreateEntityGroup(group, GetAroundPlayerSpawnPosition(player));
			}
			public Entity[] CreateEntityGroup(PinouResourcesEntities.SpawnGroup group, Vector3 center)
			{
				Entity[] ents = new Entity[group.Entities.Length];
				if (group.Entities.Length > 1)
				{
					if (Data.entityCreationMethod == EntityCreationMethod.AroundPlayer2D_XYPlane)
					{
						float randAngle = Random.Range(0f, 2 * Mathf.PI);
						for (int i = 0; i < group.Entities.Length; i++)
						{
							Vector2 angleVector = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle));
							Vector2 posOffset = angleVector * group.GroupRadius;

							ents[i] = CreateEntity(group.Entities[i], center.ToV2() + posOffset, UnityEngine.Random.Range(0f, 360f));

							randAngle += (2 * Mathf.PI) / group.Entities.Length;
						}
					}
					else
					{
						throw new System.Exception("Not Implemented.");
					}

					return ents;
				}
				else if (group.Entities.Length == 1)
				{
					return new Entity[] { CreateEntity(group.Entities[0], center, UnityEngine.Random.Range(0f, 360f)) };
				}

				return new Entity[0];
			}
			public Entity CreateEntityAroundPlayer(GameObject entityModel, Entity player, float startRotation)
			{
				return CreateEntity(entityModel, GetAroundPlayerSpawnPosition(player), startRotation);
			}
			public virtual Entity CreateEntity(GameObject entityModel, Vector3 pos, float startRotation)
			{
				GameObject entGo = PinouApp.Pooler.Retrieve(entityModel, pos, Quaternion.identity, PinouApp.Scene.ActiveSceneInfos.EntitiesHolder);
				Entity ent = entGo.GetComponent<Entity>();
				if (ent == null)
				{
					Destroy(entGo);
					Debug.LogError("Model has no entity script attached to it.");
					return null;
				}

				if (Mode2D)
				{
					ent.Rotation2D = startRotation;
				}
				else
				{
					ent.Rotation = Quaternion.Euler(0f, startRotation, 0f);
				}

				RegisterEntity(ent);

				return ent;
			}
			public virtual void RegisterEntity(Entity ent)
			{
				SubscribeToEntityEvents(ent);

				switch (ent.Team)
				{
					case EntityTeam.Players:
						players.Add(ent);
						OnPlayerCreated.Invoke(ent);
						break;
					case EntityTeam.Allies:
						allies.Add(ent);
						break;
					case EntityTeam.Enemies:
						enemies.Add(ent);
						OnEnemyCreated.Invoke(ent);
						break;
				}
				OnEntityCreated.Invoke(ent);
			}
			public void DestroyEntity(Entity ent)
			{
				switch (ent.Team)
				{
					case EntityTeam.Players:
						players.Remove(ent);
						break;
					case EntityTeam.Allies:
						allies.Remove(ent);
						break;
					case EntityTeam.Enemies:
						enemies.Remove(ent);
						break;
				}
				Destroy(ent.gameObject);
			}
			public void ClearEntities()
			{
				players.Clear();
				allies.Clear();
				enemies.Clear();
			}

			private void SubscribeToEntityEvents(Entity ent)
			{
				ent.OnAbilityHitResult.Subscribe(OnEntityAbilityHitResult.Invoke);
				if (ent.HasBeing)
				{
					ent.Being.OnDeath.Subscribe(_OnEntityDeath);
				}
			}

			protected async virtual void HandleRespawnPlayer(Entity ent)
			{
				await Task.Delay(5000);
				CreateEntity(PinouApp.Resources.Data.Entities.PlayerModel, default, default);
			}
			#endregion

			#region Events
			public PinouUtils.Delegate.Action<Entity> OnPlayerCreated { get; private set; } = new PinouUtils.Delegate.Action<Entity>();
			public PinouUtils.Delegate.Action<Entity> OnEnemyCreated { get; private set; } = new PinouUtils.Delegate.Action<Entity>();
			public PinouUtils.Delegate.Action<Entity> OnEntityCreated { get; private set; } = new PinouUtils.Delegate.Action<Entity>();
			public PinouUtils.Delegate.Action<Entity, AbilityCastResult> OnEntityAbilityHitResult { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastResult>();
			public PinouUtils.Delegate.Action<Entity, AbilityCastResult> OnEntityDeath { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastResult>();

			protected virtual void _OnEntityDeath(Entity ent, AbilityCastResult killingResult)
			{
				switch (ent.Team)
				{
					case EntityTeam.Players:
						players.Remove(ent);
						HandleRespawnPlayer(ent);
						Vector3 pos = ent.Position;
						recentPlayerDeathPoses.Add(pos);
						PinouUtils.Coroutine.Invoke(delegate() { recentPlayerDeathPoses.Remove(pos); }, 10f, true);
						break;
					case EntityTeam.Allies:
						allies.Remove(ent);
						break;
					case EntityTeam.Enemies:
						enemies.Remove(ent);
						break;
				}
				OnEntityDeath.Invoke(ent, killingResult);
			}
			#endregion

		}
	}
}