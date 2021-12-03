using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelController : UILookup
{
    public string DefaultPanel = "connect_panel";
    public string CurrentPanel { get; private set; }
    
    public override void Start()
    {
        CurrentPanel = DefaultPanel;
        foreach (ElementEntry p in Elements)
        {
            ElementLookup.Add(p.id, p.panel);
            if (p.id == CurrentPanel)
            {
                p.panel.SetActive(true);
            }
            else
            {
                p.panel.SetActive(false);
            }
        }
        
    }

    public void SwitchPanel(string newPanel)
    {
        if (ElementLookup.ContainsKey(newPanel))
        {
            ElementLookup[CurrentPanel].SetActive(false);
            CurrentPanel = newPanel;
            ElementLookup[CurrentPanel].SetActive(true);
        } 
    }
    
}
