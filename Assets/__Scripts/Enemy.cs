using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected static Vector3[] directions = new Vector3[] { Vector3.right, Vector3.up, Vector3.left, Vector3.down };

    [Header("Set in Inspector: Enemy")] 
    public float maxHealth = 1;
    public float knockBackSpeed = 10;
    public float knockBackDuration = 0.25f;
    public float invincibleDuration = 0.5f;
    public GameObject guaranteedItemDrop = null;

    [Header("Set Dynamically: Enemy")] 
    public float health;
    public bool invincible = false;
    public bool knockBack = false;

    private float invincibleDone = 0;
    private float knockBackDone = 0;
    private Vector3 knockBackVel;

    protected Animator _animator;
    protected Rigidbody _rigidbody;
    protected SpriteRenderer _sRenderer;
    
    protected virtual void Awake()
    {
        health = maxHealth;
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _sRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (invincible && Time.time > invincibleDone) invincible = false;
        _sRenderer.color = invincible ? Color.red : Color.white;
        if (knockBack)
        {
            _rigidbody.velocity = knockBackVel;
            if(Time.time < knockBackDuration) return;
        }

        _animator.speed = 1;
        knockBack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (invincible) return;
        DamageEffect damageEffect = other.gameObject.GetComponent<DamageEffect>();
        if (damageEffect == null) return;

        health -= damageEffect.damage;
        if (health <= 0) Die();
        invincible = true;
        invincibleDone = Time.time + invincibleDuration;

        if (damageEffect.knockBack)
        {
            Vector3 delta = transform.position - other.transform.root.position;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                delta.x = (delta.x > 0) ? 1 : -1;
                delta.y = 0;
            }
            else
            {
                delta.x = 0;
                delta.y = (delta.y > 0) ? 1 : -1;
            }

            knockBackVel = delta * knockBackSpeed;
            _rigidbody.velocity = knockBackVel;

            knockBack = true;
            knockBackDone = Time.time + knockBackDuration;
            _animator.speed = 0;
        }
    }

    void Die()
    {
        GameObject go;
        if (guaranteedItemDrop != null)
        {
            go = Instantiate<GameObject>(guaranteedItemDrop);
            go.transform.position = transform.position;
        }
        Destroy(gameObject);
    }
}
