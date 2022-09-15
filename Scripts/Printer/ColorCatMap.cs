using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ColorCatMapLiterals
{
    public static string Instructions = 
    
    "Add Target Content after exposition.\n" +  
    "It is worth exploring as many categories as the group can handle.\n" + 
    "Build mnemonic bridge between Color and Category.\n";
    public static string TargetContent = "A few sentences or short paragraph\n";
    public static string PrintCategoryAndColoringMapMessage = "Printing Category & Coloring Map!";
    public static string ColorCatMapHeader = "Here's your color | category map!\n";
    public static string ColoringMapHeader = "Here's your {0} content as word blocks for category-color-coding!\n\n";
    public static string CatItem = "\n| {0} | {1} | \n {2}";
    public static string CatItemExample = "| | {0} | |\n";
    public static string ContentBlockItem = " | {0} | ";
   
  
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
            ColorCatMapLiterals.ColorCatMapHeader,
            string.Format(
                ColorCatMapLiterals.CatItem,
                "preview color",
                "preview category",
                string.Format(
                    ColorCatMapLiterals.CatItemExample,
                    "preview content example of preview category"
                )
            ),
            "\n\n",
            string.Format(
                ColorCatMapLiterals.ColoringMapHeader,
                "PREVIEW CATEGORY NAME"
            ),
            string.Format(
                ColorCatMapLiterals.ContentBlockItem,
                "preview content to color match"
            )
        );
    }

    
    public override void OnClickPrintButton()
    {
  
        categoryMapString = "";
        coloringMapString = "";

        if(CatManager.GetRegularCatsCount() > 0 && CatManager.GetCorporaCatsCount() > 0)
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
            PrinterLiterals.CreateLine(400),
            "\n\n\n",
            coloringMapString
        );
    
        fullStringToPrint.CopyMeToClipboard();
        Debug.Log(PrinterLiterals.TestMessage);
        
    }
    
    private void printColoringMap()
    {
        // takes the last category, 
        // assuming it contains a list of sentences in the examples

        foreach(Category cat in CatManager.GetCorporaCats())
        {
            coloringMapString = string.Concat(
                coloringMapString,
                string.Format(
                    ColorCatMapLiterals.ColoringMapHeader,
                    cat.Name.ToUpperInvariant()
                ),
                "\n"
            );

            foreach(string example in cat.Examples)
            {
                var split = example.Split(" ");
                var splitString = "";
                foreach(string word in split)
                {
                    var wordMapItemToColor = string.Format(
                        ColorCatMapLiterals.ContentBlockItem,
                        word
                    );

                    if(PrinterLiterals.LineBreaks.Contains(word.Last<char>()))
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
    }

    private void printCategoryMap()
    {
        categoryMapString = string.Concat(
            categoryMapString,
            ColorCatMapLiterals.ColorCatMapHeader
        );

        foreach(Category cat in CatManager.GetRegularCats())
        {
            var mapItemExamplesString = "";
            foreach(string example in cat.Examples)
            {
                var str = string.Format(
                    ColorCatMapLiterals.CatItemExample,
                    example
                );
                mapItemExamplesString = string.Concat(
                    mapItemExamplesString,
                    str
                );
            }

            var mapItem = string.Format(
                ColorCatMapLiterals.CatItem, 
                ColorNameConverter.FindColor(cat.CatColor),
                cat.Name.ToUpperInvariant(),
                mapItemExamplesString
            );

            categoryMapString = string.Concat(
                categoryMapString,
                mapItem
            );


        }
        
    }

}
