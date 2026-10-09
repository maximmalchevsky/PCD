using UnityEngine;

namespace Task8_11
{
    public class FanController : MonoBehaviour
    {
        public Transform blades;
        public float rotationSpeed = 750f;
        public bool isSpinning = false;
        public Transform windOrigin;
        public float windRadius = 3.8f;
        public float windForce = 14f;

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

                    float dist = toBody.magnitude;
                    float prox = 1f - Mathf.Clamp01(dist / windRadius);

                    Vector3 swirl = Vector3.Cross(Vector3.up, outward).normalized;
                    Vector3 updraft = Vector3.up * Random.Range(1.0f, 2.2f);
                    Vector3 windDir = outward * 2.0f + updraft + swirl * 0.7f;

                    float blastForce = Mathf.Max(0.8f, rb.mass * 35f) * prox;
                    rb.AddForce(windDir.normalized * blastForce, ForceMode.Force);

                    Vector3 torqueDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), Random.Range(-1f, 1f));
                    rb.AddTorque(torqueDir * (blastForce * 0.15f), ForceMode.Force);

                    if (rb.linearVelocity.magnitude > 3.5f)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * 3.5f;
                    }

                    if (rb.position.y > origin.y - 0.35f)
                    {
                        rb.position = new Vector3(rb.position.x, origin.y - 0.35f, rb.position.z);
                        if (rb.linearVelocity.y > 0f)
                        {
                            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -0.6f, rb.linearVelocity.z);
                        }
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
