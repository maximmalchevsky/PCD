using UnityEngine;

namespace Task8_11
{
    public class FanController : MonoBehaviour
    {
        public Transform blades;
        public float rotationSpeed = 750f;
        public bool isSpinning = false;
        public Transform windOrigin;
        public float windRadius = 4.0f;
        public float windForce = 16f;

        void Update()
        {
            if (isSpinning && blades != null)
            {
                blades.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            }
        }

        void FixedUpdate()
        {
            if (!isSpinning) return;

            Vector3 origin = windOrigin != null ? windOrigin.position : transform.position;

            Collider[] hits = Physics.OverlapSphere(origin, windRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                Rigidbody rb = hits[i].attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    if (rb.mass > 0.35f) continue;

                    Vector3 toBody = rb.position - origin;
                    Vector3 outward = new Vector3(toBody.x, 0f, toBody.z);
                    if (outward.sqrMagnitude < 0.02f)
                    {
                        outward = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
                    }
                    outward.Normalize();

                    Vector3 windDir = outward * 0.95f + Vector3.down * 0.25f;

                    if (rb.position.y < origin.y - 1.1f)
                    {
                        float liftPulse = 0.35f + Mathf.PingPong(Time.time * 5f + rb.position.x * 2f, 0.3f);
                        windDir += Vector3.up * liftPulse;
                    }

                    rb.AddForce(windDir.normalized * windForce, ForceMode.Force);

                    Vector3 torqueDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.3f, 0.3f), Random.Range(-1f, 1f));
                    rb.AddTorque(torqueDir * windForce * 0.4f, ForceMode.Force);

                    if (rb.linearVelocity.y > 1.2f)
                    {
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 1.2f, rb.linearVelocity.z);
                    }

                    if (rb.position.y > origin.y - 0.25f)
                    {
                        rb.position = new Vector3(rb.position.x, origin.y - 0.25f, rb.position.z);
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -0.4f, rb.linearVelocity.z);
                    }
                }
            }
        }

        public void Toggle()
        {
            isSpinning = !isSpinning;
        }
    }
}
