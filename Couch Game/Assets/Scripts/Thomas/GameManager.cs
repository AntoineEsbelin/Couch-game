using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController[] allPlayer;

    [SerializeField] private float timer;
    public float maxTimer = 60f;

    public bool drawTimer = false;
    public int drawMaxPoint = 0;
    public bool timeOut = false;

    public static GameManager instance;

    private void Awake()
    {
        if(instance != null)Destroy(gameObject);
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        allPlayer = FindObjectsOfType<PlayerController>();
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
