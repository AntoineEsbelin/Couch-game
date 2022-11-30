using System;
using System.Collections;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerConfigManager : MonoBehaviour
{
    //Input Join and Leave
    public InputAction joinAction;
    public InputAction leftAction;
    
    //Event
    public event System.Action<PlayerInput> PlayerJoinedGame;
    public event System.Action<PlayerInput> PlayerLeftGame;
    
    public List<PlayerInput> playersList = new List<PlayerInput>();
    
    public List<PlayerConfig> _playerConfigs;
    [SerializeField] private int maxPlayers = 4;
    
    public static PlayerConfigManager Instance { get; private set;}

    private void Awake()
    {
        if (Instance != null)
        {
            print("SINGLETON - Trying to create another instance");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            _playerConfigs = new List<PlayerConfig>();
        }
        
        
        joinAction.Enable();
        joinAction.performed += ctx => JoinAction(ctx);
        
        leftAction.Enable();
        leftAction.performed += ctx => LeftAction(ctx);
    }


    private void Update()
    {
        for (int i = 0; i < playersList.Count; i++)
        {
            if (playersList[i] == null)
            {
                playersList.Remove(playersList[i]);
                
            }

            
        }
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
    
    public List<PlayerConfig> GetPlayerConfig()
    {
        return _playerConfigs;
    }

    public void ReadyPlayer(int index)
    {
        
        print(_playerConfigs.All(p => p.isReady));
        if (IsBetween(_playerConfigs.Count, 1,maxPlayers) && _playerConfigs.All(p => p.isReady))
        {
            MainMenuManager.Instance.MapSelectButton();
        }
    }

    public void OnPlayerJoined(PlayerInput pi)
    {
        playersList.Add(pi);
        if (PlayerJoinedGame != null)
            PlayerJoinedGame(pi);
        print("Player Joined : " + pi.playerIndex);
        if(!_playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            _playerConfigs.Add(new PlayerConfig(pi));
            _playerConfigs[pi.playerIndex].isReady = true;
            
        }
    }
    
    public bool IsBetween(double testValue, double bound1, double bound2)
    {
        return (testValue >= Math.Min(bound1,bound2) && testValue <= Math.Max(bound1,bound2));
    }
    
    
    
    public class PlayerConfig
    {
    
        public PlayerConfig(PlayerInput pi)
        {
            PlayerIndex = pi.playerIndex;
            Input = pi;
        }
    
        public PlayerInput Input { get; set; }
        public int PlayerIndex { get; set; }
        public bool isReady { get; set; }
    }
}
