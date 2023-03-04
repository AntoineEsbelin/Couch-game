using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;
    public GameObject screen1;
    public GameObject screen2;
    public Material projection;
    public bool started = false;

    private void Update()
    {
        if (player != null)
            transform.position = new Vector3(player.transform.position.x + 4, transform.position.y, player.transform.position.z - 10);
    }
    public void follow(GameObject playerToFollow)
    {
        if (!started)
        {
            screen1.GetComponent<MeshRenderer>().material = projection;
            screen2.GetComponent<MeshRenderer>().material = projection;
            started = true;
        }
        player = playerToFollow;
    }

}
