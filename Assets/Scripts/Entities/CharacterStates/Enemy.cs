using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Ally</c> is the class for the Ally character state.
    /// </summary>
    public class Enemy : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private readonly Character _character;
        
        /// <summary>
        /// Class constructor <c>Enemy</c> initializes the class.
        /// </summary>
        /// <param name="character">The character.</param>
        public Enemy(Character character)
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
                
                // Chase the target
                var realTarget = _character.forcedTarget ? _character.forcedTarget : _character.target;
                Chase(realTarget);
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
                
                // Check if the wander timer is greater than the wander time
                if (!(_character.wanderTimer > _character.wanderTime))
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
                // Enemies don't flee
            }

            /// <summary>
            /// Method <c>Chase</c> makes the character chase a target.
            /// </summary>
            public void Chase(Transform target)
            {
                // Set the speed and acceleration
                _character.agent.speed = _character.sprintSpeed;
                _character.agent.acceleration = _character.sprintSpeed * 2f;

                // Get the character position and rotation
                var characterTransform = _character.transform;
                var characterPosition = characterTransform.position;
                
                // Get the target position
                var targetPosition = target.position;
                
                // Get the distance to the target
                var distance = Vector3.Distance(characterPosition, targetPosition);
                
                // Check if the target is close enough to attack
                if (distance <= _character.attackDistance)
                {
                    // Start the attack coroutine
                    _character.StartCoroutine(Attack());
                }
                // Move towards the target otherwise
                else
                {
                    // Get the direction to the target
                    var direction = targetPosition - characterPosition;
                    _character.agent.SetDestination(characterPosition + direction.normalized);
                }
            }
            
            /// <summary>
            /// Method <c>Attack</c> attacks.
            /// </summary>
            public IEnumerator Attack()
            {
                // Check if the character is already attacking or if the attack is on cooldown
                if (_character.attacking
                        || Time.time < _character.lastAttackTime + _character.attackRate)
                    yield break;
                // Set the flags
                _character.attacking = true;
                // Set the previous attack time
                _character.lastAttackTime = Time.time;
                // Start the attacking animation
                _character.animator.SetTrigger(_character.AnimatorAttacking);
                // Unset the flags
                _character.attacking = false;
            }
            
            /// <summary>
            /// Method <c>AttackFinished</c> is called when the attack animation finishes.
            /// </summary>
            public void AttackFinished()
            {
                // Check if there is a target
                var characterTransform = _character.transform;
                var characterPosition = characterTransform.position;
                var position = new Vector3(characterPosition.x, characterPosition.y + 1f, characterPosition.z);
                var ray = new Ray(position, characterTransform.forward);
                if (!Physics.Raycast(ray, out var hit, _character.attackDistance))
                    return;
                // Check if the collider is a target
                if (!hit.transform.CompareTag("Player")
                    && !hit.transform.CompareTag("Neutral")
                    && !hit.transform.CompareTag("Ally"))
                    return;
                // Get the character
                var target = hit.transform.GetComponent<Character>();
                // Check if the enemy is dead
                if (target == null || target.dead)
                    return;
                // Damage the target
                target.TakeDamage(_character.meleeDamage);
                // Aggro the target
                target.forcedTarget = _character.transform;
            }

            /// <summary>
            /// Method <c>Shoot</c> shoots the projectile.
            /// </summary>
            /// <param name="projectileSpawnPointPosition">The projectile spawn point position.</param>
            /// <param name="projectileAimDirection">The projectile aim direction.</param>
            public IEnumerator Shoot(Vector3 projectileSpawnPointPosition, Vector3 projectileAimDirection)
            {
                // Enemies don't shoot (at least for now)
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
                // Drop a random item
                DropItem();

                // Make the enemy disappear slowly, decreasing its size
                while (_character.transform.localScale.x > 0)
                {
                    _character.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                    yield return null;
                }

                // Destroy the enemy
                Object.Destroy(_character.gameObject);
            }

            /// <summary>
            /// Method <c>DropItem</c> drops an item.
            /// </summary>
            public void DropItem()
            {
                var position = _character.transform.position;
                var dropPosition = new Vector3(position.x, position.y + 1.5f, position.z);
            
                // If there's a mandatory drop, drop it
                if (_character.mandatoryDrop != null)
                {
                    Object.Instantiate(_character.mandatoryDrop, dropPosition, Quaternion.identity);
                    return;
                }
            
                // If there's optional drop, have a 90% change of dropping one of them
                if (_character.optionalDrops.Length > 0 && Random.Range(0, 4) == 0)
                {
                    var drop = Random.Range(0, _character.optionalDrops.Length);
                    Object.Instantiate(_character.optionalDrops[drop], dropPosition, Quaternion.identity);
                }
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
                        if ((col.CompareTag("Player") || col.CompareTag("Neutral") || col.CompareTag("Ally"))
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
                        if ((col.CompareTag("Player") || col.CompareTag("Neutral") || col.CompareTag("Ally"))
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
                        if ((col.CompareTag("Player") || col.CompareTag("Neutral") || col.CompareTag("Ally"))
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