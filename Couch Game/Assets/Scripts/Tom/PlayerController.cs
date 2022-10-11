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
    }
    public References refs;

    bool isMoving;
    [SerializeField] private bool isSpinning;

    #region Updates

        void FixedUpdate()
        {
            if(!CheckAllStatus())
            {
                isMoving = input.move != Vector2.zero;

                Movement();
                ApplyMovement();
                Countering();
                TimerLastPlayer();
            }
        }

    #endregion

    #region Player Interactions
        [System.Serializable] public class PlayersInteract
        {
            public PlayerController lastPlayerContact;
            public int playerPoint = 0;

            public float lastPlayerTimer;
            public float maxLastPlayerTimer = 5f;
        }

        public void TimerLastPlayer()
        {
            if(playersInteract.lastPlayerTimer <= 0)
            {
                UpdateLastPlayerTouched(null);
            }
            else
            {
                playersInteract.lastPlayerTimer -= Time.deltaTime;
            }
        }
        public void UpdateLastPlayerTouched(PlayerController player)
        {
            playersInteract.lastPlayerContact = player;
        }
    #endregion
    public PlayersInteract playersInteract;


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

                Quaternion q = Quaternion.LookRotation(moveDirection.normalized);
            
                refs.mesh.rotation = Quaternion.RotateTowards(refs.mesh.rotation, q, movement.rotationSpeed * Time.deltaTime);
            }
            else
            {
                move = Vector2.MoveTowards(move, Vector2.zero, movement.dx);
            }
            //if (moveDirection == Vector3.zero) return;
            
        }

        void ApplyMovement()
        {
            refs.rb.velocity = new Vector3(move.x, refs.rb.velocity.y, move.y);
        }

    #endregion
    

    #region Collect Inputs

        [System.Serializable] public class Inputs
        {
            public Vector2 move;
            public bool counter;
        }

        [HideInInspector] public Inputs input;


        void OnMove(InputValue v)
        {
            input.move = v.Get<Vector2>();
        }

        void OnSpin(InputValue v)
        {
            Debug.Log("spin");
        }

        void OnQuickSpin(InputValue v)
        {
            Debug.Log("quick spin");
        }

        private void OnCounter(InputValue v)
        {
            if(v.isPressed)input.counter = true;

            if(!counter.prepCounter)
            {
                counter.counterPrepTime = counter.maxCounterTime;
                counter.prepCounter = true;
            }
        }

    #endregion


    #region Attack
        [System.Serializable] public class CounterSettings
        {
            public bool prepCounter;
            public float counterPrepTime;
            public float maxCounterTime = 2f;
            public float counteringTime;
            public float maxCounteringTime = .5f;
            public bool isCountering;

            [Header("Hitbox")]
            public Transform counterPos;
            public float size;
            public float maxDistance;
            public LayerMask playerLM;

            public float counterForce;
        }

        public CounterSettings counter;
        private RaycastHit hit;

        private void Countering()
        {
            //Prepare to counter
            if(counter.prepCounter)
            {
                if(counter.counterPrepTime > 0)
                {
                    counter.counterPrepTime -= Time.deltaTime * counter.maxCounterTime;
                    //refs.rb.velocity = Vector3.zero;
                    //Debug.Log("COUNTERING INCOMING");
                }
                else
                {
                    input.counter = false;
                    counter.prepCounter = false;
                    counter.isCountering = true;
                    counter.counteringTime = counter.maxCounteringTime;
                    Debug.Log("START COUNTERING");
                }
            }

            //is actually countering
            if(counter.isCountering)
            {
                if(counter.counteringTime > 0)
                {
                    bool hitted = Physics.Raycast(counter.counterPos.position, counter.counterPos.forward, out hit, counter.maxDistance, counter.playerLM);
                    if(hitted)
                    {
                        //Debug.Log("HIT SOMETHING");
                        //Debug.Log(hit.collider.name);
                        //Debug.Log("PLAYER COUNTERED");
                        PlayerController enemyPlayer = hit.collider.GetComponent<PlayerController>();
                        if(enemyPlayer.isSpinning)
                        {
                            Debug.Log("PLAYER STUNNED");
                            enemyPlayer.status.stunedTime = 2f;
                        }
                        else
                        {
                            Debug.Log("PLAYER REPOUSSED");
                            enemyPlayer.refs.rb.AddForce(-(transform.position - enemyPlayer.transform.position).normalized * counter.counterForce, ForceMode.Impulse);
                        }

                        //Set Last touched player timer
                        enemyPlayer.UpdateLastPlayerTouched(this);
                        enemyPlayer.playersInteract.lastPlayerTimer = enemyPlayer.playersInteract.maxLastPlayerTimer;
                    }
                    counter.counteringTime -= Time.deltaTime;
                }
                else
                {
                    counter.isCountering = false;
                    Debug.Log("STOP COUNTERING");
                }
            }
        }

    #endregion

    #region Status

    public Status status;
    [System.Serializable] public class Status
    {
        [Header("Stun")]
        public float stunedTime;
    }

    private bool CheckAllStatus()
    {
        if(status.stunedTime > 0)
        {
            status.stunedTime -= Time.deltaTime;
            return true;
        }
        else return false;
    }
    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if(counter.counterPos != null)Gizmos.DrawRay(counter.counterPos.position, counter.counterPos.forward * counter.maxDistance);
    }
    #endregion
}   
