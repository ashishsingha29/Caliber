
using UnityEngine;

public class Recoil : MonoBehaviour
{
    

    public GunInfo gunInfo;

    

    // Update is called once per frame
    void Update()
    {
        gunInfo.targetRotation = Vector3.Lerp(gunInfo.targetRotation, Vector3.zero, gunInfo.returnSpeed * Time.deltaTime);
        gunInfo.currentRotation = Vector3.Slerp(gunInfo.currentRotation , gunInfo.targetRotation , gunInfo.snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(gunInfo.currentRotation);
    }

    public void RecoilFire()
    {
        gunInfo.targetRotation += new Vector3(gunInfo.recoilX, gunInfo.recoilY, Random.Range(-gunInfo.recoilZ, gunInfo.recoilZ));
    }


}
