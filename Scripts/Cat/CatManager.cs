using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CatManager : MonoBehaviour
{
    [SerializeField] private List<Category> regular;
    [SerializeField] private List<Category> corpora;
    private int lastRegularCatCount, lastCorporaCount;
    // cat status monitor
    [SerializeField] private TMPro.TMP_Text regularCount, corporaCount;
    
    private void Awake()
    {
        regular = new List<Category>();
        corpora = new List<Category>();
    }

    public void AddCategory(Category newCat)
    {
        if(newCat == null)
        {
            Debug.Log("Cat Manager: I can't add a null cat! Sorry!");
            return;
        }

        // check if we already have a cat by that name
        var sameCatIndex = regular.FindIndex(0, regular.Count, c => c.Name.Equals(newCat.Name));

        // if that's the case, we automatically update its color and its examples.
        if(sameCatIndex >= 0)
        {
            Debug.Log("Cat Manager: I can't add the same cat name twice! However, I will update examples and color!");   
            var same = regular[sameCatIndex];
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

            // send a regular to corpus if that's the case,
            /// IS THIS REALLY DESIRABLE?
            // if(!same.IsCorpus && newCat.IsCorpus)
            // {
            //     corpora.Add(newCat);
            // }
            return;
        }

        if(newCat.IsCorpus)
        {
            // if it's a corpus cat, we add to the corpora list
            Debug.Log("Added cat to corpora!");
            corpora.Add(newCat);
            lastCorporaCount++;
        }
        else
        {
            // iof it's a regular cat we add to the regular list
            Debug.Log("Added cat to regular!");
            regular.Add(newCat);
            lastRegularCatCount++;
        }
        updateStatusMonitor();
        
        // Debug.LogFormat("Cat Manager: added cat {0} exemplified by {1}", newCat.Name, newCat.Examples);
        
    }

    public void RemoveCategory(Category cat)
    {
        if(cat == null)
        {
            Debug.Log("Cat Manager: I can't remove a null cat! Sorry!");
            return;
        }

        if(cat.IsCorpus)
        {
            if(corpora.RemoveAll(c => cat.Equals(cat)) != 0)
            {
                Debug.LogFormat("Cat Manager: removed a corpus cat! we used to have {0} corpora cat, now there's {1}!", lastCorporaCount, corpora.Count);
                lastCorporaCount = corpora.Count;
            }
        }
       
        if(regular.RemoveAll(c => c.Equals(cat)) != 0)
        {
            Debug.LogFormat("Cat Manager: removed a regular cat! we used to have {0} regular cat, now there's {1}!", lastRegularCatCount, regular.Count);
            lastRegularCatCount = regular.Count;
        }
        updateStatusMonitor();
    }

    public void ClearCats()
    {
        regular.Clear();
        corpora.Clear();
        updateStatusMonitor();
    }

    public int GetRegularCatsCount()
    {
        return regular.Count;
    }

    public int GetCorporaCatsCount()
    {
        return corpora.Count;
    }

    public List<Category> GetRegularCats()
    {
        return regular;
    }

    public List<Category> GetCorporaCats()
    {
        return corpora;
    }

    private void updateStatusMonitor()
    {
        regularCount.text = lastRegularCatCount.ToString();
        corporaCount.text = lastCorporaCount.ToString();
    }
}
  
