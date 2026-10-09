using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Task8_11
{
    [InitializeOnLoad]
    public static class RoomBuilder
    {
        private static bool isBuilding = false;

        static RoomBuilder()
        {
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
        }

        private static void TriggerBuild()
        {
        }

        private static void CheckAndBuildIfNeeded()
        {
        }

        [MenuItem("City/Build Interior Room (Tasks 8-11)")]
        public static void BuildRoomMenu()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            BuildRoomAndPortals(scene);
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            Debug.Log("Interior Room, City View, and Portals built and saved successfully!");
        }

        public static void BuildRoomAndPortals(Scene scene)
        {
            GameObject existingRoom = GameObject.Find("--- INTERIOR ROOM (TASKS 8-11) ---");
            if (existingRoom != null)
            {
                Object.DestroyImmediate(existingRoom);
            }

            GameObject existingCityPortal = GameObject.Find("Portal_City_To_Room");
            if (existingCityPortal != null)
            {
                Object.DestroyImmediate(existingCityPortal);
            }

            GameObject existingCitySpawn = GameObject.Find("SpawnPoint_City");
            if (existingCitySpawn != null)
            {
                Object.DestroyImmediate(existingCitySpawn);
            }

            GameObject roomRoot = new GameObject("--- INTERIOR ROOM (TASKS 8-11) ---");

            Material litShaderMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            Material portalMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            portalMat.SetColor("_BaseColor", new Color(0.1f, 0.85f, 1.0f, 1.0f));
            portalMat.color = new Color(0.1f, 0.85f, 1.0f, 1.0f);
            AssetDatabase.CreateAsset(portalMat, "Assets/Materials/Mat_Portal_Blue.mat");

            Material thresholdMat = new Material(litShaderMat);
            thresholdMat.SetColor("_BaseColor", new Color(0.18f, 0.20f, 0.22f));
            thresholdMat.SetFloat("_Smoothness", 0.3f);
            AssetDatabase.CreateAsset(thresholdMat, "Assets/Materials/Mat_Portal_Threshold.mat");

            Material wallMat = new Material(litShaderMat);
            wallMat.SetColor("_BaseColor", new Color(0.92f, 0.89f, 0.83f));
            AssetDatabase.CreateAsset(wallMat, "Assets/Materials/Mat_Room_Wall.mat");

            Material floorMat = new Material(litShaderMat);
            floorMat.SetColor("_BaseColor", new Color(0.72f, 0.54f, 0.36f));
            AssetDatabase.CreateAsset(floorMat, "Assets/Materials/Mat_Room_Floor.mat");

            Material ceilingMat = new Material(litShaderMat);
            ceilingMat.SetColor("_BaseColor", new Color(0.96f, 0.96f, 0.96f));
            AssetDatabase.CreateAsset(ceilingMat, "Assets/Materials/Mat_Room_Ceiling.mat");

            Material woodMat = new Material(litShaderMat);
            woodMat.SetColor("_BaseColor", new Color(0.55f, 0.35f, 0.20f));
            AssetDatabase.CreateAsset(woodMat, "Assets/Materials/Mat_Room_Wood.mat");

            Material blindsMat = new Material(litShaderMat);
            blindsMat.SetColor("_BaseColor", new Color(0.88f, 0.88f, 0.86f));
            AssetDatabase.CreateAsset(blindsMat, "Assets/Materials/Mat_Room_Blinds.mat");

            Material paperMat = new Material(litShaderMat);
            paperMat.SetColor("_BaseColor", new Color(0.96f, 0.96f, 0.93f));
            paperMat.SetFloat("_Smoothness", 0.1f);
            AssetDatabase.CreateAsset(paperMat, "Assets/Materials/Mat_Room_Paper.mat");

            Material asphaltMat = new Material(litShaderMat);
            asphaltMat.SetColor("_BaseColor", new Color(0.22f, 0.22f, 0.24f));
            AssetDatabase.CreateAsset(asphaltMat, "Assets/Materials/Mat_City_Asphalt.mat");

            Material skyMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            skyMat.SetColor("_BaseColor", new Color(0.48f, 0.72f, 0.94f));
            AssetDatabase.CreateAsset(skyMat, "Assets/Materials/Mat_City_Sky.mat");

            Material handleMat = new Material(litShaderMat);
            handleMat.SetColor("_BaseColor", new Color(0.85f, 0.72f, 0.30f));
            handleMat.SetFloat("_Metallic", 0.85f);
            handleMat.SetFloat("_Smoothness", 0.75f);

            Vector3 rCenter = new Vector3(300f, -50f, 300f);
            float rWidth = 5.6f;
            float rLength = 5.6f;
            float rHeight = 2.2f;
            float floorY = rCenter.y;
            float ceilY = floorY + rHeight;

            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Room_Floor";
            floor.transform.SetParent(roomRoot.transform);
            floor.transform.position = new Vector3(rCenter.x, floorY - 0.1f, rCenter.z);
            floor.transform.localScale = new Vector3(rWidth, 0.2f, rLength);
            floor.GetComponent<Renderer>().sharedMaterial = floorMat;

            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Room_Ceiling";
            ceiling.transform.SetParent(roomRoot.transform);
            ceiling.transform.position = new Vector3(rCenter.x, ceilY + 0.25f, rCenter.z);
            ceiling.transform.localScale = new Vector3(rWidth, 0.5f, rLength);
            ceiling.GetComponent<Renderer>().sharedMaterial = ceilingMat;

            GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightWall.name = "Wall_Right";
            rightWall.transform.SetParent(roomRoot.transform);
            rightWall.transform.position = new Vector3(rCenter.x + rWidth * 0.5f + 0.1f, floorY + rHeight * 0.5f, rCenter.z);
            rightWall.transform.localScale = new Vector3(0.2f, rHeight, rLength);
            rightWall.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject frontWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frontWall.name = "Wall_Front";
            frontWall.transform.SetParent(roomRoot.transform);
            frontWall.transform.position = new Vector3(rCenter.x, floorY + rHeight * 0.5f, rCenter.z - rLength * 0.5f - 0.1f);
            frontWall.transform.localScale = new Vector3(rWidth, rHeight, 0.2f);
            frontWall.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject backWallLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWallLeft.name = "Wall_Back_Left";
            backWallLeft.transform.SetParent(roomRoot.transform);
            backWallLeft.transform.position = new Vector3(rCenter.x - 1.85f, floorY + rHeight * 0.5f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallLeft.transform.localScale = new Vector3(1.9f, rHeight, 0.2f);
            backWallLeft.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject backWallRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWallRight.name = "Wall_Back_Right";
            backWallRight.transform.SetParent(roomRoot.transform);
            backWallRight.transform.position = new Vector3(rCenter.x + 1.85f, floorY + rHeight * 0.5f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallRight.transform.localScale = new Vector3(1.9f, rHeight, 0.2f);
            backWallRight.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject backWallUnder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWallUnder.name = "Wall_Back_UnderWindow";
            backWallUnder.transform.SetParent(roomRoot.transform);
            backWallUnder.transform.position = new Vector3(rCenter.x, floorY + 0.40f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallUnder.transform.localScale = new Vector3(1.8f, 0.8f, 0.2f);
            backWallUnder.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject backWallOver = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWallOver.name = "Wall_Back_OverWindow";
            backWallOver.transform.SetParent(roomRoot.transform);
            backWallOver.transform.position = new Vector3(rCenter.x, floorY + 1.95f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallOver.transform.localScale = new Vector3(1.8f, 0.5f, 0.2f);
            backWallOver.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject windowSill = GameObject.CreatePrimitive(PrimitiveType.Cube);
            windowSill.name = "Window_Sill";
            windowSill.transform.SetParent(roomRoot.transform);
            windowSill.transform.position = new Vector3(rCenter.x, floorY + 0.79f, rCenter.z + rLength * 0.5f + 0.05f);
            windowSill.transform.localScale = new Vector3(1.85f, 0.04f, 0.30f);
            windowSill.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject windowGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            windowGlass.name = "Window_Glass";
            windowGlass.transform.SetParent(roomRoot.transform);
            windowGlass.transform.position = new Vector3(rCenter.x, floorY + 1.25f, rCenter.z + rLength * 0.5f + 0.05f);
            windowGlass.transform.localScale = new Vector3(1.75f, 0.9f, 0.04f);
            Material glassMat = new Material(litShaderMat);
            glassMat.SetColor("_BaseColor", new Color(0.6f, 0.85f, 0.95f, 0.25f));
            glassMat.SetFloat("_Surface", 1f);
            windowGlass.GetComponent<Renderer>().sharedMaterial = glassMat;

            GameObject windowSunObj = new GameObject("Window_Sunlight");
            windowSunObj.transform.SetParent(roomRoot.transform);
            windowSunObj.transform.position = new Vector3(rCenter.x, floorY + 2.4f, rCenter.z + rLength * 0.5f + 2.5f);
            Light windowSun = windowSunObj.AddComponent<Light>();
            windowSun.type = LightType.Spot;
            windowSun.range = 8.5f;
            windowSun.spotAngle = 72f;
            windowSun.intensity = 2.2f;
            windowSun.color = new Color(1.0f, 0.96f, 0.88f);
            windowSunObj.transform.LookAt(new Vector3(rCenter.x, floorY + 0.9f, rCenter.z + 1.5f));

            GameObject blindsRoot = new GameObject("Window_Blinds_Mechanism");
            blindsRoot.transform.SetParent(roomRoot.transform);
            blindsRoot.transform.position = new Vector3(rCenter.x, floorY + 1.25f, rCenter.z + rLength * 0.5f - 0.05f);

            GameObject blindsTopBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            blindsTopBox.name = "Blinds_Top_Box";
            blindsTopBox.transform.SetParent(blindsRoot.transform);
            blindsTopBox.transform.localPosition = new Vector3(0f, 0.44f, 0f);
            blindsTopBox.transform.localScale = new Vector3(1.76f, 0.05f, 0.10f);
            blindsTopBox.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(blindsTopBox.GetComponent<Collider>());

            List<Transform> slatList = new List<Transform>();
            int slatCount = 10;
            float slatSpacing = 0.86f / slatCount;
            float slatStartOffset = -0.42f + slatSpacing * 0.5f;

            for (int i = 0; i < slatCount; i++)
            {
                GameObject slat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slat.name = "Slat_" + i;
                slat.transform.SetParent(blindsRoot.transform);
                slat.transform.localPosition = new Vector3(0f, slatStartOffset + i * slatSpacing, 0f);
                slat.transform.localScale = new Vector3(1.74f, 0.015f, 0.12f);
                slat.GetComponent<Renderer>().sharedMaterial = blindsMat;
                Object.DestroyImmediate(slat.GetComponent<Collider>());
                slatList.Add(slat.transform);
            }

            BoxCollider blindsCol = blindsRoot.AddComponent<BoxCollider>();
            blindsCol.size = new Vector3(1.8f, 0.95f, 0.25f);
            blindsCol.isTrigger = false;

            WindowBlinds blindsComp = blindsRoot.AddComponent<WindowBlinds>();
            blindsComp.slats = slatList.ToArray();
            blindsComp.windowLight = windowSun;
            blindsComp.closedAngle = 84f;
            blindsComp.isOpen = true;

            GameObject cityViewRoot = new GameObject("Window_City_View");
            cityViewRoot.transform.SetParent(roomRoot.transform);

            GameObject cityRoad = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cityRoad.name = "CityView_Road";
            cityRoad.transform.SetParent(cityViewRoot.transform);
            cityRoad.transform.position = new Vector3(rCenter.x, floorY - 3.2f, rCenter.z + 14.0f);
            cityRoad.transform.localScale = new Vector3(45f, 0.2f, 18f);
            cityRoad.GetComponent<Renderer>().sharedMaterial = asphaltMat;
            Object.DestroyImmediate(cityRoad.GetComponent<Collider>());

            GameObject citySky = GameObject.CreatePrimitive(PrimitiveType.Cube);
            citySky.name = "CityView_Sky";
            citySky.transform.SetParent(cityViewRoot.transform);
            citySky.transform.position = new Vector3(rCenter.x, floorY + 8.0f, rCenter.z + 28.0f);
            citySky.transform.localScale = new Vector3(60f, 25f, 0.5f);
            citySky.GetComponent<Renderer>().sharedMaterial = skyMat;
            Object.DestroyImmediate(citySky.GetComponent<Collider>());

            GameObject bSkyscraperA = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Buildings/Models/FBX format/building-skyscraper-a.fbx");
            if (bSkyscraperA != null)
            {
                SpawnProp(bSkyscraperA, new Vector3(rCenter.x - 7.5f, floorY - 3.1f, rCenter.z + 20.0f), Quaternion.Euler(0f, 180f, 0f), 1.0f, "CityView_Skyscraper_A", 0f, cityViewRoot.transform);
            }

            GameObject bSkyscraperB = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Buildings/Models/FBX format/building-skyscraper-b.fbx");
            if (bSkyscraperB != null)
            {
                SpawnProp(bSkyscraperB, new Vector3(rCenter.x + 7.5f, floorY - 3.1f, rCenter.z + 21.0f), Quaternion.Euler(0f, 180f, 0f), 1.0f, "CityView_Skyscraper_B", 0f, cityViewRoot.transform);
            }

            GameObject bBuildingA = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Buildings/Models/FBX format/building-a.fbx");
            if (bBuildingA != null)
            {
                SpawnProp(bBuildingA, new Vector3(rCenter.x, floorY - 3.1f, rCenter.z + 18.5f), Quaternion.Euler(0f, 180f, 0f), 1.0f, "CityView_Building_A", 0f, cityViewRoot.transform);
            }

            GameObject bBuildingC = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Buildings/Models/FBX format/building-c.fbx");
            if (bBuildingC != null)
            {
                SpawnProp(bBuildingC, new Vector3(rCenter.x - 16.0f, floorY - 3.1f, rCenter.z + 19.0f), Quaternion.Euler(0f, 180f, 0f), 1.0f, "CityView_Building_C", 0f, cityViewRoot.transform);
            }

            GameObject bBuildingD = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Buildings/Models/FBX format/building-d.fbx");
            if (bBuildingD != null)
            {
                SpawnProp(bBuildingD, new Vector3(rCenter.x + 16.0f, floorY - 3.1f, rCenter.z + 19.5f), Quaternion.Euler(0f, 180f, 0f), 1.0f, "CityView_Building_D", 0f, cityViewRoot.transform);
            }

            GameObject carSedan = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Vehicles/Models/FBX format/sedan.fbx");
            if (carSedan != null)
            {
                SpawnProp(carSedan, new Vector3(rCenter.x - 2.5f, floorY - 3.1f, rCenter.z + 11.0f), Quaternion.Euler(0f, 90f, 0f), 1.0f, "CityView_Sedan", 0f, cityViewRoot.transform);
            }

            GameObject carSports = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/Vehicles/Models/FBX format/sedan-sports.fbx");
            if (carSports != null)
            {
                SpawnProp(carSports, new Vector3(rCenter.x + 3.8f, floorY - 3.1f, rCenter.z + 13.5f), Quaternion.Euler(0f, -90f, 0f), 1.0f, "CityView_Sedan_Sports", 0f, cityViewRoot.transform);
            }

            GameObject leftWallFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWallFront.name = "Wall_Left_Front";
            leftWallFront.transform.SetParent(roomRoot.transform);
            leftWallFront.transform.position = new Vector3(rCenter.x - rWidth * 0.5f - 0.1f, floorY + rHeight * 0.5f, rCenter.z - 1.6f);
            leftWallFront.transform.localScale = new Vector3(0.2f, rHeight, 2.4f);
            leftWallFront.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject leftWallBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWallBack.name = "Wall_Left_Back";
            leftWallBack.transform.SetParent(roomRoot.transform);
            leftWallBack.transform.position = new Vector3(rCenter.x - rWidth * 0.5f - 0.1f, floorY + rHeight * 0.5f, rCenter.z + 1.6f);
            leftWallBack.transform.localScale = new Vector3(0.2f, rHeight, 2.4f);
            leftWallBack.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject leftWallOverDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWallOverDoor.name = "Wall_Left_OverDoor";
            leftWallOverDoor.transform.SetParent(roomRoot.transform);
            leftWallOverDoor.transform.position = new Vector3(rCenter.x - rWidth * 0.5f - 0.1f, floorY + 1.8f, rCenter.z);
            leftWallOverDoor.transform.localScale = new Vector3(0.2f, 0.8f, 1.0f);
            leftWallOverDoor.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject doorFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorFrame.name = "Entrance_DoorFrame";
            doorFrame.transform.SetParent(roomRoot.transform);
            doorFrame.transform.position = new Vector3(rCenter.x - rWidth * 0.5f, floorY + 0.7f, rCenter.z);
            doorFrame.transform.localScale = new Vector3(0.18f, 1.42f, 0.98f);
            doorFrame.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(doorFrame.GetComponent<Collider>());

            GameObject doorPivot = new GameObject("Entrance_Door");
            doorPivot.transform.SetParent(roomRoot.transform);
            doorPivot.transform.position = new Vector3(rCenter.x - rWidth * 0.5f, floorY, rCenter.z + 0.44f);

            GameObject doorLeaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorLeaf.name = "Door_Leaf";
            doorLeaf.transform.SetParent(doorPivot.transform);
            doorLeaf.transform.localPosition = new Vector3(0f, 0.675f, -0.44f);
            doorLeaf.transform.localScale = new Vector3(0.06f, 1.35f, 0.88f);
            doorLeaf.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject doorKnob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            doorKnob.name = "Knob";
            doorKnob.transform.SetParent(doorLeaf.transform);
            doorKnob.transform.localPosition = new Vector3(0.55f, 0f, -0.34f);
            doorKnob.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            doorKnob.GetComponent<Renderer>().sharedMaterial = handleMat;
            Object.DestroyImmediate(doorKnob.GetComponent<Collider>());

            EntranceDoor doorComp = doorPivot.AddComponent<EntranceDoor>();
            doorComp.doorLeaf = doorPivot.transform;
            doorComp.isOpen = false;

            GameObject ceilingLightObj = new GameObject("Room_Ceiling_Light");
            ceilingLightObj.transform.SetParent(roomRoot.transform);
            ceilingLightObj.transform.position = new Vector3(rCenter.x, floorY + rHeight - 0.2f, rCenter.z);
            Light ceilingLight = ceilingLightObj.AddComponent<Light>();
            ceilingLight.type = LightType.Point;
            ceilingLight.range = 8.5f;
            ceilingLight.intensity = 2.8f;
            ceilingLight.color = new Color(1.0f, 0.95f, 0.88f);

            GameObject lampLightObj = new GameObject("Desk_Lamp_Light");
            lampLightObj.transform.SetParent(roomRoot.transform);
            lampLightObj.transform.position = new Vector3(rCenter.x + 1.05f, floorY + 0.95f, rCenter.z + 1.35f);
            Light tableLampLight = lampLightObj.AddComponent<Light>();
            tableLampLight.type = LightType.Point;
            tableLampLight.range = 3.5f;
            tableLampLight.intensity = 1.8f;
            tableLampLight.color = new Color(1.0f, 0.92f, 0.72f);

            GameObject switchObj = new GameObject("Light_Switch");
            switchObj.transform.SetParent(roomRoot.transform);
            switchObj.transform.position = new Vector3(rCenter.x - rWidth * 0.5f + 0.08f, floorY + 0.95f, rCenter.z + 0.7f);

            GameObject switchPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            switchPlate.name = "Switch_Plate";
            switchPlate.transform.SetParent(switchObj.transform);
            switchPlate.transform.localPosition = Vector3.zero;
            switchPlate.transform.localScale = new Vector3(0.04f, 0.14f, 0.10f);
            switchPlate.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject switchLever = GameObject.CreatePrimitive(PrimitiveType.Cube);
            switchLever.name = "Switch_Lever";
            switchLever.transform.SetParent(switchObj.transform);
            switchLever.transform.localPosition = new Vector3(0.025f, 0f, 0f);
            switchLever.transform.localScale = new Vector3(0.03f, 0.05f, 0.025f);
            switchLever.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(switchLever.GetComponent<Collider>());

            LightSwitch switchComp = switchObj.AddComponent<LightSwitch>();
            switchComp.toggleLever = switchLever.transform;
            switchComp.targetLights = new Light[] { ceilingLight, tableLampLight };
            switchComp.isOn = true;
            switchComp.switchName = "свет";

            GameObject fanRoot = new GameObject("Ceiling_Fan");
            fanRoot.transform.SetParent(roomRoot.transform);
            fanRoot.transform.position = new Vector3(rCenter.x + 1.6f, floorY + rHeight - 0.12f, rCenter.z + 1.45f);

            GameObject fanRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fanRod.name = "Fan_Rod";
            fanRod.transform.SetParent(fanRoot.transform);
            fanRod.transform.localPosition = new Vector3(0f, -0.10f, 0f);
            fanRod.transform.localScale = new Vector3(0.03f, 0.10f, 0.03f);
            fanRod.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(fanRod.GetComponent<Collider>());

            GameObject fanHub = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fanHub.name = "Fan_Hub";
            fanHub.transform.SetParent(fanRoot.transform);
            fanHub.transform.localPosition = new Vector3(0f, -0.22f, 0f);
            fanHub.transform.localScale = new Vector3(0.24f, 0.04f, 0.24f);
            fanHub.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(fanHub.GetComponent<Collider>());

            GameObject fanBlades = new GameObject("Fan_Blades");
            fanBlades.transform.SetParent(fanHub.transform);
            fanBlades.transform.localPosition = Vector3.zero;

            for (int bi = 0; bi < 3; bi++)
            {
                float bAngle = bi * 120f;
                GameObject blade = GameObject.CreatePrimitive(PrimitiveType.Cube);
                blade.name = "Blade_" + bi;
                blade.transform.SetParent(fanBlades.transform);
                blade.transform.localPosition = Quaternion.Euler(0f, bAngle, 0f) * new Vector3(0.38f, 0f, 0f);
                blade.transform.localRotation = Quaternion.Euler(6f, bAngle, 0f);
                blade.transform.localScale = new Vector3(0.50f, 0.012f, 0.11f);
                blade.GetComponent<Renderer>().sharedMaterial = woodMat;
                Object.DestroyImmediate(blade.GetComponent<Collider>());
            }

            BoxCollider fanTrigger = fanRoot.AddComponent<BoxCollider>();
            fanTrigger.size = new Vector3(1.2f, 0.8f, 1.2f);
            fanTrigger.center = new Vector3(0f, -0.25f, 0f);
            fanTrigger.isTrigger = false;

            FanController fanComp = fanRoot.AddComponent<FanController>();
            fanComp.blades = fanBlades.transform;
            fanComp.windOrigin = fanRoot.transform;
            fanComp.windRadius = 4.0f;
            fanComp.windForce = 16f;
            fanComp.isSpinning = false;

            float deskTopY = floorY + 0.54f;

            GameObject deskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/desk.fbx");
            if (deskPrefab != null)
            {
                SpawnProp(deskPrefab, new Vector3(rCenter.x + 1.6f, floorY, rCenter.z + 1.5f), Quaternion.Euler(0f, 180f, 0f), 0.14f, "Desk", 35f, roomRoot.transform, true, true);
            }

            GameObject chairDeskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/chairDesk.fbx");
            if (chairDeskPrefab != null)
            {
                SpawnProp(chairDeskPrefab, new Vector3(rCenter.x + 1.6f, floorY + 0.005f, rCenter.z + 1.95f), Quaternion.Euler(0f, 0f, 0f), 0.085f, "Chair_Office", 12f, roomRoot.transform, true, true, "Офисное кресло");
            }

            GameObject rugRectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/rugRectangle.fbx");
            if (rugRectPrefab != null)
            {
                SpawnProp(rugRectPrefab, new Vector3(rCenter.x + 1.6f, floorY + 0.002f, rCenter.z + 1.65f), Quaternion.identity, 0.14f, "Rug_Desk", 0f, roomRoot.transform, false, false);
            }

            GameObject monitorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/computerScreen.fbx");
            if (monitorPrefab != null)
            {
                SpawnProp(monitorPrefab, new Vector3(rCenter.x + 1.6f, deskTopY, rCenter.z + 1.30f), Quaternion.Euler(0f, 180f, 0f), 0.11f, "Monitor", 0f, roomRoot.transform, false, true);
            }

            GameObject kbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/computerKeyboard.fbx");
            if (kbPrefab != null)
            {
                SpawnProp(kbPrefab, new Vector3(rCenter.x + 1.6f, deskTopY, rCenter.z + 1.60f), Quaternion.Euler(0f, 180f, 0f), 0.11f, "Keyboard", 0f, roomRoot.transform, false, true);
            }

            GameObject mousePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/computerMouse.fbx");
            if (mousePrefab != null)
            {
                SpawnProp(mousePrefab, new Vector3(rCenter.x + 1.35f, deskTopY, rCenter.z + 1.60f), Quaternion.Euler(0f, 180f, 0f), 0.11f, "Mouse", 0f, roomRoot.transform, false, true);
            }

            GameObject lampTablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/lampSquareTable.fbx");
            if (lampTablePrefab != null)
            {
                GameObject deskLampObj = SpawnProp(lampTablePrefab, new Vector3(rCenter.x + 1.05f, deskTopY, rCenter.z + 1.35f), Quaternion.Euler(0f, 45f, 0f), 0.11f, "Lamp", 0f, roomRoot.transform, false, true);
                if (deskLampObj != null)
                {
                    LightSwitch deskLampSwitch = deskLampObj.AddComponent<LightSwitch>();
                    deskLampSwitch.targetLights = new Light[] { tableLampLight };
                    deskLampSwitch.isOn = true;
                    deskLampSwitch.switchName = "настольную лампу";
                    deskLampSwitch.hotkey = KeyCode.L;
                }
            }

            GameObject laptopPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/laptop.fbx");
            if (laptopPrefab != null)
            {
                SpawnPickup(laptopPrefab, new Vector3(rCenter.x + 2.05f, deskTopY, rCenter.z + 1.48f), Quaternion.Euler(0f, 155f, 0f), 0.045f, "Ноутбук", 2.0f, roomRoot.transform);
            }

            GameObject trashPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/trashcan.fbx");
            if (trashPrefab != null)
            {
                SpawnPickup(trashPrefab, new Vector3(rCenter.x + 2.15f, floorY, rCenter.z + 1.95f), Quaternion.identity, 0.045f, "Корзина для бумаг", 1.2f, roomRoot.transform);
            }

            SpawnPaper(new Vector3(rCenter.x + 1.45f, deskTopY + 0.003f, rCenter.z + 1.48f), 10f, "Чертеж проекта", paperMat, roomRoot.transform);
            SpawnPaper(new Vector3(rCenter.x + 1.46f, deskTopY + 0.006f, rCenter.z + 1.47f), -5f, "План здания", paperMat, roomRoot.transform);
            SpawnPaper(new Vector3(rCenter.x + 1.44f, deskTopY + 0.009f, rCenter.z + 1.49f), 15f, "Отчет по практике", paperMat, roomRoot.transform);
            SpawnPaper(new Vector3(rCenter.x + 1.75f, deskTopY + 0.003f, rCenter.z + 1.45f), -18f, "Лист бумаги", paperMat, roomRoot.transform);
            SpawnPaper(new Vector3(rCenter.x + 1.75f, deskTopY + 0.006f, rCenter.z + 1.52f), 22f, "Записка с кодом", paperMat, roomRoot.transform);
            SpawnPaper(new Vector3(rCenter.x + 1.85f, deskTopY + 0.003f, rCenter.z + 1.40f), -12f, "Документ А4", paperMat, roomRoot.transform);

            GameObject loungeSofaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/loungeSofa.fbx");
            if (loungeSofaPrefab != null)
            {
                SpawnProp(loungeSofaPrefab, new Vector3(rCenter.x - 1.0f, floorY, rCenter.z + 2.15f), Quaternion.identity, 0.12f, "Sofa_Lounge", 0f, roomRoot.transform, false, true);
            }

            GameObject pillowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/pillow.fbx");
            if (pillowPrefab != null)
            {
                SpawnProp(pillowPrefab, new Vector3(rCenter.x - 1.35f, floorY + 0.35f, rCenter.z + 2.05f), Quaternion.Euler(0f, 15f, 0f), 0.09f, "Pillow_1", 0f, roomRoot.transform, false, false);
            }

            GameObject pillowBluePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/pillowBlue.fbx");
            if (pillowBluePrefab != null)
            {
                SpawnProp(pillowBluePrefab, new Vector3(rCenter.x - 0.65f, floorY + 0.35f, rCenter.z + 2.05f), Quaternion.Euler(0f, -15f, 0f), 0.09f, "Pillow_Blue", 0f, roomRoot.transform, false, false);
            }

            GameObject rugRoundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/rugRound.fbx");
            if (rugRoundPrefab != null)
            {
                SpawnProp(rugRoundPrefab, new Vector3(rCenter.x - 1.0f, floorY + 0.003f, rCenter.z + 1.15f), Quaternion.identity, 0.15f, "Rug_Coffee", 0f, roomRoot.transform, false, false);
            }

            GameObject coffeeTablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/tableCoffee.fbx");
            if (coffeeTablePrefab != null)
            {
                SpawnProp(coffeeTablePrefab, new Vector3(rCenter.x - 1.0f, floorY, rCenter.z + 1.15f), Quaternion.identity, 0.13f, "Table_Coffee", 15f, roomRoot.transform, true, true);
            }

            GameObject chairPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/chair.fbx");
            if (chairPrefab != null)
            {
                SpawnProp(chairPrefab, new Vector3(rCenter.x - 0.15f, floorY, rCenter.z + 1.15f), Quaternion.Euler(0f, -90f, 0f), 0.085f, "Chair_Side", 12f, roomRoot.transform, true, true, "Стул");
            }

            GameObject floorLampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/lampRoundFloor.fbx");
            if (floorLampPrefab != null)
            {
                GameObject floorLampObj = SpawnProp(floorLampPrefab, new Vector3(rCenter.x - 2.15f, floorY, rCenter.z + 2.15f), Quaternion.identity, 0.12f, "Lamp_Floor", 0f, roomRoot.transform, false, true);

                GameObject floorLightObj = new GameObject("Floor_Lamp_Light");
                floorLightObj.transform.SetParent(roomRoot.transform);
                floorLightObj.transform.position = new Vector3(rCenter.x - 2.15f, floorY + 1.2f, rCenter.z + 2.15f);
                Light floorLight = floorLightObj.AddComponent<Light>();
                floorLight.type = LightType.Point;
                floorLight.range = 3.0f;
                floorLight.intensity = 1.4f;
                floorLight.color = new Color(1.0f, 0.88f, 0.70f);

                if (floorLampObj != null)
                {
                    LightSwitch floorLampSwitch = floorLampObj.AddComponent<LightSwitch>();
                    floorLampSwitch.targetLights = new Light[] { floorLight };
                    floorLampSwitch.isOn = true;
                    floorLampSwitch.switchName = "торшер";
                }

                if (switchComp != null)
                {
                    switchComp.targetLights = new Light[] { ceilingLight, tableLampLight, floorLight };
                }
            }

            GameObject tvCabPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/cabinetTelevision.fbx");
            if (tvCabPrefab != null)
            {
                SpawnProp(tvCabPrefab, new Vector3(rCenter.x + 2.15f, floorY, rCenter.z - 0.85f), Quaternion.Euler(0f, 90f, 0f), 0.13f, "TV_Cabinet", 0f, roomRoot.transform, false, true);
            }

            GameObject tvPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/televisionModern.fbx");
            if (tvPrefab != null)
            {
                SpawnProp(tvPrefab, new Vector3(rCenter.x + 2.15f, floorY + 0.39f, rCenter.z - 0.85f), Quaternion.Euler(0f, 90f, 0f), 0.13f, "TV_Modern", 0f, roomRoot.transform, false, true);
            }

            GameObject speakerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/speakerSmall.fbx");
            if (speakerPrefab != null)
            {
                SpawnProp(speakerPrefab, new Vector3(rCenter.x + 2.15f, floorY + 0.39f, rCenter.z - 0.35f), Quaternion.Euler(0f, 90f, 0f), 0.11f, "Speaker_Left", 0f, roomRoot.transform, false, true);
                SpawnProp(speakerPrefab, new Vector3(rCenter.x + 2.15f, floorY + 0.39f, rCenter.z - 1.35f), Quaternion.Euler(0f, 90f, 0f), 0.11f, "Speaker_Right", 0f, roomRoot.transform, false, true);
            }

            GameObject bookcasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/bookcaseOpen.fbx");
            if (bookcasePrefab != null)
            {
                SpawnProp(bookcasePrefab, new Vector3(rCenter.x + 1.8f, floorY, rCenter.z - rLength * 0.5f + 0.32f), Quaternion.identity, 0.13f, "Bookcase_Open", 0f, roomRoot.transform, false, true);
            }

            GameObject sideTableDrawersPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/sideTableDrawers.fbx");
            if (sideTableDrawersPrefab != null)
            {
                SpawnProp(sideTableDrawersPrefab, new Vector3(rCenter.x - 1.35f, floorY, rCenter.z - rLength * 0.5f + 0.32f), Quaternion.identity, 0.13f, "Coffee_Station_Table", 0f, roomRoot.transform, false, true);
            }

            GameObject coffeeMachinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/kitchenCoffeeMachine.fbx");
            if (coffeeMachinePrefab != null)
            {
                SpawnProp(coffeeMachinePrefab, new Vector3(rCenter.x - 1.35f, floorY + 0.52f, rCenter.z - rLength * 0.5f + 0.32f), Quaternion.identity, 0.11f, "Coffee_Machine", 0f, roomRoot.transform, false, true);
            }

            GameObject fridgePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/kitchenFridgeSmall.fbx");
            if (fridgePrefab != null)
            {
                SpawnProp(fridgePrefab, new Vector3(rCenter.x - 2.05f, floorY, rCenter.z - rLength * 0.5f + 0.32f), Quaternion.identity, 0.13f, "Fridge_Small", 0f, roomRoot.transform, false, true);
            }

            GameObject coatRackPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/coatRackStanding.fbx");
            if (coatRackPrefab != null)
            {
                SpawnProp(coatRackPrefab, new Vector3(rCenter.x - rWidth * 0.5f + 0.45f, floorY, rCenter.z - 1.15f), Quaternion.identity, 0.13f, "Coat_Rack", 0f, roomRoot.transform, false, true);
            }

            GameObject pottedPlantPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/pottedPlant.fbx");
            if (pottedPlantPrefab != null)
            {
                SpawnProp(pottedPlantPrefab, new Vector3(rCenter.x - rWidth * 0.5f + 0.45f, floorY, rCenter.z + 1.25f), Quaternion.identity, 0.13f, "Potted_Plant_Entrance", 0f, roomRoot.transform, false, true);
            }

            GameObject cabinetRoot = new GameObject("Cabinet_With_Doors");
            cabinetRoot.transform.SetParent(roomRoot.transform);
            cabinetRoot.transform.position = new Vector3(rCenter.x, floorY, rCenter.z - rLength * 0.5f + 0.32f);

            GameObject cabBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabBack.name = "Cabinet_Back";
            cabBack.transform.SetParent(cabinetRoot.transform);
            cabBack.transform.localPosition = new Vector3(0f, 0.70f, -0.17f);
            cabBack.transform.localScale = new Vector3(1.3f, 1.40f, 0.04f);
            cabBack.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabLeft.name = "Cabinet_Left";
            cabLeft.transform.SetParent(cabinetRoot.transform);
            cabLeft.transform.localPosition = new Vector3(-0.63f, 0.70f, 0f);
            cabLeft.transform.localScale = new Vector3(0.04f, 1.40f, 0.38f);
            cabLeft.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabRight.name = "Cabinet_Right";
            cabRight.transform.SetParent(cabinetRoot.transform);
            cabRight.transform.localPosition = new Vector3(0.63f, 0.70f, 0f);
            cabRight.transform.localScale = new Vector3(0.04f, 1.40f, 0.38f);
            cabRight.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabTop.name = "Cabinet_Top";
            cabTop.transform.SetParent(cabinetRoot.transform);
            cabTop.transform.localPosition = new Vector3(0f, 1.38f, 0f);
            cabTop.transform.localScale = new Vector3(1.3f, 0.04f, 0.38f);
            cabTop.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabBottom.name = "Cabinet_Bottom";
            cabBottom.transform.SetParent(cabinetRoot.transform);
            cabBottom.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            cabBottom.transform.localScale = new Vector3(1.3f, 0.04f, 0.38f);
            cabBottom.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabShelf1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabShelf1.name = "Shelf_1";
            cabShelf1.transform.SetParent(cabinetRoot.transform);
            cabShelf1.transform.localPosition = new Vector3(0f, 0.48f, 0f);
            cabShelf1.transform.localScale = new Vector3(1.22f, 0.03f, 0.35f);
            cabShelf1.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabShelf2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabShelf2.name = "Shelf_2";
            cabShelf2.transform.SetParent(cabinetRoot.transform);
            cabShelf2.transform.localPosition = new Vector3(0f, 0.92f, 0f);
            cabShelf2.transform.localScale = new Vector3(1.22f, 0.03f, 0.35f);
            cabShelf2.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject leftDoorPivot = new GameObject("LeftDoor_Pivot");
            leftDoorPivot.transform.SetParent(cabinetRoot.transform);
            leftDoorPivot.transform.localPosition = new Vector3(-0.62f, 0f, 0.19f);

            GameObject leftDoorLeaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftDoorLeaf.name = "LeftDoor_Leaf";
            leftDoorLeaf.transform.SetParent(leftDoorPivot.transform);
            leftDoorLeaf.transform.localPosition = new Vector3(0.30f, 0.70f, 0f);
            leftDoorLeaf.transform.localScale = new Vector3(0.60f, 1.34f, 0.03f);
            leftDoorLeaf.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject leftDoorKnob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftDoorKnob.name = "Knob";
            leftDoorKnob.transform.SetParent(leftDoorLeaf.transform);
            leftDoorKnob.transform.localPosition = new Vector3(0.40f, 0f, 0.55f);
            leftDoorKnob.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            leftDoorKnob.GetComponent<Renderer>().sharedMaterial = handleMat;
            Object.DestroyImmediate(leftDoorKnob.GetComponent<Collider>());

            GameObject rightDoorPivot = new GameObject("RightDoor_Pivot");
            rightDoorPivot.transform.SetParent(cabinetRoot.transform);
            rightDoorPivot.transform.localPosition = new Vector3(0.62f, 0f, 0.19f);

            GameObject rightDoorLeaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightDoorLeaf.name = "RightDoor_Leaf";
            rightDoorLeaf.transform.SetParent(rightDoorPivot.transform);
            rightDoorLeaf.transform.localPosition = new Vector3(-0.30f, 0.70f, 0f);
            rightDoorLeaf.transform.localScale = new Vector3(0.60f, 1.34f, 0.03f);
            rightDoorLeaf.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject rightDoorKnob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightDoorKnob.name = "Knob";
            rightDoorKnob.transform.SetParent(rightDoorLeaf.transform);
            rightDoorKnob.transform.localPosition = new Vector3(-0.40f, 0.55f, 0.55f);
            rightDoorKnob.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            rightDoorKnob.GetComponent<Renderer>().sharedMaterial = handleMat;
            Object.DestroyImmediate(rightDoorKnob.GetComponent<Collider>());

            CabinetDoor cabDoorComp = cabinetRoot.AddComponent<CabinetDoor>();
            cabDoorComp.leftDoor = leftDoorPivot.transform;
            cabDoorComp.rightDoor = rightDoorPivot.transform;
            cabDoorComp.isOpen = false;

            float shelf1Y = floorY + 0.50f;
            float shelf2Y = floorY + 0.94f;
            float coffeeTableTopY = floorY + 0.38f;
            float cabZ = cabinetRoot.transform.position.z + 0.05f;

            GameObject booksPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/books.fbx");
            if (booksPrefab != null)
            {
                SpawnPickup(booksPrefab, new Vector3(rCenter.x + 1.05f, deskTopY, rCenter.z + 1.58f), Quaternion.identity, 0.18f, "Книги на столе", 1.0f, roomRoot.transform);
                SpawnPickup(booksPrefab, new Vector3(rCenter.x - 0.25f, shelf2Y, cabZ), Quaternion.identity, 0.16f, "Книги в шкафу", 0.8f, roomRoot.transform, cabDoorComp);
                SpawnPickup(booksPrefab, new Vector3(rCenter.x + 0.25f, shelf2Y, cabZ), Quaternion.identity, 0.16f, "Энциклопедия в шкафу", 0.8f, roomRoot.transform, cabDoorComp);
                SpawnPickup(booksPrefab, new Vector3(rCenter.x + 1.8f, floorY + 0.55f, rCenter.z - rLength * 0.5f + 0.32f), Quaternion.identity, 0.16f, "Книги на стеллаже", 0.8f, roomRoot.transform);
            }

            GameObject plant2Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/plantSmall2.fbx");
            if (plant2Prefab != null)
            {
                SpawnPickup(plant2Prefab, new Vector3(rCenter.x - 0.25f, shelf1Y, cabZ), Quaternion.identity, 0.09f, "Цветок в шкафу", 0.9f, roomRoot.transform, cabDoorComp);
            }

            GameObject boxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/cardboardBoxClosed.fbx");
            if (boxPrefab != null)
            {
                SpawnPickup(boxPrefab, new Vector3(rCenter.x + 0.25f, shelf1Y, cabZ), Quaternion.identity, 0.11f, "Коробка в шкафу", 1.2f, roomRoot.transform, cabDoorComp);
                SpawnPickup(boxPrefab, new Vector3(rCenter.x - rWidth * 0.5f + 0.45f, floorY, rCenter.z - 1.8f), Quaternion.Euler(0f, 25f, 0f), 0.16f, "Коробка у входа", 2.2f, roomRoot.transform);
            }

            GameObject radioPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/radio.fbx");
            if (radioPrefab != null)
            {
                SpawnPickup(radioPrefab, new Vector3(rCenter.x - 0.85f, coffeeTableTopY, rCenter.z + 1.15f), Quaternion.identity, 0.08f, "Радиоприемник", 1.1f, roomRoot.transform);
            }

            GameObject plant1Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/plantSmall1.fbx");
            if (plant1Prefab != null)
            {
                SpawnPickup(plant1Prefab, new Vector3(rCenter.x - 1.15f, coffeeTableTopY, rCenter.z + 1.15f), Quaternion.identity, 0.09f, "Цветок на столике", 1.0f, roomRoot.transform);
            }

            GameObject bearPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/bear.fbx");
            if (bearPrefab != null)
            {
                SpawnPickup(bearPrefab, new Vector3(rCenter.x - 1.0f, floorY + 0.35f, rCenter.z + 2.05f), Quaternion.identity, 0.08f, "Плюшевый мишка", 0.7f, roomRoot.transform);
            }

            GameObject plant3Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/plantSmall3.fbx");
            if (plant3Prefab != null)
            {
                SpawnPickup(plant3Prefab, new Vector3(rCenter.x + 1.8f, floorY + 0.95f, rCenter.z - rLength * 0.5f + 0.32f), Quaternion.identity, 0.09f, "Цветок на стеллаже", 0.9f, roomRoot.transform);
            }

            GameObject cityPortalObj = new GameObject("Portal_City_To_Room");
            cityPortalObj.transform.position = new Vector3(-13.73f, 0.02f, -3.98f);

            GameObject cityMatObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cityMatObj.name = "Entrance_Threshold_Mat";
            cityMatObj.transform.SetParent(cityPortalObj.transform);
            cityMatObj.transform.localPosition = new Vector3(0f, 0.01f, 0f);
            cityMatObj.transform.localScale = new Vector3(0.55f, 0.02f, 1.4f);
            cityMatObj.GetComponent<Renderer>().sharedMaterial = thresholdMat;
            Object.DestroyImmediate(cityMatObj.GetComponent<Collider>());

            GameObject cityStripObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cityStripObj.name = "Glowing_Accent_Strip";
            cityStripObj.transform.SetParent(cityPortalObj.transform);
            cityStripObj.transform.localPosition = new Vector3(-0.25f, 0.015f, 0f);
            cityStripObj.transform.localScale = new Vector3(0.04f, 0.025f, 1.36f);
            cityStripObj.GetComponent<Renderer>().sharedMaterial = portalMat;
            Object.DestroyImmediate(cityStripObj.GetComponent<Collider>());

            GameObject cityLightObj = new GameObject("Entrance_Porch_Light");
            cityLightObj.transform.SetParent(cityPortalObj.transform);
            cityLightObj.transform.localPosition = new Vector3(0f, 1.9f, 0f);
            Light cityLight = cityLightObj.AddComponent<Light>();
            cityLight.type = LightType.Point;
            cityLight.range = 3.5f;
            cityLight.intensity = 1.8f;
            cityLight.color = new Color(0.85f, 0.92f, 1.0f);

            GameObject roomSpawnPoint = new GameObject("SpawnPoint_Room");
            roomSpawnPoint.transform.SetParent(roomRoot.transform);
            roomSpawnPoint.transform.position = new Vector3(rCenter.x - rWidth * 0.5f + 0.9f, floorY + 0.1f, rCenter.z);
            roomSpawnPoint.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            TeleportZone cityPortalComp = cityPortalObj.AddComponent<TeleportZone>();
            cityPortalComp.targetPoint = roomSpawnPoint.transform;
            cityPortalComp.interactRadius = 1.35f;
            cityPortalComp.promptText = "[E] Войти в здание";
            cityPortalComp.glowLight = cityLight;
            cityPortalComp.ringRenderer = cityStripObj.GetComponent<Renderer>();

            GameObject roomPortalObj = new GameObject("Portal_Room_To_City");
            roomPortalObj.transform.SetParent(roomRoot.transform);
            roomPortalObj.transform.position = new Vector3(rCenter.x - rWidth * 0.5f + 0.45f, floorY, rCenter.z);

            GameObject doormatPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/rugDoormat.fbx");
            Renderer roomIndicatorRenderer = null;
            if (doormatPrefab != null)
            {
                SpawnProp(doormatPrefab, new Vector3(rCenter.x - rWidth * 0.5f + 0.45f, floorY + 0.002f, rCenter.z), Quaternion.Euler(0f, 90f, 0f), 0.14f, "Doormat_Entrance", 0f, roomRoot.transform, false, false);
            }
            else
            {
                GameObject roomMatObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                roomMatObj.name = "Room_Threshold_Mat";
                roomMatObj.transform.SetParent(roomPortalObj.transform);
                roomMatObj.transform.localPosition = new Vector3(0f, 0.01f, 0f);
                roomMatObj.transform.localScale = new Vector3(0.55f, 0.02f, 0.9f);
                roomMatObj.GetComponent<Renderer>().sharedMaterial = thresholdMat;
                Object.DestroyImmediate(roomMatObj.GetComponent<Collider>());
            }

            GameObject roomStripObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roomStripObj.name = "Room_Glowing_Strip";
            roomStripObj.transform.SetParent(roomPortalObj.transform);
            roomStripObj.transform.localPosition = new Vector3(0.25f, 0.015f, 0f);
            roomStripObj.transform.localScale = new Vector3(0.04f, 0.025f, 0.85f);
            roomStripObj.GetComponent<Renderer>().sharedMaterial = portalMat;
            Object.DestroyImmediate(roomStripObj.GetComponent<Collider>());
            roomIndicatorRenderer = roomStripObj.GetComponent<Renderer>();

            GameObject citySpawnPoint = new GameObject("SpawnPoint_City");
            citySpawnPoint.transform.position = new Vector3(-13.73f, 0.1f, -2.85f);
            citySpawnPoint.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            TeleportZone roomPortalComp = roomPortalObj.AddComponent<TeleportZone>();
            roomPortalComp.targetPoint = citySpawnPoint.transform;
            roomPortalComp.interactRadius = 1.25f;
            roomPortalComp.promptText = "[E] Выйти на улицу";
            roomPortalComp.glowLight = null;
            roomPortalComp.ringRenderer = roomIndicatorRenderer;

            GameObject sentinelObj = new GameObject("Room_Build_v7");
            sentinelObj.transform.SetParent(roomRoot.transform);

            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                PlayerInteraction pi = player.GetComponent<PlayerInteraction>();
                if (pi == null) pi = player.AddComponent<PlayerInteraction>();
            }
        }

        private static void SpawnPaper(Vector3 pos, float yaw, string docName, Material mat, Transform parent)
        {
            GameObject paper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            paper.name = "Paper_" + docName.Replace(" ", "_");
            paper.transform.SetParent(parent);
            paper.transform.position = pos;
            paper.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            paper.transform.localScale = new Vector3(0.21f, 0.003f, 0.297f);
            paper.GetComponent<Renderer>().sharedMaterial = mat;

            Rigidbody rb = paper.AddComponent<Rigidbody>();
            rb.mass = 0.025f;
            rb.linearDamping = 1.8f;
            rb.angularDamping = 2.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            PickupableItem item = paper.AddComponent<PickupableItem>();
            item.itemName = docName;
        }

        private static GameObject SpawnProp(GameObject prefab, Vector3 bottomCenterPos, Quaternion rot, float scale, string name, float mass, Transform parent, bool freezeTilt = true, bool withCollider = true, string pickupName = null)
        {
            GameObject wrapper = new GameObject(name);
            wrapper.transform.SetParent(parent);
            wrapper.transform.position = bottomCenterPos;
            wrapper.transform.rotation = rot;

            GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            model.transform.SetParent(wrapper.transform, false);

            MeshFilter mf = model.GetComponentInChildren<MeshFilter>();
            Bounds b = (mf != null && mf.sharedMesh != null) ? mf.sharedMesh.bounds : new Bounds(Vector3.zero, Vector3.one);
            float yMin = b.center.y - b.size.y * 0.5f;

            model.transform.localPosition = new Vector3(-b.center.x, -yMin, -b.center.z) * scale;
            model.transform.localScale = Vector3.one * scale;

            foreach (var c in model.GetComponentsInChildren<Collider>())
            {
                Object.DestroyImmediate(c);
            }

            if (withCollider)
            {
                BoxCollider col = wrapper.AddComponent<BoxCollider>();
                col.center = new Vector3(0f, b.size.y * scale * 0.5f, 0f);
                col.size = b.size * scale;
            }

            if (mass > 0.01f)
            {
                Rigidbody rb = wrapper.AddComponent<Rigidbody>();
                rb.mass = mass;
                rb.linearDamping = 3.0f;
                rb.angularDamping = 3.0f;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                if (freezeTilt)
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
            }

            if (!string.IsNullOrEmpty(pickupName))
            {
                PickupableItem item = wrapper.AddComponent<PickupableItem>();
                item.itemName = pickupName;
            }

            return wrapper;
        }

        private static void SpawnPickup(GameObject prefab, Vector3 bottomCenterPos, Quaternion rot, float scale, string name, float mass, Transform parent, CabinetDoor cabDoor = null)
        {
            GameObject wrapper = new GameObject("Item_" + prefab.name);
            wrapper.transform.SetParent(parent);
            wrapper.transform.position = bottomCenterPos;
            wrapper.transform.rotation = rot;

            GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            model.transform.SetParent(wrapper.transform, false);

            MeshFilter mf = model.GetComponentInChildren<MeshFilter>();
            Bounds b = (mf != null && mf.sharedMesh != null) ? mf.sharedMesh.bounds : new Bounds(Vector3.zero, Vector3.one);
            float yMin = b.center.y - b.size.y * 0.5f;

            model.transform.localPosition = new Vector3(-b.center.x, -yMin, -b.center.z) * scale;
            model.transform.localScale = Vector3.one * scale;

            foreach (var c in model.GetComponentsInChildren<Collider>())
            {
                Object.DestroyImmediate(c);
            }

            BoxCollider col = wrapper.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, b.size.y * scale * 0.5f, 0f);
            col.size = b.size * scale;

            Rigidbody rb = wrapper.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.linearDamping = 1.5f;
            rb.angularDamping = 1.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            PickupableItem item = wrapper.AddComponent<PickupableItem>();
            item.itemName = name;
            item.parentCabinetDoor = cabDoor;
        }
    }
}
