using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour, IPunObservable
{
    public float movementSpeed = 10f;
    public float rotationSpeed = 10f;
    private Vector3 movement;
    private Rigidbody _rigidbody;
    private CharacterController _controller;
    public float fireRate = 0.75f;
    private float nextFire;
    public GameObject bulletPrefab;
    public Transform bulletOrigin;
    public int hp = 100;
    public Slider hpBar;
    [SerializeField] private LayerMask groundMask;
    private Camera _camera;
    private PhotonView _photonView;
    
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        
        Move();
        Aim();
        if (Input.GetKey(KeyCode.Mouse0))
        {
           _photonView.RPC("Fire", RpcTarget.AllViaServer);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
        }
        else
        {
            hp = (int)stream.ReceiveNext();
            hpBar.value = hp;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
           Bullet bullet = collision.gameObject.GetComponent<Bullet>();
           TakeDamage(bullet);
        }
    }

    void TakeDamage(Bullet bullet)
    {
        hp -= bullet.damage;
        hpBar.value = hp;
        if (hp <= 0)
        {
            bullet.owner.AddScore(1);
            PlayerDied();
        }
    }

    void PlayerDied()
    {
        hp = 100;
        hpBar.value = hp;
    }

    void Move()
   {
       if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
           return;
       
       float horizontalInput = Input.GetAxis("Horizontal");
       float verticalInput = Input.GetAxis("Vertical");

       Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
       movement.Normalize();
       
       _controller.Move(movement * movementSpeed * Time.deltaTime);
   }

    void Aim()
    {
        Ray cameraRay = _camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }


    [PunRPC]
   void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            GameObject bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.identity);

            bullet.GetComponent<Bullet>()?.InitializeBullet(transform.rotation * Vector3.forward, _photonView.Owner);
        }
    }
}
