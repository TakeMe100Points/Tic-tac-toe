using Photon.Pun;
using UnityEngine;
using PN = Photon.Pun.PhotonNetwork;

namespace Assets.Scripts.Service
{
    /// <summary>
    /// Логика лобби
    /// </summary>
    public class LobbyController : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// На старте необходимо задать настройки
        /// </summary>
        void Start()
        {
            InitializeLobby();
        }

        #region PN Callbacks
        /// <summary>
        /// При присоединении к комнате загрузить уровень
        /// </summary>
        public override void OnJoinedRoom()
        {
            PN.LoadLevel(1);
        }
        #endregion

        /// <summary>
        /// Создать комнату с настройками
        /// </summary>
        public void CreateRoom()
        {
            PN.CreateRoom(null,
                new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }

        /// <summary>
        /// Присоединиться к случайной комнате
        /// </summary>
        public void JoinRandomRoom()
        {
            PN.JoinRandomRoom();
        }

        /// <summary>
        /// Инициализирование лобби настройками
        /// </summary>
        private void InitializeLobby()
        {
            PN.GameVersion = "1";                            //Версия игры
            PN.NickName = "player " + Random.Range(0, 10000);//Ник игрока
            PN.AutomaticallySyncScene = false;               //Автомат. Синхр. Сцен
            PN.ConnectUsingSettings();                       //Соединение с фотоном используя настройки (Привет кэп!)
        }
    }
}