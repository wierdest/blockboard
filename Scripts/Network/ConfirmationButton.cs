using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmationButton : MonoBehaviour
{
    [SerializeField] private GameObject confirmationUI;

    public void OnClick()
    {
        Time.timeScale = 0.0f;
        confirmationUI.SetActive(true);
    }

    public void SetConfirmationUI(GameObject userInterface)
    {
        confirmationUI = userInterface;

    }
}
