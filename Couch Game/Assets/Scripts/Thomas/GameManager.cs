using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public List<PlayerManager> allPlayer;
    public List<PlayerInput> playersList = new List<PlayerInput>();

    [SerializeField] private float timer;
    public float maxTimer = 60f;

    public bool drawTimer = false;
    public int drawMaxPoint = 0;
    public bool timeOut = false;

    public static GameManager instance;
    //Input
    public InputAction joinAction;
    public InputAction leftAction;
    
    //Event
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    
    private void OnPlayerJoined(PlayerInput playerInput)
    {
        playersList.Add(playerInput);
        if (PlayerJoinedGame != null)
            PlayerJoinedGame(playerInput);
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
        timer = maxTimer;
    }

    // Update is called once per frame
    private void Update()
    {
        //RoundTimer();
    }

    // private void RoundTimer()
    // {
    //     if(!timeOut)
    //     {
    //         if(!drawTimer)
    //         {
    //             if(timer > 0)
    //             {
    //                 timer -= Time.deltaTime;
    //             }
    //             else
    //             {
    //                 int playerPoint = 0;
    //                 PlayerController playerMaxPoint = null;
    //                 for(int i = 0; i < allPlayer.Length; i++)
    //                 {
    //                     if(allPlayer[i].playersInteract.playerPoint > playerPoint)
    //                     {
    //                         playerPoint = allPlayer[i].playersInteract.playerPoint;
    //                         playerMaxPoint = allPlayer[i];
    //                     }
    //                 }
                    
    //                 int equalTime = 0;
    //                 for(int i = 0; i < allPlayer.Length; i++)
    //                 {
    //                     if(playerPoint == allPlayer[i].playersInteract.playerPoint)equalTime += 1;
    //                 }

    //                 if(equalTime > 1)
    //                 {
    //                     Debug.Log("DRAWWW TIMME");
    //                     drawTimer = true;
    //                     drawMaxPoint = playerPoint;
    //                 }
    //                 else
    //                 {
    //                     Debug.Log($"{playerMaxPoint.name} WIN WITH {playerMaxPoint.playersInteract.playerPoint} POINTS !");
    //                     timeOut = true;
    //                 }
    //             }
    //         }
    //     }
    // }

}
