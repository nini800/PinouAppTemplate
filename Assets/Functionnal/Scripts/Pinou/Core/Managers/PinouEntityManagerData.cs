using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

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
			private List<Entity> _players = new List<Entity>();
			private List<Entity> _allies = new List<Entity>();
			private List<Entity> _enemies = new List<Entity>();

			public Entity Player => _players.Count > 0 ? _players[0] : null;
			public Entity[] Players => _players.ToArray();
			public Entity[] Allies => _allies.ToArray();
			public Entity[] Enemies => _enemies.ToArray();

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
								_players.Add(entity);
                                break;
							case EntityTeam.Allies:
								_allies.Add(entity);
                                break;
							case EntityTeam.Enemies:
								_enemies.Add(entity);
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
				for (int i = _enemies.Count - 1; i >= 0; i--)
				{
					if (_enemies[i] == null)
					{
						_enemies.RemoveAt(i);
						continue;
					}
					bool shouldKill = true;
					for (int j = _players.Count - 1; j >= 0; j--)
					{
						if (_players[j] == null)
						{
							_players.RemoveAt(j);
							continue;
						}
						if (Vector3.SqrMagnitude(_players[j].Position - _enemies[i].Position) <= Data.enemyDespawnDistance * Data.enemyDespawnDistance)
						{
							shouldKill = false;
							break;
						}
					}
					if (shouldKill == true)
					{
						_enemies[i].OnRemovedByManager.Invoke(_enemies[i]);
						Destroy(_enemies[i].gameObject);
						_enemies.RemoveAt(i);
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
					return 
						new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle)) *
						Random.Range(Data.spawnDistanceRange2D.x, Data.spawnDistanceRange2D.y) +
						player.Position2D;
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
				GameObject entGo = Instantiate(entityModel, pos, Quaternion.identity, PinouApp.Scene.ActiveSceneInfos.EntitiesHolder);
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
			public void RegisterEntity(Entity ent)
			{
				SubscribeToEntityEvents(ent);

				switch (ent.Team)
				{
					case EntityTeam.Players:
						_players.Add(ent);
						OnPlayerCreated.Invoke(ent);
						break;
					case EntityTeam.Allies:
						_allies.Add(ent);
						break;
					case EntityTeam.Enemies:
						_enemies.Add(ent);
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
						_players.Remove(ent);
						break;
					case EntityTeam.Allies:
						_allies.Remove(ent);
						break;
					case EntityTeam.Enemies:
						_enemies.Remove(ent);
						break;
				}
				Destroy(ent.gameObject);
			}
			public void ClearEntities()
			{
				_players.Clear();
				_allies.Clear();
				_enemies.Clear();
			}

			private void SubscribeToEntityEvents(Entity ent)
			{
				if (ent.HasBeing)
				{
					ent.Being.OnDeath.Subscribe(_OnEntityDeath);
				}
			}

			private void _OnEntityDeath(Entity ent, AbilityCastResult killingResult)
			{
				switch (ent.Team)
				{
					case EntityTeam.Players:
						_players.Remove(ent);
						break;
					case EntityTeam.Allies:
						_allies.Remove(ent);
						break;
					case EntityTeam.Enemies:
						_enemies.Remove(ent);
						break;
				}
				OnEntityDeath.Invoke(ent, killingResult);
			}


			#endregion

			#region Events
			public PinouUtils.Delegate.Action<Entity> OnPlayerCreated { get; private set; } = new PinouUtils.Delegate.Action<Entity>();
			public PinouUtils.Delegate.Action<Entity> OnEnemyCreated { get; private set; } = new PinouUtils.Delegate.Action<Entity>();
			public PinouUtils.Delegate.Action<Entity> OnEntityCreated { get; private set; } = new PinouUtils.Delegate.Action<Entity>();
			public PinouUtils.Delegate.Action<Entity, AbilityCastResult> OnEntityDeath { get; private set; } = new PinouUtils.Delegate.Action<Entity, AbilityCastResult>();
			#endregion

		}
	}
}