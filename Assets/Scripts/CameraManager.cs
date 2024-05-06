using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    private CinemachineVirtualCamera virtualcam;
    [HideInInspector] public GameManager gamemanager;
    [HideInInspector] public Transform target;

    void Start()
    {
        virtualcam = GetComponent<CinemachineVirtualCamera>();
        if(gamemanager == null)
        {
            target = GameObject.FindGameObjectWithTag("Character").transform;
        }
        SetupCamTarget();
    }

    public void SetupCamTarget()
    {
        virtualcam.m_LookAt = target;
        virtualcam.m_Follow = target;
    }
}
