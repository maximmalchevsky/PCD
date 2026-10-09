using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Task8_11
{
    [InitializeOnLoad]
    public static class FitRoomColliders
    {
        static FitRoomColliders()
        {
            EditorApplication.delayCall += ExecuteFit;
        }

        [MenuItem("Tools/Fit Room Colliders & Door Handle")]
        public static void ExecuteFit()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.name.Contains("task6_7")) return;

            GameObject roomRoot = GameObject.Find("--- INTERIOR ROOM (TASKS 8-11) ---");
            if (roomRoot == null) roomRoot = GameObject.Find("Room_Root");
            if (roomRoot == null) return;

            EnsureDoorHandle(roomRoot);

            for (int i = 0; i < roomRoot.transform.childCount; i++)
            {
                Transform t = roomRoot.transform.GetChild(i);
                string n = t.name;

                if (n.Contains("Rug") || n.Contains("Doormat"))
                {
                    Collider rc = t.GetComponent<Collider>();
                    if (rc != null) Object.DestroyImmediate(rc);
                    continue;
                }

                if (n.StartsWith("Room_Floor") || n.StartsWith("Room_Ceiling") || n.StartsWith("Wall_") ||
                    n.StartsWith("Window_") || n.StartsWith("Portal_") || n.StartsWith("SpawnPoint_") ||
                    n.StartsWith("Ceiling_Fan") || n.StartsWith("Light_") || n.StartsWith("Floor_Lamp_Light") ||
                    n.StartsWith("Desk_Lamp_Light") || n.StartsWith("Cabinet_With_Doors") || n.StartsWith("Entrance_DoorFrame"))
                {
                    continue;
                }

                FitColliderToVisuals(t.gameObject);
            }

            EditorSceneManager.MarkSceneDirty(activeScene);
            EditorSceneManager.SaveScene(activeScene);
        }

        public static void EnsureDoorHandle(GameObject roomRoot)
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

            Material brassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_T2_Capsule_Gold.mat");
            if (brassMat == null) brassMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Mat_Room_Wood.mat");

            GameObject escutcheon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            escutcheon.name = "Handle_Plate";
            escutcheon.transform.SetParent(handleAssembly.transform, false);
            escutcheon.transform.localPosition = Vector3.zero;
            escutcheon.transform.localScale = new Vector3(0.08f, 0.22f, 0.055f);
            if (brassMat != null) escutcheon.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.DestroyImmediate(escutcheon.GetComponent<Collider>());

            GameObject insideStem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            insideStem.name = "Inside_Stem";
            insideStem.transform.SetParent(handleAssembly.transform, false);
            insideStem.transform.localPosition = new Vector3(0.052f, 0.045f, 0f);
            insideStem.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            insideStem.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
            if (brassMat != null) insideStem.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.DestroyImmediate(insideStem.GetComponent<Collider>());

            GameObject insideLever = GameObject.CreatePrimitive(PrimitiveType.Cube);
            insideLever.name = "Inside_Lever";
            insideLever.transform.SetParent(handleAssembly.transform, false);
            insideLever.transform.localPosition = new Vector3(0.068f, 0.045f, 0.065f);
            insideLever.transform.localScale = new Vector3(0.022f, 0.024f, 0.13f);
            if (brassMat != null) insideLever.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.DestroyImmediate(insideLever.GetComponent<Collider>());

            GameObject outsideStem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            outsideStem.name = "Outside_Stem";
            outsideStem.transform.SetParent(handleAssembly.transform, false);
            outsideStem.transform.localPosition = new Vector3(-0.052f, 0.045f, 0f);
            outsideStem.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            outsideStem.transform.localScale = new Vector3(0.022f, 0.022f, 0.022f);
            if (brassMat != null) outsideStem.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.DestroyImmediate(outsideStem.GetComponent<Collider>());

            GameObject outsideLever = GameObject.CreatePrimitive(PrimitiveType.Cube);
            outsideLever.name = "Outside_Lever";
            outsideLever.transform.SetParent(handleAssembly.transform, false);
            outsideLever.transform.localPosition = new Vector3(-0.068f, 0.045f, 0.065f);
            outsideLever.transform.localScale = new Vector3(0.022f, 0.024f, 0.13f);
            if (brassMat != null) outsideLever.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.DestroyImmediate(outsideLever.GetComponent<Collider>());

            GameObject keyhole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            keyhole.name = "Keyhole_Cylinder";
            keyhole.transform.SetParent(handleAssembly.transform, false);
            keyhole.transform.localPosition = new Vector3(0f, -0.055f, 0f);
            keyhole.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            keyhole.transform.localScale = new Vector3(0.016f, 0.042f, 0.016f);
            if (brassMat != null) keyhole.GetComponent<Renderer>().sharedMaterial = brassMat;
            Object.DestroyImmediate(keyhole.GetComponent<Collider>());
        }

        public static void FitColliderToVisuals(GameObject obj)
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
    }
}
