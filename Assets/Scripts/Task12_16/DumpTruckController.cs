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
                rb.mass = 7800f;
                rb.linearDamping = 1.2f;
                rb.angularDamping = 2.5f;
                rb.centerOfMass = new Vector3(0f, -0.35f, 0f);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!isPlayerInside) return;

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
        }

        void FixedUpdate()
        {
            if (!isPlayerInside || rb == null) return;

            float vert = Input.GetAxis("Vertical");
            float horiz = Input.GetAxis("Horizontal");

            Vector3 moveTarget = transform.forward * (vert * driveSpeed);
            rb.linearVelocity = new Vector3(moveTarget.x, rb.linearVelocity.y, moveTarget.z);

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
