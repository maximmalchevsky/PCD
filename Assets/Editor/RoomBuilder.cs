using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Task8_11
{
    public static class RoomBuilder
    {
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
            Debug.Log("Interior Room and Portals built and saved successfully!");
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

            Material portalMat = new Material(litShaderMat);
            portalMat.SetColor("_BaseColor", new Color(0.2f, 0.75f, 1.0f));
            portalMat.EnableKeyword("_EMISSION");
            portalMat.SetColor("_EmissionColor", new Color(0.1f, 0.7f, 1.0f) * 3.5f);
            AssetDatabase.CreateAsset(portalMat, "Assets/Materials/Mat_Portal_Blue.mat");

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
            blindsMat.SetColor("_BaseColor", new Color(0.86f, 0.86f, 0.84f));
            AssetDatabase.CreateAsset(blindsMat, "Assets/Materials/Mat_Room_Blinds.mat");

            Material handleMat = new Material(litShaderMat);
            handleMat.SetColor("_BaseColor", new Color(0.85f, 0.72f, 0.30f));
            handleMat.SetFloat("_Metallic", 0.85f);
            handleMat.SetFloat("_Smoothness", 0.75f);

            Vector3 rCenter = new Vector3(300f, -50f, 300f);
            float rWidth = 7.2f;
            float rLength = 7.2f;
            float rHeight = 3.2f;
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
            ceiling.transform.position = new Vector3(rCenter.x, ceilY + 0.1f, rCenter.z);
            ceiling.transform.localScale = new Vector3(rWidth, 0.2f, rLength);
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
            backWallLeft.transform.position = new Vector3(rCenter.x - 2.4f, floorY + rHeight * 0.5f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallLeft.transform.localScale = new Vector3(2.4f, rHeight, 0.2f);
            backWallLeft.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject backWallRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWallRight.name = "Wall_Back_Right";
            backWallRight.transform.SetParent(roomRoot.transform);
            backWallRight.transform.position = new Vector3(rCenter.x + 2.4f, floorY + rHeight * 0.5f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallRight.transform.localScale = new Vector3(2.4f, rHeight, 0.2f);
            backWallRight.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject backWallUnder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWallUnder.name = "Wall_Back_UnderWindow";
            backWallUnder.transform.SetParent(roomRoot.transform);
            backWallUnder.transform.position = new Vector3(rCenter.x, floorY + 0.5f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallUnder.transform.localScale = new Vector3(2.4f, 1.0f, 0.2f);
            backWallUnder.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject backWallOver = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backWallOver.name = "Wall_Back_OverWindow";
            backWallOver.transform.SetParent(roomRoot.transform);
            backWallOver.transform.position = new Vector3(rCenter.x, floorY + 2.75f, rCenter.z + rLength * 0.5f + 0.1f);
            backWallOver.transform.localScale = new Vector3(2.4f, 0.9f, 0.2f);
            backWallOver.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject windowGlass = GameObject.CreatePrimitive(PrimitiveType.Cube);
            windowGlass.name = "Window_Glass";
            windowGlass.transform.SetParent(roomRoot.transform);
            windowGlass.transform.position = new Vector3(rCenter.x, floorY + 1.65f, rCenter.z + rLength * 0.5f + 0.05f);
            windowGlass.transform.localScale = new Vector3(2.3f, 1.3f, 0.04f);
            Material glassMat = new Material(litShaderMat);
            glassMat.SetColor("_BaseColor", new Color(0.6f, 0.85f, 0.95f, 0.45f));
            glassMat.SetFloat("_Surface", 1f);
            windowGlass.GetComponent<Renderer>().sharedMaterial = glassMat;

            GameObject blindsRoot = new GameObject("Window_Blinds_Mechanism");
            blindsRoot.transform.SetParent(roomRoot.transform);
            blindsRoot.transform.position = new Vector3(rCenter.x, floorY + 1.65f, rCenter.z + rLength * 0.5f - 0.05f);

            List<Transform> slatList = new List<Transform>();
            int slatCount = 9;
            float slatSpacing = 1.3f / slatCount;
            float slatStartOffset = -0.65f + slatSpacing * 0.5f;

            for (int i = 0; i < slatCount; i++)
            {
                GameObject slat = GameObject.CreatePrimitive(PrimitiveType.Cube);
                slat.name = "Slat_" + i;
                slat.transform.SetParent(blindsRoot.transform);
                slat.transform.localPosition = new Vector3(0f, slatStartOffset + i * slatSpacing, 0f);
                slat.transform.localScale = new Vector3(2.26f, 0.018f, 0.12f);
                slat.GetComponent<Renderer>().sharedMaterial = blindsMat;
                Object.DestroyImmediate(slat.GetComponent<Collider>());
                slatList.Add(slat.transform);
            }

            BoxCollider blindsTrigger = blindsRoot.AddComponent<BoxCollider>();
            blindsTrigger.size = new Vector3(2.3f, 1.35f, 0.35f);
            blindsTrigger.isTrigger = true;

            WindowBlinds blindsComp = blindsRoot.AddComponent<WindowBlinds>();
            blindsComp.slats = slatList.ToArray();
            blindsComp.isOpen = true;

            GameObject leftWallFront = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWallFront.name = "Wall_Left_Front";
            leftWallFront.transform.SetParent(roomRoot.transform);
            leftWallFront.transform.position = new Vector3(rCenter.x - rWidth * 0.5f - 0.1f, floorY + rHeight * 0.5f, rCenter.z - 2.1f);
            leftWallFront.transform.localScale = new Vector3(0.2f, rHeight, 3.0f);
            leftWallFront.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject leftWallBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWallBack.name = "Wall_Left_Back";
            leftWallBack.transform.SetParent(roomRoot.transform);
            leftWallBack.transform.position = new Vector3(rCenter.x - rWidth * 0.5f - 0.1f, floorY + rHeight * 0.5f, rCenter.z + 2.1f);
            leftWallBack.transform.localScale = new Vector3(0.2f, rHeight, 3.0f);
            leftWallBack.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject leftWallOverDoor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWallOverDoor.name = "Wall_Left_OverDoor";
            leftWallOverDoor.transform.SetParent(roomRoot.transform);
            leftWallOverDoor.transform.position = new Vector3(rCenter.x - rWidth * 0.5f - 0.1f, floorY + 2.7f, rCenter.z);
            leftWallOverDoor.transform.localScale = new Vector3(0.2f, 1.0f, 1.2f);
            leftWallOverDoor.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject doorFrame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorFrame.name = "Entrance_DoorFrame";
            doorFrame.transform.SetParent(roomRoot.transform);
            doorFrame.transform.position = new Vector3(rCenter.x - rWidth * 0.5f, floorY + 1.1f, rCenter.z);
            doorFrame.transform.localScale = new Vector3(0.18f, 2.2f, 1.16f);
            doorFrame.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(doorFrame.GetComponent<Collider>());

            GameObject doorPivot = new GameObject("Entrance_Door");
            doorPivot.transform.SetParent(roomRoot.transform);
            doorPivot.transform.position = new Vector3(rCenter.x - rWidth * 0.5f, floorY, rCenter.z + 0.52f);

            GameObject doorLeaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            doorLeaf.name = "Door_Leaf";
            doorLeaf.transform.SetParent(doorPivot.transform);
            doorLeaf.transform.localPosition = new Vector3(0f, 1.05f, -0.52f);
            doorLeaf.transform.localScale = new Vector3(0.08f, 2.1f, 1.04f);
            doorLeaf.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject doorKnob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            doorKnob.name = "Knob";
            doorKnob.transform.SetParent(doorLeaf.transform);
            doorKnob.transform.localPosition = new Vector3(0.55f, 0f, -0.4f);
            doorKnob.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            doorKnob.GetComponent<Renderer>().sharedMaterial = handleMat;
            Object.DestroyImmediate(doorKnob.GetComponent<Collider>());

            EntranceDoor doorComp = doorPivot.AddComponent<EntranceDoor>();
            doorComp.doorLeaf = doorPivot.transform;
            doorComp.isOpen = false;

            GameObject ceilingLightObj = new GameObject("Room_Ceiling_Light");
            ceilingLightObj.transform.SetParent(roomRoot.transform);
            ceilingLightObj.transform.position = new Vector3(rCenter.x, floorY + rHeight - 0.25f, rCenter.z);
            Light ceilingLight = ceilingLightObj.AddComponent<Light>();
            ceilingLight.type = LightType.Point;
            ceilingLight.range = 10f;
            ceilingLight.intensity = 2.4f;
            ceilingLight.color = new Color(1.0f, 0.95f, 0.88f);

            GameObject lampLightObj = new GameObject("Desk_Lamp_Light");
            lampLightObj.transform.SetParent(roomRoot.transform);
            lampLightObj.transform.position = new Vector3(rCenter.x + 2.5f, floorY + 1.25f, rCenter.z + 1.3f);
            Light tableLampLight = lampLightObj.AddComponent<Light>();
            tableLampLight.type = LightType.Point;
            tableLampLight.range = 4.0f;
            tableLampLight.intensity = 1.8f;
            tableLampLight.color = new Color(1.0f, 0.92f, 0.72f);

            GameObject switchObj = new GameObject("Light_Switch");
            switchObj.transform.SetParent(roomRoot.transform);
            switchObj.transform.position = new Vector3(rCenter.x - rWidth * 0.5f + 0.08f, floorY + 1.4f, rCenter.z + 0.85f);

            GameObject switchPlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            switchPlate.name = "Switch_Plate";
            switchPlate.transform.SetParent(switchObj.transform);
            switchPlate.transform.localPosition = Vector3.zero;
            switchPlate.transform.localScale = new Vector3(0.04f, 0.16f, 0.12f);
            switchPlate.GetComponent<Renderer>().sharedMaterial = wallMat;

            GameObject switchLever = GameObject.CreatePrimitive(PrimitiveType.Cube);
            switchLever.name = "Switch_Lever";
            switchLever.transform.SetParent(switchObj.transform);
            switchLever.transform.localPosition = new Vector3(0.025f, 0f, 0f);
            switchLever.transform.localScale = new Vector3(0.035f, 0.06f, 0.03f);
            switchLever.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(switchLever.GetComponent<Collider>());

            LightSwitch switchComp = switchObj.AddComponent<LightSwitch>();
            switchComp.toggleLever = switchLever.transform;
            switchComp.targetLights = new Light[] { ceilingLight, tableLampLight };
            switchComp.isOn = true;

            GameObject fanRoot = new GameObject("Ceiling_Fan");
            fanRoot.transform.SetParent(roomRoot.transform);
            fanRoot.transform.position = new Vector3(rCenter.x + 2.1f, floorY + rHeight - 0.15f, rCenter.z + 2.1f);

            GameObject fanRod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fanRod.name = "Fan_Rod";
            fanRod.transform.SetParent(fanRoot.transform);
            fanRod.transform.localPosition = new Vector3(0f, -0.15f, 0f);
            fanRod.transform.localScale = new Vector3(0.04f, 0.15f, 0.04f);
            fanRod.GetComponent<Renderer>().sharedMaterial = woodMat;
            Object.DestroyImmediate(fanRod.GetComponent<Collider>());

            GameObject fanHub = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fanHub.name = "Fan_Hub";
            fanHub.transform.SetParent(fanRoot.transform);
            fanHub.transform.localPosition = new Vector3(0f, -0.32f, 0f);
            fanHub.transform.localScale = new Vector3(0.32f, 0.05f, 0.32f);
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
                blade.transform.localPosition = Quaternion.Euler(0f, bAngle, 0f) * new Vector3(0.5f, 0f, 0f);
                blade.transform.localRotation = Quaternion.Euler(6f, bAngle, 0f);
                blade.transform.localScale = new Vector3(0.65f, 0.015f, 0.14f);
                blade.GetComponent<Renderer>().sharedMaterial = woodMat;
                Object.DestroyImmediate(blade.GetComponent<Collider>());
            }

            BoxCollider fanTrigger = fanRoot.AddComponent<BoxCollider>();
            fanTrigger.size = new Vector3(1.5f, 1.0f, 1.5f);
            fanTrigger.center = new Vector3(0f, -0.35f, 0f);
            fanTrigger.isTrigger = true;

            FanController fanComp = fanRoot.AddComponent<FanController>();
            fanComp.blades = fanBlades.transform;
            fanComp.windOrigin = fanRoot.transform;
            fanComp.windRadius = 3.2f;
            fanComp.windForce = 25f;
            fanComp.isSpinning = false;

            GameObject deskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/desk.fbx");
            if (deskPrefab != null)
            {
                SpawnProp(deskPrefab, new Vector3(rCenter.x + 2.2f, floorY, rCenter.z + 2.1f), Quaternion.Euler(0f, 180f, 0f), 0.20f, "Desk", 40f, roomRoot.transform);
            }

            GameObject chairDeskPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/chairDesk.fbx");
            if (chairDeskPrefab != null)
            {
                SpawnProp(chairDeskPrefab, new Vector3(rCenter.x + 1.25f, floorY, rCenter.z + 2.1f), Quaternion.Euler(0f, 45f, 0f), 0.25f, "Chair_Office", 10f, roomRoot.transform);
            }

            GameObject coffeeTablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/tableCoffee.fbx");
            if (coffeeTablePrefab != null)
            {
                SpawnProp(coffeeTablePrefab, new Vector3(rCenter.x - 0.8f, floorY, rCenter.z + 1.5f), Quaternion.identity, 0.20f, "Table_Coffee", 16f, roomRoot.transform);
            }

            GameObject chairPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/chair.fbx");
            if (chairPrefab != null)
            {
                SpawnProp(chairPrefab, new Vector3(rCenter.x - 1.8f, floorY, rCenter.z + 1.5f), Quaternion.Euler(0f, 90f, 0f), 0.20f, "Chair_Side_1", 8f, roomRoot.transform);
                SpawnProp(chairPrefab, new Vector3(rCenter.x + 0.2f, floorY, rCenter.z + 1.5f), Quaternion.Euler(0f, -90f, 0f), 0.20f, "Chair_Side_2", 8f, roomRoot.transform);
            }

            GameObject cabinetRoot = new GameObject("Cabinet_With_Doors");
            cabinetRoot.transform.SetParent(roomRoot.transform);
            cabinetRoot.transform.position = new Vector3(rCenter.x, floorY, rCenter.z - rLength * 0.5f + 0.4f);

            GameObject cabBack = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabBack.name = "Cabinet_Back";
            cabBack.transform.SetParent(cabinetRoot.transform);
            cabBack.transform.localPosition = new Vector3(0f, 1.0f, -0.22f);
            cabBack.transform.localScale = new Vector3(1.6f, 2.0f, 0.05f);
            cabBack.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabLeft = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabLeft.name = "Cabinet_Left";
            cabLeft.transform.SetParent(cabinetRoot.transform);
            cabLeft.transform.localPosition = new Vector3(-0.775f, 1.0f, 0f);
            cabLeft.transform.localScale = new Vector3(0.05f, 2.0f, 0.49f);
            cabLeft.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabRight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabRight.name = "Cabinet_Right";
            cabRight.transform.SetParent(cabinetRoot.transform);
            cabRight.transform.localPosition = new Vector3(0.775f, 1.0f, 0f);
            cabRight.transform.localScale = new Vector3(0.05f, 2.0f, 0.49f);
            cabRight.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabTop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabTop.name = "Cabinet_Top";
            cabTop.transform.SetParent(cabinetRoot.transform);
            cabTop.transform.localPosition = new Vector3(0f, 1.975f, 0f);
            cabTop.transform.localScale = new Vector3(1.6f, 0.05f, 0.49f);
            cabTop.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabBottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabBottom.name = "Cabinet_Bottom";
            cabBottom.transform.SetParent(cabinetRoot.transform);
            cabBottom.transform.localPosition = new Vector3(0f, 0.025f, 0f);
            cabBottom.transform.localScale = new Vector3(1.6f, 0.05f, 0.49f);
            cabBottom.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabShelf1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabShelf1.name = "Shelf_1";
            cabShelf1.transform.SetParent(cabinetRoot.transform);
            cabShelf1.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            cabShelf1.transform.localScale = new Vector3(1.5f, 0.04f, 0.45f);
            cabShelf1.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject cabShelf2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cabShelf2.name = "Shelf_2";
            cabShelf2.transform.SetParent(cabinetRoot.transform);
            cabShelf2.transform.localPosition = new Vector3(0f, 1.25f, 0f);
            cabShelf2.transform.localScale = new Vector3(1.5f, 0.04f, 0.45f);
            cabShelf2.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject leftDoorPivot = new GameObject("LeftDoor_Pivot");
            leftDoorPivot.transform.SetParent(cabinetRoot.transform);
            leftDoorPivot.transform.localPosition = new Vector3(-0.76f, 0f, 0.25f);

            GameObject leftDoorLeaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftDoorLeaf.name = "LeftDoor_Leaf";
            leftDoorLeaf.transform.SetParent(leftDoorPivot.transform);
            leftDoorLeaf.transform.localPosition = new Vector3(0.375f, 1.0f, 0f);
            leftDoorLeaf.transform.localScale = new Vector3(0.75f, 1.92f, 0.04f);
            leftDoorLeaf.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject leftDoorKnob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leftDoorKnob.name = "Knob";
            leftDoorKnob.transform.SetParent(leftDoorLeaf.transform);
            leftDoorKnob.transform.localPosition = new Vector3(0.42f, 0f, 0.55f);
            leftDoorKnob.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            leftDoorKnob.GetComponent<Renderer>().sharedMaterial = handleMat;
            Object.DestroyImmediate(leftDoorKnob.GetComponent<Collider>());

            GameObject rightDoorPivot = new GameObject("RightDoor_Pivot");
            rightDoorPivot.transform.SetParent(cabinetRoot.transform);
            rightDoorPivot.transform.localPosition = new Vector3(0.76f, 0f, 0.25f);

            GameObject rightDoorLeaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightDoorLeaf.name = "RightDoor_Leaf";
            rightDoorLeaf.transform.SetParent(rightDoorPivot.transform);
            rightDoorLeaf.transform.localPosition = new Vector3(-0.375f, 1.0f, 0f);
            rightDoorLeaf.transform.localScale = new Vector3(0.75f, 1.92f, 0.04f);
            rightDoorLeaf.GetComponent<Renderer>().sharedMaterial = woodMat;

            GameObject rightDoorKnob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightDoorKnob.name = "Knob";
            rightDoorKnob.transform.SetParent(rightDoorLeaf.transform);
            rightDoorKnob.transform.localPosition = new Vector3(-0.42f, 0f, 0.55f);
            rightDoorKnob.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            rightDoorKnob.GetComponent<Renderer>().sharedMaterial = handleMat;
            Object.DestroyImmediate(rightDoorKnob.GetComponent<Collider>());

            BoxCollider cabTrigger = cabinetRoot.AddComponent<BoxCollider>();
            cabTrigger.size = new Vector3(1.8f, 2.1f, 1.2f);
            cabTrigger.center = new Vector3(0f, 1.0f, 0.4f);
            cabTrigger.isTrigger = true;

            CabinetDoor cabDoorComp = cabinetRoot.AddComponent<CabinetDoor>();
            cabDoorComp.leftDoor = leftDoorPivot.transform;
            cabDoorComp.rightDoor = rightDoorPivot.transform;
            cabDoorComp.isOpen = false;

            float deskTopY = floorY + 0.77f;
            float shelf1Y = floorY + 0.67f;
            float shelf2Y = floorY + 1.27f;
            float coffeeTableTopY = floorY + 0.46f;
            float cabZ = rCenter.z - rLength * 0.5f + 0.4f;

            GameObject screenPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/computerScreen.fbx");
            if (screenPrefab != null)
            {
                SpawnProp(screenPrefab, new Vector3(rCenter.x + 2.5f, deskTopY, rCenter.z + 2.1f), Quaternion.Euler(0f, -90f, 0f), 0.15f, "Screen", 0f, roomRoot.transform);
            }

            GameObject kbPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/computerKeyboard.fbx");
            if (kbPrefab != null)
            {
                SpawnProp(kbPrefab, new Vector3(rCenter.x + 2.15f, deskTopY, rCenter.z + 2.1f), Quaternion.Euler(0f, -90f, 0f), 0.15f, "Keyboard", 0f, roomRoot.transform);
            }

            GameObject mousePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/computerMouse.fbx");
            if (mousePrefab != null)
            {
                SpawnProp(mousePrefab, new Vector3(rCenter.x + 2.15f, deskTopY, rCenter.z + 1.85f), Quaternion.Euler(0f, -90f, 0f), 0.15f, "Mouse", 0f, roomRoot.transform);
            }

            GameObject lampTablePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/lampSquareTable.fbx");
            if (lampTablePrefab != null)
            {
                SpawnProp(lampTablePrefab, new Vector3(rCenter.x + 2.5f, deskTopY, rCenter.z + 1.4f), Quaternion.identity, 0.15f, "Lamp", 0f, roomRoot.transform);
            }

            GameObject laptopPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/laptop.fbx");
            if (laptopPrefab != null)
            {
                SpawnPickup(laptopPrefab, new Vector3(rCenter.x + 1.9f, deskTopY, rCenter.z + 2.5f), Quaternion.Euler(0f, -60f, 0f), 0.06f, "Ноутбук", 1.5f, roomRoot.transform);
            }

            GameObject booksPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/books.fbx");
            if (booksPrefab != null)
            {
                SpawnPickup(booksPrefab, new Vector3(rCenter.x + 2.45f, deskTopY, rCenter.z + 2.6f), Quaternion.identity, 0.24f, "Книги на столе", 1.0f, roomRoot.transform);
                SpawnPickup(booksPrefab, new Vector3(rCenter.x - 0.35f, shelf2Y, cabZ), Quaternion.identity, 0.22f, "Книги в шкафу (верх)", 0.8f, roomRoot.transform, cabDoorComp);
                SpawnPickup(booksPrefab, new Vector3(rCenter.x + 0.35f, shelf2Y, cabZ), Quaternion.identity, 0.22f, "Книги в шкафу (верх 2)", 0.8f, roomRoot.transform, cabDoorComp);
            }

            GameObject plant2Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/plantSmall2.fbx");
            if (plant2Prefab != null)
            {
                SpawnPickup(plant2Prefab, new Vector3(rCenter.x - 0.35f, shelf1Y, cabZ), Quaternion.identity, 0.12f, "Цветок в шкафу", 0.9f, roomRoot.transform, cabDoorComp);
            }

            GameObject boxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/cardboardBoxClosed.fbx");
            if (boxPrefab != null)
            {
                SpawnPickup(boxPrefab, new Vector3(rCenter.x + 0.35f, shelf1Y, cabZ), Quaternion.identity, 0.15f, "Коробка в шкафу", 1.2f, roomRoot.transform, cabDoorComp);
                SpawnPickup(boxPrefab, new Vector3(rCenter.x + 2.4f, floorY, rCenter.z - 1.5f), Quaternion.Euler(0f, 25f, 0f), 0.22f, "Большая коробка", 2.2f, roomRoot.transform);
            }

            GameObject radioPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/radio.fbx");
            if (radioPrefab != null)
            {
                SpawnPickup(radioPrefab, new Vector3(rCenter.x - 0.6f, coffeeTableTopY, rCenter.z + 1.5f), Quaternion.identity, 0.10f, "Радиоприемник", 1.1f, roomRoot.transform);
            }

            GameObject plant1Prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/plantSmall1.fbx");
            if (plant1Prefab != null)
            {
                SpawnPickup(plant1Prefab, new Vector3(rCenter.x - 1.0f, coffeeTableTopY, rCenter.z + 1.5f), Quaternion.identity, 0.12f, "Цветок на столике", 1.0f, roomRoot.transform);
            }

            GameObject bearPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/bear.fbx");
            if (bearPrefab != null)
            {
                SpawnPickup(bearPrefab, new Vector3(rCenter.x - 1.8f, floorY + 0.45f, rCenter.z + 1.5f), Quaternion.Euler(0f, 90f, 0f), 0.10f, "Плюшевый мишка", 0.7f, roomRoot.transform);
            }

            GameObject trashPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/CityAssets/FurnitureKit/Models/FBX format/trashcan.fbx");
            if (trashPrefab != null)
            {
                SpawnPickup(trashPrefab, new Vector3(rCenter.x + 2.5f, floorY, rCenter.z + 0.9f), Quaternion.identity, 0.05f, "Корзина для бумаг", 1.0f, roomRoot.transform);
            }

            GameObject cityPortalObj = new GameObject("Portal_City_To_Room");
            cityPortalObj.transform.position = new Vector3(-8.5f, 0.02f, -3.5f);

            GameObject cityPortalDisk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cityPortalDisk.name = "Blue_Glowing_Zone";
            cityPortalDisk.transform.SetParent(cityPortalObj.transform);
            cityPortalDisk.transform.localPosition = Vector3.zero;
            cityPortalDisk.transform.localScale = new Vector3(1.8f, 0.02f, 1.8f);
            cityPortalDisk.GetComponent<Renderer>().sharedMaterial = portalMat;
            Object.DestroyImmediate(cityPortalDisk.GetComponent<Collider>());

            GameObject cityLightObj = new GameObject("Portal_Glow_Light");
            cityLightObj.transform.SetParent(cityPortalObj.transform);
            cityLightObj.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            Light cityLight = cityLightObj.AddComponent<Light>();
            cityLight.type = LightType.Point;
            cityLight.range = 3.2f;
            cityLight.intensity = 2.2f;
            cityLight.color = new Color(0.1f, 0.7f, 1.0f);

            SphereCollider cityTrigger = cityPortalObj.AddComponent<SphereCollider>();
            cityTrigger.radius = 1.0f;
            cityTrigger.isTrigger = true;

            GameObject roomSpawnPoint = new GameObject("SpawnPoint_Room");
            roomSpawnPoint.transform.SetParent(roomRoot.transform);
            roomSpawnPoint.transform.position = new Vector3(rCenter.x - 0.8f, floorY + 0.2f, rCenter.z);
            roomSpawnPoint.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            TeleportZone cityPortalComp = cityPortalObj.AddComponent<TeleportZone>();
            cityPortalComp.targetPoint = roomSpawnPoint.transform;
            cityPortalComp.glowLight = cityLight;
            cityPortalComp.ringRenderer = cityPortalDisk.GetComponent<Renderer>();

            GameObject roomPortalObj = new GameObject("Portal_Room_To_City");
            roomPortalObj.transform.SetParent(roomRoot.transform);
            roomPortalObj.transform.position = new Vector3(rCenter.x - 2.8f, floorY + 0.02f, rCenter.z);

            GameObject roomPortalDisk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            roomPortalDisk.name = "Blue_Glowing_Zone";
            roomPortalDisk.transform.SetParent(roomPortalObj.transform);
            roomPortalDisk.transform.localPosition = Vector3.zero;
            roomPortalDisk.transform.localScale = new Vector3(1.6f, 0.02f, 1.6f);
            roomPortalDisk.GetComponent<Renderer>().sharedMaterial = portalMat;
            Object.DestroyImmediate(roomPortalDisk.GetComponent<Collider>());

            GameObject roomLightObj = new GameObject("Portal_Glow_Light");
            roomLightObj.transform.SetParent(roomPortalObj.transform);
            roomLightObj.transform.localPosition = new Vector3(0f, 0.4f, 0f);
            Light roomLight = roomLightObj.AddComponent<Light>();
            roomLight.type = LightType.Point;
            roomLight.range = 3.0f;
            roomLight.intensity = 2.0f;
            roomLight.color = new Color(0.1f, 0.7f, 1.0f);

            SphereCollider roomTrigger = roomPortalObj.AddComponent<SphereCollider>();
            roomTrigger.radius = 0.9f;
            roomTrigger.isTrigger = true;

            GameObject citySpawnPoint = new GameObject("SpawnPoint_City");
            citySpawnPoint.transform.position = new Vector3(-7.2f, 0.5f, -3.5f);
            citySpawnPoint.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

            TeleportZone roomPortalComp = roomPortalObj.AddComponent<TeleportZone>();
            roomPortalComp.targetPoint = citySpawnPoint.transform;
            roomPortalComp.glowLight = roomLight;
            roomPortalComp.ringRenderer = roomPortalDisk.GetComponent<Renderer>();

            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                PlayerInteraction pi = player.GetComponent<PlayerInteraction>();
                if (pi == null) pi = player.AddComponent<PlayerInteraction>();
            }
        }

        private static void SpawnProp(GameObject prefab, Vector3 bottomCenterPos, Quaternion rot, float scale, string name, float mass, Transform parent)
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

            BoxCollider col = wrapper.AddComponent<BoxCollider>();
            col.center = new Vector3(0f, b.size.y * scale * 0.5f, 0f);
            col.size = b.size * scale;

            if (mass > 0.01f)
            {
                Rigidbody rb = wrapper.AddComponent<Rigidbody>();
                rb.mass = mass;
                rb.linearDamping = 2.5f;
                rb.angularDamping = 2.5f;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
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
            rb.linearDamping = 0.8f;
            rb.angularDamping = 0.8f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            PickupableItem item = wrapper.AddComponent<PickupableItem>();
            item.itemName = name;
            item.parentCabinetDoor = cabDoor;
        }
    }
}
