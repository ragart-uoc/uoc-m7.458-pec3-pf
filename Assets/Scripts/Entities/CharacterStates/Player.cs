using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using PEC3.Controllers;
using PEC3.Managers;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Ally</c> is the class for the Ally character state.
    /// </summary>
    public class Player : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private readonly Character _character;
        
        /// <value>Property <c>MouseWorldPosition</c> represents the mouse world position.</value>
        private Vector3 _mouseWorldPosition;
        
        /// <summary>
        /// Class constructor <c>Player</c> initializes the class.
        /// </summary>
        /// <param name="character">The character.</param>
        public Player(Character character)
        {
            _character = character;
        }

        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        public void StartState()
        {
            // Get the player controller
            if (_character.controller == null)
                _character.controller = _character.GetComponent<CustomThirdPersonController>();
            
            // Set the virtual cameras to follow the player camera root
            if (_character.playerCameraRoot == null)
                _character.playerCameraRoot = _character.transform.Find("PlayerCameraRoot");
            if (_character.playerFollowCamera == null)
                _character.playerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineVirtualCamera>();
            _character.playerFollowCamera.Follow = _character.playerCameraRoot;
            if (_character.playerAimingCamera == null)
                _character.playerAimingCamera = GameObject.Find("PlayerAimingCamera").GetComponent<CinemachineVirtualCamera>();
            _character.playerAimingCamera.Follow = _character.playerCameraRoot;
            
            // Reset the animator
            _character.animator.Rebind();
            _character.animator.Update(0f);
        }
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        public void UpdateState()
        {
            // Raycast to the mouse position
            _mouseWorldPosition = Vector3.zero;
            var screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var ray = _character.mainCamera.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out var hit, 999f, _character.aimLayerMask))
            {
                _mouseWorldPosition = hit.point;
            }
            
            // Check if the character is dead
            if (_character.dead)
                return;
            
            // Check if the player is aiming
            if (_character.aiming)
            {
                // Set the aiming camera as the priority
                _character.playerFollowCamera.Priority = 0;
                _character.playerAimingCamera.Priority = 1;
                // Set the aiming sensitivity
                _character.controller.SetSensitivity(_character.aimSensitivity);
                // Disable the aiming rotation
                _character.controller.SetRotateOnMove(false);
                // Start the charging animation
                _character.animator.SetBool(_character.AnimatorCharging, true);
                // Make the character look at the mouse position
                var characterTransform = _character.transform;
                var characterPosition = characterTransform.position;
                var worldAimTarget = _mouseWorldPosition;
                worldAimTarget.y = characterPosition.y;
                var aimDirection = (worldAimTarget - characterPosition).normalized;
                characterTransform.forward = Vector3.Lerp(characterTransform.forward, aimDirection, Time.deltaTime * 20f);
            }
            else
            {
                // Set the normal camera as the priority
                _character.playerFollowCamera.Priority = 1;
                _character.playerAimingCamera.Priority = 0;
                // Set the normal sensitivity
                _character.controller.SetSensitivity(_character.normalSensitivity);
                // Enable the aiming rotation
                _character.controller.SetRotateOnMove(true);
                // Reset the charging animation
                _character.animator.SetBool(_character.AnimatorCharging, false);
                // Unset the charging finished flag
                _character.chargingFinished = false;
            }
        }
        
        #region Actions

            /// <summary>
            /// Method <c>Move</c> moves the character.
            /// </summary>
            public void Move()
            {
                // Player movement is handled by the character controller
            }

            /// <summary>
            /// Method <c>Wander</c> makes the character wander.
            /// </summary>
            public void Wander()
            {
                // Players don't wander
            }

            /// <summary>
            /// Method <c>Flee</c> makes the character flee.
            /// </summary>
            public void Flee(Transform target)
            {
                // Players don't flee
            }

            /// <summary>
            /// Method <c>Chase</c> makes the character chase a target.
            /// </summary>
            public void Chase(Transform target)
            {
                // Players don't chase
            }
            
            /// <summary>
            /// Method <c>Attack</c> attacks.
            /// </summary>
            public IEnumerator Attack()
            {
                // Set the previous attack time
                _character.lastAttackTime = Time.time;
                // Start the attacking animation
                _character.animator.SetTrigger(_character.AnimatorAttacking);
                // Unset the flags
                _character.attacking = false;
                yield break;
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
                if (!hit.transform.CompareTag("Enemy"))
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
                // Set the previous attack time
                _character.lastShootTime = Time.time;
                // Instantiate the projectile
                Object.Instantiate(
                    _character.projectilePrefab,
                    projectileSpawnPointPosition,
                    Quaternion.LookRotation(projectileAimDirection, Vector3.up)
                );
                // Start the shooting animation
                _character.animator.SetTrigger(_character.AnimatorShooting);
                // Unset the flags
                _character.shooting = false;
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
                _character.health += _character.maxHealth * multiplier;
                _character.health = Mathf.Clamp(_character.health, 0.0f, _character.maxHealth);
                UIManager.Instance.UpdatePlayerUI(_character.health, _character.shield);
                _character.restoreParticles.gameObject.SetActive(true);
                yield break;
            }
            
            /// <summary>
            /// Method <c>RestoreShield</c> restores the character shield.
            /// </summary>
            public IEnumerator RestoreShield(float multiplier)
            {
                _character.shield += _character.maxShield * multiplier;
                _character.shield = Mathf.Clamp(_character.shield, 0.0f, _character.maxShield);
                UIManager.Instance.UpdatePlayerUI(_character.health, _character.shield);
                _character.restoreParticles.gameObject.SetActive(true);
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
            }
            
            /// <summary>
            /// Method <c>DeadFinished</c> is called when the dead animation finishes.
            /// </summary>
            public IEnumerator DeadFinished()
            {
                // Change the character type
                _character.ChangeType(CharacterProperties.Types.Enemy);
                // Launch the rebirth particles
                _character.rebornParticles.gameObject.SetActive(true);
                yield break;
            }

            /// <summary>
            /// Method <c>DropItem</c> drops an item.
            /// </summary>
            public void DropItem()
            {
                // Allies don't drop items (at least for now)
            }
        
        #endregion
        
        #region Input
        
            /// <summary>
            /// Method <c>InputAttack</c> invokes the state OnAttack method.
            /// </summary>
            public void InputAttack(bool newAttackState)
            {
                if (!newAttackState
                        || _character.aiming
                        || _character.attacking
                        || Time.time < _character.lastAttackTime + _character.attackRate)
                    return;
                _character.attacking = true;
                _character.StartCoroutine(Attack());
            }
            
            /// <summary>
            /// Method <c>InputAim</c> invokes the state OnAim method.
            /// </summary>
            public void InputAim(bool newAimState)
            {
                _character.aiming = newAimState;
                if (newAimState)
                {
                    _character.StartCoroutine(LerpRigWeight(_character.aimBodyRig, 0f, 0.7f, 1f));
                    _character.StartCoroutine(LerpRigWeight(_character.aimFistRig, 0f, 1f, 1f));
                }
                else
                {
                    _character.StartCoroutine(LerpRigWeight(_character.aimBodyRig, 0.7f, 0f, 1f));
                    _character.StartCoroutine(LerpRigWeight(_character.aimFistRig, 1f, 0f, 1f));
                }
            }
            
            /// <summary>
            /// Method <c>InputShoot</c> invokes the state OnShoot method.
            /// </summary>
            public void InputShoot(bool newShootState)
            {
                if (!newShootState
                    || !_character.aiming
                    || !_character.chargingFinished
                    || _character.shooting
                    || Time.time < _character.lastShootTime + _character.attackRate)
                    return;
                _character.shooting = true;
                var projectileSpawnPointPosition = _character.projectileSpawnPoint.position;
                var projectileAimDirection = (_mouseWorldPosition - projectileSpawnPointPosition).normalized;
                _character.StartCoroutine(Shoot(projectileSpawnPointPosition, projectileAimDirection));
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
                if (col.gameObject.CompareTag("Item") && tag.Equals("CollisionInner"))
                {
                    _character.itemColliderList.Add(col);
                    _character.LookAtClosestItem();
                }
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
                if (col.gameObject.CompareTag("Item") && tag.Equals("CollisionInner"))
                {
                    _character.itemColliderList.Remove(col);
                    _character.LookAtClosestItem();
                }
            }
        
        #endregion
        
        #region Animation Rigging
            
            /// <summary>
            /// Method <c>LerpRigWeight</c> lerps the rig weight.
            /// </summary>
            /// <param name="aim">The rig constraint.</param>
            /// <param name="initialWeight">The initial weight.</param>
            /// <param name="finalWeight">The final weight.</param>
            /// <param name="duration">The duration of the lerp.</param>
            private IEnumerator LerpRigWeight(IRigConstraint aim, float initialWeight, float finalWeight, float duration)
            {
                var elapsedTime = 0f;
                while (elapsedTime < duration)
                {
                    aim.weight = Mathf.Lerp(initialWeight, finalWeight, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        
        #endregion
    }
}