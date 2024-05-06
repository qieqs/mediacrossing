using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera virtualcam;
    [HideInInspector] private GameManager gamemanager;
    [HideInInspector] private Transform target;

    void Start()
    {
        virtualcam = GetComponent<CinemachineVirtualCamera>();
        gamemanager = GetComponent<GameManager>();
        if(gamemanager == null)
        {
            target = GameObject.FindGameObjectWithTag("Character").transform;
        }
        else
        {
            target = gamemanager.camtarget.transform; 
        }
        SetupCamTarget();
    }

    private void SetupCamTarget()
    {
        virtualcam.m_LookAt = target;
        virtualcam.m_Follow = target;
    }
}
