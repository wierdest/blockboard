using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastButton : MonoBehaviour
{
    [SerializeField] private GameObject confirmationInterface;

    public void OnClick()
    {
        Time.timeScale = 0.0f;
        confirmationInterface.SetActive(true);
    }
}
