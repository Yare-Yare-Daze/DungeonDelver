using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollowDray : MonoBehaviour
{
    public static bool TRANSITIONING = false;

    [Header("Set in Inspector")] 
    public InRoom drayInRoom;
    public float transTime = 0.5f;

    private Vector3 p0, p1;

    private InRoom inRoom;
    private float transStart;

    private void Awake()
    {
        inRoom = GetComponent<InRoom>();
    }

    private void Update()
    {
        if (TRANSITIONING)
        {
            float u = (Time.time - transStart) / transTime;
            if (u >= 1)
            {
                u = 1;
                TRANSITIONING = false;
            }

            transform.position = (1 - u) * p0 + u * p1;
        }
        else
        {
            if (drayInRoom.roomNum != inRoom.roomNum)
            {
                TransitionTo(drayInRoom.roomNum);
            }
        }
    }

    void TransitionTo(Vector2 rm)
    {
        p0 = transform.position;
        inRoom.roomNum = rm;
        p1 = transform.position + (Vector3.back * 10);
        transform.position = p0;

        transStart = Time.time;
        TRANSITIONING = true;

    }
}
