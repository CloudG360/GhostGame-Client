using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Connect_ButtonController : MonoBehaviour
{
    public Button Button;
    void Start()
    {
        Button.onClick.AddListener(ClickedButton);
    }

    void ClickedButton()
    {
        
    }
}
