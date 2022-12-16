using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace EZVibrations
{
    [AddComponentMenu("EZ Vibrations/Gamepad Vibrations")]
    public class Vibrations : MonoBehaviour
    {
        /// <summary>
        /// The single instance of the CameraShaker in the current scene. Do not use if you have multiple instances.
        /// </summary>
        public static Vibrations Instance;
        static Dictionary<string, Vibrations> instanceList = new Dictionary<string, Vibrations>();

        List<VibrationsInstance> vibrationsInstances = new List<VibrationsInstance>();

        void Awake()
        {
            Instance = this;
            instanceList.Add(gameObject.name, this);
        }

        private void Update()
        {
            Debug.Log(vibrationsInstances.Count);
            //controller vibration
            for (int i = 0; i < vibrationsInstances.Count; i++)
            {
                if (vibrationsInstances[i].whichController != null)
                {
                    if (vibrationsInstances[i].whichController.GetDevice<Gamepad>() == null) return;
                        Gamepad gamepad = vibrationsInstances[i].whichController.GetDevice<Gamepad>();

                    if (gamepad == Gamepad.current)
                    {
                        if (vibrationsInstances[i].vibroTimer > 0)
                        {
                            vibrationsInstances[i].vibroTimer -= Time.deltaTime;
                            Debug.Log(vibrationsInstances[i].MagnitudeLeft);
                            Debug.Log(gamepad);

                            gamepad.SetMotorSpeeds(vibrationsInstances[i].MagnitudeLeft, vibrationsInstances[i].MagnitudeRight);
                        }
                        else
                        {
                            gamepad.SetMotorSpeeds(0, 0);
                            vibrationsInstances.RemoveAt(i);
                        }
                    }
                }
            }
        }
        public VibrationsInstance VibrateOnce(float magnitudeLeft, float magnitudeRight, PlayerInput whichPlayer, float timer)
        {
            VibrationsInstance shake = new VibrationsInstance(magnitudeLeft, magnitudeRight, whichPlayer, timer);
            vibrationsInstances.Add(shake);
            return shake;
        }

        void OnDestroy()
        {
                instanceList.Remove(gameObject.name);
        }
    }
}