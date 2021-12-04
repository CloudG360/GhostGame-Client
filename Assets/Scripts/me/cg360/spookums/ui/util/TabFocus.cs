using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace me.cg360.spookums.ui.util
{
    public class TabFocus : MonoBehaviour
    {
        public TMP_InputField ThisInput;
        public TMP_InputField NextInput;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (ThisInput != null && NextInput != null && ThisInput.isFocused)
                {
                    ThisInput.ReleaseSelection();
                    NextInput.Select();
                }
            }
        }
    }
}
