using Photon.Pun;
using UnityEngine;
using PN = Photon.Pun.PhotonNetwork;


public class LobbyController : MonoBehaviourPunCallbacks
{ 
    void Start()
    {
        InitializeLobby();
    }

    #region PN Callbacks
    public override void OnConnectedToMaster()
    {

    }

    public override void OnJoinedRoom()
    {
        PN.LoadLevel(1);
    }    
    #endregion

    public void CreateRoom()
    {
        PN.CreateRoom(null, 
            new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }

    public void JoinRandomRoom()
    {
        PN.JoinRandomRoom();
    }

    private void InitializeLobby()
    {
        PN.GameVersion = "1";
        PN.NickName = "player " + Random.Range(0, 1000);
        PN.AutomaticallySyncScene = true;
        PN.ConnectUsingSettings();
    }
}
