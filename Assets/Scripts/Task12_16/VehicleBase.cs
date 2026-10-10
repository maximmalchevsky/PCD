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

        protected Rigidbody vehicleRb;

        protected virtual void Start()
        {
            vehicleRb = GetComponent<Rigidbody>();
            if (vehicleRb != null)
            {
                vehicleRb.isKinematic = true;
                vehicleRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

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
                    if (dist <= interactRadius)
                    {
                        bool isTargetingItem = false;
                        var pi = Object.FindAnyObjectByType<Task8_11.PlayerInteraction>();
                        if (pi != null && pi.IsTargetingItem()) isTargetingItem = true;

                        bool wantsEnter = Input.GetKeyDown(KeyCode.F) || (!isTargetingItem && Input.GetKeyDown(KeyCode.E));
                        if (wantsEnter)
                        {
                            EnterVehicle();
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X))
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

            if (vehicleRb != null)
            {
                vehicleRb.isKinematic = false;
                vehicleRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }

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

            if (vehicleRb != null)
            {
                vehicleRb.linearVelocity = Vector3.zero;
                vehicleRb.angularVelocity = Vector3.zero;
                vehicleRb.isKinematic = true;
            }

            if (transform.position.y > 0.35f && !(this is TowerCraneController))
            {
                transform.position = new Vector3(transform.position.x, 0.22f, transform.position.z);
            }

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
                        bool isTargetingItem = false;
                        var pi = Object.FindAnyObjectByType<Task8_11.PlayerInteraction>();
                        if (pi != null && pi.IsTargetingItem()) isTargetingItem = true;

                        GUIStyle promptStyle = new GUIStyle(GUI.skin.box)
                        {
                            fontSize = 15,
                            fontStyle = FontStyle.Bold,
                            alignment = TextAnchor.MiddleCenter
                        };
                        promptStyle.normal.textColor = new Color(1f, 0.95f, 0.3f);
                        GUI.backgroundColor = new Color(0.1f, 0.12f, 0.18f, 0.92f);
                        string enterKey = isTargetingItem ? "[F]" : "[F / E]";
                        GUI.Box(new Rect(cx - 190f, Screen.height - 95f, 380f, 38f), enterKey + " Сесть в " + vehicleName, promptStyle);
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
            int lineCount = controlGuide.Split('\n').Length;
            float boxHeight = Mathf.Max(48f, lineCount * 22f + 14f);
            float boxY = Screen.height - 58f - boxHeight;
            GUI.Box(new Rect(cx - 380f, boxY, 760f, boxHeight), controlGuide, guideStyle);

            GUIStyle exitStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            exitStyle.normal.textColor = new Color(0.9f, 0.95f, 1f);
            GUI.backgroundColor = new Color(0.4f, 0.1f, 0.1f, 0.88f);
            GUI.Box(new Rect(cx - 180f, Screen.height - 54f, 360f, 26f), "[Escape / X] Выйти из кабины", exitStyle);
        }
    }
}
