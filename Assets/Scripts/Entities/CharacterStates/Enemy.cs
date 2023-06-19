using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PEC3.Managers;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Enemy</c> is the class for the enemy character state.
    /// </summary>
    public class Enemy : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private readonly Character _character;

        /// <value>Property <c>TargetTags</c> represents the tags of the targets.</value>
        public List<string> TargetTags { get; set; } = new()
        {
            "Player",
            "Ally",
            "Neutral"
        };

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
            
            // Set the difficulty
            _character.maxHealth *= GameManager.Instance.difficulty;
            _character.health *= GameManager.Instance.difficulty;
            _character.maxShield *= GameManager.Instance.difficulty;
            _character.shield *= GameManager.Instance.difficulty;
            _character.meleeDamage *= GameManager.Instance.difficulty;
        }
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        public void UpdateState()
        {
            if (_character.dead)
                return;
            Move();
            
            // Pass the velocity to the animator
            _character.animator.SetFloat(_character.AnimatorSpeed, _character.agent.velocity.magnitude);
            _character.animator.SetFloat(_character.AnimatorMotionSpeed, _character.agent.velocity.magnitude > 0 ? 1 : 0);
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
                // Play the attack sound
                _character.HandlePlaySound(_character.attackSound);
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
                if (!TargetTags.Contains(hit.transform.tag))
                    return;
                // Get the character
                var target = hit.transform.GetComponent<Character>();
                // Check if the target is dead
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
                // Launch the hit1 and hit2 particles
                _character.hit1Particles.gameObject.SetActive(true);
                _character.hit2Particles.Play();
                // Play the hit sound
                _character.HandlePlaySound(_character.hitSound);
                // Unset the flags
                _character.hit = false;
                // Check if the character is dead
                if (_character.health <= 0)
                {
                    _character.StartCoroutine(Die());
                }
            }
            
            /// <summary>
            /// Method <c>RestoreHealth</c> restores the character health.
            /// </summary>
            public IEnumerator RestoreHealth(float multiplier)
            {
                // Enemies don't restore health (at least for now)
                yield break;
            }
            
            /// <summary>
            /// Method <c>RestoreShield</c> restores the character shield.
            /// </summary>
            public IEnumerator RestoreShield(float multiplier)
            {
                // Enemies don't restore health (at least for now)
                yield break;
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
                // Launch the dead particles
                _character.deathParticles.gameObject.SetActive(true);
                // Play the dead sound
                _character.HandlePlaySound(_character.deathSound);
            }
            
            /// <summary>
            /// Method <c>DeadFinished</c> is called when the dead animation finishes.
            /// </summary>
            public IEnumerator DeadFinished()
            {
                // Drop a random item
                DropItem();

                // Make the character disappear slowly, decreasing its size
                while (_character.transform.localScale.x > 0)
                {
                    _character.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                    yield return null;
                }
                
                // Launch the after death particles
                _character.afterDeathParticles.gameObject.SetActive(true);

                // Destroy the character
                _character.animator.StopPlayback();
                Object.Destroy(_character.gameObject);
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
                var position = _character.transform.position;
                var dropPosition = new Vector3(position.x, position.y + 1f, position.z);
            
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
                switch (tag)
                {
                    case "CollisionInner":
                        // Check if the collider is a target
                        if (TargetTags.Contains(col.tag)
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
                        if (TargetTags.Contains(col.tag)
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
                        if (TargetTags.Contains(col.tag)
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