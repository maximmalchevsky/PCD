using System.Collections.Generic;
using UnityEngine;
using Task8_11;

namespace Task12_16
{
    public class ConstructionSiteBuilder : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoBuildOnPlay()
        {
            if (GameObject.Find("--- CONSTRUCTION SITE (TASKS 12-16) ---") != null) return;
            GameObject holder = new GameObject("ConstructionSite_Runtime");
            holder.AddComponent<ConstructionSiteBuilder>();
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("City/Build Construction Site")]
        public static void BuildSiteInEditor()
        {
            GameObject existing = GameObject.Find("--- CONSTRUCTION SITE (TASKS 12-16) ---");
            if (existing != null) DestroyImmediate(existing);
            GameObject holder = new GameObject("ConstructionSite_EditorTemp");
            ConstructionSiteBuilder builder = holder.AddComponent<ConstructionSiteBuilder>();
            builder.BuildFullSite();
            DestroyImmediate(holder);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
#endif

        private Material matDirt;
        private Material matConcrete;
        private Material matYellow;
        private Material matOrange;
        private Material matDarkMetal;
        private Material matSand;
        private Material matRock;
        private Material matBrick;
        private Material matWood;
        private Material matGlass;
        private Material matVehicles;
        private Material matTrain;
        private Material matEnvironment;

        private GameObject prefabDozer;
        private GameObject prefabTractor;
        private GameObject prefabTruckFlat;
        private GameObject prefabCarriageDirt;
        private GameObject prefabFence;
        private GameObject prefabBarrier;
        private GameObject prefabCone;
        private GameObject prefabLight;
        private GameObject prefabDumpster;
        private GameObject prefabContainerBlue;
        private GameObject prefabContainerRed;
        private GameObject prefabFlatbed;
        private GameObject prefabDebrisBolt;
        private GameObject prefabDebrisNut;
        private GameObject prefabDebrisPlate;
        private GameObject prefabDebrisTire;
        private GameObject prefabWheelTractorFront;
        private GameObject prefabWheelTractorBack;
        private GameObject prefabWheelTruck;
        private GameObject prefabElectricityPole;

        void Awake()
        {
            GameObject existing = GameObject.Find("--- CONSTRUCTION SITE (TASKS 12-16) ---");
            if (existing != null) Destroy(existing);
            BuildFullSite();
        }

        void Start()
        {
            PurgeForeignObjectsInZone();
        }

        public static bool IsInConstructionZone(Vector3 pos)
        {
            return pos.x >= -75f && pos.x <= -25f && pos.z >= -5f && pos.z <= 45f;
        }

        public void PurgeForeignObjectsInZone()
        {
            GameObject site = GameObject.Find("--- CONSTRUCTION SITE (TASKS 12-16) ---");
            Transform siteTransform = site != null ? site.transform : null;

            Bounds zoneBounds = new Bounds(new Vector3(-50f, 10f, 20f), new Vector3(50f, 40f, 50f));
            Renderer[] allRenderers = Object.FindObjectsByType<Renderer>(FindObjectsInactive.Exclude);
            for (int i = 0; i < allRenderers.Length; i++)
            {
                Renderer r = allRenderers[i];
                if (r == null) continue;
                if (siteTransform != null && r.transform.IsChildOf(siteTransform)) continue;
                if (r.gameObject.name == "Ground" || r.gameObject.name.Contains("Terrain")) continue;
                if (r.GetComponentInParent<CharacterController>() != null) continue;

                if (zoneBounds.Intersects(r.bounds) || IsInConstructionZone(r.bounds.center) || IsInConstructionZone(r.transform.position))
                {
                    Transform root = r.transform;
                    while (root.parent != null && root.parent.parent != null &&
                           !root.parent.name.StartsWith("---") && !root.parent.name.StartsWith("City_Clone"))
                    {
                        root = root.parent;
                    }
                    Destroy(root.gameObject);
                }
            }

            Task6_7.CarAgent[] cars = Object.FindObjectsByType<Task6_7.CarAgent>(FindObjectsInactive.Exclude);
            for (int i = 0; i < cars.Length; i++)
            {
                if (cars[i] != null && IsInConstructionZone(cars[i].transform.position))
                {
                    Destroy(cars[i].gameObject);
                }
            }

            Task6_7.PedestrianAgent[] peds = Object.FindObjectsByType<Task6_7.PedestrianAgent>(FindObjectsInactive.Exclude);
            for (int i = 0; i < peds.Length; i++)
            {
                if (peds[i] != null && IsInConstructionZone(peds[i].transform.position))
                {
                    Destroy(peds[i].gameObject);
                }
            }
        }

        public void BuildFullSite()
        {
            LoadAssetsAndMaterials();

            GameObject siteRoot = new GameObject("--- CONSTRUCTION SITE (TASKS 12-16) ---");
            siteRoot.transform.position = new Vector3(-50f, 0.20f, 20f);

            BuildGroundSlab(siteRoot.transform);
            BuildRoadAccessApron(siteRoot.transform);
            BuildPerimeterEnclosure(siteRoot.transform);
            BuildExcavationPit(siteRoot.transform);
            BuildFoundationUnderConstruction(siteRoot.transform);
            BuildSitePropsAndDumpsters(siteRoot.transform);
            SpawnGranularCargo(siteRoot.transform);

            BuildBulldozer(siteRoot.transform);
            BuildDumpTruck(siteRoot.transform);
            BuildExcavator(siteRoot.transform);
            BuildTowerCrane(siteRoot.transform);
            IgnoreChassisCollisionsWithCargo(siteRoot.transform);
        }

        private void LoadAssetsAndMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            matDirt = CreateMat("Mat_Site_Dirt", litShader, new Color(0.38f, 0.28f, 0.17f), 0.1f, 0f);
            matConcrete = CreateMat("Mat_Site_Concrete", litShader, new Color(0.70f, 0.70f, 0.72f), 0.2f, 0.05f);
            matYellow = CreateMat("Mat_Site_Yellow", litShader, new Color(0.96f, 0.76f, 0.06f), 0.6f, 0.2f);
            matOrange = CreateMat("Mat_Site_Orange", litShader, new Color(0.95f, 0.45f, 0.05f), 0.6f, 0.2f);
            matDarkMetal = CreateMat("Mat_Site_DarkMetal", litShader, new Color(0.20f, 0.21f, 0.24f), 0.4f, 0.5f);
            matSand = CreateMat("Mat_Site_Sand", litShader, new Color(0.82f, 0.71f, 0.46f), 0.1f, 0f);
            matRock = CreateMat("Mat_Site_Rock", litShader, new Color(0.48f, 0.49f, 0.51f), 0.15f, 0f);
            matBrick = CreateMat("Mat_Site_Brick", litShader, new Color(0.74f, 0.28f, 0.18f), 0.1f, 0f);
            matWood = CreateMat("Mat_Site_Wood", litShader, new Color(0.58f, 0.42f, 0.26f), 0.2f, 0f);
            matGlass = CreateMat("Mat_Site_Glass", litShader, new Color(0.5f, 0.75f, 0.85f, 0.8f), 0.9f, 0.1f);

            matVehicles = Resources.Load<Material>("Mat_Vehicles");
            matTrain = Resources.Load<Material>("Mat_TrainKit");
            matEnvironment = Resources.Load<Material>("Mat_Environment");

            if (matVehicles == null) matVehicles = matYellow;
            if (matTrain == null) matTrain = matOrange;
            if (matEnvironment == null) matEnvironment = matDarkMetal;

            prefabDozer = Resources.Load<GameObject>("tractor-shovel");
            prefabTractor = Resources.Load<GameObject>("tractor");
            prefabTruckFlat = Resources.Load<GameObject>("truck-flat");
            prefabCarriageDirt = Resources.Load<GameObject>("train-carriage-dirt") ?? Resources.Load<GameObject>("carriage-dirt");
            prefabFence = Resources.Load<GameObject>("construction-fence");
            prefabBarrier = Resources.Load<GameObject>("construction-barrier");
            prefabCone = Resources.Load<GameObject>("construction-cone");
            prefabLight = Resources.Load<GameObject>("construction-light");
            prefabDumpster = Resources.Load<GameObject>("dumpster");
            prefabContainerBlue = Resources.Load<GameObject>("train-carriage-container-blue") ?? Resources.Load<GameObject>("container-blue");
            prefabContainerRed = Resources.Load<GameObject>("train-carriage-container-red") ?? Resources.Load<GameObject>("container-red");
            prefabFlatbed = Resources.Load<GameObject>("train-carriage-flatbed");
            prefabDebrisBolt = Resources.Load<GameObject>("debris-bolt");
            prefabDebrisNut = Resources.Load<GameObject>("debris-nut");
            prefabDebrisPlate = Resources.Load<GameObject>("debris-plate-a");
            prefabDebrisTire = Resources.Load<GameObject>("debris-tire");
            prefabWheelTractorFront = Resources.Load<GameObject>("wheel-tractor-front");
            prefabWheelTractorBack = Resources.Load<GameObject>("wheel-tractor-back");
            prefabWheelTruck = Resources.Load<GameObject>("wheel-truck");
            prefabElectricityPole = Resources.Load<GameObject>("electricity-pole");
        }

        private Material CreateMat(string name, Shader shader, Color color, float smoothness, float metallic)
        {
            Material m = new Material(shader);
            m.name = name;
            m.color = color;
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            return m;
        }

        private void ApplyMaterialRecursively(GameObject target, Material mat)
        {
            if (target == null || mat == null) return;
            Renderer[] rends = target.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < rends.Length; i++)
            {
                if (rends[i] != null) rends[i].sharedMaterial = mat;
            }
        }

        private static Transform FindChildDeep(Transform parent, string name)
        {
            if (parent == null) return null;
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase)) return child;
                Transform deep = FindChildDeep(child, name);
                if (deep != null) return deep;
            }
            return null;
        }

        private static void StripColliders(GameObject target)
        {
            if (target == null) return;
            Collider[] cols = target.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i] != null) Object.Destroy(cols[i]);
            }
        }

        private void BuildGroundSlab(Transform root)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Site_Ground_Slab";
            ground.transform.SetParent(root, false);
            ground.transform.localPosition = new Vector3(0f, 0f, 0f);
            ground.transform.localScale = new Vector3(38f, 0.40f, 42f);
            ground.GetComponent<Renderer>().sharedMaterial = matDirt;

            CreateWall("Curb_North", root, new Vector3(0f, 0.32f, 21f), new Vector3(38f, 0.65f, 0.8f), matConcrete);
            CreateWall("Curb_South", root, new Vector3(0f, 0.32f, -21f), new Vector3(38f, 0.65f, 0.8f), matConcrete);
            CreateWall("Curb_West", root, new Vector3(-19f, 0.32f, 0f), new Vector3(0.8f, 0.65f, 42f), matConcrete);

            CreateWall("Curb_East_1", root, new Vector3(19f, 0.32f, 13.5f), new Vector3(0.8f, 0.65f, 15f), matConcrete);
            CreateWall("Curb_East_2", root, new Vector3(19f, 0.32f, -13.5f), new Vector3(0.8f, 0.65f, 15f), matConcrete);
        }

        private void BuildRoadAccessApron(Transform root)
        {
            GameObject apron = GameObject.CreatePrimitive(PrimitiveType.Cube);
            apron.name = "Road_Access_Apron";
            apron.transform.SetParent(root, false);
            apron.transform.localPosition = new Vector3(20.5f, -0.08f, 0f);
            apron.transform.localRotation = Quaternion.Euler(0f, 0f, -4.5f);
            apron.transform.localScale = new Vector3(4.8f, 0.28f, 9.5f);
            apron.GetComponent<Renderer>().sharedMaterial = matConcrete;

            GameObject rampStripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rampStripe.name = "Yellow_Access_Stripe";
            rampStripe.transform.SetParent(apron.transform, false);
            rampStripe.transform.localPosition = new Vector3(0.48f, 0.51f, 0f);
            rampStripe.transform.localScale = new Vector3(0.08f, 0.05f, 0.95f);
            rampStripe.GetComponent<Renderer>().sharedMaterial = matYellow;
            Object.Destroy(rampStripe.GetComponent<Collider>());
        }

        private GameObject CreateWall(string name, Transform parent, Vector3 pos, Vector3 scale, Material mat)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = name;
            wall.transform.SetParent(parent, false);
            wall.transform.localPosition = pos;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().sharedMaterial = mat;
            return wall;
        }

        private void BuildPerimeterEnclosure(Transform root)
        {
            BuildFenceAlongLine(root, new Vector3(-18.5f, 0.20f, 20.5f), new Vector3(18.5f, 0.20f, 20.5f));
            BuildFenceAlongLine(root, new Vector3(-18.5f, 0.20f, -20.5f), new Vector3(18.5f, 0.20f, -20.5f));
            BuildFenceAlongLine(root, new Vector3(-18.5f, 0.20f, -20.5f), new Vector3(-18.5f, 0.20f, 20.5f));
            BuildFenceAlongLine(root, new Vector3(18.5f, 0.20f, 5.0f), new Vector3(18.5f, 0.20f, 20.5f));
            BuildFenceAlongLine(root, new Vector3(18.5f, 0.20f, -20.5f), new Vector3(18.5f, 0.20f, -5.0f));

            BuildGateWarningBarriers(root);
            BuildBillboard(root);
            BuildFloodlights(root);
        }

        private void BuildFenceAlongLine(Transform root, Vector3 start, Vector3 end)
        {
            float dist = Vector3.Distance(start, end);
            int count = Mathf.Max(1, Mathf.RoundToInt(dist / 2.2f));
            Vector3 dir = (end - start).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);

            for (int i = 0; i < count; i++)
            {
                Vector3 p = Vector3.Lerp(start, end, (float)i / count) + dir * 1.1f;

                if (prefabFence != null)
                {
                    GameObject fence = Instantiate(prefabFence, root);
                    fence.name = "Construction_Fence";
                    fence.transform.localPosition = p;
                    fence.transform.localRotation = rot;
                    ApplyMaterialRecursively(fence, matEnvironment);
                    BoxCollider col = fence.GetComponent<BoxCollider>();
                    if (col == null) col = fence.AddComponent<BoxCollider>();
                    col.size = new Vector3(0.15f, 2.0f, 2.2f);
                    col.center = new Vector3(0f, 1.0f, 0f);
                }
                else
                {
                    GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    panel.name = "Fence_Panel";
                    panel.transform.SetParent(root, false);
                    panel.transform.localPosition = p + Vector3.up * 1f;
                    panel.transform.localRotation = rot;
                    panel.transform.localScale = new Vector3(0.04f, 1.8f, 2.2f);
                    panel.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                }
            }
        }

        private void SnapToGround(GameObject obj, float targetBottomY = 0.20f)
        {
            if (obj == null) return;
            Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
            if (rends != null && rends.Length > 0)
            {
                Bounds b = rends[0].bounds;
                for (int r = 1; r < rends.Length; r++) b.Encapsulate(rends[r].bounds);
                float delta = targetBottomY - b.min.y;
                obj.transform.position += Vector3.up * delta;
            }
        }

        private void BuildGateWarningBarriers(Transform root)
        {
            Vector3[] barrierPositions = new Vector3[]
            {
                new Vector3(18.2f, 0.20f, 4.6f),
                new Vector3(18.2f, 0.20f, -4.6f),
                new Vector3(16.0f, 0.20f, 3.0f),
                new Vector3(16.0f, 0.20f, -3.0f)
            };

            for (int i = 0; i < barrierPositions.Length; i++)
            {
                GameObject bar = null;
                if (prefabBarrier != null)
                {
                    bar = Instantiate(prefabBarrier, root);
                    bar.name = "Construction_Barrier_" + i;
                    bar.transform.localPosition = barrierPositions[i];
                    bar.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
                    bar.transform.localScale = Vector3.one * 2.8f;
                    ApplyMaterialRecursively(bar, matEnvironment);

                    MeshFilter mf = bar.GetComponentInChildren<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                    {
                        MeshCollider mc = mf.gameObject.GetComponent<MeshCollider>();
                        if (mc == null) mc = mf.gameObject.AddComponent<MeshCollider>();
                    }
                    else if (bar.GetComponent<Collider>() == null)
                    {
                        bar.AddComponent<BoxCollider>();
                    }

                    SnapToGround(bar, 0.20f);
                }
                else
                {
                    bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bar.name = "Warning_Barrier_" + i;
                    bar.transform.SetParent(root, false);
                    bar.transform.localPosition = barrierPositions[i] + Vector3.up * 0.55f;
                    bar.transform.localScale = new Vector3(0.55f, 1.1f, 2.4f);
                    bar.GetComponent<Renderer>().sharedMaterial = matOrange;
                    SnapToGround(bar, 0.20f);
                }
            }

            Vector3[] conePositions = new Vector3[]
            {
                new Vector3(17.2f, 0.20f, 1.6f),
                new Vector3(17.2f, 0.20f, -1.6f),
                new Vector3(14.8f, 0.20f, 0f)
            };

            for (int i = 0; i < conePositions.Length; i++)
            {
                if (prefabCone != null)
                {
                    GameObject cone = Instantiate(prefabCone, root);
                    cone.name = "Traffic_Cone_" + i;
                    cone.transform.localPosition = conePositions[i];
                    cone.transform.localScale = Vector3.one * 3.2f;
                    ApplyMaterialRecursively(cone, matEnvironment);
                    if (cone.GetComponent<Collider>() == null) cone.AddComponent<BoxCollider>();
                    SnapToGround(cone, 0.20f);
                }
                else
                {
                    GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    cone.name = "Traffic_Cone_" + i;
                    cone.transform.SetParent(root, false);
                    cone.transform.localPosition = conePositions[i] + Vector3.up * 0.45f;
                    cone.transform.localScale = new Vector3(0.65f, 0.95f, 0.65f);
                    cone.GetComponent<Renderer>().sharedMaterial = matOrange;
                    SnapToGround(cone, 0.20f);
                }
            }
        }

        private void BuildBillboard(Transform root)
        {
            GameObject board = new GameObject("Site_Information_Board");
            board.transform.SetParent(root, false);
            board.transform.localPosition = new Vector3(18.2f, 0.20f, 8.0f);
            board.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postL.transform.SetParent(board.transform, false);
            postL.transform.localPosition = new Vector3(-1.6f, 1.8f, 0f);
            postL.transform.localScale = new Vector3(0.1f, 1.8f, 0.1f);
            postL.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postR.transform.SetParent(board.transform, false);
            postR.transform.localPosition = new Vector3(1.6f, 1.8f, 0f);
            postR.transform.localScale = new Vector3(0.1f, 1.8f, 0.1f);
            postR.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.transform.SetParent(board.transform, false);
            panel.transform.localPosition = new Vector3(0f, 2.4f, 0f);
            panel.transform.localScale = new Vector3(3.6f, 1.8f, 0.08f);
            panel.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject textPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            textPlate.transform.SetParent(board.transform, false);
            textPlate.transform.localPosition = new Vector3(0f, 2.4f, -0.05f);
            textPlate.transform.localScale = new Vector3(3.3f, 1.5f, 0.02f);
            textPlate.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(textPlate.GetComponent<Collider>());
        }

        private void BuildFloodlights(Transform root)
        {
            Vector3[] floodlightPos = new Vector3[]
            {
                new Vector3(-17.5f, 0.20f, 19.5f),
                new Vector3(17.5f, 0.20f, 19.5f),
                new Vector3(-17.5f, 0.20f, -19.5f)
            };

            for (int i = 0; i < floodlightPos.Length; i++)
            {
                GameObject pole = null;
                if (prefabLight != null)
                {
                    pole = Instantiate(prefabLight, root);
                    pole.name = "Construction_Light_Tower_" + i;
                    pole.transform.localPosition = floodlightPos[i];
                    pole.transform.localScale = Vector3.one * 1.5f;
                    ApplyMaterialRecursively(pole, matEnvironment);
                }
                else
                {
                    pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    pole.name = "Floodlight_Pole_" + i;
                    pole.transform.SetParent(root, false);
                    pole.transform.localPosition = floodlightPos[i] + Vector3.up * 4.8f;
                    pole.transform.localScale = new Vector3(0.16f, 4.8f, 0.16f);
                    pole.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                }

                GameObject lightObj = new GameObject("Light_Source");
                lightObj.transform.SetParent(pole.transform, false);
                lightObj.transform.localPosition = new Vector3(0f, 4.2f, 0f);
                Light l = lightObj.AddComponent<Light>();
                l.type = LightType.Spot;
                l.range = 38f;
                l.spotAngle = 78f;
                l.intensity = 2.6f;
                l.color = new Color(1f, 0.96f, 0.88f);
                lightObj.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            }
        }

        private void BuildSitePropsAndDumpsters(Transform root)
        {
            if (prefabDumpster != null)
            {
                GameObject dump1 = Instantiate(prefabDumpster, root);
                dump1.name = "Site_Dumpster_1";
                dump1.transform.localPosition = new Vector3(13.5f, 0.20f, 15.0f);
                dump1.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                dump1.transform.localScale = Vector3.one * 1.6f;
                ApplyMaterialRecursively(dump1, matEnvironment);
                if (dump1.GetComponent<Collider>() == null) dump1.AddComponent<BoxCollider>();

                GameObject dump2 = Instantiate(prefabDumpster, root);
                dump2.name = "Site_Dumpster_2";
                dump2.transform.localPosition = new Vector3(13.5f, 0.20f, 12.0f);
                dump2.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                dump2.transform.localScale = Vector3.one * 1.6f;
                ApplyMaterialRecursively(dump2, matEnvironment);
                if (dump2.GetComponent<Collider>() == null) dump2.AddComponent<BoxCollider>();
            }

            Vector3[] debrisSpots = new Vector3[]
            {
                new Vector3(11.5f, 0.22f, 8.5f),
                new Vector3(9.5f, 0.22f, -11.5f),
                new Vector3(-12.5f, 0.22f, 12.5f)
            };

            for (int i = 0; i < debrisSpots.Length; i++)
            {
                if (prefabDebrisPlate != null)
                {
                    GameObject dp = Instantiate(prefabDebrisPlate, root);
                    dp.name = "Debris_Plate_" + i;
                    dp.transform.localPosition = debrisSpots[i];
                    dp.transform.localScale = Vector3.one * 1.5f;
                    ApplyMaterialRecursively(dp, matVehicles);
                    Collider c = dp.GetComponent<Collider>();
                    if (c != null) Object.Destroy(c);
                }
                if (prefabDebrisBolt != null)
                {
                    GameObject db = Instantiate(prefabDebrisBolt, root);
                    db.name = "Debris_Bolt_" + i;
                    db.transform.localPosition = debrisSpots[i] + new Vector3(0.4f, 0f, 0.3f);
                    db.transform.localScale = Vector3.one * 1.5f;
                    ApplyMaterialRecursively(db, matVehicles);
                    Collider c = db.GetComponent<Collider>();
                    if (c != null) Object.Destroy(c);
                }
            }
        }

        private void BuildExcavationPit(Transform root)
        {
            GameObject pit = new GameObject("Excavation_Pit");
            pit.transform.SetParent(root, false);
            pit.transform.localPosition = new Vector3(-3.5f, 0.0f, 5.5f);

            // Perimeter safety retaining walls (West and North only, East and South open for vehicle access)
            CreateWall("Pit_Wall_N", pit.transform, new Vector3(-1.5f, 0.45f, 5.5f), new Vector3(10f, 0.9f, 0.5f), matConcrete);
            CreateWall("Pit_Wall_W", pit.transform, new Vector3(-6.4f, 0.45f, 0f), new Vector3(0.5f, 0.9f, 11f), matConcrete);

            // Level excavation pit bed (flush with site ground, no collision bumps!)
            GameObject pitFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pitFloor.name = "Pit_Floor";
            pitFloor.transform.SetParent(pit.transform, false);
            pitFloor.transform.localPosition = new Vector3(-0.5f, 0.01f, 0f);
            pitFloor.transform.localScale = new Vector3(11.5f, 0.02f, 10.5f);
            pitFloor.GetComponent<Renderer>().sharedMaterial = matDirt;
            StripColliders(pitFloor);

            // Soil mounds as visual terrain contours (colliders stripped so tracks roll over freely without wedging)
            GameObject mound1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mound1.name = "Soil_Mound_1";
            mound1.transform.SetParent(pit.transform, false);
            mound1.transform.localPosition = new Vector3(-3.5f, 0.25f, 2.5f);
            mound1.transform.localScale = new Vector3(5.0f, 1.2f, 4.5f);
            mound1.GetComponent<Renderer>().sharedMaterial = matDirt;
            StripColliders(mound1);

            GameObject mound2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mound2.name = "Soil_Mound_2";
            mound2.transform.SetParent(pit.transform, false);
            mound2.transform.localPosition = new Vector3(-1.5f, 0.20f, -2.5f);
            mound2.transform.localScale = new Vector3(4.5f, 1.0f, 4.0f);
            mound2.GetComponent<Renderer>().sharedMaterial = matDirt;
            StripColliders(mound2);
        }

        private void BuildFoundationUnderConstruction(Transform root)
        {
            GameObject fnd = new GameObject("Foundation_Zone");
            fnd.transform.SetParent(root, false);
            fnd.transform.localPosition = new Vector3(-6f, 0.20f, -10f);

            CreateWall("Footing_Base", fnd.transform, new Vector3(0f, 0.15f, 0f), new Vector3(15f, 0.35f, 13f), matConcrete);

            Vector3[] colPositions = new Vector3[]
            {
                new Vector3(-5.5f, 1.6f, -4.5f),
                new Vector3(0f, 1.6f, -4.5f),
                new Vector3(5.5f, 1.6f, -4.5f),
                new Vector3(-5.5f, 1.6f, 4.5f),
                new Vector3(0f, 1.6f, 4.5f),
                new Vector3(5.5f, 1.6f, 4.5f)
            };

            for (int i = 0; i < colPositions.Length; i++)
            {
                GameObject col = GameObject.CreatePrimitive(PrimitiveType.Cube);
                col.name = "Concrete_Column_" + i;
                col.transform.SetParent(fnd.transform, false);
                col.transform.localPosition = colPositions[i];
                col.transform.localScale = new Vector3(1.1f, 3.0f, 1.1f);
                col.GetComponent<Renderer>().sharedMaterial = matConcrete;

                for (int r = 0; r < 4; r++)
                {
                    GameObject rebar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    rebar.name = "Rebar_" + r;
                    rebar.transform.SetParent(col.transform, false);
                    float rx = (r % 2 == 0) ? -0.35f : 0.35f;
                    float rz = (r < 2) ? -0.35f : 0.35f;
                    rebar.transform.localPosition = new Vector3(rx, 0.72f, rz);
                    rebar.transform.localScale = new Vector3(0.06f, 0.38f, 0.06f);
                    rebar.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                    Object.Destroy(rebar.GetComponent<Collider>());
                }
            }
        }

        private void SpawnGranularCargo(Transform root)
        {
            GameObject cargoRoot = new GameObject("Granular_Piles");
            cargoRoot.transform.SetParent(root, false);

            // 1. PRIMARY EXCAVATION TRENCH (directly in front of excavator bucket reach!)
            for (int i = 0; i < 20; i++)
            {
                float rx = Random.Range(1.6f, 3.6f);
                float rz = Random.Range(3.6f, 6.4f);
                float ry = Random.Range(0.25f, 0.45f);

                GameObject sand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sand.name = "Sand_Pebble_" + i;
                sand.transform.SetParent(cargoRoot.transform, false);
                sand.transform.localPosition = new Vector3(rx, ry, rz);
                sand.transform.localScale = Vector3.one * Random.Range(0.35f, 0.55f);
                sand.GetComponent<Renderer>().sharedMaterial = matSand;

                GranularItem gi = sand.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Sand;

                PickupableItem pi = sand.AddComponent<PickupableItem>();
                pi.itemName = "Песчаный окатыш";
                pi.rb = sand.GetComponent<Rigidbody>();
            }

            for (int i = 0; i < 16; i++)
            {
                float rx = Random.Range(1.6f, 3.6f);
                float rz = Random.Range(3.6f, 6.4f);
                float ry = Random.Range(0.30f, 0.55f);

                GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rock.name = "Rock_Stone_" + i;
                rock.transform.SetParent(cargoRoot.transform, false);
                rock.transform.localPosition = new Vector3(rx, ry, rz);
                rock.transform.localRotation = Random.rotation;
                rock.transform.localScale = new Vector3(Random.Range(0.50f, 0.85f), Random.Range(0.40f, 0.65f), Random.Range(0.50f, 0.85f));
                rock.GetComponent<Renderer>().sharedMaterial = matRock;

                GranularItem gi = rock.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Rock;

                PickupableItem pi = rock.AddComponent<PickupableItem>();
                pi.itemName = "Строительный камень";
                pi.rb = rock.GetComponent<Rigidbody>();
            }

            for (int i = 0; i < 12; i++)
            {
                float bx = Random.Range(1.6f, 3.4f);
                float bz = Random.Range(3.6f, 6.2f);
                float by = 0.20f + (i % 3) * 0.26f;

                GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                brick.name = "Brick_" + i;
                brick.transform.SetParent(cargoRoot.transform, false);
                brick.transform.localPosition = new Vector3(bx, by, bz);
                brick.transform.localRotation = Quaternion.Euler(0f, (i % 2) * 90f, 0f);
                brick.transform.localScale = new Vector3(0.48f, 0.24f, 0.28f);
                brick.GetComponent<Renderer>().sharedMaterial = matBrick;

                GranularItem gi = brick.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Brick;

                PickupableItem pi = brick.AddComponent<PickupableItem>();
                pi.itemName = "Красный кирпич";
                pi.rb = brick.GetComponent<Rigidbody>();
            }

            // 2. BULLDOZER EARTHMOVING ZONE (northern trench in front of bulldozer blade!)
            for (int i = 20; i < 35; i++)
            {
                float rx = Random.Range(1.8f, 4.4f);
                float rz = Random.Range(12.0f, 15.0f);
                float ry = Random.Range(0.25f, 0.45f);

                GameObject sand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sand.name = "Sand_Pebble_" + i;
                sand.transform.SetParent(cargoRoot.transform, false);
                sand.transform.localPosition = new Vector3(rx, ry, rz);
                sand.transform.localScale = Vector3.one * Random.Range(0.35f, 0.55f);
                sand.GetComponent<Renderer>().sharedMaterial = matSand;

                GranularItem gi = sand.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Sand;

                PickupableItem pi = sand.AddComponent<PickupableItem>();
                pi.itemName = "Песчаный окатыш";
                pi.rb = sand.GetComponent<Rigidbody>();
            }

            for (int i = 16; i < 26; i++)
            {
                float rx = Random.Range(1.8f, 4.4f);
                float rz = Random.Range(12.0f, 15.0f);
                float ry = Random.Range(0.30f, 0.55f);

                GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rock.name = "Rock_Stone_" + i;
                rock.transform.SetParent(cargoRoot.transform, false);
                rock.transform.localPosition = new Vector3(rx, ry, rz);
                rock.transform.localRotation = Random.rotation;
                rock.transform.localScale = new Vector3(Random.Range(0.50f, 0.85f), Random.Range(0.40f, 0.65f), Random.Range(0.50f, 0.85f));
                rock.GetComponent<Renderer>().sharedMaterial = matRock;

                GranularItem gi = rock.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Rock;

                PickupableItem pi = rock.AddComponent<PickupableItem>();
                pi.itemName = "Строительный камень";
                pi.rb = rock.GetComponent<Rigidbody>();
            }

            // 3. HAUL ROAD & LOADING ZONE (stacked bricks near dump truck)
            for (int i = 12; i < 24; i++)
            {
                float bx = Random.Range(1.8f, 3.8f);
                float bz = Random.Range(-8.0f, -5.0f);
                float by = 0.20f + ((i - 12) % 3) * 0.26f;

                GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                brick.name = "Brick_" + i;
                brick.transform.SetParent(cargoRoot.transform, false);
                brick.transform.localPosition = new Vector3(bx, by, bz);
                brick.transform.localRotation = Quaternion.Euler(0f, (i % 2) * 90f, 0f);
                brick.transform.localScale = new Vector3(0.48f, 0.24f, 0.28f);
                brick.GetComponent<Renderer>().sharedMaterial = matBrick;

                GranularItem gi = brick.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Brick;

                PickupableItem pi = brick.AddComponent<PickupableItem>();
                pi.itemName = "Красный кирпич";
                pi.rb = brick.GetComponent<Rigidbody>();
            }

            SpawnCraneContainers(cargoRoot.transform);
        }

        private void SpawnCraneContainers(Transform cargoRoot)
        {
            // Cargo 1: Heavy Industrial Blue Site Container
            CreateSiteContainer("Cargo_Site_Container_Blue", cargoRoot, new Vector3(-12f, 0.22f, -8f), matDarkMetal, new Color(0.18f, 0.42f, 0.72f), 450f);

            // Cargo 2: Heavy Industrial Red Equipment Container
            CreateSiteContainer("Cargo_Site_Container_Red", cargoRoot, new Vector3(-11f, 0.22f, 0f), matDarkMetal, new Color(0.85f, 0.22f, 0.18f), 450f);

            // Cargo 3: Stacked Precast Reinforced Concrete Slabs
            CreateConcreteSlabBundle("Cargo_Concrete_Slabs", cargoRoot, new Vector3(-8f, 0.22f, -4f), 380f);

            // Cargo 4: Timber Euro-Pallet with Stacked Red Bricks
            CreateBrickPalletBundle("Cargo_Brick_Pallet", cargoRoot, new Vector3(-7f, 0.22f, 4f), 240f);
        }

        private GameObject CreateSiteContainer(string name, Transform parent, Vector3 localPos, Material trimMat, Color bodyColor, float mass)
        {
            GameObject container = new GameObject(name);
            container.transform.SetParent(parent, false);
            container.transform.localPosition = localPos;

            Material bodyMat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            bodyMat.color = bodyColor;

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Container_Shell";
            body.transform.SetParent(container.transform, false);
            body.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            body.transform.localScale = new Vector3(2.4f, 2.3f, 4.8f);
            body.GetComponent<Renderer>().sharedMaterial = bodyMat;
            Object.Destroy(body.GetComponent<Collider>());

            Vector3[] postPositions = new Vector3[]
            {
                new Vector3(-1.18f, 1.15f, -2.38f),
                new Vector3(1.18f, 1.15f, -2.38f),
                new Vector3(-1.18f, 1.15f, 2.38f),
                new Vector3(1.18f, 1.15f, 2.38f)
            };
            for (int i = 0; i < postPositions.Length; i++)
            {
                GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cube);
                post.name = "Corner_Post_" + i;
                post.transform.SetParent(container.transform, false);
                post.transform.localPosition = postPositions[i];
                post.transform.localScale = new Vector3(0.14f, 2.34f, 0.14f);
                post.GetComponent<Renderer>().sharedMaterial = trimMat;
                Object.Destroy(post.GetComponent<Collider>());
            }

            BoxCollider bc = container.AddComponent<BoxCollider>();
            bc.size = new Vector3(2.45f, 2.3f, 4.85f);
            bc.center = new Vector3(0f, 1.15f, 0f);

            Rigidbody rb = container.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 2.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.isKinematic = true;

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Hoist_Ring";
            ring.transform.SetParent(container.transform, false);
            ring.transform.localPosition = new Vector3(0f, 2.45f, 0f);
            ring.transform.localScale = new Vector3(0.35f, 0.22f, 0.35f);
            ring.GetComponent<Renderer>().sharedMaterial = trimMat;
            Object.Destroy(ring.GetComponent<Collider>());

            return container;
        }

        private GameObject CreateConcreteSlabBundle(string name, Transform parent, Vector3 localPos, float mass)
        {
            GameObject bundle = new GameObject(name);
            bundle.transform.SetParent(parent, false);
            bundle.transform.localPosition = localPos;

            for (int i = 0; i < 3; i++)
            {
                GameObject slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slab.name = "Concrete_Slab_" + i;
                slab.transform.SetParent(bundle.transform, false);
                slab.transform.localPosition = new Vector3(0f, 0.18f + i * 0.34f, 0f);
                slab.transform.localScale = new Vector3(2.2f, 0.26f, 3.6f);
                slab.GetComponent<Renderer>().sharedMaterial = matConcrete;
                Object.Destroy(slab.GetComponent<Collider>());
            }

            BoxCollider bc = bundle.AddComponent<BoxCollider>();
            bc.size = new Vector3(2.25f, 1.05f, 3.65f);
            bc.center = new Vector3(0f, 0.52f, 0f);

            Rigidbody rb = bundle.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 2.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.isKinematic = true;

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Hoist_Ring";
            ring.transform.SetParent(bundle.transform, false);
            ring.transform.localPosition = new Vector3(0f, 1.20f, 0f);
            ring.transform.localScale = new Vector3(0.35f, 0.20f, 0.35f);
            ring.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(ring.GetComponent<Collider>());

            return bundle;
        }

        private GameObject CreateBrickPalletBundle(string name, Transform parent, Vector3 localPos, float mass)
        {
            GameObject pallet = new GameObject(name);
            pallet.transform.SetParent(parent, false);
            pallet.transform.localPosition = localPos;

            GameObject basePallet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            basePallet.name = "Wood_Pallet_Base";
            basePallet.transform.SetParent(pallet.transform, false);
            basePallet.transform.localPosition = new Vector3(0f, 0.10f, 0f);
            basePallet.transform.localScale = new Vector3(1.8f, 0.18f, 1.8f);
            basePallet.GetComponent<Renderer>().sharedMaterial = matWood;
            Object.Destroy(basePallet.GetComponent<Collider>());

            GameObject brickStack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            brickStack.name = "Brick_Stack_Block";
            brickStack.transform.SetParent(pallet.transform, false);
            brickStack.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            brickStack.transform.localScale = new Vector3(1.6f, 0.90f, 1.6f);
            brickStack.GetComponent<Renderer>().sharedMaterial = matBrick;
            Object.Destroy(brickStack.GetComponent<Collider>());

            BoxCollider bc = pallet.AddComponent<BoxCollider>();
            bc.size = new Vector3(1.8f, 1.15f, 1.8f);
            bc.center = new Vector3(0f, 0.58f, 0f);

            Rigidbody rb = pallet.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 2.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.isKinematic = true;

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Hoist_Ring";
            ring.transform.SetParent(pallet.transform, false);
            ring.transform.localPosition = new Vector3(0f, 1.28f, 0f);
            ring.transform.localScale = new Vector3(0.32f, 0.18f, 0.32f);
            ring.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(ring.GetComponent<Collider>());

            return pallet;
        }

        private void BuildBulldozer(Transform root)
        {
            GameObject dozer = new GameObject("Bulldozer");
            dozer.transform.SetParent(root, false);
            // Stationed in the northern staging zone, facing west
            dozer.transform.localPosition = new Vector3(7.2f, 0.22f, 13.5f);
            dozer.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            // ==========================================
            // 1. CRAWLER UNDERCARRIAGE
            // ==========================================
            GameObject undercarriage = new GameObject("Undercarriage");
            undercarriage.transform.SetParent(dozer.transform, false);
            undercarriage.transform.localPosition = Vector3.zero;

            // Center carbody chassis
            GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chassis.name = "Chassis_Frame";
            chassis.transform.SetParent(undercarriage.transform, false);
            chassis.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            chassis.transform.localScale = new Vector3(1.7f, 0.45f, 3.4f);
            chassis.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(chassis);

            // Left & Right crawler tracks
            float[] trackX = new float[] { -1.15f, 1.15f };
            for (int t = 0; t < 2; t++)
            {
                string side = t == 0 ? "Track_L" : "Track_R";
                float tx = trackX[t];

                GameObject pontoon = new GameObject(side);
                pontoon.transform.SetParent(undercarriage.transform, false);
                pontoon.transform.localPosition = new Vector3(tx, 0.38f, 0f);

                // Heavy side frame beam
                GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
                frame.name = "Track_Frame";
                frame.transform.SetParent(pontoon.transform, false);
                frame.transform.localPosition = Vector3.zero;
                frame.transform.localScale = new Vector3(0.55f, 0.58f, 3.8f);
                frame.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(frame);

                // Front idler drum
                GameObject idler = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                idler.name = "Idler_Front";
                idler.transform.SetParent(pontoon.transform, false);
                idler.transform.localPosition = new Vector3(0f, 0f, 1.8f);
                idler.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                idler.transform.localScale = new Vector3(0.56f, 0.28f, 0.56f);
                idler.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(idler);

                // Rear drive sprocket drum
                GameObject sprocket = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                sprocket.name = "Sprocket_Rear";
                sprocket.transform.SetParent(pontoon.transform, false);
                sprocket.transform.localPosition = new Vector3(0f, 0f, -1.8f);
                sprocket.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                sprocket.transform.localScale = new Vector3(0.56f, 0.28f, 0.56f);
                sprocket.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(sprocket);

                // Upper tread
                GameObject topTread = GameObject.CreatePrimitive(PrimitiveType.Cube);
                topTread.name = "Tread_Top";
                topTread.transform.SetParent(pontoon.transform, false);
                topTread.transform.localPosition = new Vector3(0f, 0.30f, 0f);
                topTread.transform.localScale = new Vector3(0.60f, 0.08f, 3.7f);
                topTread.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(topTread);

                // Lower tread
                GameObject btmTread = GameObject.CreatePrimitive(PrimitiveType.Cube);
                btmTread.name = "Tread_Bottom";
                btmTread.transform.SetParent(pontoon.transform, false);
                btmTread.transform.localPosition = new Vector3(0f, -0.30f, 0f);
                btmTread.transform.localScale = new Vector3(0.60f, 0.08f, 3.7f);
                btmTread.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(btmTread);
            }

            // Undercarriage base box collider
            BoxCollider baseCol = dozer.AddComponent<BoxCollider>();
            baseCol.size = new Vector3(2.9f, 0.68f, 3.9f);
            baseCol.center = new Vector3(0f, 0.42f, 0f);

            // ==========================================
            // 2. ENGINE HOOD & OPERATOR CAB
            // ==========================================
            GameObject hood = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hood.name = "Engine_Hood";
            hood.transform.SetParent(dozer.transform, false);
            hood.transform.localPosition = new Vector3(0f, 0.95f, 0.65f);
            hood.transform.localScale = new Vector3(1.5f, 0.95f, 1.8f);
            hood.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(hood);

            GameObject exhaust = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            exhaust.name = "Exhaust_Pipe";
            exhaust.transform.SetParent(dozer.transform, false);
            exhaust.transform.localPosition = new Vector3(0.55f, 1.65f, 0.35f);
            exhaust.transform.localScale = new Vector3(0.12f, 0.65f, 0.12f);
            exhaust.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(exhaust);

            GameObject grille = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grille.name = "Radiator_Grille";
            grille.transform.SetParent(dozer.transform, false);
            grille.transform.localPosition = new Vector3(0f, 0.95f, 1.56f);
            grille.transform.localScale = new Vector3(1.3f, 0.8f, 0.06f);
            grille.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(grille);

            GameObject cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cab.name = "Operator_Cab";
            cab.transform.SetParent(dozer.transform, false);
            cab.transform.localPosition = new Vector3(0f, 1.45f, -0.65f);
            cab.transform.localScale = new Vector3(1.55f, 1.25f, 1.45f);
            cab.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(cab);

            GameObject windshield = GameObject.CreatePrimitive(PrimitiveType.Cube);
            windshield.name = "Cab_Windshield";
            windshield.transform.SetParent(cab.transform, false);
            windshield.transform.localPosition = new Vector3(0f, 0.12f, 0.51f);
            windshield.transform.localScale = new Vector3(0.85f, 0.65f, 0.05f);
            windshield.GetComponent<Renderer>().sharedMaterial = matGlass;
            StripColliders(windshield);

            GameObject cabRoof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabRoof.name = "Cab_Roof";
            cabRoof.transform.SetParent(cab.transform, false);
            cabRoof.transform.localPosition = new Vector3(0f, 0.52f, 0f);
            cabRoof.transform.localScale = new Vector3(1.08f, 0.10f, 1.08f);
            cabRoof.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(cabRoof);

            GameObject ripper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ripper.name = "Rear_Ripper_Beam";
            ripper.transform.SetParent(dozer.transform, false);
            ripper.transform.localPosition = new Vector3(0f, 0.55f, -1.95f);
            ripper.transform.localScale = new Vector3(1.8f, 0.20f, 0.35f);
            ripper.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(ripper);

            for (int r = 0; r < 3; r++)
            {
                GameObject shank = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shank.name = "Ripper_Shank_" + r;
                shank.transform.SetParent(ripper.transform, false);
                shank.transform.localPosition = new Vector3(-0.65f + r * 0.65f, -0.32f, 0f);
                shank.transform.localScale = new Vector3(0.12f, 0.65f, 0.16f);
                shank.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(shank);
            }

            // ==========================================
            // 3. FRONT BULLDOZER BLADE (MOLDBOARD)
            // ==========================================
            GameObject bladeAssembly = new GameObject("Blade_Assembly");
            bladeAssembly.transform.SetParent(dozer.transform, false);
            bladeAssembly.transform.localPosition = new Vector3(0f, 0.22f, 2.15f);

            float[] armX = new float[] { -0.95f, 0.95f };
            for (int a = 0; a < 2; a++)
            {
                GameObject pushArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pushArm.name = "Push_Arm_" + a;
                pushArm.transform.SetParent(bladeAssembly.transform, false);
                pushArm.transform.localPosition = new Vector3(armX[a], 0.15f, -0.55f);
                pushArm.transform.localScale = new Vector3(0.16f, 0.22f, 1.25f);
                pushArm.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(pushArm);
            }

            GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blade.name = "Blade_Moldboard";
            blade.transform.SetParent(bladeAssembly.transform, false);
            blade.transform.localPosition = new Vector3(0f, 0.42f, 0.15f);
            blade.transform.localScale = new Vector3(3.2f, 0.95f, 0.30f);
            blade.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(blade);

            GameObject cuttingEdge = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cuttingEdge.name = "Cutting_Edge";
            cuttingEdge.transform.SetParent(blade.transform, false);
            cuttingEdge.transform.localPosition = new Vector3(0f, -0.48f, 0.05f);
            cuttingEdge.transform.localScale = new Vector3(1.0f, 0.12f, 0.20f);
            cuttingEdge.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(cuttingEdge);

            for (int sp = 0; sp < 2; sp++)
            {
                GameObject sidePlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sidePlate.name = "Side_Wing_" + sp;
                sidePlate.transform.SetParent(blade.transform, false);
                sidePlate.transform.localPosition = new Vector3(sp == 0 ? -0.49f : 0.49f, 0f, 0.15f);
                sidePlate.transform.localScale = new Vector3(0.04f, 0.95f, 0.40f);
                sidePlate.GetComponent<Renderer>().sharedMaterial = matYellow;
                StripColliders(sidePlate);
            }

            BoxCollider bladeCol = bladeAssembly.AddComponent<BoxCollider>();
            bladeCol.size = new Vector3(3.25f, 0.95f, 0.45f);
            bladeCol.center = new Vector3(0f, 0.42f, 0.15f);

            PhysicsMaterial bladePhysMat = new PhysicsMaterial("BladePhysMat")
            {
                dynamicFriction = 0.15f,
                staticFriction = 0.25f,
                bounciness = 0.02f
            };
            bladeCol.sharedMaterial = bladePhysMat;

            // ==========================================
            // 4. CAMERA & CONTROLLER
            // ==========================================
            GameObject camObj = new GameObject("Bulldozer_Camera");
            camObj.transform.SetParent(dozer.transform, false);
            camObj.transform.localPosition = new Vector3(0f, 3.4f, -4.8f);
            camObj.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(dozer.transform, false);
            exitObj.transform.localPosition = new Vector3(2.4f, 0f, -0.6f);

            BulldozerController bc = dozer.AddComponent<BulldozerController>();
            bc.bladeTransform = bladeAssembly.transform;
            bc.wheels = null;
            bc.vehicleCamera = cam;
            bc.exitTransform = exitObj.transform;
            bc.bladeMin = 0.05f;
            bc.bladeMax = 1.25f;
            bc.bladeSpeed = 0.85f;
        }

        private void BuildDumpTruck(Transform root)
        {
            GameObject truck = null;

            if (prefabTruckFlat != null)
            {
                truck = Instantiate(prefabTruckFlat, root);
                truck.name = "Dump_Truck";
                truck.transform.localPosition = new Vector3(7.2f, 0.22f, -11.5f);
                truck.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                truck.transform.localScale = Vector3.one * 1.6f;
                ApplyMaterialRecursively(truck, matVehicles);
                StripColliders(truck);
            }
            else
            {
                truck = new GameObject("Dump_Truck");
                truck.transform.SetParent(root, false);
                truck.transform.localPosition = new Vector3(8.0f, 0.22f, -13.0f);
                truck.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

                GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chassis.name = "Frame_Chassis";
                chassis.transform.SetParent(truck.transform, false);
                chassis.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                chassis.transform.localScale = new Vector3(2.1f, 0.5f, 5.8f);
                chassis.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            }

            // Cab collider ONLY (covers front driver cab, NEVER overlaps the dump bed!)
            BoxCollider cabCol = truck.AddComponent<BoxCollider>();
            cabCol.size = new Vector3(2.2f, 1.8f, 1.6f);
            cabCol.center = new Vector3(0f, 0.95f, 1.85f);

            // Find existing 4 wheels from the model
            List<Transform> frontWheels = new List<Transform>();
            List<Transform> rearWheels = new List<Transform>();

            Transform wfl = FindChildDeep(truck.transform, "wheel-front-left");
            Transform wfr = FindChildDeep(truck.transform, "wheel-front-right");
            Transform wbl = FindChildDeep(truck.transform, "wheel-back-left");
            Transform wbr = FindChildDeep(truck.transform, "wheel-back-right");

            if (wfl != null) { frontWheels.Add(wfl); StripColliders(wfl.gameObject); }
            if (wfr != null) { frontWheels.Add(wfr); StripColliders(wfr.gameObject); }
            if (wbl != null) { rearWheels.Add(wbl); StripColliders(wbl.gameObject); }
            if (wbr != null) { rearWheels.Add(wbr); StripColliders(wbr.gameObject); }

            // Rear hinge placed at the very rear of the frame (Z = -1.9f)
            GameObject bedPivot = new GameObject("Dump_Bed_Pivot");
            bedPivot.transform.SetParent(truck.transform, false);
            bedPivot.transform.localPosition = new Vector3(0f, 0.95f, -1.9f);

            GameObject dumpBed = new GameObject("Dump_Bed_Body");
            dumpBed.transform.SetParent(bedPivot.transform, false);
            dumpBed.transform.localPosition = Vector3.zero;

            PhysicsMaterial bedPhysMat = new PhysicsMaterial("BedCargoMat")
            {
                dynamicFriction = 0.2f,
                staticFriction = 0.25f,
                bounciness = 0f,
                bounceCombine = PhysicsMaterialCombine.Minimum,
                frictionCombine = PhysicsMaterialCombine.Average
            };

            // Bed Floor (holds cargo directly on the floor with NO floating invisible colliders!)
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Bed_Floor";
            floor.transform.SetParent(dumpBed.transform, false);
            floor.transform.localPosition = new Vector3(0f, 0.08f, 1.6f);
            floor.transform.localScale = new Vector3(2.1f, 0.14f, 3.2f);
            floor.GetComponent<Renderer>().sharedMaterial = matOrange;
            StripColliders(floor);
            BoxCollider bedFloorCol = floor.AddComponent<BoxCollider>();
            bedFloorCol.size = new Vector3(2.1f, 0.14f, 3.2f);
            bedFloorCol.center = Vector3.zero;
            bedFloorCol.sharedMaterial = bedPhysMat;

            // Bed Left Wall
            GameObject wallL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallL.name = "Bed_Wall_Left";
            wallL.transform.SetParent(dumpBed.transform, false);
            wallL.transform.localPosition = new Vector3(-1.0f, 0.55f, 1.6f);
            wallL.transform.localScale = new Vector3(0.1f, 0.9f, 3.2f);
            wallL.GetComponent<Renderer>().sharedMaterial = matOrange;
            StripColliders(wallL);
            BoxCollider colL = wallL.AddComponent<BoxCollider>();
            colL.size = new Vector3(0.12f, 0.95f, 3.2f);
            colL.center = Vector3.zero;
            colL.sharedMaterial = bedPhysMat;

            // Bed Right Wall
            GameObject wallR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallR.name = "Bed_Wall_Right";
            wallR.transform.SetParent(dumpBed.transform, false);
            wallR.transform.localPosition = new Vector3(1.0f, 0.55f, 1.6f);
            wallR.transform.localScale = new Vector3(0.1f, 0.9f, 3.2f);
            wallR.GetComponent<Renderer>().sharedMaterial = matOrange;
            StripColliders(wallR);
            BoxCollider colR = wallR.AddComponent<BoxCollider>();
            colR.size = new Vector3(0.12f, 0.95f, 3.2f);
            colR.center = Vector3.zero;
            colR.sharedMaterial = bedPhysMat;

            // Bed Front Wall
            GameObject wallFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallFront.name = "Bed_Wall_Front";
            wallFront.transform.SetParent(dumpBed.transform, false);
            wallFront.transform.localPosition = new Vector3(0f, 0.75f, 3.2f);
            wallFront.transform.localScale = new Vector3(2.1f, 1.3f, 0.12f);
            wallFront.GetComponent<Renderer>().sharedMaterial = matOrange;
            StripColliders(wallFront);
            BoxCollider colFront = wallFront.AddComponent<BoxCollider>();
            colFront.size = new Vector3(2.1f, 1.3f, 0.14f);
            colFront.center = Vector3.zero;
            colFront.sharedMaterial = bedPhysMat;

            // Cab protector visor
            GameObject cabVisor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabVisor.name = "Cab_Protector_Visor";
            cabVisor.transform.SetParent(dumpBed.transform, false);
            cabVisor.transform.localPosition = new Vector3(0f, 1.45f, 3.7f);
            cabVisor.transform.localScale = new Vector3(2.1f, 0.1f, 1.0f);
            cabVisor.GetComponent<Renderer>().sharedMaterial = matOrange;
            StripColliders(cabVisor);

            // Tailgate Leaf
            GameObject tailgatePivot = new GameObject("Tailgate_Pivot");
            tailgatePivot.transform.SetParent(dumpBed.transform, false);
            tailgatePivot.transform.localPosition = new Vector3(0f, 0.95f, 0f);

            GameObject tailgate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tailgate.name = "Tailgate_Leaf";
            tailgate.transform.SetParent(tailgatePivot.transform, false);
            tailgate.transform.localPosition = new Vector3(0f, -0.38f, 0f);
            tailgate.transform.localScale = new Vector3(2.05f, 0.74f, 0.1f);
            tailgate.GetComponent<Renderer>().sharedMaterial = matOrange;
            StripColliders(tailgate);
            BoxCollider colTail = tailgate.AddComponent<BoxCollider>();
            colTail.size = new Vector3(2.05f, 0.74f, 0.12f);
            colTail.center = Vector3.zero;
            colTail.sharedMaterial = bedPhysMat;

            GameObject camObj = new GameObject("Truck_Camera");
            camObj.transform.SetParent(truck.transform, false);
            camObj.transform.localPosition = new Vector3(0f, 3.6f, -6.5f);
            camObj.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(truck.transform, false);
            exitObj.transform.localPosition = new Vector3(-2.4f, 0f, 1.75f);

            DumpTruckController dtc = truck.AddComponent<DumpTruckController>();
            dtc.dumpBed = bedPivot.transform;
            dtc.tailgate = tailgatePivot.transform;
            dtc.frontWheels = frontWheels.ToArray();
            dtc.rearWheels = rearWheels.ToArray();
            dtc.vehicleCamera = cam;
            dtc.exitTransform = exitObj.transform;
            dtc.maxTiltAngle = 50f;
            dtc.tiltSpeed = 24f;
        }

        private void BuildExcavator(Transform root)
        {
            GameObject exc = new GameObject("Excavator");
            exc.transform.SetParent(root, false);
            // Stationed on level ground east of the excavation trench, facing west toward the pit
            exc.transform.localPosition = new Vector3(6.0f, 0.22f, 5.0f);
            exc.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            // ==========================================
            // 1. CRAWLER UNDERCARRIAGE (NO RUBBER WHEELS)
            // ==========================================
            GameObject undercarriage = new GameObject("Undercarriage");
            undercarriage.transform.SetParent(exc.transform, false);
            undercarriage.transform.localPosition = Vector3.zero;

            // Center carbody frame (connecting left and right track assemblies)
            GameObject carbody = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carbody.name = "Center_Carbody";
            carbody.transform.SetParent(undercarriage.transform, false);
            carbody.transform.localPosition = new Vector3(0f, 0.42f, 0f);
            carbody.transform.localScale = new Vector3(1.6f, 0.40f, 2.6f);
            carbody.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(carbody);

            // Left & Right continuous crawler track pontoons
            float[] trackXOffsets = new float[] { -1.35f, 1.35f };
            for (int t = 0; t < 2; t++)
            {
                string side = t == 0 ? "Track_Left" : "Track_Right";
                float tx = trackXOffsets[t];

                GameObject pontoon = new GameObject(side);
                pontoon.transform.SetParent(undercarriage.transform, false);
                pontoon.transform.localPosition = new Vector3(tx, 0.40f, 0f);

                // Heavy side frame beam
                GameObject trackBeam = GameObject.CreatePrimitive(PrimitiveType.Cube);
                trackBeam.name = "Track_Frame";
                trackBeam.transform.SetParent(pontoon.transform, false);
                trackBeam.transform.localPosition = Vector3.zero;
                trackBeam.transform.localScale = new Vector3(0.70f, 0.65f, 4.4f);
                trackBeam.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(trackBeam);

                // Front idler drum
                GameObject idler = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                idler.name = "Idler_Front";
                idler.transform.SetParent(pontoon.transform, false);
                idler.transform.localPosition = new Vector3(0f, 0f, 2.1f);
                idler.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                idler.transform.localScale = new Vector3(0.62f, 0.36f, 0.62f);
                idler.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(idler);

                // Rear drive sprocket drum
                GameObject sprocket = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                sprocket.name = "Sprocket_Rear";
                sprocket.transform.SetParent(pontoon.transform, false);
                sprocket.transform.localPosition = new Vector3(0f, 0f, -2.1f);
                sprocket.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                sprocket.transform.localScale = new Vector3(0.62f, 0.36f, 0.62f);
                sprocket.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(sprocket);

                // Upper tread run
                GameObject topRun = GameObject.CreatePrimitive(PrimitiveType.Cube);
                topRun.name = "Tread_Top";
                topRun.transform.SetParent(pontoon.transform, false);
                topRun.transform.localPosition = new Vector3(0f, 0.35f, 0f);
                topRun.transform.localScale = new Vector3(0.75f, 0.08f, 4.3f);
                topRun.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(topRun);

                // Lower tread run (resting on ground)
                GameObject btmRun = GameObject.CreatePrimitive(PrimitiveType.Cube);
                btmRun.name = "Tread_Bottom";
                btmRun.transform.SetParent(pontoon.transform, false);
                btmRun.transform.localPosition = new Vector3(0f, -0.34f, 0f);
                btmRun.transform.localScale = new Vector3(0.75f, 0.08f, 4.3f);
                btmRun.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(btmRun);
            }

            // Undercarriage base collider
            BoxCollider underCol = exc.AddComponent<BoxCollider>();
            underCol.size = new Vector3(3.5f, 0.78f, 4.8f);
            underCol.center = new Vector3(0f, 0.42f, 0f);

            // ==========================================
            // 2. SLEWING TURRET DECK
            // ==========================================
            GameObject turret = new GameObject("Turret");
            turret.transform.SetParent(exc.transform, false);
            turret.transform.localPosition = new Vector3(0f, 0.85f, 0f);

            // Slewing ring bearing
            GameObject slewingRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            slewingRing.name = "Slewing_Ring";
            slewingRing.transform.SetParent(turret.transform, false);
            slewingRing.transform.localPosition = new Vector3(0f, 0.08f, 0f);
            slewingRing.transform.localScale = new Vector3(2.0f, 0.16f, 2.0f);
            slewingRing.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(slewingRing);

            // Main revolving deck platform
            GameObject deck = GameObject.CreatePrimitive(PrimitiveType.Cube);
            deck.name = "Turret_Deck";
            deck.transform.SetParent(turret.transform, false);
            deck.transform.localPosition = new Vector3(0f, 0.24f, 0f);
            deck.transform.localScale = new Vector3(3.2f, 0.22f, 4.2f);
            deck.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(deck);

            // ==========================================
            // 3. OPERATOR CABIN (FRONT-LEFT)
            // ==========================================
            GameObject cabin = new GameObject("Operator_Cabin");
            cabin.transform.SetParent(turret.transform, false);
            cabin.transform.localPosition = new Vector3(-0.95f, 0.35f, 0.65f);

            // Cab structural shell
            GameObject cabShell = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabShell.name = "Cab_Shell";
            cabShell.transform.SetParent(cabin.transform, false);
            cabShell.transform.localPosition = new Vector3(0f, 0.95f, 0f);
            cabShell.transform.localScale = new Vector3(1.15f, 1.85f, 1.75f);
            cabShell.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(cabShell);

            // Front panoramic windshield
            GameObject frontGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frontGlass.name = "Glass_Front";
            frontGlass.transform.SetParent(cabin.transform, false);
            frontGlass.transform.localPosition = new Vector3(0f, 1.05f, 0.89f);
            frontGlass.transform.localScale = new Vector3(0.95f, 1.35f, 0.06f);
            frontGlass.GetComponent<Renderer>().sharedMaterial = matGlass;
            StripColliders(frontGlass);

            // Left operator window
            GameObject leftGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftGlass.name = "Glass_Left";
            leftGlass.transform.SetParent(cabin.transform, false);
            leftGlass.transform.localPosition = new Vector3(-0.59f, 1.05f, 0.15f);
            leftGlass.transform.localScale = new Vector3(0.06f, 1.25f, 1.25f);
            leftGlass.GetComponent<Renderer>().sharedMaterial = matGlass;
            StripColliders(leftGlass);

            // Right window overlooking the boom
            GameObject rightGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightGlass.name = "Glass_Right";
            rightGlass.transform.SetParent(cabin.transform, false);
            rightGlass.transform.localPosition = new Vector3(0.59f, 1.05f, 0.15f);
            rightGlass.transform.localScale = new Vector3(0.06f, 1.25f, 1.25f);
            rightGlass.GetComponent<Renderer>().sharedMaterial = matGlass;
            StripColliders(rightGlass);

            // Cab roof protector visor
            GameObject cabRoof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabRoof.name = "Cab_Roof_Visor";
            cabRoof.transform.SetParent(cabin.transform, false);
            cabRoof.transform.localPosition = new Vector3(0f, 1.92f, 0.15f);
            cabRoof.transform.localScale = new Vector3(1.25f, 0.12f, 1.95f);
            cabRoof.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(cabRoof);

            // Cab roof work lights
            GameObject cabLight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabLight.name = "Cab_WorkLight";
            cabLight.transform.SetParent(cabin.transform, false);
            cabLight.transform.localPosition = new Vector3(0f, 1.92f, 1.10f);
            cabLight.transform.localScale = new Vector3(0.6f, 0.14f, 0.18f);
            cabLight.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(cabLight);

            // ==========================================
            // 4. ENGINE HOUSING & COUNTERWEIGHT (RIGHT & REAR)
            // ==========================================
            GameObject engineBay = GameObject.CreatePrimitive(PrimitiveType.Cube);
            engineBay.name = "Engine_Housing";
            engineBay.transform.SetParent(turret.transform, false);
            engineBay.transform.localPosition = new Vector3(0.65f, 1.15f, -0.2f);
            engineBay.transform.localScale = new Vector3(1.75f, 1.55f, 3.2f);
            engineBay.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(engineBay);

            // Heavy rear counterweight
            GameObject counterWeight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            counterWeight.name = "Rear_Counterweight";
            counterWeight.transform.SetParent(turret.transform, false);
            counterWeight.transform.localPosition = new Vector3(0f, 1.15f, -1.95f);
            counterWeight.transform.localScale = new Vector3(3.2f, 1.6f, 0.85f);
            counterWeight.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(counterWeight);

            // Exhaust stack
            GameObject exhaust = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            exhaust.name = "Exhaust_Stack";
            exhaust.transform.SetParent(turret.transform, false);
            exhaust.transform.localPosition = new Vector3(1.1f, 2.2f, -1.3f);
            exhaust.transform.localScale = new Vector3(0.14f, 0.55f, 0.14f);
            exhaust.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(exhaust);

            // Hydraulic oil cooler grille
            GameObject grille = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grille.name = "Cooling_Grille";
            grille.transform.SetParent(turret.transform, false);
            grille.transform.localPosition = new Vector3(1.54f, 1.25f, -0.2f);
            grille.transform.localScale = new Vector3(0.08f, 0.85f, 1.8f);
            grille.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            StripColliders(grille);

            // ==========================================
            // 5. ARTICULATED BOOM, STICK & DIGGING BUCKET
            // ==========================================
            GameObject boomPivot = new GameObject("Boom_Pivot");
            boomPivot.transform.SetParent(turret.transform, false);
            boomPivot.transform.localPosition = new Vector3(0.42f, 0.95f, 1.1f);

            // Excavator heavy main boom (extended reach)
            GameObject boomArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boomArm.name = "Boom_Arm";
            boomArm.transform.SetParent(boomPivot.transform, false);
            boomArm.transform.localPosition = new Vector3(0f, 1.8f, 1.6f);
            boomArm.transform.localRotation = Quaternion.Euler(-28f, 0f, 0f);
            boomArm.transform.localScale = new Vector3(0.48f, 0.70f, 5.2f);
            boomArm.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(boomArm);

            // Boom hydraulic lift cylinders
            float[] cylX = new float[] { -0.32f, 0.32f };
            for (int c = 0; c < 2; c++)
            {
                GameObject boomCyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                boomCyl.name = "Hydraulic_Cylinder_Boom_" + c;
                boomCyl.transform.SetParent(boomPivot.transform, false);
                boomCyl.transform.localPosition = new Vector3(cylX[c], 0.85f, 0.95f);
                boomCyl.transform.localRotation = Quaternion.Euler(32f, 0f, 0f);
                boomCyl.transform.localScale = new Vector3(0.14f, 1.35f, 0.14f);
                boomCyl.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(boomCyl);
            }

            // Stick hinge
            GameObject stickPivot = new GameObject("Stick_Pivot");
            stickPivot.transform.SetParent(boomPivot.transform, false);
            stickPivot.transform.localPosition = new Vector3(0f, 3.65f, 3.45f);

            // Excavator dipper stick (extended reach)
            GameObject stickArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stickArm.name = "Stick_Arm";
            stickArm.transform.SetParent(stickPivot.transform, false);
            stickArm.transform.localPosition = new Vector3(0f, 0.15f, 1.95f);
            stickArm.transform.localRotation = Quaternion.Euler(38f, 0f, 0f);
            stickArm.transform.localScale = new Vector3(0.42f, 0.55f, 4.2f);
            stickArm.GetComponent<Renderer>().sharedMaterial = matYellow;
            StripColliders(stickArm);

            // Bucket hinge
            GameObject bucketPivot = new GameObject("Bucket_Pivot");
            bucketPivot.transform.SetParent(stickPivot.transform, false);
            bucketPivot.transform.localPosition = new Vector3(0f, -1.05f, 3.55f);

            // Digging bucket scoop
            GameObject bucket = null;
            if (prefabDozer != null)
            {
                Transform dozerShovel = prefabDozer.transform.Find("shovel");
                if (dozerShovel != null)
                {
                    GameObject scoopModel = Instantiate(dozerShovel.gameObject, bucketPivot.transform);
                    scoopModel.name = "Bucket_Scoop";
                    scoopModel.transform.localPosition = new Vector3(0f, -0.2f, 0.35f);
                    scoopModel.transform.localRotation = Quaternion.Euler(35f, 180f, 0f);
                    scoopModel.transform.localScale = Vector3.one * 1.35f;
                    ApplyMaterialRecursively(scoopModel, matVehicles);
                    StripColliders(scoopModel);
                    bucket = scoopModel;
                }
            }

            if (bucket == null)
            {
                bucket = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bucket.name = "Bucket_Scoop";
                bucket.transform.SetParent(bucketPivot.transform, false);
                bucket.transform.localPosition = new Vector3(0f, -0.35f, 0.35f);
                bucket.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                bucket.transform.localScale = new Vector3(1.5f, 1.0f, 1.35f);
                bucket.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(bucket);
            }

            // Excavator digging teeth
            for (int t = 0; t < 4; t++)
            {
                GameObject tooth = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tooth.name = "Bucket_Tooth_" + t;
                tooth.transform.SetParent(bucket.transform, false);
                tooth.transform.localPosition = new Vector3(-0.52f + t * 0.35f, -0.38f, 0.60f);
                tooth.transform.localScale = new Vector3(0.12f, 0.12f, 0.34f);
                tooth.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                StripColliders(tooth);
            }

            BoxCollider bucketCol = bucket.AddComponent<BoxCollider>();
            bucketCol.size = new Vector3(1.6f, 1.1f, 1.4f);
            PhysicsMaterial bucketMat = new PhysicsMaterial("BucketPhysMat")
            {
                dynamicFriction = 0.15f,
                staticFriction = 0.25f,
                bounciness = 0.05f
            };
            bucketCol.sharedMaterial = bucketMat;

            // ==========================================
            // 6. CAMERA & CONTROLLER
            // ==========================================
            GameObject camObj = new GameObject("Excavator_Camera");
            camObj.transform.SetParent(turret.transform, false);
            camObj.transform.localPosition = new Vector3(-2.6f, 3.8f, -4.8f);
            camObj.transform.localRotation = Quaternion.Euler(18f, 15f, 0f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(exc.transform, false);
            exitObj.transform.localPosition = new Vector3(-2.8f, 0f, 0.5f);

            ExcavatorController ec = exc.AddComponent<ExcavatorController>();
            ec.turret = turret.transform;
            ec.boom = boomPivot.transform;
            ec.stick = stickPivot.transform;
            ec.bucket = bucketPivot.transform;
            ec.vehicleCamera = cam;
            ec.exitTransform = exitObj.transform;

            // Clean initial resting pose:
            ec.currentTurretYaw = 0f;
            ec.currentBoomAngle = -28f;
            ec.currentStickAngle = 30f;
            ec.currentBucketAngle = 20f;
        }

        private void BuildTowerCrane(Transform root)
        {
            GameObject crane = new GameObject("Tower_Crane");
            crane.transform.SetParent(root, false);
            crane.transform.localPosition = new Vector3(-13.5f, 0.20f, -6f);

            GameObject basePlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            basePlate.name = "Crane_Base";
            basePlate.transform.SetParent(crane.transform, false);
            basePlate.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            basePlate.transform.localScale = new Vector3(4.5f, 0.9f, 4.5f);
            basePlate.GetComponent<Renderer>().sharedMaterial = matConcrete;

            GameObject mastRoot = new GameObject("Lattice_Mast");
            mastRoot.transform.SetParent(crane.transform, false);
            mastRoot.transform.localPosition = Vector3.zero;

            Vector3[] chordOffsets = new Vector3[]
            {
                new Vector3(-0.85f, 12f, -0.85f),
                new Vector3(0.85f, 12f, -0.85f),
                new Vector3(-0.85f, 12f, 0.85f),
                new Vector3(0.85f, 12f, 0.85f)
            };

            for (int i = 0; i < chordOffsets.Length; i++)
            {
                GameObject chord = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                chord.name = "Mast_Chord_" + i;
                chord.transform.SetParent(mastRoot.transform, false);
                chord.transform.localPosition = chordOffsets[i];
                chord.transform.localScale = new Vector3(0.22f, 12f, 0.22f);
                chord.GetComponent<Renderer>().sharedMaterial = matYellow;
            }

            for (int level = 1; level <= 8; level++)
            {
                float ly = level * 2.8f;
                GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ring.name = "Mast_Tie_" + level;
                ring.transform.SetParent(mastRoot.transform, false);
                ring.transform.localPosition = new Vector3(0f, ly, 0f);
                ring.transform.localScale = new Vector3(1.85f, 0.16f, 1.85f);
                ring.GetComponent<Renderer>().sharedMaterial = matYellow;
            }

            BoxCollider mastCol = mastRoot.AddComponent<BoxCollider>();
            mastCol.size = new Vector3(1.9f, 24f, 1.9f);
            mastCol.center = new Vector3(0f, 12f, 0f);

            GameObject ladder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ladder.name = "Mast_Ladder";
            ladder.transform.SetParent(crane.transform, false);
            ladder.transform.localPosition = new Vector3(0.95f, 12f, 0f);
            ladder.transform.localScale = new Vector3(0.12f, 23f, 0.45f);
            ladder.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(ladder.GetComponent<Collider>());

            GameObject slewingUnit = new GameObject("Slewing_Jib_Unit");
            slewingUnit.transform.SetParent(crane.transform, false);
            slewingUnit.transform.localPosition = new Vector3(0f, 24f, 0f);

            GameObject cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cab.name = "Operator_Cabin";
            cab.transform.SetParent(slewingUnit.transform, false);
            cab.transform.localPosition = new Vector3(1.3f, 0.6f, 0.9f);
            cab.transform.localScale = new Vector3(1.2f, 1.7f, 1.5f);
            cab.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject cabGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabGlass.name = "Cab_Glass";
            cabGlass.transform.SetParent(cab.transform, false);
            cabGlass.transform.localPosition = new Vector3(0f, 0.15f, 0.52f);
            cabGlass.transform.localScale = new Vector3(0.9f, 0.75f, 0.05f);
            cabGlass.GetComponent<Renderer>().sharedMaterial = matGlass;
            Object.Destroy(cabGlass.GetComponent<Collider>());

            GameObject jibBoom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            jibBoom.name = "Working_Jib";
            jibBoom.transform.SetParent(slewingUnit.transform, false);
            jibBoom.transform.localPosition = new Vector3(0f, 1.2f, 12.5f);
            jibBoom.transform.localScale = new Vector3(1.1f, 1.2f, 25f);
            jibBoom.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject counterJib = GameObject.CreatePrimitive(PrimitiveType.Cube);
            counterJib.name = "Counter_Jib";
            counterJib.transform.SetParent(slewingUnit.transform, false);
            counterJib.transform.localPosition = new Vector3(0f, 1.2f, -4.5f);
            counterJib.transform.localScale = new Vector3(1.1f, 1.2f, 9f);
            counterJib.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject counterWeight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            counterWeight.name = "Counter_Weight";
            counterWeight.transform.SetParent(slewingUnit.transform, false);
            counterWeight.transform.localPosition = new Vector3(0f, 0.9f, -8.2f);
            counterWeight.transform.localScale = new Vector3(2.4f, 1.6f, 2.2f);
            counterWeight.GetComponent<Renderer>().sharedMaterial = matConcrete;

            GameObject topApex = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            topApex.name = "Tower_Top_Spire";
            topApex.transform.SetParent(slewingUnit.transform, false);
            topApex.transform.localPosition = new Vector3(0f, 3.8f, 0f);
            topApex.transform.localScale = new Vector3(0.18f, 2.2f, 0.18f);
            topApex.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(topApex.GetComponent<Collider>());

            GameObject beaconObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beaconObj.name = "Crane_Top_Beacon";
            beaconObj.transform.SetParent(slewingUnit.transform, false);
            beaconObj.transform.localPosition = new Vector3(0f, 6.1f, 0f);
            beaconObj.transform.localScale = new Vector3(0.25f, 0.18f, 0.25f);
            beaconObj.GetComponent<Renderer>().sharedMaterial = matOrange;
            Object.Destroy(beaconObj.GetComponent<Collider>());

            GameObject beaconLight = new GameObject("Beacon_Light");
            beaconLight.transform.SetParent(beaconObj.transform, false);
            Light bl = beaconLight.AddComponent<Light>();
            bl.type = LightType.Point;
            bl.range = 22f;

            WarningBeacon wb = beaconObj.AddComponent<WarningBeacon>();
            wb.beaconLight = bl;
            wb.lensRenderer = beaconObj.GetComponent<Renderer>();
            wb.flashFrequency = 1.2f;

            GameObject trolley = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trolley.name = "Trolley";
            trolley.transform.SetParent(slewingUnit.transform, false);
            trolley.transform.localPosition = new Vector3(0f, 0.5f, 8f);
            trolley.transform.localScale = new Vector3(0.95f, 0.35f, 1.1f);
            trolley.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(trolley.GetComponent<Collider>());

            GameObject hook = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hook.name = "Hook_Block";
            hook.transform.SetParent(slewingUnit.transform, false);
            hook.transform.localPosition = new Vector3(0f, -11.5f, 8f);
            hook.transform.localScale = new Vector3(0.55f, 0.65f, 0.55f);
            hook.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject hookGrab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hookGrab.name = "Hook_Latch";
            hookGrab.transform.SetParent(hook.transform, false);
            hookGrab.transform.localPosition = new Vector3(0f, -0.6f, 0f);
            hookGrab.transform.localScale = new Vector3(0.35f, 0.25f, 0.35f);
            hookGrab.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject cabinCamObj = new GameObject("Crane_Camera");
            cabinCamObj.transform.SetParent(slewingUnit.transform, false);
            cabinCamObj.transform.localPosition = new Vector3(0f, 10f, -18f);
            cabinCamObj.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
            Camera cabinCam = cabinCamObj.AddComponent<Camera>();
            cabinCam.fieldOfView = 60f;
            cabinCam.nearClipPlane = 0.3f;
            cabinCam.farClipPlane = 400f;
            cabinCam.enabled = false;

            GameObject hookCamObj = new GameObject("Crane_Hook_Camera");
            hookCamObj.transform.SetParent(trolley.transform, false);
            hookCamObj.transform.localPosition = new Vector3(0f, 2.5f, 0f);
            hookCamObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            Camera hookCam = hookCamObj.AddComponent<Camera>();
            hookCam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(crane.transform, false);
            exitObj.transform.localPosition = new Vector3(2.4f, 0.1f, 0f);

            TowerCraneController tcc = crane.AddComponent<TowerCraneController>();
            tcc.jib = slewingUnit.transform;
            tcc.trolley = trolley.transform;
            tcc.hook = hook.transform;
            tcc.vehicleCamera = cabinCam;
            tcc.hookCamera = hookCam;
            tcc.exitTransform = exitObj.transform;
            tcc.minTrolleyDist = 3.5f;
            tcc.maxTrolleyDist = 23f;
            tcc.minCableLength = 2.5f;
            tcc.maxCableLength = 22f;
        }

        private void IgnoreChassisCollisionsWithCargo(Transform root)
        {
            List<Collider> chassisCols = new List<Collider>();

            // Find root BoxColliders on Excavator, Bulldozer, Dump Truck
            Transform exc = root.Find("Excavator");
            if (exc != null) { Collider c = exc.GetComponent<BoxCollider>(); if (c != null) chassisCols.Add(c); }

            Transform dozer = root.Find("Bulldozer");
            if (dozer != null) { Collider c = dozer.GetComponent<BoxCollider>(); if (c != null) chassisCols.Add(c); }

            Transform truck = root.Find("Dump_Truck");
            if (truck != null) { Collider c = truck.GetComponent<BoxCollider>(); if (c != null) chassisCols.Add(c); }

            GranularItem[] items = root.GetComponentsInChildren<GranularItem>();
            for (int i = 0; i < items.Length; i++)
            {
                Collider itemCol = items[i].GetComponent<Collider>();
                if (itemCol == null) continue;

                for (int c = 0; c < chassisCols.Count; c++)
                {
                    Physics.IgnoreCollision(itemCol, chassisCols[c], true);
                }
            }
        }
    }
}
