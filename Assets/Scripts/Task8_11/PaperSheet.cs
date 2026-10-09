using UnityEngine;

namespace Task8_11
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class PaperSheet : MonoBehaviour
    {
        public float width = 0.21f;
        public float length = 0.297f;
        public float thickness = 0.0012f;
        public float archDepth = 0.007f;
        public float cornerCurl = 0.018f;
        public float flutterDrag = 0.22f;
        public float flutterLift = 0.065f;
        public float terminalVelocity = 1.35f;

        private Rigidbody rb;
        private float flutterSeed;
        private FanController roomFan;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.mass = 0.022f;
            rb.linearDamping = 0.8f;
            rb.angularDamping = 1.6f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            flutterSeed = Random.Range(0f, 100f);

            BuildCurvedPaperMesh();
        }

        void Start()
        {
            roomFan = Object.FindAnyObjectByType<FanController>();
        }

        private void BuildCurvedPaperMesh()
        {
            int nx = 8;
            int nz = 12;
            int numGrid = (nx + 1) * (nz + 1);

            Vector3[] vertices = new Vector3[numGrid * 2];
            Vector2[] uvs = new Vector2[numGrid * 2];

            for (int j = 0; j <= nz; j++)
            {
                float v = (float)j / nz;
                float z = (v - 0.5f) * length;

                for (int i = 0; i <= nx; i++)
                {
                    float u = (float)i / nx;
                    float x = (u - 0.5f) * width;

                    float arch = -Mathf.Sin(u * Mathf.PI) * archDepth;
                    float bow = -Mathf.Sin(v * Mathf.PI) * (archDepth * 0.45f);

                    float c1 = Mathf.Max(0f, (u - 0.65f) + (v - 0.65f) - 0.15f);
                    float curlTopRight = c1 * c1 * (cornerCurl * 4.5f);

                    float c2 = Mathf.Max(0f, (0.35f - u) + (0.35f - v) - 0.15f);
                    float curlBottomLeft = c2 * c2 * (cornerCurl * 2.2f);

                    float yTop = arch + bow + curlTopRight + curlBottomLeft;
                    float yBot = yTop - thickness;

                    int topIdx = j * (nx + 1) + i;
                    int botIdx = numGrid + topIdx;

                    vertices[topIdx] = new Vector3(x, yTop, z);
                    vertices[botIdx] = new Vector3(x, yBot, z);

                    uvs[topIdx] = new Vector2(u, v);
                    uvs[botIdx] = new Vector2(u, v);
                }
            }

            int topTrisCount = nx * nz * 6;
            int skirtTrisCount = (nx * 2 + nz * 2) * 6;
            int[] triangles = new int[topTrisCount * 2 + skirtTrisCount];
            int triOffset = 0;

            for (int j = 0; j < nz; j++)
            {
                for (int i = 0; i < nx; i++)
                {
                    int row1 = j * (nx + 1);
                    int row2 = (j + 1) * (nx + 1);

                    int v0 = row1 + i;
                    int v1 = row1 + i + 1;
                    int v2 = row2 + i + 1;
                    int v3 = row2 + i;

                    triangles[triOffset++] = v0;
                    triangles[triOffset++] = v2;
                    triangles[triOffset++] = v1;

                    triangles[triOffset++] = v0;
                    triangles[triOffset++] = v3;
                    triangles[triOffset++] = v2;

                    int b0 = numGrid + v0;
                    int b1 = numGrid + v1;
                    int b2 = numGrid + v2;
                    int b3 = numGrid + v3;

                    triangles[triOffset++] = b0;
                    triangles[triOffset++] = b1;
                    triangles[triOffset++] = b2;

                    triangles[triOffset++] = b0;
                    triangles[triOffset++] = b2;
                    triangles[triOffset++] = b3;
                }
            }

            for (int i = 0; i < nx; i++)
            {
                int tA = i;
                int tB = i + 1;
                int bA = numGrid + tA;
                int bB = numGrid + tB;

                triangles[triOffset++] = tA;
                triangles[triOffset++] = bB;
                triangles[triOffset++] = tB;

                triangles[triOffset++] = tA;
                triangles[triOffset++] = bA;
                triangles[triOffset++] = bB;

                int topRowA = nz * (nx + 1) + i;
                int topRowB = topRowA + 1;
                int botRowA = numGrid + topRowA;
                int botRowB = numGrid + topRowB;

                triangles[triOffset++] = topRowA;
                triangles[triOffset++] = topRowB;
                triangles[triOffset++] = botRowB;

                triangles[triOffset++] = topRowA;
                triangles[triOffset++] = botRowB;
                triangles[triOffset++] = botRowA;
            }

            for (int j = 0; j < nz; j++)
            {
                int tLeftA = j * (nx + 1);
                int tLeftB = (j + 1) * (nx + 1);
                int bLeftA = numGrid + tLeftA;
                int bLeftB = numGrid + tLeftB;

                triangles[triOffset++] = tLeftA;
                triangles[triOffset++] = tLeftB;
                triangles[triOffset++] = bLeftB;

                triangles[triOffset++] = tLeftA;
                triangles[triOffset++] = bLeftB;
                triangles[triOffset++] = bLeftA;

                int tRightA = j * (nx + 1) + nx;
                int tRightB = (j + 1) * (nx + 1) + nx;
                int bRightA = numGrid + tRightA;
                int bRightB = numGrid + tRightB;

                triangles[triOffset++] = tRightA;
                triangles[triOffset++] = bRightB;
                triangles[triOffset++] = tRightB;

                triangles[triOffset++] = tRightA;
                triangles[triOffset++] = bRightA;
                triangles[triOffset++] = bRightB;
            }

            Mesh mesh = new Mesh();
            mesh.name = "CurvedPaperMesh";
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            MeshFilter mf = GetComponent<MeshFilter>();
            if (mf != null)
            {
                mf.sharedMesh = mesh;
            }

            transform.localScale = Vector3.one;

            BoxCollider col = GetComponent<BoxCollider>();
            if (col != null)
            {
                col.center = new Vector3(0f, 0.003f, 0f);
                col.size = new Vector3(width, 0.015f, length);
            }
        }

        void FixedUpdate()
        {
            if (rb == null || rb.isKinematic) return;

            Vector3 vel = rb.linearVelocity;
            float speed = vel.magnitude;

            if (speed > 0.04f)
            {
                Vector3 normal = transform.up;
                float dot = Vector3.Dot(vel.normalized, normal);

                float faceResistance = dot * speed * speed * flutterDrag;
                rb.AddForce(-normal * faceResistance, ForceMode.Force);

                Vector3 planeVel = Vector3.ProjectOnPlane(vel, normal);
                float glideSpeed = planeVel.magnitude;
                if (glideSpeed > 0.08f)
                {
                    Vector3 liftDir = normal * (dot < 0f ? 1f : -1f);
                    rb.AddForce(liftDir * (glideSpeed * speed * flutterLift), ForceMode.Force);
                }

                if (vel.y < -0.15f)
                {
                    float flutterWave = Mathf.Sin(Time.time * 7.5f + flutterSeed) * 0.0035f;
                    rb.AddTorque(transform.forward * flutterWave + transform.right * (flutterWave * 0.4f), ForceMode.Force);
                }

                if (rb.linearVelocity.y < -terminalVelocity)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, -terminalVelocity, rb.linearVelocity.z);
                }
            }

            if (roomFan != null && roomFan.isSpinning)
            {
                Vector3 fanPos = roomFan.windOrigin != null ? roomFan.windOrigin.position : roomFan.transform.position;
                Vector3 toFan = rb.position - fanPos;
                float dist = toFan.magnitude;

                if (dist < roomFan.windRadius)
                {
                    Vector3 horiz = new Vector3(toFan.x, 0f, toFan.z);
                    Vector3 outDir = horiz.sqrMagnitude > 0.01f ? horiz.normalized : new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

                    float proximity = 1f - Mathf.Clamp01(dist / roomFan.windRadius);
                    Vector3 updraft = Vector3.up * (1.2f + proximity * 1.5f);
                    Vector3 swirl = Vector3.Cross(Vector3.up, outDir) * 0.8f;
                    Vector3 blast = (outDir * 2.2f + updraft + swirl) * (proximity * 0.85f);

                    rb.AddForce(blast, ForceMode.Force);
                    rb.AddTorque(Random.insideUnitSphere * 0.08f, ForceMode.Force);

                    if (rb.position.y > fanPos.y - 0.35f)
                    {
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, -0.6f, rb.linearVelocity.z);
                    }
                }
            }
        }
    }
}
