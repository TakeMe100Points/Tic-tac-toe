using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PN = Photon.Pun.PhotonNetwork;


public class LobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text logger;

    void Start()
    {
        InitializeLobby();
    }

    #region PN Callbacks
    public override void OnConnectedToMaster()
    {
        log("Connected to master server.");
    }

    public override void OnJoinedRoom()
    {
        log("player joined room");
        PN.LoadLevel(1);
    }    
    #endregion

    public void CreateRoom()
    {
        PN.CreateRoom(null, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRandomRoom()
    {
        PN.JoinRandomRoom();
    }

    private void log(string lg)
    {
        logger.text += "\n" + lg;
    }

    private void InitializeLobby()
    {
        PN.GameVersion = "1";
        PN.NickName = "player " + Random.Range(0, 1000);
        PN.AutomaticallySyncScene = true;
        PN.ConnectUsingSettings();
    }
}
