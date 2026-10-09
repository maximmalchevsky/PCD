using UnityEngine;

namespace Task12_16
{
    public abstract class VehicleBase : MonoBehaviour
    {
        public string vehicleName = "Техника";
        public float interactRadius = 3.8f;
        public Transform exitTransform;
        public Camera vehicleCamera;

        protected bool isPlayerInside = false;
        protected GameObject playerObj;
        protected CharacterController playerCC;
        protected MonoBehaviour playerControllerScript;
        protected Camera playerCamera;
        protected Renderer[] playerRenderers;

        private static VehicleBase activeVehicle = null;

        public static VehicleBase GetActiveVehicle()
        {
            return activeVehicle;
        }

        protected virtual void Start()
        {
            if (vehicleCamera != null)
            {
                vehicleCamera.enabled = false;
                AudioListener al = vehicleCamera.GetComponent<AudioListener>();
                if (al != null) al.enabled = false;
            }

            FindPlayerReferences();
        }

        protected void FindPlayerReferences()
        {
            if (playerObj != null) return;

            var pc = Object.FindAnyObjectByType<Task4_5.PlayerController>();
            if (pc != null)
            {
                playerObj = pc.gameObject;
                playerControllerScript = pc;
                playerCC = pc.GetComponent<CharacterController>();
                playerCamera = pc.playerCamera != null ? pc.playerCamera : pc.GetComponentInChildren<Camera>(true);
                playerRenderers = pc.GetComponentsInChildren<Renderer>(true);
            }
            else
            {
                GameObject taggedPlayer = GameObject.FindWithTag("Player");
                if (taggedPlayer != null)
                {
                    playerObj = taggedPlayer;
                    playerCC = taggedPlayer.GetComponent<CharacterController>();
                    playerCamera = taggedPlayer.GetComponentInChildren<Camera>(true);
                    playerRenderers = taggedPlayer.GetComponentsInChildren<Renderer>(true);
                }
            }
        }

        protected virtual void Update()
        {
            if (playerObj == null)
            {
                FindPlayerReferences();
                if (playerObj == null) return;
            }

            if (!isPlayerInside)
            {
                if (activeVehicle == null)
                {
                    float dist = Vector3.Distance(transform.position, playerObj.transform.position);
                    if (dist <= interactRadius && Input.GetKeyDown(KeyCode.E))
                    {
                        EnterVehicle();
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ExitVehicle();
                }
            }
        }

        public virtual void EnterVehicle()
        {
            if (activeVehicle != null && activeVehicle != this) return;

            activeVehicle = this;
            isPlayerInside = true;

            if (playerCC != null) playerCC.enabled = false;
            if (playerControllerScript != null) playerControllerScript.enabled = false;
            if (playerCamera != null) playerCamera.enabled = false;

            if (playerRenderers != null)
            {
                for (int i = 0; i < playerRenderers.Length; i++)
                {
                    if (playerRenderers[i] != null) playerRenderers[i].enabled = false;
                }
            }

            if (vehicleCamera != null)
            {
                vehicleCamera.enabled = true;
                AudioListener al = vehicleCamera.GetComponent<AudioListener>();
                if (al != null) al.enabled = true;
            }
        }

        public virtual void ExitVehicle()
        {
            isPlayerInside = false;
            if (activeVehicle == this) activeVehicle = null;

            if (vehicleCamera != null)
            {
                vehicleCamera.enabled = false;
                AudioListener al = vehicleCamera.GetComponent<AudioListener>();
                if (al != null) al.enabled = false;
            }

            Vector3 spawnPos = exitTransform != null ? exitTransform.position : transform.position + transform.right * 3f + Vector3.up * 0.1f;
            if (playerObj != null)
            {
                playerObj.transform.position = spawnPos;
            }

            if (playerRenderers != null)
            {
                for (int i = 0; i < playerRenderers.Length; i++)
                {
                    if (playerRenderers[i] != null) playerRenderers[i].enabled = true;
                }
            }

            if (playerCamera != null) playerCamera.enabled = true;
            if (playerControllerScript != null) playerControllerScript.enabled = true;
            if (playerCC != null) playerCC.enabled = true;
        }

        protected virtual void OnGUI()
        {
            float cx = Screen.width * 0.5f;

            if (!isPlayerInside)
            {
                if (activeVehicle == null && playerObj != null)
                {
                    float dist = Vector3.Distance(transform.position, playerObj.transform.position);
                    if (dist <= interactRadius)
                    {
                        GUIStyle promptStyle = new GUIStyle(GUI.skin.box)
                        {
                            fontSize = 15,
                            fontStyle = FontStyle.Bold,
                            alignment = TextAnchor.MiddleCenter
                        };
                        promptStyle.normal.textColor = new Color(1f, 0.95f, 0.3f);
                        GUI.backgroundColor = new Color(0.1f, 0.12f, 0.18f, 0.92f);
                        GUI.Box(new Rect(cx - 190f, Screen.height - 95f, 380f, 38f), "[E] Применить: Сесть в " + vehicleName, promptStyle);
                    }
                }
            }
        }

        protected void DrawVehicleHUD(string controlGuide)
        {
            float cx = Screen.width * 0.5f;

            GUIStyle titleStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            titleStyle.normal.textColor = new Color(1f, 0.9f, 0.2f);
            GUI.backgroundColor = new Color(0.12f, 0.12f, 0.16f, 0.92f);
            GUI.Box(new Rect(cx - 240f, 20f, 480f, 34f), "УПРАВЛЕНИЕ: " + vehicleName.ToUpper(), titleStyle);

            GUIStyle guideStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter
            };
            guideStyle.normal.textColor = Color.white;
            GUI.backgroundColor = new Color(0.08f, 0.1f, 0.14f, 0.88f);
            GUI.Box(new Rect(cx - 360f, Screen.height - 110f, 720f, 48f), controlGuide, guideStyle);

            GUIStyle exitStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            exitStyle.normal.textColor = new Color(0.9f, 0.95f, 1f);
            GUI.backgroundColor = new Color(0.4f, 0.1f, 0.1f, 0.88f);
            GUI.Box(new Rect(cx - 160f, Screen.height - 54f, 320f, 26f), "[E] Применить: Выйти из кабины", exitStyle);
        }
    }
}
