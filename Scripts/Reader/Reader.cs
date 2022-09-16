using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Reader : MonoBehaviour
{
    public CatManager CatManager;
    public ReaderUI ReaderUI;
    public abstract void OnClickReadButton();
    public abstract void OnClickLastButton();
    public abstract void OnClickNextButton();
    public abstract void OnClickOptionButton();
    public abstract void BuildHeaderText();
    public abstract void BuildPageReferenceText();
   
}
