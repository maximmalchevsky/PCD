using UnityEngine;

namespace Task8_11
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class PaperSheet : MonoBehaviour
    {
        public float width = 0.21f;
        public float thickness = 0.002f;
        public float length = 0.297f;

        private Rigidbody rb;
        private FanController roomFan;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.mass = 0.02f;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 2.0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            transform.localScale = new Vector3(width, thickness, length);

            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                col.size = Vector3.one;
                col.center = Vector3.zero;
            }

            PickupableItem pickup = GetComponent<PickupableItem>();
            if (pickup == null)
            {
                pickup = gameObject.AddComponent<PickupableItem>();
            }
            if (string.IsNullOrEmpty(pickup.itemName))
            {
                pickup.itemName = name.Replace("Paper_", "").Replace("_", " ");
            }
            pickup.rb = rb;
        }

        void Start()
        {
            roomFan = Object.FindAnyObjectByType<FanController>();
            transform.localScale = new Vector3(width, thickness, length);
        }

        void FixedUpdate()
        {
            if (rb == null || rb.isKinematic) return;

            Vector3 vel = rb.linearVelocity;
            if (vel.y < -0.1f)
            {
                rb.AddForce(Vector3.up * (-vel.y * 0.45f * rb.mass * 9.81f), ForceMode.Force);
            }

            if (roomFan != null && roomFan.isSpinning)
            {
                Vector3 fanPos = roomFan.windOrigin != null ? roomFan.windOrigin.position : roomFan.transform.position;
                Vector3 toFan = rb.position - fanPos;
                float dist = toFan.magnitude;

                if (dist < roomFan.windRadius)
                {
                    Vector3 horiz = new Vector3(toFan.x, 0f, toFan.z);
                    Vector3 outDir = horiz.sqrMagnitude > 0.01f ? horiz.normalized : new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

                    float proximity = 1f - Mathf.Clamp01(dist / roomFan.windRadius);
                    Vector3 updraft = Vector3.up * (0.8f + proximity * 1.0f);
                    Vector3 swirl = Vector3.Cross(Vector3.up, outDir) * 0.5f;
                    Vector3 blast = (outDir * 1.5f + updraft + swirl) * (proximity * 0.4f);

                    rb.AddForce(blast, ForceMode.Force);
                    rb.AddTorque(Random.insideUnitSphere * 0.04f, ForceMode.Force);
                }
            }

            Vector3 curP = rb.position;
            float cX = Mathf.Clamp(curP.x, 297.45f, 302.55f);
            float cZ = Mathf.Clamp(curP.z, 297.45f, 302.55f);
            float cY = Mathf.Clamp(curP.y, -49.99f, -48.05f);
            if (curP.x != cX || curP.z != cZ || curP.y != cY)
            {
                rb.position = new Vector3(cX, cY, cZ);
            }
        }
    }
}
