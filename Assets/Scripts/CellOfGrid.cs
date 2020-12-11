using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using PN = Photon.Pun.PhotonNetwork;

public class CellOfGrid : MonoBehaviourPun
{
    

    /// <summary>
    /// Событие клика на иконку
    /// </summary>
    public System.Action<CellOfGrid> onCellClicked;

    /// <summary>
    /// Изображение
    /// </summary>
    [SerializeField] private Image currentIcon;

    [SerializeField] private Sprite player1;
    [SerializeField] private Sprite player2;
    
    private byte CHANGE_MARK = 12;

    public void OnCellClicked()
    {
        //onCellClicked.Invoke(this);
        UpdateMark(1);
    }

    private void OnEnable()
    {
        PN.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
    }
    private void OnDisable()
    {
        PN.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
    }


    private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
    {
        if (obj.Code != CHANGE_MARK) return;
        
        object[] datas = obj.CustomData as object[];

        if(photonView.ViewID == (int) datas[1])
        {
            int index = (int) datas[0];

            if (index == 1)
                currentIcon.sprite = player1;
            else
                currentIcon.sprite = player2;
        }
    }



    public void UpdateMark(int player)
    {
        object[] datas = new object[] { player, photonView.ViewID };

        RaiseEventOptions options = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache
        };

        PN.RaiseEvent(CHANGE_MARK, datas, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
}
