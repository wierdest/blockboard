using System.Collections.Generic;
using UnityEngine;


public static class PrinterLiterals
{
    public static string TestMessage = " | | | Printer working fine, boss! | | | ";
    public static string InstructionsTemplate = "Target content:\n\n{0}\nInstructions:\n\n{1}\n";
    public static string PreviewHeader = "Preview:\n\n";
    public static List<char> LineBreaks = new List<char>() { '.', '?', '!'};
    public static string CreateLine(int length)
    {
        string line = "";
        for(int i = 0; i < length; i++)
        {
            line += "_";
        }
        return line;
    }

}

public abstract class Printer : MonoBehaviour
{
    public CatManager CatManager;
    public string Preview, Instructions;
    public abstract void OnClickPrintButton();
    public abstract string BuildInstructionString();
    public abstract string BuildPreviewString();

    private void Start()
    {
        Instructions = BuildInstructionString();
        Preview = BuildPreviewString();
        // Debug.LogFormat("Printer: {0} Start with Instructions {1}", name, Instructions);
    }

    public void PrintTestMessage()
    {
        PrinterLiterals.TestMessage.CopyMeToClipboard();
        Debug.Log(PrinterLiterals.TestMessage);
    }
   
}
