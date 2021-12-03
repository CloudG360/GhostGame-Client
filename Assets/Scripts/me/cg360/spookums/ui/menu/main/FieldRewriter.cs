using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace me.cg360.spookums.ui.menu.main
{
    public class FieldRewriter : UILookup
    {

        public bool WriteField(string fieldID, string content)
        {
            if (ElementLookup.ContainsKey(fieldID))
            {
                GameObject field = ElementLookup[fieldID];
                TMP_Text textEditor = field.GetComponent<TMP_Text>();

                if (textEditor is null)
                {
                    Debug.LogWarning($"Missing a TMP_Text type on field {fieldID}");
                    return false;
                }
                
                textEditor.SetText(content);
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}