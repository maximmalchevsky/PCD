using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Task4_5;
using Task6_7;

namespace CityEditor
{
    public static class CityBuilder
    {
        [MenuItem("City/Build Full City Scene")]
        public static void BuildCity()
        {
            AssetPreparer.PrepareAll();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject lightingRoot = new GameObject("--- LIGHTING ---");
            GameObject sunObj = new GameObject("Sun Light");
            sunObj.transform.SetParent(lightingRoot.transform);
            Light sunLight = sunObj.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.color = new Color(1f, 0.96f, 0.90f);
            sunLight.intensity = 1.35f;
            sunLight.shadows = LightShadows.Soft;
            sunObj.transform.rotation = Quaternion.Euler(50f, -35f, 0f);

            RenderSettings.ambientLight = new Color(0.28f, 0.32f, 0.38f);

            GameObject envRoot = new GameObject("--- ENVIRONMENT ---");
            GameObject groundObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            groundObj.name = "Ground";
            groundObj.transform.SetParent(envRoot.transform);
            groundObj.transform.position = new Vector3(8f, -0.5f, 0f);
            groundObj.transform.localScale = new Vector3(140f, 1f, 190f);

            Material groundMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            groundMat.color = new Color(0.18f, 0.22f, 0.20f);
            groundMat.SetFloat("_Smoothness", 0.1f);
            groundObj.GetComponent<Renderer>().sharedMaterial = groundMat;

            GameObject roadsRoot = new GameObject("--- ROADS & SIDEWALKS ---");
            GameObject roadStraightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/road-straight.fbx");
            GameObject roadCrossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/road-crossroad.fbx");
            GameObject lampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/light-curved.fbx");
            Material sidewalkMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Sidewalk.mat");
            Material envMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Environment.mat");

            float[] aveX = new float[] { -22f, 0f, 22f };
            float[] crossZ = new float[] { -48f, -16f, 16f, 48f };

            foreach (float x in aveX)
            {
                foreach (float z in crossZ)
                {
                    if (roadCrossPrefab != null)
                    {
                        GameObject cross = (GameObject)PrefabUtility.InstantiatePrefab(roadCrossPrefab);
                        cross.transform.SetParent(roadsRoot.transform);
                        cross.transform.position = new Vector3(x, 0.00f, z);
                        cross.transform.localScale = new Vector3(4f, 1f, 4f);
                        CleanImportedObject(cross);
                        if (envMat != null) ApplyMaterialToRenderers(cross, envMat);
                        AddBoxColliderToHierarchy(cross);
                    }
                }
            }

            foreach (float x in aveX)
            {
                for (float z = -72f; z <= 72f; z += 4f)
                {
                    bool isCross = false;
                    foreach (float cz in crossZ)
                    {
                        if (Mathf.Abs(z - cz) < 2.5f) { isCross = true; break; }
                    }
                    if (!isCross && roadStraightPrefab != null)
                    {
                        GameObject str = (GameObject)PrefabUtility.InstantiatePrefab(roadStraightPrefab);
                        str.transform.SetParent(roadsRoot.transform);
                        str.transform.position = new Vector3(x, 0.00f, z);
                        str.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                        str.transform.localScale = new Vector3(4f, 1f, 4f);
                        CleanImportedObject(str);
                        if (envMat != null) ApplyMaterialToRenderers(str, envMat);
                        AddBoxColliderToHierarchy(str);
                    }
                }
            }

            foreach (float z in crossZ)
            {
                for (float x = -34f; x <= 30f; x += 4f)
                {
                    bool isCross = false;
                    foreach (float ax in aveX)
                    {
                        if (Mathf.Abs(x - ax) < 2.5f) { isCross = true; break; }
                    }
                    if (!isCross && roadStraightPrefab != null)
                    {
                        GameObject str = (GameObject)PrefabUtility.InstantiatePrefab(roadStraightPrefab);
                        str.transform.SetParent(roadsRoot.transform);
                        str.transform.position = new Vector3(x, 0.00f, z);
                        str.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
                        str.transform.localScale = new Vector3(4f, 1f, 4f);
                        CleanImportedObject(str);
                        if (envMat != null) ApplyMaterialToRenderers(str, envMat);
                        AddBoxColliderToHierarchy(str);
                    }
                }
            }

            float[] swXOffsets = new float[] { -3.8f, 3.8f };
            foreach (float ax in aveX)
            {
                foreach (float off in swXOffsets)
                {
                    float sx = ax + off;
                    SpawnSidewalkSegment(new Vector3(sx, 0.08f, -61f), new Vector3(3.6f, 0.16f, 22f), sidewalkMat, roadsRoot.transform);
                    SpawnSidewalkSegment(new Vector3(sx, 0.08f, -32f), new Vector3(3.6f, 0.16f, 28f), sidewalkMat, roadsRoot.transform);
                    SpawnSidewalkSegment(new Vector3(sx, 0.08f, 0f), new Vector3(3.6f, 0.16f, 28f), sidewalkMat, roadsRoot.transform);
                    SpawnSidewalkSegment(new Vector3(sx, 0.08f, 32f), new Vector3(3.6f, 0.16f, 28f), sidewalkMat, roadsRoot.transform);
                    SpawnSidewalkSegment(new Vector3(sx, 0.08f, 61f), new Vector3(3.6f, 0.16f, 22f), sidewalkMat, roadsRoot.transform);
                }
            }

            float[] swZOffsets = new float[] { -3.8f, 3.8f };
            foreach (float cz in crossZ)
            {
                foreach (float off in swZOffsets)
                {
                    float sz = cz + off;
                    SpawnSidewalkSegment(new Vector3(-29f, 0.08f, sz), new Vector3(10f, 0.16f, 3.6f), sidewalkMat, roadsRoot.transform);
                    SpawnSidewalkSegment(new Vector3(-11f, 0.08f, sz), new Vector3(14.4f, 0.16f, 3.6f), sidewalkMat, roadsRoot.transform);
                    SpawnSidewalkSegment(new Vector3(11f, 0.08f, sz), new Vector3(14.4f, 0.16f, 3.6f), sidewalkMat, roadsRoot.transform);
                    SpawnSidewalkSegment(new Vector3(28f, 0.08f, sz), new Vector3(6f, 0.16f, 3.6f), sidewalkMat, roadsRoot.transform);
                }
            }

            Material zebraMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Zebra.mat");

            float[] aveXList = new float[] { -22f, 0f, 22f };
            foreach (float ax in aveXList)
            {
                SpawnDashedCenterLine(new Vector3(ax, 0.02f, -72f), new Vector3(ax, 0.02f, -21f), zebraMat, roadsRoot.transform);
                SpawnDashedCenterLine(new Vector3(ax, 0.02f, -11f), new Vector3(ax, 0.02f, 11f), zebraMat, roadsRoot.transform);
                SpawnDashedCenterLine(new Vector3(ax, 0.02f, 21f), new Vector3(ax, 0.02f, 72f), zebraMat, roadsRoot.transform);
            }

            foreach (float cz in crossZ)
            {
                SpawnDashedCenterLine(new Vector3(-34f, 0.02f, cz), new Vector3(-24f, 0.02f, cz), zebraMat, roadsRoot.transform);
                SpawnDashedCenterLine(new Vector3(-20f, 0.02f, cz), new Vector3(-2f, 0.02f, cz), zebraMat, roadsRoot.transform);
                SpawnDashedCenterLine(new Vector3(2f, 0.02f, cz), new Vector3(20f, 0.02f, cz), zebraMat, roadsRoot.transform);
                SpawnDashedCenterLine(new Vector3(24f, 0.02f, cz), new Vector3(30f, 0.02f, cz), zebraMat, roadsRoot.transform);
            }

            SpawnStopLine(new Vector3(1.15f, 0.02f, -20.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(0f, 0.02f, -18.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(-1.15f, 0.02f, -11.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(0f, 0.02f, -13.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(-5.0f, 0.02f, -17.15f), false, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(-3.2f, 0.02f, -16.0f), false, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(5.0f, 0.02f, -14.85f), false, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(3.2f, 0.02f, -16.0f), false, zebraMat, roadsRoot.transform);

            SpawnStopLine(new Vector3(1.15f, 0.02f, 11.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(0f, 0.02f, 13.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(-1.15f, 0.02f, 20.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(0f, 0.02f, 18.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(-5.0f, 0.02f, 14.85f), false, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(-3.2f, 0.02f, 16.0f), false, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(5.0f, 0.02f, 17.15f), false, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(3.2f, 0.02f, 16.0f), false, zebraMat, roadsRoot.transform);

            SpawnStopLine(new Vector3(-20.85f, 0.02f, -20.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(-22f, 0.02f, -18.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(-23.15f, 0.02f, -11.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(-22f, 0.02f, -13.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(-20.85f, 0.02f, 11.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(-22f, 0.02f, 13.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(-23.15f, 0.02f, 20.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(-22f, 0.02f, 18.5f), true, zebraMat, roadsRoot.transform);

            SpawnStopLine(new Vector3(23.15f, 0.02f, -20.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(22f, 0.02f, -18.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(20.85f, 0.02f, -11.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(22f, 0.02f, -13.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(23.15f, 0.02f, 11.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(22f, 0.02f, 13.5f), true, zebraMat, roadsRoot.transform);
            SpawnStopLine(new Vector3(20.85f, 0.02f, 20.5f), true, zebraMat, roadsRoot.transform);
            SpawnZebraCrosswalk(new Vector3(22f, 0.02f, 18.5f), true, zebraMat, roadsRoot.transform);

            if (lampPrefab != null)
            {
                for (float z = -64f; z <= 64f; z += 16f)
                {
                    SpawnLamp(lampPrefab, new Vector3(-24.2f, 0.02f, z), Quaternion.Euler(0f, 90f, 0f), envRoot.transform, envMat);
                    SpawnLamp(lampPrefab, new Vector3(-19.8f, 0.02f, z), Quaternion.Euler(0f, -90f, 0f), envRoot.transform, envMat);
                    SpawnLamp(lampPrefab, new Vector3(-2.2f, 0.02f, z), Quaternion.Euler(0f, 90f, 0f), envRoot.transform, envMat);
                    SpawnLamp(lampPrefab, new Vector3(2.2f, 0.02f, z), Quaternion.Euler(0f, -90f, 0f), envRoot.transform, envMat);
                    SpawnLamp(lampPrefab, new Vector3(19.8f, 0.02f, z), Quaternion.Euler(0f, 90f, 0f), envRoot.transform, envMat);
                    SpawnLamp(lampPrefab, new Vector3(24.2f, 0.02f, z), Quaternion.Euler(0f, -90f, 0f), envRoot.transform, envMat);
                }
            }

            GameObject greeneryRoot = new GameObject("--- GREENERY (TREES & BUSHES) ---");
            Material darkFoliage = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Foliage_Dark.mat");
            Material lightFoliage = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Foliage_Light.mat");
            Material trunkMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Tree_Trunk.mat");

            SpawnTree(new Vector3(-11f, 0.05f, 0f), darkFoliage, lightFoliage, trunkMat, greeneryRoot.transform);
            SpawnTree(new Vector3(11f, 0.05f, 0f), lightFoliage, darkFoliage, trunkMat, greeneryRoot.transform);

            SpawnBush(new Vector3(-11.5f, 0.05f, -2.5f), new Vector3(1.3f, 1.0f, 1.3f), darkFoliage, greeneryRoot.transform);
            SpawnBush(new Vector3(-10.5f, 0.05f, 2.5f), new Vector3(1.1f, 0.9f, 1.1f), lightFoliage, greeneryRoot.transform);
            SpawnBush(new Vector3(10.5f, 0.05f, -2.5f), new Vector3(1.2f, 0.95f, 1.2f), lightFoliage, greeneryRoot.transform);
            SpawnBush(new Vector3(11.5f, 0.05f, 2.5f), new Vector3(1.3f, 1.0f, 1.3f), darkFoliage, greeneryRoot.transform);

            GameObject trafficLightsRoot = new GameObject("--- TRAFFIC LIGHTS ---");
            Material tlPoleMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Pole.mat");
            Material tlHousingMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Housing.mat");
            Material tlRedOn = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Red_On.mat");
            Material tlRedOff = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Red_Off.mat");
            Material tlYelOn = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Yellow_On.mat");
            Material tlYelOff = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Yellow_Off.mat");
            Material tlGrnOn = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Green_On.mat");
            Material tlGrnOff = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Environment/Mat_Traffic_Green_Off.mat");

            SpawnTrafficLightColumn(new Vector3(2.35f, 0f, -20.5f), Quaternion.Euler(0f, 180f, 0f), TrafficLightController.LightState.Green, 0f, new Vector3(1.0f, 0.8f, -19.5f), new Vector3(1.8f, 1.6f, 2.4f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);
            SpawnTrafficLightColumn(new Vector3(-2.35f, 0f, -11.5f), Quaternion.Euler(0f, 0f, 0f), TrafficLightController.LightState.Green, 0f, new Vector3(-1.0f, 0.8f, -12.5f), new Vector3(1.8f, 1.6f, 2.4f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);
            SpawnTrafficLightColumn(new Vector3(-5.0f, 0f, -18.35f), Quaternion.Euler(0f, -90f, 0f), TrafficLightController.LightState.Red, 0f, new Vector3(-3.8f, 0.8f, -17.0f), new Vector3(2.4f, 1.6f, 1.8f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);
            SpawnTrafficLightColumn(new Vector3(5.0f, 0f, -13.65f), Quaternion.Euler(0f, 90f, 0f), TrafficLightController.LightState.Red, 0f, new Vector3(3.8f, 0.8f, -15.0f), new Vector3(2.4f, 1.6f, 1.8f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);

            SpawnTrafficLightColumn(new Vector3(2.35f, 0f, 11.5f), Quaternion.Euler(0f, 180f, 0f), TrafficLightController.LightState.Green, 0f, new Vector3(1.0f, 0.8f, 12.5f), new Vector3(1.8f, 1.6f, 2.4f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);
            SpawnTrafficLightColumn(new Vector3(-2.35f, 0f, 20.5f), Quaternion.Euler(0f, 0f, 0f), TrafficLightController.LightState.Green, 0f, new Vector3(-1.0f, 0.8f, 19.5f), new Vector3(1.8f, 1.6f, 2.4f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);
            SpawnTrafficLightColumn(new Vector3(-5.0f, 0f, 13.65f), Quaternion.Euler(0f, -90f, 0f), TrafficLightController.LightState.Red, 0f, new Vector3(-3.8f, 0.8f, 15.0f), new Vector3(2.4f, 1.6f, 1.8f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);
            SpawnTrafficLightColumn(new Vector3(5.0f, 0f, 18.35f), Quaternion.Euler(0f, 90f, 0f), TrafficLightController.LightState.Red, 0f, new Vector3(3.8f, 0.8f, 17.0f), new Vector3(2.4f, 1.6f, 1.8f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);

            SpawnTrafficLightColumn(new Vector3(-19.65f, 0f, -20.5f), Quaternion.Euler(0f, 180f, 0f), TrafficLightController.LightState.Green, 0f, new Vector3(-21.0f, 0.8f, -19.5f), new Vector3(1.8f, 1.6f, 2.4f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);
            SpawnTrafficLightColumn(new Vector3(24.35f, 0f, -20.5f), Quaternion.Euler(0f, 180f, 0f), TrafficLightController.LightState.Green, 0f, new Vector3(23.0f, 0.8f, -19.5f), new Vector3(1.8f, 1.6f, 2.4f), tlPoleMat, tlHousingMat, zebraMat, tlRedOn, tlRedOff, tlYelOn, tlYelOff, tlGrnOn, tlGrnOff, trafficLightsRoot.transform);

            GameObject bldRoot = new GameObject("--- COMMERCIAL BUILDINGS & SKYSCRAPERS ---");
            Material commMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Commercial/Mat_Commercial.mat");

            string[] skyScrapers = new string[]
            {
                "Assets/CityAssets/Commercial/building-skyscraper-a.fbx",
                "Assets/CityAssets/Commercial/building-skyscraper-b.fbx",
                "Assets/CityAssets/Commercial/building-skyscraper-c.fbx",
                "Assets/CityAssets/Commercial/building-skyscraper-d.fbx",
                "Assets/CityAssets/Commercial/building-skyscraper-e.fbx"
            };

            string[] commStores = new string[]
            {
                "Assets/CityAssets/Commercial/building-a.fbx",
                "Assets/CityAssets/Commercial/building-b.fbx",
                "Assets/CityAssets/Commercial/building-c.fbx",
                "Assets/CityAssets/Commercial/building-d.fbx",
                "Assets/CityAssets/Commercial/building-e.fbx",
                "Assets/CityAssets/Commercial/building-f.fbx",
                "Assets/CityAssets/Commercial/building-g.fbx",
                "Assets/CityAssets/Commercial/building-h.fbx",
                "Assets/CityAssets/Commercial/building-i.fbx",
                "Assets/CityAssets/Commercial/building-j.fbx",
                "Assets/CityAssets/Commercial/building-k.fbx",
                "Assets/CityAssets/Commercial/building-l.fbx",
                "Assets/CityAssets/Commercial/building-m.fbx",
                "Assets/CityAssets/Commercial/building-n.fbx"
            };

            List<GameObject> skyPrefabs = new List<GameObject>();
            foreach (string p in skyScrapers)
            {
                GameObject b = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                if (b != null) skyPrefabs.Add(b);
            }

            List<GameObject> storePrefabs = new List<GameObject>();
            foreach (string p in commStores)
            {
                GameObject b = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                if (b != null) storePrefabs.Add(b);
            }

            int sIdx = 0;
            int stIdx = 0;

            float[] westBlockZ = new float[] { -60f, -38f, -26f, -6f, 6f, 26f, 38f, 60f };
            foreach (float z in westBlockZ)
            {
                if (skyPrefabs.Count > 0 && sIdx % 2 == 0)
                {
                    SpawnBuilding(skyPrefabs[sIdx++ % skyPrefabs.Count], new Vector3(-30f, 0f, z), Quaternion.Euler(0f, 90f, 0f), new Vector3(4f, 4f, 4f), bldRoot.transform, commMat);
                }
                else if (storePrefabs.Count > 0)
                {
                    SpawnBuilding(storePrefabs[stIdx++ % storePrefabs.Count], new Vector3(-30f, 0f, z), Quaternion.Euler(0f, 90f, 0f), new Vector3(3.8f, 3.8f, 3.8f), bldRoot.transform, commMat);
                }
            }

            float[] centerBlockZ = new float[] { -40f, -32f, -24f, -5f, 0f, 5f, 24f, 32f, 40f };
            foreach (float z in centerBlockZ)
            {
                float bScale = (Mathf.Abs(z) <= 5.1f) ? 3.2f : 3.8f;
                float sScale = (Mathf.Abs(z) <= 5.1f) ? 3.0f : 3.5f;

                if (skyPrefabs.Count > 0 && sIdx % 3 == 0)
                {
                    SpawnBuilding(skyPrefabs[sIdx++ % skyPrefabs.Count], new Vector3(-11f, 0f, z), Quaternion.Euler(0f, 90f, 0f), new Vector3(bScale, bScale, bScale), bldRoot.transform, commMat);
                }
                else if (storePrefabs.Count > 0)
                {
                    SpawnBuilding(storePrefabs[stIdx++ % storePrefabs.Count], new Vector3(-11f, 0f, z), Quaternion.Euler(0f, 90f, 0f), new Vector3(sScale, sScale, sScale), bldRoot.transform, commMat);
                }

                if (skyPrefabs.Count > 0 && sIdx % 2 == 0)
                {
                    SpawnBuilding(skyPrefabs[sIdx++ % skyPrefabs.Count], new Vector3(11f, 0f, z), Quaternion.Euler(0f, -90f, 0f), new Vector3(bScale, bScale, bScale), bldRoot.transform, commMat);
                }
                else if (storePrefabs.Count > 0)
                {
                    SpawnBuilding(storePrefabs[stIdx++ % storePrefabs.Count], new Vector3(11f, 0f, z), Quaternion.Euler(0f, -90f, 0f), new Vector3(sScale, sScale, sScale), bldRoot.transform, commMat);
                }
            }

            float[] eastBlockZ = new float[] { -60f, -38f, -26f, -6f, 6f, 26f, 38f, 60f };
            foreach (float z in eastBlockZ)
            {
                if (storePrefabs.Count > 0)
                {
                    SpawnBuilding(storePrefabs[stIdx++ % storePrefabs.Count], new Vector3(29f, 0f, z), Quaternion.Euler(0f, -90f, 0f), new Vector3(3.8f, 3.8f, 3.8f), bldRoot.transform, commMat);
                }
            }

            GameObject detailsRoot = new GameObject("--- STREET DETAILS & PROPS ---");
            GameObject awningPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Commercial/detail-awning.fbx");
            GameObject awningWidePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Commercial/detail-awning-wide.fbx");
            GameObject parasolAPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Commercial/detail-parasol-a.fbx");
            GameObject parasolBPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Commercial/detail-parasol-b.fbx");
            GameObject dumpsterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/dumpster.fbx");
            GameObject conePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/construction-cone.fbx");
            GameObject barrierPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/construction-barrier.fbx");
            GameObject stopSignPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/road-sign-stop.fbx");
            GameObject streetSignPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/road-sign-street.fbx");
            GameObject warnSignPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/road-sign-warning.fbx");
            GameObject elecPolePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Environment/Models/FBX format/electricity-pole.fbx");

            if (awningPrefab != null)
            {
                SpawnProp(awningPrefab, new Vector3(-8.8f, 2.2f, -5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.2f, 1.2f, 1.2f), detailsRoot.transform, commMat, false);
                SpawnProp(awningPrefab, new Vector3(-8.8f, 2.2f, 5f), Quaternion.Euler(0f, 90f, 0f), new Vector3(1.2f, 1.2f, 1.2f), detailsRoot.transform, commMat, false);
                SpawnProp(awningPrefab, new Vector3(8.8f, 2.2f, -5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.2f, 1.2f, 1.2f), detailsRoot.transform, commMat, false);
                SpawnProp(awningPrefab, new Vector3(8.8f, 2.2f, 5f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.2f, 1.2f, 1.2f), detailsRoot.transform, commMat, false);
            }

            if (parasolAPrefab != null)
            {
                SpawnProp(parasolAPrefab, new Vector3(-6.5f, 0.05f, -4f), Quaternion.identity, Vector3.one, detailsRoot.transform, commMat);
                SpawnProp(parasolAPrefab, new Vector3(-6.5f, 0.05f, 4f), Quaternion.identity, Vector3.one, detailsRoot.transform, commMat);
            }
            if (parasolBPrefab != null)
            {
                SpawnProp(parasolBPrefab, new Vector3(6.5f, 0.05f, -4f), Quaternion.identity, Vector3.one, detailsRoot.transform, commMat);
                SpawnProp(parasolBPrefab, new Vector3(6.5f, 0.05f, 4f), Quaternion.identity, Vector3.one, detailsRoot.transform, commMat);
            }

            if (dumpsterPrefab != null)
            {
                SpawnProp(dumpsterPrefab, new Vector3(-35f, 0.05f, -20f), Quaternion.Euler(0f, 90f, 0f), Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(dumpsterPrefab, new Vector3(-35f, 0.05f, 20f), Quaternion.Euler(0f, 90f, 0f), Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(dumpsterPrefab, new Vector3(32f, 0.05f, -20f), Quaternion.Euler(0f, -90f, 0f), Vector3.one, detailsRoot.transform, envMat);
            }

            if (stopSignPrefab != null)
            {
                SpawnProp(stopSignPrefab, new Vector3(-2.2f, 0.02f, -14.2f), Quaternion.Euler(0f, 0f, 0f), Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(stopSignPrefab, new Vector3(2.2f, 0.02f, 14.2f), Quaternion.Euler(0f, 180f, 0f), Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(stopSignPrefab, new Vector3(-24.2f, 0.02f, -14.2f), Quaternion.Euler(0f, 0f, 0f), Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(stopSignPrefab, new Vector3(24.2f, 0.02f, 14.2f), Quaternion.Euler(0f, 180f, 0f), Vector3.one, detailsRoot.transform, envMat);
            }

            if (streetSignPrefab != null)
            {
                SpawnProp(streetSignPrefab, new Vector3(-2.8f, 0.02f, 18.8f), Quaternion.Euler(0f, 45f, 0f), Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(streetSignPrefab, new Vector3(2.8f, 0.02f, -18.8f), Quaternion.Euler(0f, 225f, 0f), Vector3.one, detailsRoot.transform, envMat);
            }

            if (conePrefab != null)
            {
                SpawnProp(conePrefab, new Vector3(-23f, 0.02f, 62f), Quaternion.identity, Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(conePrefab, new Vector3(-22f, 0.02f, 63f), Quaternion.identity, Vector3.one, detailsRoot.transform, envMat);
                SpawnProp(conePrefab, new Vector3(-21f, 0.02f, 64f), Quaternion.identity, Vector3.one, detailsRoot.transform, envMat);
            }

            if (barrierPrefab != null)
            {
                SpawnProp(barrierPrefab, new Vector3(-22f, 0.02f, 66f), Quaternion.Euler(0f, 0f, 0f), Vector3.one, detailsRoot.transform, envMat);
            }
            if (warnSignPrefab != null)
            {
                SpawnProp(warnSignPrefab, new Vector3(-24.5f, 0.02f, 58f), Quaternion.Euler(0f, 0f, 0f), Vector3.one, detailsRoot.transform, envMat);
            }

            if (elecPolePrefab != null)
            {
                for (float z = -60f; z <= 60f; z += 30f)
                {
                    SpawnProp(elecPolePrefab, new Vector3(-33f, 0.02f, z), Quaternion.identity, Vector3.one, detailsRoot.transform, envMat);
                }
            }

            GameObject railwayRoot = new GameObject("--- RAILWAY & STATION ---");
            GameObject railStraightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/TrainKit/railroad-straight.fbx");
            Material trainKitMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/TrainKit/Mat_TrainKit.mat");
            float railX = 36f;

            if (railStraightPrefab != null)
            {
                for (float rz = -88f; rz <= 88f; rz += 4f)
                {
                    GameObject rail = (GameObject)PrefabUtility.InstantiatePrefab(railStraightPrefab);
                    rail.transform.SetParent(railwayRoot.transform);
                    rail.transform.position = new Vector3(railX, 0.05f, rz);
                    rail.transform.rotation = Quaternion.identity;
                    rail.transform.localScale = Vector3.one;
                    CleanImportedObject(rail);
                    if (trainKitMat != null) ApplyMaterialToRenderers(rail, trainKitMat);
                }
            }

            GameObject stationPlatform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stationPlatform.name = "Station_Platform";
            stationPlatform.transform.SetParent(railwayRoot.transform);
            stationPlatform.transform.position = new Vector3(33.2f, 0.25f, 0f);
            stationPlatform.transform.localScale = new Vector3(3.2f, 0.5f, 36f);

            Material platMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            platMat.color = new Color(0.45f, 0.45f, 0.48f);
            stationPlatform.GetComponent<Renderer>().sharedMaterial = platMat;

            GameObject stationBuildingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Commercial/building-g.fbx");
            if (stationBuildingPrefab != null)
            {
                SpawnBuilding(stationBuildingPrefab, new Vector3(29.5f, 0.5f, 0f), Quaternion.Euler(0f, -90f, 0f), new Vector3(3.5f, 3.5f, 3.5f), railwayRoot.transform, commMat);
            }

            GameObject overhangPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Commercial/detail-overhang-wide.fbx");
            if (overhangPrefab != null)
            {
                SpawnProp(overhangPrefab, new Vector3(33f, 3f, -6f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.5f, 1.5f, 1.5f), railwayRoot.transform, commMat, false);
                SpawnProp(overhangPrefab, new Vector3(33f, 3f, 6f), Quaternion.Euler(0f, -90f, 0f), new Vector3(1.5f, 1.5f, 1.5f), railwayRoot.transform, commMat, false);
            }

            GameObject trainRoot = new GameObject("BigVehicle_Train");
            trainRoot.transform.SetParent(railwayRoot.transform);
            trainRoot.transform.position = new Vector3(railX, 0.1f, -85f);

            TrainController trainCtrl = trainRoot.AddComponent<TrainController>();
            trainCtrl.startZ = -85f;
            trainCtrl.stationZ = 0f;
            trainCtrl.endZ = 85f;
            trainCtrl.maxSpeed = 16f;
            trainCtrl.stationWaitDuration = 5f;

            GameObject bulletLocoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/TrainKit/train-electric-bullet-a.fbx");
            GameObject coachContainerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/TrainKit/train-carriage-container-blue.fbx");
            GameObject coachDirtPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/TrainKit/train-carriage-dirt.fbx");
            GameObject coachBoxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/TrainKit/train-carriage-box.fbx");
            GameObject bulletRearPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/TrainKit/train-electric-bullet-c.fbx");

            if (bulletLocoPrefab != null)
            {
                GameObject loco = (GameObject)PrefabUtility.InstantiatePrefab(bulletLocoPrefab);
                loco.name = "Train_Locomotive";
                loco.transform.SetParent(trainRoot.transform);
                loco.transform.localPosition = new Vector3(0f, 0f, 0f);
                loco.transform.localRotation = Quaternion.identity;
                loco.transform.localScale = Vector3.one;
                CleanImportedObject(loco);
                if (trainKitMat != null) ApplyMaterialToRenderers(loco, trainKitMat);
                AddBoxColliderToHierarchy(loco);
            }

            if (coachContainerPrefab != null)
            {
                GameObject coach = (GameObject)PrefabUtility.InstantiatePrefab(coachContainerPrefab);
                coach.name = "Train_Coach_1";
                coach.transform.SetParent(trainRoot.transform);
                coach.transform.localPosition = new Vector3(0f, 0f, -4.2f);
                coach.transform.localRotation = Quaternion.identity;
                coach.transform.localScale = Vector3.one;
                CleanImportedObject(coach);
                if (trainKitMat != null) ApplyMaterialToRenderers(coach, trainKitMat);
                AddBoxColliderToHierarchy(coach);
            }

            if (coachDirtPrefab != null)
            {
                GameObject coach = (GameObject)PrefabUtility.InstantiatePrefab(coachDirtPrefab);
                coach.name = "Train_Coach_2";
                coach.transform.SetParent(trainRoot.transform);
                coach.transform.localPosition = new Vector3(0f, 0f, -8.4f);
                coach.transform.localRotation = Quaternion.identity;
                coach.transform.localScale = Vector3.one;
                CleanImportedObject(coach);
                if (trainKitMat != null) ApplyMaterialToRenderers(coach, trainKitMat);
                AddBoxColliderToHierarchy(coach);
            }

            if (coachBoxPrefab != null)
            {
                GameObject coach = (GameObject)PrefabUtility.InstantiatePrefab(coachBoxPrefab);
                coach.name = "Train_Coach_3";
                coach.transform.SetParent(trainRoot.transform);
                coach.transform.localPosition = new Vector3(0f, 0f, -12.6f);
                coach.transform.localRotation = Quaternion.identity;
                coach.transform.localScale = Vector3.one;
                CleanImportedObject(coach);
                if (trainKitMat != null) ApplyMaterialToRenderers(coach, trainKitMat);
                AddBoxColliderToHierarchy(coach);
            }

            if (bulletRearPrefab != null)
            {
                GameObject rear = (GameObject)PrefabUtility.InstantiatePrefab(bulletRearPrefab);
                rear.name = "Train_RearCab";
                rear.transform.SetParent(trainRoot.transform);
                rear.transform.localPosition = new Vector3(0f, 0f, -16.8f);
                rear.transform.localRotation = Quaternion.identity;
                rear.transform.localScale = Vector3.one;
                CleanImportedObject(rear);
                if (trainKitMat != null) ApplyMaterialToRenderers(rear, trainKitMat);
                AddBoxColliderToHierarchy(rear);
            }

            GameObject carWaypointsRoot = new GameObject("--- CAR WAYPOINTS ---");

            Transform[] rMainNB = CreateLoopWaypoints("CarRoute_MainAve_NB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(1.15f, 0.05f, -72f),
                new Vector3(1.15f, 0.05f, -48f),
                new Vector3(1.15f, 0.05f, -20.5f),
                new Vector3(1.15f, 0.05f, -11.5f),
                new Vector3(1.15f, 0.05f, 11.5f),
                new Vector3(1.15f, 0.05f, 20.5f),
                new Vector3(1.15f, 0.05f, 48f),
                new Vector3(1.15f, 0.05f, 72f)
            });

            Transform[] rMainSB = CreateLoopWaypoints("CarRoute_MainAve_SB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(-1.15f, 0.05f, 72f),
                new Vector3(-1.15f, 0.05f, 48f),
                new Vector3(-1.15f, 0.05f, 20.5f),
                new Vector3(-1.15f, 0.05f, 11.5f),
                new Vector3(-1.15f, 0.05f, -11.5f),
                new Vector3(-1.15f, 0.05f, -20.5f),
                new Vector3(-1.15f, 0.05f, -48f),
                new Vector3(-1.15f, 0.05f, -72f)
            });

            Transform[] rWestNB = CreateLoopWaypoints("CarRoute_WestAve_NB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(-20.85f, 0.05f, -72f),
                new Vector3(-20.85f, 0.05f, -48f),
                new Vector3(-20.85f, 0.05f, -20.5f),
                new Vector3(-20.85f, 0.05f, -11.5f),
                new Vector3(-20.85f, 0.05f, 11.5f),
                new Vector3(-20.85f, 0.05f, 20.5f),
                new Vector3(-20.85f, 0.05f, 48f),
                new Vector3(-20.85f, 0.05f, 72f)
            });

            Transform[] rWestSB = CreateLoopWaypoints("CarRoute_WestAve_SB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(-23.15f, 0.05f, 72f),
                new Vector3(-23.15f, 0.05f, 48f),
                new Vector3(-23.15f, 0.05f, 20.5f),
                new Vector3(-23.15f, 0.05f, 11.5f),
                new Vector3(-23.15f, 0.05f, -11.5f),
                new Vector3(-23.15f, 0.05f, -20.5f),
                new Vector3(-23.15f, 0.05f, -48f),
                new Vector3(-23.15f, 0.05f, -72f)
            });

            Transform[] rEastNB = CreateLoopWaypoints("CarRoute_EastAve_NB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(23.15f, 0.05f, -72f),
                new Vector3(23.15f, 0.05f, -48f),
                new Vector3(23.15f, 0.05f, -20.5f),
                new Vector3(23.15f, 0.05f, -11.5f),
                new Vector3(23.15f, 0.05f, 11.5f),
                new Vector3(23.15f, 0.05f, 20.5f),
                new Vector3(23.15f, 0.05f, 48f),
                new Vector3(23.15f, 0.05f, 72f)
            });

            Transform[] rEastSB = CreateLoopWaypoints("CarRoute_EastAve_SB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(20.85f, 0.05f, 72f),
                new Vector3(20.85f, 0.05f, 48f),
                new Vector3(20.85f, 0.05f, 20.5f),
                new Vector3(20.85f, 0.05f, 11.5f),
                new Vector3(20.85f, 0.05f, -11.5f),
                new Vector3(20.85f, 0.05f, -20.5f),
                new Vector3(20.85f, 0.05f, -48f),
                new Vector3(20.85f, 0.05f, -72f)
            });

            Transform[] rCrossSEB = CreateLoopWaypoints("CarRoute_CrossSouth_EB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(-34f, 0.05f, -17.15f),
                new Vector3(-24f, 0.05f, -17.15f),
                new Vector3(-20f, 0.05f, -17.15f),
                new Vector3(-2f, 0.05f, -17.15f),
                new Vector3(2f, 0.05f, -17.15f),
                new Vector3(20f, 0.05f, -17.15f),
                new Vector3(24f, 0.05f, -17.15f),
                new Vector3(30f, 0.05f, -17.15f)
            });

            Transform[] rCrossSWB = CreateLoopWaypoints("CarRoute_CrossSouth_WB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(30f, 0.05f, -14.85f),
                new Vector3(24f, 0.05f, -14.85f),
                new Vector3(20f, 0.05f, -14.85f),
                new Vector3(2f, 0.05f, -14.85f),
                new Vector3(-2f, 0.05f, -14.85f),
                new Vector3(-20f, 0.05f, -14.85f),
                new Vector3(-24f, 0.05f, -14.85f),
                new Vector3(-34f, 0.05f, -14.85f)
            });

            Transform[] rCrossNEB = CreateLoopWaypoints("CarRoute_CrossNorth_EB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(-34f, 0.05f, 14.85f),
                new Vector3(-24f, 0.05f, 14.85f),
                new Vector3(-20f, 0.05f, 14.85f),
                new Vector3(-2f, 0.05f, 14.85f),
                new Vector3(2f, 0.05f, 14.85f),
                new Vector3(20f, 0.05f, 14.85f),
                new Vector3(24f, 0.05f, 14.85f),
                new Vector3(30f, 0.05f, 14.85f)
            });

            Transform[] rCrossNWB = CreateLoopWaypoints("CarRoute_CrossNorth_WB", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(30f, 0.05f, 17.15f),
                new Vector3(24f, 0.05f, 17.15f),
                new Vector3(20f, 0.05f, 17.15f),
                new Vector3(2f, 0.05f, 17.15f),
                new Vector3(-2f, 0.05f, 17.15f),
                new Vector3(-20f, 0.05f, 17.15f),
                new Vector3(-24f, 0.05f, 17.15f),
                new Vector3(-34f, 0.05f, 17.15f)
            });

            Transform[] rTurn1 = CreateLoopWaypoints("CarRoute_Turn_SouthMain_to_CrossEast", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(1.15f, 0.05f, -72f),
                new Vector3(1.15f, 0.05f, -48f),
                new Vector3(1.15f, 0.05f, -20.5f),
                new Vector3(1.6f, 0.05f, -17.15f),
                new Vector3(20f, 0.05f, -17.15f),
                new Vector3(24f, 0.05f, -17.15f),
                new Vector3(30f, 0.05f, -17.15f)
            });

            Transform[] rTurn2 = CreateLoopWaypoints("CarRoute_Turn_NorthMain_to_CrossWest", carWaypointsRoot.transform, new Vector3[] {
                new Vector3(-1.15f, 0.05f, 72f),
                new Vector3(-1.15f, 0.05f, 48f),
                new Vector3(-1.15f, 0.05f, 20.5f),
                new Vector3(-1.6f, 0.05f, 17.15f),
                new Vector3(-20f, 0.05f, 17.15f),
                new Vector3(-24f, 0.05f, 17.15f),
                new Vector3(-34f, 0.05f, 17.15f)
            });

            GameObject trafficRoot = new GameObject("--- TRAFFIC SYSTEM ---");
            TrafficManager trafficMgr = trafficRoot.AddComponent<TrafficManager>();
            trafficMgr.maxActiveCars = 10;
            trafficMgr.spawnInterval = 3.5f;

            string[] carModels = new string[]
            {
                "Assets/CityAssets/Vehicles/Models/FBX format/police.fbx",
                "Assets/CityAssets/Vehicles/Models/FBX format/ambulance.fbx",
                "Assets/CityAssets/Vehicles/Models/FBX format/firetruck.fbx",
                "Assets/CityAssets/Vehicles/Models/FBX format/taxi.fbx",
                "Assets/CityAssets/Vehicles/Models/FBX format/delivery.fbx",
                "Assets/CityAssets/Vehicles/Models/FBX format/garbage-truck.fbx",
                "Assets/CityAssets/Vehicles/Models/FBX format/suv-luxury.fbx",
                "Assets/CityAssets/Vehicles/Models/FBX format/sedan-sports.fbx"
            };

            Material vehMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Vehicles/Mat_Vehicles.mat");
            trafficMgr.carMaterial = vehMat;

            List<GameObject> loadedCarPrefabs = new List<GameObject>();
            for (int ci = 0; ci < carModels.Length; ci++)
            {
                GameObject cp = AssetDatabase.LoadAssetAtPath<GameObject>(carModels[ci]);
                if (cp != null) loadedCarPrefabs.Add(cp);
            }
            trafficMgr.carPrefabs = loadedCarPrefabs.ToArray();

            trafficMgr.routes = new List<CarRoute>
            {
                new CarRoute(rMainNB),
                new CarRoute(rMainSB),
                new CarRoute(rWestNB),
                new CarRoute(rWestSB),
                new CarRoute(rEastNB),
                new CarRoute(rEastSB),
                new CarRoute(rCrossSEB),
                new CarRoute(rCrossSWB),
                new CarRoute(rCrossNEB),
                new CarRoute(rCrossNWB),
                new CarRoute(rTurn1),
                new CarRoute(rTurn2)
            };

            GameObject vehRoot = new GameObject("--- INITIAL VEHICLES ---");
            vehRoot.transform.SetParent(trafficRoot.transform);

            Transform[][] initRoutes = new Transform[][] { rMainNB, rMainSB, rWestNB, rEastSB, rCrossSEB, rCrossNWB };
            int[] initStartIdx = new int[] { 2, 2, 3, 3, 2, 3 };
            float[] initSpeeds = new float[] { 5.2f, 5.0f, 4.8f, 5.2f, 4.9f, 5.1f };

            for (int ii = 0; ii < initRoutes.Length; ii++)
            {
                if (ii >= loadedCarPrefabs.Count) break;
                GameObject cPrefab = loadedCarPrefabs[ii];
                Transform[] route = initRoutes[ii];
                int wpStartIdx = initStartIdx[ii];
                Vector3 sPos = route[wpStartIdx].position;
                Vector3 nPos = route[Mathf.Min(wpStartIdx + 1, route.Length - 1)].position;
                Vector3 dir = (nPos - sPos).normalized;
                Quaternion rot = dir != Vector3.zero ? Quaternion.LookRotation(dir) : Quaternion.identity;

                GameObject carObj = (GameObject)PrefabUtility.InstantiatePrefab(cPrefab);
                carObj.name = "InitCar_" + cPrefab.name;
                carObj.transform.SetParent(vehRoot.transform);
                carObj.transform.position = sPos;
                carObj.transform.rotation = rot;
                carObj.transform.localScale = new Vector3(0.70f, 0.70f, 0.70f);
                CleanImportedObject(carObj);
                if (vehMat != null) ApplyMaterialToRenderers(carObj, vehMat);

                AddCapsuleCollider(carObj, 0.55f, 1.9f);

                Rigidbody rb = carObj.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;

                CarAgent agent = carObj.AddComponent<CarAgent>();
                agent.waypoints = route;
                agent.speed = initSpeeds[ii];
                agent.currentWaypointIndex = wpStartIdx + 1;
                agent.loopWaypoints = false;
            }

            GameObject pedWaypointsRoot = new GameObject("--- PEDESTRIAN WAYPOINTS (SIDEWALKS & CROSSWALKS) ---");

            Transform[] p1Waypoints = CreateLoopWaypoints("PedRoute_Sidewalk_Central", pedWaypointsRoot.transform, new Vector3[] {
                new Vector3(-3.8f, 0.16f, -13.5f),
                new Vector3(-3.8f, 0.16f, 13.5f),
                new Vector3(-1.8f, 0.035f, 13.5f),
                new Vector3(0.0f, 0.035f, 13.5f),
                new Vector3(1.8f, 0.035f, 13.5f),
                new Vector3(3.8f, 0.16f, 13.5f),
                new Vector3(3.8f, 0.16f, -13.5f),
                new Vector3(1.8f, 0.035f, -13.5f),
                new Vector3(0.0f, 0.035f, -13.5f),
                new Vector3(-1.8f, 0.035f, -13.5f)
            });

            Transform[] p2Waypoints = CreateLoopWaypoints("PedRoute_Sidewalk_WestAvenue", pedWaypointsRoot.transform, new Vector3[] {
                new Vector3(-18.2f, 0.16f, -13.5f),
                new Vector3(-18.2f, 0.16f, 13.5f),
                new Vector3(-20.1f, 0.035f, 13.5f),
                new Vector3(-22.0f, 0.035f, 13.5f),
                new Vector3(-23.9f, 0.035f, 13.5f),
                new Vector3(-25.8f, 0.16f, 13.5f),
                new Vector3(-25.8f, 0.16f, -13.5f),
                new Vector3(-23.9f, 0.035f, -13.5f),
                new Vector3(-22.0f, 0.035f, -13.5f),
                new Vector3(-20.1f, 0.035f, -13.5f)
            });

            Transform[] p3Waypoints = CreateLoopWaypoints("PedRoute_Sidewalk_EastAvenue", pedWaypointsRoot.transform, new Vector3[] {
                new Vector3(18.2f, 0.16f, -13.5f),
                new Vector3(18.2f, 0.16f, 13.5f),
                new Vector3(20.1f, 0.035f, 13.5f),
                new Vector3(22.0f, 0.035f, 13.5f),
                new Vector3(23.9f, 0.035f, 13.5f),
                new Vector3(25.8f, 0.16f, 13.5f),
                new Vector3(25.8f, 0.16f, -13.5f),
                new Vector3(23.9f, 0.035f, -13.5f),
                new Vector3(22.0f, 0.035f, -13.5f),
                new Vector3(20.1f, 0.035f, -13.5f)
            });

            GameObject charsRoot = new GameObject("--- CHARACTERS (MEMES) ---");

            Material shrekHeadMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/Shrek/Mat_Shrek_Head.mat");
            Material shrekBodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/Shrek/Mat_Shrek_Body.mat");
            Material shrekEyesMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/Shrek/Mat_Shrek_Eyes.mat");
            RuntimeAnimatorController shrekAnim = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/CityAssets/Characters/Shrek/Shrek_Controller.controller");

            GameObject shrekPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Characters/Shrek/source/Walking.fbx");
            if (shrekPrefab != null)
            {
                GameObject shrekRoot = new GameObject("NPC_Shrek");
                shrekRoot.transform.SetParent(charsRoot.transform);
                shrekRoot.transform.position = p1Waypoints[0].position;

                GameObject shrekModel = (GameObject)PrefabUtility.InstantiatePrefab(shrekPrefab);
                shrekModel.name = "Shrek_Model";
                shrekModel.transform.SetParent(shrekRoot.transform);
                shrekModel.transform.localPosition = new Vector3(0f, 0.095f, 0f);
                shrekModel.transform.localRotation = Quaternion.identity;
                shrekModel.transform.localScale = new Vector3(0.014f, 0.014f, 0.014f);
                CleanImportedObject(shrekModel);

                Transform bodyObj = shrekModel.transform.Find("Body");
                if (bodyObj != null)
                {
                    SkinnedMeshRenderer smr = bodyObj.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null && shrekBodyMat != null)
                    {
                        smr.sharedMaterials = new Material[] { shrekBodyMat, shrekBodyMat };
                    }
                }

                Transform headObj = shrekModel.transform.Find("head");
                if (headObj != null)
                {
                    SkinnedMeshRenderer smr = headObj.GetComponent<SkinnedMeshRenderer>();
                    if (smr != null)
                    {
                        smr.sharedMaterials = new Material[]
                        {
                            shrekEyesMat != null ? shrekEyesMat : shrekBodyMat,
                            shrekEyesMat != null ? shrekEyesMat : shrekBodyMat,
                            shrekBodyMat != null ? shrekBodyMat : shrekHeadMat,
                            shrekHeadMat != null ? shrekHeadMat : shrekBodyMat
                        };
                    }
                }

                Animator anim = shrekModel.GetComponent<Animator>();
                if (anim == null) anim = shrekModel.AddComponent<Animator>();
                anim.runtimeAnimatorController = shrekAnim;
                anim.applyRootMotion = false;

                AddCapsuleCollider(shrekRoot, 0.35f, 1.26f);
                PedestrianAgent pAgent = shrekRoot.AddComponent<PedestrianAgent>();
                pAgent.waypoints = p1Waypoints;
                pAgent.speed = 2.2f;
                pAgent.SetInitialWaypointIndex(1);
            }

            Material steveMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/Steve/Mat_Steve.mat");
            RuntimeAnimatorController steveAnim = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/CityAssets/Characters/Steve/Steve_Controller.controller");
            GameObject stevePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Characters/Steve/source/The Perfect Steve Rigged/Steve/model/FBX/Steve Rigged.fbx");
            if (stevePrefab != null)
            {
                GameObject steveRoot = new GameObject("NPC_Steve");
                steveRoot.transform.SetParent(charsRoot.transform);
                steveRoot.transform.position = p2Waypoints[0].position;

                GameObject steveModel = (GameObject)PrefabUtility.InstantiatePrefab(stevePrefab);
                steveModel.name = "Steve_Model";
                steveModel.transform.SetParent(steveRoot.transform);
                steveModel.transform.localPosition = new Vector3(0f, 0.54f, 0f);
                steveModel.transform.localRotation = Quaternion.identity;
                steveModel.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
                CleanImportedObject(steveModel);

                Renderer[] rends = steveModel.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rends)
                {
                    if (steveMat != null) r.sharedMaterial = steveMat;
                }

                Animator anim = steveModel.GetComponent<Animator>();
                if (anim == null) anim = steveModel.AddComponent<Animator>();
                anim.runtimeAnimatorController = steveAnim;
                anim.applyRootMotion = false;

                AddCapsuleCollider(steveRoot, 0.3f, 1.08f);
                PedestrianAgent sAgent = steveRoot.AddComponent<PedestrianAgent>();
                sAgent.waypoints = p2Waypoints;
                sAgent.speed = 2.0f;
                sAgent.SetInitialWaypointIndex(1);
            }

            RuntimeAnimatorController bearAnim = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/CityAssets/Characters/DropoutBear/Bear_Controller.controller");
            GameObject bearPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Characters/DropoutBear/source/TESTBear.fbx");
            if (bearPrefab != null)
            {
                GameObject bearRoot = new GameObject("NPC_DropoutBear");
                bearRoot.transform.SetParent(charsRoot.transform);
                bearRoot.transform.position = p3Waypoints[0].position;

                GameObject bearModel = (GameObject)PrefabUtility.InstantiatePrefab(bearPrefab);
                bearModel.name = "Bear_Model";
                bearModel.transform.SetParent(bearRoot.transform);
                bearModel.transform.localPosition = Vector3.zero;
                bearModel.transform.localRotation = Quaternion.identity;
                bearModel.transform.localScale = new Vector3(0.72f, 0.72f, 0.72f);
                CleanImportedObject(bearModel);

                Animator anim = bearModel.GetComponent<Animator>();
                if (anim == null) anim = bearModel.AddComponent<Animator>();
                anim.runtimeAnimatorController = bearAnim;
                anim.applyRootMotion = false;

                AddCapsuleCollider(bearRoot, 0.32f, 1.1f);
                PedestrianAgent bAgent = bearRoot.AddComponent<PedestrianAgent>();
                bAgent.waypoints = p3Waypoints;
                bAgent.speed = 2.0f;
                bAgent.SetInitialWaypointIndex(1);
            }

            Material capyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/Capybara/Mat_Capybara.mat");
            GameObject capyPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Characters/Capybara/source/capybara.obj");
            if (capyPrefab != null)
            {
                GameObject capyRoot = new GameObject("NPC_Capybara");
                capyRoot.transform.SetParent(charsRoot.transform);
                capyRoot.transform.position = p2Waypoints[5].position;

                GameObject capyModel = (GameObject)PrefabUtility.InstantiatePrefab(capyPrefab);
                capyModel.name = "Capybara_Model";
                capyModel.transform.SetParent(capyRoot.transform);
                capyModel.transform.localPosition = Vector3.zero;
                capyModel.transform.localRotation = Quaternion.identity;
                capyModel.transform.localScale = new Vector3(0.09f, 0.09f, 0.09f);
                CleanImportedObject(capyModel);

                Renderer[] rends = capyModel.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rends)
                {
                    if (capyMat != null) r.sharedMaterial = capyMat;
                }

                AddCapsuleCollider(capyRoot, 0.25f, 0.55f);
                PedestrianAgent cAgent = capyRoot.AddComponent<PedestrianAgent>();
                cAgent.waypoints = p2Waypoints;
                cAgent.speed = 1.6f;
                cAgent.SetInitialWaypointIndex(6);
            }

            Material amongBodyMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/AmongUs/Mat_AmongUs_Body.mat");
            Material amongVisorMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/AmongUs/Mat_AmongUs_Visor.mat");
            GameObject amongPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Characters/AmongUs/among_us_crewmate.obj");
            if (amongPrefab != null)
            {
                GameObject amongRoot = new GameObject("NPC_AmongUs");
                amongRoot.transform.SetParent(charsRoot.transform);
                amongRoot.transform.position = p1Waypoints[5].position;

                GameObject amongModel = (GameObject)PrefabUtility.InstantiatePrefab(amongPrefab);
                amongModel.name = "AmongUs_Model";
                amongModel.transform.SetParent(amongRoot.transform);
                amongModel.transform.localPosition = new Vector3(0f, 0.666f, 0f);
                amongModel.transform.localRotation = Quaternion.identity;
                amongModel.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                CleanImportedObject(amongModel);

                Renderer[] rends = amongModel.GetComponentsInChildren<Renderer>(true);
                for (int ri = 0; ri < rends.Length; ri++)
                {
                    if (ri == 0 && amongBodyMat != null) rends[ri].sharedMaterial = amongBodyMat;
                    else if (amongVisorMat != null) rends[ri].sharedMaterial = amongVisorMat;
                }

                AddCapsuleCollider(amongRoot, 0.32f, 1.05f);
                PedestrianAgent aAgent = amongRoot.AddComponent<PedestrianAgent>();
                aAgent.waypoints = p1Waypoints;
                aAgent.speed = 2.1f;
                aAgent.SetInitialWaypointIndex(6);
            }

            Material tungMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/CityAssets/Characters/TungTung/Mat_TungTung.mat");
            RuntimeAnimatorController tungAnim = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/CityAssets/Characters/TungTung/TungTung_Controller.controller");
            GameObject tungPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Characters/TungTung/source/ZAHUR.fbx");
            if (tungPrefab != null)
            {
                GameObject tungRoot = new GameObject("NPC_TungTung");
                tungRoot.transform.SetParent(charsRoot.transform);
                tungRoot.transform.position = p3Waypoints[5].position;

                GameObject tungModel = (GameObject)PrefabUtility.InstantiatePrefab(tungPrefab);
                tungModel.name = "TungTung_Model";
                tungModel.transform.SetParent(tungRoot.transform);
                tungModel.transform.localPosition = new Vector3(0f, 0.119f, 0f);
                tungModel.transform.localRotation = Quaternion.identity;
                tungModel.transform.localScale = new Vector3(0.50f, 0.50f, 0.50f);
                CleanImportedObject(tungModel);

                Renderer[] rends = tungModel.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rends)
                {
                    if (tungMat != null) r.sharedMaterial = tungMat;
                }

                Animator anim = tungModel.GetComponent<Animator>();
                if (anim == null) anim = tungModel.AddComponent<Animator>();
                anim.runtimeAnimatorController = tungAnim;
                anim.applyRootMotion = false;

                AddCapsuleCollider(tungRoot, 0.3f, 1.1f);
                PedestrianAgent tAgent = tungRoot.AddComponent<PedestrianAgent>();
                tAgent.waypoints = p3Waypoints;
                tAgent.speed = 1.9f;
                tAgent.SetInitialWaypointIndex(6);
            }

            GameObject camerasRoot = new GameObject("--- CAMERAS & PLAYER ---");

            GameObject playerObj = new GameObject("Player");
            playerObj.transform.SetParent(camerasRoot.transform);
            playerObj.transform.position = new Vector3(-19.5f, 0.65f, -24f);

            CharacterController charCtrl = playerObj.AddComponent<CharacterController>();
            charCtrl.height = 1.10f;
            charCtrl.radius = 0.26f;
            charCtrl.center = new Vector3(0f, 0.55f, 0f);
            charCtrl.stepOffset = 0.25f;

            PlayerController playerCtrl = playerObj.AddComponent<PlayerController>();

            GameObject fpsCamObj = new GameObject("FPS_Camera");
            fpsCamObj.transform.SetParent(playerObj.transform);
            fpsCamObj.transform.localPosition = new Vector3(0f, 0.98f, 0f);
            fpsCamObj.transform.localRotation = Quaternion.identity;
            Camera fpsCam = fpsCamObj.AddComponent<Camera>();
            fpsCam.nearClipPlane = 0.1f;
            fpsCam.farClipPlane = 500f;
            fpsCamObj.AddComponent<AudioListener>();

            GameObject streetCamObj = new GameObject("Street_Camera");
            streetCamObj.transform.SetParent(camerasRoot.transform);
            streetCamObj.transform.position = new Vector3(-5.5f, 3.2f, -25.0f);
            streetCamObj.transform.rotation = Quaternion.Euler(10f, 25f, 0f);
            Camera streetCam = streetCamObj.AddComponent<Camera>();
            streetCam.nearClipPlane = 0.1f;
            streetCam.farClipPlane = 500f;
            streetCamObj.AddComponent<AudioListener>();

            GameObject orbitCamObj = new GameObject("Overview_Camera");
            orbitCamObj.transform.SetParent(camerasRoot.transform);
            orbitCamObj.transform.position = new Vector3(0f, 65f, -80f);
            orbitCamObj.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
            Camera orbitCam = orbitCamObj.AddComponent<Camera>();
            orbitCam.nearClipPlane = 0.1f;
            orbitCam.farClipPlane = 500f;
            orbitCamObj.AddComponent<AudioListener>();

            GameObject stationCamObj = new GameObject("Station_Camera");
            stationCamObj.transform.SetParent(camerasRoot.transform);
            stationCamObj.transform.position = new Vector3(45f, 8f, -22f);
            stationCamObj.transform.rotation = Quaternion.Euler(14f, -48f, 0f);
            Camera stationCam = stationCamObj.AddComponent<Camera>();
            stationCam.nearClipPlane = 0.1f;
            stationCam.farClipPlane = 500f;
            stationCamObj.AddComponent<AudioListener>();

            CameraController camManager = camerasRoot.AddComponent<CameraController>();
            camManager.cameras = new Camera[] { fpsCam, streetCam, orbitCam, stationCam };
            camManager.cameraNames = new string[] { "1-е лицо", "Перекресток", "Обзор города", "Вокзал" };
            camManager.activeIndex = 1;
            camManager.UpdateActiveCamera();

            EditorSceneManager.SaveScene(scene, "Assets/Scenes/task4_5.unity");
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/task6_7.unity");
            Debug.Log("Full City Scene with Kenney Expansion Built and Saved Successfully!");
        }

        private static Transform[] CreateLoopWaypoints(string parentName, Transform parent, Vector3[] positions)
        {
            GameObject root = new GameObject(parentName);
            root.transform.SetParent(parent);
            Transform[] list = new Transform[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                GameObject wp = new GameObject("WP_" + i);
                wp.transform.SetParent(root.transform);
                wp.transform.position = positions[i];
                list[i] = wp.transform;
            }
            return list;
        }

        private static void SpawnSidewalkSegment(Vector3 pos, Vector3 scale, Material mat, Transform parent)
        {
            GameObject sw = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sw.name = "Sidewalk";
            sw.transform.SetParent(parent);
            sw.transform.position = pos;
            sw.transform.localScale = scale;
            if (mat != null) sw.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void SpawnLamp(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent, Material mat = null)
        {
            GameObject lamp = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            lamp.transform.SetParent(parent);
            lamp.transform.position = pos;
            lamp.transform.rotation = rot;
            lamp.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            CleanImportedObject(lamp);
            if (mat != null) ApplyMaterialToRenderers(lamp, mat);

            GameObject ptLightObj = new GameObject("LampLight");
            ptLightObj.transform.SetParent(lamp.transform);
            ptLightObj.transform.localPosition = new Vector3(0f, 1.6f, 0.4f);
            Light l = ptLightObj.AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(1f, 0.92f, 0.75f);
            l.range = 8.5f;
            l.intensity = 1.2f;
            l.shadows = LightShadows.None;
        }

        private static void SpawnTree(Vector3 pos, Material fol1, Material fol2, Material trunkMat, Transform parent)
        {
            GameObject tree = new GameObject("Tree_Foliage");
            tree.transform.SetParent(parent);
            tree.transform.position = pos;

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform);
            trunk.transform.localPosition = new Vector3(0f, 1.25f, 0f);
            trunk.transform.localScale = new Vector3(0.45f, 1.25f, 0.45f);
            if (trunkMat != null) trunk.GetComponent<Renderer>().sharedMaterial = trunkMat;

            GameObject crown1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crown1.name = "Crown1";
            crown1.transform.SetParent(tree.transform);
            crown1.transform.localPosition = new Vector3(0f, 2.9f, 0f);
            crown1.transform.localScale = new Vector3(2.4f, 2.1f, 2.4f);
            if (fol1 != null) crown1.GetComponent<Renderer>().sharedMaterial = fol1;

            GameObject crown2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crown2.name = "Crown2";
            crown2.transform.SetParent(tree.transform);
            crown2.transform.localPosition = new Vector3(0f, 3.8f, 0f);
            crown2.transform.localScale = new Vector3(1.7f, 1.6f, 1.7f);
            if (fol2 != null) crown2.GetComponent<Renderer>().sharedMaterial = fol2;

            AddCapsuleCollider(tree, 0.5f, 4.5f);
        }

        private static void SpawnBush(Vector3 pos, Vector3 scale, Material folMat, Transform parent)
        {
            GameObject bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bush.name = "Bush";
            bush.transform.SetParent(parent);
            bush.transform.position = pos + new Vector3(0f, scale.y * 0.4f, 0f);
            bush.transform.localScale = scale;
            if (folMat != null) bush.GetComponent<Renderer>().sharedMaterial = folMat;
        }

        private static void SpawnZebraCrosswalk(Vector3 center, bool isAcrossAvenue, Material mat, Transform parent)
        {
            GameObject zebra = new GameObject("ZebraCrosswalk");
            zebra.transform.SetParent(parent);
            zebra.transform.position = new Vector3(center.x, 0f, center.z);

            float[] offsets = new float[] { -1.4f, -0.7f, 0f, 0.7f, 1.4f };
            foreach (float off in offsets)
            {
                GameObject bar = GameObject.CreatePrimitive(PrimitiveType.Cube);
                bar.name = "Stripe";
                bar.transform.SetParent(zebra.transform);
                Object.DestroyImmediate(bar.GetComponent<Collider>());
                if (mat != null) bar.GetComponent<Renderer>().sharedMaterial = mat;

                if (isAcrossAvenue)
                {
                    bar.transform.localPosition = new Vector3(off, 0.025f, 0f);
                    bar.transform.localScale = new Vector3(0.42f, 0.02f, 2.2f);
                }
                else
                {
                    bar.transform.localPosition = new Vector3(0f, 0.025f, off);
                    bar.transform.localScale = new Vector3(2.2f, 0.02f, 0.42f);
                }
            }
        }

        private static void SpawnDashedCenterLine(Vector3 start, Vector3 end, Material mat, Transform parent)
        {
            float dashLen = 1.4f;
            float gapLen = 1.6f;
            float step = dashLen + gapLen;
            Vector3 delta = end - start;
            float totalDist = delta.magnitude;
            Vector3 dir = delta.normalized;
            int count = Mathf.FloorToInt(totalDist / step);
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = start + dir * (i * step + dashLen * 0.5f);
                GameObject dash = GameObject.CreatePrimitive(PrimitiveType.Cube);
                dash.name = "DashedLine_1.5";
                dash.transform.SetParent(parent);
                dash.transform.position = new Vector3(pos.x, 0.025f, pos.z);
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
                {
                    dash.transform.localScale = new Vector3(dashLen, 0.02f, 0.16f);
                }
                else
                {
                    dash.transform.localScale = new Vector3(0.16f, 0.02f, dashLen);
                }
                Object.DestroyImmediate(dash.GetComponent<Collider>());
                if (mat != null) dash.GetComponent<Renderer>().sharedMaterial = mat;
            }
        }

        private static void SpawnStopLine(Vector3 pos, bool isAcrossAvenue, Material mat, Transform parent)
        {
            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "StopLine_1.12";
            line.transform.SetParent(parent);
            line.transform.position = new Vector3(pos.x, 0.025f, pos.z);
            if (isAcrossAvenue)
            {
                line.transform.localScale = new Vector3(2.0f, 0.02f, 0.4f);
            }
            else
            {
                line.transform.localScale = new Vector3(0.4f, 0.02f, 2.0f);
            }
            Object.DestroyImmediate(line.GetComponent<Collider>());
            if (mat != null) line.GetComponent<Renderer>().sharedMaterial = mat;
        }

        private static void SpawnTrafficLightColumn(Vector3 pos, Quaternion rot, TrafficLightController.LightState initState, float startOffset, Vector3 stopColPos, Vector3 stopColSize, Material poleMat, Material housingMat, Material zebraMat, Material rOn, Material rOff, Material yOn, Material yOff, Material gOn, Material gOff, Transform parent)
        {
            GameObject tl = new GameObject("TrafficLight_Column");
            tl.transform.SetParent(parent);
            tl.transform.position = pos;
            tl.transform.rotation = rot;

            GameObject baseObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            baseObj.name = "Column_Base";
            baseObj.transform.SetParent(tl.transform);
            baseObj.transform.localPosition = new Vector3(0f, 0.12f, 0f);
            baseObj.transform.localScale = new Vector3(0.38f, 0.12f, 0.38f);
            Object.DestroyImmediate(baseObj.GetComponent<Collider>());
            if (poleMat != null) baseObj.GetComponent<Renderer>().sharedMaterial = poleMat;

            GameObject mastObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mastObj.name = "Column_Mast";
            mastObj.transform.SetParent(tl.transform);
            mastObj.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            mastObj.transform.localScale = new Vector3(0.16f, 1.6f, 0.16f);
            Object.DestroyImmediate(mastObj.GetComponent<Collider>());
            if (poleMat != null) mastObj.GetComponent<Renderer>().sharedMaterial = poleMat;

            GameObject borderObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            borderObj.name = "Backplate_Border";
            borderObj.transform.SetParent(tl.transform);
            borderObj.transform.localPosition = new Vector3(0f, 2.7f, 0.08f);
            borderObj.transform.localScale = new Vector3(0.60f, 1.34f, 0.02f);
            Object.DestroyImmediate(borderObj.GetComponent<Collider>());
            if (zebraMat != null) borderObj.GetComponent<Renderer>().sharedMaterial = zebraMat;

            GameObject backplate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backplate.name = "Backplate_Screen";
            backplate.transform.SetParent(tl.transform);
            backplate.transform.localPosition = new Vector3(0f, 2.7f, 0.09f);
            backplate.transform.localScale = new Vector3(0.54f, 1.28f, 0.02f);
            Object.DestroyImmediate(backplate.GetComponent<Collider>());
            if (housingMat != null) backplate.GetComponent<Renderer>().sharedMaterial = housingMat;

            GameObject housing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            housing.name = "Housing";
            housing.transform.SetParent(tl.transform);
            housing.transform.localPosition = new Vector3(0f, 2.7f, 0.20f);
            housing.transform.localScale = new Vector3(0.38f, 1.20f, 0.20f);
            Object.DestroyImmediate(housing.GetComponent<Collider>());
            if (housingMat != null) housing.GetComponent<Renderer>().sharedMaterial = housingMat;

            GameObject rVisor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rVisor.name = "Visor_Red";
            rVisor.transform.SetParent(tl.transform);
            rVisor.transform.localPosition = new Vector3(0f, 3.16f, 0.32f);
            rVisor.transform.localRotation = Quaternion.Euler(-18f, 0f, 0f);
            rVisor.transform.localScale = new Vector3(0.28f, 0.04f, 0.16f);
            Object.DestroyImmediate(rVisor.GetComponent<Collider>());
            if (housingMat != null) rVisor.GetComponent<Renderer>().sharedMaterial = housingMat;

            GameObject yVisor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yVisor.name = "Visor_Yellow";
            yVisor.transform.SetParent(tl.transform);
            yVisor.transform.localPosition = new Vector3(0f, 2.76f, 0.32f);
            yVisor.transform.localRotation = Quaternion.Euler(-18f, 0f, 0f);
            yVisor.transform.localScale = new Vector3(0.28f, 0.04f, 0.16f);
            Object.DestroyImmediate(yVisor.GetComponent<Collider>());
            if (housingMat != null) yVisor.GetComponent<Renderer>().sharedMaterial = housingMat;

            GameObject gVisor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gVisor.name = "Visor_Green";
            gVisor.transform.SetParent(tl.transform);
            gVisor.transform.localPosition = new Vector3(0f, 2.36f, 0.32f);
            gVisor.transform.localRotation = Quaternion.Euler(-18f, 0f, 0f);
            gVisor.transform.localScale = new Vector3(0.28f, 0.04f, 0.16f);
            Object.DestroyImmediate(gVisor.GetComponent<Collider>());
            if (housingMat != null) gVisor.GetComponent<Renderer>().sharedMaterial = housingMat;

            GameObject rLens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            rLens.name = "Lens_Red";
            rLens.transform.SetParent(tl.transform);
            rLens.transform.localPosition = new Vector3(0f, 3.06f, 0.30f);
            rLens.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            rLens.transform.localScale = new Vector3(0.22f, 0.04f, 0.22f);
            Object.DestroyImmediate(rLens.GetComponent<Collider>());

            GameObject yLens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            yLens.name = "Lens_Yellow";
            yLens.transform.SetParent(tl.transform);
            yLens.transform.localPosition = new Vector3(0f, 2.66f, 0.30f);
            yLens.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            yLens.transform.localScale = new Vector3(0.22f, 0.04f, 0.22f);
            Object.DestroyImmediate(yLens.GetComponent<Collider>());

            GameObject gLens = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gLens.name = "Lens_Green";
            gLens.transform.SetParent(tl.transform);
            gLens.transform.localPosition = new Vector3(0f, 2.26f, 0.30f);
            gLens.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            gLens.transform.localScale = new Vector3(0.22f, 0.04f, 0.22f);
            Object.DestroyImmediate(gLens.GetComponent<Collider>());

            GameObject pedHead = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedHead.name = "Pedestrian_Head";
            pedHead.transform.SetParent(tl.transform);
            pedHead.transform.localPosition = new Vector3(0f, 1.45f, 0.16f);
            pedHead.transform.localScale = new Vector3(0.24f, 0.46f, 0.16f);
            Object.DestroyImmediate(pedHead.GetComponent<Collider>());
            if (housingMat != null) pedHead.GetComponent<Renderer>().sharedMaterial = housingMat;

            GameObject pedBtn = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedBtn.name = "Pedestrian_Button";
            pedBtn.transform.SetParent(tl.transform);
            pedBtn.transform.localPosition = new Vector3(0f, 0.95f, 0.12f);
            pedBtn.transform.localScale = new Vector3(0.12f, 0.18f, 0.08f);
            Object.DestroyImmediate(pedBtn.GetComponent<Collider>());
            if (housingMat != null) pedBtn.GetComponent<Renderer>().sharedMaterial = housingMat;

            GameObject stopObj = new GameObject("StopZone");
            stopObj.transform.SetParent(tl.transform);
            stopObj.transform.position = stopColPos;
            BoxCollider stopCol = stopObj.AddComponent<BoxCollider>();
            stopCol.size = new Vector3(Mathf.Abs(stopColSize.x), Mathf.Abs(stopColSize.y), Mathf.Abs(stopColSize.z));
            stopCol.isTrigger = false;

            TrafficLightController ctrl = tl.AddComponent<TrafficLightController>();
            ctrl.redRenderer = rLens.GetComponent<Renderer>();
            ctrl.yellowRenderer = yLens.GetComponent<Renderer>();
            ctrl.greenRenderer = gLens.GetComponent<Renderer>();

            ctrl.redOnMat = rOn;
            ctrl.redOffMat = rOff;
            ctrl.yellowOnMat = yOn;
            ctrl.yellowOffMat = yOff;
            ctrl.greenOnMat = gOn;
            ctrl.greenOffMat = gOff;

            ctrl.stopCollider = stopCol;
            ctrl.SetInitialState(initState, startOffset);
        }

        private static void SpawnBuilding(GameObject prefab, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent, Material mat = null)
        {
            if (prefab == null) return;
            GameObject bld = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            bld.transform.SetParent(parent);
            bld.transform.position = pos;
            bld.transform.rotation = rot;
            bld.transform.localScale = scale;
            CleanImportedObject(bld);
            if (mat != null) ApplyMaterialToRenderers(bld, mat);
            AddBoxColliderToHierarchy(bld);
        }

        private static void SpawnProp(GameObject prefab, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent, Material mat = null, bool addCol = true)
        {
            if (prefab == null) return;
            GameObject prop = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            prop.transform.SetParent(parent);
            prop.transform.position = pos;
            prop.transform.rotation = rot;
            prop.transform.localScale = scale;
            CleanImportedObject(prop);
            if (mat != null) ApplyMaterialToRenderers(prop, mat);
            if (addCol) AddBoxColliderToHierarchy(prop);
        }

        private static void ApplyMaterialToRenderers(GameObject obj, Material mat)
        {
            if (mat == null) return;
            Renderer[] rends = obj.GetComponentsInChildren<Renderer>(true);
            foreach (var r in rends)
            {
                r.sharedMaterial = mat;
            }
        }

        private static void CleanImportedObject(GameObject root)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(root))
            {
                PrefabUtility.UnpackPrefabInstance(root, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }
            Camera[] cams = root.GetComponentsInChildren<Camera>(true);
            for (int i = cams.Length - 1; i >= 0; i--)
            {
                if (cams[i] != null)
                {
                    cams[i].enabled = false;
                }
            }
            AudioListener[] listeners = root.GetComponentsInChildren<AudioListener>(true);
            for (int i = listeners.Length - 1; i >= 0; i--)
            {
                if (listeners[i] != null) Object.DestroyImmediate(listeners[i]);
            }
            Light[] lights = root.GetComponentsInChildren<Light>(true);
            for (int i = lights.Length - 1; i >= 0; i--)
            {
                if (lights[i] != null)
                {
                    lights[i].enabled = false;
                }
            }
        }

        private static void AddBoxColliderToHierarchy(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return;

            Bounds b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                b.Encapsulate(renderers[i].bounds);
            }

            BoxCollider col = root.GetComponent<BoxCollider>();
            if (col == null) col = root.AddComponent<BoxCollider>();

            col.center = root.transform.InverseTransformPoint(b.center);
            Vector3 worldSize = b.size;
            Vector3 lossy = root.transform.lossyScale;
            col.size = new Vector3(
                lossy.x != 0 ? Mathf.Abs(worldSize.x / lossy.x) : worldSize.x,
                lossy.y != 0 ? Mathf.Abs(worldSize.y / lossy.y) : worldSize.y,
                lossy.z != 0 ? Mathf.Abs(worldSize.z / lossy.z) : worldSize.z
            );
        }

        private static void AddCapsuleCollider(GameObject root, float radius, float height)
        {
            CapsuleCollider col = root.GetComponent<CapsuleCollider>();
            if (col == null) col = root.AddComponent<CapsuleCollider>();
            col.radius = radius;
            col.height = height;
            col.center = new Vector3(0f, height * 0.5f, 0f);
        }
    }
}
