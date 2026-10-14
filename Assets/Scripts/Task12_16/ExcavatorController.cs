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
        public Collider chassisCollider;

        public float turretSpeed = 38f;
        public float boomSpeed = 28f;
        public float stickSpeed = 32f;
        public float bucketSpeed = 42f;
        public float driveSpeed = 5.5f;
        public float steerSpeed = 44f;

        public float currentTurretYaw = 0f;
        public float currentBoomAngle = -15f;
        public float currentStickAngle = 25f;
        public float currentBucketAngle = 10f;

        // Smooth Mouse Orbit Camera
        public float cameraDistance = 7.5f;
        public float minCameraDistance = 3.5f;
        public float maxCameraDistance = 16f;
        public float cameraYaw = 35f;
        public float cameraPitch = 22f;
        public float mouseSensitivity = 2.8f;

        private Rigidbody rb;
        private int totalExcavatedChunks = 0;
        private float digDustCooldown = 0f;

        // Bucket tooth probe points in local bucket space
        private readonly float[] toothXOffsets = new float[] { -0.60f, -0.30f, 0f, 0.30f, 0.60f };
        private const float TOOTH_LOCAL_Y = -0.44f;
        private const float TOOTH_LOCAL_Z = 0.95f;

        protected override void Start()
        {
            vehicleName = "Гусеничный экскаватор";
            base.Start();

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.mass = 22000f;
                rb.linearDamping = 1.8f;
                rb.angularDamping = 3.5f;
                rb.centerOfMass = new Vector3(0f, -0.4f, 0f);
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = true;
            }

            if (chassisCollider == null) chassisCollider = GetComponent<Collider>();
            IgnoreCargoCollisions();

            if (turret != null) turret.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
            if (boom != null) boom.localRotation = Quaternion.Euler(currentBoomAngle, 0f, 0f);
            if (stick != null) stick.localRotation = Quaternion.Euler(currentStickAngle, 0f, 0f);
            if (bucket != null) bucket.localRotation = Quaternion.Euler(currentBucketAngle, 0f, 0f);
        }

        public override void EnterVehicle()
        {
            base.EnterVehicle();

            cameraYaw = transform.eulerAngles.y + currentTurretYaw + 30f;
            cameraPitch = 22f;
            cameraDistance = 7.5f;

            IgnoreCargoCollisions();
        }

        public void IgnoreCargoCollisions()
        {
            if (chassisCollider == null) chassisCollider = GetComponent<Collider>();
            if (chassisCollider == null) return;

            var items = Object.FindObjectsByType<GranularItem>(FindObjectsInactive.Exclude);
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                Collider col = items[i].GetComponent<Collider>();
                if (col != null)
                {
                    Physics.IgnoreCollision(chassisCollider, col, true);
                }
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

            // 1. Turret rotation (Q / E)
            float yawInput = 0f;
            if (Input.GetKey(KeyCode.Q)) yawInput -= 1f;
            if (Input.GetKey(KeyCode.E)) yawInput += 1f;
            currentTurretYaw += yawInput * turretSpeed * Time.deltaTime;

            // 2. Boom elevation (R: Up / F: Down into ground)
            float boomInput = 0f;
            if (Input.GetKey(KeyCode.R)) boomInput += 1f;
            if (Input.GetKey(KeyCode.F)) boomInput -= 1f;
            currentBoomAngle = Mathf.Clamp(currentBoomAngle + boomInput * boomSpeed * Time.deltaTime, -58f, 22f);

            // 3. Stick arm (T: Reach out / G: Pull in)
            float stickInput = 0f;
            if (Input.GetKey(KeyCode.T)) stickInput += 1f;
            if (Input.GetKey(KeyCode.G)) stickInput -= 1f;
            currentStickAngle = Mathf.Clamp(currentStickAngle + stickInput * stickSpeed * Time.deltaTime, 5f, 118f);

            // 4. Bucket tilt (Space: Curl/Scoop / C: Dump out)
            float bucketInput = 0f;
            if (Input.GetKey(KeyCode.Space)) bucketInput += 1f;
            if (Input.GetKey(KeyCode.C)) bucketInput -= 1f;
            currentBucketAngle = Mathf.Clamp(currentBucketAngle + bucketInput * bucketSpeed * Time.deltaTime, -52f, 88f);

            if (turret != null) turret.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
            if (boom != null) boom.localRotation = Quaternion.Euler(currentBoomAngle, 0f, 0f);
            if (stick != null) stick.localRotation = Quaternion.Euler(currentStickAngle, 0f, 0f);
            if (bucket != null) bucket.localRotation = Quaternion.Euler(currentBucketAngle, 0f, 0f);

            // Perform real-time ground digging and scooping
            UpdateGroundDigging(boomInput, stickInput, bucketInput);
            UpdateScoopCargoRetention();
        }

        protected virtual void LateUpdate()
        {
            if (!isPlayerInside || vehicleCamera == null) return;

            // Smooth Mouse Orbit Camera
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            cameraYaw += mouseX * mouseSensitivity * 45f * Time.deltaTime;
            cameraPitch -= mouseY * mouseSensitivity * 45f * Time.deltaTime;
            cameraPitch = Mathf.Clamp(cameraPitch, -2f, 75f);

            if (Mathf.Abs(scroll) > 0.001f)
            {
                cameraDistance = Mathf.Clamp(cameraDistance - scroll * 5.0f, minCameraDistance, maxCameraDistance);
            }

            Vector3 focusPoint = (turret != null) ? turret.position + Vector3.up * 1.5f : transform.position + Vector3.up * 2.2f;
            Quaternion camRot = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
            Vector3 targetCamPos = focusPoint - camRot * Vector3.forward * cameraDistance;

            // Ground clamp so camera never clips below soil
            if (targetCamPos.y < 0.65f) targetCamPos.y = 0.65f;

            vehicleCamera.transform.position = Vector3.Lerp(vehicleCamera.transform.position, targetCamPos, Time.deltaTime * 20f);
            vehicleCamera.transform.rotation = Quaternion.LookRotation(focusPoint - vehicleCamera.transform.position);
        }

        private void UpdateGroundDigging(float boomInput, float stickInput, float bucketInput)
        {
            if (bucket == null || DiggableEarth.Instance == null) return;

            // Digging motion is engaged when curling bucket (Space), pulling stick (G), or lowering boom (F)
            bool isDiggingMotion = bucketInput > 0.05f || stickInput < -0.05f || boomInput < -0.05f;

            digDustCooldown -= Time.deltaTime;

            for (int t = 0; t < toothXOffsets.Length; t++)
            {
                Vector3 toothLocal = new Vector3(toothXOffsets[t], TOOTH_LOCAL_Y, TOOTH_LOCAL_Z);
                Vector3 toothWorld = bucket.TransformPoint(toothLocal);

                if (isDiggingMotion)
                {
                    float digDepth = toothWorld.y - 0.08f;
                    if (DiggableEarth.Instance.DigAt(toothWorld, 0.65f, digDepth, out float volumeRemoved))
                    {
                        if (digDustCooldown <= 0f)
                        {
                            EmitDiggingDust(toothWorld);
                            digDustCooldown = 0.12f;
                        }

                        if (DiggableEarth.Instance.accumulatedSoilVolume >= DiggableEarth.VOLUME_PER_SOIL_CHUNK)
                        {
                            DiggableEarth.Instance.accumulatedSoilVolume -= DiggableEarth.VOLUME_PER_SOIL_CHUNK;
                            totalExcavatedChunks++;

                            Vector3 spawnPos = toothWorld - bucket.forward * 0.35f + bucket.up * 0.25f;
                            Vector3 initialVel = -bucket.forward * 1.6f + Vector3.up * 0.9f;
                            GameObject newChunk = DiggableEarth.Instance.SpawnExcavatedSoilChunk(spawnPos, initialVel);

                            if (chassisCollider != null && newChunk != null)
                            {
                                Collider col = newChunk.GetComponent<Collider>();
                                if (col != null) Physics.IgnoreCollision(chassisCollider, col, true);
                            }
                        }
                    }
                }
            }
        }

        private void EmitDiggingDust(Vector3 position)
        {
            GameObject dustObj = new GameObject("Dirt_Puff");
            dustObj.transform.position = position;
            Destroy(dustObj, 0.6f);
        }

        private void UpdateScoopCargoRetention()
        {
            if (bucket == null) return;

            if (currentBucketAngle >= 5f)
            {
                Vector3 bucketCavity = bucket.position + bucket.forward * 0.15f + bucket.up * 0.10f;
                Collider[] hits = Physics.OverlapSphere(bucketCavity, 1.4f);
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i] == null) continue;
                    GranularItem item = hits[i].GetComponent<GranularItem>();
                    if (item != null)
                    {
                        Rigidbody irb = item.GetComponent<Rigidbody>();
                        if (irb != null && !irb.isKinematic)
                        {
                            Vector3 toCavity = bucketCavity - irb.position;
                            if (toCavity.sqrMagnitude > 0.02f)
                            {
                                irb.AddForce(toCavity * 16f, ForceMode.Acceleration);
                            }
                        }
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (!isPlayerInside || rb == null) return;

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
                string info = "W / S — Движение гусениц вперед / назад  |  A / D — Разворот гусениц\n" +
                              "Q / E — Вращение башни (360°)  |  R / F — Стрела поднять / опустить\n" +
                              "T / G — Рукоять вытянуть / подгрести  |  Пробел — Зачерпнуть (копать)  |  C — Высыпать\n" +
                              "Мышь — Вращать камеру 360°  |  Колесико — Приблизить / отдалить\n" +
                              "Выкопано грунта: " + totalExcavatedChunks + " комьев  |  [Esc / X] — Выйти из кабины";
                DrawVehicleHUD(info);
            }
        }
    }
}
