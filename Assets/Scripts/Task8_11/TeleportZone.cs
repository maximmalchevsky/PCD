using UnityEngine;

namespace Task8_11
{
    public class TeleportZone : MonoBehaviour
    {
        public Transform targetPoint;
        public float interactRadius = 1.4f;
        public string promptText = "[E] Войти в здание";
        public Light glowLight;
        public Renderer ringRenderer;

        private static float globalLastTeleportTime = -100f;
        private Transform playerTransform;
        private CharacterController playerCC;
        private Material ringMat;
        private Color baseEmissionColor = new Color(0.1f, 0.85f, 1.0f);
        private bool isInRange = false;
        private float fadeAlpha = 0f;
        private bool isFadingOut = false;
        private bool isFadingIn = false;
        private float fadeSpeed = 3.5f;
        private Texture2D fadeTex;

        void Start()
        {
            if (ringRenderer != null)
            {
                ringMat = ringRenderer.material;
            }
            FindPlayer();
            fadeTex = new Texture2D(1, 1);
            fadeTex.SetPixel(0, 0, Color.black);
            fadeTex.Apply();
        }

        private void FindPlayer()
        {
            GameObject p = GameObject.Find("Player");
            if (p != null)
            {
                playerTransform = p.transform;
                playerCC = p.GetComponent<CharacterController>();
            }
        }

        void Update()
        {
            if (playerTransform == null || playerCC == null)
            {
                FindPlayer();
                if (playerTransform == null) return;
            }

            Vector2 pFlat = new Vector2(playerTransform.position.x, playerTransform.position.z);
            Vector2 zoneFlat = new Vector2(transform.position.x, transform.position.z);
            float dist = Vector2.Distance(pFlat, zoneFlat);
            float deltaY = Mathf.Abs(playerTransform.position.y - transform.position.y);
            isInRange = dist <= interactRadius && deltaY < 2.0f;

            if (isFadingOut)
            {
                fadeAlpha += Time.deltaTime * fadeSpeed;
                if (fadeAlpha >= 1f)
                {
                    fadeAlpha = 1f;
                    isFadingOut = false;
                    ExecuteTeleport();
                    isFadingIn = true;
                }
                return;
            }

            if (isFadingIn)
            {
                fadeAlpha -= Time.deltaTime * fadeSpeed;
                if (fadeAlpha <= 0f)
                {
                    fadeAlpha = 0f;
                    isFadingIn = false;
                }
                return;
            }

            PlayerInteraction pi = playerTransform.GetComponentInChildren<PlayerInteraction>();
            bool isAimingItem = pi != null && pi.IsTargetingItem();

            if (isInRange && targetPoint != null && !isAimingItem)
            {
                if (Time.realtimeSinceStartup - globalLastTeleportTime > 0.8f)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        StartTransition();
                    }
                }
            }

            float pulse = 0.8f + Mathf.PingPong(Time.time * 2f, 0.4f);
            if (ringMat != null)
            {
                ringMat.SetColor("_BaseColor", baseEmissionColor * (isInRange ? pulse * 1.3f : pulse * 0.85f));
            }
            if (glowLight != null)
            {
                glowLight.intensity = (isInRange ? 2.2f : 1.1f) * pulse;
            }
        }

        public void StartTransition()
        {
            if (targetPoint == null || isFadingOut || isFadingIn) return;
            globalLastTeleportTime = Time.realtimeSinceStartup;
            isFadingOut = true;
            fadeAlpha = 0f;
        }

        private void ExecuteTeleport()
        {
            if (playerCC != null) playerCC.enabled = false;
            playerTransform.position = targetPoint.position;
            playerTransform.rotation = targetPoint.rotation;
            Physics.SyncTransforms();
            if (playerCC != null) playerCC.enabled = true;
        }

        void OnGUI()
        {
            PlayerInteraction pi = playerTransform != null ? playerTransform.GetComponentInChildren<PlayerInteraction>() : null;
            bool isAimingItem = pi != null && pi.IsTargetingItem();

            if (isInRange && !isFadingOut && !isFadingIn && !string.IsNullOrEmpty(promptText) && !isAimingItem)
            {
                float w = 240f;
                float h = 38f;
                float x = (Screen.width - w) * 0.5f;
                float y = Screen.height * 0.72f;

                GUI.Box(new Rect(x, y, w, h), string.Empty);
                GUIStyle st = new GUIStyle(GUI.skin.label);
                st.alignment = TextAnchor.MiddleCenter;
                st.fontSize = 15;
                st.fontStyle = FontStyle.Bold;
                st.normal.textColor = Color.cyan;
                GUI.Label(new Rect(x, y, w, h), promptText, st);
            }

            if (fadeAlpha > 0.001f && fadeTex != null)
            {
                Color prev = GUI.color;
                GUI.color = new Color(0f, 0f, 0f, Mathf.Clamp01(fadeAlpha));
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTex);
                GUI.color = prev;
            }
        }
    }
}
