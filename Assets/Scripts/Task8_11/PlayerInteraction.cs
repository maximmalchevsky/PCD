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
        private PickupableItem targetItem = null;
        private Transform holdPoint;
        private string currentPrompt = string.Empty;
        private string warningMessage = string.Empty;
        private float warningTimer = 0f;
        private bool showHelp = true;

        public bool IsTargetingItem()
        {
            return heldItem != null || targetItem != null;
        }

        public bool HasActiveInteraction()
        {
            return heldItem != null || !string.IsNullOrEmpty(currentPrompt);
        }

        void Start()
        {
            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>(true);
            }

            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(playerCamera.transform);
            hp.transform.localPosition = new Vector3(0f, -0.12f, 0.75f);
            hp.transform.localRotation = Quaternion.identity;
            holdPoint = hp.transform;

            SetupRoomProps();
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
            targetItem = null;

            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit[] hits = Physics.RaycastAll(ray, interactDistance);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            PickupableItem bestItem = null;
            CabinetDoor bestCab = null;
            Collider bestCabHitCol = null;
            LightSwitch bestSwitch = null;
            EntranceDoor bestDoor = null;
            WindowBlinds bestBlinds = null;
            FanController bestFan = null;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit h = hits[i];
                if (h.collider.isTrigger) continue;

                PickupableItem item = h.collider.GetComponentInParent<PickupableItem>();
                if (item != null && bestItem == null)
                {
                    bestItem = item;
                }

                LightSwitch sw = h.collider.GetComponentInParent<LightSwitch>();
                if (sw != null && bestSwitch == null)
                {
                    bestSwitch = sw;
                }

                CabinetDoor cab = h.collider.GetComponentInParent<CabinetDoor>();
                if (cab != null && bestCab == null)
                {
                    bestCab = cab;
                    bestCabHitCol = h.collider;
                }

                EntranceDoor ed = h.collider.GetComponentInParent<EntranceDoor>();
                if (ed != null && bestDoor == null)
                {
                    bestDoor = ed;
                }

                WindowBlinds wb = h.collider.GetComponentInParent<WindowBlinds>();
                if (wb != null && bestBlinds == null)
                {
                    bestBlinds = wb;
                }

                FanController fan = h.collider.GetComponentInParent<FanController>();
                if (fan != null && bestFan == null)
                {
                    bestFan = fan;
                }
            }

            if (bestItem != null)
            {
                targetItem = bestItem;
                if (bestItem.parentCabinetDoor == null || bestItem.parentCabinetDoor.isOpen)
                {
                    if (bestSwitch != null && (bestSwitch.gameObject == bestItem.gameObject || bestSwitch.transform.IsChildOf(bestItem.transform) || bestItem.transform.IsChildOf(bestSwitch.transform)))
                    {
                        string switchKey = bestSwitch.hotkey != KeyCode.None ? bestSwitch.hotkey.ToString() : "F";
                        currentPrompt = "[E] Взять: " + bestItem.itemName + " | [" + switchKey + "] " + (bestSwitch.isOn ? "Выкл. " : "Вкл. ") + bestSwitch.switchName;
                        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                        {
                            string reason;
                            if (bestItem.CanPickUp(out reason))
                            {
                                PickUp(bestItem);
                            }
                            else
                            {
                                ShowWarning(reason);
                            }
                        }
                        else if ((bestSwitch.hotkey != KeyCode.None && Input.GetKeyDown(bestSwitch.hotkey)) || Input.GetKeyDown(KeyCode.F))
                        {
                            bestSwitch.Toggle();
                        }
                    }
                    else
                    {
                        currentPrompt = "[E] Взять: " + bestItem.itemName;
                        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                        {
                            string reason;
                            if (bestItem.CanPickUp(out reason))
                            {
                                PickUp(bestItem);
                            }
                            else
                            {
                                ShowWarning(reason);
                            }
                        }
                    }
                    return;
                }
            }

            if (bestCab != null)
            {
                if (!bestCab.isOpen || bestCab.IsLookingAtDoorLeaf(bestCabHitCol))
                {
                    currentPrompt = bestCab.isOpen ? "[E / F] Закрыть шкаф" : "[E / F] Открыть шкаф";
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                    {
                        bestCab.Toggle();
                    }
                    return;
                }
            }

            if (bestDoor != null)
            {
                currentPrompt = bestDoor.isOpen ? "[E / F] Закрыть дверь" : "[E / F] Открыть дверь";
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                {
                    bestDoor.Toggle();
                }
                return;
            }

            if (bestSwitch != null)
            {
                string keyHint = bestSwitch.hotkey != KeyCode.None ? "[E / " + bestSwitch.hotkey + "]" : "[E]";
                currentPrompt = bestSwitch.isOn ? keyHint + " Выключить " + bestSwitch.switchName : keyHint + " Включить " + bestSwitch.switchName;
                if (Input.GetKeyDown(KeyCode.E) || (bestSwitch.hotkey != KeyCode.None && Input.GetKeyDown(bestSwitch.hotkey)) || Input.GetMouseButtonDown(0))
                {
                    bestSwitch.Toggle();
                }
                return;
            }

            if (bestBlinds != null)
            {
                currentPrompt = bestBlinds.isOpen ? "[E / J] Закрыть жалюзи" : "[E / J] Открыть жалюзи";
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.J) || Input.GetMouseButtonDown(0))
                {
                    bestBlinds.Toggle();
                }
                return;
            }

            if (bestFan != null)
            {
                currentPrompt = bestFan.isSpinning ? "[E / T] Выключить вентилятор" : "[E / T] Включить вентилятор";
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.T) || Input.GetMouseButtonDown(0))
                {
                    bestFan.Toggle();
                }
                return;
            }
        }

        private void PickUp(PickupableItem item)
        {
            heldItem = item;
            heldItem.rb.useGravity = false;
            heldItem.rb.linearVelocity = Vector3.zero;
            heldItem.rb.angularVelocity = Vector3.zero;

            Collider playerCol = GetComponent<Collider>();
            if (playerCol != null)
            {
                Collider[] itemCols = item.GetComponentsInChildren<Collider>();
                for (int i = 0; i < itemCols.Length; i++)
                {
                    Physics.IgnoreCollision(playerCol, itemCols[i], true);
                }
            }
        }

        private void UpdateHeldItem()
        {
            currentPrompt = "[E] Поставить | [Q / ПКМ] Бросить";

            float holdDist = heldItem.rb != null && heldItem.rb.mass > 10f ? 1.45f : 0.85f;
            Vector3 targetPos = playerCamera.transform.position + playerCamera.transform.forward * holdDist + playerCamera.transform.up * -0.15f;
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

            Collider playerCol = GetComponent<Collider>();
            if (playerCol != null)
            {
                Collider[] itemCols = heldItem.GetComponentsInChildren<Collider>();
                for (int i = 0; i < itemCols.Length; i++)
                {
                    Physics.IgnoreCollision(playerCol, itemCols[i], false);
                }
            }

            heldItem.rb.useGravity = true;
            heldItem = null;
        }

        private void Throw()
        {
            if (heldItem == null) return;

            Collider playerCol = GetComponent<Collider>();
            if (playerCol != null)
            {
                Collider[] itemCols = heldItem.GetComponentsInChildren<Collider>();
                for (int i = 0; i < itemCols.Length; i++)
                {
                    Physics.IgnoreCollision(playerCol, itemCols[i], false);
                }
            }

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
            body.AddForce(pushDir * pushPower * 8f, ForceMode.Force);
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
                                  "• Дверной порог со светом — [E] Переход город / комната\n" +
                                  "• [E] или ЛКМ — Взять предмет / Нажать выключатель\n" +
                                  "• [Q] или ПКМ — Бросить предмет в руках\n" +
                                  "• [F] — Открыть / закрыть шкаф или дверь\n" +
                                  "• [T] — Включить / выключить вентилятор (сдувает бумаги со стола)\n" +
                                  "• [J] — Открыть / закрыть жалюзи на окне\n" +
                                  "• [L] — Выключатель света\n" +
                                  "• Столы и стулья можно толкать персонажем\n" +
                                  "• [H] — Скрыть / показать эту подсказку";

                GUI.Box(new Rect(15f, 15f, 340f, 185f), helpText, helpStyle);
            }
        }

        private void SetupRoomProps()
        {
            var allObjs = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            for (int i = 0; i < allObjs.Length; i++)
            {
                if (allObjs[i].name.StartsWith("Paper_") && allObjs[i].GetComponent<PaperSheet>() == null)
                {
                    allObjs[i].AddComponent<PaperSheet>();
                }
            }

            GameObject roomRoot = GameObject.Find("Room_Root");
            if (roomRoot == null) return;

            Transform[] children = roomRoot.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < children.Length; i++)
            {
                Transform t = children[i];
                if (t == roomRoot.transform) continue;

                string n = t.name;

                if (n.StartsWith("Paper_"))
                {
                    if (t.GetComponent<PaperSheet>() == null)
                    {
                        t.gameObject.AddComponent<PaperSheet>();
                    }
                    continue;
                }

                if (n.StartsWith("Room_Floor") || n.StartsWith("Room_Ceiling") || n.StartsWith("Room_Wall") ||
                    n.StartsWith("Room_Window") || n.StartsWith("Room_Doorway") || n.StartsWith("Window_") ||
                    n.StartsWith("Wall_") || n.StartsWith("Door_") || n.StartsWith("Portal_") ||
                    n.StartsWith("SpawnPoint_") || n.StartsWith("Light_") || n.StartsWith("Floor_Lamp_Light") ||
                    n.StartsWith("Desk_Lamp_Light") || n.StartsWith("Switch_Plate") || n.StartsWith("Switch_Lever") ||
                    n.StartsWith("Wall_Switch") || n.StartsWith("Ceiling_Fan") || n.StartsWith("Fan_") ||
                    n.StartsWith("Blade_") || n.StartsWith("Cabinet_Back") || n.StartsWith("Cabinet_Left") ||
                    n.StartsWith("Cabinet_Right") || n.StartsWith("Cabinet_Top") || n.StartsWith("Cabinet_Bottom") ||
                    n.StartsWith("Shelf_") || n.StartsWith("LeftDoor_") || n.StartsWith("RightDoor_") ||
                    n.StartsWith("Cabinet_With_Doors") || n.StartsWith("Knob"))
                {
                    continue;
                }

                if (t.parent != roomRoot.transform)
                {
                    continue;
                }

                if (t.GetComponent<PickupableItem>() != null)
                {
                    continue;
                }

                Collider col = t.GetComponent<Collider>();
                if (col == null)
                {
                    Renderer[] rends = t.GetComponentsInChildren<Renderer>();
                    if (rends.Length > 0)
                    {
                        Bounds b = rends[0].bounds;
                        for (int r = 1; r < rends.Length; r++) b.Encapsulate(rends[r].bounds);
                        BoxCollider bc = t.gameObject.AddComponent<BoxCollider>();
                        bc.center = t.InverseTransformPoint(b.center);
                        bc.size = new Vector3(
                            t.lossyScale.x > 0.001f ? b.size.x / t.lossyScale.x : b.size.x,
                            t.lossyScale.y > 0.001f ? b.size.y / t.lossyScale.y : b.size.y,
                            t.lossyScale.z > 0.001f ? b.size.z / t.lossyScale.z : b.size.z
                        );
                    }
                    else
                    {
                        continue;
                    }
                }

                Rigidbody rb = t.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = t.gameObject.AddComponent<Rigidbody>();
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }

                string displayName = GetFriendlyPropName(n);
                float mass = GetAppropriateMass(n);
                rb.mass = mass;
                rb.linearDamping = 1.0f;
                rb.angularDamping = 1.5f;

                if (n.Contains("Chair") || n.Contains("Table") || n.Contains("Desk") || n.Contains("TV") ||
                    n.Contains("Sofa") || n.Contains("Bookcase") || n.Contains("Fridge") || n.Contains("Coat"))
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }

                PickupableItem pItem = t.gameObject.AddComponent<PickupableItem>();
                pItem.itemName = displayName;
            }
        }

        private string GetFriendlyPropName(string n)
        {
            if (n.Contains("Chair_Office") || n.Contains("chairDesk")) return "Офисное кресло";
            if (n.Contains("Chair")) return "Стул";
            if (n.Contains("Monitor")) return "Монитор";
            if (n.Contains("Keyboard")) return "Клавиатура";
            if (n.Contains("Mouse")) return "Мышь";
            if (n.Contains("Lamp_Floor")) return "Торшер";
            if (n.Contains("Lamp")) return "Настольная лампа";
            if (n.Contains("TV_Modern")) return "Телевизор";
            if (n.Contains("TV_Cabinet")) return "Тумба под ТВ";
            if (n.Contains("Speaker_Left")) return "Левая колонка";
            if (n.Contains("Speaker_Right")) return "Правая колонка";
            if (n.Contains("Table_Coffee")) return "Журнальный столик";
            if (n.Contains("Desk")) return "Письменный стол";
            if (n.Contains("Sofa")) return "Диван";
            if (n.Contains("Pillow_Blue")) return "Синяя подушка";
            if (n.Contains("Pillow")) return "Подушка";
            if (n.Contains("Coffee_Machine")) return "Кофемашина";
            if (n.Contains("Coffee_Station_Table") || n.Contains("sideTableDrawers")) return "Тумба для кофе";
            if (n.Contains("Fridge")) return "Холодильник";
            if (n.Contains("Bookcase")) return "Стеллаж";
            if (n.Contains("Coat_Rack") || n.Contains("coatRack")) return "Вешалка";
            if (n.Contains("Potted_Plant") || n.Contains("pottedPlant")) return "Растение в горшке";
            if (n.Contains("Rug_Desk")) return "Коврик у стола";
            if (n.Contains("Rug")) return "Коврик";
            return n.Replace("_", " ");
        }

        private float GetAppropriateMass(string n)
        {
            if (n.Contains("Mouse")) return 0.2f;
            if (n.Contains("Keyboard")) return 0.8f;
            if (n.Contains("Pillow")) return 0.5f;
            if (n.Contains("Lamp_Floor")) return 3.5f;
            if (n.Contains("Lamp")) return 1.5f;
            if (n.Contains("Monitor")) return 3.0f;
            if (n.Contains("Speaker")) return 2.0f;
            if (n.Contains("Chair_Office")) return 12f;
            if (n.Contains("Chair")) return 9f;
            if (n.Contains("TV_Modern")) return 7f;
            if (n.Contains("Table_Coffee")) return 14f;
            if (n.Contains("Coffee_Machine")) return 4.0f;
            if (n.Contains("Coffee_Station_Table")) return 12f;
            if (n.Contains("Potted_Plant")) return 5f;
            if (n.Contains("Coat_Rack")) return 4f;
            if (n.Contains("Desk")) return 25f;
            if (n.Contains("TV_Cabinet")) return 18f;
            if (n.Contains("Sofa")) return 26f;
            if (n.Contains("Fridge")) return 16f;
            if (n.Contains("Bookcase")) return 18f;
            return 5f;
        }
    }
}
