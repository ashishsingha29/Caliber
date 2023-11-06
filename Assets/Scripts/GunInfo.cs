using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damage;


    [HideInInspector]public Vector3 currentRotation;
    [HideInInspector]public Vector3 targetRotation; 

    [SerializeField] public float recoilX;
    [SerializeField] public float recoilY;
    [SerializeField] public float recoilZ;


    [SerializeField] public float snappiness;
    [SerializeField] public float returnSpeed;

     public int maxAmmo;
     public int currentAmmo;
     public float reloadTime;

    

}
