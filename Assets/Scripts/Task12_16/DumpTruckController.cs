using UnityEngine;

namespace Task12_16
{
    [RequireComponent(typeof(Rigidbody))]
    public class DumpTruckController : VehicleBase
    {
        public Transform dumpBed;
        public Transform tailgate;
        public Transform[] frontWheels;
        public Transform[] rearWheels;
        public float driveSpeed = 11.5f;
        public float steerSpeed = 50f;
        public float tiltSpeed = 24f;
        public float maxTiltAngle = 52f;
        public float currentTiltAngle = 0f;

        private Rigidbody rb;

        protected override void Start()
        {
            vehicleName = "Самосвал";
            base.Start();

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 8500f;
                rb.linearDamping = 1.5f;
                rb.angularDamping = 3.5f;
                rb.centerOfMass = new Vector3(0f, -0.6f, 0f);
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = true;
            }

            currentTiltAngle = 0f;
            if (dumpBed != null) dumpBed.localRotation = Quaternion.identity;
            if (tailgate != null) tailgate.localRotation = Quaternion.identity;
        }

        protected override void Update()
        {
            base.Update();

            if (!isPlayerInside)
            {
                // Ensure unoccupied truck never floats in mid-air
                if (transform.position.y > 0.35f)
                {
                    transform.position = new Vector3(transform.position.x, 0.22f, transform.position.z);
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }
                return;
            }

            if (Input.GetKey(KeyCode.R))
            {
                currentTiltAngle += tiltSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.F))
            {
                currentTiltAngle -= tiltSpeed * Time.deltaTime;
            }

            currentTiltAngle = Mathf.Clamp(currentTiltAngle, 0f, maxTiltAngle);

            if (dumpBed != null)
            {
                dumpBed.localRotation = Quaternion.Euler(-currentTiltAngle, 0f, 0f);
            }

            if (tailgate != null)
            {
                if (currentTiltAngle > 14f)
                {
                    float swing = Mathf.Clamp((currentTiltAngle - 14f) * 1.7f, 0f, 70f);
                    tailgate.localRotation = Quaternion.Euler(swing, 0f, 0f);
                }
                else
                {
                    tailgate.localRotation = Quaternion.identity;
                }
            }

            // Smooth cargo discharge when bed is elevated
            if (currentTiltAngle > 14f && dumpBed != null)
            {
                Vector3 bedCenter = dumpBed.position + dumpBed.forward * 1.6f + Vector3.up * 0.4f;
                Collider[] hits = Physics.OverlapSphere(bedCenter, 2.5f);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i] == null) continue;
                    Rigidbody crb = hits[i].attachedRigidbody;
                    if (crb != null && crb != rb && crb.GetComponent<GranularItem>() != null)
                    {
                        Vector3 slideForce = (-dumpBed.forward * 12f + Vector3.down * 6f) * (currentTiltAngle / maxTiltAngle);
                        crb.AddForce(slideForce, ForceMode.Acceleration);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (!isPlayerInside || rb == null) return;

            // Strict upright stabilization: guarantee zero pitch and zero roll
            Vector3 euler = transform.eulerAngles;
            rb.rotation = Quaternion.Euler(0f, euler.y, 0f);
            rb.angularVelocity = new Vector3(0f, rb.angularVelocity.y, 0f);

            // Ground vehicle physics lock: prevent any upward catapulting or depenetration impulses
            float clampedVy = Mathf.Min(0f, rb.linearVelocity.y);
            if (transform.position.y > 0.35f)
            {
                transform.position = new Vector3(transform.position.x, 0.22f, transform.position.z);
                clampedVy = 0f;
            }

            float vert = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vert += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vert -= 1f;
            if (Mathf.Abs(vert) < 0.01f) vert = Input.GetAxis("Vertical");

            float horiz = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horiz -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horiz += 1f;
            if (Mathf.Abs(horiz) < 0.01f) horiz = Input.GetAxis("Horizontal");

            Vector3 moveTarget = transform.forward * (vert * driveSpeed);
            rb.linearVelocity = new Vector3(moveTarget.x, clampedVy, moveTarget.z);

            if (Mathf.Abs(horiz) > 0.01f)
            {
                float turnDir = vert < -0.05f ? -horiz : horiz;
                Quaternion turn = Quaternion.Euler(0f, turnDir * steerSpeed * Time.fixedDeltaTime, 0f);
                rb.MoveRotation(rb.rotation * turn);
            }

            if (Mathf.Abs(vert) > 0.05f)
            {
                float rollAngle = vert * driveSpeed * Time.fixedDeltaTime * 150f;
                RotateWheelList(frontWheels, rollAngle);
                RotateWheelList(rearWheels, rollAngle);
            }
        }

        private void RotateWheelList(Transform[] list, float angle)
        {
            if (list == null) return;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] != null) list[i].Rotate(Vector3.right, angle, Space.Self);
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (isPlayerInside)
            {
                string info = "W / S — Движение вперед / назад  |  A / D — Руление\nR / F — Подъем / опускание кузова (выгрузка сыпучего груза)\nУгол подъема кузова: " + currentTiltAngle.ToString("F1") + "°";
                DrawVehicleHUD(info);
            }
        }
    }
}
