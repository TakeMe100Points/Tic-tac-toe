using Photon.Pun;
using UnityEngine;
using TMPro;

namespace Assets.Scripts.Service
{
    /// <summary>
    /// Экран с победившим игроком
    /// </summary>
    public class WinMenu : MonoBehaviourPun
    {
        /// <summary>
        /// Баннер победы, необходим для отображения/скрытия
        /// </summary>
        [SerializeField] GameObject winBanner;

        /// <summary>
        /// Текст для результатов
        /// </summary>
        [SerializeField] TextMeshProUGUI winText;

        /// <summary>
        /// Кнопка рестарта
        /// </summary>
        [SerializeField] GameObject replayButton;

        /// <summary>
        /// Убрать на старте экран выйгрыша
        /// </summary>
        private void Start()
        {
            winBanner.SetActive(false);
        }

        /// <summary>
        /// Отображение победного банера
        /// </summary>
        /// <param name="nickname">Победивший игрок</param>
        [PunRPC]
        public void ShowWinBanner(string nickname)
        {
            //Убрать кнопку перезапуска для клиента
            if (!PhotonNetwork.IsMasterClient)
                replayButton.SetActive(false);

            //Отобразить и вывести победившего игрока
            winText.SetText(nickname + " Winner!");
            winBanner.SetActive(true);
        }

        /// <summary>
        /// Убрать баннер
        /// </summary>
        [PunRPC]
        public void HideWinBanner()
        {
            winBanner.SetActive(false);
        }
    }
}