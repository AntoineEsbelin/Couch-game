using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToupieController : MonoBehaviour
{
    [System.Serializable] public class References
        {
            public Rigidbody rb;
            public Transform orientation;
            public Transform mesh;
        }
        public References refs;
    
        bool isMoving;
    
        #region Updates
    
            void FixedUpdate()
            {
                isMoving = input.move != Vector3.zero;
    
                Movement();
                ApplyMovement();
            }
    
        #endregion
    
    
        #region Move Player
    
            [System.Serializable] public class MovementSettings
            {
                public float speed = 8f;
                public float dx = 4f; //décélération
                public float rotationSpeed = 800f;
            }
            public MovementSettings movement;
    
            Vector2 move;
    
            void Movement()
            {
                var moveDirection = refs.orientation.forward * input.move.y + refs.orientation.right * input.move.x;
    
                if (isMoving)
                {
                    move = input.move * movement.speed;
                }
                else
                {
                    move = Vector2.MoveTowards(move, Vector2.zero, movement.dx);
                }
                //if (moveDirection == Vector3.zero) return;
                Quaternion q = Quaternion.LookRotation(moveDirection.normalized);
                
                refs.mesh.rotation = Quaternion.RotateTowards(refs.mesh.rotation, q, movement.rotationSpeed * Time.deltaTime);
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
            }
    
            [HideInInspector] public Inputs input;
    
    
            void OnMove(InputValue v)
            {
                input.move = v.Get<Vector3>();
            }
    
        #endregion
}
