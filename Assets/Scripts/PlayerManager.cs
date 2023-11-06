using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun.UtilityScripts;

public class PlayerManager : MonoBehaviour
{
    PhotonView pv;

    GameObject controller;

    int kills;
    int deaths;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (pv.IsMine)
        {
            CreateController();
        }
    }


    void CreateController()
    {
        Transform spawnPoint = SpawnManager.instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"),spawnPoint.position ,spawnPoint.rotation, 0, new object[] {pv.ViewID});
    }
    
    public void Die()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;


        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        pv.RPC(nameof(RPC_Getkill), pv.Owner);
    }

    [PunRPC]
    void RPC_Getkill()
    {
        kills++;


        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }
}
