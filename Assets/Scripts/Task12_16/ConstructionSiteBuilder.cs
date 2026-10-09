using System.Collections.Generic;
using UnityEngine;

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
            if (GameObject.Find("--- CONSTRUCTION SITE (TASKS 12-16) ---") != null) return;
            BuildFullSite();
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
                    ApplyMaterialRecursively(bar, matEnvironment);
                    if (bar.GetComponent<Collider>() == null) bar.AddComponent<BoxCollider>();
                }
                else
                {
                    bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    bar.name = "Warning_Barrier_" + i;
                    bar.transform.SetParent(root, false);
                    bar.transform.localPosition = barrierPositions[i] + Vector3.up * 0.42f;
                    bar.transform.localScale = new Vector3(0.35f, 0.85f, 1.6f);
                    bar.GetComponent<Renderer>().sharedMaterial = matOrange;
                }

                if (i < 2 && bar != null)
                {
                    GameObject beaconObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    beaconObj.name = "Barrier_Beacon";
                    beaconObj.transform.SetParent(bar.transform, false);
                    beaconObj.transform.localPosition = new Vector3(0f, 0.85f, 0f);
                    beaconObj.transform.localScale = new Vector3(0.25f, 0.18f, 0.25f);
                    beaconObj.GetComponent<Renderer>().sharedMaterial = matOrange;
                    Object.Destroy(beaconObj.GetComponent<Collider>());

                    GameObject bl = new GameObject("Beacon_Light");
                    bl.transform.SetParent(beaconObj.transform, false);
                    Light l = bl.AddComponent<Light>();
                    l.type = LightType.Point;
                    l.range = 8.5f;

                    WarningBeacon wb = beaconObj.AddComponent<WarningBeacon>();
                    wb.beaconLight = l;
                    wb.lensRenderer = beaconObj.GetComponent<Renderer>();
                    wb.flashFrequency = 1.8f;
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
                    ApplyMaterialRecursively(cone, matEnvironment);
                    if (cone.GetComponent<Collider>() == null) cone.AddComponent<BoxCollider>();
                }
                else
                {
                    GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    cone.name = "Traffic_Cone_" + i;
                    cone.transform.SetParent(root, false);
                    cone.transform.localPosition = conePositions[i] + Vector3.up * 0.25f;
                    cone.transform.localScale = new Vector3(0.3f, 0.35f, 0.3f);
                    cone.GetComponent<Renderer>().sharedMaterial = matOrange;
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
                }
                if (prefabDebrisBolt != null)
                {
                    GameObject db = Instantiate(prefabDebrisBolt, root);
                    db.name = "Debris_Bolt_" + i;
                    db.transform.localPosition = debrisSpots[i] + new Vector3(0.4f, 0f, 0.3f);
                    db.transform.localScale = Vector3.one * 1.5f;
                    ApplyMaterialRecursively(db, matVehicles);
                }
                if (prefabDebrisTire != null)
                {
                    GameObject dt = Instantiate(prefabDebrisTire, root);
                    dt.name = "Debris_Tire_" + i;
                    dt.transform.localPosition = debrisSpots[i] + new Vector3(-0.4f, 0f, -0.3f);
                    dt.transform.localScale = Vector3.one * 1.4f;
                    ApplyMaterialRecursively(dt, matVehicles);
                }
            }
        }

        private void BuildExcavationPit(Transform root)
        {
            GameObject pit = new GameObject("Excavation_Pit");
            pit.transform.SetParent(root, false);
            pit.transform.localPosition = new Vector3(-5f, 0.20f, 6.5f);

            CreateWall("Pit_Wall_N", pit.transform, new Vector3(0f, 0.65f, 5.8f), new Vector3(13f, 1.4f, 0.6f), matConcrete);
            CreateWall("Pit_Wall_S", pit.transform, new Vector3(0f, 0.65f, -5.8f), new Vector3(13f, 1.4f, 0.6f), matConcrete);
            CreateWall("Pit_Wall_W", pit.transform, new Vector3(-6.4f, 0.65f, 0f), new Vector3(0.6f, 1.4f, 12f), matConcrete);

            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Pit_Ramp";
            ramp.transform.SetParent(pit.transform, false);
            ramp.transform.localPosition = new Vector3(5.8f, 0.35f, 0f);
            ramp.transform.localRotation = Quaternion.Euler(0f, 0f, 10f);
            ramp.transform.localScale = new Vector3(3.8f, 0.35f, 9f);
            ramp.GetComponent<Renderer>().sharedMaterial = matDirt;

            GameObject pitFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pitFloor.name = "Pit_Floor";
            pitFloor.transform.SetParent(pit.transform, false);
            pitFloor.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            pitFloor.transform.localScale = new Vector3(13f, 0.30f, 12f);
            pitFloor.GetComponent<Renderer>().sharedMaterial = matDirt;

            GameObject mound1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mound1.name = "Soil_Mound_1";
            mound1.transform.SetParent(pit.transform, false);
            mound1.transform.localPosition = new Vector3(-3.0f, 0.45f, 2.5f);
            mound1.transform.localScale = new Vector3(4.8f, 1.6f, 4.4f);
            mound1.GetComponent<Renderer>().sharedMaterial = matDirt;

            GameObject mound2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mound2.name = "Soil_Mound_2";
            mound2.transform.SetParent(pit.transform, false);
            mound2.transform.localPosition = new Vector3(1.8f, 0.35f, -2.2f);
            mound2.transform.localScale = new Vector3(4.4f, 1.4f, 4.2f);
            mound2.GetComponent<Renderer>().sharedMaterial = matDirt;
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

            for (int i = 0; i < 40; i++)
            {
                float rx = Random.Range(-9.5f, -1.0f);
                float rz = Random.Range(3.0f, 9.5f);
                float ry = Random.Range(0.45f, 0.85f);

                GameObject sand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sand.name = "Sand_Pebble_" + i;
                sand.transform.SetParent(cargoRoot.transform, false);
                sand.transform.localPosition = new Vector3(rx, ry, rz);
                sand.transform.localScale = Vector3.one * Random.Range(0.36f, 0.52f);
                sand.GetComponent<Renderer>().sharedMaterial = matSand;

                GranularItem gi = sand.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Sand;
            }

            for (int i = 0; i < 24; i++)
            {
                float rx = Random.Range(-8.5f, -0.5f);
                float rz = Random.Range(3.5f, 9.5f);
                float ry = Random.Range(0.50f, 0.95f);

                GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
                rock.name = "Rock_Stone_" + i;
                rock.transform.SetParent(cargoRoot.transform, false);
                rock.transform.localPosition = new Vector3(rx, ry, rz);
                rock.transform.localRotation = Random.rotation;
                rock.transform.localScale = new Vector3(Random.Range(0.55f, 0.85f), Random.Range(0.45f, 0.75f), Random.Range(0.55f, 0.85f));
                rock.GetComponent<Renderer>().sharedMaterial = matRock;

                GranularItem gi = rock.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Rock;
            }

            for (int i = 0; i < 30; i++)
            {
                float bx = Random.Range(-2.0f, 5.0f);
                float bz = Random.Range(-5.0f, -1.0f);
                float by = 0.35f + (i % 3) * 0.25f;

                GameObject brick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                brick.name = "Brick_" + i;
                brick.transform.SetParent(cargoRoot.transform, false);
                brick.transform.localPosition = new Vector3(bx, by, bz);
                brick.transform.localRotation = Quaternion.Euler(0f, (i % 2) * 90f, 0f);
                brick.transform.localScale = new Vector3(0.52f, 0.24f, 0.28f);
                brick.GetComponent<Renderer>().sharedMaterial = matBrick;

                GranularItem gi = brick.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Brick;
            }

            SpawnCraneContainers(cargoRoot.transform);
        }

        private void SpawnCraneContainers(Transform cargoRoot)
        {
            if (prefabContainerBlue != null)
            {
                GameObject cb = Instantiate(prefabContainerBlue, cargoRoot);
                cb.name = "Cargo_Shipping_Container_Blue";
                cb.transform.localPosition = new Vector3(-12f, 0.35f, -8f);
                cb.transform.localScale = Vector3.one * 1.4f;
                ApplyMaterialRecursively(cb, matTrain);
                PrepareCraneCargo(cb, 320f);
            }

            if (prefabContainerRed != null)
            {
                GameObject cr = Instantiate(prefabContainerRed, cargoRoot);
                cr.name = "Cargo_Shipping_Container_Red";
                cr.transform.localPosition = new Vector3(-11f, 0.35f, 0f);
                cr.transform.localScale = Vector3.one * 1.4f;
                ApplyMaterialRecursively(cr, matTrain);
                PrepareCraneCargo(cr, 320f);
            }

            GameObject pallet = null;
            if (prefabFlatbed != null)
            {
                pallet = Instantiate(prefabFlatbed, cargoRoot);
                pallet.name = "Cargo_Pallet_Bricks";
                pallet.transform.localPosition = new Vector3(-8f, 0.35f, -4f);
                pallet.transform.localScale = Vector3.one * 1.2f;
                ApplyMaterialRecursively(pallet, matTrain);
            }
            else
            {
                pallet = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pallet.name = "Cargo_Pallet_Bricks";
                pallet.transform.SetParent(cargoRoot, false);
                pallet.transform.localPosition = new Vector3(-8f, 0.45f, -4f);
                pallet.transform.localScale = new Vector3(1.8f, 0.8f, 1.8f);
                pallet.GetComponent<Renderer>().sharedMaterial = matWood;
            }
            PrepareCraneCargo(pallet, 240f);
        }

        private void PrepareCraneCargo(GameObject obj, float mass)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) rb = obj.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 2.0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            if (obj.GetComponent<Collider>() == null)
            {
                BoxCollider bc = obj.AddComponent<BoxCollider>();
                bc.size = new Vector3(2.5f, 2.5f, 4.5f);
            }

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Hoist_Ring";
            ring.transform.SetParent(obj.transform, false);
            ring.transform.localPosition = new Vector3(0f, 1.35f, 0f);
            ring.transform.localScale = new Vector3(0.35f, 0.20f, 0.35f);
            ring.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(ring.GetComponent<Collider>());
        }

        private void BuildBulldozer(Transform root)
        {
            GameObject dozer = null;

            if (prefabDozer != null)
            {
                dozer = Instantiate(prefabDozer, root);
                dozer.name = "Bulldozer";
                dozer.transform.localPosition = new Vector3(7.5f, 0.25f, 6.5f);
                dozer.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                dozer.transform.localScale = Vector3.one * 1.6f;
                ApplyMaterialRecursively(dozer, matVehicles);
            }
            else
            {
                dozer = new GameObject("Bulldozer");
                dozer.transform.SetParent(root, false);
                dozer.transform.localPosition = new Vector3(7.5f, 0.5f, 6.5f);
                dozer.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

                GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chassis.name = "Chassis";
                chassis.transform.SetParent(dozer.transform, false);
                chassis.transform.localPosition = new Vector3(0f, 0.45f, 0f);
                chassis.transform.localScale = new Vector3(2.2f, 0.85f, 3.8f);
                chassis.GetComponent<Renderer>().sharedMaterial = matYellow;
            }

            BoxCollider baseCol = dozer.GetComponent<BoxCollider>();
            if (baseCol == null) baseCol = dozer.AddComponent<BoxCollider>();
            baseCol.size = new Vector3(2.4f, 1.8f, 3.8f);
            baseCol.center = new Vector3(0f, 0.9f, 0f);

            List<Transform> wheelList = new List<Transform>();
            if (prefabWheelTractorFront != null && prefabWheelTractorBack != null)
            {
                Vector3[] fPos = new Vector3[] { new Vector3(-0.95f, 0.48f, 1.05f), new Vector3(0.95f, 0.48f, 1.05f) };
                for (int i = 0; i < 2; i++)
                {
                    GameObject wf = Instantiate(prefabWheelTractorFront, dozer.transform);
                    wf.name = "Wheel_Front_" + i;
                    wf.transform.localPosition = fPos[i];
                    wf.transform.localRotation = Quaternion.Euler(0f, i == 1 ? 180f : 0f, 0f);
                    ApplyMaterialRecursively(wf, matVehicles);
                    wheelList.Add(wf.transform);
                }

                Vector3[] bPos = new Vector3[] { new Vector3(-0.95f, 0.72f, -0.92f), new Vector3(0.95f, 0.72f, -0.92f) };
                for (int i = 0; i < 2; i++)
                {
                    GameObject wb = Instantiate(prefabWheelTractorBack, dozer.transform);
                    wb.name = "Wheel_Back_" + i;
                    wb.transform.localPosition = bPos[i];
                    wb.transform.localRotation = Quaternion.Euler(0f, i == 1 ? 180f : 0f, 0f);
                    ApplyMaterialRecursively(wb, matVehicles);
                    wheelList.Add(wb.transform);
                }
            }

            Transform bladeTr = dozer.transform.Find("shovel");
            GameObject bladeAssembly = null;

            if (bladeTr != null)
            {
                bladeAssembly = bladeTr.gameObject;
                bladeAssembly.name = "Blade_Assembly";
                BoxCollider bCol = bladeAssembly.GetComponent<BoxCollider>();
                if (bCol == null) bCol = bladeAssembly.AddComponent<BoxCollider>();
                bCol.size = new Vector3(2.6f, 0.9f, 0.8f);
                bCol.center = new Vector3(0f, 0.35f, 0.3f);

                PhysicsMaterial bladeMat = new PhysicsMaterial("BladePhysMat")
                {
                    dynamicFriction = 0.15f,
                    staticFriction = 0.25f,
                    bounciness = 0.05f
                };
                bCol.sharedMaterial = bladeMat;
            }
            else
            {
                bladeAssembly = new GameObject("Blade_Assembly");
                bladeAssembly.transform.SetParent(dozer.transform, false);
                bladeAssembly.transform.localPosition = new Vector3(0f, 0.2f, 2.45f);

                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blade.name = "Blade_Moldboard";
                blade.transform.SetParent(bladeAssembly.transform, false);
                blade.transform.localPosition = Vector3.zero;
                blade.transform.localScale = new Vector3(3.2f, 0.95f, 0.35f);
                blade.GetComponent<Renderer>().sharedMaterial = matYellow;

                PhysicsMaterial bladeMat = new PhysicsMaterial("BladePhysMat")
                {
                    dynamicFriction = 0.15f,
                    staticFriction = 0.25f,
                    bounciness = 0.05f
                };
                blade.GetComponent<BoxCollider>().sharedMaterial = bladeMat;
            }

            GameObject camObj = new GameObject("Bulldozer_Camera");
            camObj.transform.SetParent(dozer.transform, false);
            camObj.transform.localPosition = new Vector3(0f, 3.4f, -5.2f);
            camObj.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(dozer.transform, false);
            exitObj.transform.localPosition = new Vector3(2.6f, 0f, 0f);

            BulldozerController bc = dozer.AddComponent<BulldozerController>();
            bc.bladeTransform = bladeAssembly.transform;
            bc.wheels = wheelList.ToArray();
            bc.vehicleCamera = cam;
            bc.exitTransform = exitObj.transform;
            bc.bladeMin = 0.02f;
            bc.bladeMax = 1.35f;
            bc.bladeSpeed = 0.85f;
        }

        private void BuildDumpTruck(Transform root)
        {
            GameObject truck = null;

            if (prefabTruckFlat != null)
            {
                truck = Instantiate(prefabTruckFlat, root);
                truck.name = "Dump_Truck";
                truck.transform.localPosition = new Vector3(7.5f, 0.25f, -6f);
                truck.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
                truck.transform.localScale = Vector3.one * 1.6f;
                ApplyMaterialRecursively(truck, matVehicles);
            }
            else
            {
                truck = new GameObject("Dump_Truck");
                truck.transform.SetParent(root, false);
                truck.transform.localPosition = new Vector3(7.5f, 0.5f, -6f);
                truck.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

                GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chassis.name = "Frame_Chassis";
                chassis.transform.SetParent(truck.transform, false);
                chassis.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                chassis.transform.localScale = new Vector3(2.1f, 0.5f, 5.8f);
                chassis.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            }

            BoxCollider cabCol = truck.GetComponent<BoxCollider>();
            if (cabCol == null) cabCol = truck.AddComponent<BoxCollider>();
            cabCol.size = new Vector3(2.2f, 1.8f, 2.2f);
            cabCol.center = new Vector3(0f, 0.9f, 1.6f);

            List<Transform> frontWheels = new List<Transform>();
            List<Transform> rearWheels = new List<Transform>();
            if (prefabWheelTruck != null)
            {
                Vector3[] fwPos = new Vector3[] { new Vector3(-0.95f, 0.42f, 1.85f), new Vector3(0.95f, 0.42f, 1.85f) };
                for (int i = 0; i < 2; i++)
                {
                    GameObject w = Instantiate(prefabWheelTruck, truck.transform);
                    w.name = "Wheel_Truck_F_" + i;
                    w.transform.localPosition = fwPos[i];
                    w.transform.localRotation = Quaternion.Euler(0f, i == 1 ? 180f : 0f, 0f);
                    ApplyMaterialRecursively(w, matVehicles);
                    frontWheels.Add(w.transform);
                }

                Vector3[] rwPos = new Vector3[]
                {
                    new Vector3(-0.95f, 0.42f, -0.85f), new Vector3(0.95f, 0.42f, -0.85f),
                    new Vector3(-0.95f, 0.42f, -1.95f), new Vector3(0.95f, 0.42f, -1.95f)
                };
                for (int i = 0; i < 4; i++)
                {
                    GameObject w = Instantiate(prefabWheelTruck, truck.transform);
                    w.name = "Wheel_Truck_R_" + i;
                    w.transform.localPosition = rwPos[i];
                    w.transform.localRotation = Quaternion.Euler(0f, (i % 2 == 1) ? 180f : 0f, 0f);
                    ApplyMaterialRecursively(w, matVehicles);
                    rearWheels.Add(w.transform);
                }
            }

            GameObject bedPivot = new GameObject("Dump_Bed_Pivot");
            bedPivot.transform.SetParent(truck.transform, false);
            bedPivot.transform.localPosition = new Vector3(0f, 0.75f, -1.8f);

            GameObject dumpBed = null;
            if (prefabCarriageDirt != null)
            {
                dumpBed = Instantiate(prefabCarriageDirt, bedPivot.transform);
                dumpBed.name = "Dump_Bed_Body";
                dumpBed.transform.localPosition = new Vector3(0f, 0.1f, 1.4f);
                dumpBed.transform.localRotation = Quaternion.identity;
                dumpBed.transform.localScale = new Vector3(1.15f, 1.1f, 1.05f);
                ApplyMaterialRecursively(dumpBed, matTrain);

                BoxCollider bedCol = dumpBed.GetComponent<BoxCollider>();
                if (bedCol == null) bedCol = dumpBed.AddComponent<BoxCollider>();
                bedCol.size = new Vector3(2.2f, 1.2f, 3.4f);
                bedCol.center = new Vector3(0f, 0.6f, 0f);
            }
            else
            {
                dumpBed = new GameObject("Dump_Bed_Body");
                dumpBed.transform.SetParent(bedPivot.transform, false);
                dumpBed.transform.localPosition = new Vector3(0f, 0f, 1.4f);

                GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = "Bed_Floor";
                floor.transform.SetParent(dumpBed.transform, false);
                floor.transform.localPosition = new Vector3(0f, 0f, 0f);
                floor.transform.localScale = new Vector3(2.2f, 0.18f, 3.2f);
                floor.GetComponent<Renderer>().sharedMaterial = matOrange;

                GameObject wallL = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallL.name = "Bed_Wall_Left";
                wallL.transform.SetParent(dumpBed.transform, false);
                wallL.transform.localPosition = new Vector3(-1.05f, 0.55f, 0f);
                wallL.transform.localScale = new Vector3(0.12f, 1.0f, 3.2f);
                wallL.GetComponent<Renderer>().sharedMaterial = matOrange;

                GameObject wallR = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallR.name = "Bed_Wall_Right";
                wallR.transform.SetParent(dumpBed.transform, false);
                wallR.transform.localPosition = new Vector3(1.05f, 0.55f, 0f);
                wallR.transform.localScale = new Vector3(0.12f, 1.0f, 3.2f);
                wallR.GetComponent<Renderer>().sharedMaterial = matOrange;

                GameObject wallFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
                wallFront.name = "Bed_Wall_Front";
                wallFront.transform.SetParent(dumpBed.transform, false);
                wallFront.transform.localPosition = new Vector3(0f, 0.7f, 1.55f);
                wallFront.transform.localScale = new Vector3(2.2f, 1.3f, 0.14f);
                wallFront.GetComponent<Renderer>().sharedMaterial = matOrange;
            }

            GameObject tailgatePivot = new GameObject("Tailgate_Pivot");
            tailgatePivot.transform.SetParent(dumpBed.transform, false);
            tailgatePivot.transform.localPosition = new Vector3(0f, 0.95f, -1.55f);

            GameObject tailgate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tailgate.name = "Tailgate_Leaf";
            tailgate.transform.SetParent(tailgatePivot.transform, false);
            tailgate.transform.localPosition = new Vector3(0f, -0.45f, 0f);
            tailgate.transform.localScale = new Vector3(2.15f, 0.92f, 0.12f);
            tailgate.GetComponent<Renderer>().sharedMaterial = matOrange;

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
            dtc.maxTiltAngle = 52f;
            dtc.tiltSpeed = 24f;
        }

        private void BuildExcavator(Transform root)
        {
            GameObject exc = new GameObject("Excavator");
            exc.transform.SetParent(root, false);
            exc.transform.localPosition = new Vector3(1.5f, 0.45f, 7.0f);
            exc.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject undercarriage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            undercarriage.name = "Undercarriage";
            undercarriage.transform.SetParent(exc.transform, false);
            undercarriage.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            undercarriage.transform.localScale = new Vector3(2.8f, 0.85f, 4.0f);
            undercarriage.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject trackL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trackL.name = "Track_Frame_L";
            trackL.transform.SetParent(undercarriage.transform, false);
            trackL.transform.localPosition = new Vector3(-0.52f, 0f, 0f);
            trackL.transform.localScale = new Vector3(0.28f, 1.15f, 1.10f);
            trackL.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject trackR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trackR.name = "Track_Frame_R";
            trackR.transform.SetParent(undercarriage.transform, false);
            trackR.transform.localPosition = new Vector3(0.52f, 0f, 0f);
            trackR.transform.localScale = new Vector3(0.28f, 1.15f, 1.10f);
            trackR.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            if (prefabWheelTractorFront != null)
            {
                float[] zOffsets = new float[] { -1.3f, -0.65f, 0f, 0.65f, 1.3f };
                for (int i = 0; i < zOffsets.Length; i++)
                {
                    GameObject rwL = Instantiate(prefabWheelTractorFront, undercarriage.transform);
                    rwL.name = "Roller_L_" + i;
                    rwL.transform.localPosition = new Vector3(-0.52f, -0.2f, zOffsets[i] / 4.0f);
                    rwL.transform.localScale = Vector3.one * 0.45f;
                    ApplyMaterialRecursively(rwL, matDarkMetal);

                    GameObject rwR = Instantiate(prefabWheelTractorFront, undercarriage.transform);
                    rwR.name = "Roller_R_" + i;
                    rwR.transform.localPosition = new Vector3(0.52f, -0.2f, zOffsets[i] / 4.0f);
                    rwR.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    rwR.transform.localScale = Vector3.one * 0.45f;
                    ApplyMaterialRecursively(rwR, matDarkMetal);
                }
            }

            GameObject turret = new GameObject("Turret");
            turret.transform.SetParent(exc.transform, false);
            turret.transform.localPosition = new Vector3(0f, 0.95f, 0f);

            if (prefabTractor != null)
            {
                GameObject cabModel = Instantiate(prefabTractor, turret.transform);
                cabModel.name = "Excavator_Cab_Body";
                cabModel.transform.localPosition = new Vector3(-0.25f, 0.15f, -0.2f);
                cabModel.transform.localRotation = Quaternion.identity;
                cabModel.transform.localScale = Vector3.one * 1.4f;
                ApplyMaterialRecursively(cabModel, matVehicles);
            }
            else
            {
                GameObject house = GameObject.CreatePrimitive(PrimitiveType.Cube);
                house.name = "Engine_Housing";
                house.transform.SetParent(turret.transform, false);
                house.transform.localPosition = new Vector3(0f, 0.65f, -0.4f);
                house.transform.localScale = new Vector3(2.5f, 1.25f, 2.6f);
                house.GetComponent<Renderer>().sharedMaterial = matYellow;

                GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cabin.name = "Cabin";
                cabin.transform.SetParent(turret.transform, false);
                cabin.transform.localPosition = new Vector3(-0.8f, 0.95f, 0.65f);
                cabin.transform.localScale = new Vector3(0.9f, 1.45f, 1.4f);
                cabin.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            }

            GameObject counterWeight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            counterWeight.name = "Rear_Counterweight";
            counterWeight.transform.SetParent(turret.transform, false);
            counterWeight.transform.localPosition = new Vector3(0f, 0.75f, -1.8f);
            counterWeight.transform.localScale = new Vector3(2.6f, 1.1f, 1.2f);
            counterWeight.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject boomPivot = new GameObject("Boom_Pivot");
            boomPivot.transform.SetParent(turret.transform, false);
            boomPivot.transform.localPosition = new Vector3(0.55f, 0.95f, 0.9f);

            GameObject boomArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boomArm.name = "Boom_Arm";
            boomArm.transform.SetParent(boomPivot.transform, false);
            boomArm.transform.localPosition = new Vector3(0f, 1.5f, 1.1f);
            boomArm.transform.localRotation = Quaternion.Euler(-32f, 0f, 0f);
            boomArm.transform.localScale = new Vector3(0.45f, 0.60f, 3.8f);
            boomArm.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject boomCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            boomCylinder.name = "Hydraulic_Cylinder_Boom";
            boomCylinder.transform.SetParent(boomPivot.transform, false);
            boomCylinder.transform.localPosition = new Vector3(0f, 0.65f, 0.55f);
            boomCylinder.transform.localRotation = Quaternion.Euler(30f, 0f, 0f);
            boomCylinder.transform.localScale = new Vector3(0.18f, 1.1f, 0.18f);
            boomCylinder.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(boomCylinder.GetComponent<Collider>());

            GameObject stickPivot = new GameObject("Stick_Pivot");
            stickPivot.transform.SetParent(boomPivot.transform, false);
            stickPivot.transform.localPosition = new Vector3(0f, 2.85f, 2.4f);

            GameObject stickArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stickArm.name = "Stick_Arm";
            stickArm.transform.SetParent(stickPivot.transform, false);
            stickArm.transform.localPosition = new Vector3(0f, 0.1f, 1.35f);
            stickArm.transform.localRotation = Quaternion.Euler(38f, 0f, 0f);
            stickArm.transform.localScale = new Vector3(0.40f, 0.50f, 3.0f);
            stickArm.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject bucketPivot = new GameObject("Bucket_Pivot");
            bucketPivot.transform.SetParent(stickPivot.transform, false);
            bucketPivot.transform.localPosition = new Vector3(0f, -0.75f, 2.45f);

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
                    scoopModel.transform.localScale = Vector3.one * 1.15f;
                    ApplyMaterialRecursively(scoopModel, matVehicles);
                    bucket = scoopModel;
                }
            }

            if (bucket == null)
            {
                bucket = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bucket.name = "Bucket_Scoop";
                bucket.transform.SetParent(bucketPivot.transform, false);
                bucket.transform.localPosition = new Vector3(0f, -0.4f, 0.35f);
                bucket.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
                bucket.transform.localScale = new Vector3(1.1f, 0.85f, 1.1f);
                bucket.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            }

            BoxCollider bucketCol = bucket.GetComponent<BoxCollider>();
            if (bucketCol == null) bucketCol = bucket.AddComponent<BoxCollider>();
            bucketCol.size = new Vector3(1.4f, 1.0f, 1.2f);

            PhysicsMaterial bucketMat = new PhysicsMaterial("BucketPhysMat")
            {
                dynamicFriction = 0.15f,
                staticFriction = 0.25f,
                bounciness = 0.05f
            };
            bucketCol.sharedMaterial = bucketMat;

            GameObject camObj = new GameObject("Excavator_Camera");
            camObj.transform.SetParent(turret.transform, false);
            camObj.transform.localPosition = new Vector3(-2.2f, 3.5f, -4.2f);
            camObj.transform.localRotation = Quaternion.Euler(18f, 12f, 0f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(exc.transform, false);
            exitObj.transform.localPosition = new Vector3(-2.6f, 0f, 0f);

            ExcavatorController ec = exc.AddComponent<ExcavatorController>();
            ec.turret = turret.transform;
            ec.boom = boomPivot.transform;
            ec.stick = stickPivot.transform;
            ec.bucket = bucketPivot.transform;
            ec.vehicleCamera = cam;
            ec.exitTransform = exitObj.transform;
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

            GameObject cabinCamObj = new GameObject("Crane_Cabin_Camera");
            cabinCamObj.transform.SetParent(slewingUnit.transform, false);
            cabinCamObj.transform.localPosition = new Vector3(1.3f, 1.8f, 1.6f);
            cabinCamObj.transform.localRotation = Quaternion.Euler(22f, 0f, 0f);
            Camera cabinCam = cabinCamObj.AddComponent<Camera>();
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
    }
}
