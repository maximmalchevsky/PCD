using UnityEngine;

namespace Task8_11
{
    public class FanController : MonoBehaviour
    {
        public Transform blades;
        public float rotationSpeed = 750f;
        public bool isSpinning = false;
        public Transform windOrigin;
        public float windRadius = 4.2f;
        public float windForce = 26f;

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
                    Vector3 toBody = rb.position - origin;
                    Vector3 radial = new Vector3(toBody.x, 0f, toBody.z);
                    if (radial.sqrMagnitude < 0.04f)
                    {
                        radial = Random.insideUnitSphere;
                        radial.y = 0f;
                    }

                    float mass = Mathf.Max(0.01f, rb.mass);
                    float lightFactor = Mathf.Clamp01((0.4f - mass) / 0.4f);
                    float liftVal = lightFactor > 0.05f ? 0.65f + Mathf.PingPong(Time.time * 6f + rb.position.x * 4f, 0.45f) : 0.1f;
                    Vector3 lift = Vector3.up * liftVal;

                    Vector3 gust = (radial.normalized * 0.9f + lift + Random.insideUnitSphere * 0.3f).normalized;
                    float appliedForce = windForce * (1f + lightFactor * 1.5f);
                    rb.AddForce(gust * appliedForce, ForceMode.Force);

                    Vector3 torqueDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-0.5f, 0.5f), Random.Range(-1f, 1f));
                    rb.AddTorque(torqueDir * appliedForce * 0.8f, ForceMode.Force);
                }
            }
        }

        public void Toggle()
        {
            isSpinning = !isSpinning;
        }
    }
}
