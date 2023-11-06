using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefabs;
    [SerializeField] CanvasGroup scoreBoard;


    Dictionary<Player , ScoreBoardItem> scoreBoardItems = new Dictionary<Player , ScoreBoardItem>();

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreBoardItem(player);
        }
    }

    void AddScoreBoardItem(Player player)
    {
        ScoreBoardItem scoreBoardItem = Instantiate(scoreboardItemPrefabs, container).GetComponent<ScoreBoardItem>();
        scoreBoardItem.Initialize(player);
        scoreBoardItems[player] = scoreBoardItem;
    }


    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreBoardItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreBoardItem(otherPlayer);
    }

    void RemoveScoreBoardItem(Player player)
    {
        Destroy(scoreBoardItems[player].gameObject);
        scoreBoardItems.Remove(player);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.alpha = 0;
        }
    }
}
