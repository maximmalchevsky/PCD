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
        public float driveSpeed = 5.5f;
        public float steerSpeed = 44f;

        public float currentTurretYaw = 0f;
        public float currentBoomAngle = -28f;
        public float currentStickAngle = 32f;
        public float currentBucketAngle = 18f;

        private Rigidbody rb;
        private int totalExcavatedChunks = 0;
        private float digDustCooldown = 0f;

        // Bucket tooth probe points in local bucket space
        private readonly float[] toothXOffsets = new float[] { -0.62f, -0.31f, 0f, 0.31f, 0.62f };
        private const float TOOTH_LOCAL_Y = -0.44f;
        private const float TOOTH_LOCAL_Z = 0.88f;

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

        private void UpdateGroundDigging(float boomInput, float stickInput, float bucketInput)
        {
            if (bucket == null || DiggableEarth.Instance == null) return;

            // Check if digging motion is engaged:
            // curling bucket (Space), pulling stick (G), or lowering boom (F)
            bool isDiggingMotion = bucketInput > 0.05f || stickInput < -0.05f || boomInput < -0.05f;

            digDustCooldown -= Time.deltaTime;

            for (int t = 0; t < toothXOffsets.Length; t++)
            {
                Vector3 toothLocal = new Vector3(toothXOffsets[t], TOOTH_LOCAL_Y, TOOTH_LOCAL_Z);
                Vector3 toothWorld = bucket.TransformPoint(toothLocal);

                // Check depth against diggable terrain
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

                        // Check if accumulated volume warrants spawning a newly dug physical soil chunk
                        if (DiggableEarth.Instance.accumulatedSoilVolume >= DiggableEarth.VOLUME_PER_SOIL_CHUNK)
                        {
                            DiggableEarth.Instance.accumulatedSoilVolume -= DiggableEarth.VOLUME_PER_SOIL_CHUNK;
                            totalExcavatedChunks++;

                            // Spawn chunk into the hollow of the bucket scoop
                            Vector3 spawnPos = toothWorld - bucket.forward * 0.35f + bucket.up * 0.25f;
                            Vector3 initialVel = -bucket.forward * 1.6f + Vector3.up * 0.9f;
                            DiggableEarth.Instance.SpawnExcavatedSoilChunk(spawnPos, initialVel);
                        }
                    }
                }
            }
        }

        private void EmitDiggingDust(Vector3 position)
        {
            // Subtle visual feedback: creates dynamic small earth particles kicked up by teeth
            GameObject dustObj = new GameObject("Dirt_Puff");
            dustObj.transform.position = position;
            Destroy(dustObj, 0.6f);
        }

        private void UpdateScoopCargoRetention()
        {
            if (bucket == null) return;

            // When bucket is curled upright, retain loose soil chunks and bricks inside the scoop
            if (currentBucketAngle >= 8f)
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
                string info = "W / S — Движение гусениц вперед / назад  |  A / D — Разворот гусениц\n" +
                              "Q / E — Вращение башни (360°)  |  F / R — Стрела опустить / поднять\n" +
                              "G / T — Рукоять подгрести / вытянуть  |  Пробел — Копать грунт (зачерпнуть)  |  C — Высыпать\n" +
                              "Выкопано грунта: " + totalExcavatedChunks + " комьев  |  [Esc / X] — Выйти из кабины";
                DrawVehicleHUD(info);
            }
        }
    }
}
