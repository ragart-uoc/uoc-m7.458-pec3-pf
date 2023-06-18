using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using Cinemachine;
using StarterAssets;
using PEC3.Controllers;
using PEC3.Entities.CharacterStates;
using PEC3.Managers;
using Random = UnityEngine.Random;

namespace PEC3.Entities
{
    /// <summary>
    /// Class <c>Character</c> is the base class for all characters.
    /// </summary>
    public class Character : MonoBehaviour
    {
        #region Character States
            
            /// <value>Property <c>characterType</c> represents the character type.</value>
            public CharacterProperties.Types characterType;
        
            /// <value>Property <c>_characterStates</c> represents the list of character states.</value>
            private readonly Dictionary<CharacterProperties.Types, ICharacterState> _characterStates = new Dictionary<CharacterProperties.Types, ICharacterState>();
            
            /// <value>Property <c>CurrentState</c> represents the current character state.</value>
            public ICharacterState CurrentState;
            
        #endregion
        
        #region Component references
        
            /// <value>Property <c>controller</c> represents the custom third person controller.</value>
            [Header("Components")]
            public CustomThirdPersonController controller;

            /// <value>Property <c>animator</c> represents the character animator.</value>
            public Animator animator;

            /// <value>Property <c>rigBuilder</c> represents the rig builder.</value>
            public RigBuilder rigBuilder;
            
            /// <value>Property <c>agent</c> represents the character nav mesh agent.</value>
            public NavMeshAgent agent;

            /// <value>Property <c>playerInputs</c> represents the player inputs.</value>
            public StarterAssetsInputs playerInputs;
        
        #endregion
        
        #region Character Settings
        
            /// <value>Property <c>maxHealth</c> represents the maximum health of the character.</value>
            public float maxHealth = 100f;

            /// <value>Property <c>health</c> represents the health of the character.</value>
            [Header("Character Settings")]
            public float health = 100f;
            
            /// <value>Property <c>maxShield</c> represents the maximum shield of the character.</value>
            public float maxShield = 100f;
            
            /// <value>Property <c>shield</c> represents the shield of the character.</value>
            public float shield;
            
            /// <value>Property <c>damage</c> represents the damage of the character.</value>
            public float meleeDamage = 50f;
            
            /// <value>Property <c>attackRate</c> represents the attack rate.</value>
            public float attackRate = 1f;
            
            /// <value>Property <c>targetDistance</c> represents the minimum distance to attack.</value>
            public float attackDistance = 1f;
            
            /// <value>Property <c>damageTakenMultiplier</c> represents the damage taken multiplier.</value>
            public float damageTakenMultiplier = 1f;
            
            /// <value>Property <c>lastAttackTime</c> represents the last attack time.</value>
            [HideInInspector]
            public float lastAttackTime;
            
            /// <value>Property <c>lastShootTime</c> represents the last shoot time.</value>
            [HideInInspector]
            public float lastShootTime;
            
            /// <value>Property <c>moveSpeed</c> represents the move speed.</value>
            public float moveSpeed = 5f;
            
            /// <value>Property <c>sprintSpeed</c> represents the sprint speed.</value>
            public float sprintSpeed = 10f;
            
            /// <value>Property <c>wanderRadius</c> represents the wander radius.</value>
            public float wanderRadius = 10f;
            
            /// <value>Property <c>wanderTime</c> represents the wander time.</value>
            public float wanderTime = 5f;
            
            /// <value>Property <c>fleeTime</c> represents the flee time.</value>
            public float fleeTime = 2f;

        #endregion
        
        #region Item Drops

            /// <value>Property <c>mandatoryDrop</c> represents the mandatory drop of the enemy.</value>
            [Header("Item Drop")]
            public GameObject mandatoryDrop;
                
            /// <value>Property <c>optionalDrops</c> represents the optional drops of the enemy.</value>
            public GameObject[] optionalDrops;
            
        #endregion
        
        #region Keys

            /// <value>Property <c>KeyColors</c> represents the colors of the key.</value>
            private readonly Dictionary<KeyProperties.Colors, bool> _keysObtained = new Dictionary<KeyProperties.Colors, bool>()
            {
                {KeyProperties.Colors.Blue, false},
                {KeyProperties.Colors.Green, false},
                {KeyProperties.Colors.Red, false}
            };

        #endregion

        #region Actions

            /// <value>Property <c>attacking</c> represents whether the character is attacking or not.</value>
            [Header("Character Actions")]
            public bool attacking;

            /// <value>Property <c>aiming</c> represents whether the character is aiming or not.</value>
            public bool aiming;
                    
            /// <value>Property <c>chargingFinished</c> represents whether the animation for charging has finished or not.</value>
            public bool chargingFinished;
                    
            /// <value>Property <c>shooting</c> represents whether the character is shooting or not.</value>
            public bool shooting;

            /// <value>Property <c>hit</c> represents whether the animation for being hit has started or not.</value>
            public bool hit;

            /// <value>Property <c>dead</c> represents whether the character is dead or not.</value>
            public bool dead;

        #endregion
        
        #region Navigating
            
            /// <value>Property <c>wanderTimer</c> represents the wander timer.</value>
            public float wanderTimer;
            
            /// <value>Property <c>fleeTimer</c> represents the flee timer.</value>
            public float fleeTimer;
        
        #endregion
        
        #region Targetting
        
            /// <value>Property <c>targetColliderList</c> represents the list of targets colliding with the player.</value>
            public List<Collider> targetColliderList = new List<Collider>();
        
            /// <value>Property <c>itemColliderList</c> represents the list of items colliding with the player.</value>
            public List<Collider> itemColliderList = new List<Collider>();
        
            /// <value>Property <c>target</c> represents the current target.</value>
            public Transform target;
            
            /// <value>Property <c>forcedTarget</c> represents the forced target.</value>
            public Transform forcedTarget;

        #endregion
        
        #region Aiming

            /// <value>Property <c>normalSensitivity</c> represents the normal sensitivity.</value>
            [Header("Aiming")]
            public float normalSensitivity = 1f;
                
            /// <value>Property <c>aimSensitivity</c> represents the aim sensitivity.</value>
            public float aimSensitivity = 0.5f;
            
            /// <value>Property <c>aimLayerMask</c> represents the aim layer mask.</value>
            public LayerMask aimLayerMask;
        
        #endregion
        
        #region Shooting
        
            /// <value>Property <c>projectilePrefab</c> represents the projectile prefab.</value>
            [Header("Shooting")]
            public Transform projectilePrefab;
                
            /// <value>Property <c>projectileSpawnPoint</c> represents the projectile spawn point.</value>
            public Transform projectileSpawnPoint;
        
        #endregion
        
        #region Animation Rigging
            
            /// <value>Property <c>aimHeadRig</c> represents the aim head rig.</value>
            [Header("Animation Rigging")]
            public MultiAimConstraint aimHeadRig;
            
            /// <value>Property <c>aimUpperBodyRig</c> represents the aim upper body rig.</value>
            public MultiAimConstraint aimUpperBodyRig;

            /// <value>Property <c>aimBodyRig</c> represents the aim body rig.</value>
            public MultiAimConstraint aimBodyRig;
            
            /// <value>Property <c>aimHeadRig</c> represents the aim head rig.</value>
            public MultiAimConstraint aimFistRig;
        
        #endregion
            
        #region Animator References
        
            /// <value>Property <c>AnimatorSpeed</c> represents the character speed animation.</value>
            public readonly int AnimatorSpeed = Animator.StringToHash("Speed");
            
            /// <value>Property <c>AnimatorAiming</c> represents the character aiming animation.</value>
            public readonly int AnimatorMotionSpeed = Animator.StringToHash("MotionSpeed");
            
            /// <value>Property <c>AnimatorAttacking</c> represents the character attacking animation.</value>
            public readonly int AnimatorAttacking = Animator.StringToHash("Attacking");
            
            /// <value>Property <c>AnimatorCharging</c> represents the character charging animation.</value>
            public readonly int AnimatorCharging = Animator.StringToHash("Charging");
            
            /// <value>Property <c>AnimatorShooting</c> represents the character shooting animation.</value>
            public readonly int AnimatorShooting = Animator.StringToHash("Shooting");
            
            /// <value>Property <c>AnimatorHit</c> represents the character hit animation.</value>
            public readonly int AnimatorHit = Animator.StringToHash("Hit");
            
            /// <value>Property <c>AnimatorDead</c> represents the character dead animation.</value>
            public readonly int AnimatorDead = Animator.StringToHash("Dead");

        #endregion
        
        #region Particles
        
            /// <value>Property <c>hit1Particles</c> represents the hit 1 particles.</value>
            [Header("Particles")]
            public ParticleSystem hit1Particles;
            
            /// <value>Property <c>hit2Particles</c> represents the hit 2 particles.</value>
            public ParticleSystem hit2Particles;
            
            /// <value>Property <c>restoreParticles</c> represents the restore particles.</value>
            public ParticleSystem restoreParticles;
            
            /// <value>Property <c>deathParticles</c> represents the death particles.</value>
            public ParticleSystem deathParticles;
            
            /// <value>Property <c>afterDeathParticles</c> represents the after death particles.</value>
            public ParticleSystem afterDeathParticles;

            /// <value>Property <c>rebornParticles</c> represents the reborn particles.</value>
            public ParticleSystem rebornParticles;

            /// <value>Property <c>explodeParticles</c> represents the explode particles.</value>
            public ParticleSystem explodeParticles;
                
        #endregion
        
        #region Sounds
        
            /// <value>Property <c>audioSource</c> represents the audio source.</value>
            [Header("Sounds")]
            public AudioSource audioSource;
        
            /// <value>Property <c>attackSound</c> represents the attack sound.</value>
            public AudioClip attackSound;
            
            /// <value>Property <c>chargeSound</c> represents the charge sound.</value>
            public AudioClip chargeSound;
            
            /// <value>Property <c>shootSound</c> represents the shoot sound.</value>
            public AudioClip shootSound;
            
            /// <value>Property <c>hitSound</c> represents the hit sound.</value>
            public AudioClip hitSound;
            
            /// <value>Property <c>deathSound</c> represents the death sound.</value>
            public AudioClip deathSound;
            
            /// <value>Property <c>screamSound</c> represents the scream sound.</value>
            public AudioClip screamSound;
            
            /// <value>Property <c>detectSound</c> represents the detect sound.</value>
            public AudioClip detectSound;
            
            /// <value>Property <c>rebornSound</c> represents the reborn sound.</value>
            public AudioClip rebornSound;
            
            /// <value>Property <c>explodeSound</c> represents the explode sound.</value>
            public AudioClip explodeSound;
            
        #endregion
        
        #region Cameras
        
            /// <value>Property <c>mainCamera</c> represents the main camera.</value>
            [Header("Cameras")]
            public Camera mainCamera;
            
            /// <value>Property <c>playerFollowCamera</c> represents the player follow camera.</value>
            public CinemachineVirtualCamera playerFollowCamera;

            /// <value>Property <c>playerAimingCamera</c> represents the player aiming camera.</value>
            public CinemachineVirtualCamera playerAimingCamera;
            
            /// <value>Property <c>playerCameraRoot</c> represents the player camera root.</value>
            public Transform playerCameraRoot;
        
        #endregion

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Get the character states
            _characterStates.Add(CharacterProperties.Types.Ally, new Ally(this));
            _characterStates.Add(CharacterProperties.Types.Boss, new Boss(this));
            _characterStates.Add(CharacterProperties.Types.Enemy, new Enemy(this));
            _characterStates.Add(CharacterProperties.Types.Neutral, new Neutral(this));
            _characterStates.Add(CharacterProperties.Types.Player, new Player(this));
            
            // Set the current state
            CurrentState = _characterStates[characterType];
        }

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Get the components
            if (animator == null)
                animator = GetComponent<Animator>();
            
            // Set the character speed
            moveSpeed *= GameManager.Instance.gameSpeed;
            sprintSpeed *= GameManager.Instance.gameSpeed;
            attackRate /= GameManager.Instance.gameSpeed;
            
            // Invoke the current state Start method
            CurrentState.StartState();
            
            // Set the wander timer
            wanderTimer = wanderTime;
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // Invoke the current state Update method
            CurrentState.UpdateState();
        }
        
        #region Health and Shield
        
            /// <summary>
            /// Method <c>TakeDamage</c> is called when the character takes damage.
            /// </summary>
            /// <param name="damage"></param>
            public void TakeDamage(float damage)
            {
                // Invoke the current state TakeDamage method
                StartCoroutine(CurrentState.TakeDamage(damage * damageTakenMultiplier));
            }
            
            /// <summary>
            /// Method <c>RestoreHealth</c> is called when the player restores health.
            /// </summary>
            /// <param name="multiplier">The multiplier of the health.</param>
            public void RestoreHealth(float multiplier)
            {
                StartCoroutine(CurrentState.RestoreHealth(multiplier));
            }
            
            /// <summary>
            /// Method <c>RestoreShield</c> is called when the player restores shield.
            /// </summary>
            /// <param name="multiplier">The multiplier of the shield.</param>
            public void RestoreShield(float multiplier)
            {
                StartCoroutine(CurrentState.RestoreShield(multiplier));
            }
        
        #endregion

        #region Input Action Callbacks

            /// <summary>
            /// Method <c>OnAttack</c> is called when the player presses the attack button.
            /// </summary>
            /// <param name="value">The input value.</param>
            private void OnAttack(InputValue value)
            {
                CurrentState.InputAttack(value.isPressed);
            }
            
            /// <summary>
            /// Method <c>OnAim</c> is called when the player presses the aim button.
            /// </summary>
            /// <param name="value">The input value.</param>
            private void OnAim(InputValue value)
            {
                CurrentState.InputAim(value.isPressed);
            }

            /// <summary>
            /// Method <c>OnShoot</c> is called when the player presses the shoot button.
            /// </summary>
            /// <param name="value">The input value.</param>
            private void OnShoot(InputValue value)
            {
                CurrentState.InputShoot(value.isPressed);
            }
            
        #endregion
        
        #region Animation Callbacks

            /// <summary>
            /// Method <c>OnChargeComplete</c> is called when the animation for charging is completed.
            /// </summary>
            /// <param name="animationEvent">The animation event.</param>
            private void OnChargeComplete(AnimationEvent animationEvent)
            {
                chargingFinished = true;
            }
            
            /// <summary>
            /// Method <c>OnShootComplete</c> is called when the animation for shooting is completed.
            /// </summary>
            /// <param name="animationEvent">The animation event.</param>
            private void OnShootComplete(AnimationEvent animationEvent)
            {
            }
            
            /// <summary>
            /// Method <c>OnAttackComplete</c> is called when the animation for attacking is completed.
            /// </summary>
            /// <param name="animationEvent">The animation event.</param>
            private void OnAttackComplete(AnimationEvent animationEvent)
            {
                CurrentState.AttackFinished();
            }
            
            /// <summary>
            /// Method <c>OnHitComplete</c> is called when the animation for being hit is completed.
            /// </summary>
            /// <param name="animationEvent"></param>
            private void OnHitComplete(AnimationEvent animationEvent)
            {
            }
            
            /// <summary>
            /// Method <c>OnDeadComplete</c> is called when the animation for dying is completed.
            /// </summary>
            /// <param name="animationEvent"></param>
            private void OnDeadComplete(AnimationEvent animationEvent)
            {
                StartCoroutine(CurrentState.DeadFinished());
            }
        
        #endregion
        
        #region Collisions

            /// <summary>
            /// Method <c>OnCollisionEnter</c> is called when the character enters a collision.
            /// </summary>
            /// <param name="col">The collision.</param>
            private void OnCollisionEnter(Collision col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleCollisionEnter(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnCollisionStay</c> is called when the character stays in a collision.
            /// </summary>
            /// <param name="col">The collision.</param>
            private void OnCollisionStay(Collision col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleCollisionStay(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnCollisionExit</c> is called when the character exits a collision.
            /// </summary>
            /// <param name="col">The collision.</param>
            private void OnCollisionExit(Collision col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleCollisionExit(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnTriggerEnter</c> is called when the character enters a trigger.
            /// </summary>
            /// <param name="col">The other collider.</param>
            private void OnTriggerEnter(Collider col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleTriggerEnter(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnTriggerStay</c> is called when the character stays in a trigger.
            /// </summary>
            /// <param name="col">The other collider.</param>
            private void OnTriggerStay(Collider col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleTriggerStay(col, transform.tag);
            }
            
            /// <summary>
            /// Method <c>OnTriggerExit</c> is called when the character exits a trigger.
            /// </summary>
            /// <param name="col">The other collider.</param>
            private void OnTriggerExit(Collider col)
            {
                if (col.transform.parent != transform)
                    CurrentState.HandleTriggerExit(col, transform.tag);
            }

        #endregion
        
        #region Navigating
        
            /// <summary>
            /// Method <c>RandomNavSphere</c> returns a random position on the navmesh.
            /// </summary>
            /// <param name="origin">The origin position.</param>
            /// <param name="distance">The distance from the origin position.</param>
            /// <param name="layermask">The layermask the navmesh is on.</param>
            /// <returns></returns>
            public Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
            {
                var randomDirection = Random.insideUnitSphere * distance;
                randomDirection += origin;
                NavMesh.SamplePosition(randomDirection, out var navHit, distance, layermask);
                return navHit.position;
            }
        
        #endregion

        #region Targetting
        
            /// <summary>
            /// Method <c>GetClosestCollider</c> returns the closest collider from a list of colliders.
            /// </summary>
            /// <param name="colliderList">The list of colliders.</param>
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
                var closestDistance = Vector3.Distance(transform.position, closestCollider.transform.position);
                
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
                    var distance = Vector3.Distance(transform.position, col.transform.position);
                    // If the distance is lesser than the closest distance, update the closest distance and collider
                    if (!(distance < closestDistance))
                        continue;
                    closestDistance = distance;
                    closestCollider = col;
                }
                return (closestCollider.transform, tempColliderList);
            }

            /// <summary>
            /// Method <c>SetTarget</c> sets the target.
            /// </summary>
            public void SetTarget()
            {
                // Store the current target
                var currentTarget = target;
                // Loop the list of targets and remove the null, inactive or dead characters
                var tempTargetColliderList = new List<Collider>(targetColliderList);
                foreach (var col in targetColliderList.Where(col => !col || !col.gameObject.activeSelf || col.transform.GetComponent<Character>().dead))
                {
                    tempTargetColliderList.Remove(col);
                }
                targetColliderList = tempTargetColliderList;
                // Get the closest collider transform
                var (closestTarget, updatedTargetList) = GetClosestCollider(targetColliderList);
                // Update the list of colliders
                targetColliderList = updatedTargetList;
                // Set the target
                target = closestTarget;
                // If the target is different from the current target, play the detection sound
                if (target == currentTarget || target == null)
                    return;
                switch (CurrentState)
                {
                    case Neutral:
                        HandlePlaySound(screamSound);
                        break;
                    default:
                        HandlePlaySound(detectSound);
                        break;
                }
            }
                
            /// <summary>
            /// Method <c>LookAtClosestItem</c> makes the character look at the closest item.
            /// </summary>
            public void LookAtClosestItem()
            {
                // Get the closest collider transform
                var (closestItem, updatedItemList) = GetClosestCollider(itemColliderList);
                // Update the list of colliders
                itemColliderList = updatedItemList;
                // Update the aim rig
                var sources = new WeightedTransformArray(0);
                if (closestItem != null)
                    sources.Add(new WeightedTransform(closestItem, 1));
                aimHeadRig.data.sourceObjects = sources;
                aimUpperBodyRig.data.sourceObjects = sources;
                animator.enabled = false;
                rigBuilder.Build();
                animator.enabled = true;
            }
            
        #endregion

        /// <summary>
        /// Method <c>ResetAllProperties</c> resets all the properties of the character.
        /// TODO: Some of these properties are hardcoded. They should be set as public properties.
        /// TODO: Also, there are other properties that are not reset. They should be reset as well.
        /// </summary>
        private void ResetAllProperties()
        {
            // Reset the character settings
            health = maxHealth;
            shield = maxShield;
            lastAttackTime = 0;
            lastShootTime = 0;
            
            // Reset the action settings
            attacking = false;
            aiming = false;
            chargingFinished = false;
            shooting = false;
            hit = false;
            dead = false;
            
            // Reset the navigating settings
            wanderTimer = 0;
            fleeTimer = 0;
            
            // Reset the targetting settings
            targetColliderList = new List<Collider>();
            itemColliderList = new List<Collider>();
            target = null;
            forcedTarget = null;
        }

        /// <summary>
        /// Method <c>RemovePlayerComponents</c> removes the player components.
        /// TODO: This method is hardcoded. It should be improved.
        /// </summary>
        private void RemovePlayerComponents()
        {
            // Return if the character is not a player
            if (CurrentState != _characterStates[CharacterProperties.Types.Player])
                return;
            
            // Set a list of the types of components that should be disabled
            var componentTypesToDisable = new List<string>
            {
                "CharacterController",
                "CustomThirdPersonController",
                "PlayerInput",
                "StarterAssetsInputs",
                "RigBuilder",
                "BoneRenderer"
            };
            
            // Set a list of the types of components that should be enabled
            var componentTypesToEnable = new List<string>
            {
                "CapsuleCollider",
                "NavMeshAgent"
            };
            
            // Disable the component types
            foreach (var componentType in componentTypesToDisable)
            {
                var component = GetComponent(componentType);
                if (component)
                    component.GetType().GetProperty("enabled")?.SetValue(component, false, null);
            }
            
            // Enable the component types
            foreach (var componentType in componentTypesToEnable)
            {
                var component = GetComponent(componentType);
                if (component)
                    component.GetType().GetProperty("enabled")?.SetValue(component, true, null);
            }
        }

        /// <summary>
        /// Method <c>ChangeType</c> changes the type of the character.
        /// </summary>
        public void ChangeType(CharacterProperties.Types charType)
        {
            // Change the tag and type
            characterType = charType;
            tag = charType.ToString();
            
            // Reset all properties
            ResetAllProperties();

            // If the character is a player, reset the components
            if (CurrentState == _characterStates[CharacterProperties.Types.Player])
                RemovePlayerComponents();
            
            // Change the state
            CurrentState = _characterStates[characterType];
            
            // Invoke the new state Start method
            CurrentState.StartState();
        }
        
        /// <summary>
        /// Method <c>GetKey</c> gets a key.
        /// </summary>
        /// <param name="keyColor"></param>
        public void GetKey(KeyProperties.Colors keyColor)
        {
            _keysObtained[keyColor] = true;
            GameManager.Instance.SummonBoss(
                _keysObtained[KeyProperties.Colors.Red],
                _keysObtained[KeyProperties.Colors.Green],
                _keysObtained[KeyProperties.Colors.Blue]
            );
            UIManager.Instance.UpdateKeyUI(
                _keysObtained[KeyProperties.Colors.Red],
                _keysObtained[KeyProperties.Colors.Green],
                _keysObtained[KeyProperties.Colors.Blue]
            );
        }
        
        /// <summary>
        /// Method <c>HasKey</c> checks if the player has a key.
        /// </summary>
        /// <param name="keyColor">The color of the key.</param>
        public bool HasKey(KeyProperties.Colors keyColor)
        {
            return _keysObtained[keyColor];
        }
        
        /// <summary>
        /// Method <c>HasAllKeys</c> checks if the player has all the keys.
        /// </summary>
        public bool HasAllKeys()
        {
            return _keysObtained[KeyProperties.Colors.Red] &&
                   _keysObtained[KeyProperties.Colors.Green] &&
                   _keysObtained[KeyProperties.Colors.Blue];
        }
        
        /// <summary>
        /// Method <c>HandlePlaySound</c> tries to play a sound.
        /// </summary>
        /// <param name="clip"></param>
        public void HandlePlaySound(AudioClip clip)
        {
            if (clip == null)
                return;
            audioSource.PlayOneShot(clip);
        }
        
        /// <summary>
        /// Method <c>OnPause</c> is called when the player pauses the game.
        /// </summary>
        private void OnPause()
        {
            GameManager.Instance.TogglePause();
        }
        
        /// <summary>
        /// Method <c>ToggleInput</c> toggles the input.
        /// </summary>
        public void ToggleInput()
        {
            playerInputs.cursorLocked = !playerInputs.cursorLocked;
            playerInputs.cursorInputForLook = !playerInputs.cursorInputForLook;
        }
    }
}
