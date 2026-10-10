using UnityEngine;

namespace Task12_16
{
    [RequireComponent(typeof(Rigidbody))]
    public class ExcavatorController : VehicleBase
    {
        public Transform turret;
        public Transform boom;
        public Transform stick;
        public Transform bucket;

        public float turretSpeed = 38f;
        public float boomSpeed = 28f;
        public float stickSpeed = 32f;
        public float bucketSpeed = 42f;
        public float driveSpeed = 6.0f;
        public float steerSpeed = 46f;

        public float currentTurretYaw = 0f;
        public float currentBoomAngle = -24f;
        public float currentStickAngle = 36f;
        public float currentBucketAngle = 18f;

        private Rigidbody rb;

        protected override void Start()
        {
            vehicleName = "Экскаватор";
            base.Start();

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 16000f;
                rb.linearDamping = 1.5f;
                rb.angularDamping = 3.0f;
                rb.centerOfMass = new Vector3(0f, -0.6f, 0f);
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = true;
            }

            if (turret != null) turret.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
            if (boom != null) boom.localRotation = Quaternion.Euler(currentBoomAngle, 0f, 0f);
            if (stick != null) stick.localRotation = Quaternion.Euler(currentStickAngle, 0f, 0f);
            if (bucket != null) bucket.localRotation = Quaternion.Euler(currentBucketAngle, 0f, 0f);
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

            // Turret rotation (Q / E)
            float yawInput = 0f;
            if (Input.GetKey(KeyCode.Q)) yawInput -= 1f;
            if (Input.GetKey(KeyCode.E)) yawInput += 1f;
            currentTurretYaw += yawInput * turretSpeed * Time.deltaTime;

            // Boom elevation (R / F)
            float boomInput = 0f;
            if (Input.GetKey(KeyCode.R)) boomInput += 1f;
            if (Input.GetKey(KeyCode.F)) boomInput -= 1f;
            currentBoomAngle = Mathf.Clamp(currentBoomAngle + boomInput * boomSpeed * Time.deltaTime, -55f, 20f);

            // Stick arm (T / G)
            float stickInput = 0f;
            if (Input.GetKey(KeyCode.T)) stickInput += 1f;
            if (Input.GetKey(KeyCode.G)) stickInput -= 1f;
            currentStickAngle = Mathf.Clamp(currentStickAngle + stickInput * stickSpeed * Time.deltaTime, 5f, 115f);

            // Bucket tilt (Space / C)
            float bucketInput = 0f;
            if (Input.GetKey(KeyCode.Space)) bucketInput += 1f;
            if (Input.GetKey(KeyCode.C)) bucketInput -= 1f;
            currentBucketAngle = Mathf.Clamp(currentBucketAngle + bucketInput * bucketSpeed * Time.deltaTime, -50f, 85f);

            if (turret != null) turret.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
            if (boom != null) boom.localRotation = Quaternion.Euler(currentBoomAngle, 0f, 0f);
            if (stick != null) stick.localRotation = Quaternion.Euler(currentStickAngle, 0f, 0f);
            if (bucket != null) bucket.localRotation = Quaternion.Euler(currentBucketAngle, 0f, 0f);

            UpdateScoopPhysics();
        }

        private void UpdateScoopPhysics()
        {
            if (bucket == null) return;

            if (currentBucketAngle >= 5f)
            {
                Vector3 bucketCenter = bucket.position;
                Collider[] hits = Physics.OverlapSphere(bucketCenter, 1.6f);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i] == null) continue;
                    GranularItem item = hits[i].GetComponent<GranularItem>();
                    if (item != null)
                    {
                        Rigidbody irb = item.GetComponent<Rigidbody>();
                        if (irb != null && !irb.isKinematic)
                        {
                            Vector3 targetPos = bucketCenter + bucket.forward * 0.2f + Vector3.up * 0.15f;
                            Vector3 pull = targetPos - irb.position;
                            if (pull.sqrMagnitude > 0.01f)
                            {
                                irb.AddForce(pull * 22f, ForceMode.Acceleration);
                            }
                        }
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

            float vert = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vert += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vert -= 1f;

            float horiz = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horiz -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horiz += 1f;

            float clampedVy = Mathf.Min(0f, rb.linearVelocity.y);
            if (transform.position.y > 0.35f)
            {
                transform.position = new Vector3(transform.position.x, 0.22f, transform.position.z);
                clampedVy = 0f;
            }

            Vector3 moveTarget = transform.forward * (vert * driveSpeed);
            rb.linearVelocity = new Vector3(moveTarget.x, clampedVy, moveTarget.z);

            if (Mathf.Abs(horiz) > 0.01f)
            {
                float turnDir = vert < -0.05f ? -horiz : horiz;
                Quaternion turn = Quaternion.Euler(0f, turnDir * steerSpeed * Time.fixedDeltaTime, 0f);
                rb.MoveRotation(rb.rotation * turn);
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (isPlayerInside)
            {
                string info = "W / S — Движение гусениц вперед / назад  |  A / D — Руление\nQ / E — Поворот башни  |  F / R — Стрела вверх / вниз\nT / G — Рукоять  |  Пробел / C — Ковш зачерпнуть / высыпать";
                DrawVehicleHUD(info);
            }
        }
    }
}
