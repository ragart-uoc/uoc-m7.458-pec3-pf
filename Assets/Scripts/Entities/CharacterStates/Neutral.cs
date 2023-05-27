using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Ally</c> is the class for the Ally character state.
    /// </summary>
    public class Neutral : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private readonly Character _character;
        
        /// <summary>
        /// Class constructor <c>Neutral</c> initializes the class.
        /// </summary>
        /// <param name="character">The character.</param>
        public Neutral(Character character)
        {
            _character = character;
        }

        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        public void StartState()
        {
            // Get the nav mesh agent
            if (_character.agent == null)
                _character.agent = _character.GetComponent<NavMeshAgent>();

            // Reset the animator
            _character.animator.Rebind();
            _character.animator.Update(0f);
            
            // Set the flee timer
            _character.fleeTimer = _character.fleeTime;
        }
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        public void UpdateState()
        {
            if (_character.dead)
                return;
            Move();
        }
        
        #region Actions

            /// <summary>
            /// Method <c>Move</c> moves the character.
            /// </summary>
            public void Move()
            {
                // Check if there is a target
                if (_character.forcedTarget == null && _character.target == null)
                {
                    Wander();
                    return;
                }
                
                // Update the flee timer
                _character.fleeTimer += Time.deltaTime;
                
                // Check if the wander time was exceeded
                if (_character.fleeTimer <= _character.fleeTime)
                    return;
                
                // Flee from the target
                var realTarget = _character.forcedTarget ? _character.forcedTarget : _character.target;
                Flee(realTarget);
            }

            /// <summary>
            /// Method <c>Wander</c> makes the character wander.
            /// </summary>
            public void Wander()
            {
                // Set the speed and acceleration
                _character.agent.speed = _character.moveSpeed;
                _character.agent.acceleration = _character.moveSpeed * 2f;
                
                // Update the wander timer
                _character.wanderTimer += Time.deltaTime;
                
                // Check if the wander time was exceeded
                if (_character.wanderTimer <= _character.wanderTime)
                    return;

                // Get a random position
                var randomPosition = _character.RandomNavSphere(_character.transform.position, _character.wanderRadius, -1);
                    
                // Set the destination
                _character.agent.SetDestination(randomPosition);
                    
                // Reset the wander timer
                _character.wanderTimer = 0f;
            }

            /// <summary>
            /// Method <c>Flee</c> makes the character flee.
            /// </summary>
            public void Flee(Transform target)
            {
                // Set the speed and acceleration
                _character.agent.speed = _character.sprintSpeed;
                _character.agent.acceleration = _character.sprintSpeed * 2f;
                
                // Get the character position and rotation
                var characterTransform = _character.transform;
                var characterPosition = characterTransform.position;
                
                // Get the target position
                var targetPosition = target.position;
                
                // Run away from the target
                var direction = characterPosition - targetPosition;
                direction = Quaternion.Euler(0, Random.Range(-180, 180), 0) * direction;
                _character.agent.destination = characterPosition + direction;
                
                // Reset the flee timer
                _character.fleeTimer = 0f;
            }

            /// <summary>
            /// Method <c>Chase</c> makes the character chase a target.
            /// </summary>
            public void Chase(Transform target)
            {
                // Neutrals don't chase
            }
            
            /// <summary>
            /// Method <c>Attack</c> attacks.
            /// </summary>
            public IEnumerator Attack()
            {
                // Neutrals don't attack
                yield break;
            }
            
            /// <summary>
            /// Method <c>AttackFinished</c> is called when the attack animation finishes.
            /// </summary>
            public void AttackFinished()
            {
                // Neutrals don't attack
            }

            /// <summary>
            /// Method <c>Shoot</c> shoots the projectile.
            /// </summary>
            /// <param name="projectileSpawnPointPosition">The projectile spawn point position.</param>
            /// <param name="projectileAimDirection">The projectile aim direction.</param>
            public IEnumerator Shoot(Vector3 projectileSpawnPointPosition, Vector3 projectileAimDirection)
            {
                // Neutrals don't shoot
                yield break;
            }
        
            /// <summary>
            /// Method <c>TakeDamage</c> takes damage.
            /// </summary>
            /// <param name="damage">The damage received.</param>
            public IEnumerator TakeDamage(float damage)
            {
                // Check if the character is already being hit or is dead
                if (_character.hit || _character.dead)
                    yield break;
                // Set the flags
                _character.hit = true;
                // Take damage
                _character.health -= damage * 0.1f;
                _character.shield -= damage * 0.9f;
                if (_character.shield < 0.0f)
                {
                    _character.health += _character.shield;
                    _character.shield = 0.0f;
                }
                // Start the hit animation
                _character.animator.SetTrigger(_character.AnimatorHit);
                // Unset the flags
                _character.hit = false;
                // Check if the character is dead
                if (_character.health <= 0)
                {
                    _character.StartCoroutine(Die());
                }
            }

            /// <summary>
            /// Method <c>Die</c> makes the character die.
            /// </summary>
            public IEnumerator Die()
            {
                // Check if the character is already dead
                if (_character.dead)
                    yield break;
                // Set the dead flag
                _character.dead = true;
                // Start the dead animation
                _character.animator.SetBool(_character.AnimatorDead, true);
            }
            
            /// <summary>
            /// Method <c>DeadFinished</c> is called when the dead animation finishes.
            /// </summary>
            public IEnumerator DeadFinished()
            {
                _character.Enemify();
                yield break;
            }

            /// <summary>
            /// Method <c>DropItem</c> drops an item.
            /// </summary>
            public void DropItem()
            {
                // Neutrals don't drop items (at least for now)
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
                switch (tag)
                {
                    case "CollisionInner":
                        // Check if the collider is a target
                        if ((col.CompareTag("Enemy"))
                            && !_character.targetColliderList.Contains(col))
                        {
                            // Add the collider to the target list
                            _character.targetColliderList.Add(col);
                            _character.SetTarget();
                        }
                        break;
                    case "CollisionOuter":
                        break;
                    case "Player":
                        break;
                }
            }
            
            /// <summary>
            /// Method <c>HandleTriggerStay</c> invokes the state OnTriggerStay method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerStay(Collider col, string tag)
            {
                switch (tag)
                {
                    case "CollisionInner":
                        // Check if the collider is a target
                        if ((col.CompareTag("Enemy"))
                            && !_character.targetColliderList.Contains(col))
                        {
                            // Add the collider to the target list
                            _character.targetColliderList.Add(col);
                            _character.SetTarget();
                        }
                        break;
                    case "CollisionOuter":
                        break;
                    case "Player":
                        break;
                }
            }
            
            /// <summary>
            /// Method <c>HandleTriggerExit</c> invokes the state OnTriggerExit method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerExit(Collider col, string tag)
            {
                switch (tag)
                {
                    case "CollisionInner":
                        // Check if the collider is a target
                        if ((col.CompareTag("Enemy"))
                            && _character.targetColliderList.Contains(col))
                        {
                            // Remove the collider from the target list
                            _character.targetColliderList.Remove(col);
                            _character.SetTarget();
                        }
                        break;
                    case "CollisionOuter":
                        // If the collider is the forced target, remove it
                        if (col.transform == _character.forcedTarget)
                            _character.forcedTarget = null;
                        break;
                    case "Player":
                        break;
                }
            }
        
        #endregion
    }
}