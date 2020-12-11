using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] GameObject startBanner;

    void Start()
    {
        //startBanner.SetActive(true);
    }

    
    public void LetsStart()
    {
        startBanner.SetActive(false);
    }
    
    public void StopGame()
    {
        startBanner.SetActive(true);
    }
}
