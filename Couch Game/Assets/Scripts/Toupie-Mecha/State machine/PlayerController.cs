using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using EZCameraShake;
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
        public SpinStunState SpinStunState;
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
    public bool hasCountered;
    public float maxTimeLastPlayer = 5f;
    public float timeLastPlayer;

    public bool isMoving;
    public bool isReady;

    [Header("Stun Duration")]
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
    public GameObject playerCrown;
    public bool hasDaCrown;
    public TrailRenderer trailRenderer;
    public SpinningAnim spinningAnim;

    private CapsuleCollider capsuleCol;
    private SphereCollider sphereCol;

    [Header("multiplier points")]
    [SerializeField]private int pointMultiplier = 1;
    /*[HideInInspector]*/public int multiplier = 1;
    public float maxtimeMultiplier = 30f;
    /*[HideInInspector]*/public float timeMultiplier;


    [Header("Respawn invincibility")]

    public float invincibilityTimerMax = 3f;
    public float invincibilityTimer;
    bool invincible;

    public bool stopBumpKb;

    public GameObject arrow;
    public GameObject chargeParticles;
    private ParticleSystem.MainModule settings;

    public AudioSource sfx;
    public GameObject vfx;


    void OnEnable()
    {
        currentState = NormalState;

        currentState.EnterState(this);

        cameraTarget = GameObject.FindWithTag("MainCamera").GetComponent<CameraTarget>();
        cameraTarget.targets.Add(transform);
        playerAnimator = this.GetComponentInChildren<Animator>();
        timeMultiplier = maxtimeMultiplier;
        //transform.position = spawnPlayer.spawnPoints[playerId].position;
    }

    void Start()
    {
        GameManager.instance.allPlayer.Add(this);

        settings = chargeParticles.GetComponent<ParticleSystem>().main;
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

        var realTimer = ((int)spinTimer);

        switch (realTimer)
        {
            case 0:
                arrow.GetComponent<SpriteRenderer>().color = new Color(0, 1, 1, 1);
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 1, 1, 1));
                break;
            case 1:
                arrow.GetComponent<SpriteRenderer>().color = new Color(1, 0.8f, 0, 1);
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0.8f, 0, 1));
                break;
            case 2:
                arrow.GetComponent<SpriteRenderer>().color = new Color(1, 0.3f, 0, 1);
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0.3f, 0, 1));
                break;
            case 4:
                arrow.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0.05f, 1);
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0, 0.05f, 1));
                break;
        }

        //point multiplier
        PointMultiplier();
    }

    private void PointMultiplier()
    {
        if(!GameManager.instance.gameStarted || GameManager.instance.gameTimer.drawTimer)return;
        
        if(timeMultiplier > 0)
        {
            multiplier = 1;
            timeMultiplier -= Time.deltaTime;
        }
        else multiplier = pointMultiplier;
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
                if(bounceWalled)return;
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
                    arrow.GetComponent<SpriteRenderer>().enabled = true;
                    chargeParticles.SetActive(true);
                    playerAnimator.SetBool("ChargingSpin", true);
                    sfx = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Spin Charge"], transform.position, AudioManager.instance.soundEffectMixer, false, true);
                    GameObject spinnerVFX = Instantiate(SpinnerState.spinnerFX.spinningVFX, this.transform);
                    SpinnerState.allSpinnerVFX.Add(spinnerVFX);
                    SpinnerState.allSpinnerSFX.Add(sfx);
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
                                //Debug.Log(bonusSpeedPerPhase[i]);
                                if(sfx != null)Destroy(sfx.gameObject);
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
            arrow.GetComponent<SpriteRenderer>().enabled = false;
            chargeParticles.SetActive(false);
    }

        public void OnBrake(InputAction.CallbackContext ctx)
        {
            if(GameManager.instance.allPlayer.Count != GameManager.instance.tempPlayerNb.howManyPlayer)return;
            if (currentState != SpinnerState) return;

            if (ctx.performed)
            {
                //Debug.Log("start brake");
                SpinnerState.mSettings.brakeManiability = SpinnerState.mSettings.brakeManiabilityModifier;
                SpinnerState.mSettings.brakeSpeed = SpinnerState.mSettings.brakeSpeedModifier;

                int randomBrake = Random.Range(0, 6);
                AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"Spin Brake {randomBrake + 1}"], transform.position, AudioManager.instance.soundEffectMixer, true, false);
                GameObject brakeVFX = Instantiate(SpinnerState.spinnerFX.brakeVFX, transform);
                SpinnerState.allSpinnerVFX.Add(brakeVFX);
            }

            if (ctx.canceled)
            {
                //Debug.Log("stop brake");
                SpinnerState.mSettings.brakeManiability = 1f;
            }
        }

    #endregion

    #region Collisions

        public Vector3 wallNormal;
        public Vector3 playerDirection;

        public bool bounceWalled;

        [SerializeField]private float timer;
        public float timerCount;



        //AYO LE BUMP
        [Header("LE BUMP ON REPREND")]
        [SerializeField]private bool canBump = false;
        [SerializeField]private bool hasBumped = false;
        [SerializeField]private bool stillInWall = false;
        [SerializeField]private bool hasBumpedPlayer = false;
        public bool firstBumpPlayer = false;
        public bool bumpPlayer;

        [SerializeField]private float maxWallStuckTimer = .5f; 
        [SerializeField]private float wallStuckTimer; 


        private void OnCollisionEnter(Collision other)
        {
            
            // if(other.gameObject.tag == "Wall")
            // {
            //     walled = true;
            // }

            //quand entre mur, si mur alors canbump
            if(other.gameObject.tag != "Wall")return;
            canBump = true;
            
        }

        private void OnCollisionExit(Collision other)
        {
            // if(other.gameObject.tag != "Wall")return;
            // if(walled)walled = false;
            // StartCoroutine(WaitBounceWall());

            //quand quitte mur, ne peut plus bump et ne bump plus
            if(other.gameObject.tag != "Wall")return;
            canBump = false;

            //mettre timer avant reset ?
            hasBumped = false;
        }
        private void OnCollisionStay(Collision other)
        {
            if(other.gameObject.tag != "Wall")return;
            if(canBump && !hasBumped)WallBounce(other);

            //si can bump false quand trop dans la collision, remet la possibilité de bump
            if(!stillInWall)return;
            if(wallStuckTimer > 0)wallStuckTimer -= Time.deltaTime;
            else
            {
                WallBounce(other);
                stillInWall = false;
            }
            
            // if((walled || NormalState.isKnockbacked) && !bounceWalled)
            // {
            //     WallBounce(other);
            // }
            // else if(!walled && bounceWalled)
            // {
            //     WallBounce(other);
            //     return;
            // }

            //quand reste dans collision, si peut bump et n'a pas déjà bump alors bump
        }

        private IEnumerator WaitBeforeReset(bool var, float time)
        {
            yield return new WaitForSeconds(time);
            var = !var;
        }


        private void WallBounce(Collision other)
        {
            //if(bounceWalled)return;
            if(currentState == SpinnerState)
            {
                CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
                wallNormal = other.contacts[0].normal;
                playerDirection = SpinnerState.moveDir;
                wallNormal.y = 0;
                playerDirection.y = 0;
                timer = timerCount;
                //Debug.Log(Vector3.Distance(this.transform.position, other.transform.position));

                if(Vector3.Dot(wallNormal, playerDirection) < 0) //produit scalaire
                {
                    WallEvent wallEvent = other.gameObject.GetComponentInParent<WallEvent>();
                    if(wallEvent == null)BounceWall(null);
                    else BounceWall(wallEvent);
                }
                else
                {
                    //test
                    SpinnerState.moveDir = wallNormal;
                    float newAngle = Vector3.SignedAngle(playerDirection, SpinnerState.moveDir, Vector3.up);

                    Vector3 newVector = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + newAngle, transform.rotation.eulerAngles.z);
                    transform.rotation = Quaternion.Euler(newVector);
                }
                //bounceWalled = true;
                //walled = false;
                NormalState.isKnockbacked = false;
                
                canBump = false;
                hasBumped = true;
                wallStuckTimer = maxWallStuckTimer;
                stillInWall = true;

                //Debug.Log(bounceWalled);
                //Debug.Log(walled);
                //Debug.Log(NormalState.isKnockbacked);
                //mettre timer
                
            }
            else if (currentState == StunState)
            {
                CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
                wallNormal = other.contacts[0].normal;
                playerDirection = StunState.knockbackDir;
                BounceWallStuned();

                canBump = false;
                hasBumped = true;

                //bounceWalled = true;
                //walled = false;
                NormalState.isKnockbacked = false;
                //Debug.Log(bounceWalled);
                //Debug.Log(walled);
                //Debug.Log(NormalState.isKnockbacked);
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger || other.tag == "hitbox") return;
            bumpPlayer = true;
        }

        void OnTriggerStay(Collider other)
        {
            if(!other.isTrigger || other.tag == "hitbox")return;
            if(bumpPlayer && !hasBumpedPlayer)BumpPlayer(other);
        }

        void OnTriggerExit(Collider other)
        {
            if(!other.isTrigger || other.tag == "hitbox")return;
            bumpPlayer = false;
            hasBumpedPlayer = false;
        }

        

        void BumpPlayer(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerController triggerPlayer = other.GetComponentInParent<PlayerController>();

                triggerPlayer.lastPlayerContacted = this;
                triggerPlayer.timeLastPlayer = triggerPlayer.maxTimeLastPlayer;
                if (currentState == SpinnerState)
                {
                    CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
                    

                    //Si le joueur est pas stun [???]
                    if(triggerPlayer.SpinnerState.repoussed)return;
                    if ((triggerPlayer.currentState == triggerPlayer.NormalState || triggerPlayer.currentState == triggerPlayer.StunState || triggerPlayer.currentState == triggerPlayer.SpinStunState || triggerPlayer.currentState == triggerPlayer.SpinnerState) && !triggerPlayer.hasCountered)
                    {
                        Debug.Log("2 fois ?");
                        stateMachine.SwitchState(NormalState);
                        triggerPlayer.stateMachine.SwitchState(triggerPlayer.NormalState);
                        //Debug.Log(triggerPlayer);
                        //triggerPlayer.hasCountered = false;
                        //activate bounce player of this spinner
                        Instantiate(explosion, this.transform.position, Quaternion.identity);
                        triggerPlayer.StunState.timerMax = triggerPlayer.stunDurationKnockback;
                        triggerPlayer.lastPlayerContacted = this;
                        triggerPlayer.NormalState.isKnockbacked = true;
                        int randomSpinHitPlayer = Random.Range(0, 7);
                        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault($"Spin Hit Player {randomSpinHitPlayer + 1}"), transform.position, AudioManager.instance.soundEffectMixer, true, false);
                        //triggerPlayer.SpinnerState.mSettings.moveSpeed = StunState.kbSpeed;
                        //Debug.Log("MV : " + triggerPlayer.SpinnerState.mSettings.moveSpeed);
                        //Debug.Log("MV : " + StunState.kbSpeed);
                        rb.velocity = triggerPlayer.rb.velocity;
                        triggerPlayer.stateMachine.SwitchState(triggerPlayer.StunState);
                        StunState.timerMax = 0.1f;
                        stateMachine.SwitchState(StunState);
                        //triggerPlayer.SpinnerState.repoussed = true;
                        //activate knockback for triggered player >:(
                        //other.GetComponentInParent<Knockback>().spinnerKnockbacking = this.spinnerControler;
                        
                    }
                    if (triggerPlayer.currentState == triggerPlayer.SpinnerState && !SpinnerState.repoussed) BounceSpinner();
                    bumpPlayer = false;
                    hasBumpedPlayer = true;
                    firstBumpPlayer = true;
                }
                else if(currentState == StunState)
                {
                    if(!bumpPlayer)return;
                    //Debug.Log("JE SUIS STUNNED NORMALEMENT");
                    if(/*(!SpinnerState.repoussed ||*/ triggerPlayer.currentState == triggerPlayer.SpinnerState/*)*/)return;
                 if(triggerPlayer.firstBumpPlayer)
                 {
                     //WaitBeforeReset(triggerPlayer.firstBumpPlayer, .1f);
                     triggerPlayer.firstBumpPlayer = false;
                 }
                 else
                 {
                    triggerPlayer.stateMachine.SwitchState(triggerPlayer.NormalState);
                     Debug.Log("HOO");
                        CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);

                        triggerPlayer.lastPlayerContacted = this;
                        triggerPlayer.timeLastPlayer = triggerPlayer.maxTimeLastPlayer;
                        Instantiate(explosion, this.transform.position, Quaternion.identity);
                        triggerPlayer.StunState.timerMax = triggerPlayer.stunDurationSpinEnd;
                        triggerPlayer.lastPlayerContacted = this;
                        
                        int randomSpinHitPlayer = Random.Range(0, 7);
                        if (triggerPlayer.currentState != triggerPlayer.StunState)
                            triggerPlayer.rb.velocity = rb.velocity;
                        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault($"Spin Hit Player {randomSpinHitPlayer + 1}"), transform.position, AudioManager.instance.soundEffectMixer, true, false);
                        triggerPlayer.stateMachine.SwitchState(triggerPlayer.StunState);
                        //triggerPlayer.SpinnerState.repoussed = true;
                        bumpPlayer = false;
                        hasBumpedPlayer = true;
                    }
                }
            }
        }

        void BounceWall(WallEvent wallEvent)
        {
            //Debug.Log(playerDirection);
            SpinnerState.moveDir = Vector3.Reflect(playerDirection, wallNormal);
            float newAngle = Vector3.SignedAngle(playerDirection, SpinnerState.moveDir, Vector3.up);

            Vector3 newVector = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + newAngle, transform.rotation.eulerAngles.z);
            transform.rotation = Quaternion.Euler(newVector);
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Spin Hit Wall"), transform.position, AudioManager.instance.soundEffectMixer, true, false);
            
            //Anim billard and button
            if(wallEvent != null)NeonBugBounce(wallEvent);
            
            bounceWalled = true;
        }

        void BounceWallStuned()
        {
            StunState.knockbackDir = Vector3.Reflect(StunState.knockbackDir, wallNormal);
            //bounceWalled = true;
        }

        private void BounceWallTimer()
        {
            if (!bounceWalled) return;
            
            if(timer > 0)timer -=Time.deltaTime;
            else bounceWalled = false;
        }

        void BounceSpinner()
        {
            //Bounce against other player
            //Instantiate(explosion, this.transform.position, Quaternion.identity);
            Instantiate(SpinnerState.spinnerFX.spinerVsSpinerVFX, transform);
            CameraShaker.Instance.ShakeOnce(1f, 4f, 0.1f, 0.5f);
            //Debug.Log("bounce");
            SpinnerState.moveDir = -SpinnerState.moveDir;
            move = -move;
            //transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles);
            //SpinnerState.mSettings.dashDuration /= dashDurationReduction;
            SpinnerState.repoussed = true;
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("Spin Hit Spin"), transform.position, AudioManager.instance.soundEffectMixer, true, false);
        }

    #endregion

    public void UpdateScore(int value)
    {
        if (OnScoreChanged != null)
        {
            StartCoroutine(Scoring(value, GameManager.instance.refreshTime));
        }
    }

    public IEnumerator Scoring(int value, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        int score = playerPoint - value;
        while(score != playerPoint)
        {
            //Debug.Log(score);
            if(value >= 0)score++;
            else score--;
            OnScoreChanged(score);
            yield return null;
        }

    }

    
    public void RespawnPlayer()
    {
        playerAnimator.SetTrigger("Respawn");
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


    public void NeonBugBounce(WallEvent wallEvent)
    {
        if(wallEvent.wallAnim == null || wallEvent.billardAnim == null)return;
        wallEvent.wallAnim.ResetTrigger("BounceWall");
        wallEvent.billardAnim.ResetTrigger("BounceWall");
        
        wallEvent.wallAnim.SetTrigger("BounceWall");
        wallEvent.billardAnim.SetTrigger("BounceWall");
    }

    //A enlever après prod
    public void OnRechargeGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
