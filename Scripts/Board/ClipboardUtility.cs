using System.Runtime.InteropServices;
using UnityEngine;

// this is a great example of a string extension class
public static class ClipboardCopy
{
    // Using a JavaScript plugin 
    [DllImport("__Internal")]
    public static extern void CopyToClipboard(string str); // notice: this!

    public static void CopyMeToClipboard(this string str)
    {
        #if UNITY_WEBGL && UNITY_EDITOR == false
            CopyToClipboard(str);
        #else
            GUIUtility.systemCopyBuffer = str;
        #endif
    }

}
