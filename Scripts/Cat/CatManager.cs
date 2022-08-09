using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    [SerializeField] private List<Category> cats;
    
    private void Awake()
    {
        cats = new List<Category>();
    }

    public void AddCategory(Category newCat)
    {
        if(newCat == null)
        {
            Debug.Log("Cat Manager: I can't add a null cat! Sorry!");
            return;
        }

        // check if we already have a cat by that name
        var sameCatIndex = cats.FindIndex(0, cats.Count, c => c.name.Equals(newCat.name));

        // if that's the case, we automatically update its color and its examples.
        if(sameCatIndex >= 0)
        {
            Debug.Log("Cat Manager: I can't add the same cat name twice! However, I will update examples and color!");   
            
            // update examples
            foreach(string example in newCat.examples)
            {
                if(!cats[sameCatIndex].examples.Contains(example))
                {
                    cats[sameCatIndex].examples.Add(example);
                }
            }

            // update color
            cats[sameCatIndex].color = newCat.color;
            return;

        }
        cats.Add(newCat);
        Debug.LogFormat("Cat Manager: added cat {0} exemplified by {1}", newCat.name, newCat.examples);
        
    }

    public void RemoveCategory(Category cat)
    {
        if(cat == null)
        {
            Debug.Log("Cat Manager: I can't remove a null cat! Sorry!");
        }
        var lastCatCount = cats.Count;
        if(cats.Remove(cat))
        {
            Debug.LogFormat("Cat Manager: removed a cat! we used to have {0} cat, now there's {1}!", lastCatCount, cats.Count);
        }
    }


    public int GetCatsCount()
    {
        return cats.Count;
    }

    public List<Category> GetCats()
    {
        return cats;
    }

}
  
