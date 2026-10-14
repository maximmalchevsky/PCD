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

        // Smooth Mouse Orbit Camera & Cab View
        public float cameraDistance = 7.5f;
        public float minCameraDistance = 3.5f;
        public float maxCameraDistance = 16f;
        public float cameraYaw = 35f;
        public float cameraPitch = 22f;
        public float mouseSensitivity = 2.8f;
        public bool isCabView = false;

        private Rigidbody rb;
        private int totalExcavatedChunks = 0;
        private float digDustCooldown = 0f;
        private float ignoreCollisionTimer = 0f;

        // Bucket tooth probe points in local bucket space
        private readonly float[] toothXOffsets = new float[] { -0.60f, -0.30f, 0f, 0.30f, 0.60f };
        private const float TOOTH_LOCAL_Y = -0.44f;
        private const float TOOTH_LOCAL_Z = 0.95f;

        public bool isFbxRig = false;
        private Quaternion turretStartRelRot = Quaternion.identity;
        private Quaternion boomInitialRot = Quaternion.identity;
        private Quaternion stickInitialRot = Quaternion.identity;
        private Quaternion bucketInitialRot = Quaternion.identity;
        private Transform[] fbxTeeth;

        public Transform boomCylinderLeft;
        public Transform boomCylinderRight;
        public Transform boomCylinderLeftTarget;
        public Transform boomCylinderRightTarget;
        public Transform armCylinder;
        public Transform armCylinderTarget;
        public Transform bucketCylinder;
        public Transform bucketCylinderTarget;

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
                // Keep upright (prevent tipping/flipping), but allow Y motion with gravity!
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                rb.isKinematic = false;
            }

            if (turret != null) turretStartRelRot = Quaternion.Inverse(transform.rotation) * turret.rotation;
            if (boom != null) boomInitialRot = boom.localRotation;
            if (stick != null) stickInitialRot = stick.localRotation;
            if (bucket != null) bucketInitialRot = bucket.localRotation;

            if (isFbxRig && bucket != null)
            {
                var teethList = new System.Collections.Generic.List<Transform>();
                for (int b = 1; b <= 10; b++)
                {
                    string bName = "Bolt." + b.ToString("D3");
                    Transform t = FindDeep(bucket, bName);
                    if (t != null) teethList.Add(t);
                }
                if (teethList.Count > 0) fbxTeeth = teethList.ToArray();
            }

            if (chassisCollider == null) chassisCollider = GetComponent<Collider>();
            IgnoreCargoCollisions();

            UpdateJointRotations();
        }

        private static Transform FindDeep(Transform parent, string name)
        {
            if (parent == null) return null;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase)) return child;
                Transform deep = FindDeep(child, name);
                if (deep != null) return deep;
            }
            return null;
        }

        private void UpdateJointRotations()
        {
            if (isFbxRig)
            {
                if (turret != null) turret.rotation = transform.rotation * Quaternion.Euler(0f, currentTurretYaw, 0f) * turretStartRelRot;
                if (boom != null) boom.localRotation = boomInitialRot * Quaternion.Euler(currentBoomAngle, 0f, 0f);
                if (stick != null) stick.localRotation = stickInitialRot * Quaternion.Euler(-currentStickAngle, 0f, 0f);
                if (bucket != null) bucket.localRotation = bucketInitialRot * Quaternion.Euler(currentBucketAngle, 0f, 0f);

                // Update hydraulic cylinder look-at targets for photorealistic motion
                if (boomCylinderLeft != null && boomCylinderLeftTarget != null) boomCylinderLeft.LookAt(boomCylinderLeftTarget.position, transform.up);
                if (boomCylinderRight != null && boomCylinderRightTarget != null) boomCylinderRight.LookAt(boomCylinderRightTarget.position, transform.up);
                if (armCylinder != null && armCylinderTarget != null) armCylinder.LookAt(armCylinderTarget.position, transform.up);
                if (bucketCylinder != null && bucketCylinderTarget != null) bucketCylinder.LookAt(bucketCylinderTarget.position, transform.up);
            }
            else
            {
                if (turret != null) turret.localRotation = Quaternion.Euler(0f, currentTurretYaw, 0f);
                if (boom != null) boom.localRotation = Quaternion.Euler(currentBoomAngle, 0f, 0f);
                if (stick != null) stick.localRotation = Quaternion.Euler(currentStickAngle, 0f, 0f);
                if (bucket != null) bucket.localRotation = Quaternion.Euler(currentBucketAngle, 0f, 0f);
            }
        }

        public override void EnterVehicle()
        {
            base.EnterVehicle();

            cameraYaw = transform.eulerAngles.y + currentTurretYaw + 30f;
            cameraPitch = 22f;
            cameraDistance = 7.5f;
            isCabView = false;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            IgnoreCargoCollisions();
        }

        public override void ExitVehicle()
        {
            base.ExitVehicle();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void IgnoreCargoCollisions()
        {
            Collider[] vehicleCols = GetComponentsInChildren<Collider>();
            var items = Object.FindObjectsByType<GranularItem>(FindObjectsInactive.Exclude);

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                Collider itemCol = items[i].GetComponent<Collider>();
                if (itemCol == null) continue;

                for (int v = 0; v < vehicleCols.Length; v++)
                {
                    Collider vc = vehicleCols[v];
                    if (vc == null) continue;
                    // Keep bucket colliders active so bucket can scoop cargo!
                    if (bucket != null && vc.transform.IsChildOf(bucket)) continue;

                    Physics.IgnoreCollision(vc, itemCol, true);
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if (!isPlayerInside) return;

            // Periodically ignore collision with newly excavated or dropped cargo
            ignoreCollisionTimer += Time.deltaTime;
            if (ignoreCollisionTimer > 1.2f)
            {
                ignoreCollisionTimer = 0f;
                IgnoreCargoCollisions();
            }

            // View toggle (V key): Third-person orbit <-> First-person cabin view
            if (Input.GetKeyDown(KeyCode.V))
            {
                isCabView = !isCabView;
            }

            // 1. Turret rotation (Q / E or Left / Right Arrows)
            float yawInput = 0f;
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) yawInput -= 1f;
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.RightArrow)) yawInput += 1f;
            currentTurretYaw += yawInput * turretSpeed * Time.deltaTime;

            // 2. Boom elevation (R / F or Up / Down Arrows)
            float boomInput = 0f;
            if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.UpArrow)) boomInput += 1f;
            if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.DownArrow)) boomInput -= 1f;
            currentBoomAngle = Mathf.Clamp(currentBoomAngle + boomInput * boomSpeed * Time.deltaTime, -58f, 22f);

            // 3. Stick arm (T / G or I / K)
            float stickInput = 0f;
            if (Input.GetKey(KeyCode.T) || Input.GetKey(KeyCode.I)) stickInput += 1f;
            if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.K)) stickInput -= 1f;
            currentStickAngle = Mathf.Clamp(currentStickAngle + stickInput * stickSpeed * Time.deltaTime, 5f, 118f);

            // 4. Bucket tilt (Space / C or J / L)
            float bucketInput = 0f;
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J)) bucketInput += 1f;
            if (Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.L)) bucketInput -= 1f;
            currentBucketAngle = Mathf.Clamp(currentBucketAngle + bucketInput * bucketSpeed * Time.deltaTime, -52f, 88f);

            UpdateJointRotations();

            // Perform real-time ground digging and scooping
            UpdateGroundDigging(boomInput, stickInput, bucketInput);
            UpdateScoopCargoRetention();
        }

        protected virtual void LateUpdate()
        {
            if (!isPlayerInside || vehicleCamera == null) return;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (isCabView)
            {
                // First-Person Operator Cabin View (Inside Cab at eye level)
                Vector3 cabEyePos = (turret != null)
                    ? turret.TransformPoint(new Vector3(-0.96f, 1.45f, 0.70f))
                    : transform.TransformPoint(new Vector3(-0.96f, 2.25f, 0.70f));

                cameraYaw += mouseX * mouseSensitivity * 40f * Time.deltaTime;
                cameraPitch -= mouseY * mouseSensitivity * 40f * Time.deltaTime;
                cameraPitch = Mathf.Clamp(cameraPitch, -35f, 50f);

                vehicleCamera.transform.position = cabEyePos;
                float turretBaseYaw = (turret != null) ? turret.eulerAngles.y : transform.eulerAngles.y;
                vehicleCamera.transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
            }
            else
            {
                // Third-Person Smooth Mouse Orbit Camera
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

                // Ground clamp so camera never clips into dirt or trenches
                if (targetCamPos.y < 0.65f) targetCamPos.y = 0.65f;

                vehicleCamera.transform.position = Vector3.Lerp(vehicleCamera.transform.position, targetCamPos, Time.deltaTime * 20f);
                vehicleCamera.transform.rotation = Quaternion.LookRotation(focusPoint - vehicleCamera.transform.position);
            }
        }

        private void UpdateGroundDigging(float boomInput, float stickInput, float bucketInput)
        {
            if (bucket == null || DiggableEarth.Instance == null) return;

            // Digging motion is active when curling bucket, pulling stick, lowering boom, or pressing Space/J/G/F
            bool isDiggingMotion = bucketInput > 0.05f || stickInput < -0.05f || boomInput < -0.05f ||
                                   Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.F);

            digDustCooldown -= Time.deltaTime;

            if (isFbxRig && fbxTeeth != null && fbxTeeth.Length > 0)
            {
                for (int t = 0; t < fbxTeeth.Length; t++)
                {
                    if (fbxTeeth[t] == null) continue;
                    ProcessDiggingPoint(fbxTeeth[t].position, isDiggingMotion);
                }
            }
            else
            {
                for (int t = 0; t < toothXOffsets.Length; t++)
                {
                    Vector3 toothLocal = new Vector3(toothXOffsets[t], TOOTH_LOCAL_Y, TOOTH_LOCAL_Z);
                    Vector3 toothWorld = bucket.TransformPoint(toothLocal);
                    ProcessDiggingPoint(toothWorld, isDiggingMotion);
                }
            }
        }

        private void ProcessDiggingPoint(Vector3 toothWorld, bool isDiggingMotion)
        {
            float groundWorldY = DiggableEarth.Instance.GetHeightAtWorld(toothWorld);

            if (isDiggingMotion || toothWorld.y <= groundWorldY + 0.15f)
            {
                float digDepth = toothWorld.y - 0.12f;
                if (DiggableEarth.Instance.DigAt(toothWorld, 0.85f, digDepth, out float volumeRemoved))
                {
                    if (digDustCooldown <= 0f)
                    {
                        EmitDiggingDust(toothWorld);
                        digDustCooldown = 0.10f;
                    }

                    if (DiggableEarth.Instance.accumulatedSoilVolume >= DiggableEarth.VOLUME_PER_SOIL_CHUNK)
                    {
                        DiggableEarth.Instance.accumulatedSoilVolume -= DiggableEarth.VOLUME_PER_SOIL_CHUNK;
                        totalExcavatedChunks++;

                        Vector3 spawnPos = toothWorld - bucket.forward * 0.40f + bucket.up * 0.30f;
                        Vector3 initialVel = -bucket.forward * 1.8f + Vector3.up * 1.0f;
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
            if (Input.GetKey(KeyCode.W)) vert += 1f;
            if (Input.GetKey(KeyCode.S)) vert -= 1f;

            float horiz = 0f;
            if (Input.GetKey(KeyCode.A)) horiz -= 1f;
            if (Input.GetKey(KeyCode.D)) horiz += 1f;

            Vector3 moveTarget = transform.forward * (vert * driveSpeed);
            float currentYVel = rb.linearVelocity.y;
            currentYVel = Mathf.Max(currentYVel - 9.81f * Time.fixedDeltaTime, -15f);
            rb.linearVelocity = new Vector3(moveTarget.x, currentYVel, moveTarget.z);

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
                string viewMode = isCabView ? "[Кабина]" : "[3D Орбита]";
                string info = "W / S — Гусеницы вперед / назад  |  A / D — Разворот гусениц\n" +
                              "Q / E (или \u2190 / \u2192) — Поворот башни (360\u00B0)  |  R / F (или \u2191 / \u2193) — Стрела поднять / опустить\n" +
                              "T / G (или I / K) — Рукоять вытянуть / подгрести  |  Пробел (или J) — Копать  |  C (или L) — Высыпать\n" +
                              "Мышь — Обзор 360\u00B0  |  V — Вид из кабины / орбита " + viewMode + "  |  Колесико — Zoom\n" +
                              "Выкопано грунта: " + totalExcavatedChunks + " комьев  |  [Esc / X] — Выйти из кабины";
                DrawVehicleHUD(info);
            }
        }
    }
}
