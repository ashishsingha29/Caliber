using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks , IDamageable
{
    [SerializeField] Image healthBar;
    [SerializeField] GameObject ui;

    [SerializeField] GameObject cameraHolder;
    [SerializeField] GameObject cameraHolder2;

    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpforce, smoothTIme;

    [SerializeField] Item[] items;

    
   

    int itemIndex;
    int previousItemIndex = -1;


    float verticalLookRotation;
    bool grounded;
    Vector3 smoothVelocity;
    Vector3 moveamount;

    Rigidbody rb;

    PhotonView pv;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        
        
        if(pv.IsMine)
        {
            EquipItem(1);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }
    }

    private void Update()
    {
        if (!pv.IsMine)
        {
            return;
        }

        LookAround();
        Move();
        Jump();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }

        if (transform.position.y < -10f)
        {
            Die();
        }
       
    }

     
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveamount = Vector3.SmoothDamp(moveamount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothVelocity, smoothTIme);
    }

    void Jump() 
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpforce);
        }
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex)
        {
            return;
        }


        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameObject.SetActive(false);

        }

        previousItemIndex = itemIndex;

        if (pv.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }



    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !pv.IsMine && targetPlayer == pv.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }


    void LookAround()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        cameraHolder2.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;

    }

    private void FixedUpdate()
    {
        if (!pv.IsMine)
        {
            return;
        }
        rb.MovePosition(rb.position + transform.TransformDirection(moveamount) * Time.fixedDeltaTime);
    }


    public void TakeDamage(float Damage) 
    {
        pv.RPC(nameof(RPC_TakeDamage), pv.Owner, Damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float Damage , PhotonMessageInfo info)
    {
        
        currentHealth -=  Damage;
        healthBar.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }

    }

    void Die() 
    {
        playerManager.Die();
    }
}
