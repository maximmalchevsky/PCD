using System.Collections.Generic;
using UnityEngine;

namespace Task12_16
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class DiggableEarth : MonoBehaviour
    {
        public static DiggableEarth Instance { get; private set; }

        public int gridResolutionX = 48;
        public int gridResolutionZ = 48;
        public float sizeX = 16f;
        public float sizeZ = 16f;
        public float maxDigDepth = -3.2f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private Mesh terrainMesh;

        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uvs;
        private int[] triangles;

        private float minLocalX;
        private float maxLocalX;
        private float minLocalZ;
        private float maxLocalZ;
        private float dx;
        private float dz;

        private bool isMeshDirty = false;
        private float colliderUpdateTimer = 0f;
        private const float COLLIDER_UPDATE_INTERVAL = 0.06f;

        public float accumulatedSoilVolume = 0f;
        public const float VOLUME_PER_SOIL_CHUNK = 0.12f;

        public Transform cargoRoot;
        public Material matDirt;
        public Material matRock;

        private int chunkSpawnCounter = 0;

        void Awake()
        {
            Instance = this;
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null) meshCollider = gameObject.AddComponent<MeshCollider>();

            BuildInitialTerrainGrid();
        }

        public void BuildInitialTerrainGrid()
        {
            minLocalX = -sizeX * 0.5f;
            maxLocalX =  sizeX * 0.5f;
            minLocalZ = -sizeZ * 0.5f;
            maxLocalZ =  sizeZ * 0.5f;

            dx = sizeX / gridResolutionX;
            dz = sizeZ / gridResolutionZ;

            int numVerts = (gridResolutionX + 1) * (gridResolutionZ + 1);
            vertices = new Vector3[numVerts];
            normals = new Vector3[numVerts];
            uvs = new Vector2[numVerts];

            int vertIdx = 0;
            for (int z = 0; z <= gridResolutionZ; z++)
            {
                float pz = minLocalZ + z * dz;
                for (int x = 0; x <= gridResolutionX; x++)
                {
                    float px = minLocalX + x * dx;

                    // Initial ground contour:
                    // Natural excavation trench slope towards center-east where excavator works
                    float distFromCenter = Mathf.Sqrt(px * px + pz * pz);
                    float edgeFade = Mathf.Clamp01((sizeX * 0.5f - Mathf.Abs(px)) / 2.0f) *
                                     Mathf.Clamp01((sizeZ * 0.5f - Mathf.Abs(pz)) / 2.0f);

                    float trenchDepth = 0f;
                    // Trench depression between x: -5 to +2, z: -3 to +5
                    if (px > -5.5f && px < 2.5f && pz > -3.5f && pz < 5.5f)
                    {
                        float nx = Mathf.InverseLerp(-5.5f, 2.5f, px);
                        float nz = Mathf.InverseLerp(-3.5f, 5.5f, pz);
                        float trenchBowl = Mathf.Sin(nx * Mathf.PI) * Mathf.Sin(nz * Mathf.PI);
                        trenchDepth = -0.35f * trenchBowl;
                    }

                    // Add subtle natural earth surface texture
                    float naturalSoilWave = Mathf.Sin(px * 1.2f) * Mathf.Cos(pz * 1.2f) * 0.04f;
                    float initialY = (trenchDepth + naturalSoilWave) * edgeFade;

                    vertices[vertIdx] = new Vector3(px, initialY, pz);
                    normals[vertIdx] = Vector3.up;
                    uvs[vertIdx] = new Vector2((float)x / gridResolutionX * 4f, (float)z / gridResolutionZ * 4f);
                    vertIdx++;
                }
            }

            int numQuads = gridResolutionX * gridResolutionZ;
            triangles = new int[numQuads * 6];
            int triIdx = 0;

            for (int z = 0; z < gridResolutionZ; z++)
            {
                for (int x = 0; x < gridResolutionX; x++)
                {
                    int row1 = z * (gridResolutionX + 1);
                    int row2 = (z + 1) * (gridResolutionX + 1);

                    int v0 = row1 + x;
                    int v1 = row1 + x + 1;
                    int v2 = row2 + x + 1;
                    int v3 = row2 + x;

                    triangles[triIdx++] = v0;
                    triangles[triIdx++] = v3;
                    triangles[triIdx++] = v1;

                    triangles[triIdx++] = v1;
                    triangles[triIdx++] = v3;
                    triangles[triIdx++] = v2;
                }
            }

            terrainMesh = new Mesh();
            terrainMesh.name = "Dynamic_Diggable_Earth";
            terrainMesh.vertices = vertices;
            terrainMesh.uv = uvs;
            terrainMesh.triangles = triangles;
            terrainMesh.RecalculateNormals();
            terrainMesh.RecalculateBounds();

            meshFilter.sharedMesh = terrainMesh;
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = terrainMesh;
        }

        void Update()
        {
            if (isMeshDirty)
            {
                colliderUpdateTimer += Time.deltaTime;
                if (colliderUpdateTimer >= COLLIDER_UPDATE_INTERVAL)
                {
                    ApplyMeshDeformation();
                    colliderUpdateTimer = 0f;
                }
            }
        }

        private void ApplyMeshDeformation()
        {
            if (terrainMesh == null || vertices == null) return;

            terrainMesh.vertices = vertices;
            terrainMesh.RecalculateNormals();
            terrainMesh.RecalculateBounds();

            // Refresh physical collision mesh
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = terrainMesh;

            isMeshDirty = false;
        }

        public float GetHeightAtLocal(float localX, float localZ)
        {
            if (vertices == null) return 0f;
            int x0 = Mathf.Clamp(Mathf.FloorToInt((localX - minLocalX) / dx), 0, gridResolutionX);
            int z0 = Mathf.Clamp(Mathf.FloorToInt((localZ - minLocalZ) / dz), 0, gridResolutionZ);
            int idx = z0 * (gridResolutionX + 1) + x0;
            return vertices[idx].y;
        }

        public float GetHeightAtWorld(Vector3 worldPoint)
        {
            Vector3 localPt = transform.InverseTransformPoint(worldPoint);
            float h = GetHeightAtLocal(localPt.x, localPt.z);
            return transform.TransformPoint(new Vector3(localPt.x, h, localPt.z)).y;
        }

        /// <summary>
        /// Carves soil at the specified world position downwards to targetWorldY.
        /// Returns true if soil was carved, along with volume removed in m3.
        /// </summary>
        public bool DigAt(Vector3 worldPoint, float radius, float targetWorldY, out float volumeRemoved)
        {
            volumeRemoved = 0f;
            if (vertices == null) return false;

            Vector3 localPt = transform.InverseTransformPoint(worldPoint);

            // Bounds check
            if (localPt.x < minLocalX - radius || localPt.x > maxLocalX + radius ||
                localPt.z < minLocalZ - radius || localPt.z > maxLocalZ + radius)
            {
                return false;
            }

            float currentLocalY = GetHeightAtLocal(localPt.x, localPt.z);
            float targetLocalY = transform.InverseTransformPoint(new Vector3(localPt.x, targetWorldY, localPt.z)).y;
            // Always ensure digging cuts into soil if bucket touches surface
            if (targetLocalY >= currentLocalY)
            {
                targetLocalY = currentLocalY - 0.12f;
            }
            targetLocalY = Mathf.Max(targetLocalY, maxDigDepth);

            int minX = Mathf.Clamp(Mathf.FloorToInt((localPt.x - radius - minLocalX) / dx), 0, gridResolutionX);
            int maxX = Mathf.Clamp(Mathf.CeilToInt((localPt.x + radius - minLocalX) / dx), 0, gridResolutionX);
            int minZ = Mathf.Clamp(Mathf.FloorToInt((localPt.z - radius - minLocalZ) / dz), 0, gridResolutionZ);
            int maxZ = Mathf.Clamp(Mathf.CeilToInt((localPt.z + radius - minLocalZ) / dz), 0, gridResolutionZ);

            float rSq = radius * radius;
            bool modified = false;

            for (int z = minZ; z <= maxZ; z++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    // Keep border perimeter anchored flush with site concrete/asphalt
                    if (x <= 1 || x >= gridResolutionX - 1 || z <= 1 || z >= gridResolutionZ - 1)
                    {
                        continue;
                    }

                    int idx = z * (gridResolutionX + 1) + x;
                    Vector3 v = vertices[idx];

                    float dX = v.x - localPt.x;
                    float dZ = v.z - localPt.z;
                    float distSq = dX * dX + dZ * dZ;

                    if (distSq <= rSq)
                    {
                        float dist = Mathf.Sqrt(distSq);
                        // Smooth spherical/hemispherical scoop profile
                        float falloff = Mathf.Cos((dist / radius) * Mathf.PI * 0.5f);
                        float desiredY = Mathf.Lerp(v.y, targetLocalY, falloff);

                        if (desiredY < v.y)
                        {
                            float deltaY = v.y - desiredY;
                            volumeRemoved += deltaY * dx * dz;
                            v.y = desiredY;
                            vertices[idx] = v;
                            modified = true;
                        }
                    }
                }
            }

            if (modified)
            {
                isMeshDirty = true;
                accumulatedSoilVolume += volumeRemoved;
            }

            return modified;
        }

        /// <summary>
        /// Levels or pushes soil (e.g. for bulldozer blade grading)
        /// </summary>
        public void GradeAt(Vector3 worldPoint, float radius, float targetWorldY)
        {
            if (vertices == null) return;
            Vector3 localPt = transform.InverseTransformPoint(worldPoint);
            float targetLocalY = transform.InverseTransformPoint(new Vector3(0f, targetWorldY, 0f)).y;

            int minX = Mathf.Clamp(Mathf.FloorToInt((localPt.x - radius - minLocalX) / dx), 0, gridResolutionX);
            int maxX = Mathf.Clamp(Mathf.CeilToInt((localPt.x + radius - minLocalX) / dx), 0, gridResolutionX);
            int minZ = Mathf.Clamp(Mathf.FloorToInt((localPt.z - radius - minLocalZ) / dz), 0, gridResolutionZ);
            int maxZ = Mathf.Clamp(Mathf.CeilToInt((localPt.z + radius - minLocalZ) / dz), 0, gridResolutionZ);

            float rSq = radius * radius;
            bool modified = false;

            for (int z = minZ; z <= maxZ; z++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (x <= 1 || x >= gridResolutionX - 1 || z <= 1 || z >= gridResolutionZ - 1) continue;

                    int idx = z * (gridResolutionX + 1) + x;
                    Vector3 v = vertices[idx];
                    float distSq = (v.x - localPt.x) * (v.x - localPt.x) + (v.z - localPt.z) * (v.z - localPt.z);
                    if (distSq <= rSq)
                    {
                        float falloff = 1f - Mathf.Sqrt(distSq) / radius;
                        v.y = Mathf.Lerp(v.y, targetLocalY, falloff * 0.25f);
                        vertices[idx] = v;
                        modified = true;
                    }
                }
            }

            if (modified) isMeshDirty = true;
        }

        /// <summary>
        /// Spawns an authentic faceted excavated soil chunk at the bucket tip.
        /// </summary>
        public GameObject SpawnExcavatedSoilChunk(Vector3 spawnWorldPos, Vector3 initialVelocity)
        {
            if (cargoRoot == null)
            {
                GameObject rootObj = GameObject.Find("Granular_Piles");
                if (rootObj != null) cargoRoot = rootObj.transform;
                else cargoRoot = transform;
            }

            chunkSpawnCounter++;
            int seed = chunkSpawnCounter * 37 + (int)(spawnWorldPos.x * 10f);

            string[] models = new string[] { "rock_smallA", "rock_smallB", "rock_smallC", "stone_smallA", "stone_smallB" };
            string chosenModel = models[Mathf.Abs(seed) % models.Length];
            GameObject prefab = Resources.Load<GameObject>(chosenModel);

            GameObject chunk = null;
            float chunkSize = Random.Range(0.38f, 0.54f);

            if (prefab != null)
            {
                chunk = Instantiate(prefab, cargoRoot);
                chunk.name = "Excavated_Soil_Chunk_" + chunkSpawnCounter;
                chunk.transform.position = spawnWorldPos;
                chunk.transform.rotation = Random.rotation;
                chunk.transform.localScale = Vector3.one * (chunkSize * 1.8f);

                Collider[] oldCols = chunk.GetComponentsInChildren<Collider>(true);
                for (int i = 0; i < oldCols.Length; i++) Destroy(oldCols[i]);

                Renderer[] rends = chunk.GetComponentsInChildren<Renderer>(true);
                Material dMat = matDirt != null ? matDirt : GetComponent<Renderer>().sharedMaterial;
                for (int i = 0; i < rends.Length; i++) rends[i].sharedMaterial = dMat;
            }
            else
            {
                chunk = new GameObject("Excavated_Soil_Chunk_" + chunkSpawnCounter);
                chunk.transform.SetParent(cargoRoot, true);
                chunk.transform.position = spawnWorldPos;
                chunk.transform.rotation = Random.rotation;

                MeshFilter mf = chunk.AddComponent<MeshFilter>();
                mf.sharedMesh = ConstructionMeshFactory.CreateSoilChunkMesh(seed, chunkSize);

                MeshRenderer mr = chunk.AddComponent<MeshRenderer>();
                mr.sharedMaterial = matDirt != null ? matDirt : GetComponent<Renderer>().sharedMaterial;
            }

            MeshCollider mc = chunk.GetComponentInChildren<MeshCollider>();
            if (mc == null) mc = chunk.AddComponent<MeshCollider>();
            mc.convex = true;

            PhysicsMaterial soilMat = new PhysicsMaterial("ExcavatedSoilPhysMat")
            {
                dynamicFriction = 0.90f,
                staticFriction = 0.98f,
                bounciness = 0.0f,
                frictionCombine = PhysicsMaterialCombine.Maximum,
                bounceCombine = PhysicsMaterialCombine.Minimum
            };
            mc.sharedMaterial = soilMat;

            Rigidbody rb = chunk.AddComponent<Rigidbody>();
            rb.mass = 1.8f;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 2.4f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.linearVelocity = initialVelocity;

            GranularItem gi = chunk.AddComponent<GranularItem>();
            gi.itemType = GranularItem.GranularType.Sand;

            Task8_11.PickupableItem pi = chunk.AddComponent<Task8_11.PickupableItem>();
            pi.itemName = "Ком вынутого грунта";
            pi.rb = rb;

            return chunk;
        }
    }
}
