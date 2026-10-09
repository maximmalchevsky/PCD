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
        private Material matBarrier;

        void Awake()
        {
            if (GameObject.Find("--- CONSTRUCTION SITE (TASKS 12-16) ---") != null) return;
            BuildFullSite();
        }

        public void BuildFullSite()
        {
            InitMaterials();

            GameObject siteRoot = new GameObject("--- CONSTRUCTION SITE (TASKS 12-16) ---");
            siteRoot.transform.position = new Vector3(-50f, 0f, 20f);

            BuildGroundAndEnclosure(siteRoot.transform);
            BuildExcavationPit(siteRoot.transform);
            BuildFoundationUnderConstruction(siteRoot.transform);
            SpawnGranularCargo(siteRoot.transform);

            BuildBulldozer(siteRoot.transform);
            BuildDumpTruck(siteRoot.transform);
            BuildExcavator(siteRoot.transform);
            BuildTowerCrane(siteRoot.transform);
        }

        private void InitMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            matDirt = CreateMat("Mat_Site_Dirt", litShader, new Color(0.44f, 0.34f, 0.22f), 0.1f, 0f);
            matConcrete = CreateMat("Mat_Site_Concrete", litShader, new Color(0.72f, 0.72f, 0.74f), 0.2f, 0.05f);
            matYellow = CreateMat("Mat_Site_Yellow", litShader, new Color(0.96f, 0.76f, 0.06f), 0.6f, 0.2f);
            matOrange = CreateMat("Mat_Site_Orange", litShader, new Color(0.95f, 0.45f, 0.05f), 0.6f, 0.2f);
            matDarkMetal = CreateMat("Mat_Site_DarkMetal", litShader, new Color(0.22f, 0.23f, 0.26f), 0.4f, 0.5f);
            matSand = CreateMat("Mat_Site_Sand", litShader, new Color(0.82f, 0.71f, 0.46f), 0.1f, 0f);
            matRock = CreateMat("Mat_Site_Rock", litShader, new Color(0.48f, 0.49f, 0.51f), 0.15f, 0f);
            matBrick = CreateMat("Mat_Site_Brick", litShader, new Color(0.74f, 0.28f, 0.18f), 0.1f, 0f);
            matWood = CreateMat("Mat_Site_Wood", litShader, new Color(0.58f, 0.42f, 0.26f), 0.2f, 0f);
            matGlass = CreateMat("Mat_Site_Glass", litShader, new Color(0.5f, 0.75f, 0.85f, 0.8f), 0.9f, 0.1f);
            matBarrier = CreateMat("Mat_Site_Barrier", litShader, new Color(0.95f, 0.55f, 0.1f), 0.4f, 0.1f);
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

        private void BuildGroundAndEnclosure(Transform root)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Site_Ground";
            ground.transform.SetParent(root, false);
            ground.transform.localPosition = new Vector3(0f, -0.2f, 0f);
            ground.transform.localScale = new Vector3(34f, 0.4f, 38f);
            ground.GetComponent<Renderer>().sharedMaterial = matDirt;

            GameObject curbN = CreateWall("Curb_North", root, new Vector3(0f, 0.15f, 19f), new Vector3(34f, 0.5f, 0.6f), matConcrete);
            GameObject curbS = CreateWall("Curb_South", root, new Vector3(0f, 0.15f, -19f), new Vector3(34f, 0.5f, 0.6f), matConcrete);
            GameObject curbW = CreateWall("Curb_West", root, new Vector3(-17f, 0.15f, 0f), new Vector3(0.6f, 0.5f, 38f), matConcrete);

            GameObject curbE1 = CreateWall("Curb_East_1", root, new Vector3(17f, 0.15f, 11f), new Vector3(0.6f, 0.5f, 16f), matConcrete);
            GameObject curbE2 = CreateWall("Curb_East_2", root, new Vector3(17f, 0.15f, -11f), new Vector3(0.6f, 0.5f, 16f), matConcrete);

            BuildFenceLine(root, new Vector3(-16.5f, 0f, 18.5f), new Vector3(16.5f, 0f, 18.5f));
            BuildFenceLine(root, new Vector3(-16.5f, 0f, -18.5f), new Vector3(16.5f, 0f, -18.5f));
            BuildFenceLine(root, new Vector3(-16.5f, 0f, -18.5f), new Vector3(-16.5f, 0f, 18.5f));
            BuildFenceLine(root, new Vector3(16.5f, 0f, 3f), new Vector3(16.5f, 0f, 18.5f));
            BuildFenceLine(root, new Vector3(16.5f, 0f, -18.5f), new Vector3(16.5f, 0f, -3f));

            BuildGateBarriers(root);
            BuildBillboard(root);
            BuildFloodlights(root);
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

        private void BuildFenceLine(Transform root, Vector3 start, Vector3 end)
        {
            float dist = Vector3.Distance(start, end);
            int sections = Mathf.Max(1, Mathf.RoundToInt(dist / 2.5f));
            Vector3 dir = (end - start).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);

            for (int i = 0; i <= sections; i++)
            {
                Vector3 p = Vector3.Lerp(start, end, (float)i / sections);

                GameObject post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                post.name = "Fence_Post";
                post.transform.SetParent(root, false);
                post.transform.localPosition = p + Vector3.up * 1f;
                post.transform.localScale = new Vector3(0.08f, 1f, 0.08f);
                post.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

                if (i < sections)
                {
                    Vector3 midP = Vector3.Lerp(p, p + dir * (dist / sections), 0.5f);
                    GameObject meshPanel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    meshPanel.name = "Fence_Panel";
                    meshPanel.transform.SetParent(root, false);
                    meshPanel.transform.localPosition = midP + Vector3.up * 1f;
                    meshPanel.transform.localRotation = rot;
                    meshPanel.transform.localScale = new Vector3(0.03f, 1.8f, dist / sections);
                    meshPanel.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                }
            }
        }

        private void BuildGateBarriers(Transform root)
        {
            Vector3[] barrierPositions = new Vector3[]
            {
                new Vector3(16.2f, 0.45f, 3.4f),
                new Vector3(16.2f, 0.45f, -3.4f),
                new Vector3(14.5f, 0.45f, 2.2f),
                new Vector3(14.5f, 0.45f, -2.2f)
            };

            for (int i = 0; i < barrierPositions.Length; i++)
            {
                GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bar.name = "Warning_Barrier_" + i;
                bar.transform.SetParent(root, false);
                bar.transform.localPosition = barrierPositions[i];
                bar.transform.localScale = new Vector3(0.35f, 0.85f, 1.6f);
                bar.GetComponent<Renderer>().sharedMaterial = matBarrier;

                GameObject stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stripe.name = "Stripe";
                stripe.transform.SetParent(bar.transform, false);
                stripe.transform.localPosition = new Vector3(0f, 0.15f, 0f);
                stripe.transform.localScale = new Vector3(1.02f, 0.28f, 0.98f);
                stripe.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                Object.Destroy(stripe.GetComponent<Collider>());
            }

            Vector3[] conePositions = new Vector3[]
            {
                new Vector3(15.5f, 0.25f, 1.2f),
                new Vector3(15.5f, 0.25f, -1.2f),
                new Vector3(13.8f, 0.25f, 0f)
            };

            for (int i = 0; i < conePositions.Length; i++)
            {
                GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cone.name = "Traffic_Cone_" + i;
                cone.transform.SetParent(root, false);
                cone.transform.localPosition = conePositions[i];
                cone.transform.localScale = new Vector3(0.3f, 0.35f, 0.3f);
                cone.GetComponent<Renderer>().sharedMaterial = matOrange;
            }
        }

        private void BuildBillboard(Transform root)
        {
            GameObject board = new GameObject("Site_Information_Board");
            board.transform.SetParent(root, false);
            board.transform.localPosition = new Vector3(16.3f, 0f, 6.5f);
            board.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            GameObject postL = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postL.transform.SetParent(board.transform, false);
            postL.transform.localPosition = new Vector3(-1.4f, 1.6f, 0f);
            postL.transform.localScale = new Vector3(0.1f, 1.6f, 0.1f);
            postL.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject postR = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            postR.transform.SetParent(board.transform, false);
            postR.transform.localPosition = new Vector3(1.4f, 1.6f, 0f);
            postR.transform.localScale = new Vector3(0.1f, 1.6f, 0.1f);
            postR.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.transform.SetParent(board.transform, false);
            panel.transform.localPosition = new Vector3(0f, 2.2f, 0f);
            panel.transform.localScale = new Vector3(3.2f, 1.6f, 0.08f);
            panel.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject textPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            textPlate.transform.SetParent(board.transform, false);
            textPlate.transform.localPosition = new Vector3(0f, 2.2f, -0.05f);
            textPlate.transform.localScale = new Vector3(3.0f, 1.4f, 0.02f);
            textPlate.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(textPlate.GetComponent<Collider>());
        }

        private void BuildFloodlights(Transform root)
        {
            Vector3[] floodlightPos = new Vector3[]
            {
                new Vector3(-15.5f, 0f, 17.5f),
                new Vector3(15.5f, 0f, 17.5f),
                new Vector3(-15.5f, 0f, -17.5f)
            };

            for (int i = 0; i < floodlightPos.Length; i++)
            {
                GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                pole.name = "Floodlight_Pole_" + i;
                pole.transform.SetParent(root, false);
                pole.transform.localPosition = floodlightPos[i] + Vector3.up * 4.5f;
                pole.transform.localScale = new Vector3(0.16f, 4.5f, 0.16f);
                pole.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

                GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
                head.name = "Lamp_Head";
                head.transform.SetParent(pole.transform, false);
                head.transform.localPosition = new Vector3(0f, 1.02f, 0f);
                head.transform.localScale = new Vector3(4f, 0.35f, 4f);
                head.GetComponent<Renderer>().sharedMaterial = matYellow;

                GameObject lightObj = new GameObject("Light_Source");
                lightObj.transform.SetParent(pole.transform, false);
                lightObj.transform.localPosition = new Vector3(0f, 0.95f, 0f);
                Light l = lightObj.AddComponent<Light>();
                l.type = LightType.Spot;
                l.range = 35f;
                l.spotAngle = 75f;
                l.intensity = 2.4f;
                l.color = new Color(1f, 0.96f, 0.85f);
                lightObj.transform.localRotation = Quaternion.Euler(65f, 0f, 0f);
            }
        }

        private void BuildExcavationPit(Transform root)
        {
            GameObject pit = new GameObject("Excavation_Pit");
            pit.transform.SetParent(root, false);
            pit.transform.localPosition = new Vector3(-5f, 0f, 6f);

            CreateWall("Pit_Wall_N", pit.transform, new Vector3(0f, -0.6f, 5.5f), new Vector3(12f, 1.4f, 0.5f), matDirt);
            CreateWall("Pit_Wall_S", pit.transform, new Vector3(0f, -0.6f, -5.5f), new Vector3(12f, 1.4f, 0.5f), matDirt);
            CreateWall("Pit_Wall_W", pit.transform, new Vector3(-6f, -0.6f, 0f), new Vector3(0.5f, 1.4f, 11f), matDirt);

            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "Pit_Ramp";
            ramp.transform.SetParent(pit.transform, false);
            ramp.transform.localPosition = new Vector3(5.5f, -0.6f, 0f);
            ramp.transform.localRotation = Quaternion.Euler(0f, 0f, 12f);
            ramp.transform.localScale = new Vector3(3.5f, 0.3f, 8f);
            ramp.GetComponent<Renderer>().sharedMaterial = matDirt;

            GameObject pitFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pitFloor.name = "Pit_Floor";
            pitFloor.transform.SetParent(pit.transform, false);
            pitFloor.transform.localPosition = new Vector3(0f, -1.35f, 0f);
            pitFloor.transform.localScale = new Vector3(12f, 0.3f, 11f);
            pitFloor.GetComponent<Renderer>().sharedMaterial = matDirt;

            GameObject mound1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mound1.name = "Soil_Mound_1";
            mound1.transform.SetParent(pit.transform, false);
            mound1.transform.localPosition = new Vector3(-2.8f, -0.95f, 2.5f);
            mound1.transform.localScale = new Vector3(4.5f, 1.4f, 4.2f);
            mound1.GetComponent<Renderer>().sharedMaterial = matDirt;

            GameObject mound2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mound2.name = "Soil_Mound_2";
            mound2.transform.SetParent(pit.transform, false);
            mound2.transform.localPosition = new Vector3(1.5f, -1.05f, -2f);
            mound2.transform.localScale = new Vector3(3.8f, 1.2f, 3.5f);
            mound2.GetComponent<Renderer>().sharedMaterial = matSand;
        }

        private void BuildFoundationUnderConstruction(Transform root)
        {
            GameObject foundation = new GameObject("Foundation_Zone");
            foundation.transform.SetParent(root, false);
            foundation.transform.localPosition = new Vector3(-6f, 0f, -10f);

            CreateWall("Base_Slab", foundation.transform, new Vector3(0f, 0.2f, 0f), new Vector3(14f, 0.4f, 11f), matConcrete);

            for (int x = -5; x <= 5; x += 5)
            {
                for (int z = -4; z <= 4; z += 4)
                {
                    GameObject column = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    column.name = "Column_" + x + "_" + z;
                    column.transform.SetParent(foundation.transform, false);
                    column.transform.localPosition = new Vector3(x, 1.8f, z);
                    column.transform.localScale = new Vector3(0.7f, 2.8f, 0.7f);
                    column.GetComponent<Renderer>().sharedMaterial = matConcrete;

                    GameObject rebar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    rebar.name = "Rebar";
                    rebar.transform.SetParent(column.transform, false);
                    rebar.transform.localPosition = new Vector3(0f, 0.65f, 0f);
                    rebar.transform.localScale = new Vector3(0.2f, 0.35f, 0.2f);
                    rebar.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
                    Object.Destroy(rebar.GetComponent<Collider>());
                }
            }

            CreateWall("Brick_Wall_1", foundation.transform, new Vector3(-5f, 1.1f, -2f), new Vector3(0.5f, 1.4f, 3.6f), matBrick);
            CreateWall("Brick_Wall_2", foundation.transform, new Vector3(0f, 1.1f, 4f), new Vector3(4.5f, 1.4f, 0.5f), matBrick);
        }

        private void SpawnGranularCargo(Transform root)
        {
            GameObject cargoRoot = new GameObject("Granular_And_Cargo_Items");
            cargoRoot.transform.SetParent(root, false);

            for (int i = 0; i < 35; i++)
            {
                float rx = Random.Range(-7.5f, -2.5f);
                float rz = Random.Range(4.5f, 9.5f);
                float ry = Random.Range(-0.8f, -0.4f);

                GameObject sand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sand.name = "Sand_Pebble_" + i;
                sand.transform.SetParent(cargoRoot.transform, false);
                sand.transform.localPosition = new Vector3(rx, ry, rz);
                sand.transform.localScale = Vector3.one * Random.Range(0.38f, 0.54f);
                sand.GetComponent<Renderer>().sharedMaterial = matSand;

                GranularItem gi = sand.AddComponent<GranularItem>();
                gi.itemType = GranularItem.GranularType.Sand;
            }

            for (int i = 0; i < 18; i++)
            {
                float rx = Random.Range(-6f, -0.5f);
                float rz = Random.Range(3f, 8f);
                float ry = Random.Range(-0.8f, -0.3f);

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

            for (int i = 0; i < 24; i++)
            {
                float bx = Random.Range(-1.5f, 4.5f);
                float bz = Random.Range(-4.5f, -1.5f);
                float by = 0.25f + (i % 3) * 0.24f;

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

            BuildHoistableCargo(cargoRoot.transform, "Cargo_Pallet_Bricks", new Vector3(-8f, 0.2f, -4f), new Vector3(1.6f, 1.1f, 1.6f), matWood);
            BuildHoistableCargo(cargoRoot.transform, "Cargo_Steel_Container", new Vector3(-12f, 0.6f, -8f), new Vector3(2.4f, 1.4f, 1.5f), matDarkMetal);
            BuildHoistableCargo(cargoRoot.transform, "Cargo_Concrete_Slab", new Vector3(-11f, 0.2f, 0f), new Vector3(2.8f, 0.35f, 1.8f), matConcrete);
        }

        private void BuildHoistableCargo(Transform parent, string name, Vector3 pos, Vector3 size, Material mat)
        {
            GameObject cargo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cargo.name = name;
            cargo.transform.SetParent(parent, false);
            cargo.transform.localPosition = pos;
            cargo.transform.localScale = size;
            cargo.GetComponent<Renderer>().sharedMaterial = mat;

            Rigidbody rb = cargo.AddComponent<Rigidbody>();
            rb.mass = 220f;
            rb.linearDamping = 1.0f;
            rb.angularDamping = 2.0f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            ring.name = "Hoist_Ring";
            ring.transform.SetParent(cargo.transform, false);
            ring.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            ring.transform.localScale = new Vector3(0.25f, 0.15f, 0.25f);
            ring.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(ring.GetComponent<Collider>());
        }

        private void BuildBulldozer(Transform root)
        {
            GameObject dozer = new GameObject("Bulldozer");
            dozer.transform.SetParent(root, false);
            dozer.transform.localPosition = new Vector3(6f, 0.4f, 6f);
            dozer.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chassis.name = "Chassis";
            chassis.transform.SetParent(dozer.transform, false);
            chassis.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            chassis.transform.localScale = new Vector3(2.2f, 0.85f, 3.8f);
            chassis.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject cabin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabin.name = "Cabin";
            cabin.transform.SetParent(dozer.transform, false);
            cabin.transform.localPosition = new Vector3(0f, 1.45f, -0.4f);
            cabin.transform.localScale = new Vector3(1.6f, 1.25f, 1.7f);
            cabin.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject glass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            glass.name = "Windshield";
            glass.transform.SetParent(cabin.transform, false);
            glass.transform.localPosition = new Vector3(0f, 0.1f, 0.52f);
            glass.transform.localScale = new Vector3(0.85f, 0.65f, 0.05f);
            glass.GetComponent<Renderer>().sharedMaterial = matGlass;
            Object.Destroy(glass.GetComponent<Collider>());

            GameObject trackL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trackL.name = "Track_Left";
            trackL.transform.SetParent(dozer.transform, false);
            trackL.transform.localPosition = new Vector3(-1.25f, 0.35f, 0f);
            trackL.transform.localScale = new Vector3(0.55f, 0.7f, 4.1f);
            trackL.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject trackR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trackR.name = "Track_Right";
            trackR.transform.SetParent(dozer.transform, false);
            trackR.transform.localPosition = new Vector3(1.25f, 0.35f, 0f);
            trackR.transform.localScale = new Vector3(0.55f, 0.7f, 4.1f);
            trackR.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject bladeAssembly = new GameObject("Blade_Assembly");
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
                dynamicFriction = 0.2f,
                staticFriction = 0.3f,
                bounciness = 0.05f
            };
            blade.GetComponent<BoxCollider>().sharedMaterial = bladeMat;

            GameObject armL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            armL.name = "Push_Arm_Left";
            armL.transform.SetParent(bladeAssembly.transform, false);
            armL.transform.localPosition = new Vector3(-1.2f, 0.1f, -0.7f);
            armL.transform.localScale = new Vector3(0.2f, 0.25f, 1.4f);
            armL.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(armL.GetComponent<Collider>());

            GameObject armR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            armR.name = "Push_Arm_Right";
            armR.transform.SetParent(bladeAssembly.transform, false);
            armR.transform.localPosition = new Vector3(1.2f, 0.1f, -0.7f);
            armR.transform.localScale = new Vector3(0.2f, 0.25f, 1.4f);
            armR.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(armR.GetComponent<Collider>());

            GameObject camObj = new GameObject("Bulldozer_Camera");
            camObj.transform.SetParent(dozer.transform, false);
            camObj.transform.localPosition = new Vector3(0f, 3.2f, -4.8f);
            camObj.transform.localRotation = Quaternion.Euler(18f, 0f, 0f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(dozer.transform, false);
            exitObj.transform.localPosition = new Vector3(2.4f, 0f, 0f);

            BulldozerController bc = dozer.AddComponent<BulldozerController>();
            bc.bladeTransform = bladeAssembly.transform;
            bc.vehicleCamera = cam;
            bc.exitTransform = exitObj.transform;
            bc.bladeMin = 0.06f;
            bc.bladeMax = 1.35f;
            bc.bladeSpeed = 0.85f;
        }

        private void BuildDumpTruck(Transform root)
        {
            GameObject truck = new GameObject("Dump_Truck");
            truck.transform.SetParent(root, false);
            truck.transform.localPosition = new Vector3(6f, 0.45f, -6f);
            truck.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

            GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chassis.name = "Frame_Chassis";
            chassis.transform.SetParent(truck.transform, false);
            chassis.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            chassis.transform.localScale = new Vector3(2.1f, 0.5f, 5.8f);
            chassis.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cab.name = "Cab";
            cab.transform.SetParent(truck.transform, false);
            cab.transform.localPosition = new Vector3(0f, 1.45f, 1.75f);
            cab.transform.localScale = new Vector3(2.0f, 1.45f, 1.8f);
            cab.GetComponent<Renderer>().sharedMaterial = matOrange;

            GameObject cabGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabGlass.name = "Cab_Windshield";
            cabGlass.transform.SetParent(cab.transform, false);
            cabGlass.transform.localPosition = new Vector3(0f, 0.15f, 0.52f);
            cabGlass.transform.localScale = new Vector3(0.85f, 0.55f, 0.05f);
            cabGlass.GetComponent<Renderer>().sharedMaterial = matGlass;
            Object.Destroy(cabGlass.GetComponent<Collider>());

            List<Transform> frontW = new List<Transform>();
            List<Transform> rearW = new List<Transform>();

            frontW.Add(CreateWheel(truck.transform, new Vector3(-1.18f, 0.38f, 1.85f)));
            frontW.Add(CreateWheel(truck.transform, new Vector3(1.18f, 0.38f, 1.85f)));

            rearW.Add(CreateWheel(truck.transform, new Vector3(-1.18f, 0.38f, -0.85f)));
            rearW.Add(CreateWheel(truck.transform, new Vector3(1.18f, 0.38f, -0.85f)));
            rearW.Add(CreateWheel(truck.transform, new Vector3(-1.18f, 0.38f, -2.15f)));
            rearW.Add(CreateWheel(truck.transform, new Vector3(1.18f, 0.38f, -2.15f)));

            GameObject bedPivot = new GameObject("Dump_Bed_Pivot");
            bedPivot.transform.SetParent(truck.transform, false);
            bedPivot.transform.localPosition = new Vector3(0f, 0.85f, -2.45f);

            GameObject dumpBed = new GameObject("Dump_Bed_Body");
            dumpBed.transform.SetParent(bedPivot.transform, false);
            dumpBed.transform.localPosition = new Vector3(0f, 0f, 1.6f);

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Bed_Floor";
            floor.transform.SetParent(dumpBed.transform, false);
            floor.transform.localPosition = new Vector3(0f, 0f, 0f);
            floor.transform.localScale = new Vector3(2.2f, 0.18f, 3.5f);
            floor.GetComponent<Renderer>().sharedMaterial = matOrange;

            GameObject wallL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallL.name = "Bed_Wall_Left";
            wallL.transform.SetParent(dumpBed.transform, false);
            wallL.transform.localPosition = new Vector3(-1.05f, 0.55f, 0f);
            wallL.transform.localScale = new Vector3(0.12f, 1.0f, 3.5f);
            wallL.GetComponent<Renderer>().sharedMaterial = matOrange;

            GameObject wallR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallR.name = "Bed_Wall_Right";
            wallR.transform.SetParent(dumpBed.transform, false);
            wallR.transform.localPosition = new Vector3(1.05f, 0.55f, 0f);
            wallR.transform.localScale = new Vector3(0.12f, 1.0f, 3.5f);
            wallR.GetComponent<Renderer>().sharedMaterial = matOrange;

            GameObject wallFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallFront.name = "Bed_Wall_Front";
            wallFront.transform.SetParent(dumpBed.transform, false);
            wallFront.transform.localPosition = new Vector3(0f, 0.7f, 1.72f);
            wallFront.transform.localScale = new Vector3(2.2f, 1.3f, 0.14f);
            wallFront.GetComponent<Renderer>().sharedMaterial = matOrange;

            GameObject tailgatePivot = new GameObject("Tailgate_Pivot");
            tailgatePivot.transform.SetParent(dumpBed.transform, false);
            tailgatePivot.transform.localPosition = new Vector3(0f, 0.95f, -1.72f);

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
            dtc.vehicleCamera = cam;
            dtc.exitTransform = exitObj.transform;
            dtc.frontWheels = frontW.ToArray();
            dtc.rearWheels = rearW.ToArray();
            dtc.maxTiltAngle = 52f;
            dtc.tiltSpeed = 24f;
        }

        private Transform CreateWheel(Transform parent, Vector3 localPos)
        {
            GameObject wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = "Wheel";
            wheel.transform.SetParent(parent, false);
            wheel.transform.localPosition = localPos;
            wheel.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            wheel.transform.localScale = new Vector3(0.75f, 0.22f, 0.75f);
            wheel.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            return wheel.transform;
        }

        private void BuildExcavator(Transform root)
        {
            GameObject exc = new GameObject("Excavator");
            exc.transform.SetParent(root, false);
            exc.transform.localPosition = new Vector3(1f, 0.45f, 6.5f);
            exc.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject undercarriage = GameObject.CreatePrimitive(PrimitiveType.Cube);
            undercarriage.name = "Undercarriage";
            undercarriage.transform.SetParent(exc.transform, false);
            undercarriage.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            undercarriage.transform.localScale = new Vector3(2.6f, 0.75f, 3.6f);
            undercarriage.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject trackL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trackL.name = "Track_L";
            trackL.transform.SetParent(undercarriage.transform, false);
            trackL.transform.localPosition = new Vector3(-0.55f, 0f, 0f);
            trackL.transform.localScale = new Vector3(0.3f, 1.15f, 1.12f);
            trackL.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject trackR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trackR.name = "Track_R";
            trackR.transform.SetParent(undercarriage.transform, false);
            trackR.transform.localPosition = new Vector3(0.55f, 0f, 0f);
            trackR.transform.localScale = new Vector3(0.3f, 1.15f, 1.12f);
            trackR.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            GameObject turret = new GameObject("Turret");
            turret.transform.SetParent(exc.transform, false);
            turret.transform.localPosition = new Vector3(0f, 0.85f, 0f);

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

            GameObject cabGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabGlass.name = "Cab_Glass";
            cabGlass.transform.SetParent(cabin.transform, false);
            cabGlass.transform.localPosition = new Vector3(0f, 0.15f, 0.52f);
            cabGlass.transform.localScale = new Vector3(0.85f, 0.65f, 0.05f);
            cabGlass.GetComponent<Renderer>().sharedMaterial = matGlass;
            Object.Destroy(cabGlass.GetComponent<Collider>());

            GameObject boomPivot = new GameObject("Boom_Pivot");
            boomPivot.transform.SetParent(turret.transform, false);
            boomPivot.transform.localPosition = new Vector3(0.45f, 0.85f, 0.8f);

            GameObject boomArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            boomArm.name = "Boom_Arm";
            boomArm.transform.SetParent(boomPivot.transform, false);
            boomArm.transform.localPosition = new Vector3(0f, 1.5f, 1.1f);
            boomArm.transform.localRotation = Quaternion.Euler(-32f, 0f, 0f);
            boomArm.transform.localScale = new Vector3(0.42f, 0.55f, 3.6f);
            boomArm.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject stickPivot = new GameObject("Stick_Pivot");
            stickPivot.transform.SetParent(boomPivot.transform, false);
            stickPivot.transform.localPosition = new Vector3(0f, 2.75f, 2.3f);

            GameObject stickArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stickArm.name = "Stick_Arm";
            stickArm.transform.SetParent(stickPivot.transform, false);
            stickArm.transform.localPosition = new Vector3(0f, 0.1f, 1.25f);
            stickArm.transform.localRotation = Quaternion.Euler(38f, 0f, 0f);
            stickArm.transform.localScale = new Vector3(0.38f, 0.45f, 2.8f);
            stickArm.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject bucketPivot = new GameObject("Bucket_Pivot");
            bucketPivot.transform.SetParent(stickPivot.transform, false);
            bucketPivot.transform.localPosition = new Vector3(0f, -0.7f, 2.3f);

            GameObject bucket = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bucket.name = "Bucket_Scoop";
            bucket.transform.SetParent(bucketPivot.transform, false);
            bucket.transform.localPosition = new Vector3(0f, -0.4f, 0.35f);
            bucket.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
            bucket.transform.localScale = new Vector3(0.95f, 0.75f, 0.95f);
            bucket.GetComponent<Renderer>().sharedMaterial = matDarkMetal;

            PhysicsMaterial bucketMat = new PhysicsMaterial("BucketPhysMat")
            {
                dynamicFriction = 0.25f,
                staticFriction = 0.35f,
                bounciness = 0.05f
            };
            bucket.GetComponent<BoxCollider>().sharedMaterial = bucketMat;

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
            crane.transform.localPosition = new Vector3(-13f, 0f, -6f);

            GameObject basePlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            basePlate.name = "Crane_Base";
            basePlate.transform.SetParent(crane.transform, false);
            basePlate.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            basePlate.transform.localScale = new Vector3(4.5f, 0.9f, 4.5f);
            basePlate.GetComponent<Renderer>().sharedMaterial = matConcrete;

            GameObject mast = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mast.name = "Lattice_Mast";
            mast.transform.SetParent(crane.transform, false);
            mast.transform.localPosition = new Vector3(0f, 12f, 0f);
            mast.transform.localScale = new Vector3(1.6f, 23f, 1.6f);
            mast.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject ladder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ladder.name = "Mast_Ladder";
            ladder.transform.SetParent(crane.transform, false);
            ladder.transform.localPosition = new Vector3(0.85f, 12f, 0f);
            ladder.transform.localScale = new Vector3(0.12f, 23f, 0.45f);
            ladder.GetComponent<Renderer>().sharedMaterial = matDarkMetal;
            Object.Destroy(ladder.GetComponent<Collider>());

            GameObject slewingUnit = new GameObject("Slewing_Jib_Unit");
            slewingUnit.transform.SetParent(crane.transform, false);
            slewingUnit.transform.localPosition = new Vector3(0f, 23.5f, 0f);

            GameObject cab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cab.name = "Operator_Cabin";
            cab.transform.SetParent(slewingUnit.transform, false);
            cab.transform.localPosition = new Vector3(1.2f, 0.5f, 0.8f);
            cab.transform.localScale = new Vector3(1.1f, 1.6f, 1.4f);
            cab.GetComponent<Renderer>().sharedMaterial = matYellow;

            GameObject cabGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabGlass.name = "Cab_Glass";
            cabGlass.transform.SetParent(cab.transform, false);
            cabGlass.transform.localPosition = new Vector3(0f, 0.1f, 0.52f);
            cabGlass.transform.localScale = new Vector3(0.85f, 0.7f, 0.05f);
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

            GameObject camObj = new GameObject("Crane_Camera");
            camObj.transform.SetParent(slewingUnit.transform, false);
            camObj.transform.localPosition = new Vector3(0f, 3.8f, -2.5f);
            camObj.transform.localRotation = Quaternion.Euler(24f, 0f, 0f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.enabled = false;

            GameObject exitObj = new GameObject("Exit_Point");
            exitObj.transform.SetParent(crane.transform, false);
            exitObj.transform.localPosition = new Vector3(2.4f, 0.1f, 0f);

            TowerCraneController tcc = crane.AddComponent<TowerCraneController>();
            tcc.jib = slewingUnit.transform;
            tcc.trolley = trolley.transform;
            tcc.hook = hook.transform;
            tcc.vehicleCamera = cam;
            tcc.exitTransform = exitObj.transform;
            tcc.minTrolleyDist = 3.5f;
            tcc.maxTrolleyDist = 23f;
            tcc.minCableLength = 2.5f;
            tcc.maxCableLength = 22f;
        }
    }
}
