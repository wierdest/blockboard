using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class ChooseCatMapLiterals
{
    public static string Instructions = 
    "Make sure content contains unambiguous category examples.\n" + 
    "By default, the program scrambles category order when choosing.\n";
    public static string TargetContent = "A few sentences or short paragraph.\n";
    public static string PrintChooseCatMapMessage = "Printing Choose Category Map!";
    public static string ChooseCatMapHeader = "Here's your category map!\n";
    public static string NoColorCatItem = "\n| | | | {0} | \n {1}"; // ignores the color aspect
    public static string CatItemExample = "| | {0} | |\n";
    public static string ChoosingMapHeader = "Here's your {0} content as a multiple choice activity!\n\n";
    public static string NormalBlockItem = " | {0} | ";
    public static string ChooseContentBlockItem = " | </{0}/> | ";
    public static string ContentBlockItemChoice = "/ {0} /";

    
}
public class ChooseCatMapString : Printer
{
    private string fullStringToPrint, categoryMapString, choosingMapString;

    public override string BuildInstructionString()
    {
        return string.Format(
            PrinterLiterals.InstructionsTemplate,
            ChooseCatMapLiterals.TargetContent,
            ChooseCatMapLiterals.Instructions
        );
    }
    public override string BuildPreviewString()
    {
        return string.Concat(
            PrinterLiterals.PreviewHeader,
            ChooseCatMapLiterals.ChooseCatMapHeader,
            string.Format(
                ChooseCatMapLiterals.NoColorCatItem,
                "preview category",
                string.Format(
                    ChooseCatMapLiterals.CatItemExample,
                    "preview example of preview category"
                )
            ),
            "\n\n",

            string.Format(
                ChooseCatMapLiterals.ChoosingMapHeader,
                "PREVIEW CATEGORY NAME"
            ),
            string.Concat(

                string.Format(
                    ChooseCatMapLiterals.NormalBlockItem,
                    "I"
                ),
                string.Format(
                    ChooseCatMapLiterals.ChooseContentBlockItem,
                    string.Concat(
                        string.Format(ChooseCatMapLiterals.ContentBlockItemChoice, "is"),
                        string.Format(ChooseCatMapLiterals.ContentBlockItemChoice, "am"),
                        string.Format(ChooseCatMapLiterals.ContentBlockItemChoice, "are")
                    )

                ),
                string.Format(
                    ChooseCatMapLiterals.NormalBlockItem,
                    "a multiple choice preview"
                )
            )
        );
    }

    public override void OnClickPrintButton()
    {
        categoryMapString = "";
        choosingMapString = "";

        if(CatManager.GetRegularCatsCount() > 0 && CatManager.GetCorporaCatsCount() > 0)
        {
            Debug.Log(ChooseCatMapLiterals.PrintChooseCatMapMessage);
            printCategoryMap();
            printChoosingMap();

        }

        fullStringToPrint = "";
        fullStringToPrint = string.Concat(
            fullStringToPrint,
            categoryMapString,
            "\n\n\n",
            PrinterLiterals.CreateLine(400),
            "\n\n\n",
            choosingMapString
        );
    
        fullStringToPrint.CopyMeToClipboard();
        Debug.Log(PrinterLiterals.TestMessage);

    }
    private void printCategoryMap()
    {
        categoryMapString = string.Concat(
            categoryMapString,
            ChooseCatMapLiterals.ChooseCatMapHeader
        );

        foreach(Category cat in CatManager.GetRegularCats())
        {
            var mapItemExamplesString = "";

            foreach(string example in cat.Examples)
            {
                var itemExample = string.Format(
                    ChooseCatMapLiterals.CatItemExample,
                    example
                );

                mapItemExamplesString = string.Concat(
                    mapItemExamplesString,
                    itemExample
                );
            }
            
            var mapItem = string.Format(
                ChooseCatMapLiterals.NoColorCatItem,
                cat.Name.ToUpperInvariant(),
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
        foreach(Category cat in CatManager.GetCorporaCats())
        {
            choosingMapString = string.Concat(
                choosingMapString,
                string.Format(
                    ChooseCatMapLiterals.ChoosingMapHeader,
                    cat.Name.ToUpperInvariant()
                )
            );

            foreach(string example in cat.Examples)
            {
                var split  = example.Split(" ");
                foreach(string block in split)
                {
                    string blockString = "";
                    IEnumerable<Category> testArray = CatManager.GetRegularCats().Where(c => c.Examples.Contains<string>(block));
                    if(testArray.Count() != 0)
                    {
                        // if the word in the example is a category
                        Category catToChoose = testArray.First(); // we take the first we find
                        // get the options we have to choose from
                        List<string> optionsToChoose = catToChoose.Examples;
                        // randomly shuffles them
                        optionsToChoose.Shuffle();
                        string optionString = "";
                        foreach(string option in optionsToChoose)
                        {
                            optionString = string.Concat(
                                optionString,
                                string.Format(
                                    ChooseCatMapLiterals.ContentBlockItemChoice,
                                    option
                                )
                            );
                        }

                        blockString = string.Format(
                            ChooseCatMapLiterals.ChooseContentBlockItem,
                            optionString
                        );
                    }
                    else
                    {
                        // normal word
                        blockString = string.Format(
                            ChooseCatMapLiterals.NormalBlockItem,
                            block
                        );
                    }

                    // this will come up sometime: the category

                    if(PrinterLiterals.LineBreaks.Contains(block.Last<char>()))
                    {
                        blockString = string.Concat(
                            blockString,
                            "\n\n"
                        );
                    }
                    
                    choosingMapString = string.Concat(
                        choosingMapString,
                        blockString
                    );
                }
            }
        }
    }


}
