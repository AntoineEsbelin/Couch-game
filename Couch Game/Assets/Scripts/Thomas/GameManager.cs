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
    
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        
        playersList.Add(playerInput);
        if (PlayerJoinedGame != null)
            PlayerJoinedGame(playerInput);

        PlayerController playerController = playerInput.GetComponent<PlayerController>(); 
        playerController.playerId = playersList.Count;
        playerController.transform.position = spawnPoints[playerController.playerId - 1].position;
        playerController.playerFBX = Instantiate(charactersFBX[playerController.playerId - 1], playerController.transform);    
        playerController.toupieFBX = playerController.playerFBX.GetComponentInChildren<SpinningAnim>().gameObject;
        playerController.toupieFBX.SetActive(false);
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
    }
    
    private void OnDisable()
    {
        joinAction.Disable();
        leftAction.Disable();
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
        AudioSource ost = AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio.GetValueOrDefault("OST"), transform.position, AudioManager.instance.ostMixer);
        ost.loop = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        RoundTimer();
    }

    private void RoundTimer()
    {
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
                if(equalTime >= 1)
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
        Debug.Log($"{playerWinner.name} WIN WITH {playerWinner.playerPoint} POINTS !");
        gameTimer.timeOut = true;
        
        //general victory voice sound
        AudioManager.instance.PlayClipAt(AudioManager.instance.allAudio["Voice Victory"], this.transform.position, AudioManager.instance.soundEffectMixer);
    }

    private void PlayVoiceAtTime(float time, ref bool alreadyPlayed, AudioClip voice)
    {
        if(alreadyPlayed)return;
        if(gameTimer.timer > time)return;
        AudioManager.instance.PlayClipAt(voice, this.transform.position, AudioManager.instance.soundEffectMixer, true);
        alreadyPlayed = true;
    }
}
