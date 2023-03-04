using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LD_Manager : MonoBehaviour
{
    [SerializeField] private GameObject[] ldElements;
    [SerializeField] [Tooltip("Enlève le dernier élement à la fin du dernier timer")] private bool removeElementInTheEnd = true;
    [Header("DOIT ETRE DE LA MEME TAILLE QUE LD ELEMENT ! \nLe dernier timer est celui avant le destroy potentiel de l'objet")]
    [SerializeField] private float[] elementTimer;
    [SerializeField] private float startTimer;

    [Header("DEBUG")]
    [SerializeField] private float actualTimer;
    [SerializeField] private int elementActualPos;
    [SerializeField] private GameObject actualElement;
    [SerializeField] private GameObject warningHasard;
    [SerializeField] private float warningTimer = 3f;
    private bool elementSpawned = true;
    // Start is called before the first frame update
    private void Start()
    {
        actualTimer = startTimer;
        warningHasard.SetActive(false);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Element();
    }

    private void Element()
    {
        if(elementActualPos > ldElements.Length || !GameManager.instance.gameStarted)return;
        if(!elementSpawned)
        {
            int element = Random.Range(0, ldElements.Length);
            actualElement = Instantiate(ldElements[element], this.transform.position, Quaternion.identity);
            //Debug.Log("Element Spawned : " + actualElement.name);
            elementSpawned = true;
        }

        if (actualTimer <= warningTimer) warningHasard.SetActive(true);      
        if(actualTimer > 0)actualTimer -= Time.deltaTime;
        else
        {
            warningHasard.SetActive(false);
            if(elementActualPos >= ldElements.Length)
            {
                if(actualElement != null && removeElementInTheEnd)Destroy(actualElement);
                return;
            }
            else
            {
                if(actualElement != null)Destroy(actualElement);
            }
            elementSpawned = false;
            //if(elementActualPos >= ldElements.Length)return;
            actualTimer = elementTimer[elementActualPos];
            elementActualPos += 1;
        }
    }
}
