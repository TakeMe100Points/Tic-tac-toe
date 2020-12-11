using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

public class GameRoom : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] StartGame start;
    
    public void Leave()
    {
        PN.LeaveRoom();
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    #region PN Callbacks
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Другой игрок присоединился
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log(" joined!" + newPlayer.NickName);
    }

    public override void OnJoinedRoom()
    {
        if (PN.CurrentRoom.PlayerCount == 2)
        {
            //подрубить игру
            start.LetsStart();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        start.StopGame();
        Debug.Log(" left!" + otherPlayer.NickName);
    }
    #endregion
}
