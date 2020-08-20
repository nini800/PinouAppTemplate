#pragma warning disable 0649, 0414
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Pinou.Editor
{
    [CreateAssetMenu(fileName = "Editor_SpriteRendererLayerOrdersData", menuName = "Editor/Editor_SpriteRendererLayerOrdersData", order = 1000)]
    public class Editor_SpriteRendererLayerOrdersData : SerializedScriptableObject
	{
        [System.Serializable]
        public class LayerOrderData
        {
            [SerializeField] private string _name;
            [SerializeField] private int _layerOrder;
            public string Name => _name;
            public int LayerOrder => _layerOrder;
        }

        [SerializeField] private string _autoScriptPath;
        [SerializeField] private LayerOrderData[] _orderDatas;
        public LayerOrderData[] Shortcuts { get => _orderDatas; }

		private void OnValidate()
		{
            string[] _names = new string[_orderDatas.Length];
            int[] _orders = new int[_orderDatas.Length];
			for (int i = 0; i < _orderDatas.Length; i++)
			{
                _names[i] = _orderDatas[i].Name;
                _orders[i] = _orderDatas[i].LayerOrder;
			}
            PinouAutoscript.E_UpdateSpriteRendererLayerOrderExtender(_autoScriptPath, _names, _orders);
		}
	}
}