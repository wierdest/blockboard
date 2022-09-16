using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ReaderUI : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text pageRef, header, pageCount;
    [SerializeField] private GameObject contentHolder, readerLine, lineItem, actionButton;
    [SerializeField] private Button lastButton, nextButton;


    public void SetUpLastButtonOnClick(UnityAction action)
    {
        lastButton.onClick.AddListener(action);
    }

    public void SetUpNextButtonOnClick(UnityAction action)
    {
        nextButton.onClick.AddListener(action);
    }

}
