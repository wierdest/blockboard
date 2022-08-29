using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmationInterface : MonoBehaviour
{

   public void OnClickSureButton()
   {
        int activeScene = SceneManager.GetActiveScene().buildIndex;
        if(activeScene == 1)
        {
            // go back to our first scene
            SceneManager.LoadScene(0);
            return;
        }
        Time.timeScale = 1.0f;
        // go to our network lobby scene
        SceneManager.LoadScene(1);
   }

   public void OnClickNopeButton()
   {
		Time.timeScale = 1.0f;
        gameObject.SetActive(false);
   }
}
