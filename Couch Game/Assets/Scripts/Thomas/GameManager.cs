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
    
    public List<TextMeshProUGUI> playerRoomUI;
    public GameObject playersRoom;
    public List<PlayerController> allPlayer;
    public List<PlayerInput> playersList = new List<PlayerInput>();

    public static GameManager instance;
    
    //Input Join and Leave
    public InputAction joinAction;
    public InputAction leftAction;
    public InputAction StartAction;

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
        public TMP_Text comparetimerTXT;

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
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        if(gameStarted)return;
        playersList.Add(playerInput);
        if (PlayerJoinedGame != null)
            PlayerJoinedGame(playerInput);
        PlayerController playerController = playerInput.GetComponent<PlayerController>();
        playerController.playerId = playersList.Count;
        playerController.isReady = true;
        playerRoomUI[playerController.playerId - 1].text = "Ready !"; 
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
        print("aled");
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
    }
    
    private void OnDisable()
    {
        joinAction.Disable();
        leftAction.Disable();
        StartAction.Disable();
    }

    private void JoinAction(InputAction.CallbackContext ctx)
    {
        
        PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(ctx);
    }
    
    private void LeftAction(InputAction.CallbackContext ctx)
    {
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
        playersList.Remove(playerInput);
        if (PlayerLeftGame != null)
            PlayerLeftGame(playerInput);
        
        Destroy(playerInput.transform.gameObject);
    }


    // Start is called before the first frame update
    private void Start()
    {
        
        gameTimer.timer = gameTimer.maxTimer;
        gameStarted = false;
        gameTimer.drawTimer = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        RoundTimer();

        if (StartAction.IsInProgress())
        {
            startSlider.value = Mathf.MoveTowards(startSlider.value, startSlider.maxValue, 1f * Time.deltaTime);
            print(startSlider.value);
        }
        else
        {
            startSlider.value = Mathf.MoveTowards(startSlider.value, startSlider.minValue, 1f * Time.deltaTime);
        }

        if(gameTimer.drawTimer || !gameStarted)return;
        if(Input.GetKey(KeyCode.Alpha1))ChangingMap(1);
        if(Input.GetKey(KeyCode.Alpha2))ChangingMap(2);
    }
    

    private void RoundTimer()
    {
        if(!gameStarted)return;
        if(!gameTimer.drawTimer)
        {
            if(gameTimer.timer > 0)
            {
                gameTimer.timer -= Time.deltaTime;
                switch(gameTimer.timer)
                {
                    case > 10 :
                        gameTimer.timerTXT.text = Mathf.Round(gameTimer.timer).ToString();
                        gameTimer.comparetimerTXT.text = Mathf.Round(gameTimer.timer).ToString();
                    break;

                    case < 10 :
                        gameTimer.timerTXT.text = gameTimer.timer.ToString("0.00");
                        gameTimer.comparetimerTXT.text = gameTimer.timer.ToString("0.0");
                    
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
                int playerPoint = 0;
                PlayerController playerMaxPoint = null;
                //get max point + player
                for(int i = 0; i < allPlayer.Count; i++)
                {
                    if(allPlayer[i].playerPoint >= playerPoint)
                    {
                        playerPoint = allPlayer[i].playerPoint;
                        playerMaxPoint = allPlayer[i];
                    }
                }
                
                int equalTime = 0;
                //if 2 player has same point,
                for(int i = 0; i < allPlayer.Count; i++)
                {
                    if(playerPoint == allPlayer[i].playerPoint)equalTime += 1;
                }

                //it's draw time !
                if(equalTime > 1)
                {
                    gameTimer.drawTimer = true;
                    gameTimer.drawMaxPoint = playerPoint;
                    gameTimer.timerTXT.text = "SUDDEN DEATH !";
                    AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Voice Sudden Death"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
                    Destroy(ost);
                    AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Overtime"], this.transform.position, AudioManager.instance.ostMixer, false, true);
                }
                else
                {
                    PlayerWin(playerMaxPoint);
                }
            }
        }
        
    }

    public void PlayerWin(PlayerController playerWinner)
    {
        if(gameTimer.timeOut)return;
        gameTimer.timerTXT.text = $"Player {allPlayer.IndexOf(playerWinner) + 1} WIN !";
        Debug.Log($"{playerWinner.name} WIN WITH {playerWinner.playerPoint} POINTS !");

        //general victory voice sound
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"{allPlayer.IndexOf(playerWinner) + 1} Win"], this.transform.position, AudioManager.instance.soundEffectMixer, true, false);
        gameTimer.timeOut = true;
        gameStarted = false;
        foreach(PlayerController player in allPlayer)
        {
            player.move = Vector3.zero;
        }

        
        foreach(PlayerController player in allPlayer)
        {
            Destroy(player.sfx);
        }
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
        print(count(allPlayer, true));
        if( count(allPlayer, true) == 0 || count(allPlayer, true) < 1 || count(allPlayer, true) > 4) return;
        
        tempPlayerNb.howManyPlayer = count(allPlayer, true);
        
        playersRoom.SetActive(false);
        gameStarted = true;
        gameTimer.drawTimer = false;

        AudioSource readyGo = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Ready Go"], this.transform.position, AudioManager.instance.announcerMixer, true, false);
        StartCoroutine(WaitBeforeGameStart(readyGo.clip.length - 1.3f));
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
        ost = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("OST"), transform.position, AudioManager.instance.ostMixer, false, false);
        gameStarted = true;
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

    public void Cheer()
    {
        for(int i = 0; i < allCrowd.Length; i++)
        {
            StartCoroutine(allCrowd[i].CrowdCheer());
        }
    }
}
