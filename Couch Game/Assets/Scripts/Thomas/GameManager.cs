using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public List<PlayerController> allPlayer;
    public List<PlayerInput> playersList = new List<PlayerInput>();

    public static GameManager instance;
    
    //Input Join and Leave
    public InputAction joinAction;
    public InputAction leftAction;

    //Event
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;

    public Transform[] spawnPoints;
    public GameObject[] charactersFBX;
    public AudioClip clip;

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
    
    private void OnPlayerJoined(PlayerInput playerInput)
    {
<<<<<<< HEAD

        
        
=======
        if(tempPlayerNb.howManyPlayer == 0)return;
        playersList.Add(playerInput);
        if (PlayerJoinedGame != null)
            PlayerJoinedGame(playerInput);
>>>>>>> parent of 2bddf0b (MultiPlayer menu)
        PlayerController playerController = playerInput.GetComponent<PlayerController>(); 
        playerController.playerId = playersList.Count;
        playerController.transform.position = spawnPoints[playerController.playerId - 1].position;
        playerController.playerFBX = Instantiate(charactersFBX[playerController.playerId - 1], playerController.transform);    
        playerController.toupieFBX = playerController.playerFBX.GetComponentInChildren<SpinningAnim>().gameObject;
        playerController.toupieFBX.SetActive(false);


        if(PlayerConfigManager.Instance.playersList.Count != tempPlayerNb.howManyPlayer)return;
        AudioSource readyGo = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Ready Go"], this.transform.position, AudioManager.instance.soundEffectMixer, true);
        AudioSource ost = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("OST"), transform.position, AudioManager.instance.ostMixer, false);

        ost.loop = true;
        StartCoroutine(WaitBeforeGameStart(readyGo.clip.length - 1.3f));
    }
    
<<<<<<< HEAD
    
=======
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
    }
    
    private void OnDisable()
    {
        joinAction.Disable();
        leftAction.Disable();
    }
>>>>>>> parent of 2bddf0b (MultiPlayer menu)

   

<<<<<<< HEAD
   
=======
    private void UnregisterPlayer(PlayerInput playerInput)
    {
        playersList.Remove(playerInput);
        if (PlayerLeftGame != null)
            PlayerLeftGame(playerInput);
        
        Destroy(playerInput.transform.gameObject);
    }
>>>>>>> parent of 2bddf0b (MultiPlayer menu)


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
                    
                        PlayVoiceAtTime(3, ref gameTimer.nearTimeOut, AudioManager.instance.allAudio["Voice 321"]);
                    break;

                    case < 60 :
                        OneMinuteRemaining();
                    break;
                }
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
                    AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Voice Sudden Death"], this.transform.position, AudioManager.instance.soundEffectMixer, true);
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
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio[$"{allPlayer.IndexOf(playerWinner) + 1} Win"], this.transform.position, AudioManager.instance.soundEffectMixer, true);
        gameTimer.timeOut = true;
        gameStarted = false;
        foreach(PlayerController player in allPlayer)
        {
            player.move = Vector3.zero;
        }
    }

    private void PlayVoiceAtTime(float time, ref bool alreadyPlayed, AudioClip voice)
    {
        if(alreadyPlayed)return;
        if(gameTimer.timer > time)return;
        AudioManager.instance.PlayClipAt(voice, this.transform.position, AudioManager.instance.soundEffectMixer, true);
        alreadyPlayed = true;
    }

    public void StartGame(TMP_InputField inputField)
    {
        if(inputField.contentType != TMP_InputField.ContentType.IntegerNumber)return;
        if(int.Parse(inputField.text) < 1 || int.Parse(inputField.text) > 4)return;

        tempPlayerNb.howManyPlayer = int.Parse(inputField.text);
        inputField.transform.parent.gameObject.SetActive(false);
        gameStarted = true;
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Title"], this.transform.position, AudioManager.instance.soundEffectMixer, true);
    }

    private IEnumerator WaitBeforeGameStart(float length)
    {
        gameStarted = false;
        yield return new WaitForSeconds(length);
        gameStarted = true;
        gameTimer.drawTimer = false;
    }

    private void OneMinuteRemaining()
    {
        if(tempPlayerNb.oneMinute)return;
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["One Minute Remaining"], this.transform.position, AudioManager.instance.soundEffectMixer, true);
        tempPlayerNb.oneMinute = true;
    }
}
