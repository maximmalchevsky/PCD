using UnityEngine;

namespace Task8_11
{
    public class TeleportZone : MonoBehaviour
    {
        public Transform targetPoint;
        public float cooldown = 1.8f;
        public Light glowLight;
        public Renderer ringRenderer;

        private static float globalLastTeleportTime = -100f;
        private Material ringMat;
        private Color baseEmissionColor = new Color(0.1f, 0.6f, 1.0f);
        private Transform playerTransform;
        private CharacterController playerCC;

        void Start()
        {
            if (ringRenderer != null)
            {
                ringMat = ringRenderer.material;
            }
            FindPlayer();
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
            float pulse = 0.75f + Mathf.PingPong(Time.time * 1.5f, 0.45f);
            if (ringMat != null)
            {
                ringMat.SetColor("_EmissionColor", baseEmissionColor * pulse);
            }
            if (glowLight != null)
            {
                glowLight.intensity = pulse * 2.2f;
            }

            if (Time.realtimeSinceStartup - globalLastTeleportTime < cooldown) return;
            if (targetPoint == null) return;

            if (playerTransform == null || playerCC == null)
            {
                FindPlayer();
                if (playerTransform == null || playerCC == null) return;
            }

            Vector2 pFlat = new Vector2(playerTransform.position.x, playerTransform.position.z);
            Vector2 zoneFlat = new Vector2(transform.position.x, transform.position.z);
            if (Vector2.Distance(pFlat, zoneFlat) < 1.15f && Mathf.Abs(playerTransform.position.y - transform.position.y) < 2.0f)
            {
                globalLastTeleportTime = Time.realtimeSinceStartup;
                DoTeleport();
            }
        }

        private void DoTeleport()
        {
            if (playerCC != null) playerCC.enabled = false;
            playerTransform.position = targetPoint.position;
            playerTransform.rotation = targetPoint.rotation;
            Physics.SyncTransforms();
            if (playerCC != null) playerCC.enabled = true;
        }
    }
}
