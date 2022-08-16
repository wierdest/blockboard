using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ColorCatMapLiterals
{
    public static string Instructions = "Add appropriate content before exposition.\n" +  
    "It is worth exploring as many categories as the group can handle.\n" + "Don't shy away from asking for input\n";
    public static string TargetContent = "A few sentences or short paragraph\n";
    public static string PrintCategoryAndColoringMapMessage = "Printing Category & Coloring Map!";
    public static string CategoryMapHeader = "Here's your color | category map!\n";
    public static string ColoringMapHeader = "Here's your {0} content as word blocks for category-color-coding!\n\n";
    public static string MapItem = "\n| {0} | {1} | \n {2}";
    public static string MapItemExample = "| | {0} | |\n";
    public static string ColoringMapItem = " | {0} | ";
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

public class ColorCatMap : Printer
{
    private string fullStringToPrint, categoryMapString, coloringMapString;

    public override string BuildInstructionString()
    {
        return string.Format(
            PrinterLiterals.InstructionsTemplate,
            ColorCatMapLiterals.TargetContent,
            ColorCatMapLiterals.Instructions
        );
    }

    public override string BuildPreviewString()
    {
        return string.Concat(
            PrinterLiterals.PreviewHeader,
            ColorCatMapLiterals.CategoryMapHeader,
            string.Format(
                ColorCatMapLiterals.MapItem,
                "preview color",
                "preview category",
                string.Format(
                    ColorCatMapLiterals.MapItemExample,
                    "preview content example of preview category"
                )
            ),
            "\n\n",
            string.Format(
                ColorCatMapLiterals.ColoringMapHeader,
                "PREVIEW CATEGORY NAME"
            ),
            string.Format(
                ColorCatMapLiterals.ColoringMapItem,
                "preview content to color match"
            )
        );
    }

    

    override public void OnClickPrintButton()
    {
  
        categoryMapString = "";
        coloringMapString = "";

        if(CatManager.GetCatsCount() > 0)
        {
            Debug.Log(ColorCatMapLiterals.PrintCategoryAndColoringMapMessage);
            printCategoryMap();
            printColoringMap();                                                                                      

        }
        fullStringToPrint = "";
        fullStringToPrint = string.Concat(
            fullStringToPrint,
            categoryMapString,
            "\n\n\n",
            ColorCatMapLiterals.CreateLine(400),
            "\n\n\n",
            coloringMapString
        );
    
        fullStringToPrint.CopyMeToClipboard();
        Debug.Log(PrinterLiterals.TestMessage);
        
    }
    

    private void printColoringMap()
    {
        // takes the first category, 
        // assuming it contains a list of sentences in the examples
        var cat = CatManager.GetCats()[0];
        coloringMapString = string.Concat(
            coloringMapString,
            string.Format(
                ColorCatMapLiterals.ColoringMapHeader,
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
                    ColorCatMapLiterals.ColoringMapItem,
                    word
                );

                if(ColorCatMapLiterals.LineBreaks.Contains(word.Last<char>()))
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
            ColorCatMapLiterals.CategoryMapHeader
        );

        foreach(Category cat in CatManager.GetCats())
        {
            var mapItemExamplesString = "";
            foreach(string example in cat.examples)
            {
                var str = string.Format(
                    ColorCatMapLiterals.MapItemExample,
                    example
                );
                mapItemExamplesString = string.Concat(
                    mapItemExamplesString,
                    str
                );
            }

            var mapItem = string.Format(
                ColorCatMapLiterals.MapItem, 
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
