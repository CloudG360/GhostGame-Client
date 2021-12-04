using UnityEngine;
using UnityEngine.UI;

namespace me.cg360.spookums.ui.menu.main
{
    public class ControllerConnectErrorMenu : MonoBehaviour
    {
        public Main Main;
        public Button Button;

        void Start()
        {
            Button.onClick.AddListener(ClickedButton);
            Button.interactable = true;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        void ClickedButton()
        {
            if (Button.interactable)
            {
                Main.MainMenuController.SwitchPanel("connect_panel");
            }
        }
    }
}

