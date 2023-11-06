using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SingleShotgun : Gun 
{
    [SerializeField] Camera cam;
    PhotonView pv;

    public Recoil recoil;

    public GunInfo gunInfo;


    private bool reloading = false;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        //recoil = GameObject.Find("CameraRot/CameraRecoil").GetComponent<Recoil>();
        gunInfo.currentAmmo = gunInfo.maxAmmo;

    }

    void OnEnable()
    {
        reloading = false;
    }

   

    private void Update()
    {
        if (!reloading && gunInfo.currentAmmo <=0)
        {
            StartCoroutine(Reload());
        }
    
        
    }

    IEnumerator Reload() 
    {
        
        reloading = true;
        Debug.Log("relaoding");
        yield return new WaitForSeconds(3f);
        gunInfo.currentAmmo = gunInfo.maxAmmo;

        reloading = false;
    }

    public override void Use()
    {
        if (gunInfo.currentAmmo >0)
        {
            Shoot();
        }
    }


    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);

        }

        pv.RPC(nameof(RPC_Shoot), RpcTarget.All, hit.point, hit.normal);

        recoil.RecoilFire();

        gunInfo.currentAmmo--;

    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {

        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletimpactobj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletimpactobj, 3f);
            bulletimpactobj.transform.SetParent(colliders[0].transform);
        }
    }
}
