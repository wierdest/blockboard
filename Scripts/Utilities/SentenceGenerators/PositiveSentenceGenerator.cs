using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// todo: add more literals in different themes / structures
// make the generator go through them randomly, etc...

public static class PositiveSentenceLiterals
{ 
    public static int Count = 1;
	public static List<string> SelfAffirmation = new List<string>() 
	{ 
        "I am great!", "I can do it!", "I am enough!", "I am courageous!", "I am strong!","I am cute!", "I am optimistic!", 
        "I believe in myself!", "I am grateful!", "I approve of myself!", "I love myself fully!", "I have the power to change!", 
        "My future is bright!", "I am free to create the life I desire!", "I am in charge!", "I am focused!", "I am worthy!", 
        "I love myself!", "I love my life", "I have lots of great ideas!", "I am full of potential!", "I can ask for help if I need to"
	
	};
	
}

public class PositiveSentenceGenerator : MonoBehaviour
{
    // feeds input field with a sentence  with content from a literals collection
    // picked at random 
   [SerializeField] private TMPro.TMP_InputField inputField;


   public void OnClickGenerateButton()
   {
        inputField.text = pickSelfAffirmation();
   }

   private string pickSelfAffirmation()
   {
        int count = PositiveSentenceLiterals.SelfAffirmation.Count;
        int random = Random.Range(0, count);

        return PositiveSentenceLiterals.SelfAffirmation[random];

   }
}
