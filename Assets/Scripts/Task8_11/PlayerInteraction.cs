using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Task8_11
{
    public class PlayerInteraction : MonoBehaviour
    {
        public Camera playerCamera;
        public float interactDistance = 2.8f;
        public float pushPower = 2.5f;
        public float throwForce = 7.5f;

        private PickupableItem heldItem = null;
        private Transform holdPoint;
        private string currentPrompt = string.Empty;
        private string warningMessage = string.Empty;
        private float warningTimer = 0f;
        private bool showHelp = true;

        void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>(true);
            }

            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(playerCamera.transform);
            hp.transform.localPosition = new Vector3(0f, -0.2f, 1.25f);
            hp.transform.localRotation = Quaternion.identity;
            holdPoint = hp.transform;
        }

        void Update()
        {
            if (playerCamera == null || !playerCamera.enabled) return;

            if (warningTimer > 0f)
            {
                warningTimer -= Time.deltaTime;
                if (warningTimer <= 0f) warningMessage = string.Empty;
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                showHelp = !showHelp;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                FanController fan = Object.FindAnyObjectByType<FanController>();
                if (fan != null) fan.Toggle();
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                WindowBlinds blinds = Object.FindAnyObjectByType<WindowBlinds>();
                if (blinds != null) blinds.Toggle();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                LightSwitch sw = Object.FindAnyObjectByType<LightSwitch>();
                if (sw != null) sw.Toggle();
            }

            if (heldItem != null)
            {
                UpdateHeldItem();
                return;
            }

            CheckLookTarget();
        }

        private void CheckLookTarget()
        {
            currentPrompt = string.Empty;
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactDistance))
            {
                PickupableItem item = hit.collider.GetComponentInParent<PickupableItem>();
                if (item != null)
                {
                    currentPrompt = "[E] Взять: " + item.itemName;
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        string reason;
                        if (item.CanPickUp(out reason))
                        {
                            PickUp(item);
                        }
                        else
                        {
                            ShowWarning(reason);
                        }
                    }
                    return;
                }

                LightSwitch sw = hit.collider.GetComponentInParent<LightSwitch>();
                if (sw != null)
                {
                    currentPrompt = sw.isOn ? "[E] Выключить свет" : "[E] Включить свет";
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                    {
                        sw.Toggle();
                    }
                    return;
                }

                CabinetDoor cab = hit.collider.GetComponentInParent<CabinetDoor>();
                if (cab != null)
                {
                    currentPrompt = cab.isOpen ? "[E / F] Закрыть шкаф" : "[E / F] Открыть шкаф";
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                    {
                        cab.Toggle();
                    }
                    return;
                }

                EntranceDoor ed = hit.collider.GetComponentInParent<EntranceDoor>();
                if (ed != null)
                {
                    currentPrompt = ed.isOpen ? "[E / F] Закрыть дверь" : "[E / F] Открыть дверь";
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                    {
                        ed.Toggle();
                    }
                    return;
                }

                WindowBlinds wb = hit.collider.GetComponentInParent<WindowBlinds>();
                if (wb != null)
                {
                    currentPrompt = wb.isOpen ? "[E / J] Закрыть жалюзи" : "[E / J] Открыть жалюзи";
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
                    {
                        wb.Toggle();
                    }
                    return;
                }

                FanController fan = hit.collider.GetComponentInParent<FanController>();
                if (fan != null)
                {
                    currentPrompt = fan.isSpinning ? "[E / T] Выключить вентилятор" : "[E / T] Включить вентилятор";
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.T) || Input.GetMouseButtonDown(0))
                    {
                        fan.Toggle();
                    }
                    return;
                }
            }
        }

        private void PickUp(PickupableItem item)
        {
            heldItem = item;
            heldItem.rb.useGravity = false;
            heldItem.rb.linearVelocity = Vector3.zero;
            heldItem.rb.angularVelocity = Vector3.zero;
        }

        private void UpdateHeldItem()
        {
            currentPrompt = "[E] Поставить | [Q / ПКМ] Бросить";

            Vector3 targetPos = holdPoint.position;
            heldItem.rb.linearVelocity = (targetPos - heldItem.transform.position) * 12f;
            heldItem.transform.rotation = Quaternion.Slerp(heldItem.transform.rotation, playerCamera.transform.rotation, Time.deltaTime * 10f);

            if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
            {
                Drop();
            }
            else if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(1))
            {
                Throw();
            }
        }

        private void Drop()
        {
            if (heldItem == null) return;
            heldItem.rb.useGravity = true;
            heldItem = null;
        }

        private void Throw()
        {
            if (heldItem == null) return;
            heldItem.rb.useGravity = true;
            heldItem.rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
            heldItem = null;
        }

        private void ShowWarning(string msg)
        {
            warningMessage = msg;
            warningTimer = 2.5f;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic) return;
            if (hit.moveDirection.y < -0.3f) return;
            if (heldItem != null && body == heldItem.rb) return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z).normalized;
            body.AddForceAtPosition(pushDir * pushPower * 10f, hit.point, ForceMode.Force);
        }

        void OnGUI()
        {
            if (playerCamera == null || !playerCamera.enabled) return;

            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            GUI.color = Color.white;
            GUI.Box(new Rect(cx - 2f, cy - 2f, 4f, 4f), GUIContent.none);

            if (!string.IsNullOrEmpty(currentPrompt))
            {
                GUIStyle promptStyle = new GUIStyle(GUI.skin.box);
                promptStyle.fontSize = 15;
                promptStyle.normal.textColor = Color.white;
                promptStyle.alignment = TextAnchor.MiddleCenter;

                GUI.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 0.85f);
                GUI.Box(new Rect(cx - 200f, Screen.height - 95f, 400f, 36f), currentPrompt, promptStyle);
            }

            if (!string.IsNullOrEmpty(warningMessage))
            {
                GUIStyle warnStyle = new GUIStyle(GUI.skin.box);
                warnStyle.fontSize = 15;
                warnStyle.normal.textColor = new Color(1f, 0.35f, 0.35f);
                warnStyle.alignment = TextAnchor.MiddleCenter;

                GUI.backgroundColor = new Color(0.3f, 0.05f, 0.05f, 0.9f);
                GUI.Box(new Rect(cx - 220f, cy + 45f, 440f, 34f), warningMessage, warnStyle);
            }

            if (showHelp)
            {
                GUIStyle helpStyle = new GUIStyle(GUI.skin.box);
                helpStyle.fontSize = 12;
                helpStyle.normal.textColor = new Color(0.9f, 0.95f, 1f);
                helpStyle.alignment = TextAnchor.UpperLeft;
                helpStyle.padding = new RectOffset(10, 10, 8, 8);

                GUI.backgroundColor = new Color(0.08f, 0.12f, 0.2f, 0.82f);
                string helpText = "УПРАВЛЕНИЕ В КОМНАТЕ:\n" +
                                  "• Синий круг на полу — Портал в город / в комнату\n" +
                                  "• [E] или ЛКМ — Взять предмет / Нажать выключатель\n" +
                                  "• [Q] или ПКМ — Бросить предмет в руках\n" +
                                  "• [F] — Открыть / закрыть шкаф или дверь\n" +
                                  "• [T] — Включить / выключить вентилятор (сдувает предметы)\n" +
                                  "• [J] — Открыть / закрыть жалюзи на окне\n" +
                                  "• [L] — Выключатель света\n" +
                                  "• Столы и стулья можно толкать персонажем\n" +
                                  "• [H] — Скрыть / показать эту подсказку";

                GUI.Box(new Rect(15f, 15f, 340f, 185f), helpText, helpStyle);
            }
        }
    }
}
