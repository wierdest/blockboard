using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    [SerializeField] private List<Category> cats;
    [SerializeField] private List<Category> corpora;
    private int lastCatCount;
    // monitoring status
    [SerializeField] private TMPro.TMP_Text statusText;
    private readonly string emptyStatus = "nothing to print yet!";
    private readonly string statusTemplate = "{0}; ";
    private readonly string fullStatusTemplate = "to print: {0} and {1} more!";
    private int maxStatusLength;
    
    private void Awake()
    {
        cats = new List<Category>();
        maxStatusLength = statusText.text.Length;
        statusText.text = emptyStatus;
    }

    public void AddCategory(Category newCat)
    {
        if(newCat == null)
        {
            Debug.Log("Cat Manager: I can't add a null cat! Sorry!");
            return;
        }

        // check if we already have a cat by that name
        var sameCatIndex = cats.FindIndex(0, cats.Count, c => c.Name.Equals(newCat.Name));

        // if that's the case, we automatically update its color and its examples.
        if(sameCatIndex >= 0)
        {
            Debug.Log("Cat Manager: I can't add the same cat name twice! However, I will update examples and color!");   
            var same = cats[sameCatIndex];
            // update examples
            foreach(string example in newCat.Examples)
            {
                if(!same.Examples.Contains(example))
                {
                    same.Examples.Add(example);
                }
            }

            // update color
            
            same.CatColor = newCat.CatColor;

            // update to corpus if that's the case,
            if(!same.IsCorpus && newCat.IsCorpus)
            {
                corpora.Add(newCat);
            }
            return;

        }

        if(newCat.IsCorpus)
        {
            Debug.Log("Added cat to corpora!");
            corpora.Add(newCat);
        }
        else
        {
            Debug.Log("Added cat to cats!");
            cats.Add(newCat);
        }

        
        updateStatusMonitor();
        lastCatCount++;
        // Debug.LogFormat("Cat Manager: added cat {0} exemplified by {1}", newCat.Name, newCat.Examples);
        
    }

    public void RemoveCategory(Category cat)
    {
        if(cat == null)
        {
            Debug.Log("Cat Manager: I can't remove a null cat! Sorry!");
            return;
        }

        if(cats.RemoveAll(c => c.Equals(cat)) != 0)
        {
            Debug.LogFormat("Cat Manager: removed a cat! we used to have {0} cat, now there's {1}!", lastCatCount, cats.Count);
            updateStatusMonitor();
        }
        lastCatCount = cats.Count;
        
    }

    public void ClearCats()
    {
        cats.Clear();
        updateStatusMonitor();
    }

    public int GetCatsCount()
    {
        return cats.Count;
    }

    public List<Category> GetCats()
    {
        return cats;
    }

    private void updateStatusMonitor()
    {
        if(cats.Count == 0)
        {
            statusText.text = emptyStatus;
            return;
        }

        if(statusText.text.Length >= maxStatusLength)
        {
            statusText.text = string.Format(
                fullStatusTemplate,
                cats[0].Name,
                cats.Count + 1
            );
        }
        else
        {
            statusText.text = "to print: ";
            foreach(Category cat in cats)
            {
                statusText.text += string.Concat(
                    string.Format(
                        statusTemplate,
                        cat.Name
                    )
                ); 
            }
            
        }
    }

   

}
  
