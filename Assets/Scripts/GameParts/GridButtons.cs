using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using PN = Photon.Pun.PhotonNetwork;

namespace Assets.Scripts.GameParts
{

    /// <summary>
    /// Класс управления сеткой кнопко
    /// </summary>
    public class GridButtons : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Префаб ячейки
        /// </summary>
        [SerializeField] private GameObject cellPrefab;

        /// <summary>
        /// Размер сетки, должен быть 3 :)
        /// Общедоступен для получения в игровом контроллере размера сетки
        /// </summary>
        [Header("Size grid [x][x]")] public int gridSize = 3;

        /// <summary>
        /// Матрица ячеек
        /// </summary>
        private CellOfGrid[,] _grid;

        /// <summary>
        /// В начале заспавнить ячейки, спавняться они для вида
        /// Потому что при подключении клиента, ячейки пересоздадуться
        /// </summary>
        private void Start()
        {
            InstantiateCells();
        }

        /// <summary>
        /// Пересоздание ячеек
        /// </summary>
        public void ResetGrid()
        {
            //Уничтожить существующие
            foreach (var cell in _grid)
                PN.Destroy(cell.photonView);

            //Создать новые, и отправить их клиенту для синхронизации
            InstantiateCells();
            SendToClientCellViewId();
        }


        /// <summary>
        /// Создать новые ячейки (!Только мастер клиент) 
        /// </summary>
        private void InstantiateCells()
        {
            //Создаются только на мастере
            if (PN.IsMasterClient)
            {
                _grid = new CellOfGrid[gridSize, gridSize];

                for (int i = 0; i < gridSize; i++)
                    for (int j = 0; j < gridSize; j++)
                    {
                        //Создание и привязка к родителю 
                        _grid[i, j] = PN.Instantiate(cellPrefab.name, transform.position, Quaternion.identity).GetComponent<CellOfGrid>();
                        _grid[i, j].transform.SetParent(gameObject.transform);
                    }
            }
        }

        /// <summary>
        /// Получение матрицы значений из сетки,
        /// Каждая ячейка уже хранит свое значение
        /// </summary>
        /// <returns></returns>
        public int[,] GetArrayOfValue()
        {
            int[,] result = new int[3, 3];

            //Копировать значения в двумерный массив и вернуть его
            for (int i = 0; i < gridSize; i++)
                for (int j = 0; j < gridSize; j++)
                    result[i, j] = _grid[i, j].value;

            return result;
        }

        /// <summary>
        /// При присоединении игрока, необходимо отправить ему viewID ячеек для установки их родителя
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);

            SendToClientCellViewId();
        }

        /// <summary>
        /// Отправить ViewId ячеек клиенту для установки родителя
        /// </summary>
        private void SendToClientCellViewId()
        {
            // Отправка только из хоста
            if (PN.IsMasterClient)
            {
                int[] viewsID = new int[gridSize * gridSize];

                //Просто копируем идентификаторы, только в одномерный массив
                //Т.к. Photon умеет отправлять только одномерные массивы
                for (int j = 0; j < gridSize; j++)
                    for (int i = 0; i < gridSize; i++)
                    {
                        viewsID[j * gridSize + i] = _grid[j, i].photonView.ViewID;
                    }

                //Вызов функции у клиента
                photonView.RPC("RPC_SetParent", RpcTarget.Others, viewsID);
            }
        }


        /// <summary>
        /// Вызывается у клиента для установки родителя
        /// Иначе созданные ячейки у клиента будут в на вершине иерархии и не будут рендериться
        /// </summary>
        /// <param name="obj"></param>
        [PunRPC]
        public void RPC_SetParent(int[] obj)
        {
            foreach (var id in obj)
            {
                PhotonView.Find(id).gameObject.transform.SetParent(gameObject.transform);
            }
        }
    }
}