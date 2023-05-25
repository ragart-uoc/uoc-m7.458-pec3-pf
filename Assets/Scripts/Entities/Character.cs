using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using PEC3.Entities.CharacterStates;
using UnityEngine.InputSystem;

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
        
        #region Character Settings

            /// <value>Property <c>life</c> represents the life of the character.</value>
            public float life = 100f;
            
        #endregion
        
        #region Character Actions and Animations

            /// <value>Property <c>attacking</c> represents wether the character is attacking or not.</value>
            public bool attacking;
            
            /// <value>Property <c>charging</c> represents wether the character is charging or not.</value>
            public bool charging;
            
            /// <value>Property <c>chargingFinished</c> represents wether the character has finished charging or not.</value>
            public bool chargingFinished;
            
            /// <value>Property <c>shooting</c> represents wether the character is shooting or not.</value>
            public bool shooting;

            /// <value>Property <c>animator</c> represents the character animator.</value>
            [HideInInspector]
            public Animator animator;
            
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
            public Camera mainCamera;
            
            /// <value>Property <c>playerFollowCamera</c> represents the player follow camera.</value>
            public CinemachineVirtualCamera playerFollowCamera;

            /// <value>Property <c>playerAimingCamera</c> represents the player aiming camera.</value>
            public CinemachineVirtualCamera playerAimingCamera;
        
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
            // Get the character animator
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

        /// <summary>
        /// Method <c>OnAttack</c> is called when the player presses the attack button.
        /// </summary>
        /// <param name="value">The input value.</param>
        private void OnAttack(InputValue value)
        {
            CurrentState.InputAttack(value);
        }

        /// <summary>
        /// Method <c>OnShoot</c> is called when the player presses the shoot button.
        /// </summary>
        /// <param name="value">The input value.</param>
        private void OnShoot(InputValue value)
        {
            CurrentState.InputShoot(value);
        }

        /// <summary>
        /// Method <c>OnChargeComplete</c> is called when the player finishes charging. It cames from an animation event.
        /// </summary>
        /// <param name="animationEvent">The animation event.</param>
        private void OnChargeComplete(AnimationEvent animationEvent)
        {
            chargingFinished = true;
        }
        
        /// <summary>
        /// Method <c>OnShootComplete</c> is called when the player finishes shooting. It cames from an animation event.
        /// </summary>
        /// <param name="animationEvent">The animation event.</param>
        private void OnShootComplete(AnimationEvent animationEvent)
        {
            shooting = false;
            animator.SetBool(AnimatorShooting, false);
        }
    }
}
