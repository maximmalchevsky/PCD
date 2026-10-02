using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace CityEditor
{
    public static class AssetPreparer
    {
        [MenuItem("City/Prepare All Assets")]
        public static void PrepareAll()
        {
            PrepareTextures();
            PrepareMaterials();
            PrepareAnimations();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("All Assets Prepared Successfully!");
        }

        private static void PrepareTextures()
        {
            string steveTexPath = "Assets/CityAssets/Characters/Steve/textures/Diffuse.png";
            TextureImporter ti = AssetImporter.GetAtPath(steveTexPath) as TextureImporter;
            if (ti != null)
            {
                ti.filterMode = FilterMode.Point;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                ti.SaveAndReimport();
            }

            string[] colorMaps = new string[]
            {
                "Assets/CityAssets/Commercial/Textures/colormap.png",
                "Assets/CityAssets/TrainKit/Textures/colormap.png",
                "Assets/CityAssets/Vehicles/Models/FBX format/Textures/colormap.png",
                "Assets/CityAssets/Environment/Models/FBX format/Textures/colormap.png"
            };

            foreach (string p in colorMaps)
            {
                TextureImporter cti = AssetImporter.GetAtPath(p) as TextureImporter;
                if (cti != null)
                {
                    cti.filterMode = FilterMode.Point;
                    cti.wrapMode = TextureWrapMode.Clamp;
                    cti.textureCompression = TextureImporterCompression.Uncompressed;
                    cti.SaveAndReimport();
                }
            }
        }

        private static void PrepareMaterials()
        {
            Shader litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null) litShader = Shader.Find("Standard");

            Material steveMat = GetOrCreateMaterial("Assets/CityAssets/Characters/Steve/Mat_Steve.mat", litShader);
            Texture2D steveTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Steve/textures/Diffuse.png");
            if (steveTex != null)
            {
                steveMat.SetTexture("_BaseMap", steveTex);
                steveMat.SetColor("_BaseColor", Color.white);
            }
            steveMat.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(steveMat);

            Material shrekHead = GetOrCreateMaterial("Assets/CityAssets/Characters/Shrek/Mat_Shrek_Head.mat", litShader);
            Texture2D headTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Shrek/textures/shrekhead.jpeg");
            Texture2D headNrm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Shrek/textures/ShrekHead_Nrm.png");
            if (headTex != null) shrekHead.SetTexture("_BaseMap", headTex);
            if (headNrm != null) shrekHead.SetTexture("_BumpMap", headNrm);
            shrekHead.SetColor("_BaseColor", Color.white);
            shrekHead.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(shrekHead);

            Material shrekBody = GetOrCreateMaterial("Assets/CityAssets/Characters/Shrek/Mat_Shrek_Body.mat", litShader);
            Texture2D bodyTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Shrek/textures/shrekbody.png");
            Texture2D bodyNrm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Shrek/textures/ShrekBody_Nrm.png");
            if (bodyTex != null) shrekBody.SetTexture("_BaseMap", bodyTex);
            if (bodyNrm != null) shrekBody.SetTexture("_BumpMap", bodyNrm);
            shrekBody.SetColor("_BaseColor", Color.white);
            shrekBody.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(shrekBody);

            Material shrekEyes = GetOrCreateMaterial("Assets/CityAssets/Characters/Shrek/Mat_Shrek_Eyes.mat", litShader);
            Texture2D eyesTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Shrek/textures/shrekball.jpeg");
            if (eyesTex != null) shrekEyes.SetTexture("_BaseMap", eyesTex);
            shrekEyes.SetColor("_BaseColor", Color.white);
            shrekEyes.SetFloat("_Smoothness", 0.5f);
            EditorUtility.SetDirty(shrekEyes);

            Material capyMat = GetOrCreateMaterial("Assets/CityAssets/Characters/Capybara/Mat_Capybara.mat", litShader);
            Texture2D capyTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Capybara/textures/Capybara_mat_baseColor_1.jpeg");
            Texture2D capyNrm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/Capybara/textures/Capybara_mat_normal_scale2_norm_0.jpeg");
            if (capyTex != null) capyMat.SetTexture("_BaseMap", capyTex);
            if (capyNrm != null) capyMat.SetTexture("_BumpMap", capyNrm);
            capyMat.SetColor("_BaseColor", Color.white);
            capyMat.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(capyMat);

            Material tungMat = GetOrCreateMaterial("Assets/CityAssets/Characters/TungTung/Mat_TungTung.mat", litShader);
            Texture2D tungTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/TungTung/textures/Material_ALB.png");
            Texture2D tungNrm = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Characters/TungTung/textures/Material_NRM.png");
            if (tungTex != null) tungMat.SetTexture("_BaseMap", tungTex);
            if (tungNrm != null) tungMat.SetTexture("_BumpMap", tungNrm);
            tungMat.SetColor("_BaseColor", Color.white);
            tungMat.SetFloat("_Smoothness", 0.2f);
            EditorUtility.SetDirty(tungMat);

            Material amongBody = GetOrCreateMaterial("Assets/CityAssets/Characters/AmongUs/Mat_AmongUs_Body.mat", litShader);
            amongBody.SetColor("_BaseColor", new Color(0.85f, 0.08f, 0.08f));
            amongBody.SetFloat("_Smoothness", 0.3f);
            EditorUtility.SetDirty(amongBody);

            Material amongVisor = GetOrCreateMaterial("Assets/CityAssets/Characters/AmongUs/Mat_AmongUs_Visor.mat", litShader);
            amongVisor.SetColor("_BaseColor", new Color(0.2f, 0.75f, 0.95f));
            amongVisor.SetFloat("_Smoothness", 0.85f);
            EditorUtility.SetDirty(amongVisor);

            SetupEmissiveMat("Assets/CityAssets/Environment/Mat_Traffic_Red_On.mat", litShader, new Color(1f, 0.1f, 0.1f), true);
            SetupEmissiveMat("Assets/CityAssets/Environment/Mat_Traffic_Red_Off.mat", litShader, new Color(0.25f, 0.05f, 0.05f), false);
            SetupEmissiveMat("Assets/CityAssets/Environment/Mat_Traffic_Yellow_On.mat", litShader, new Color(1f, 0.85f, 0.1f), true);
            SetupEmissiveMat("Assets/CityAssets/Environment/Mat_Traffic_Yellow_Off.mat", litShader, new Color(0.25f, 0.22f, 0.05f), false);
            SetupEmissiveMat("Assets/CityAssets/Environment/Mat_Traffic_Green_On.mat", litShader, new Color(0.1f, 1f, 0.25f), true);
            SetupEmissiveMat("Assets/CityAssets/Environment/Mat_Traffic_Green_Off.mat", litShader, new Color(0.05f, 0.25f, 0.08f), false);

            Material tlPole = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Traffic_Pole.mat", litShader);
            tlPole.SetColor("_BaseColor", new Color(0.22f, 0.24f, 0.26f));
            tlPole.SetFloat("_Metallic", 0.75f);
            tlPole.SetFloat("_Smoothness", 0.4f);
            EditorUtility.SetDirty(tlPole);

            Material tlHousing = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Traffic_Housing.mat", litShader);
            tlHousing.SetColor("_BaseColor", new Color(0.11f, 0.12f, 0.13f));
            tlHousing.SetFloat("_Metallic", 0.1f);
            tlHousing.SetFloat("_Smoothness", 0.2f);
            EditorUtility.SetDirty(tlHousing);

            Material tlZebra = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Traffic_Zebra.mat", litShader);
            tlZebra.SetColor("_BaseColor", new Color(0.92f, 0.93f, 0.95f));
            tlZebra.SetFloat("_Metallic", 0.0f);
            tlZebra.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(tlZebra);

            Material darkFoliage = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Foliage_Dark.mat", litShader);
            darkFoliage.SetColor("_BaseColor", new Color(0.12f, 0.42f, 0.16f));
            darkFoliage.SetFloat("_Smoothness", 0.05f);
            EditorUtility.SetDirty(darkFoliage);

            Material lightFoliage = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Foliage_Light.mat", litShader);
            lightFoliage.SetColor("_BaseColor", new Color(0.20f, 0.58f, 0.22f));
            lightFoliage.SetFloat("_Smoothness", 0.05f);
            EditorUtility.SetDirty(lightFoliage);

            Material treeTrunk = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Tree_Trunk.mat", litShader);
            treeTrunk.SetColor("_BaseColor", new Color(0.32f, 0.20f, 0.12f));
            treeTrunk.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(treeTrunk);

            Material sidewalkMat = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Sidewalk.mat", litShader);
            sidewalkMat.color = new Color(0.65f, 0.67f, 0.70f);
            sidewalkMat.SetFloat("_Smoothness", 0.1f);
            EditorUtility.SetDirty(sidewalkMat);

            Material commMat = GetOrCreateMaterial("Assets/CityAssets/Commercial/Mat_Commercial.mat", litShader);
            Texture2D commTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Commercial/Textures/colormap.png");
            if (commTex != null) commMat.SetTexture("_BaseMap", commTex);
            commMat.SetFloat("_Smoothness", 0.12f);
            EditorUtility.SetDirty(commMat);

            Material trainMat = GetOrCreateMaterial("Assets/CityAssets/TrainKit/Mat_TrainKit.mat", litShader);
            Texture2D trainTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/TrainKit/Textures/colormap.png");
            if (trainTex != null) trainMat.SetTexture("_BaseMap", trainTex);
            trainMat.SetFloat("_Smoothness", 0.15f);
            EditorUtility.SetDirty(trainMat);

            Material vehMat = GetOrCreateMaterial("Assets/CityAssets/Vehicles/Mat_Vehicles.mat", litShader);
            Texture2D vehTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Vehicles/Models/FBX format/Textures/colormap.png");
            if (vehTex != null) vehMat.SetTexture("_BaseMap", vehTex);
            vehMat.SetFloat("_Smoothness", 0.35f);
            EditorUtility.SetDirty(vehMat);

            Material envMat = GetOrCreateMaterial("Assets/CityAssets/Environment/Mat_Environment.mat", litShader);
            Texture2D envTex = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CityAssets/Environment/Models/FBX format/Textures/colormap.png");
            if (envTex != null) envMat.SetTexture("_BaseMap", envTex);
            envMat.SetFloat("_Smoothness", 0.15f);
            EditorUtility.SetDirty(envMat);
        }

        private static Material GetOrCreateMaterial(string path, Shader shader)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            {
                mat = new Material(shader);
                AssetDatabase.CreateAsset(mat, path);
            }
            mat.shader = shader;
            return mat;
        }

        private static void SetupEmissiveMat(string path, Shader shader, Color col, bool isEmissive)
        {
            Material m = GetOrCreateMaterial(path, shader);
            m.SetColor("_BaseColor", col);
            if (isEmissive)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", col * 2.5f);
            }
            else
            {
                m.DisableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", Color.black);
            }
            m.SetFloat("_Smoothness", 0.5f);
            EditorUtility.SetDirty(m);
        }

        private static void PrepareAnimations()
        {
            CreateLoopingController(
                "Assets/CityAssets/Characters/Shrek/source/Walking.fbx",
                "mixamo.com",
                "Assets/CityAssets/Characters/Shrek/Shrek_WalkLoop.anim",
                "Assets/CityAssets/Characters/Shrek/Shrek_Controller.controller"
            );

            CreateLoopingController(
                "Assets/CityAssets/Characters/Steve/source/The Perfect Steve Rigged/Steve/model/FBX/Steve Rigged.fbx",
                "Armature.001|Walk",
                "Assets/CityAssets/Characters/Steve/Steve_WalkLoop.anim",
                "Assets/CityAssets/Characters/Steve/Steve_Controller.controller"
            );

            CreateLoopingController(
                "Assets/CityAssets/Characters/DropoutBear/source/TESTBear.fbx",
                "Take 001",
                "Assets/CityAssets/Characters/DropoutBear/Bear_WalkLoop.anim",
                "Assets/CityAssets/Characters/DropoutBear/Bear_Controller.controller"
            );

            CreateTungTungWalkController(
                "Assets/CityAssets/Characters/TungTung/TungTung_Controller.controller",
                "Assets/CityAssets/Characters/TungTung/TungTung_WalkLoop.anim"
            );
        }

        private static void CreateLoopingController(string fbxPath, string clipName, string animPath, string controllerPath)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            AnimationClip sourceClip = null;
            foreach (var a in assets)
            {
                if (a is AnimationClip && a.name == clipName)
                {
                    sourceClip = a as AnimationClip;
                    break;
                }
            }
            if (sourceClip == null) return;

            AnimationClip loopClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            if (loopClip == null)
            {
                loopClip = Object.Instantiate(sourceClip);
                loopClip.name = sourceClip.name + "_Loop";
                AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(loopClip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(loopClip, settings);
                AssetDatabase.CreateAsset(loopClip, animPath);
            }

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                AnimatorStateMachine sm = controller.layers[0].stateMachine;
                AnimatorState st = sm.AddState("Walk");
                st.motion = loopClip;
                sm.defaultState = st;
                EditorUtility.SetDirty(controller);
            }
        }

        private static void CreateTungTungWalkController(string controllerPath, string animPath)
        {
            AnimationClip walkClip = new AnimationClip();
            walkClip.name = "TungTung_WalkLoop";

            Keyframe[] leftThighKeys = new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 160f),
                new Keyframe(0.25f, 24f, 0f, 0f),
                new Keyframe(0.5f, 0f, -160f, -160f),
                new Keyframe(0.75f, -24f, 0f, 0f),
                new Keyframe(1f, 0f, 160f, 0f)
            };
            AnimationCurve leftThighCurve = new AnimationCurve(leftThighKeys);

            Keyframe[] rightThighKeys = new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, -160f),
                new Keyframe(0.25f, -24f, 0f, 0f),
                new Keyframe(0.5f, 0f, 160f, 160f),
                new Keyframe(0.75f, 24f, 0f, 0f),
                new Keyframe(1f, 0f, -160f, 0f)
            };
            AnimationCurve rightThighCurve = new AnimationCurve(rightThighKeys);

            Keyframe[] leftKneeKeys = new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 0f),
                new Keyframe(0.25f, 0f, 0f, 0f),
                new Keyframe(0.5f, 0f, 0f, 80f),
                new Keyframe(0.75f, 22f, 0f, 0f),
                new Keyframe(1f, 0f, -80f, 0f)
            };
            AnimationCurve leftKneeCurve = new AnimationCurve(leftKneeKeys);

            Keyframe[] rightKneeKeys = new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 80f),
                new Keyframe(0.25f, 22f, 0f, 0f),
                new Keyframe(0.5f, 0f, -80f, 0f),
                new Keyframe(0.75f, 0f, 0f, 0f),
                new Keyframe(1f, 0f, 0f, 0f)
            };
            AnimationCurve rightKneeCurve = new AnimationCurve(rightKneeKeys);

            Keyframe[] leftArmKeys = new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, -140f),
                new Keyframe(0.25f, -20f, 0f, 0f),
                new Keyframe(0.5f, 0f, 140f, 140f),
                new Keyframe(0.75f, 20f, 0f, 0f),
                new Keyframe(1f, 0f, -140f, 0f)
            };
            AnimationCurve leftArmCurve = new AnimationCurve(leftArmKeys);

            Keyframe[] rightArmKeys = new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, 140f),
                new Keyframe(0.25f, 20f, 0f, 0f),
                new Keyframe(0.5f, 0f, -140f, -140f),
                new Keyframe(0.75f, -20f, 0f, 0f),
                new Keyframe(1f, 0f, 140f, 0f)
            };
            AnimationCurve rightArmCurve = new AnimationCurve(rightArmKeys);

            Keyframe[] spineSwayKeys = new Keyframe[]
            {
                new Keyframe(0f, 0f, 0f, -25f),
                new Keyframe(0.25f, -3.5f, 0f, 0f),
                new Keyframe(0.5f, 0f, 25f, 25f),
                new Keyframe(0.75f, 3.5f, 0f, 0f),
                new Keyframe(1f, 0f, -25f, 0f)
            };
            AnimationCurve spineSwayCurve = new AnimationCurve(spineSwayKeys);

            walkClip.SetCurve("metarig/spine/thigh.L", typeof(Transform), "localEulerAnglesRaw.x", leftThighCurve);
            walkClip.SetCurve("metarig/spine/thigh.R", typeof(Transform), "localEulerAnglesRaw.x", rightThighCurve);
            walkClip.SetCurve("metarig/spine/thigh.L/shin.L", typeof(Transform), "localEulerAnglesRaw.x", leftKneeCurve);
            walkClip.SetCurve("metarig/spine/thigh.R/shin.R", typeof(Transform), "localEulerAnglesRaw.x", rightKneeCurve);
            walkClip.SetCurve("metarig/spine/spine.001/spine.002/spine.003/upper_arm.L", typeof(Transform), "localEulerAnglesRaw.x", leftArmCurve);
            walkClip.SetCurve("metarig/spine/spine.001/spine.002/spine.003/upper_arm.R", typeof(Transform), "localEulerAnglesRaw.x", rightArmCurve);
            walkClip.SetCurve("metarig/spine", typeof(Transform), "localEulerAnglesRaw.z", spineSwayCurve);

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(walkClip);
            settings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(walkClip, settings);

            if (AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath) != null)
            {
                AssetDatabase.DeleteAsset(animPath);
            }
            AssetDatabase.CreateAsset(walkClip, animPath);

            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (controller == null)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
                AnimatorStateMachine sm = controller.layers[0].stateMachine;
                AnimatorState st = sm.AddState("Walk");
                st.motion = walkClip;
                sm.defaultState = st;
            }
            else
            {
                AnimatorStateMachine sm = controller.layers[0].stateMachine;
                if (sm.states.Length > 0)
                {
                    sm.states[0].state.motion = walkClip;
                }
                else
                {
                    AnimatorState st = sm.AddState("Walk");
                    st.motion = walkClip;
                    sm.defaultState = st;
                }
            }
            EditorUtility.SetDirty(controller);
        }
    }
}
