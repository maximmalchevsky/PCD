using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Task4_5
{
    public class CameraController : MonoBehaviour
    {
        public Camera[] cameras;
        public string[] cameraNames;
        public int activeIndex = 0;

        void Start()
        {
            UpdateActiveCamera();
        }

        void Update()
        {
            bool toggleKey = Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Tab);
            int pressedNum = -1;
            for (int k = 0; k < 9; k++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + k) || Input.GetKeyDown(KeyCode.Keypad1 + k))
                {
                    pressedNum = k;
                    break;
                }
            }

#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                toggleKey |= Keyboard.current.vKey.wasPressedThisFrame || Keyboard.current.cKey.wasPressedThisFrame || Keyboard.current.tabKey.wasPressedThisFrame;
                if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame) pressedNum = 0;
                else if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame) pressedNum = 1;
                else if (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame) pressedNum = 2;
                else if (Keyboard.current.digit4Key.wasPressedThisFrame || Keyboard.current.numpad4Key.wasPressedThisFrame) pressedNum = 3;
            }
#endif

            if (toggleKey && cameras != null && cameras.Length > 0)
            {
                activeIndex = (activeIndex + 1) % cameras.Length;
                UpdateActiveCamera();
            }
            else if (pressedNum >= 0 && cameras != null && pressedNum < cameras.Length)
            {
                SwitchTo(pressedNum);
            }
        }

        public void SwitchTo(int index)
        {
            if (cameras != null && index >= 0 && index < cameras.Length)
            {
                activeIndex = index;
                UpdateActiveCamera();
            }
        }

        public void UpdateActiveCamera()
        {
            if (cameras == null || cameras.Length == 0) return;

            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] != null)
                {
                    bool isActive = (i == activeIndex);
                    cameras[i].enabled = isActive;
                    cameras[i].gameObject.SetActive(isActive);
                    cameras[i].depth = isActive ? 100f : -100f;
                    if (isActive)
                    {
                        cameras[i].tag = "MainCamera";
                    }
                    AudioListener listener = cameras[i].GetComponent<AudioListener>();
                    if (listener != null)
                    {
                        listener.enabled = isActive;
                    }
                }
            }

            if (activeIndex == 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public string GetCurrentCameraName()
        {
            if (cameraNames != null && activeIndex < cameraNames.Length)
            {
                return cameraNames[activeIndex];
            }
            return "Камера " + (activeIndex + 1);
        }

        public int GetActiveIndex()
        {
            return activeIndex;
        }

        void OnGUI()
        {
            if (cameras == null || cameras.Length == 0) return;
            float btnWidth = 110f;
            float totalWidth = Mathf.Min(Screen.width - 20f, cameras.Length * btnWidth + 10f);
            GUILayout.BeginArea(new Rect(Screen.width - totalWidth - 10f, 10f, totalWidth, 38f));
            GUILayout.BeginHorizontal();
            for (int i = 0; i < cameras.Length; i++)
            {
                string title = (cameraNames != null && i < cameraNames.Length) ? cameraNames[i] : ("Кам " + (i + 1));
                if (title.Length > 11) title = title.Substring(0, 10) + "..";
                string btnText = (i + 1) + ": " + title + " [" + (i + 1) + "]";
                if (GUILayout.Button(btnText, GUILayout.Height(30))) SwitchTo(i);
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
