using UnityEngine;

namespace Pinou
{
    public class GameObjectInfos : MonoBehaviour
    {
        private float _spawnTime;
        public float SpawnTime { get { return _spawnTime; } }
        public float TimeSinceSpawn { get { return Time.time - _spawnTime; } }

        private void Awake()
        {
            _spawnTime = Time.time;
        }
    }
}