using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Visualization.ClassDiagram;
using Visualization.UI;

namespace Visualization
{
    public class FlyCamera : MonoBehaviour
    {
        /*
        Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.
        Converted to C# 27-02-13 - no credit wanted.
        Reformatted and cleaned by Ryan Breaker 23-6-18

        Original comment:
        Simple flycam I made, since I couldn't find any others made public.
        Made simple to use (drag and drop, done) for regular keyboard layout.

        Controls:
        WASD  : Directional movement
        Shift : Increase speed
        Space : Moves camera directly up per its local Y-axis
        */

        public float mainSpeed = 5.0f; // Regular speed
        public float shiftAdd = 15.0f; // Amount to accelerate when shift is pressed
        public float maxShift = 50.0f; // Maximum speed when holding shift
        public float camSens = 0.15f; // Mouse sensitivity
        public float movementToolSpeed = 2.5f;
        public float offsetX;
        public float offsetY;
        private float offsetZ = 800;  
        public GameObject[] IgnoredInputs;

        private Vector3
            lastMouse = new Vector3(255, 255,
                255); // kind of in the middle of the screen, rather than at the top (play)

        private float totalRun = 1.0f;

        private void Start()
        {
            transform.position = transform.position + new Vector3(offsetX, offsetY);
        }

        void Update()
        {
            if (DiagramPool.Instance.ClassDiagram.graph == null)
                return;

            if (ToolManager.Instance.Reset)
            {
                transform.position = new Vector3(130, 20, -700);
                transform.eulerAngles = new Vector3(0, 0, 0);
                ToolManager.Instance.Reset = false;
            }

            if (Input.GetMouseButton(0) && ToolManager.Instance.SelectedTool == ToolManager.Tool.Movement3D &&
                !IsMouseOverUI())
            {
                lastMouse = Input.mousePosition - lastMouse;
                lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
                lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y,
                    0);
                transform.eulerAngles = lastMouse;
            }

            lastMouse = Input.mousePosition;

            if (IgnoredInputs.Any(item => item.GetComponentInChildren<TMP_InputField>()?.isFocused == true))
            {
                return;
            }

            // Keyboard commands
            Vector3 p = GetBaseInput();
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p *= totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p *= mainSpeed;
            }

            p *= Time.deltaTime;
            transform.Translate(p);

            if (ToolManager.Instance.IsJump)
            {
                if (ToolManager.Instance.InterGraphJump == 0)
                    transform.position = new Vector3(offsetX, offsetY, -700);

                if (ToolManager.Instance.InterGraphJump == 1)
                    transform.position = new Vector3(offsetX, offsetY, -700 + offsetZ);

                if (ToolManager.Instance.InterGraphJump == 2)
                    transform.position = new Vector3(offsetX, offsetY, -700 + 2*offsetZ);
                
                if (ToolManager.Instance.InterGraphJump == 3)
                    transform.position = new Vector3(offsetX, offsetY, -700 + 3*offsetZ);
                ToolManager.Instance.IsJump = false;
            }

            if (Input.GetMouseButton(0) && ToolManager.Instance.SelectedTool == ToolManager.Tool.CameraMovement &&
                !IsMouseOverUI())
            {
                //grab the rotation of the camera so we can move in a psuedo local XY space
                //transform.rotation = transform.rotation;
                transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * movementToolSpeed);
                transform.Translate(transform.up * -Input.GetAxis("Mouse Y") * movementToolSpeed, Space.World);
            }
        }

        // Returns the basic values, if it's 0 than it's not active.
        private Vector3 GetBaseInput()
        {
            Vector3 p_Velocity = new Vector3();

            // Forwards
            if (ToolManager.Instance.ZoomingIn)
                p_Velocity += new Vector3(0, 0, 1);

            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetAxis("Mouse ScrollWheel") > 0)
                p_Velocity += new Vector3(0, 0, 10);

            // Backwards
            if (ToolManager.Instance.ZoomingOut)
                p_Velocity += new Vector3(0, 0, -1);
            
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetAxis("Mouse ScrollWheel") < 0)
                p_Velocity += new Vector3(0, 0, -10);
            // Left
            /*
            if (Input.GetKey(KeyCode.A))
                p_Velocity += new Vector3(-1, 0, 0);

            // Right
            if (Input.GetKey(KeyCode.D))
                p_Velocity += new Vector3(1, 0, 0);

            // Up
            if (Input.GetKey(KeyCode.Space))
                p_Velocity += new Vector3(0, 1, 0);

            // Down
            if (Input.GetKey(KeyCode.LeftControl))
                p_Velocity += new Vector3(0, -1, 0);
                */
            return p_Velocity;
        }

        private bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
