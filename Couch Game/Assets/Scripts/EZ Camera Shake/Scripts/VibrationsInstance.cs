using UnityEngine;
using UnityEngine.InputSystem;

namespace EZVibrations
{
    public class VibrationsInstance
    {
        /// <summary>
        /// The intensity of the shake left.
        /// </summary>
        public float MagnitudeLeft;

        /// <summary>
        /// Roughness of the shake right.
        /// </summary>
        public float MagnitudeRight;

        /// <summary>
        /// Which controller is affected.
        /// </summary>
        public PlayerInput whichController;

        /// <summary>
        /// Duration of vibrations.
        /// </summary>
        public float vibroTimer;


        /// <summary>
        /// Will create a new instance that will shake once and fade over the given number of seconds.
        /// </summary>
        /// <param name="magnitudeLeft">The intensity of the shake left.</param>
        /// <param name="magnitudeRight">The intensity of the shake right.</param>
        /// <param name="whichPlayer">Which controller is affected.</param>
        /// <param name="timer">Duration of vibrations.</param>
        public VibrationsInstance(float magnitudeLeft, float magnitudeRight, PlayerInput whichPlayer, float timer)
        {
            MagnitudeLeft = magnitudeLeft;
            MagnitudeRight = magnitudeRight;
            whichController = whichPlayer;
            vibroTimer = timer;
        }
    }
}