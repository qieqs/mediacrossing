using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CameraManager cameraManager;
    public SpelerController[] spelers;


    void Start()
    {
        cameraManager.gamemanager = this;
        spelers = FindObjectsOfType<SpelerController>();
        if(spelers == null)
        {
            Debug.LogError("er zijn geen spelers in het spel");
        }
        else
        {
            StartCoroutine(followroutine());
        }
    }

    void Update()
    {
        
    }

    private Transform characterselect()
    {
        int randomnumber = Random.Range(0, spelers.Length);
        return spelers[randomnumber].transform;
    }

    private IEnumerator followroutine()
    {
        cameraManager.target = characterselect();
        cameraManager.SetupCamTarget();
        yield return new WaitForSeconds(Random.Range(10,20));
    }
}
