using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace me.cg360.spookums.ui.menu
{
    public class MenuPanelController : UILookup
    {
        public string DefaultPanel = "connect_panel";
        public string CurrentPanel { get; private set; }

        public override void Awake()
        {
            CurrentPanel = DefaultPanel;
            ElementLookup = new Dictionary<string, GameObject>();
            foreach (ElementEntry p in Elements)
            {
                ElementLookup.Add(p.id, p.panel);
                p.panel.SetActive(true);
                if (p.id != CurrentPanel)
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
}
