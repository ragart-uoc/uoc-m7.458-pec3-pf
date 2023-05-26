using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using Cinemachine;
using PEC3.Controllers;
using PEC3.Entities.CharacterStates;

namespace PEC3.Entities
{
    /// <summary>
    /// Class <c>Character</c> is the base class for all characters.
    /// </summary>
    public class Character : MonoBehaviour
    {
        #region Character States
            
            /// <value>Property <c>CharacterStates</c> represents the list of character states.</value>
            public enum CharacterType
            {
                Ally,
                Enemy,
                Neutral,
                Player
            }
            
            /// <value>Property <c>characterType</c> represents the character type.</value>
            public CharacterType characterType;
        
            /// <value>Property <c>_characterStates</c> represents the list of character states.</value>
            private readonly Dictionary<CharacterType, ICharacterState> _characterStates = new Dictionary<CharacterType, ICharacterState>();
            
            /// <value>Property <c>CurrentState</c> represents the current character state.</value>
            public ICharacterState CurrentState;
            
        #endregion
        
        /// <value>Property <c>controller</c> represents the custom third person controller.</value>
        public CustomThirdPersonController controller;

        /// <value>Property <c>animator</c> represents the character animator.</value>
        public Animator animator;
        
        #region Character Settings

            /// <value>Property <c>life</c> represents the life of the character.</value>
            [Header("Character Settings")]
            public float life = 100f;
            
            /// <value>Property <c>shield</c> represents the shield of the character.</value>
            public float shield = 100f;
            
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
            
        /// <value>Property <c>shootingStarted</c> represents whether the animation for shooting has started or not.</value>
        public bool shootingStarted;
            
        /// <value>Property <c>shootingFinished</c> represents whether the animation for shooting has finished or not.</value>
        public bool shootingFinished;

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

            /// <value>Property <c>rigBuilder</c> represents the rig builder.</value>
            [Header("Animation Rigging")]
            public RigBuilder rigBuilder;
            
            /// <value>Property <c>aimHeadRig</c> represents the aim head rig.</value>
            public MultiAimConstraint aimHeadRig;
            
            /// <value>Property <c>aimUpperBodyRig</c> represents the aim upper body rig.</value>
            public MultiAimConstraint aimUpperBodyRig;

            /// <value>Property <c>aimBodyRig</c> represents the aim body rig.</value>
            public MultiAimConstraint aimBodyRig;
            
            /// <value>Property <c>aimHeadRig</c> represents the aim head rig.</value>
            public MultiAimConstraint aimFistRig;
        
        #endregion
            
        #region Animation References
            
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
            _characterStates.Add(CharacterType.Ally, new Ally(this));
            _characterStates.Add(CharacterType.Enemy, new Enemy(this));
            _characterStates.Add(CharacterType.Neutral, new Neutral(this));
            _characterStates.Add(CharacterType.Player, new Player(this));
            
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
            
            // Invoke the current state Start method
            CurrentState.StartState();
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // Invoke the current state Update method
            CurrentState.UpdateState();
        }
        
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
                shootingFinished = true;
            }
            
            /// <summary>
            /// Method <c>OnAttackComplete</c> is called when the animation for attacking is completed.
            /// </summary>
            /// <param name="animationEvent">The animation event.</param>
            private void OnAttackComplete(AnimationEvent animationEvent)
            {
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
    }
}
