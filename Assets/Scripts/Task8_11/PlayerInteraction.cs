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

            RaycastHit[] sphereHits = Physics.SphereCastAll(ray, 0.18f, interactDistance);
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
            Collider[] itemCols = heldItem.GetComponentsInChildren<Collider>();
            for (int i = 0; i < itemCols.Length; i++)
            {
                if (playerCol != null) Physics.IgnoreCollision(playerCol, itemCols[i], false);
            }

            if (heldItem.rb != null)
            {
                heldItem.rb.useGravity = true;
                heldItem.rb.linearVelocity = Vector3.down * 0.5f;

                if (heldItem.name.Contains("Chair") || heldItem.name.Contains("Desk") || heldItem.name.Contains("Table") || heldItem.name.Contains("Sofa"))
                {
                    heldItem.rb.linearDamping = 8f;
                    heldItem.rb.angularDamping = 8f;
                }
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
                                  "• [H] — Скрыть / показать эту подсказку";

                GUI.Box(new Rect(15f, 15f, 340f, 170f), helpText, helpStyle);
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
            if (roomRoot == null) return;

            EnsureDoorHandle(roomRoot);

            for (int c = 0; c < roomRoot.transform.childCount; c++)
            {
                Transform t = roomRoot.transform.GetChild(c);
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

                if (n.StartsWith("Room_") || n.StartsWith("Wall_") || n.StartsWith("Door_") ||
                    n.StartsWith("Entrance_") || n.StartsWith("Portal_") || n.StartsWith("SpawnPoint_") ||
                    n.StartsWith("Light_") || n.StartsWith("Window_") || n.StartsWith("Floor_Lamp_Light") ||
                    n.StartsWith("Desk_Lamp_Light") || n.StartsWith("Switch_") || n.StartsWith("Ceiling_Fan") ||
                    n.StartsWith("Fan_") || n.StartsWith("Blade_") || n.StartsWith("Cabinet_") ||
                    n.StartsWith("Shelf_") || n.StartsWith("LeftDoor_") || n.StartsWith("RightDoor_") ||
                    n.StartsWith("Knob"))
                {
                    continue;
                }

                if (n.Contains("Sofa") || n.Contains("TV_Cabinet") || n.Contains("Desk") ||
                    n.Contains("Table_Coffee") || n.Contains("Coffee_Station_Table") ||
                    n.Contains("Bookcase") || n.Contains("Fridge") || n.Contains("Coat_Rack"))
                {
                    FitColliderToVisuals(t.gameObject);
                    Rigidbody srb = t.GetComponent<Rigidbody>();
                    if (srb != null)
                    {
                        srb.isKinematic = true;
                    }
                    continue;
                }

                if (n.Contains("Chair"))
                {
                    FitColliderToVisuals(t.gameObject);
                    Rigidbody crb = t.GetComponent<Rigidbody>();
                    if (crb == null) crb = t.gameObject.AddComponent<Rigidbody>();
                    crb.isKinematic = true;
                    crb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                    PickupableItem cItem = t.GetComponent<PickupableItem>();
                    if (cItem == null) cItem = t.gameObject.AddComponent<PickupableItem>();
                    cItem.itemName = n.Contains("Office") ? "Офисное кресло" : "Стул";
                    cItem.rb = crb;
                    continue;
                }

                if (n.StartsWith("Item_") || n.Contains("Lamp") || n.Contains("Monitor") ||
                    n.Contains("Keyboard") || n.Contains("Mouse") || n.Contains("Pillow") ||
                    n.Contains("Speaker") || n.Contains("Coffee_Machine") || n.Contains("TV_Modern"))
                {
                    FitColliderToVisuals(t.gameObject);
                    Rigidbody prb = t.GetComponent<Rigidbody>();
                    if (prb == null) prb = t.gameObject.AddComponent<Rigidbody>();
                    prb.mass = GetAppropriateMass(n);
                    prb.linearDamping = 3f;
                    prb.angularDamping = 3f;

                    if (n.Contains("TV") || n.Contains("Monitor") || n.Contains("Speaker"))
                    {
                        prb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                    }

                    PickupableItem pItem = t.GetComponent<PickupableItem>();
                    if (pItem == null) pItem = t.gameObject.AddComponent<PickupableItem>();
                    pItem.itemName = GetFriendlyPropName(n);
                    pItem.rb = prb;
                }
            }
        }

        private void EnsureDoorHandle(GameObject roomRoot)
        {
            Transform doorTr = roomRoot.transform.Find("Entrance_Door");
            if (doorTr == null)
            {
                GameObject d = GameObject.Find("Entrance_Door");
                if (d != null) doorTr = d.transform;
            }
            if (doorTr == null) return;

            Transform doorLeafTr = doorTr.Find("Door_Leaf");
            if (doorLeafTr != null)
            {
                BoxCollider leafCol = doorLeafTr.GetComponent<BoxCollider>();
                if (leafCol == null) leafCol = doorLeafTr.gameObject.AddComponent<BoxCollider>();
                leafCol.size = Vector3.one;
                leafCol.center = Vector3.zero;
            }

            if (doorTr.Find("Door_Handle_Assembly") != null) return;

            GameObject handleAssembly = new GameObject("Door_Handle_Assembly");
            handleAssembly.transform.SetParent(doorTr, false);
            handleAssembly.transform.localPosition = new Vector3(0f, 0.95f, -0.76f);
            handleAssembly.transform.localRotation = Quaternion.identity;

            Material brassMat = null;
            var allMats = Resources.FindObjectsOfTypeAll<Material>();
            for (int m = 0; m < allMats.Length; m++)
            {
                if (allMats[m] != null && allMats[m].name.Contains("Capsule_Gold"))
                {
                    brassMat = allMats[m];
                    break;
                }
            }

            GameObject escutcheon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            escutcheon.name = "Handle_Plate";
            escutcheon.transform.SetParent(handleAssembly.transform, false);
            escutcheon.transform.localPosition = Vector3.zero;
            escutcheon.transform.localScale = new Vector3(0.08f, 0.22f, 0.055f);
            if (brassMat != null) escutcheon.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.Destroy(escutcheon.GetComponent<Collider>());

            GameObject insideStem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            insideStem.name = "Inside_Stem";
            insideStem.transform.SetParent(handleAssembly.transform, false);
            insideStem.transform.localPosition = new Vector3(0.052f, 0.045f, 0f);
            insideStem.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            insideStem.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
            if (brassMat != null) insideStem.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.Destroy(insideStem.GetComponent<Collider>());

            GameObject insideLever = GameObject.CreatePrimitive(PrimitiveType.Cube);
            insideLever.name = "Inside_Lever";
            insideLever.transform.SetParent(handleAssembly.transform, false);
            insideLever.transform.localPosition = new Vector3(0.068f, 0.045f, 0.065f);
            insideLever.transform.localScale = new Vector3(0.022f, 0.024f, 0.13f);
            if (brassMat != null) insideLever.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.Destroy(insideLever.GetComponent<Collider>());

            GameObject outsideStem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            outsideStem.name = "Outside_Stem";
            outsideStem.transform.SetParent(handleAssembly.transform, false);
            outsideStem.transform.localPosition = new Vector3(-0.052f, 0.045f, 0f);
            outsideStem.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            outsideStem.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
            if (brassMat != null) outsideStem.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.Destroy(outsideStem.GetComponent<Collider>());

            GameObject outsideLever = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outsideLever.name = "Outside_Lever";
            outsideLever.transform.SetParent(handleAssembly.transform, false);
            outsideLever.transform.localPosition = new Vector3(-0.068f, 0.045f, 0.065f);
            outsideLever.transform.localScale = new Vector3(0.022f, 0.024f, 0.13f);
            if (brassMat != null) outsideLever.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.Destroy(outsideLever.GetComponent<Collider>());

            GameObject keyhole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            keyhole.name = "Keyhole_Cylinder";
            keyhole.transform.SetParent(handleAssembly.transform, false);
            keyhole.transform.localPosition = new Vector3(0f, -0.055f, 0f);
            keyhole.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            keyhole.transform.localScale = new Vector3(0.016f, 0.042f, 0.016f);
            if (brassMat != null) keyhole.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.Destroy(keyhole.GetComponent<Collider>());
        }

        private void FitColliderToVisuals(GameObject obj)
        {
            Renderer[] rends = obj.GetComponentsInChildren<Renderer>(true);
            if (rends == null || rends.Length == 0) return;

            Matrix4x4 w2l = obj.transform.worldToLocalMatrix;
            bool hasPoint = false;
            Vector3 min = Vector3.zero;
            Vector3 max = Vector3.zero;

            for (int r = 0; r < rends.Length; r++)
            {
                Renderer rend = rends[r];
                if (rend == null || !rend.enabled) continue;
                if (rend is ParticleSystemRenderer || rend.GetComponent<Light>() != null) continue;
                string rn = rend.gameObject.name;
                if (rn.Contains("Strip") || rn.Contains("Accent") || rn.Contains("Mat") || rn.Contains("Gizmo")) continue;

                MeshFilter mf = rend.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    Vector3[] verts = mf.sharedMesh.vertices;
                    Matrix4x4 l2p = w2l * rend.transform.localToWorldMatrix;
                    for (int v = 0; v < verts.Length; v++)
                    {
                        Vector3 pt = l2p.MultiplyPoint3x4(verts[v]);
                        if (!hasPoint)
                        {
                            min = pt;
                            max = pt;
                            hasPoint = true;
                        }
                        else
                        {
                            min = Vector3.Min(min, pt);
                            max = Vector3.Max(max, pt);
                        }
                    }
                }
                else
                {
                    Bounds b = rend.bounds;
                    Vector3 b0 = w2l.MultiplyPoint3x4(new Vector3(b.min.x, b.min.y, b.min.z));
                    Vector3 b1 = w2l.MultiplyPoint3x4(new Vector3(b.max.x, b.max.y, b.max.z));
                    Vector3 b2 = w2l.MultiplyPoint3x4(new Vector3(b.min.x, b.min.y, b.max.z));
                    Vector3 b3 = w2l.MultiplyPoint3x4(new Vector3(b.max.x, b.min.y, b.min.z));
                    Vector3 b4 = w2l.MultiplyPoint3x4(new Vector3(b.min.x, b.max.y, b.min.z));
                    Vector3 b5 = w2l.MultiplyPoint3x4(new Vector3(b.max.x, b.max.y, b.min.z));
                    Vector3 b6 = w2l.MultiplyPoint3x4(new Vector3(b.min.x, b.max.y, b.max.z));
                    Vector3 b7 = w2l.MultiplyPoint3x4(new Vector3(b.max.x, b.max.y, b.max.z));

                    Vector3[] bPts = new Vector3[] { b0, b1, b2, b3, b4, b5, b6, b7 };
                    for (int bp = 0; bp < bPts.Length; bp++)
                    {
                        if (!hasPoint)
                        {
                            min = bPts[bp];
                            max = bPts[bp];
                            hasPoint = true;
                        }
                        else
                        {
                            min = Vector3.Min(min, bPts[bp]);
                            max = Vector3.Max(max, bPts[bp]);
                        }
                    }
                }
            }

            if (hasPoint)
            {
                Vector3 size = max - min;
                if (size.x > 0.005f && size.y > 0.005f && size.z > 0.005f)
                {
                    BoxCollider bc = obj.GetComponent<BoxCollider>();
                    if (bc == null) bc = obj.AddComponent<BoxCollider>();
                    bc.center = (min + max) * 0.5f;
                    bc.size = size;
                }
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
            if (n.Contains("Speaker_Left")) return "Левая колонка";
            if (n.Contains("Speaker_Right")) return "Правая колонка";
            if (n.Contains("Pillow_Blue")) return "Синяя подушка";
            if (n.Contains("Pillow")) return "Подушка";
            if (n.Contains("Coffee_Machine")) return "Кофемашина";
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
            if (n.Contains("TV_Modern")) return 7f;
            if (n.Contains("Coffee_Machine")) return 4.0f;
            if (n.Contains("Item_radio") || n.Contains("radio")) return 1.2f;
            if (n.Contains("Item_laptop") || n.Contains("laptop")) return 2.2f;
            if (n.Contains("Item_bear") || n.Contains("bear")) return 0.6f;
            if (n.Contains("Item_trashcan") || n.Contains("trashcan")) return 0.8f;
            if (n.Contains("Item_books") || n.Contains("books")) return 1.0f;
            return 2f;
        }
    }
}
