using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dray : MonoBehaviour, IFacingMover, IKeyMaster
{
    public enum eMode
    {
        idle,
        move,
        attack,
        transition,
        knockBack
    };
    
    [Header("Set in Inspector")] 
    public float speed = 5;
    public float attackDuration = 0.25f;
    public float attackDelay = 0.5f;
    public float transitionDelay = 0.5f;
    public int maxHealth = 10;
    public float knockBackSpeed = 10;
    public float knockBackDuration = 0.25f;
    public float invincibleDuration = 0.5f;

    [Header("Set Dynamically")] 
    public int dirHeld = -1;
    public int facing = 1;
    public eMode mode = eMode.idle;
    public int numKeys = 0;
    public bool invincible = false;
    public bool hasGrappler = false;
    public Vector3 lastSafeLoc;
    public int lastSafeFacing;

    [SerializeField] 
    private int _health;

    public int health
    {
        get { return _health; }
        set { _health = value; }
    }

    private float timeAttackDone = 0;
    private float timeAttackNext = 0;

    private float transitionDone = 0;
    private Vector2 transitionPos;
    private float knockBackDone = 0;
    private float invincibleDone = 0;
    private Vector3 knockBackVel;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private InRoom _inRoom;

    private Vector3[] directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down};

    private KeyCode[] keys = new KeyCode[]
        { KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow };
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _inRoom = GetComponent<InRoom>();
        health = maxHealth;
        lastSafeLoc = transform.position;
        lastSafeFacing = facing;
    }

    private void Update()
    {
        if (invincible && Time.time > invincibleDone) invincible = false;
        _spriteRenderer.color = invincible ? Color.red : Color.white;
        if (mode == eMode.knockBack)
        {
            _rigidbody.velocity = knockBackVel;
            if(Time.time < knockBackDone) return;
        }

        if (mode == eMode.transition)
        {
            _rigidbody.velocity = Vector3.zero;
            _animator.speed = 0;
            roomPos = transitionPos;
            if(Time.time < transitionDone) return;
            mode = eMode.idle;
        }
        
        dirHeld = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Input.GetKey(keys[i])) dirHeld = i;
        }

        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= timeAttackNext)
        {
            mode = eMode.attack;
            timeAttackDone = Time.time + attackDuration;
            timeAttackNext = Time.time + attackDelay;
        }

        if (Time.time >= timeAttackDone)
        {
            mode = eMode.idle;
        }

        if (mode != eMode.attack)
        {
            if (dirHeld == -1)
            {
                mode = eMode.idle;
            }
            else
            {
                facing = dirHeld;
                mode = eMode.move;
            }
        }
        
        Vector3 vel = Vector3.zero;
        switch (mode)
        {
            case eMode.attack:
                _animator.CrossFade("Dray_Attack_" + facing, 0);
                _animator.speed = 0;
                break;
            
            case eMode.idle:
                _animator.CrossFade("Dray_Walk_" + facing, 0);
                _animator.speed = 0;
                break;
            
            case eMode.move:
                vel = directions[dirHeld];
                _animator.CrossFade("Dray_Walk_" + facing, 0);
                _animator.speed = 1;
                break;
        }

        _rigidbody.velocity = vel * speed;
    }

    private void LateUpdate()
    {
        Vector2 rPos = GetRoomPosOnGrid(0.5f);

        int doorNum;
        for (doorNum = 0; doorNum < 4; doorNum++)
        {
            if (rPos == InRoom.DOORS[doorNum])
            {
                break;
            }
        }

        if (doorNum > 3 || doorNum != facing) return;

        Vector2 rm = roomNum;
        switch (doorNum)
        {
            case 0:
                rm.x += 1;
                break;
            case 1:
                rm.y += 1;
                break;
            case 2:
                rm.x -= 1;
                break;
            case 3:
                rm.y -= 1;
                break;
        }

        if (rm.x >= 0 && rm.x <= InRoom.MAX_RM_X)
        {
            if (rm.y >= 0 && rm.y <= InRoom.MAX_RM_Y)
            {
                roomNum = rm;
                transitionPos = InRoom.DOORS[(doorNum + 2) % 4];
                roomPos = transitionPos;
                lastSafeLoc = transform.position;
                lastSafeFacing = facing;
                mode = eMode.transition;
                transitionDone = Time.time + transitionDelay;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(invincible) return;
        DamageEffect damageEffect = other.gameObject.GetComponent<DamageEffect>();
        if(damageEffect == null) return;

        health -= damageEffect.damage;
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;

        if (damageEffect.knockBack)
        {
            Vector3 delta = transform.position - other.transform.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                // ???????????????????????? ???? ??????????????????????
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            }
            else
            {
                // ?????????????????????? ???? ??????????????????
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            knockBackVel = delta * knockBackSpeed;
            _rigidbody.velocity = knockBackVel;

            mode = eMode.knockBack;
            knockBackDone = Time.time + knockBackDuration;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PickUp pickUp = other.GetComponent<PickUp>();
        if (pickUp == null) return;

        switch (pickUp.itemType)
        {
            case PickUp.eType.health:
                health = Mathf.Min(health + 2, maxHealth);
                break;
            
            case PickUp.eType.key:
                keyCount++;
                break;
            
            case PickUp.eType.grapper:
                hasGrappler = true;
                break;
        }
        
        Destroy(other.gameObject);
    }

    public void ResetInRoom(int healthLoss = 0)
    {
        transform.position = lastSafeLoc;
        facing = lastSafeFacing;
        health -= healthLoss;

        invincible = true;
        invincibleDone = Time.time + invincibleDuration;
    }

    public int GetFacing()
    {
        return facing;
    }

    public bool moving
    {
        get { return (mode == eMode.move); }
    }

    public float GetSpeed()
    {
        return speed;
    }

    public float gridMult
    {
        get { return _inRoom.gridMult; }
    }

    public Vector2 roomPos
    {
        get { return _inRoom.roomPos; }
        set { _inRoom.roomPos = value; }
    }

    public Vector2 roomNum
    {
        get { return _inRoom.roomNum;}
        set { _inRoom.roomNum = value; }
    }

    public Vector2 GetRoomPosOnGrid(float mult = -1)
    {
        return _inRoom.GetRoomPosOnGrid(mult);
    }

    public int keyCount
    {
        get { return numKeys;}
        set { numKeys = value; }
    }
}
