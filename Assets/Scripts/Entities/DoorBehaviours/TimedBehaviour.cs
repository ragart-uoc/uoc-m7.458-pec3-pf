using UnityEngine;
using PEC3.Managers;

namespace PEC3.Entities.DoorBehaviours
{
    /// <summary>
    /// Class <c>TimedBehaviour</c> represents the behaviour of a door that opens for a certain amount of time after pressing a switch.
    /// </summary>
    public class TimedBehaviour : IDoorBehaviour
    {
        /// <value>Property <c>_door</c> represents the door.</value>
        private readonly Door _door;

        /// <value>Property <c>_timer</c> represents the timer.</value>
        private float _timer;
        
        private bool _timerIsRunning;

        /// <summary>
        /// Method <c>TimedBehaviour</c> is the constructor of the class.
        /// </summary>
        /// <param name="door">The door.</param>
        public TimedBehaviour(Door door)
        {
            _door = door;
        }

        /// <summary>
        /// Method <c>UpdateBehaviour</c> updates the behaviour of the door using the parent update method.
        /// </summary>
        public void UpdateBehaviour()
        {
            if (_door.timerButtonPressed)
            {
                _door.timerButtonPressed = false;
                _timer = _door.maxTime;
                _timerIsRunning = true;
                _door.isOpening = true;
                _door.isClosing = false;
            }

            if (!_timerIsRunning)
                return;
            
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
                UIManager.Instance.UpdateTimerText(_timer, true);
            }

            if (_timer <= 0f)
            {
                _timer = 0f;
                _timerIsRunning = false;
                _door.isOpening = false;
                _door.isClosing = true;
                UIManager.Instance.UpdateTimerText(0f, false);
            }
        }

        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the Collider other enters the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        public void OnTriggerEnter(Collider col)
        {
            if (!col.gameObject.CompareTag("Player"))
                return;
            if (_timer <= 0f)
            {
                UIManager.Instance.UpdateMessageText("Switch needed", 2f);
            }
        }
        
        /// <summary>
        /// Method <c>OnTriggerStay</c> is called once per frame for every Collider other that is touching the trigger.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        public void OnTriggerStay(Collider col) {}
        
        /// <summary>
        /// Method <c>OnTriggerExit</c> is called when the Collider other has stopped touching the trigger.
        /// </summary>
        public void OnTriggerExit(Collider col) {}
    }
}
