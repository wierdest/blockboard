using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// when we add photon fusion support this becomes an INetworkRunnerCallbacks
public class NetworkManager : MonoBehaviour
{
    // manages the connection between user interface and network options
    [SerializeField] private GameObject networkInterface;

    public void OnClickNetButton()
    {
        if(networkInterface.activeInHierarchy)
        {
            networkInterface.SetActive(false);
            // resume 
            Time.timeScale = 1.0f;
            return;
        }
        
        // pause the game
        Time.timeScale = 0.0f;
        networkInterface.SetActive(true);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
