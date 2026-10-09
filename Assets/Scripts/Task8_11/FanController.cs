using UnityEngine;

namespace Task8_11
{
    public class FanController : MonoBehaviour
    {
        public Transform blades;
        public float rotationSpeed = 720f;
        public bool isSpinning = false;
        public Transform windOrigin;
        public float windRadius = 3.5f;
        public float windForce = 18f;

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
            Vector3 windDir = windOrigin != null ? -windOrigin.up : Vector3.down;

            Collider[] hits = Physics.OverlapSphere(origin, windRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                Rigidbody rb = hits[i].attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    PickupableItem item = hits[i].GetComponentInParent<PickupableItem>();
                    if (item != null)
                    {
                        Vector3 push = (windDir * 0.7f + Random.insideUnitSphere * 0.4f).normalized;
                        rb.AddForce(push * windForce, ForceMode.Force);
                        rb.AddTorque(Random.insideUnitSphere * windForce * 0.5f, ForceMode.Force);
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
