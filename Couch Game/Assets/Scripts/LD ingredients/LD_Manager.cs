using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LD_Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] ldElements;
    [SerializeField] [Tooltip("Enlève le dernier élement entre chaque timer")] private bool removeElement = true;
    [Header("DOIT ETRE DE LA MEME TAILLE QUE LD ELEMENT !")]
    [SerializeField] private float[] elementTimer;
    [Header("Actual Element & Timer")]
    [SerializeField] private float actualTimer;
    [SerializeField] private int elementActualPos;
    [SerializeField] private GameObject actualElement;
    private bool elementSpawned = false;
    // Start is called before the first frame update
    private void Start()
    {
        actualTimer = elementTimer[0];
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Element();
    }

    private void Element()
    {
        if(elementActualPos >= ldElements.Length)return;
        if(!elementSpawned)
        {
            actualElement = Instantiate(ldElements[elementActualPos], this.transform);
            elementSpawned = true;
        }

        if(actualTimer > 0)actualTimer -= Time.deltaTime;
        else
        {
            elementActualPos += 1;
            if(elementActualPos >= ldElements.Length)return;
            if(removeElement)Destroy(actualElement);
            actualTimer = elementTimer[elementActualPos];
            elementSpawned = false;
        }
    }
}
