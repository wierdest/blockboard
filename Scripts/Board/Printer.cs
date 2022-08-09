using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// this is a prototypical printer program
public static class Literals
{
    public static string TestMessage = " | | | Printer working fine, boss! | | | ";
    public static string PrintCategoryAndColoringMapMessage = "Printing Category & Coloring Map!";
    public static string CategoryMapHeader = "Here's your color | category map!\n";
    public static string ColoringMapHeader = "Here's your {0} content as word blocks for category-color-coding!\n\n";
    public static string MapItem = "\n| {0} | {1} | \n {2}";
    public static string MapItemExample = "| | {0} | |\n";
    public static string ColoringMapItem = " | {0} | ";

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
public class Printer : MonoBehaviour
{
    [SerializeField] private CatManager catManager;
    private string fullStringToPrint, categoryMapString, coloringMapString;
    private readonly List<char> lineBreaks = new List<char>() { '.', ',', '?', ':', ';', '!'};

    public void OnClickPrintButton()
    {
        // printTestMessage();
        categoryMapString = "";
        coloringMapString = "";
        if(catManager.GetCatsCount() > 0)
        {
            Debug.Log(Literals.PrintCategoryAndColoringMapMessage);
            printCategoryMap();
            printColoringMap();                                                                                      

        }
        fullStringToPrint = "";
        fullStringToPrint = string.Concat(
            fullStringToPrint,
            categoryMapString,
            "\n\n\n",
            Literals.CreateLine(400),
            "\n\n\n",
            coloringMapString
        );

        fullStringToPrint.CopyMeToClipboard();
        Debug.Log(Literals.TestMessage);
    }
    private void printTestMessage()
    {
        Literals.TestMessage.CopyMeToClipboard();
        Debug.Log(Literals.TestMessage);
    }

    private void printColoringMap()
    {
        // takes the first category, 
        // assuming it contains a list of sentences in the examples
        var cat = catManager.GetCats()[0];
        coloringMapString = string.Concat(
            coloringMapString,
            string.Format(
                Literals.ColoringMapHeader,
                cat.name.ToUpperInvariant()
            )
        );

        foreach(string example in cat.examples)
        {
            var split = example.Split(" ");
            var splitString = "";
            foreach(string word in split)
            {
                var wordMapItemToColor = string.Format(
                    Literals.ColoringMapItem,
                    word
                );

                if(lineBreaks.Contains(word.Last<char>()))
                {
                    wordMapItemToColor = string.Concat(wordMapItemToColor, "\n\n");
                }

                splitString = string.Concat(
                    splitString,
                    wordMapItemToColor
                );
                
            }

            coloringMapString = string.Concat(
                coloringMapString,
                splitString
            );
        }
    }

    private void printCategoryMap()
    {
        categoryMapString = string.Concat(
            categoryMapString,
            Literals.CategoryMapHeader
        );

        foreach(Category cat in catManager.GetCats())
        {
            var mapItemExamplesString = "";
            foreach(string example in cat.examples)
            {
                var str = string.Format(
                    Literals.MapItemExample,
                    example
                );
                mapItemExamplesString = string.Concat(
                    mapItemExamplesString,
                    str
                );
            }

            var mapItem = string.Format(
                Literals.MapItem, 
                ColorNameConverter.FindColor(cat.color),
                cat.name.ToUpperInvariant(),
                mapItemExamplesString
            );

            categoryMapString = string.Concat(
                categoryMapString,
                mapItem
            );


        }
        
    }




}
