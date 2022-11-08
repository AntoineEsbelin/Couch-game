using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerOld : MonoBehaviour
{
    /*
    [System.Serializable] public class References
    {
        public Rigidbody rb;
        public Transform orientation;
        public Transform mesh;
        public TrailRenderer trail;
        public CapsuleCollider collider;
    }
    public References refs;

    bool isMoving;
    [SerializeField] private bool isSpinning;
    

    bool dashing;
    bool canDash;
    Vector3 dashDir;

    public Color playerStartingColor;

    #region Updates

    
        void FixedUpdate()
        {
            if(!CheckAllStatus())
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
                
                // juste en dessous est le déplacement du joueur que j'ai fait tu peux enlever stv tkt
                if (input.move.magnitude >= 0.1f && input.spinCharge == false)
                {
                    float targetAngle = Mathf.Atan2(input.move.x, input.move.z) * Mathf.Rad2Deg;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, 
                        ref movement.turnSmoothVelocity, movement.turnSmoothTime);
            
                    transform.rotation = Quaternion.Euler(0f, angle, 0f);
                    Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

                    refs.rb.velocity = new Vector3(moveDir.x,0f,moveDir.z)* movement.speed * Time.fixedDeltaTime;
                }

                // Movement();
                // ApplyMovement();
                if (dashDuration > 0) DashMovement();

                #region Timers

                    if (dashDuration > 0) dashDuration = Mathf.Clamp(dashDuration - Time.deltaTime, 0, movement.dashDurationMax);

                #endregion

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
            if(playersInteract.lastPlayerContact != null)
            {
                if(playersInteract.lastPlayerTimer <= 0)
                {
                    refs.mesh.GetComponentInChildren<MeshRenderer>().material.color = playerStartingColor;
                    UpdateLastPlayerTouched(null);
                }
                else
                {
                    playersInteract.lastPlayerTimer -= Time.deltaTime;
                }
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
            [Range(.01f, .5f)]public float turnSmoothTime = 0.1f;
            [HideInInspector] public float turnSmoothVelocity;

            public float dashSpeedMax = 30f;
            public float dashDurationMax = 3f;
        }
        public MovementSettings movement;

        Vector3 move;

        float dashDuration;

        void Movement()
        {
            //Ne marchais pas du tout quand j'ai testé avec manette et clavier (j'ai changé ton input Action et mis les valeur en Vector 3)
            
            // var moveDirection = refs.orientation.forward * input.move.y + refs.orientation.right * input.move.x;
            //
            // if (dashDuration > 0) return;
            //
            // if (isMoving)
            // {
            //     if (input.spinCharge)
            //         move = Vector3.zero;
            //     else
            //         move = input.move * movement.speed;
            //
            //     Quaternion q = Quaternion.LookRotation(moveDirection.normalized);
            //
            //     refs.mesh.rotation = Quaternion.RotateTowards(refs.mesh.rotation, q, movement.rotationSpeed * Time.deltaTime);
            // }
            // else
            // {
            //     move = Vector3.MoveTowards(move, Vector3.zero, movement.dx);
            // }
            //if (moveDirection == Vector3.zero) return;
            
           
            
        }

        void DashMovement()
        {
            Vector3 forward = refs.orientation.TransformDirection(Vector3.forward); 
            move = forward * movement.dashSpeedMax;
            
        }

        void ApplyMovement()
        {
            refs.rb.velocity = new Vector3(move.x, refs.rb.velocity.y, move.y);
        }

    #endregion
    

    #region Collect Inputs

        [System.Serializable] public class Inputs
        {
            public Vector3 move;
            public bool spinCharge;
            public bool spinRelease;
            [HideInInspector]public int playerID = 0;
            public Vector3 startPos;
        }

        [HideInInspector] public Inputs input;

        private void Start()
        {
            transform.position = input.startPos;
            playerStartingColor = refs.mesh.GetComponentInChildren<MeshRenderer>().material.color;
        }
        public void OnMove(InputAction.CallbackContext ctx)
        {
            input.move = ctx.ReadValue<Vector3>();
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

        public void OnCounter(InputAction.CallbackContext ctx)
        {
            if(ctx.performed)
            {
                if(counter.prepCounter == false && counter.isCountering == false)
                {
                    counter.prepCounter = true;
                    counter.counterPrepTime = counter.maxCounterTime;
                    
                    refs.mesh.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
                }
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
                    counter.prepCounter = false;
                    counter.isCountering = true;
                    counter.counteringTime = counter.maxCounteringTime;
                    refs.mesh.GetComponentInChildren<MeshRenderer>().material.color = Color.magenta;
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
                        
                        enemyPlayer.GetComponentInChildren<MeshRenderer>().material.color = Color.red;

                        //Set Last touched player timer
                        enemyPlayer.UpdateLastPlayerTouched(this);
                        enemyPlayer.playersInteract.lastPlayerTimer = enemyPlayer.playersInteract.maxLastPlayerTimer;
                    }
                    counter.counteringTime -= Time.deltaTime;
                }
                else
                {
                    counter.isCountering = false;
                    refs.mesh.GetComponentInChildren<MeshRenderer>().material.color = playerStartingColor;
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
    
    public IEnumerator DeathState()
    {
        refs.mesh.gameObject.SetActive(false);
        refs.rb.isKinematic = true;
        refs.collider.enabled = false;
        transform.position = PlayerSpawnManager.instance.spawnLocations[input.playerID - 1].position;
        yield return new WaitForSeconds(3f);
        refs.mesh.gameObject.SetActive(true);
        refs.rb.isKinematic = false;
        refs.collider.enabled = true;
    }
    #endregion

    #region Gizmos

    private void OnDrawGizmos()
    {
        if(counter.counterPos != null)Gizmos.DrawRay(counter.counterPos.position, counter.counterPos.forward * counter.maxDistance);
    }
    #endregion

    */
}   
