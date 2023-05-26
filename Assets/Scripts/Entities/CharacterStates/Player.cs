using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Cinemachine;
using PEC3.Controllers;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Class <c>Ally</c> is the class for the Ally character state.
    /// </summary>
    public class Player : ICharacterState
    {
        /// <value>Property <c>Character</c> represents the character.</value>
        private readonly Character _character;
        
        /// <value>Property <c>_itemColliderList</c> represents the list of items colliding with the player.</value>
        private List<Collider> _itemColliderList = new List<Collider>();
        
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
        }
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        public void UpdateState()
        {
            // Raycast to the mouse position
            var mouseWorldPosition = Vector3.zero;
            var screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            var ray = _character.mainCamera.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out var hit, 999f, _character.aimLayerMask))
            {
                mouseWorldPosition = hit.point;
            }
            
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
                var worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = characterPosition.y;
                var aimDirection = (worldAimTarget - characterPosition).normalized;
                characterTransform.forward = Vector3.Lerp(characterTransform.forward, aimDirection, Time.deltaTime * 20f);
                
                // Check if the charging animation is finished
                if (_character.chargingFinished && _character.shooting && !_character.shootingStarted)
                {
                    var projectileSpawnPointPosition = _character.projectileSpawnPoint.position;
                    var projectileAimDirection = (mouseWorldPosition - projectileSpawnPointPosition).normalized;
                    _character.StartCoroutine(Shoot(projectileSpawnPointPosition, projectileAimDirection));
                }
                else if (_character.shooting)
                {
                    _character.shooting = false;
                }
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

                // Check if the player is attacking
                if (_character.attacking)
                {
                    // Start the attacking animation
                    _character.animator.SetBool(_character.AnimatorAttacking, true);
                    _character.attacking = false;
                }
                else
                {
                    // Reset the attacking animation
                    _character.animator.SetBool(_character.AnimatorAttacking, false);
                }
            }
        }
        
        #region Input
        
            /// <summary>
            /// Method <c>InputAttack</c> invokes the state OnAttack method.
            /// </summary>
            public void InputAttack(bool newAttackState)
            {
                _character.attacking = newAttackState;
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
                _character.shooting = newShootState;
            }
        
        #endregion

        /// <summary>
        /// Method <c>Shoot</c> shoots the projectile.
        /// </summary>
        private IEnumerator Shoot(Vector3 projectileSpawnPointPosition, Vector3 projectileAimDirection)
        {
            // Set the shooting started flag
            _character.shootingStarted = true;
            // Instantiate the projectile
            Object.Instantiate(
                _character.projectilePrefab,
                projectileSpawnPointPosition,
                Quaternion.LookRotation(projectileAimDirection, Vector3.up)
            );
            // Start the shooting animation
            _character.animator.SetBool(_character.AnimatorShooting, true);
            // Wait for the shooting animation to finish
            while (!_character.shootingFinished)
                yield return null;
            // Reset the shooting animation
            _character.animator.SetBool(_character.AnimatorShooting, false);
            // Unset the flags
            _character.shooting = false;
            _character.shootingStarted = false;
            _character.shootingFinished = false;
        }

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
        
        #region Collisions

            /// <summary>
            /// Method <c>HandleCollisionEnter</c> invokes the state OnCollisionEnter method.
            /// </summary>
            /// <param name="col">The collision.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleCollisionEnter(Collision col, string tag)
            {
                if (col.gameObject.CompareTag("Item"))
                    Debug.Log($"[PlayerAimingState] Enter collision on {tag}");
            }

            /// <summary>
            /// Method <c>HandleCollisionStay</c> invokes the state OnCollisionStay method.
            /// </summary>
            /// <param name="col">The collision.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleCollisionStay(Collision col, string tag)
            {
                if (col.gameObject.CompareTag("Item"))
                    Debug.Log($"[PlayerAimingState] Stays collision on {tag}");
            }

            /// <summary>
            /// Method <c>HandleCollisionExit</c> invokes the state OnCollisionExit method.
            /// </summary>
            /// <param name="col">The collision.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleCollisionExit(Collision col, string tag)
            {
                if (col.gameObject.CompareTag("Item"))
                    Debug.Log($"[PlayerAimingState] Exit collision on {tag}");
            }

            /// <summary>
            /// Method <c>HandleTriggerEnter</c> invokes the state OnTriggerEnter method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerEnter(Collider col, string tag)
            {
                if (col.gameObject.CompareTag("Item"))
                    Debug.Log($"[PlayerAimingState] Enter trigger on {tag}");
                
                if (col.gameObject.CompareTag("Item") && tag.Equals("CollisionInner"))
                {
                    _itemColliderList.Add(col);
                    LookAtClosestItem();
                }
            }
            
            /// <summary>
            /// Method <c>HandleTriggerStay</c> invokes the state OnTriggerStay method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerStay(Collider col, string tag)
            {
                if (col.gameObject.CompareTag("Item"))
                    Debug.Log($"[PlayerAimingState] Stays trigger on {tag}");
            }
            
            /// <summary>
            /// Method <c>HandleTriggerExit</c> invokes the state OnTriggerExit method.
            /// </summary>
            /// <param name="col">The other collider.</param>
            /// <param name="tag">The tag of the game object containing the collider.</param>
            public void HandleTriggerExit(Collider col, string tag)
            {
                if (col.gameObject.CompareTag("Item"))
                    Debug.Log($"[PlayerAimingState] Exit trigger on {tag}");
                
                if (col.gameObject.CompareTag("Item") && tag.Equals("CollisionInner"))
                {
                    _itemColliderList.Remove(col);
                    LookAtClosestItem();
                }
            }
            
            /// <summary>
            /// Method <c>GetClosestItemCollider</c> returns the closest item collider.
            /// </summary>
            private (Transform, List<Collider>) GetClosestCollider(List<Collider> colliderList)
            {
                // If the list is null or empty, return null
                if (colliderList == null || colliderList.Count == 0)
                    return (null, new List<Collider>());
                
                // Get the first collider in list which is not null or inactive
                var closestCollider = colliderList.FirstOrDefault(col => col != null && col.gameObject.activeSelf);
                if (!closestCollider)
                    return (null, new List<Collider>());
                
                // Get the distance between the character and the closest collider
                var closestDistance = Vector3.Distance(_character.transform.position, closestCollider.transform.position);
            
                // Iterate through the list of colliders
                var tempColliderList = new List<Collider>(colliderList);
                foreach (var col in colliderList)
                {
                    // If the collider is null or inactive, remove it from the list
                    if (!col || !col.gameObject.activeSelf)
                    {
                        tempColliderList.Remove(col);
                        continue;
                    }
                    // Get the distance between the character and the collider
                    var distance = Vector3.Distance(_character.transform.position, col.transform.position);
                    // If the distance is lesser than the closest distance, update the closest distance and collider
                    if (!(distance < closestDistance))
                        continue;
                    closestDistance = distance;
                    closestCollider = col;
                }
                return (closestCollider.transform, tempColliderList);
            }
            
            /// <summary>
            /// Method <c>LookAtClosestItem</c> makes the character look at the closest item.
            /// </summary>
            private void LookAtClosestItem()
            {
                // Get the closest collider transform
                var (closestItem, updatedItemList) = GetClosestCollider(_itemColliderList);
                // Update the list of colliders
                _itemColliderList = updatedItemList;
                // Update the aim rig
                var sources = new WeightedTransformArray(0);
                if (closestItem != null)
                {
                    sources.Add(new WeightedTransform(closestItem, 1));
                }
                _character.aimHeadRig.data.sourceObjects = sources;
                _character.aimUpperBodyRig.data.sourceObjects = sources;
                _character.animator.enabled = false;
                _character.rigBuilder.Build();
                _character.animator.enabled = true;
            }
        
        #endregion
    }
}