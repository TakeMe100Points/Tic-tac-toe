using UnityEngine;
using Photon.Pun;

namespace Assets.Scripts.Service
{
    /// <summary>
    /// Управление стартовым баннером
    /// </summary>
    public class StartGame : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// Окно ожидания клиента
        /// </summary>
        [SerializeField] private GameObject startBanner;

        public void HideStartBanner()
        {
            startBanner.SetActive(false);
        }

        public void ShowStartBanner()
        {
            startBanner.SetActive(true);
        }
    }
}