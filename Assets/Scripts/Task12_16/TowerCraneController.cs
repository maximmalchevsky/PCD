using UnityEngine;

namespace Task12_16
{
    public class TowerCraneController : VehicleBase
    {
        public Transform jib;
        public Transform trolley;
        public Transform hook;
        public LineRenderer cableRenderer;
        public Camera hookCamera;

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
        public float cargoGrabRadius = 2.4f;

        private bool isUsingHookCamera = false;

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
        }

        protected override void Update()
        {
            base.Update();

            if (!isPlayerInside) return;

            float slewInput = 0f;
            if (Input.GetKey(KeyCode.A)) slewInput -= 1f;
            if (Input.GetKey(KeyCode.D)) slewInput += 1f;
            currentJibYaw += slewInput * slewSpeed * Time.deltaTime;

            float trolleyInput = 0f;
            if (Input.GetKey(KeyCode.W)) trolleyInput += 1f;
            if (Input.GetKey(KeyCode.S)) trolleyInput -= 1f;
            currentTrolleyDist = Mathf.Clamp(currentTrolleyDist + trolleyInput * trolleySpeed * Time.deltaTime, minTrolleyDist, maxTrolleyDist);

            float winchInput = 0f;
            if (Input.GetKey(KeyCode.R)) winchInput += 1f;
            if (Input.GetKey(KeyCode.F)) winchInput -= 1f;
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

            if (Input.GetKeyDown(KeyCode.C) && hookCamera != null && vehicleCamera != null)
            {
                isUsingHookCamera = !isUsingHookCamera;
                vehicleCamera.enabled = !isUsingHookCamera;
                hookCamera.enabled = isUsingHookCamera;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleCargoHook();
            }

            if (attachedCargo != null && hook != null)
            {
                attachedCargo.position = hook.position + Vector3.down * 0.85f;
                attachedCargo.rotation = hook.rotation;
            }
        }

        private void ToggleCargoHook()
        {
            if (attachedCargo != null)
            {
                attachedCargo.isKinematic = false;
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
                if (r != null && !r.isKinematic)
                {
                    float d = Vector3.Distance(hook.position, r.worldCenterOfMass);
                    if (d < closestDist)
                    {
                        closestDist = d;
                        bestRb = r;
                    }
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
                if (r != null && !r.isKinematic) return true;
            }
            return false;
        }

        public override void ExitVehicle()
        {
            if (attachedCargo != null)
            {
                attachedCargo.isKinematic = false;
                attachedCargo = null;
            }
            if (hookCamera != null) hookCamera.enabled = false;
            isUsingHookCamera = false;
            base.ExitVehicle();
        }

        protected override void OnGUI()
        {
            base.OnGUI();

            if (isPlayerInside)
            {
                string status = "КРЮК СВОБОДЕН";
                if (attachedCargo != null)
                {
                    status = "ГРУЗ ПОДВЕШЕН: " + attachedCargo.name + " (Нажмите [Пробел] чтобы отцепить)";
                }
                else if (HasCargoNearby())
                {
                    status = "ГРУЗ РЯДОМ! (Нажмите [Пробел] чтобы захватить)";
                }

                string camHint = hookCamera != null ? "  |  C — Вид из кабины / Сверху на крюк" : "";
                string info = "A / D — Поворот стрелы  |  W / S — Каретка  |  R / F — Лебедка  |  Пробел — Захват/отцепка" + camHint + "\nВысота крюка: " + (24f - currentCableLength).ToString("F1") + " м  |  Вылет: " + currentTrolleyDist.ToString("F1") + " м  |  " + status;
                DrawVehicleHUD(info);
            }
        }
    }
}
