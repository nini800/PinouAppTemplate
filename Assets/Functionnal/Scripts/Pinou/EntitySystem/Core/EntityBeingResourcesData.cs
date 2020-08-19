#pragma warning disable 0649
using UnityEngine;
using Sirenix.OdinInspector;

namespace Pinou.EntitySystem
{
	[CreateAssetMenu(fileName = "EntityBeingResourcesData", menuName = "Pinou/Entity Being Resources Data", order = 1000)]
	public class EntityBeingResourcesData : SerializedScriptableObject
	{
        [SerializeField] private string _entityBeingDataPath;
        [SerializeField] private string _entityEnumsPath;
        [SerializeField] private string[] _beingBaseEnumLines;
        [SerializeField] private string[] _resourcesNames;

        private void OnValidate()
        {
            if(_resourcesNames.Length < 4 || 
               _resourcesNames[0] != "Health" ||
               _resourcesNames[1] != "Power" ||
               _resourcesNames[2] != "Stamina" ||
               _resourcesNames[3] != "Mana")
            {
                _resourcesNames = new string[]
                {
                    "Health",
                    "Power",
                    "Stamina",
                    "Mana"
                };
            }
            PinouAutoscript.UpdateBeingResourcesAutoScript(_entityBeingDataPath, _entityEnumsPath, _beingBaseEnumLines, _resourcesNames);
        }
    }
}