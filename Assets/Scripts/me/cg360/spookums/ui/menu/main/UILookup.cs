using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UILookup : MonoBehaviour
{
    public List<ElementEntry> Elements = new List<ElementEntry>();
    public Dictionary<string, GameObject> ElementLookup = new Dictionary<string, GameObject>();
    

    public virtual void Awake()
    {
        ElementLookup = new Dictionary<string, GameObject>();
        foreach (ElementEntry p in Elements)
        {
            ElementLookup.Add(p.id, p.panel);
        }
        
    }


    [Serializable]
    public class ElementEntry {
        public string id;
        public GameObject panel;
    }
}
