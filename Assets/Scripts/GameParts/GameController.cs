using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using PN = Photon.Pun.PhotonNetwork;
using System.Linq;
using Assets.Scripts.Service;

namespace Assets.Scripts.GameParts
{
    /// <summary>
    /// Главный класс логики игры
    /// </summary>
    public class GameController : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Ссылка на сетку из ячеек
        /// </summary>
        [SerializeField] GridButtons grid;

        /// <summary>
        /// View компонент победного экрана, для вызова RPC
        /// </summary>
        [SerializeField] PhotonView winView;

        #region Private Fields
        /// <summary>
        /// Соответствие игрока и его текущего знака (Крестик/Нолик)
        /// </summary>
        private Dictionary<int, int> dic;

        /// <summary>
        /// Actor Number игрока, у которого сейчас ход
        /// </summary>
        private int turnActor;

        /// <summary>
        /// Клиент. Коротко и ясно.
        /// </summary>
        private Player client;
        #endregion


        #region subcribe/unsubcribe to events
        public override void OnEnable()
        {
            base.OnEnable();
            PN.NetworkingClient.EventReceived += NetworkingClient_EventReceived;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            PN.NetworkingClient.EventReceived -= NetworkingClient_EventReceived;
        }
        #endregion


        /// <summary>
        /// Т.к. игра на двоих, то достаточно дождаться еще одного и получить всех игроков
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            client = newPlayer;
            //Запустить игру
            RestartLevel();
        }

        /// <summary>
        /// Перезапуск игры осуществляется только хостом
        /// Пересоздается сетка из ячеек, и убирается баннер выйгрыша
        /// </summary>
        public void RestartLevel()
        {
            if (!PN.IsMasterClient) return;

            grid.ResetGrid();
            winView.RPC("HideWinBanner", RpcTarget.All);
            onBeginRound();
        }

        /// <summary>
        /// При начале раунда необходимо инициализировать словарь
        /// </summary>
        public void onBeginRound()
        {
            if (PN.CountOfPlayers > 1)
                InitializeDictionary(PN.PlayerListOthers[0]);
        }


        /// <summary>
        /// Переинициализировать словарь, 
        /// Actor Number, index
        /// Где индекс это текущий значек игрока
        /// </summary>
        /// <param name="newPlayer"></param>
        public void InitializeDictionary(Player newPlayer)
        {
            dic = new Dictionary<int, int>();

            //Получаем индекс
            int ind = Random.Range(0, 100) >= 50 ? 1 : 0;
            //Решаем кто первый ходит
            turnActor = ind == 1 ? newPlayer.ActorNumber : PN.LocalPlayer.ActorNumber;

            //Добавить в словарь значения
            dic.Add(newPlayer.ActorNumber, ind);
            dic.Add(PN.LocalPlayer.ActorNumber, 1 - ind);
        }

        /// <summary>
        /// Обрабатывает событие нажатия на кнопку
        /// </summary>
        /// <param name="obj"></param>
        private void NetworkingClient_EventReceived(ExitGames.Client.Photon.EventData obj)
        {
            //Обрабатывается только смена значка от хоста
            if (obj.Code != RPC_codes.CHANGEMARK || !PN.IsMasterClient) return;

            //Преобразуем получаеные данные к массиву объектов
            object[] datas = obj.CustomData as object[];

            //И его в соотв. переменные
            var actorNumber = (int)datas[0];     //Идентиф. номер игрока
            var viewId = (int)datas[1];     //ViewId кнопки на которую нажали
            var isButtonMarked = (bool)datas[2];    //Назначенно ли ей значение

            //Обрабатываем нажатия игроков, у которых сейчас ход
            if (turnActor != actorNumber || isButtonMarked) return;
            else SwapTurn(); //И меняем его на следующего

            //Получаем индекс нужного значка
            int index = dic[actorNumber];

            //И отправляем обратно кнопке для установки
            photonView.RPC("UpdateButton", RpcTarget.All, viewId, index);
            CheckWinner();      //Победил ли кто?
        }


        /// <summary>
        /// Смена хода
        /// </summary>
        private void SwapTurn()
        {
            turnActor = turnActor == PN.LocalPlayer.ActorNumber ? client.ActorNumber : PN.LocalPlayer.ActorNumber;
        }


        /// <summary>
        /// Алгоритм проверки победителя
        /// Считается сумма значений в строке, столбике и диагоналях
        /// Если ее сумма +-3, то выйграл кто-то,
        /// Кто выйграл определяет знак
        /// </summary>
        private void CheckWinner()
        {

            int[,] board = grid.GetArrayOfValue();  //Матрица значений для каждой ячейки

            int[] rows = new int[grid.gridSize];    //Сумма строк 
            int[] columns = new int[grid.gridSize]; //Сумма столбцов

            int mainDiagonal = 0;                   //Сумма гл. диагонали
            int subDiagonal = 0;                    //Сумма побочн. диагонали

            for (int i = 0; i < grid.gridSize; i++)
                for (int j = 0; j < grid.gridSize; j++)
                {
                    //Считаем суммы
                    rows[i] += board[i, j];
                    columns[j] += board[i, j];

                    if (i == j)
                        mainDiagonal += board[i, j];
                    if (i + j + 1 == grid.gridSize)
                        subDiagonal += board[i, j];
                }

            //Проверяем, выйграл ли кто-то
            if (mainDiagonal == grid.gridSize || subDiagonal == grid.gridSize)
            {
                Debug.Log("win first!");
                CallShowWinBannerForIndex(1);
                return;
            }
            else if (mainDiagonal == -grid.gridSize || subDiagonal == -grid.gridSize)
            {
                Debug.Log("Win second!");
                CallShowWinBannerForIndex(0);
                return;
            }

            for (int i = 0; i < grid.gridSize; i++)
            {
                if (rows[i] == grid.gridSize || columns[i] == grid.gridSize)
                {
                    CallShowWinBannerForIndex(1);
                    Debug.Log("win first!");
                    return;
                }
                if (rows[i] == -grid.gridSize || columns[i] == -grid.gridSize)
                {
                    CallShowWinBannerForIndex(0);
                    Debug.Log("win second!");
                    return;
                }
            }
        }

        /// <summary>
        /// Из номера значка определить игрока и вывести его имя на баннере победы
        /// </summary>
        /// <param name="index"></param>
        private void CallShowWinBannerForIndex(int index)
        {
            //Получение номера игрока
            //      Выбрать Actor Number со значем индекса значка == index 
            var actorId = dic.First(x => x.Value == index).Key;
            //                  Выбрать игрока c ActorNumber == actorId        
            string nickname = PN.PlayerList.First(x => x.ActorNumber == actorId).NickName;

            //Показать победителя
            winView.RPC("ShowWinBanner", RpcTarget.All, nickname);
        }

        /// <summary>
        /// Обновить значек кнопки
        /// </summary>
        /// <param name="id">Идентификатор кнопки (ViewId)</param>
        /// <param name="index">Индекс значка</param>
        [PunRPC]
        public void UpdateButton(int id, int index)
        {
            PhotonView.Find(id).GetComponent<CellOfGrid>().ChangeIcon(index);
        }
    }
}