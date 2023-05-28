using System;
using UnityEngine;

namespace PEC3.Controllers
{
    /// <summary>
    /// Struct <c>EnemySpawnStruct</c> represents the properties of the enemy spawn.
    /// </summary>
    [Serializable]
    public struct EnemySpawnStruct
    {
        /// <value>Property <c>EnemyPrefab</c> represents the enemy prefab.</value>
        public GameObject EnemyPrefab;
        
        /// <value>Property <c>EnemyWeight</c> represents the enemy weight.</value>
        public int EnemyWeight;
        
        /// <summary>
        /// Method <c>EnemySpawnStruct</c> is the constructor of the struct.
        /// </summary>
        /// <param name="enemyPrefab">The enemy prefab.</param>
        /// <param name="enemyWeight">The enemy weight.</param>
        public EnemySpawnStruct(GameObject enemyPrefab, int enemyWeight)
        {
            EnemyPrefab = enemyPrefab;
            EnemyWeight = enemyWeight;
        }
    }
}
