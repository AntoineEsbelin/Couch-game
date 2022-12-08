using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour
{
    private Animator botAnim;

    private void Start()
    {
        botAnim = this.GetComponent<Animator>();
    }
    public IEnumerator CrowdCheer()
    {
        botAnim.SetBool("Cheering", true);
        float randomCheerTime = Random.Range(GameManager.instance.cheerMinTime, GameManager.instance.cheerMaxTime);
        yield return new WaitForSeconds(randomCheerTime);
        botAnim.SetBool("Cheering", false);
    }
}
