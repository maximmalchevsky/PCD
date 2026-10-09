using UnityEngine;
using System.Collections.Generic;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Task8_11
{
    public class PlayerInteraction : MonoBehaviour
    {
        public Camera playerCamera;
        public float interactDistance = 4.5f;
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
            interactDistance = 4.5f;

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
            UnstickRoomProps();
            CreateRoomBarriers();
        }

        void FixedUpdate()
        {
            EnforceRoomBoundaries();
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

            List<RaycastHit> hitsList = new List<RaycastHit>();
            RaycastHit[] directHits = Physics.RaycastAll(ray, interactDistance);
            if (directHits != null) hitsList.AddRange(directHits);

            RaycastHit[] sphereHits = Physics.SphereCastAll(ray, 0.20f, interactDistance);
            if (sphereHits != null)
            {
                for (int s = 0; s < sphereHits.Length; s++)
                {
                    bool exists = false;
                    for (int h = 0; h < hitsList.Count; h++)
                    {
                        if (hitsList[h].collider == sphereHits[s].collider)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists) hitsList.Add(sphereHits[s]);
                }
            }

            hitsList.Sort((a, b) => a.distance.CompareTo(b.distance));

            Collider playerCol = GetComponent<Collider>();
            CharacterController cc = GetComponent<CharacterController>();

            PickupableItem bestItem = null;
            CabinetDoor bestCab = null;
            Collider bestCabHitCol = null;
            LightSwitch bestSwitch = null;
            EntranceDoor bestDoor = null;
            WindowBlinds bestBlinds = null;
            FanController bestFan = null;

            for (int i = 0; i < hitsList.Count; i++)
            {
                RaycastHit h = hitsList[i];
                if (h.collider == null || h.collider.isTrigger) continue;
                if (h.collider == playerCol || h.collider == cc) continue;
                if (h.transform.name.StartsWith("Boundary_Barrier_")) continue;

                PickupableItem item = h.collider.GetComponentInParent<PickupableItem>();
                if (item == null) item = h.collider.GetComponentInChildren<PickupableItem>();
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
            if (item == null) return;
            heldItem = item;

            if (heldItem.rb == null)
            {
                heldItem.rb = heldItem.GetComponent<Rigidbody>();
                if (heldItem.rb == null) heldItem.rb = heldItem.GetComponentInParent<Rigidbody>();
                if (heldItem.rb == null) heldItem.rb = heldItem.gameObject.AddComponent<Rigidbody>();
            }

            heldItem.parentCabinetDoor = null;
            heldItem.rb.isKinematic = false;
            heldItem.rb.useGravity = false;
            heldItem.rb.linearVelocity = Vector3.zero;
            heldItem.rb.angularVelocity = Vector3.zero;

            Collider playerCol = GetComponent<Collider>();
            Collider[] itemCols = item.GetComponentsInChildren<Collider>();
            for (int i = 0; i < itemCols.Length; i++)
            {
                if (playerCol != null) Physics.IgnoreCollision(playerCol, itemCols[i], true);
            }
        }

        private void UpdateHeldItem()
        {
            if (heldItem == null || heldItem.rb == null)
            {
                heldItem = null;
                return;
            }

            currentPrompt = "[E] Поставить | [Q / ПКМ] Бросить";

            float holdDist = heldItem.rb.mass > 10f ? 1.4f : 0.85f;
            Vector3 targetPos = playerCamera.transform.position + playerCamera.transform.forward * holdDist + playerCamera.transform.up * -0.15f;

            targetPos.x = Mathf.Clamp(targetPos.x, 297.45f, 302.55f);
            targetPos.z = Mathf.Clamp(targetPos.z, 297.45f, 302.55f);
            targetPos.y = Mathf.Clamp(targetPos.y, -49.95f, -47.95f);

            heldItem.rb.linearVelocity = (targetPos - heldItem.transform.position) * 14f;
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
            Collider[] itemCols = heldItem.GetComponentsInChildren<Collider>();
            for (int i = 0; i < itemCols.Length; i++)
            {
                if (playerCol != null) Physics.IgnoreCollision(playerCol, itemCols[i], false);
            }

            if (heldItem.rb != null)
            {
                heldItem.rb.useGravity = true;
                heldItem.rb.linearVelocity = Vector3.down * 0.4f;
            }
            heldItem = null;
        }

        private void Throw()
        {
            if (heldItem == null) return;

            Collider playerCol = GetComponent<Collider>();
            Collider[] itemCols = heldItem.GetComponentsInChildren<Collider>();
            for (int i = 0; i < itemCols.Length; i++)
            {
                if (playerCol != null) Physics.IgnoreCollision(playerCol, itemCols[i], false);
            }

            if (heldItem.rb != null)
            {
                heldItem.rb.useGravity = true;
                heldItem.rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
            }
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
            if ((body.position.x > 302.4f && pushDir.x > 0f) || (body.position.x < 297.6f && pushDir.x < 0f)) pushDir.x = 0f;
            if ((body.position.z > 302.4f && pushDir.z > 0f) || (body.position.z < 297.6f && pushDir.z < 0f)) pushDir.z = 0f;

            body.linearVelocity = new Vector3(pushDir.x * 1.8f, body.linearVelocity.y, pushDir.z * 1.8f);
        }

        private void EnforceRoomBoundaries()
        {
            var rbs = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
            for (int i = 0; i < rbs.Length; i++)
            {
                Rigidbody r = rbs[i];
                if (r == null || r.isKinematic) continue;

                Vector3 p = r.position;
                if (p.x > 294f && p.x < 306f && p.z > 294f && p.z < 306f && p.y > -52f && p.y < -45f)
                {
                    float cX = Mathf.Clamp(p.x, 297.45f, 302.55f);
                    float cZ = Mathf.Clamp(p.z, 297.45f, 302.55f);
                    float cY = Mathf.Clamp(p.y, -49.98f, -47.95f);

                    if (p.x != cX || p.z != cZ || p.y != cY)
                    {
                        r.position = new Vector3(cX, cY, cZ);
                        Vector3 v = r.linearVelocity;
                        if ((p.x > 302.55f && v.x > 0f) || (p.x < 297.45f && v.x < 0f)) v.x = -v.x * 0.2f;
                        if ((p.z > 302.55f && v.z > 0f) || (p.z < 297.45f && v.z < 0f)) v.z = -v.z * 0.2f;
                        if (p.y > -47.95f && v.y > 0f) v.y = -0.5f;
                        if (p.y < -49.98f && v.y < 0f) v.y = 0f;
                        r.linearVelocity = v;
                    }
                }
            }
        }

        private void UnstickRoomProps()
        {
            var rbs = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
            for (int i = 0; i < rbs.Length; i++)
            {
                Rigidbody r = rbs[i];
                if (r == null) continue;

                Vector3 p = r.position;
                if (p.x > 294f && p.x < 306f && p.z > 294f && p.z < 306f && p.y > -52f && p.y < -45f)
                {
                    bool outOfBounds = p.x > 302.48f || p.x < 297.52f || p.z > 302.48f || p.z < 297.52f || p.y < -50.05f || p.y > -47.75f;
                    if (outOfBounds)
                    {
                        if (r.name.Contains("books") || r.name.Contains("Books"))
                        {
                            r.position = new Vector3(301.2f, -49.44f, 301.55f);
                            r.linearVelocity = Vector3.zero;
                        }
                        else if (r.name.StartsWith("Paper_"))
                        {
                            r.position = new Vector3(301.4f, -49.44f, 301.45f);
                            r.linearVelocity = Vector3.zero;
                        }
                        else
                        {
                            r.position = new Vector3(Mathf.Clamp(p.x, 297.8f, 302.2f), Mathf.Clamp(p.y, -49.95f, -48.2f), Mathf.Clamp(p.z, 297.8f, 302.2f));
                            r.linearVelocity = Vector3.zero;
                        }
                    }
                }
            }
        }

        private void CreateRoomBarriers()
        {
            if (GameObject.Find("Boundary_Barrier_Right") != null) return;

            CreateSingleBarrier("Boundary_Barrier_Right", new Vector3(304.1f, -48.9f, 300f), new Vector3(2.5f, 5f, 12f));
            CreateSingleBarrier("Boundary_Barrier_Left", new Vector3(295.9f, -48.9f, 300f), new Vector3(2.5f, 5f, 12f));
            CreateSingleBarrier("Boundary_Barrier_Back", new Vector3(300f, -48.9f, 304.1f), new Vector3(12f, 5f, 2.5f));
            CreateSingleBarrier("Boundary_Barrier_Front", new Vector3(300f, -48.9f, 295.9f), new Vector3(12f, 5f, 2.5f));
            CreateSingleBarrier("Boundary_Barrier_Ceil", new Vector3(300f, -46.5f, 300f), new Vector3(12f, 2.5f, 12f));
            CreateSingleBarrier("Boundary_Barrier_Floor", new Vector3(300f, -51.3f, 300f), new Vector3(12f, 2.5f, 12f));
        }

        private void CreateSingleBarrier(string name, Vector3 pos, Vector3 size)
        {
            GameObject obj = new GameObject(name);
            obj.transform.position = pos;
            BoxCollider col = obj.AddComponent<BoxCollider>();
            col.size = size;
        }

        void OnGUI()
        {
            if (playerCamera == null || !playerCamera.enabled) return;

            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            GUI.color = new Color(1f, 1f, 1f, 0.9f);
            GUI.Box(new Rect(cx - 3f, cy - 3f, 6f, 6f), GUIContent.none);

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
                                  "• [T] — Включить / выключить вентилятор\n" +
                                  "• [J] — Открыть / закрыть жалюзи на окне\n" +
                                  "• [L] — Выключатель света\n" +
                                  "• Столы и стулья можно двигать и брать в руки\n" +
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

            GameObject roomRoot = GameObject.Find("--- INTERIOR ROOM (TASKS 8-11) ---");
            if (roomRoot == null) roomRoot = GameObject.Find("Room_Root");
            if (roomRoot == null) roomRoot = GameObject.Find("Interior_Room");

            List<Transform> targetList = new List<Transform>();
            if (roomRoot != null)
            {
                for (int c = 0; c < roomRoot.transform.childCount; c++)
                {
                    targetList.Add(roomRoot.transform.GetChild(c));
                }
            }
            else
            {
                var trs = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
                for (int t = 0; t < trs.Length; t++)
                {
                    Vector3 pos = trs[t].position;
                    if (pos.x > 296f && pos.x < 304f && pos.z > 296f && pos.z < 304f && pos.y > -51f && pos.y < -46f)
                    {
                        if (trs[t].parent == null || trs[t].parent.name.Contains("INTERIOR") || trs[t].parent.name.Contains("Room"))
                        {
                            targetList.Add(trs[t]);
                        }
                    }
                }
            }

            for (int i = 0; i < targetList.Count; i++)
            {
                Transform t = targetList[i];
                string n = t.name;

                if (n.StartsWith("Paper_"))
                {
                    if (t.GetComponent<PaperSheet>() == null)
                    {
                        t.gameObject.AddComponent<PaperSheet>();
                    }
                    continue;
                }

                if (n.Contains("Rug") || n.Contains("Doormat"))
                {
                    Collider rugCol = t.GetComponent<Collider>();
                    if (rugCol != null) Object.Destroy(rugCol);
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
                    n.StartsWith("Cabinet_With_Doors") || n.StartsWith("Knob") || n.StartsWith("Boundary_Barrier_") ||
                    n.StartsWith("Entrance_DoorFrame") || n.StartsWith("Room_Build"))
                {
                    continue;
                }

                BoxCollider bc = t.GetComponent<BoxCollider>();
                if (bc == null)
                {
                    Renderer[] rends = t.GetComponentsInChildren<Renderer>();
                    if (rends.Length > 0)
                    {
                        Bounds b = rends[0].bounds;
                        for (int r = 1; r < rends.Length; r++)
                        {
                            b.Encapsulate(rends[r].bounds);
                        }

                        bc = t.gameObject.AddComponent<BoxCollider>();
                        bc.center = t.InverseTransformPoint(b.center);
                        bc.size = new Vector3(
                            Mathf.Abs(t.lossyScale.x) > 0.001f ? b.size.x / Mathf.Abs(t.lossyScale.x) : b.size.x,
                            Mathf.Abs(t.lossyScale.y) > 0.001f ? b.size.y / Mathf.Abs(t.lossyScale.y) : b.size.y,
                            Mathf.Abs(t.lossyScale.z) > 0.001f ? b.size.z / Mathf.Abs(t.lossyScale.z) : b.size.z
                        );
                        bc.size = new Vector3(Mathf.Max(bc.size.x, 0.15f), Mathf.Max(bc.size.y, 0.15f), Mathf.Max(bc.size.z, 0.15f));
                    }
                }

                Rigidbody rb = t.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = t.gameObject.AddComponent<Rigidbody>();
                }
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

                string displayName = GetFriendlyPropName(n);
                float mass = GetAppropriateMass(n);
                rb.mass = mass;
                rb.linearDamping = 0.5f;
                rb.angularDamping = 1.0f;

                if (n.Contains("Chair") || n.Contains("Table") || n.Contains("Desk") || n.Contains("TV") ||
                    n.Contains("Sofa") || n.Contains("Bookcase") || n.Contains("Fridge") || n.Contains("Coat") ||
                    n.Contains("Monitor") || n.Contains("Speaker"))
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }

                PickupableItem pItem = t.GetComponent<PickupableItem>();
                if (pItem == null) pItem = t.gameObject.AddComponent<PickupableItem>();
                pItem.itemName = displayName;
                pItem.rb = rb;
            }
        }

        private string GetFriendlyPropName(string n)
        {
            if (n.Contains("Chair_Office") || n.Contains("chairDesk")) return "Офисное кресло";
            if (n.Contains("Chair_Side") || n.Contains("Chair")) return "Стул";
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
            if (n.Contains("Item_radio") || n.Contains("radio")) return "Радиоприемник";
            if (n.Contains("Item_laptop") || n.Contains("laptop")) return "Ноутбук";
            if (n.Contains("Item_bear") || n.Contains("bear")) return "Плюшевый мишка";
            if (n.Contains("Item_trashcan") || n.Contains("trashcan")) return "Корзина для бумаг";
            if (n.Contains("Item_books") || n.Contains("books")) return "Книги";
            if (n.Contains("cardboardBox")) return "Картонная коробка";
            if (n.Contains("plantSmall")) return "Комнатный цветок";
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
            if (n.Contains("Item_radio") || n.Contains("radio")) return 1.2f;
            if (n.Contains("Item_laptop") || n.Contains("laptop")) return 2.2f;
            if (n.Contains("Item_bear") || n.Contains("bear")) return 0.6f;
            if (n.Contains("Item_trashcan") || n.Contains("trashcan")) return 0.8f;
            if (n.Contains("Item_books") || n.Contains("books")) return 1.0f;
            return 3f;
        }
    }
}
