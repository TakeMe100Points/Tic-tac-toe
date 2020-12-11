using UnityEngine;
using Photon.Pun;
using PN = Photon.Pun.PhotonNetwork;
using Photon.Realtime;


public class GridButtons : MonoBehaviourPunCallbacks
{
    //Можно хранить тут идентификатор игрока
    [SerializeField] private GameObject cellPrefab;

    [Header("Size grid [x][x]")][SerializeField] private int gridSize = 3;

    private CellOfGrid[,] _grid;


    private void Start()
    {
        if(PN.IsMasterClient)
        {
            _grid = new CellOfGrid[gridSize, gridSize];

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    _grid[i, j] = PN.Instantiate(cellPrefab.name, transform.position, Quaternion.identity).GetComponent<CellOfGrid>();
                    _grid[i, j].transform.SetParent(gameObject.transform);
                    //_grid[i, j].onCellClicked += OnCellClicked;
                }
            }
        }
        else
        {
            GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");
            Debug.Log("Setted " + cells.Length);
            for (int i = 0; i < cells.Length; i++)
                cells[i].transform.SetParent(gameObject.transform);
        }
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        //photonView.RPC("RPC_SetParent", RpcTarget.Others);
    }


    [PunRPC]
    public void RPC_SetParent()
    {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Cell");

        for (int i = 0; i < cells.Length; i++)
            cells[i].transform.SetParent(gameObject.transform);
    }

    public void OnCellClicked(CellOfGrid cell)
    {
        
    }
}
