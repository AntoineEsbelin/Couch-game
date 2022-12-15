using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    
    public Slider startSlider;
    
    [Header("SelectPlayer")]
    public List<GameObject> playerRoomUI;
    public List<Image> playerIcon;
    public List<Sprite> playerIconOff;
    public List<Sprite> playerIconOn;

    [Space(5)]
    public PlayerUIPanel[] playerUIPanels;
    public GameObject playersRoom;
    public List<PlayerController> allPlayer;
    [SerializeField] private List<PlayerController> playerRanking;
    public List<PlayerController> PlayerRanking
    {
        get {return playerRanking;}
        private set {playerRanking = PlayerRanking;}
    }
    public List<PlayerInput> playersList = new List<PlayerInput>();

    public static GameManager instance;

    public Image countdown;
    public Camera cam;
    public Camera lastCam;
    public GameObject transition;

    public GameObject fullUI;

    //Input Join and Leave
    public InputAction joinAction;
    public InputAction leftAction;
    public InputAction StartAction;
    

    [Header("ENLEVER APRES PROD")]
    public InputAction MainMenuAction;

    //Event
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    public Transform[] spawnPoints;
    public GameObject[] charactersFBX;
    public AudioSource ost;

    [System.Serializable]
    public class GameTimer
    {
        public float timer;
        public float maxTimer = 60f;

        public bool drawTimer = false;
        public int drawMaxPoint = 0;
        public bool timeOut = false;
        
        public TMP_Text timerTXT;

        public bool nearTimeOut = false;

        public bool timer3 = false;
        public bool timer2 = false;
        public bool timer1 = false;

    }
    public GameTimer gameTimer;

    [System.Serializable]
    public class TempPlayerNb
    {
        public int howManyPlayer = 0;
        public bool oneMinute = false;
    }

    public TempPlayerNb tempPlayerNb;
    public bool gameStarted;

    [Header("Crowd")]
    public Crowd[] allCrowd;
    public float cheerMinTime = 0;
    public float cheerMaxTime = .5f;

    [Header("Score Change refresh time")]
    public float refreshTime = .1f;

    private bool cantJoin = false;
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if(gameStarted && cantJoin)return;
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Appear"], this.transform.position, AudioManager.instance.announcerMixer, true, false);
        playersList.Add(playerInput);
        if (PlayerJoinedGame != null)
            PlayerJoinedGame(playerInput);
        PlayerController playerController = playerInput.GetComponent<PlayerController>();
        playerController.playerId = 1;
        if(playersList.Count > 1)
        {
            for(int i = 0; i < playersList.Count - 1; i++)
            {
                if(playerController.playerId == allPlayer[i].playerId /*&& allPlayer[i] != playerController*/)playerController.playerId += 1;
            }
        }
        //Debug.Log(playerController.playerId);
        playerController.isReady = true;
        playerIcon[playerController.playerId - 1].sprite = playerIconOn[playerController.playerId - 1];
        playerIcon[playerController.playerId - 1].transform.GetChild(0).gameObject.SetActive(false);
        playerController.transform.position = spawnPoints[playerController.playerId - 1].position;
        playerController.playerFBX = Instantiate(charactersFBX[playerController.playerId - 1], playerController.transform);    
    
        playerController.trailRenderer = playerController.playerFBX.GetComponentInChildren<TrailRenderer>(true);
        playerController.spinningAnim = playerController.playerFBX.GetComponentInChildren<SpinningAnim>();
        //playerController.toupieFBX.SetActive(false);
        
        switch(playerController.playerId)
        {
            case 1 : playerController.gameObject.layer = 10;
                break;
            case 2 : playerController.gameObject.layer = 11;
                break;
            case 3 : playerController.gameObject.layer = 12;
                break;
            case 4 : playerController.gameObject.layer = 13;
                break;
            default : playerController.gameObject.layer = 10;
                break;
        }

    }
    
    private void OnPlayerLeft(PlayerInput playerInput)
    {
        //print("aled");
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["UI Back"], this.transform.position, AudioManager.instance.announcerMixer, true, false);

    }
    private void Awake()
    {
        if(instance != null)Destroy(gameObject);
        instance = this;
        
        joinAction.Enable();
        joinAction.performed += ctx => JoinAction(ctx);
        
        leftAction.Enable();
        leftAction.performed += ctx => LeftAction(ctx);

        StartAction.Enable();
        StartAction.performed += ctx => StartGame();
        
        

        //debug
        MainMenuAction.Enable();
        MainMenuAction.performed += ctx => OnReturnMainMenu();
    }
    
    private void OnDisable()
    {
        joinAction.Disable();
        leftAction.Disable();
        StartAction.Disable();
        MainMenuAction.Disable();
        
    }

    private void JoinAction(InputAction.CallbackContext ctx)
    {
        if(gameStarted && cantJoin)return;
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
    }
    
    private void LeftAction(InputAction.CallbackContext ctx)
    {
        if(gameStarted || cantJoin)return;
        if (playersList.Count > 1)
        {
            foreach (var player in playersList)
            {
                foreach (var device in player.devices)
                {
                    if (device != null && ctx.control.device == device)
                    {
                        UnregisterPlayer(player);
                        return;
                    }
                }
            }
        }
    }

    private void UnregisterPlayer(PlayerInput playerInput)
    {
        PlayerController playerController = playerInput.GetComponent<PlayerController>();
        playerController.playerId = playerInput.playerIndex;
        playerController.isReady = false;
        playerIcon[playerInput.playerIndex].sprite = playerIconOff[playerInput.playerIndex];
        
        playersList.Remove(playerInput);
        allPlayer.Remove(playerController);
        //playerRanking.Remove(playerController);
        
        if (PlayerLeftGame != null)
            PlayerLeftGame(playerInput);
        
        Destroy(playerInput.transform.gameObject);
    }


    // Start is called before the first frame update
    private void Start()
    {
        cantJoin = false;
        gameTimer.timer = gameTimer.maxTimer;
        gameStarted = false;
        gameTimer.drawTimer = true;
        ost = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Choice OST"], this.transform.position, AudioManager.instance.ostMixer, false, true);
    }

    
    private void FixedUpdate()
    {
        RoundTimer();

        if (StartAction.IsInProgress())
        {
            startSlider.value = Mathf.MoveTowards(startSlider.value, startSlider.maxValue, 2f * Time.deltaTime);
            
        }
        else
        {
            startSlider.value = Mathf.MoveTowards(startSlider.value, startSlider.minValue, 10f * Time.deltaTime);
        }

        if(gameTimer.drawTimer || !gameStarted)return;
        //if(Input.GetKey(KeyCode.Alpha1))ChangingMap(1);
        //if(Input.GetKey(KeyCode.Alpha2))ChangingMap(2);
    }
    

    private void RoundTimer()
    {
        if(!gameStarted)return;
        if(!gameTimer.drawTimer)
        {
            if(gameTimer.timer > 0)
            {
                int trueTimer;
                gameTimer.timer -= Time.deltaTime;
                switch(Mathf.Round(gameTimer.timer))
                {
                    case > 129 :
                        gameTimer.timerTXT.fontSize = 72;
                        trueTimer = (int)Mathf.Round(gameTimer.timer) - 120;
                        gameTimer.timerTXT.text = "02:" + Mathf.Round(trueTimer).ToString();
                        break;
                    case > 120:
                        gameTimer.timerTXT.fontSize = 72;
                        trueTimer = (int)Mathf.Round(gameTimer.timer) - 120;
                        gameTimer.timerTXT.text = "02:0" + Mathf.Round(trueTimer).ToString();
                        break;
                    case 120:
                        gameTimer.timerTXT.fontSize = 72;
                        gameTimer.timerTXT.text = "02:00";
                        gameTimer.timerTXT.GetComponent<Animator>().SetBool("Bump", true);
                        break;
                    case > 69:
                        gameTimer.timerTXT.fontSize = 72;
                        gameTimer.timerTXT.GetComponent<Animator>().SetBool("Bump", false);
                        trueTimer = (int)Mathf.Round(gameTimer.timer) - 60;
                        gameTimer.timerTXT.text = "01:" + Mathf.Round(trueTimer).ToString();
                        break;
                    case > 60:
                        gameTimer.timerTXT.fontSize = 72;
                        gameTimer.timerTXT.GetComponent<Animator>().SetBool("Bump", false);
                        trueTimer = (int)Mathf.Round(gameTimer.timer) - 60;
                        gameTimer.timerTXT.text = "01:0" + Mathf.Round(trueTimer).ToString();
                        break;
                    case 60:
                        gameTimer.timerTXT.fontSize = 72;
                        gameTimer.timerTXT.text = "01:00";
                        gameTimer.timerTXT.GetComponent<Animator>().SetBool("Bump", true);
                        break;
                    case > 10:
                        gameTimer.timerTXT.fontSize = 111;
                        gameTimer.timerTXT.GetComponent<Animator>().SetBool("Bump", false);
                        trueTimer = (int)Mathf.Round(gameTimer.timer);
                        gameTimer.timerTXT.text = Mathf.Round(trueTimer).ToString();
                    break;

                    case 10:
                        gameTimer.timerTXT.fontSize = 111;
                        gameTimer.timerTXT.text = "10";
                        gameTimer.timerTXT.GetComponent<Animator>().SetBool("Bump", true); 
                        gameTimer.timerTXT.GetComponent<Animator>().SetTrigger("Color");
                        break;
                    case 0:
                        gameTimer.timerTXT.fontSize = 111;
                        gameTimer.timerTXT.GetComponent<Animator>().enabled = false;
                        gameTimer.timerTXT.text = "0";
                        break;
                    case < 0:
                        gameTimer.timerTXT.fontSize = 72;
                        break;
                    case < 10:
                        gameTimer.timerTXT.fontSize = 111;
                        gameTimer.timerTXT.GetComponent<Animator>().SetTrigger("LastBump");
                        trueTimer = (int)Mathf.Round(gameTimer.timer);
                        gameTimer.timerTXT.text = Mathf.Round(trueTimer).ToString();
                        //gameTimer.timerTXT.text = gameTimer.timer.ToString("0.00");
                    
                        PlayVoiceAtTime(3, ref gameTimer.nearTimeOut, AudioManager.instance.allAudio["Voice 321"], AudioManager.instance.announcerMixer);
                        PlayVoiceAtTime(3, ref gameTimer.timer3, AudioManager.instance.allAudio["Game Timer"], AudioManager.instance.soundEffectMixer);
                        PlayVoiceAtTime(2, ref gameTimer.timer2, AudioManager.instance.allAudio["Game Timer"], AudioManager.instance.soundEffectMixer);
                        PlayVoiceAtTime(1, ref gameTimer.timer1, AudioManager.instance.allAudio["Game Timer"], AudioManager.instance.soundEffectMixer);
                    break;
                }
                if(gameTimer.timer > 60)return;
                OneMinuteRemaining();
            }
            else
            {
                DrawTime();
            }
        }
        
    }

    public void DrawTime()
    {
        //get 1st & 2nd points, if the same then draw time
        if(playerRanking.Count <= 1)PlayerWin(playerRanking[0]);
        if(playerRanking[0].playerPoint == playerRanking[1].playerPoint)
        {
            gameTimer.drawTimer = true;
            gameTimer.timerTXT.text = "SUDDEN DEATH !";
            AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Voice Sudden Death"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
            Destroy(ost.gameObject);
            ost = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Overtime"], this.transform.position, AudioManager.instance.ostMixer, false, true);
        }
        else PlayerWin(playerRanking[0]);
        
    }

    public void PlayerWin(PlayerController playerWinner)
    {
        if(gameTimer.timeOut)return;
        gameTimer.timerTXT.text = "END";
        gameTimer.timerTXT.fontSize = 72;

        if (gameTimer.drawTimer)
        {
            if(ost != null)Destroy(ost.gameObject);
            ost = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["End Game"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        }
        StartCoroutine(WaitBeforeWin(5/*AudioManager.instance.allAudio["End Game"].length / 2*/, playerWinner));
    }

    private void PlayVoiceAtTime(float time, ref bool alreadyPlayed, AudioClip voice, UnityEngine.Audio.AudioMixerGroup soundMixer)
    {
        if(alreadyPlayed)return;
        if(gameTimer.timer > time)return;
        AudioManager.instance.PlayClipAt(voice, this.transform.position, soundMixer, true, false);
        alreadyPlayed = true;
    }

    public void StartGame()
    {
        // if(inputField.text.Length == 0 || int.Parse(inputField.text) < 1 || int.Parse(inputField.text) > 4)return;
        //print(count(allPlayer, true));
        if( count(allPlayer, true) == 0 || count(allPlayer, true) < 1 || count(allPlayer, true) > 4) return;
        Destroy(ost.gameObject);

        cantJoin = true;
        
        tempPlayerNb.howManyPlayer = count(allPlayer, true);
        
        playersRoom.SetActive(false);
        gameStarted = true;
        gameTimer.drawTimer = false;

        AudioSource readyGo = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Ready Go"], this.transform.position, AudioManager.instance.announcerMixer, true, false);
        StartCoroutine(WaitBeforeGameStart(readyGo.clip.length - 1.2f));
        countdown.GetComponent<Animator>().SetTrigger("Start");
        cam.GetComponent<Animator>().SetTrigger("Start");
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Crowd Shouting"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
    }
    
    public int count(List<PlayerController> players, bool flag){
        int value = 0;
 
        for(int i = 0; i < players.Count; i++) {
            if(players[i].isReady == flag) value++;
        }
 
        return value;
    }

    private IEnumerator WaitBeforeGameStart(float length)
    {
        gameStarted = false;
        yield return new WaitForSeconds(length);
        cam.GetComponent<Animator>().enabled = false;
        cam.GetComponent<CameraTarget>().enabled = true;
        ost = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("OST"), transform.position, AudioManager.instance.ostMixer, false, false);
        gameStarted = true;
        playerRanking = allPlayer;
        gameTimer.drawTimer = false;
    }

    private void OneMinuteRemaining()
    {
        if(tempPlayerNb.oneMinute)return;
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["One Minute Remaining"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        tempPlayerNb.oneMinute = true;
    }


    //A ENLEVER APRES PROD

    public void ChangingMap(int input)
    {
        string sceneName;
        switch(input)
        {
            case 1 :
                sceneName = "Proto Toupie-Mecha";
            break;

            case 2 :
                sceneName = "LastMouf";
            break;

            default :
                sceneName = "Proto Toupie-Mecha";
            break;
        }

        SceneManager.LoadScene(sceneName);
    }

    public void OnReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Cheer()
    {
        for(int i = 0; i < allCrowd.Length; i++)
        {
            StartCoroutine(allCrowd[i].CrowdCheer());
        }
    }

    public void UpdateRanking()
    {
        //tri le ranking
        for(int i = 1; i < playerRanking.Count; ++i)
        {
            PlayerController tempPlayer = playerRanking[i];
            int j = i - 1;
            while(j >= 0 && playerRanking[j].playerPoint < tempPlayer.playerPoint)
            {
                playerRanking[j + 1] = playerRanking[j];
                j--;
            }
            playerRanking[j + 1] = tempPlayer;
        }

        //1er du ranking
        PlayerUIPanel playerUI = playerUIPanels[playerRanking[0].playerId - 1];
        playerUI.playerIMG.sprite = playerUI.playerScoreIMG[0];
        for(int i = 1; i < playerRanking.Count; i++)
        {
            int j = i;
            playerUI = playerUIPanels[playerRanking[j].playerId - 1];
            if(playerRanking[i].playerPoint > playerRanking[i - 1].playerPoint) j -= 1;
            playerUI.playerIMG.sprite = playerUI.playerScoreIMG[j];
        }
    }

    private IEnumerator WaitBeforeWin(float waitTime, PlayerController playerWinner)
    {
        foreach(PlayerController player in allPlayer)
        {
            player.move = Vector3.zero;
        }

        foreach(PlayerController player in allPlayer)
        {
            if(player.sfx != null)Destroy(player.sfx.gameObject);
            if(player.vfx != null)Destroy(player.vfx.gameObject);
        }
        gameStarted = false;
        gameTimer.timeOut = true;
        

        yield return new WaitForSeconds(waitTime);
        transition.GetComponent<Animator>().SetTrigger("In");
        yield return new WaitForSeconds(1f);
        if(ost != null)Destroy(ost.gameObject);
        //gameTimer.timerTXT.text = $"Player {playerWinner.playerId} WIN !";
        
        AudioSource audio = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Win"], this.transform.position, AudioManager.instance.soundEffectMixer, false, true);
        //Podium scene here
        transition.GetComponent<Animator>().SetTrigger("Out");
        fullUI.gameObject.SetActive(false);
        cam.gameObject.SetActive(false);
        lastCam.gameObject.SetActive(true);
        
        //general victory voice sound
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"{playerWinner.playerId} Win"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);

        yield return new WaitForSeconds(14f);
        transition.GetComponent<Animator>().SetTrigger("In");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
    }


    
    
}
