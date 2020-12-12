using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

namespace Assets.Scripts.Service
{
    /// <summary>
    /// Класс логики поведения комнаты
    /// </summary>
    public class GameRoom : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Скрипт баннера старта
        /// </summary>
        [SerializeField] StartGame start;

        /// <summary>
        /// Для хоста врубить ожидание игрока
        /// </summary>
        private void Start()
        {
            if (PN.IsMasterClient)
                start.ShowStartBanner();
        }

        /// <summary>
        /// Покинуть комнату
        /// </summary>
        [PunRPC]
        public void Leave()
        {
            PN.LeaveRoom();
        }

        #region PN Callbacks
        /// <summary>
        /// Колбэк при выходе из комнаты
        /// </summary>
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
            base.OnPlayerEnteredRoom(newPlayer);
            //Убрать баннер ожидания
            start.HideStartBanner();
        }

        /// <summary>
        /// При смене хоста все игроки покинут комнату
        /// </summary>
        /// <param name="newMasterClient"></param>
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            base.OnMasterClientSwitched(newMasterClient);
            //Покинуть комнату вслед за хостом
            photonView.RPC("Leave", newMasterClient);
        }

        /// <summary>
        /// При присоединении игрока стартовый баннер уже не нужен
        /// </summary>
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            //Убрать банер ожидания
            start.HideStartBanner();
        }
        #endregion
    }
}