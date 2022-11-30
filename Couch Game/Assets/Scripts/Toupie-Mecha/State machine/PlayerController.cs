using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

    public GameObject explosion;
    public int dashDurationReduction = 2;

    private Animator playerAnimator;
    public Animator PlayerAnimator
    {
        get => playerAnimator ; private set{}
    }

    public GameObject playerFBX;
    public TrailRenderer trailRenderer;
    public SpinningAnim spinningAnim;

    public GameObject troupieVFX;
    private CapsuleCollider capsuleCol;
    private SphereCollider sphereCol;

    [Header("Respawn invincibility")]

    public float invincibilityTimerMax = 3f;
    float invincibilityTimer;
    bool invincible;

    void OnEnable()
    {
        currentState = NormalState;

        currentState.EnterState(this);

        cameraTarget = GameObject.FindWithTag("MainCamera").GetComponent<CameraTarget>();
        cameraTarget.targets.Add(transform);
        playerAnimator = this.GetComponentInChildren<Animator>();
        //transform.position = spawnPlayer.spawnPoints[playerId].position;
    }

    void Start()
    {
        
        GameManager.instance.allPlayer.Add(this);

        // normalSkins[playerId].SetActive(true);
        // spinSkins[playerId].SetActive(true);

        capsuleCol = GetComponent<CapsuleCollider>();
        sphereCol = GetComponent<SphereCollider>();
        
        if (OnScoreChanged != null)
            OnScoreChanged(playerPoint);
        
        invincibilityTimer = 0;
    }

    void FixedUpdate()
    {
        if(startCharging)spinTimer += Time.deltaTime;
        UpdateLastPlayer(); //last bumped player

        isMoving = move != Vector3.zero;
        currentState.UpdateState(this);

        BounceWallTimer();

        if (invincibilityTimer > 0) invincibilityTimer = Mathf.Clamp(invincibilityTimer - Time.deltaTime, 0, invincibilityTimerMax);
        else if (invincible) StopInvincibility();
    }

    private void UpdateLastPlayer()
    {
        //make last player null when this player didn't get touched in time
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
            if(ctx.performed)
            {
                if(GameManager.instance.allPlayer.Count != GameManager.instance.tempPlayerNb.howManyPlayer || !GameManager.instance.gameStarted)return;
                if(walled)return;
                move = ctx.ReadValue<Vector3>();
                playerAnimator.SetBool("IsWalking", true);
            }
            else if(ctx.canceled)playerAnimator.SetBool("IsWalking", false);
        }

        public void OnSpin(InputAction.CallbackContext ctx)
        {
            if(GameManager.instance.allPlayer.Count != GameManager.instance.tempPlayerNb.howManyPlayer || !GameManager.instance.gameStarted)return;
            if (ctx.performed)
            {
                if(currentState == NormalState)
                {
                    //NormalState.SetSpeedModifier(NormalState.mSettings.slowSpeed);
                    NormalState.SlowSpeedModifier();
                    startCharging = true;
                    playerAnimator.SetBool("ChargingSpin", true);
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
                                playerAnimator.SetBool("ChargingSpin", false);
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
            //normalPlayer.spinCharging = false;   ANNULER QUAND ON SE FAIT STUN
            startCharging = false;
            spinTimer = 0f;
        }

        public void OnBrake(InputAction.CallbackContext ctx)
        {
            if(GameManager.instance.allPlayer.Count != GameManager.instance.tempPlayerNb.howManyPlayer)return;
            if (currentState != SpinnerState) return;

            if (ctx.performed)
            {
                Debug.Log("start brake");
                SpinnerState.mSettings.brakeManiability = SpinnerState.mSettings.brakeManiabilityModifier;
                SpinnerState.mSettings.brakeSpeed = SpinnerState.mSettings.brakeSpeedModifier;
            }

            if (ctx.canceled)
            {
                Debug.Log("stop brake");
                SpinnerState.mSettings.brakeManiability = 1f;
            }
        }

    #endregion

    #region Collisions

        public Vector3 wallNormal;
        public Vector3 playerDirection;

        public bool walled;

        [SerializeField]private float timer;
        public float timerCount;

        private void OnCollisionEnter(Collision other)
        {
            
            if(other.gameObject.tag == "Wall")
            {
                if(currentState == SpinnerState)
                {

                    wallNormal = other.contacts[0].normal;
                    playerDirection = SpinnerState.moveDir;
                    wallNormal.y = 0;
                    playerDirection.y = 0;
                    timer = timerCount;
                    if(Vector3.Dot(wallNormal, playerDirection) < 0) //produit scalaire
                    {
                        BounceWall();
                    }
                    //mettre timer
                }

                if (currentState == StunState)
                {
                    wallNormal = other.contacts[0].normal;
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
                Debug.Log("haha");
                if(currentState == SpinnerState)
                {
                    PlayerController triggerPlayer = other.GetComponentInParent<PlayerController>();

                    triggerPlayer.lastPlayerContacted = this;
                    triggerPlayer.timeLastPlayer = triggerPlayer.maxTimeLastPlayer;

                    //Si le joueur est pas stun [???]

                    if (triggerPlayer.currentState == triggerPlayer.NormalState)
                    {
                        //activate bounce player of this spinner
                        Instantiate(explosion, this.transform.position, Quaternion.identity);
                        triggerPlayer.StunState.timerMax = triggerPlayer.stunDurationKnockback;
                        triggerPlayer.lastPlayerContacted = this;
                        int randomSpinHitPlayer = Random.Range(0, 8);
                        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault($"Spin Hit Player {randomSpinHitPlayer}"), transform.position, AudioManager.instance.soundEffectMixer, true);
                        triggerPlayer.stateMachine.SwitchState(triggerPlayer.StunState);

                        StunState.timerMax = stunDurationSpinEnd;
                        stateMachine.SwitchState(StunState);
                        //activate knockback for triggered player >:(
                        //other.GetComponentInParent<Knockback>().spinnerKnockbacking = this.spinnerControler;
                        
                    }
                    if (triggerPlayer.currentState == triggerPlayer.SpinnerState && !SpinnerState.repoussed) BounceSpinner();
                    
                }
            }
        }

        void BounceWall()
        {
            SpinnerState.moveDir = Vector3.Reflect(playerDirection, wallNormal);
            float newAngle = Vector3.SignedAngle(playerDirection, SpinnerState.moveDir, Vector3.up);

            Vector3 newVector = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + newAngle, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(newVector);
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Spin Hit Wall"), transform.position, AudioManager.instance.soundEffectMixer, true);
            walled = true;
        }

        void BounceWallStuned()
        {
            StunState.knockbackDir = Vector3.Reflect(StunState.knockbackDir, wallNormal);
        }

        private void BounceWallTimer()
        {
            if (!walled) return;
            
            if(timer > 0)timer -=Time.deltaTime;
            else walled = false;
        }

        void BounceSpinner()
        {
            //Bounce against other player
            Instantiate(explosion, this.transform.position, Quaternion.identity);
            //Debug.Log("bounce");
            SpinnerState.moveDir = -SpinnerState.moveDir;
            move = -move;
            //transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles);
            //SpinnerState.mSettings.dashDuration /= dashDurationReduction;
            SpinnerState.repoussed = true;
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Spin Hit Spin"), transform.position, AudioManager.instance.soundEffectMixer, true);
        }

    #endregion

    public void UpdateScore(int value)
    {
        if (OnScoreChanged != null)
            OnScoreChanged(value);
    }

    public void RespawnPlayer()
    {
        transform.position = GameManager.instance.spawnPoints[playerId - 1].position;
        //capsuleCol.enabled = false;
        sphereCol.enabled = false;
        //Physics.IgnoreLayerCollision(6, 6);

        for (int i=10; i < 14; i++)
        {
            Physics.IgnoreLayerCollision(gameObject.layer, i);
        }

        MeshCollider[] cols = trailRenderer.GetComponentsInChildren<MeshCollider>();
        foreach(MeshCollider c in cols)
        {
            c.enabled = false;
        }

        invincibilityTimer = invincibilityTimerMax;
        invincible = true;
        playerAnimator.enabled = true;
    }

    void StopInvincibility()
    {
        invincible = false;
        sphereCol.enabled = true;
        for (int i=10; i < 14; i++)
        {
            Physics.IgnoreLayerCollision(gameObject.layer, i, false);
        }
    }

    public void OnRechargeGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
