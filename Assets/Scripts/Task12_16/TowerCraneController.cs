using UnityEngine;

namespace Task12_16
{
    public class TowerCraneController : VehicleBase
    {
        public enum CraneCameraMode
        {
            Overview = 0,    // 3-е лицо: панорамный обзор всего крана, стрелы, каретки и стройки
            HookFollow = 1,  // Слежение за крюком: камера следует за крюком сверху под углом с зумом
            TopDown = 2,     // Вид сверху на крюк (ортогонально/отвесно для точного прицеливания)
            Cabin = 3        // Кабина машиниста (1-е лицо с обзором мышью во все стороны и вниз)
        }

        public Transform jib;
        public Transform trolley;
        public Transform hook;
        public LineRenderer cableRenderer;
        public Camera hookCamera; // Сохранен для совместимости

        public CraneCameraMode cameraMode = CraneCameraMode.Overview;

        public float slewSpeed = 24f;
        public float trolleySpeed = 6.5f;
        public float winchSpeed = 5.5f;

        public float currentJibYaw = 0f;
        public float currentTrolleyDist = 8f;
        public float minTrolleyDist = 3.5f;
        public float maxTrolleyDist = 24f;
        public float currentCableLength = 12f;
        public float minCableLength = 2.5f;
        public float maxCableLength = 22.5f;

        public Rigidbody attachedCargo = null;
        public float cargoGrabRadius = 2.8f;

        // Настройки камеры
        public float mouseSensitivity = 2.4f;
        public float zoomSensitivity = 6.0f;

        // Режим 1: Overview (3-е лицо)
        public float overviewYaw = 0f;
        public float overviewPitch = 36f;
        public float overviewDist = 24f;

        // Режим 2: Hook Follow
        public float hookYaw = 0f;
        public float hookPitch = 42f;
        public float hookDist = 8.5f;

        // Режим 3: Top Down
        public float topDownHeight = 16f;

        // Режим 4: Cabin
        public float cabinLookYaw = 0f;
        public float cabinLookPitch = 38f;

        private Texture2D reticleTexture;

        protected override void Start()
        {
            vehicleName = "Башенный кран";
            base.Start();

            if (cableRenderer == null && trolley != null)
            {
                cableRenderer = trolley.GetComponent<LineRenderer>();
                if (cableRenderer == null) cableRenderer = trolley.gameObject.AddComponent<LineRenderer>();
            }

            if (cableRenderer != null)
            {
                cableRenderer.positionCount = 2;
                cableRenderer.startWidth = 0.035f;
                cableRenderer.endWidth = 0.035f;
                cableRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                cableRenderer.material.color = new Color(0.2f, 0.2f, 0.22f);
            }

            if (hookCamera != null)
            {
                hookCamera.enabled = false;
                AudioListener al = hookCamera.GetComponent<AudioListener>();
                if (al != null) al.enabled = false;
            }

            CreateReticleTexture();
        }

        private void CreateReticleTexture()
        {
            int size = 32;
            reticleTexture = new Texture2D(size, size, TextureFormat.ARGB32, false);
            Color clear = new Color(0, 0, 0, 0);
            Color reticleColor = new Color(1f, 0.9f, 0.2f, 0.85f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    reticleTexture.SetPixel(x, y, clear);
                }
            }

            int mid = size / 2;
            for (int i = 4; i < mid - 2; i++)
            {
                reticleTexture.SetPixel(mid, mid + i, reticleColor);
                reticleTexture.SetPixel(mid, mid - i, reticleColor);
                reticleTexture.SetPixel(mid + i, mid, reticleColor);
                reticleTexture.SetPixel(mid - i, mid, reticleColor);
            }
            reticleTexture.Apply();
        }

        public override void EnterVehicle()
        {
            base.EnterVehicle();

            cameraMode = CraneCameraMode.Overview;
            overviewYaw = 0f;
            overviewPitch = 36f;
            overviewDist = 24f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public override void ExitVehicle()
        {
            if (attachedCargo != null)
            {
                attachedCargo.isKinematic = false;
                attachedCargo = null;
            }
            if (hookCamera != null) hookCamera.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            base.ExitVehicle();
        }

        protected override void Update()
        {
            base.Update();

            if (!isPlayerInside) return;

            // Управление стрелой (A / D или стрелки влево/вправо)
            float slewInput = 0f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) slewInput -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) slewInput += 1f;
            currentJibYaw += slewInput * slewSpeed * Time.deltaTime;

            // Управление кареткой (W / S или стрелки вверх/вниз)
            float trolleyInput = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) trolleyInput += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) trolleyInput -= 1f;
            currentTrolleyDist = Mathf.Clamp(currentTrolleyDist + trolleyInput * trolleySpeed * Time.deltaTime, minTrolleyDist, maxTrolleyDist);

            // Управление лебедкой: R — опустить крюк, F — поднять крюк
            float winchInput = 0f;
            if (Input.GetKey(KeyCode.R)) winchInput += 1f; // опустить крюк (удлинить трос)
            if (Input.GetKey(KeyCode.F)) winchInput -= 1f; // поднять крюк (смотать трос)
            currentCableLength = Mathf.Clamp(currentCableLength + winchInput * winchSpeed * Time.deltaTime, minCableLength, maxCableLength);

            if (jib != null) jib.localRotation = Quaternion.Euler(0f, currentJibYaw, 0f);
            if (trolley != null) trolley.localPosition = new Vector3(0f, 0f, currentTrolleyDist);
            if (hook != null && trolley != null)
            {
                hook.position = trolley.position + Vector3.down * currentCableLength;
                hook.rotation = Quaternion.Euler(0f, currentJibYaw, 0f);
            }

            if (cableRenderer != null && trolley != null && hook != null)
            {
                cableRenderer.SetPosition(0, trolley.position);
                cableRenderer.SetPosition(1, hook.position);
            }

            // Захват/отцепка груза по пробелу
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleCargoHook();
            }

            if (attachedCargo != null && hook != null)
            {
                attachedCargo.position = hook.position + Vector3.down * 0.85f;
                attachedCargo.rotation = hook.rotation;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Переключение режима камеры: C, V, Tab или цифры 1-4
            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.V) || Input.GetKeyDown(KeyCode.Tab))
            {
                cameraMode = (CraneCameraMode)(((int)cameraMode + 1) % 4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) cameraMode = CraneCameraMode.Overview;
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) cameraMode = CraneCameraMode.HookFollow;
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) cameraMode = CraneCameraMode.TopDown;
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) cameraMode = CraneCameraMode.Cabin;

            // Блокировка/разблокировка курсора
            if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            // Мышь для обзора (когда курсор заблокирован или зажат ПКМ)
            float mouseX = 0f;
            float mouseY = 0f;
            bool canLook = (Cursor.lockState == CursorLockMode.Locked) || Input.GetMouseButton(1);
            if (canLook)
            {
                try
                {
                    mouseX = Input.GetAxis("Mouse X");
                    mouseY = Input.GetAxis("Mouse Y");
                }
                catch { }
            }

            float scroll = 0f;
            try
            {
                scroll = Input.GetAxis("Mouse ScrollWheel");
            }
            catch { }

            switch (cameraMode)
            {
                case CraneCameraMode.Overview:
                    overviewYaw += mouseX * mouseSensitivity;
                    overviewPitch = Mathf.Clamp(overviewPitch - mouseY * mouseSensitivity, 10f, 82f);
                    overviewDist = Mathf.Clamp(overviewDist - scroll * zoomSensitivity, 10f, 48f);
                    break;

                case CraneCameraMode.HookFollow:
                    hookYaw += mouseX * mouseSensitivity;
                    hookPitch = Mathf.Clamp(hookPitch - mouseY * mouseSensitivity, 15f, 85f);
                    hookDist = Mathf.Clamp(hookDist - scroll * zoomSensitivity, 3.5f, 22f);
                    break;

                case CraneCameraMode.TopDown:
                    topDownHeight = Mathf.Clamp(topDownHeight - scroll * (zoomSensitivity * 1.5f), 6f, 32f);
                    break;

                case CraneCameraMode.Cabin:
                    cabinLookYaw = Mathf.Clamp(cabinLookYaw + mouseX * mouseSensitivity, -95f, 95f);
                    cabinLookPitch = Mathf.Clamp(cabinLookPitch - mouseY * mouseSensitivity, -25f, 82f);
                    break;
            }
        }

        protected virtual void LateUpdate()
        {
            if (!isPlayerInside || vehicleCamera == null) return;

            switch (cameraMode)
            {
                case CraneCameraMode.Overview:
                    UpdateOverviewCamera();
                    break;

                case CraneCameraMode.HookFollow:
                    UpdateHookFollowCamera();
                    break;

                case CraneCameraMode.TopDown:
                    UpdateTopDownCamera();
                    break;

                case CraneCameraMode.Cabin:
                    UpdateCabinCamera();
                    break;
            }
        }

        private void UpdateOverviewCamera()
        {
            Vector3 focusPoint = trolley != null ? Vector3.Lerp(trolley.position, hook != null ? hook.position : trolley.position, 0.45f) : transform.position + Vector3.up * 18f;

            Quaternion rot = Quaternion.Euler(overviewPitch, currentJibYaw + overviewYaw, 0f);
            Vector3 camPos = focusPoint + rot * new Vector3(0f, 0f, -overviewDist);

            vehicleCamera.transform.position = camPos;
            vehicleCamera.transform.LookAt(focusPoint);
        }

        private void UpdateHookFollowCamera()
        {
            Vector3 target = hook != null ? hook.position : (trolley != null ? trolley.position : transform.position);

            Quaternion rot = Quaternion.Euler(hookPitch, currentJibYaw + hookYaw, 0f);
            Vector3 camPos = target + rot * new Vector3(0f, 0f, -hookDist);

            vehicleCamera.transform.position = camPos;
            vehicleCamera.transform.LookAt(target + Vector3.up * 0.35f);
        }

        private void UpdateTopDownCamera()
        {
            Vector3 center = trolley != null ? trolley.position : (hook != null ? hook.position : transform.position);
            Vector3 camPos = center + Vector3.up * topDownHeight;

            vehicleCamera.transform.position = camPos;
            vehicleCamera.transform.rotation = Quaternion.Euler(90f, currentJibYaw, 0f);
        }

        private void UpdateCabinCamera()
        {
            Vector3 cabEyePos;
            if (jib != null)
            {
                cabEyePos = jib.TransformPoint(new Vector3(1.3f, 1.35f, 1.2f));
            }
            else
            {
                cabEyePos = transform.position + Vector3.up * 24.5f;
            }

            vehicleCamera.transform.position = cabEyePos;
            vehicleCamera.transform.rotation = Quaternion.Euler(cabinLookPitch, currentJibYaw + cabinLookYaw, 0f);
        }

        private void ToggleCargoHook()
        {
            if (attachedCargo != null)
            {
                attachedCargo.isKinematic = false;
                attachedCargo.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                attachedCargo.linearVelocity = Vector3.zero;
                attachedCargo = null;
                return;
            }

            if (hook == null) return;

            Collider[] hits = Physics.OverlapSphere(hook.position, cargoGrabRadius);
            float closestDist = float.MaxValue;
            Rigidbody bestRb = null;

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null) continue;
                if (hits[i].transform.IsChildOf(transform)) continue;

                Rigidbody r = hits[i].attachedRigidbody;
                if (r == null) continue;
                if (r.GetComponent<VehicleBase>() != null) continue; // Never grab vehicles!

                bool isCargo = r.name.StartsWith("Cargo_") 
                    || r.GetComponent<GranularItem>() != null 
                    || r.GetComponent<Task8_11.PickupableItem>() != null;
                if (!isCargo) continue;

                float d = Vector3.Distance(hook.position, r.worldCenterOfMass);
                if (d < closestDist)
                {
                    closestDist = d;
                    bestRb = r;
                }
            }

            if (bestRb != null)
            {
                attachedCargo = bestRb;
                attachedCargo.isKinematic = true;
            }
        }

        public bool HasCargoNearby()
        {
            if (hook == null || attachedCargo != null) return false;
            Collider[] hits = Physics.OverlapSphere(hook.position, cargoGrabRadius);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null) continue;
                if (hits[i].transform.IsChildOf(transform)) continue;
                Rigidbody r = hits[i].attachedRigidbody;
                if (r == null) continue;
                if (r.GetComponent<VehicleBase>() != null) continue; // Never grab vehicles!

                bool isCargo = r.name.StartsWith("Cargo_") 
                    || r.GetComponent<GranularItem>() != null 
                    || r.GetComponent<Task8_11.PickupableItem>() != null;
                if (isCargo) return true;
            }
            return false;
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (isPlayerInside)
            {
                string status = "КРЮК СВОБОДЕН";
                if (attachedCargo != null)
                {
                    status = "ГРУЗ ПОДВЕШЕН: " + attachedCargo.name + " ([Пробел] — отцепить)";
                }
                else if (HasCargoNearby())
                {
                    status = "ГРУЗ РЯДОМ! ([Пробел] — захватить)";
                }

                string modeName = "";
                switch (cameraMode)
                {
                    case CraneCameraMode.Overview: modeName = "1. ОБЗОР (3-е лицо)"; break;
                    case CraneCameraMode.HookFollow: modeName = "2. СЛЕЖЕНИЕ ЗА КРЮКОМ"; break;
                    case CraneCameraMode.TopDown: modeName = "3. СВЕРХУ НА КРЮК"; break;
                    case CraneCameraMode.Cabin: modeName = "4. ИЗ КАБИНЫ"; break;
                }

                string camBar = "РЕЖИМ КАМЕРЫ [C / 1-4]: " + modeName + "  |  Мышь / ПКМ — Обзор  |  Колесико — Зум";
                string controlBar = "A / D — Стрела  |  W / S — Каретка  |  F / R — Поднять / Опустить крюк  |  Пробел — Захват/отцепка";
                string telemetry = "Высота крюка: " + (24f - currentCableLength).ToString("F1") + " м  |  Вылет: " + currentTrolleyDist.ToString("F1") + " м  |  " + status;

                DrawVehicleHUD(controlBar + "\n" + camBar + "\n" + telemetry);

                if ((cameraMode == CraneCameraMode.TopDown || cameraMode == CraneCameraMode.HookFollow) && reticleTexture != null)
                {
                    float rx = Screen.width * 0.5f - 16f;
                    float ry = Screen.height * 0.5f - 16f;
                    GUI.DrawTexture(new Rect(rx, ry, 32f, 32f), reticleTexture);
                }
            }
        }
    }
}
