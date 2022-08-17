using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ChooseCatMapLiterals
{
    public static string Instructions = 

    "Add Target Content before exposition.\n" +  
    "Make sure content contains unambiguous category examples.\n" + 
    "By default, the program scrambles category order when choosing.\n";
    public static string TargetContent = "A few sentences or short paragraph.\n";
    public static string PrintChooseCatMapMessage = "Printing Choose Category Map!";
    public static string ChooseCatMapHeader = "Here's your category map!\n";
    public static string NoColorCatItem = "\n| | | | {0} | \n {1}"; // ignores the color aspect
    public static string CatItemExample = "| | {0} | |\n";
    public static string ChoosingMapHeader = "Here's your {0} content as a multiple choice activity!\n\n";
    public static string ContentBlockItem = " | </{0}/> | ";
    public static string ContentBlockItemChoice = "/ {0} /";
    
}
public class ChooseCatMap : Printer
{
    private string fullStringToPrint, categoryMapString, choosingMapString;

    public override string BuildInstructionString()
    {
        return "";
    }
    public override string BuildPreviewString()
    {
        return "";
    }

    public override void OnClickPrintButton()
    {
        categoryMapString = "";
        choosingMapString = "";

        if(CatManager.GetCatsCount() > 0)
        {

        }

    }
    private void printCategoryMap()
    {
        categoryMapString = string.Concat(
            categoryMapString,
            ChooseCatMapLiterals.ChooseCatMapHeader
        );

        foreach(Category cat in CatManager.GetCats())
        {
            var mapItemExamplesString = "";

            foreach(string example in cat.examples)
            {
                var str = string.Format(
                    ChooseCatMapLiterals.CatItemExample,
                    example
                );

                mapItemExamplesString = string.Concat(
                    mapItemExamplesString,
                    str
                );
            }

            var mapItem = string.Format(
                ChooseCatMapLiterals.NoColorCatItem,
                cat.name.ToUpperInvariant(),
                mapItemExamplesString
            );

            categoryMapString = string.Concat(
                categoryMapString,
                mapItem
            );

        }
    }

    private void printChoosingMap()
    {
        // takes the first category, 
        // assuming it contains a list of sentences in the examples
        Category cat = CatManager.GetCats()[0]; // todo: make this not hard-coded

        choosingMapString = string.Concat(
            choosingMapString,
            string.Format(
                ChooseCatMapLiterals.ChoosingMapHeader,
                cat.name.ToUpperInvariant()
            )
        );

        


        
    }


}
