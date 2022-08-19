using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearInputFieldButton : MonoBehaviour
{
   [SerializeField] private TMPro.TMP_InputField inputField;

   public void OnClick()
   {
        inputField.text = "";
   }
}
