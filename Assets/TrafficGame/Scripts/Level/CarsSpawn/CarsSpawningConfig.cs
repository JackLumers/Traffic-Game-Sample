using TrafficGame.Scripts.Level.CarsSpawn.Car;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TrafficGame.Scripts.Level.CarsSpawn
{
    [CreateAssetMenu(fileName = "CarsSpawningConfig", menuName = "Create Cars Spawning Config")]
    public class CarsSpawningConfig : ScriptableObject
    {
        [SerializeField] private AssetReferenceGameObject _carViewReference;
        [SerializeField] private int _minSpawnDelay = 1000;
        [SerializeField] private int _maxSpawnDelay = 2000;
        [SerializeField] private int _minCarSpeed = 100;
        [SerializeField] private int _maxCarSpeed = 200;
        
        public int MinSpawnDelay => _minSpawnDelay;
        public int MaxSpawnDelay => _maxSpawnDelay;
        public int MinCarSpeed => _minCarSpeed;
        public int MaxCarSpeed => _maxCarSpeed;

        public AssetReferenceGameObject CarViewReference => _carViewReference;
    }
}