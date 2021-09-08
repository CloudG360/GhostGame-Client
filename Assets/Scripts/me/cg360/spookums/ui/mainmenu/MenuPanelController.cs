using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelController : MonoBehaviour
{
    public List<PanelEntry> RegisteredPanels = new List<PanelEntry>();
    public Dictionary<string, GameObject> PanelLookup = new Dictionary<string, GameObject>();

    public string DefaultPanel = "connect_panel";
    public string CurrentPanel { get; private set; }
    
    void Start()
    {
        CurrentPanel = DefaultPanel;
        foreach (PanelEntry p in RegisteredPanels)
        {
            PanelLookup.Add(p.id, p.panel);
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
        if (PanelLookup.ContainsKey(newPanel))
        {
            PanelLookup[CurrentPanel].SetActive(false);
            CurrentPanel = newPanel;
            PanelLookup[CurrentPanel].SetActive(true);
        } 
    }


    [Serializable]
    public class PanelEntry {
        public string id;
        public GameObject panel;
    }
}
