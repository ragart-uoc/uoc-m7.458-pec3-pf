using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PEC3.Entities.CharacterStates
{
    /// <summary>
    /// Interface <c>ICharacterState</c> is the interface for the character states.
    /// </summary>
    public interface ICharacterState
    {
        /// <value>Property <c>TargetTags</c> represents the tags of the targets.</value>
        List<string> TargetTags { get; set; }

        /// <summary>
        /// Method <c>StartState</c> invokes the state Start method.
        /// </summary>
        void StartState();
        
        /// <summary>
        /// Method <c>UpdateState</c> invokes the state Update method.
        /// </summary>
        void UpdateState();
        
        /// <summary>
        /// Method <c>Move</c> invokes the state Move method.
        /// </summary>
        void Move();

        /// <summary>
        /// Method <c>Wander</c> invokes the state Wander method.
        /// </summary>
        void Wander();

        /// <summary>
        /// Method <c>Flee</c> invokes the state Flee method.
        /// </summary>
        void Flee(Transform target);

        /// <summary>
        /// Method <c>Chase</c> invokes the state Chase method.
        /// </summary>
        void Chase(Transform target);

        /// <summary>
        /// Method <c>Attack</c> invokes the state Attack method.
        /// </summary>
        IEnumerator Attack();
        
        /// <summary>
        /// Method <c>AttackFinished</c> invokes the state AttackFinished method.
        /// </summary>
        void AttackFinished();

        /// <summary>
        /// Method <c>Shoot</c> invokes the state Shoot method.
        /// </summary>
        /// <param name="projectileSpawnPointPosition">The projectile spawn point position.</param>
        /// <param name="projectileAimDirection">The projectile aim direction.</param>
        IEnumerator Shoot(Vector3 projectileSpawnPointPosition, Vector3 projectileAimDirection);

        /// <summary>
        /// Method <c>Hit</c> invokes the state Hit method.
        /// </summary>
        IEnumerator TakeDamage(float damage);
        
        /// <summary>
        /// Method <c>RestoreHealth</c> invokes the state RestoreHealth method.
        /// </summary>
        /// <param name="multiplier">The multiplier of the health.</param>
        IEnumerator RestoreHealth(float multiplier);
        
        /// <summary>
        /// Method <c>RestoreShield</c> invokes the state RestoreShield method.
        /// </summary>
        /// <param name="multiplier">The multiplier of the shield.</param>
        IEnumerator RestoreShield(float multiplier);

        /// <summary>
        /// Method <c>Die</c> invokes the state Die method.
        /// </summary>
        IEnumerator Die();
        
        /// <summary>
        /// Method <c>DeadFinished</c> invokes the state DeadFinished method.
        /// </summary>
        IEnumerator DeadFinished();

        /// <summary>
        /// Method <c>DropObject</c> invokes the state DropObject method.
        /// </summary>
        void DropItem();

        /// <summary>
        /// Method <c>InputAttack</c> invokes the state OnAttack method.
        /// </summary>
        /// <param name="value">The input value.</param>
        void InputAttack(bool value);
        
        /// <summary>
        /// Method <c>InputAim</c> invokes the state OnAim method.
        /// </summary>
        /// <param name="value"></param>
        void InputAim(bool value);

        /// <summary>
        /// Method <c>InputShoot</c> invokes the state OnShoot method.
        /// </summary>
        /// <param name="value">The input value.</param>
        void InputShoot(bool value);
        
        /// <summary>
        /// Method <c>HandleCollisionEnter</c> invokes the state OnCollisionEnter method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleCollisionEnter(Collision col, string tag);
        
        /// <summary>
        /// Method <c>HandleCollisionStay</c> invokes the state OnCollisionStay method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleCollisionStay(Collision col, string tag);
        
        /// <summary>
        /// Method <c>HandleCollisionExit</c> invokes the state OnCollisionExit method.
        /// </summary>
        /// <param name="col">The collision.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleCollisionExit(Collision col, string tag);
        
        /// <summary>
        /// Method <c>HandleTriggerEnter</c> invokes the state OnTriggerEnter method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleTriggerEnter(Collider col, string tag);
        
        /// <summary>
        /// Method <c>HandleTriggerStay</c> invokes the state OnTriggerStay method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleTriggerStay(Collider col, string tag);
        
        /// <summary>
        /// Method <c>HandleTriggerExit</c> invokes the state OnTriggerExit method.
        /// </summary>
        /// <param name="col">The other collider.</param>
        /// <param name="tag">The tag of the game object containing the collider.</param>
        void HandleTriggerExit(Collider col, string tag);
    }
}
