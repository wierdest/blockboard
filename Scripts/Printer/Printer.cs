using UnityEngine;


public static class PrinterLiterals
{
    public static string TestMessage = " | | | Printer working fine, boss! | | | ";
    public static string InstructionsTemplate = "Target content:\n\n{0}\nInstructions:\n\n{1}\n";
    public static string PreviewHeader = "Preview:\n\n";

}

public abstract class Printer : MonoBehaviour
{
    public CatManager CatManager;
    public string Preview, Instructions;
    public abstract void OnClickPrintButton();
    public abstract string BuildInstructionString();
    public abstract string BuildPreviewString();

    private void Awake()
    {
        Instructions = BuildInstructionString();
        Preview = BuildPreviewString();
        Debug.LogFormat("Printer: {0} created instructions string {1} and preview string {2}", name, Instructions, Preview);
    }

    public void PrintTestMessage()
    {
        PrinterLiterals.TestMessage.CopyMeToClipboard();
        Debug.Log(PrinterLiterals.TestMessage);
    }
   
}
