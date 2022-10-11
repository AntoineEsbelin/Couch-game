using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [System.Serializable] public class References
    {
        public Rigidbody rb;
        public Transform orientation;
        public Transform mesh;
        public TrailRenderer trail;
    }
    public References refs;

    bool isMoving;

    bool dashing;
    bool canDash;
    Vector3 dashDir;

    #region Updates

        void FixedUpdate()
        {
            isMoving = input.move != Vector3.zero;

            if (input.spinRelease)
            {
                input.spinRelease = false;
                dashing = true;
                //dashDir = refs.orientation.forward;
                if (input.move == Vector3.zero)
                {
                    
                }
                dashDir = input.move;
                dashDuration = movement.dashDurationMax; //mettre une fonction qui calcule le temps de dash avec le temps de charge du spin
            }

            Movement();
            ApplyMovement();
            if (dashDuration > 0) DashMovement();

            #region Timers

                if (dashDuration > 0) dashDuration = Mathf.Clamp(dashDuration - Time.deltaTime, 0, movement.dashDurationMax);

            #endregion
        }

    #endregion


    #region Move Player

        [System.Serializable] public class MovementSettings
        {
            public float speed = 8f;
            public float dx = 4f; //décélération
            public float rotationSpeed = 800f;

            public float dashSpeedMax = 30f;
            public float dashDurationMax = 3f;
        }
        public MovementSettings movement;

        Vector3 move;

        float dashDuration;

        void Movement()
        {
            var moveDirection = refs.orientation.forward * input.move.y + refs.orientation.right * input.move.x;

            if (dashDuration > 0) return;

            if (isMoving)
            {
                if (input.spinCharge)
                    move = Vector3.zero;
                else
                    move = input.move * movement.speed;

                Quaternion q = Quaternion.LookRotation(moveDirection.normalized);
            
                refs.mesh.rotation = Quaternion.RotateTowards(refs.mesh.rotation, q, movement.rotationSpeed * Time.deltaTime);
            }
            else
            {
                move = Vector3.MoveTowards(move, Vector2.zero, movement.dx);
            }
            //if (moveDirection == Vector3.zero) return;
            
        }

        void DashMovement()
        {
            Vector3 forward = refs.orientation.TransformDirection(Vector3.forward); 
            move = forward * movement.dashSpeedMax;
            Debug.Log(move);
        }

        void ApplyMovement()
        {
            refs.rb.velocity = new Vector3(move.x, 0, move.y);
        }

    #endregion
    

    #region Collect Inputs

        [System.Serializable] public class Inputs
        {
            public Vector3 move;
            public bool spinCharge;
            public bool spinRelease;
        }

        [HideInInspector] public Inputs input;


        public void OnMove(InputAction.CallbackContext ctx)
        {
            input.move = ctx.ReadValue<Vector2>();
        }

        public void OnSpin(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
                input.spinCharge = true;
            
            if (ctx.canceled)
            {
                input.spinCharge = false;
                input.spinRelease = true;
            }
        }

        // public void OnQuickSpin(InputAction.CallbackContext ctx)
        // {
        //     Debug.Log("quick spin");
        // }

    #endregion
}
