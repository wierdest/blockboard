using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineOfflineConfirmationUI : MonoBehaviour
{
   public void OnClickAffirmativeButton()
   {
	
        int activeScene = SceneManager.GetActiveScene().buildIndex;
        Time.timeScale = 1.0f;
        if(activeScene == 1)
        {
            var runner = FindObjectOfType<NetworkRunner>();
            if(runner)
            {
                runner.Shutdown();
            }
            return;
        }
        
        // go to our network lobby scene
        SceneManager.LoadScene(1);
   }

   public void OnClickNegativeButton()
   {
		Time.timeScale = 1.0f;
        gameObject.SetActive(false);
   }
}
