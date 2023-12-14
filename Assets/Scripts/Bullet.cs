using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;
    public float bulletSpeed = 15f;
    public int damage = 10;
    public float bulletRange = 3f;
    [HideInInspector]
    public Photon.Realtime.Player owner;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Destroy(gameObject, bulletRange);
    }

    public void InitializeBullet(Vector3 originDirection, Photon.Realtime.Player givenPlayer)
    {
        transform.forward = originDirection;
        _rigidbody.velocity = transform.forward * bulletSpeed;
        owner = givenPlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Detected");
        Debug.Log("Collision with: " + collision.gameObject.name);
        Destroy(gameObject);
    }
    
    
}
