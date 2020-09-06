#pragma warning disable 0649
using UnityEngine;
using Sirenix.OdinInspector;

namespace Pinou.Networking
{
    public enum SyncFrequency
	{
        Instant = 0,
        VeryShort = 1,
        Short = 2,
        Medium = 3,
        Long = 4,
        VeryLong = 5,
        Count = 6
	}
    public static class SyncFrequencyExtender
	{
        public static float GetFrequencyPeriod(this SyncFrequency freq)
		{
			switch (freq)
			{
				case SyncFrequency.Instant:
                    return 0f;
                case SyncFrequency.VeryShort:
                    return 0.05f;
                case SyncFrequency.Short:
                    return 0.1f;
                case SyncFrequency.Medium:
                    return 0.2f;
                case SyncFrequency.Long:
                    return 0.5f;
                case SyncFrequency.VeryLong:
                    return 1f;
                default:
                    return 1f;
			}
		}
        public static int GetFrequencyModulo(this SyncFrequency freq)
        {
            switch (freq)
            {
                case SyncFrequency.Instant:
                    return 0;
                case SyncFrequency.VeryShort:
                    return 1;
                case SyncFrequency.Short:
                    return 2;
                case SyncFrequency.Medium:
                    return 4;
                case SyncFrequency.Long:
                    return 10;
                case SyncFrequency.VeryLong:
                    return 20;
                default:
                    return 20;
            }
        }
    }

    [CreateAssetMenu(fileName = "PinouNetworkSyncableVariablesData", menuName = "Pinou/AutoScript Configs/Syncable Variables Data", order = 1000)]
    public class PinouNetworkSyncableVariablesData : SerializedScriptableObject
    {
        public enum ChannelType
		{
            Unreliable = 1,
            Unreliable_NoDelay = 2,
            Reliable = 0
		}
        [System.Serializable]
        public class VariableData
		{
            [SerializeField] private string _variableName;
            [SerializeField] private ChannelType _channel;

            public string VariableName => _variableName;
            public int Channel => (int)_channel;
        }

        [SerializeField] private string _variablesDataTemplatePath;
        [SerializeField] private VariableData[] _variables;

        public VariableData[] Variables => _variables;


        private void OnValidate()
        {
            PinouAutoscript.UpdateNetworkSyncableVariables(_variables, _variablesDataTemplatePath);
        }
    }
}