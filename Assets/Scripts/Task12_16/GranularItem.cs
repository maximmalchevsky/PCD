using UnityEngine;

namespace Task12_16
{
    [RequireComponent(typeof(Rigidbody))]
    public class GranularItem : MonoBehaviour
    {
        public enum GranularType { Sand, Rock, Brick }
        public GranularType itemType = GranularType.Sand;

        private Rigidbody rb;
        private Vector3 initialPosition;
        private Quaternion initialRotation;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            initialPosition = transform.position;
            initialRotation = transform.rotation;

            ConfigurePhysics();
        }

        void Start()
        {
            IgnoreVehicleChassisCollisions();
        }

        public void IgnoreVehicleChassisCollisions()
        {
            Collider myCol = GetComponent<Collider>();
            if (myCol == null) return;

            VehicleBase[] vehicles = Object.FindObjectsByType<VehicleBase>(FindObjectsInactive.Exclude);
            for (int i = 0; i < vehicles.Length; i++)
            {
                if (vehicles[i] == null) continue;
                Collider rootCol = vehicles[i].GetComponent<Collider>();
                if (rootCol != null)
                {
                    Physics.IgnoreCollision(myCol, rootCol, true);
                }
            }
        }

        private void ConfigurePhysics()
        {
            if (rb == null) return;

            PhysicsMaterial granularMat = new PhysicsMaterial("GranularPhysMat")
            {
                dynamicFriction = 0.85f,
                staticFriction = 0.95f,
                bounciness = 0.0f,
                frictionCombine = PhysicsMaterialCombine.Maximum,
                bounceCombine = PhysicsMaterialCombine.Minimum
            };

            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.sharedMaterial = granularMat;
            }

            switch (itemType)
            {
                case GranularType.Sand:
                    rb.mass = 0.45f;
                    rb.linearDamping = 1.0f;
                    rb.angularDamping = 2.0f;
                    break;
                case GranularType.Rock:
                    rb.mass = 2.2f;
                    rb.linearDamping = 1.2f;
                    rb.angularDamping = 2.2f;
                    break;
                case GranularType.Brick:
                    rb.mass = 3.6f;
                    rb.linearDamping = 1.4f;
                    rb.angularDamping = 2.5f;
                    break;
            }

            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        void FixedUpdate()
        {
            // Airborne safety: ensure cargo never hangs suspended in the sky
            if (transform.position.y > 6.0f)
            {
                rb.isKinematic = false;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -6.0f, rb.linearVelocity.z);
            }

            if (transform.position.y < -4f || Vector3.Distance(transform.position, initialPosition) > 60f)
            {
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                transform.position = initialPosition;
                transform.rotation = initialRotation;
            }
        }
    }
}
