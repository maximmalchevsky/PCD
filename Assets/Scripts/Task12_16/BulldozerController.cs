using UnityEngine;

namespace Task12_16
{
    [RequireComponent(typeof(Rigidbody))]
    public class BulldozerController : VehicleBase
    {
        public Transform bladeTransform;
        public Transform[] wheels;
        public float driveSpeed = 7.5f;
        public float steerSpeed = 48f;
        public float bladeSpeed = 0.9f;
        public float bladeMin = 0.08f;
        public float bladeMax = 1.45f;
        public float currentBladeHeight = 0.15f;

        private Rigidbody rb;
        private Vector3 initialBladeLocalPos;

        protected override void Start()
        {
            vehicleName = "Бульдозер";
            base.Start();

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 9500f;
                rb.linearDamping = 2.0f;
                rb.angularDamping = 4.0f;
                rb.centerOfMass = new Vector3(0f, -0.6f, 0f);
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = true;
            }

            if (bladeTransform != null)
            {
                initialBladeLocalPos = bladeTransform.localPosition;
                currentBladeHeight = initialBladeLocalPos.y;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!isPlayerInside)
            {
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
                currentBladeHeight += bladeSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.F))
            {
                currentBladeHeight -= bladeSpeed * Time.deltaTime;
            }

            currentBladeHeight = Mathf.Clamp(currentBladeHeight, bladeMin, bladeMax);

            if (bladeTransform != null)
            {
                bladeTransform.localPosition = new Vector3(initialBladeLocalPos.x, currentBladeHeight, initialBladeLocalPos.z);
            }
        }

        void FixedUpdate()
        {
            if (!isPlayerInside || rb == null) return;

            // Strict upright stabilization: guarantee zero pitch and zero roll
            Vector3 euler = transform.eulerAngles;
            rb.rotation = Quaternion.Euler(0f, euler.y, 0f);
            rb.angularVelocity = new Vector3(0f, rb.angularVelocity.y, 0f);

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

            if (wheels != null && Mathf.Abs(vert) > 0.05f)
            {
                float rotAngle = vert * driveSpeed * Time.fixedDeltaTime * 140f;
                for (int i = 0; i < wheels.Length; i++)
                {
                    if (wheels[i] != null) wheels[i].Rotate(Vector3.right, rotAngle, Space.Self);
                }
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (isPlayerInside)
            {
                string info = "W / S — Движение вперед / назад  |  A / D — Руление\nR / F — Поднять / опустить отвал  |  Текущая высота отвала: " + currentBladeHeight.ToString("F2") + " м";
                DrawVehicleHUD(info);
            }
        }
    }
}
