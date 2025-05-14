using AnimArch.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Visualization.ClassDiagram.Diagrams;

namespace Visualization.UI
{
    public class ToolManager : Singleton<ToolManager>
    {
        public enum Tool
        {
            NoToolSelected,
            CameraMovement,
            DiagramMovement,
            Movement3D,
            Highlighter
        }

        public Tool SelectedTool { private set; get; }
        public bool ZoomingIn { private set; get; }
        public bool ZoomingOut { private set; get; }
        public bool IsJump { set; get; }
        private bool Is25D { set; get; }
        
        public bool Reset { set; get; }
        public int InterGraphJump { private set; get; }
        public Color SelectedColor { private set; get; }
        [SerializeField] private string startingSelectedColor;
        [SerializeField] private Button[] buttons;
        private Tool _lastSelectedTool;

        public void ZoomIn(bool enabled)
        {
            ZoomingIn = enabled;
        }
        public void Change2D()
        {
            DiagramManager.Instance.ChangeLayout();
            Is25D = !Is25D;
        }

        public void Jump()
        {
            IsJump = true;
            InterGraphJump = ++InterGraphJump % 4;
        }

        public void Start()
        {
            SelectColor(startingSelectedColor);
        }

        public void ZoomOut(bool enabled)
        {
            ZoomingOut = enabled;
        }

        public void SelectColor(string colorID)
        {
            Color c;
            if (colorID.Contains("#"))
            {
                ColorUtility.TryParseHtmlString(colorID, out c);
            }
            else
            {
                ColorUtility.TryParseHtmlString("#" + colorID + "ff", out c);
            }

            SelectedColor = c;
        }

        public void SetActive(bool active)
        {
            if (active)
                SelectedTool = _lastSelectedTool;
            else
            {
                _lastSelectedTool = SelectedTool;
                SelectedTool = Tool.NoToolSelected;
            }
        }

        private void OnButtonClicked(Button clickedButton)
        {
            var buttonIndex = System.Array.IndexOf(buttons, clickedButton);

            if (buttonIndex == -1)
                return;

            buttons.ForEach(button => button.interactable = true);

            clickedButton.interactable = false;
        }

        private void SelectTool(Tool tool)
        {
            SelectedTool = tool;
            MenuManager.Instance.ActivatePanelColors(SelectedTool.Equals(Tool.Highlighter));
        }

        private void Awake()
        {
            buttons[0].onClick.AddListener(delegate
            {
                SelectTool(Tool.CameraMovement);
                OnButtonClicked(buttons[0]);
            });
            buttons[1].onClick.AddListener(delegate
            {
                SelectTool(Tool.DiagramMovement);
                OnButtonClicked(buttons[1]);
            });
            buttons[2].onClick.AddListener(delegate
            {
                SelectTool(Tool.Movement3D);
                OnButtonClicked(buttons[2]);
            });


            SelectTool(Tool.CameraMovement);
            OnButtonClicked(buttons[0]);

            Is25D = false;
        }
    }
}
