using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour
{
    [Header("Set in Inspector")] 
    public float speed = 5;

    [Header("Set Dynamically")] 
    public int dirHeld = -1;

    private Rigidbody rigidbody;
    private Animator _animator;

    private Vector3[] directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down};

    private KeyCode[] keys = new KeyCode[]
        { KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow };
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        dirHeld = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKey(keys[i])) dirHeld = i;
        }
        
        Vector3 vel = Vector3.zero;
        if (dirHeld > -1) vel = directions[dirHeld];

        rigidbody.velocity = vel * speed;

        if (dirHeld == -1)
        {
            _animator.speed = 0;
        }
        else
        {
            _animator.CrossFade("Dray_Walk_"+dirHeld, 0);
            _animator.speed = 1;
        }
    }
}
