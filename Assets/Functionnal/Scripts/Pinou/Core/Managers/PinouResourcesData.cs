﻿#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pinou.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
    [CreateAssetMenu(fileName = "PinouResourcesData", menuName = "Pinou/Managers/ResourcesData", order = 1000)]
	public class PinouResourcesData : PinouManagerData
	{
		[SerializeField] private PinouResourcesLayers _layers;
		[SerializeField] private PinouResourcesMaterials _materials;
		[SerializeField] private PinouResourcesEntities _entities;
		[SerializeField] private PinouResourcesDatabases _databases;
		public PinouResourcesLayers Layers => _layers;
		public PinouResourcesMaterials Materials => _materials;
		public PinouResourcesEntities Entities => _entities;
		public PinouResourcesDatabases Databases => _databases;

		public override PinouManager BuildManagerInstance()
		{
			return new PinouResources(this);
		}

		public class PinouResources : PinouManager
		{
			public PinouResources(PinouResourcesData dataRef) : base(dataRef)
			{
				Data = dataRef;
			}
			public new PinouResourcesData Data { get; private set; }
		}
	}
}