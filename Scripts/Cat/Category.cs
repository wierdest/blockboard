using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Category
{
    public string Name;
    public bool IsCorpus;
    public SerializableColor CatColor;
    public List<string> Examples;
}
