using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] private GameObject camInButton, camOutButton;
  
    public void SetCameraControls(bool state)
    {
        camInButton.SetActive(state);
        camOutButton.SetActive(state);
    }
}
