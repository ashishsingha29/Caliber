using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;    



    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform PlayerListContent;
    [SerializeField] GameObject roomListPrefab;
    [SerializeField] GameObject PlayerListPrefab;
    [SerializeField] GameObject StartGameButton;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("Title");
        Debug.Log("JoinedLobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("Room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;


        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in PlayerListContent)
        {
            Destroy(child.gameObject);
        }



        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);

        }

        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);


    }


    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "RoomCreationFailed" + message;
        MenuManager.instance.OpenMenu("Error");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("Loading");

    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("Loading");

       

    }

    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("Title");

    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {

            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);

    }
}
