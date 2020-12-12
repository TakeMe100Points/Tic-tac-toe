using Assets.Scripts.Service;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using PN = Photon.Pun.PhotonNetwork;

namespace Assets.Scripts.GameParts
{
    /// <summary>
    /// Класс кнопки - ячейки отправляет сигналы нажатия, а получает индекс иконки
    /// Для установки на спрайт
    /// </summary>
    public class CellOfGrid : MonoBehaviourPun
    {
        #region Serialized Fields
        /// <summary>
        /// Текущий спрайт (По умолчанию стоит прозрачная картинка)
        /// </summary>
        [SerializeField] private Image currentIcon;

        /// <summary>
        /// Иконка крестика
        /// </summary>
        [SerializeField] private Sprite chestIcon;

        /// <summary>
        /// Иконка нолика
        /// </summary>
        [SerializeField] private Sprite circleIcon;
        #endregion

        /// <summary>
        /// Было ли назначенно значение этой кнопке
        /// </summary>
        private bool isMarked = false;

        /// <summary>
        /// Значение кнопки для формирования массива значений в GridButtons
        /// Который необходим для определения победителя.
        /// </summary>
        public int value = 0;

        /// <summary>
        /// Вызывается при нажатии на кнопку
        /// </summary>
        public void OnCellClicked()
        {
            // Формирование отправления события нажатия на кнопку
            //                              Номер игрока                ViewId Ячейки       Имеет ли уже значение
            object[] datas = new object[] { PN.LocalPlayer.ActorNumber, photonView.ViewID, isMarked };

            //Установление настроек события
            //                                                      Получатели - Все                Не кэшировать события
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All, CachingOption = EventCaching.DoNotCache };

            //Отправить событие
            PN.RaiseEvent(RPC_codes.CHANGEMARK, datas, options, ExitGames.Client.Photon.SendOptions.SendReliable);
        }

        /// <summary>
        /// Смена значка на ...
        /// </summary>
        /// <param name="icon">Индекс нового значка</param>
        public void ChangeIcon(int icon)
        {
            if (icon == 1)
            {
                currentIcon.sprite = chestIcon;
                value++;
            }
            else
            {
                currentIcon.sprite = circleIcon;
                value--;
            }

            isMarked = true;
        }
    }
}