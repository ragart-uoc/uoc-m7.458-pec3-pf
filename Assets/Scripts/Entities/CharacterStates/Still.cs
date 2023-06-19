using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Ally</c> is the class for the Ally character state.
    /// </summary>
    public class Still : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private readonly Character _character;
        
        /// <value>Property <c>TargetTags</c> represents the tags of the targets.</value>
        public List<string> TargetTags { get; set; }

        /// <summary>
        /// Class constructor <c>Still</c> initializes the class.
        /// </summary>
        /// <param name="character">The character.</param>
        public Still(Character character)
        {
            _character = character;
        }

        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        public void StartState()
        {
        }
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        public void UpdateState()
        {
        }
        
        #region Actions

            /// <summary>
            /// Method <c>Move</c> moves the character.
            /// </summary>
            public void Move()
            {
                // Still characters don't move
            }

            /// <summary>
            /// Method <c>Wander</c> makes the character wander.
            /// </summary>
            public void Wander()
            {
                // Still characters don't wander
            }

            /// <summary>
            /// Method <c>Flee</c> makes the character flee.
            /// </summary>
            public void Flee(Transform target)
            {
                // Still characters don't flee
            }

            /// <summary>
            /// Method <c>Chase</c> makes the character chase a target.
            /// </summary>
            public void Chase(Transform target)
            {
                // Still characters don't chase
            }
            
            /// <summary>
            /// Method <c>Attack</c> attacks.
            /// </summary>
            public IEnumerator Attack()
            {
                // Still characters don't attack
                yield break;
            }
            
            /// <summary>
            /// Method <c>AttackFinished</c> is called when the attack animation finishes.
            /// </summary>
            public void AttackFinished()
            {
                // Still characters don't attack
            }

            /// <summary>
            /// Method <c>Shoot</c> shoots the projectile.
            /// </summary>
            /// <param name="projectileSpawnPointPosition">The projectile spawn point position.</param>
            /// <param name="projectileAimDirection">The projectile aim direction.</param>
            public IEnumerator Shoot(Vector3 projectileSpawnPointPosition, Vector3 projectileAimDirection)
            {
                // Still characters don't shoot
                yield break;
            }
        
            /// <summary>
            /// Method <c>TakeDamage</c> takes damage.
            /// </summary>
            /// <param name="damage">The damage received.</param>
            public IEnumerator TakeDamage(float damage)
            {
                // Still characters don't take damage
                yield break;
            }
            
            /// <summary>
            /// Method <c>RestoreHealth</c> restores the character health.
            /// </summary>
            public IEnumerator RestoreHealth(float multiplier)
            {
                // Neutrals don't restore health (at least for now)
                yield break;
            }
            
            /// <summary>
            /// Method <c>RestoreShield</c> restores the character shield.
            /// </summary>
            public IEnumerator RestoreShield(float multiplier)
            {
                // Neutrals don't restore health (at least for now)
                yield break;
            }

            /// <summary>
            /// Method <c>Die</c> makes the character die.
            /// </summary>
            public IEnumerator Die()
            {
                // Still characters don't die
                yield break;
            }

            /// <summary>
            /// Method <c>DeadFinished</c> is called when the dead animation finishes.
            /// </summary>
            public IEnumerator DeadFinished()
            {
                // Still characters don't die
                yield break;
            }
            
            /// <summary>
            /// Method <c>Explode</c> makes the character explode.
            /// </summary>
            public IEnumerator Explode()
            {
                // Disable all the character renderers
                foreach (var renderer in _character.GetComponentsInChildren<Renderer>())
                    renderer.enabled = false;
                // Launch the explosion particles
                _character.explodeParticles.gameObject.SetActive(true);
                // Play the explosion sound
                _character.HandlePlaySound(_character.explodeSound);
                // Wait for the explosion to finish
                yield return new WaitForSeconds(_character.explodeParticles.main.duration);
                // Destroy the character
                Object.Destroy(_character.gameObject);
            }

            /// <summary>
            /// Method <c>DropItem</c> drops an item.
            /// </summary>
            public void DropItem()
            {
                // Still characters don't drop items (at least for now)
            }
        
        #endregion
        
        #region Input
        
            /// <summary>
            /// Method <c>InputAttack</c> invokes the state OnAttack method.
            /// </summary>
            /// <param name="newAttackState">The new attacking state.</param>
            public void InputAttack(bool newAttackState)
            {
            }
            
            /// <summary>
            /// Method <c>InputAim</c> invokes the state OnAim method.
            /// </summary>
            /// <param name="newAimState">The new aiming state.</param>
            public void InputAim(bool newAimState)
            {
            }
            
            /// <summary>
            /// Method <c>InputShoot</c> invokes the state OnShoot method.
            /// </summary>
            /// <param name="newShootState">The new shooting state.</param>
            public void InputShoot(bool newShootState)
            {
            }
            
            /// <summary>
            /// Method <c>InputEnterShip</c> invokes the state OnEnterShip method.
            /// </summary>
            /// <param name="newEnterShipState"></param>
            public void InputEnterShip(bool newEnterShipState)
            {
            }
        
        #endregion
        
        #region Collisions

            /// <summary>
            /// Method <c>HandleCollisionEnter</c> invokes the state OnCollisionEnter method.
            /// </summary>
            /// <param name="col">The collision.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleCollisionEnter(Collision col, string tag)
            {
            }

            /// <summary>
            /// Method <c>HandleCollisionStay</c> invokes the state OnCollisionStay method.
            /// </summary>
            /// <param name="col">The collision.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleCollisionStay(Collision col, string tag)
            {
            }

            /// <summary>
            /// Method <c>HandleCollisionExit</c> invokes the state OnCollisionExit method.
            /// </summary>
            /// <param name="col">The collision.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleCollisionExit(Collision col, string tag)
            {
            }

            /// <summary>
            /// Method <c>HandleTriggerEnter</c> invokes the state OnTriggerEnter method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerEnter(Collider col, string tag)
            {
            }
            
            /// <summary>
            /// Method <c>HandleTriggerStay</c> invokes the state OnTriggerStay method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerStay(Collider col, string tag)
            {
            }
            
            /// <summary>
            /// Method <c>HandleTriggerExit</c> invokes the state OnTriggerExit method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerExit(Collider col, string tag)
            {
            }
        
        #endregion
    }
}