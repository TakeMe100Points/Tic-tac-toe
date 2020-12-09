using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

public class GameRoom : MonoBehaviourPunCallbacks
{ 


    void Start()
    {
        
    }

    public void Leave()
    {
        PN.LeaveRoom();
    }

    void Update()
    {
        
    }


    #region PN Callbacks
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(" joined!" + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(" left!" + otherPlayer.NickName);
    }
    #endregion
}
