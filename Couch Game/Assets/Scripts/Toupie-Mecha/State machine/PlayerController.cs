using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    #region States
        public PlayerStateMachine stateMachine;

         public PlayerState currentState;

        // public NormalState NormalState = new NormalState();
        // public SpinnerState SpinnerState = new SpinnerState();
        // public StunState StunState = new StunState();
        public NormalState NormalState;
        public SpinnerState SpinnerState;
        public StunState StunState;
        public DeathState DeathState;
    #endregion

    public Rigidbody rb;

    public CameraTarget cameraTarget;

    public SpawnPlayer spawnPlayer;
    public int playerId;

    public GameObject[] normalSkins;
    public GameObject[] spinSkins;
    public event System.Action<int> OnScoreChanged;
    public int playerPoint;

    [Header("Spin charging properties")]
    public bool startCharging;
    public float spinTimer = 0f;
    public float[] bonusSpeedPerPhase;
    public float[] timerPerPhase;

    public PlayerController lastPlayerContacted;
    public float maxTimeLastPlayer = 5f;
    public float timeLastPlayer;

    public bool isMoving;

    public float stunDurationKnockback = 1f;
    public float stunDurationSpinEnd = 0.3f;

    void OnEnable()
    {
        currentState = NormalState;

        currentState.EnterState(this);

        cameraTarget = GameObject.FindWithTag("MainCamera").GetComponent<CameraTarget>();
        cameraTarget.targets.Add(transform);
        
        //transform.position = spawnPlayer.spawnPoints[playerId].position;
    }

    void OnDisable()
    {
        this.GetComponent<Stretch>().enabled = false;
    }

    void Start()
    {
        
        GameManager.instance.allPlayer.Add(this);

        // normalSkins[playerId].SetActive(true);
        // spinSkins[playerId].SetActive(true);
        
        if (OnScoreChanged != null)
            OnScoreChanged(playerPoint);
    }

    void FixedUpdate()
    {
        if(startCharging)spinTimer += Time.deltaTime;
        UpdateLastPlayer(); //last bumped player

        isMoving = move != Vector3.zero;
        currentState.UpdateState(this);

        BounceWallTimer();
    }

    private void UpdateLastPlayer()
    {
        if(lastPlayerContacted != null)
        {
            if(timeLastPlayer > 0)timeLastPlayer -= Time.deltaTime;
            else lastPlayerContacted = null;
        }
    }

    #region Inputs

        [HideInInspector] public Vector3 move;

        public void OnMove(InputAction.CallbackContext ctx)
        {
            move = ctx.ReadValue<Vector3>();
        }

        public void OnSpin(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if(currentState == NormalState)
                {
                    //NormalState.SetSpeedModifier(NormalState.mSettings.slowSpeed);
                    NormalState.SlowSpeedModifier();
                    this.GetComponent<Stretch>().enabled = true;
                    startCharging = true;
                }
            }

            if(ctx.canceled)
            {
                if(currentState == NormalState)
                {
                    if(startCharging)
                    {
                        for(int i = 0; i < bonusSpeedPerPhase.Length; i++)
                        {
                            if(spinTimer < timerPerPhase[i] || (spinTimer >= timerPerPhase[timerPerPhase.Length - 1] && i == (timerPerPhase.Length - 1)))
                            {
                                //transform to spin
                                stateMachine.SwitchState(SpinnerState);
                                SpinnerState.mSettings.bonusMoveSpeed = bonusSpeedPerPhase[i];
                                //Debug.Log("VROUM");
                                break;
                            }
                        }

                        ResetCharging();
                    }
                }
            }
        }

        public void ResetCharging()
        {
            //reset properties
            NormalState.NormalSpeedModifier();
            this.GetComponent<Stretch>().noStretch = true;
            //normalPlayer.spinCharging = false;   ANNULER QUAND ON SE FAIT STUN
            startCharging = false;
            spinTimer = 0f;
        }

    #endregion

    #region Collisions

        public Vector3 normalizedWall;
        public Vector3 playerDirection;

        public bool walled;

        private float timer;
        public float timerCount;

        private void OnCollisionEnter(Collision other)
        {
            
            if(other.gameObject.tag == "Wall")
            {
                if(currentState == SpinnerState)
                {

                    normalizedWall = other.contacts[0].normal;
                    playerDirection = SpinnerState.moveDir;
                    timer = timerCount;
                    //mettre timer
                    BounceWall();
                }

                if (currentState == StunState)
                {
                    normalizedWall = other.contacts[0].normal;
                    playerDirection = StunState.knockbackDir;
                    BounceWallStuned();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger) return;
            if (other.gameObject.tag == "Player")
            {
                
                if(currentState == SpinnerState)
                {
                    PlayerController triggerPlayer = other.GetComponentInParent<PlayerController>();

                    triggerPlayer.lastPlayerContacted = this;
                    triggerPlayer.timeLastPlayer = triggerPlayer.maxTimeLastPlayer;

                    //Si le joueur est pas stun [???]

                    if (triggerPlayer.currentState == triggerPlayer.NormalState)
                    {
                        //activate bounce player of this spinner
                        triggerPlayer.StunState.timerMax = triggerPlayer.stunDurationKnockback;
                        triggerPlayer.stateMachine.SwitchState(triggerPlayer.StunState);

                        //activate knockback for triggered player >:(
                        //other.GetComponentInParent<Knockback>().spinnerKnockbacking = this.spinnerControler;
                        
                    }
                    //////////if (other.gameObject.layer == 8) bounceSpinner.enabled = true;
                    
                    triggerPlayer.StunState.timerMax = stunDurationSpinEnd;
                    stateMachine.SwitchState(StunState);
                }
            }
        }

        void BounceWall()
        {
            SpinnerState.moveDir = Vector3.Reflect(playerDirection, normalizedWall);
            float newAngle = Vector3.SignedAngle(playerDirection, SpinnerState.moveDir, Vector3.up);

            Vector3 newVector = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + newAngle, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(newVector);

            walled = true;
        }

        void BounceWallStuned()
        {
            StunState.knockbackDir = Vector3.Reflect(StunState.knockbackDir, normalizedWall);
        }

        private void BounceWallTimer()
        {
            if (!walled) return;
            
            if(timer > 0)timer -=Time.deltaTime;
            else walled = false;
        }

    #endregion

    public void UpdateScore(int value)
    {
        if (OnScoreChanged != null)
            OnScoreChanged(value);
    }

    public void RespawnPlayer()
    {
        transform.position = spawnPlayer.spawnPoints[playerId].position;
    }
}
