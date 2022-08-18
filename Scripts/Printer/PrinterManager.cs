using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PrinterManager : MonoBehaviour
{
    [SerializeField] private List<Printer> printers;
    [SerializeField] private Printer currentPrinter;
    [SerializeField] private Button printButton;
    [SerializeField] private GameObject settingsInterface;
    [SerializeField] private TMPro.TMP_Dropdown dropdown;
    [SerializeField] private TMPro.TMP_Text previewText, instructionsText;
    
    private void Awake()
    {
        printers = GetComponentsInChildren<Printer>().ToList();

        if(printers.Count == 0)
        {
            Debug.Log("Printer Manager found no printers! How come?");
            return;
        }

        Debug.LogFormat("Printer Manager has found {0} printers. {1} is current.", printers.Count, printers[0].name);
        
        // set current printer and its appropriate 
        currentPrinter = printers[0];
        updatePrintButtonOnClick();

        // set appropriate text
        updatePrinterInfoTexts();

        // populate dropdown with found options
        updateDropdownOptions();

    }

    public void OnDropdownValueChanged()
    {
        int value = dropdown.value;
        Printer choice = printers.First<Printer>(p => p.name.Equals(dropdown.options[value].text));
        if(choice.Equals(currentPrinter))
        {
            Debug.Log("Printer Manager: you've already chosen this one!");
            return;
        }

        currentPrinter = choice;
        updatePrintButtonOnClick();
        updatePrinterInfoTexts();
       
    }

    public void OnClickOKButton()
    {
        // resume
        Time.timeScale = 1.0f;
        settingsInterface.SetActive(false);

    }

    public void OnClickSettingsButton()
    {
        // pause 
        Time.timeScale = 0.0f;
        // activate printer settings interface
        settingsInterface.SetActive(true);
        updatePrinterInfoTexts();
    }

    private void updatePrinterInfoTexts()
    {
        instructionsText.text = currentPrinter.Instructions;
        previewText.text = currentPrinter.Preview;
    }

    private void updatePrintButtonOnClick()
    {
        // sets the click listener programmatically 
        printButton.onClick.AddListener(currentPrinter.OnClickPrintButton);
        Debug.Log("Tried updating printButton on click!");
    }

    private void updateDropdownOptions()
    {
        dropdown.ClearOptions();
        List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();  
        foreach(Printer p in printers)
        {
            TMPro.TMP_Dropdown.OptionData option = new  TMPro.TMP_Dropdown.OptionData();
            option.text = p.name;
            options.Add(option);
        }
        dropdown.AddOptions(options);

    }

}
