using System.Collections.Generic;
using System.Linq;
using PEC3.Managers;
using UnityEngine;

namespace PEC3.Controllers
{
    /// <summary>
    /// Class <c>EnemySpawnController</c> contains the methods and properties needed for the enemy spawn controller.
    /// </summary>
    public class EnemySpawnController : MonoBehaviour
    {
        /// <value>Property <c>EnemySpawnList</c> represents the list of enemies to spawn.</value>
        public List<EnemySpawnStruct> enemySpawnList;
        
        /// <value>Property <c>EnemySpawnTime</c> represents the time between enemy spawns.</value>
        public float lastSpawnTime;

        /// <value>Property <c>enemySpawnNumberOverTime</c> represents the number of enemies to spawn over time.</value>
        public int[] enemySpawnNumberOverTime;
        
        /// <value>Property <c>timeBetweenSpawns</c> represents the time between spawns.</value>
        public float timeBetweenSpawns = 30.0f;
        
        /// <value>Property <c>_totalWeight</c> represents the total weight of the enemies.</value>
        private int _totalWeight;
        
        /// <value>Property <c>_spawnedEnemyNumber</c> represents the number of spawned enemies.</value>
        private int _spawnedEnemyNumber;

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Calculate the total weight of the enemies
            _totalWeight = enemySpawnList.Sum(enemySpawn => enemySpawn.EnemyWeight);
            
            // Spawn the very first enemy
            lastSpawnTime = Time.time;
            SpawnEnemy();
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            _spawnedEnemyNumber = GetSpawnedEnemyNumber();
            
            if (Time.time - lastSpawnTime < timeBetweenSpawns
                    || _spawnedEnemyNumber > GameManager.Instance.maxNumberOfEnemies)
                return;
            lastSpawnTime = Time.time;
            SpawnEnemy();
        }
        
        /// <summary>
        /// Method <c>SpawnEnemy</c> spawns an enemy.
        /// </summary>
        private void SpawnEnemy()
        {
            // Get the number of enemies to spawn depending on the index of enemySpawnNumberOverTime
            var index = (int) (Time.time / timeBetweenSpawns);
            if (index >= enemySpawnNumberOverTime.Length)
                index = enemySpawnNumberOverTime.Length - 1;
            var numberOfEnemiesToSpawn = enemySpawnNumberOverTime[index];
            
            // Spawn the enemies randomly taking weight into account
            for (var i = 0; i < numberOfEnemiesToSpawn; i++)
            {
                var randomWeight = Random.Range(0, _totalWeight);
                var currentWeight = 0;
                foreach (var enemySpawn in enemySpawnList)
                {
                    currentWeight += enemySpawn.EnemyWeight;
                    if (currentWeight <= randomWeight)
                        continue;
                    Instantiate(enemySpawn.EnemyPrefab, transform.position, Quaternion.identity);
                    break;
                }
            }
        }
        
        /// <summary>
        /// Method <c>GetSpawnedEnemyNumber</c> gets the number of spawned enemies in the scene.
        /// </summary>
        private int GetSpawnedEnemyNumber()
        {
            // Get all the enemies in the scene
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            return enemies.Length;
        }
    }
}
