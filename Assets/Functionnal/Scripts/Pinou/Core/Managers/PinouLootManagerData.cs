#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pinou.EntitySystem;

namespace Pinou
{
	[CreateAssetMenu(fileName = "LootData", menuName = "Pinou/Managers/LootData", order = 1000)]
	public class PinouLootManagerData : PinouManagerData
	{
		#region Enums
		public enum DropPhysicsMode
		{
			Simulated,
			Rigidbody
		}
		#endregion

		#region Fields, Getters
		[SerializeField] private DropPhysicsMode _physicsMode;
		[SerializeField] private float _lootShareDistance = 50f;

		public DropPhysicsMode PhysicsMode => _physicsMode;
		public float LootShareDistance => _lootShareDistance;
		#endregion

		#region Core
		public override PinouManager BuildManagerInstance()
		{
			return new PinouLootManager(this);
		}
		#endregion

		public class PinouLootManager : PinouManager
		{
			#region Constructor
			public PinouLootManager(PinouLootManagerData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouLootManagerData Data { get; private set; }
			#endregion

			#region Vars, Getters
			protected List<EntityDrop> entityDrops = new List<EntityDrop>();
			protected List<Transform> dropTransforms = new List<Transform>();
			protected List<Vector3> dropVelocities = new List<Vector3>();
			protected List<Vector3> dropAngularVelocities = new List<Vector3>();
			protected List<float> dropAngularVelocities2D = new List<float>();

			private float _nextSimulatedCheckTime = -1 / 0f;
			#endregion

			#region Behaviour
			public override void SlaveStart()
			{
				PinouApp.Entity.OnEntityDeath.SafeSubscribe(OnEntityDeath);
			}

			public override void SlaveFixedUpdate()
			{
				if (Data._physicsMode == DropPhysicsMode.Simulated)
				{
					if (PinouApp.Entity.Mode2D == true)
					{
						HandleSimulatedDrops2D();
					}
					else
					{
						HandleSimulatedDrops3D();
					}
				}
			}
			private void HandleSimulatedDrops2D()
			{
				for (int i = dropTransforms.Count - 1; i >= 0; i--)
				{
					float dragStiffnessFactor = 1;
					if (PinouApp.Entity.Player != null)
					{
						Vector3 toPlayer = PinouApp.Entity.Player.Position - dropTransforms[i].position;
						float toPlayerSqrMagn = (toPlayer).sqrMagnitude;
						if (toPlayerSqrMagn <= entityDrops[i].Data.PickupDistance * entityDrops[i].Data.PickupDistance)
						{
							if (toPlayerSqrMagn == 0)
							{
								toPlayer = Vector3.zero;
							}
							else
							{
								toPlayer = toPlayer / Mathf.Sqrt(toPlayerSqrMagn) * entityDrops[i].Data.PickupSpeed;
							}
							dropVelocities[i] = Vector3.MoveTowards(dropVelocities[i], toPlayer, Time.fixedDeltaTime * entityDrops[i].Data.PickupStiffness * entityDrops[i].StiffnessProgress);
							dragStiffnessFactor = entityDrops[i].StiffnessProgress.OneMinus();
						}
					}

					dropTransforms[i].position += dropVelocities[i];
					dropTransforms[i].eulerAngles = dropTransforms[i].eulerAngles.AddZ(dropAngularVelocities2D[i]);

					dropVelocities[i] = dropVelocities[i].SubtractMagnitude(entityDrops[i].Data.VelocityDrag * Time.fixedDeltaTime * dragStiffnessFactor);
					dropAngularVelocities2D[i] = dropAngularVelocities2D[i].SubtractAbsolute(entityDrops[i].Data.AngularDrag * Time.fixedDeltaTime * dragStiffnessFactor);
				}

				if (Time.time > _nextSimulatedCheckTime)
				{
					for (int i = dropTransforms.Count - 1; i >= 0; i--)
					{
						if (dropVelocities[i].sqrMagnitude <= 0f && dropAngularVelocities2D[i] <= 0f)
						{
							StopSimulatingDrop(entityDrops[i]);
						}
					}

					_nextSimulatedCheckTime = Time.time + 1f;
				}
			}
			private void HandleSimulatedDrops3D()
			{
				for (int i = dropTransforms.Count - 1; i >= 0; i--)
				{
					dropTransforms[i].position += dropVelocities[i];
					dropTransforms[i].eulerAngles += dropAngularVelocities[i];

					dropVelocities[i] = dropVelocities[i].SubtractMagnitude(Time.fixedDeltaTime);
					dropAngularVelocities[i] = dropAngularVelocities[i].SubtractMagnitude(Time.fixedDeltaTime);
				}

				if (Time.time > _nextSimulatedCheckTime)
				{
					for (int i = dropTransforms.Count - 1; i >= 0; i--)
					{
						if (dropVelocities[i].sqrMagnitude <= 0f && dropAngularVelocities[i].sqrMagnitude <= 0f)
						{
							StopSimulatingDrop(entityDrops[i]);
						}
					}

					_nextSimulatedCheckTime = Time.time + 1f;
				}
			}
			#endregion

			#region Utilities
			public void HandleSpawnDropsForPlayer(Entity killedEntity, Vector3 position, bool isLocalPlayerKiller)
			{
				if (PinouApp.Entity.Player == null || killedEntity.HasLoot == false) { return; }
				if ((PinouApp.Entity.Player.Position - position).sqrMagnitude > Data._lootShareDistance.Squared() && isLocalPlayerKiller == false) { return; }

				HandleSpawnDrops(killedEntity.Loot.ComputeLoot(), position);
			}
			public void HandleSpawnDrops(ICollection<EntityDropData> drops, Vector3 position)
			{
				foreach (EntityDropData drop in drops)
				{
					GameObject instance = PinouApp.Pooler.Retrieve(drop.DropModel, position, Quaternion.identity, PinouApp.Scene.ActiveSceneInfos.LootsHolder);
					EntityDrop dropInstance = instance.GetComponent<EntityDrop>();
					dropInstance.FillData(drop);

					entityDrops.Add(dropInstance);
					dropTransforms.Add(instance.transform);

					HandleBaseDropPhysics(dropInstance);

					dropInstance.OnCollect.Subscribe(OnDropCollected);
				}
			}
			private void HandleBaseDropPhysics(EntityDrop dropInstance)
			{
				Vector3 velocity;
				float angularVelocity2D = 0f;
				Vector3 angularVelocity = Vector3.zero;

				if (PinouApp.Entity.Mode2D == true)
				{
					float angle = Random.Range(0f, 2 * Mathf.PI);
					velocity = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
					velocity *= Random.Range(dropInstance.Data.MinDropHorizontalVelocity, dropInstance.Data.MaxDropHorizontalVelocity);

					angularVelocity2D = Random.Range(dropInstance.Data.MinDropAngularVelocity, dropInstance.Data.MaxDropAngularVelocity); ;
				}
				else
				{
					float angle = Random.Range(0f, 2 * Mathf.PI);
					velocity = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));
					velocity *= Random.Range(dropInstance.Data.MinDropHorizontalVelocity, dropInstance.Data.MaxDropHorizontalVelocity);
					velocity.y = Random.Range(dropInstance.Data.MinDropVerticalVelocity, dropInstance.Data.MaxDropVerticalVelocity);

					angularVelocity = Random.insideUnitSphere * Random.Range(dropInstance.Data.MinDropAngularVelocity, dropInstance.Data.MaxDropAngularVelocity);
				}

				switch (Data.PhysicsMode)
				{
					case DropPhysicsMode.Simulated:
						dropVelocities.Add(velocity * Time.fixedDeltaTime);

						if (PinouApp.Entity.Mode2D == true)
						{
							dropAngularVelocities2D.Add(angularVelocity2D * Time.fixedDeltaTime);
						}
						else
						{
							dropAngularVelocities.Add(angularVelocity * Time.fixedDeltaTime);
						}
						break;
					case DropPhysicsMode.Rigidbody:
						Rigidbody r = dropInstance.GetComponent<Rigidbody>();
						if (r == null) { return; }
						r.velocity = velocity;

						if (PinouApp.Entity.Mode2D == true)
						{
							r.angularVelocity = Vector3.forward * angularVelocity2D;
						}
						else
						{
							r.angularVelocity = angularVelocity;
						}
						break;
				}
			}

			public void RegisterDropIfNeeded(EntityDrop drop)
			{
				if (entityDrops.Contains(drop) == false)
				{
					entityDrops.Add(drop);
					dropTransforms.Add(drop.transform);

					if (Data.PhysicsMode == DropPhysicsMode.Simulated)
					{
						dropVelocities.Add(default);
						if (PinouApp.Entity.Mode2D == true)
						{
							dropAngularVelocities2D.Add(default);
						}
						else
						{
							dropAngularVelocities.Add(default);
						}
					}
				}
			}

			private void StopSimulatingDrop(EntityDrop drop)
			{
				int index = entityDrops.IndexOf(drop);
				if (index < 0) { return; }

				entityDrops.RemoveAt(index);
				dropTransforms.RemoveAt(index);

				if (Data.PhysicsMode == DropPhysicsMode.Simulated)
				{
					dropVelocities.RemoveAt(index);
					if (PinouApp.Entity.Mode2D == true)
					{
						dropAngularVelocities2D.RemoveAt(index);
					}
					else
					{
						dropAngularVelocities.RemoveAt(index);
					}
				}
			}

			private void OnDropCollected(EntityDrop drop, Entity collecter)
			{
				StopSimulatingDrop(drop);
			}
			#endregion

			#region Events
			private void OnEntityDeath(Entity ent, AbilityCastResult killingResult)
			{
				if (ent.HasLoot)
				{
					HandleSpawnDropsForPlayer(ent, ent.Position, killingResult.CastData.Caster == PinouApp.Entity.Player);
				}
			}
			#endregion
		}
	}
}