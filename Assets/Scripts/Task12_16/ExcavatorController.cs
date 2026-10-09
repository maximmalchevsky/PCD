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
        public float driveSpeed = 4.2f;
        public float steerSpeed = 32f;

        public float currentTurretYaw = 0f;
        public float currentBoomAngle = -15f;
        public float currentStickAngle = 45f;
        public float currentBucketAngle = 20f;

        private Rigidbody rb;

        protected override void Start()
        {
            vehicleName = "Экскаватор";
            base.Start();

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 16000f;
                rb.linearDamping = 2.0f;
                rb.angularDamping = 3.5f;
                rb.centerOfMass = new Vector3(0f, -0.4f, 0f);
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!isPlayerInside) return;

            float yawInput = 0f;
            if (Input.GetKey(KeyCode.A)) yawInput -= 1f;
            if (Input.GetKey(KeyCode.D)) yawInput += 1f;
            currentTurretYaw += yawInput * turretSpeed * Time.deltaTime;

            float boomInput = 0f;
            if (Input.GetKey(KeyCode.W)) boomInput -= 1f;
            if (Input.GetKey(KeyCode.S)) boomInput += 1f;
            currentBoomAngle = Mathf.Clamp(currentBoomAngle + boomInput * boomSpeed * Time.deltaTime, -48f, 15f);

            float stickInput = 0f;
            if (Input.GetKey(KeyCode.Q)) stickInput += 1f;
            if (Input.GetKey(KeyCode.Z)) stickInput -= 1f;
            currentStickAngle = Mathf.Clamp(currentStickAngle + stickInput * stickSpeed * Time.deltaTime, 10f, 105f);

            float bucketInput = 0f;
            if (Input.GetKey(KeyCode.R)) bucketInput += 1f;
            if (Input.GetKey(KeyCode.F)) bucketInput -= 1f;
            currentBucketAngle = Mathf.Clamp(currentBucketAngle + bucketInput * bucketSpeed * Time.deltaTime, -45f, 75f);

            if (turret != null) turret.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
            if (boom != null) boom.localRotation = Quaternion.Euler(currentBoomAngle, 0f, 0f);
            if (stick != null) stick.localRotation = Quaternion.Euler(currentStickAngle, 0f, 0f);
            if (bucket != null) bucket.localRotation = Quaternion.Euler(currentBucketAngle, 0f, 0f);

            UpdateScoopPhysics();
        }

        private void UpdateScoopPhysics()
        {
            if (bucket == null) return;

            if (currentBucketAngle >= 10f)
            {
                Vector3 bucketCenter = bucket.position;
                Collider[] hits = Physics.OverlapSphere(bucketCenter, 1.35f);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i] == null) continue;
                    GranularItem item = hits[i].GetComponent<GranularItem>();
                    if (item != null)
                    {
                        Rigidbody irb = item.GetComponent<Rigidbody>();
                        if (irb != null && !irb.isKinematic)
                        {
                            Vector3 targetPos = bucketCenter + bucket.forward * 0.15f;
                            Vector3 pull = targetPos - irb.position;
                            if (pull.sqrMagnitude > 0.02f)
                            {
                                irb.AddForce(pull * 16f, ForceMode.Acceleration);
                            }
                        }
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (!isPlayerInside || rb == null) return;

            float trackVert = 0f;
            if (Input.GetKey(KeyCode.UpArrow)) trackVert += 1f;
            if (Input.GetKey(KeyCode.DownArrow)) trackVert -= 1f;

            float trackHoriz = 0f;
            if (Input.GetKey(KeyCode.LeftArrow)) trackHoriz -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) trackHoriz += 1f;

            Vector3 moveTarget = transform.forward * (trackVert * driveSpeed);
            rb.linearVelocity = new Vector3(moveTarget.x, rb.linearVelocity.y, moveTarget.z);

            if (Mathf.Abs(trackHoriz) > 0.01f)
            {
                Quaternion turn = Quaternion.Euler(0f, trackHoriz * steerSpeed * Time.fixedDeltaTime, 0f);
                rb.MoveRotation(rb.rotation * turn);
            }
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (isPlayerInside)
            {
                string info = "A / D — Поворот платформы  |  W / S — Подъем / спуск стрелы\nQ / Z — Выдвижение / втягивание рукояти  |  R / F — Зачерпнуть / высыпать ковш\nСтрелки ↑ ↓ ← → — Движение гусеничной базы";
                DrawVehicleHUD(info);
            }
        }
    }
}
