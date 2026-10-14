using System.Collections.Generic;
using UnityEngine;

namespace Task12_16
{
    public static class ConstructionMeshFactory
    {
        // =========================================================================
        // MESH BUILDER PRIMITIVE HELPERS
        // =========================================================================

        public static void AddBox(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris,
            Vector3 center, Vector3 size, Quaternion rot)
        {
            Vector3 half = size * 0.5f;
            Vector3[] localVerts = new Vector3[]
            {
                // Front (+Z)
                new Vector3(-half.x, -half.y,  half.z), new Vector3( half.x, -half.y,  half.z),
                new Vector3( half.x,  half.y,  half.z), new Vector3(-half.x,  half.y,  half.z),
                // Back (-Z)
                new Vector3( half.x, -half.y, -half.z), new Vector3(-half.x, -half.y, -half.z),
                new Vector3(-half.x,  half.y, -half.z), new Vector3( half.x,  half.y, -half.z),
                // Left (-X)
                new Vector3(-half.x, -half.y, -half.z), new Vector3(-half.x, -half.y,  half.z),
                new Vector3(-half.x,  half.y,  half.z), new Vector3(-half.x,  half.y, -half.z),
                // Right (+X)
                new Vector3( half.x, -half.y,  half.z), new Vector3( half.x, -half.y, -half.z),
                new Vector3( half.x,  half.y, -half.z), new Vector3( half.x,  half.y,  half.z),
                // Top (+Y)
                new Vector3(-half.x,  half.y,  half.z), new Vector3( half.x,  half.y,  half.z),
                new Vector3( half.x,  half.y, -half.z), new Vector3(-half.x,  half.y, -half.z),
                // Bottom (-Y)
                new Vector3(-half.x, -half.y, -half.z), new Vector3( half.x, -half.y, -half.z),
                new Vector3( half.x, -half.y,  half.z), new Vector3(-half.x, -half.y,  half.z)
            };

            Vector3[] localNorms = new Vector3[]
            {
                Vector3.forward,  Vector3.forward,  Vector3.forward,  Vector3.forward,
                Vector3.back,     Vector3.back,     Vector3.back,     Vector3.back,
                Vector3.left,     Vector3.left,     Vector3.left,     Vector3.left,
                Vector3.right,    Vector3.right,    Vector3.right,    Vector3.right,
                Vector3.up,       Vector3.up,       Vector3.up,       Vector3.up,
                Vector3.down,     Vector3.down,     Vector3.down,     Vector3.down
            };

            for (int f = 0; f < 6; f++)
            {
                int baseIdx = verts.Count;
                Vector3 n = rot * localNorms[f * 4];
                for (int v = 0; v < 4; v++)
                {
                    verts.Add(center + rot * localVerts[f * 4 + v]);
                    norms.Add(n);
                }
                uvs.Add(new Vector2(0f, 0f));
                uvs.Add(new Vector2(1f, 0f));
                uvs.Add(new Vector2(1f, 1f));
                uvs.Add(new Vector2(0f, 1f));

                tris.Add(baseIdx);
                tris.Add(baseIdx + 1);
                tris.Add(baseIdx + 2);
                tris.Add(baseIdx);
                tris.Add(baseIdx + 2);
                tris.Add(baseIdx + 3);
            }
        }

        public static Mesh LoadResourceMesh(string resourceName)
        {
            try
            {
                GameObject go = Resources.Load<GameObject>(resourceName);
                if (go != null)
                {
                    MeshFilter mf = go.GetComponentInChildren<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null) return mf.sharedMesh;
                }
                Mesh m = Resources.Load<Mesh>(resourceName);
                if (m != null) return m;
            }
            catch { }
            return null;
        }

        public static void AddBoxBetween(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris,
            Vector3 p1, Vector3 p2, float width, float thickness)
        {
            Vector3 delta = p2 - p1;
            float length = delta.magnitude;
            if (length < 0.0001f) return;

            Vector3 center = (p1 + p2) * 0.5f;
            Vector3 forward = delta / length;

            Vector3 up = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(forward, up)) > 0.95f) up = Vector3.forward;

            Vector3 right = Vector3.Cross(up, forward).normalized;
            up = Vector3.Cross(forward, right).normalized;

            Quaternion rot = Quaternion.LookRotation(forward, up);
            AddBox(verts, norms, uvs, tris, center, new Vector3(width, thickness, length), rot);
        }

        public static void AddCylinder(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris,
            Vector3 center, float radius, float height, int segments, Quaternion rot)
        {
            float halfH = height * 0.5f;
            float angleStep = (Mathf.PI * 2f) / segments;

            // Side wall
            int sideStart = verts.Count;
            for (int i = 0; i <= segments; i++)
            {
                float a = i * angleStep;
                float cos = Mathf.Cos(a);
                float sin = Mathf.Sin(a);
                Vector3 rad = new Vector3(cos * radius, 0f, sin * radius);
                Vector3 n = rot * new Vector3(cos, 0f, sin);

                verts.Add(center + rot * (rad + Vector3.up * halfH));
                norms.Add(n);
                uvs.Add(new Vector2((float)i / segments, 1f));

                verts.Add(center + rot * (rad - Vector3.up * halfH));
                norms.Add(n);
                uvs.Add(new Vector2((float)i / segments, 0f));
            }

            for (int i = 0; i < segments; i++)
            {
                int b = sideStart + i * 2;
                tris.Add(b);
                tris.Add(b + 2);
                tris.Add(b + 1);

                tris.Add(b + 1);
                tris.Add(b + 2);
                tris.Add(b + 3);
            }

            // Top Cap
            int topCenter = verts.Count;
            verts.Add(center + rot * (Vector3.up * halfH));
            norms.Add(rot * Vector3.up);
            uvs.Add(new Vector2(0.5f, 0.5f));

            int topRing = verts.Count;
            for (int i = 0; i < segments; i++)
            {
                float a = i * angleStep;
                Vector3 rad = new Vector3(Mathf.Cos(a) * radius, halfH, Mathf.Sin(a) * radius);
                verts.Add(center + rot * rad);
                norms.Add(rot * Vector3.up);
                uvs.Add(new Vector2(0.5f + Mathf.Cos(a) * 0.5f, 0.5f + Mathf.Sin(a) * 0.5f));
            }
            for (int i = 0; i < segments; i++)
            {
                tris.Add(topCenter);
                tris.Add(topRing + i);
                tris.Add(topRing + (i + 1) % segments);
            }

            // Bottom Cap
            int btmCenter = verts.Count;
            verts.Add(center + rot * (Vector3.down * halfH));
            norms.Add(rot * Vector3.down);
            uvs.Add(new Vector2(0.5f, 0.5f));

            int btmRing = verts.Count;
            for (int i = 0; i < segments; i++)
            {
                float a = i * angleStep;
                Vector3 rad = new Vector3(Mathf.Cos(a) * radius, -halfH, Mathf.Sin(a) * radius);
                verts.Add(center + rot * rad);
                norms.Add(rot * Vector3.down);
                uvs.Add(new Vector2(0.5f + Mathf.Cos(a) * 0.5f, 0.5f + Mathf.Sin(a) * 0.5f));
            }
            for (int i = 0; i < segments; i++)
            {
                tris.Add(btmCenter);
                tris.Add(btmRing + (i + 1) % segments);
                tris.Add(btmRing + i);
            }
        }

        public static void AddWedge(List<Vector3> verts, List<Vector3> norms, List<Vector2> uvs, List<int> tris,
            Vector3 center, Vector3 size, Quaternion rot)
        {
            Vector3 half = size * 0.5f;
            // A triangular wedge: base on -Y, slope rising towards +Z
            Vector3[] p = new Vector3[]
            {
                // Bottom
                new Vector3(-half.x, -half.y, -half.z), // 0
                new Vector3( half.x, -half.y, -half.z), // 1
                new Vector3( half.x, -half.y,  half.z), // 2
                new Vector3(-half.x, -half.y,  half.z), // 3
                // Top ridge at front (+Z, +Y)
                new Vector3(-half.x,  half.y,  half.z), // 4
                new Vector3( half.x,  half.y,  half.z)  // 5
            };

            void AddQuad(int a, int b, int c, int d)
            {
                int baseI = verts.Count;
                Vector3 va = center + rot * p[a];
                Vector3 vb = center + rot * p[b];
                Vector3 vc = center + rot * p[c];
                Vector3 vd = center + rot * p[d];
                Vector3 n = Vector3.Cross(vb - va, vc - va).normalized;

                verts.Add(va); norms.Add(n); uvs.Add(new Vector2(0, 0));
                verts.Add(vb); norms.Add(n); uvs.Add(new Vector2(1, 0));
                verts.Add(vc); norms.Add(n); uvs.Add(new Vector2(1, 1));
                verts.Add(vd); norms.Add(n); uvs.Add(new Vector2(0, 1));

                tris.Add(baseI); tris.Add(baseI + 1); tris.Add(baseI + 2);
                tris.Add(baseI); tris.Add(baseI + 2); tris.Add(baseI + 3);
            }

            void AddTri(int a, int b, int c)
            {
                int baseI = verts.Count;
                Vector3 va = center + rot * p[a];
                Vector3 vb = center + rot * p[b];
                Vector3 vc = center + rot * p[c];
                Vector3 n = Vector3.Cross(vb - va, vc - va).normalized;

                verts.Add(va); norms.Add(n); uvs.Add(new Vector2(0, 0));
                verts.Add(vb); norms.Add(n); uvs.Add(new Vector2(1, 0));
                verts.Add(vc); norms.Add(n); uvs.Add(new Vector2(0.5f, 1));

                tris.Add(baseI); tris.Add(baseI + 1); tris.Add(baseI + 2);
            }

            // Bottom
            AddQuad(0, 1, 2, 3);
            // Sloped face
            AddQuad(3, 2, 5, 4);
            // Back face
            AddQuad(1, 0, 4, 5);
            // Left triangle
            AddTri(0, 3, 4);
            // Right triangle
            AddTri(2, 1, 5);
        }

        // =========================================================================
        // 1. REALISTIC EXCAVATOR UNDERCARRIAGE MESH
        // =========================================================================
        public static Mesh CreateExcavatorUndercarriageMesh()
        {
            Mesh res = LoadResourceMesh("excavator-tracks");
            if (res != null) return res;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Center X-carbody chassis
            AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.44f, 0f), new Vector3(1.75f, 0.38f, 2.7f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.58f, 0f), new Vector3(2.2f, 0.16f, 1.6f), Quaternion.identity);

            // Slewing ring foundation ring
            AddCylinder(verts, norms, uvs, tris, new Vector3(0f, 0.70f, 0f), 0.95f, 0.18f, 24, Quaternion.identity);

            // Left & Right Track assemblies
            float[] trackX = new float[] { -1.35f, 1.35f };
            for (int t = 0; t < 2; t++)
            {
                float tx = trackX[t];

                // Side track frame beam
                AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.42f, 0f), new Vector3(0.68f, 0.52f, 3.8f), Quaternion.identity);

                // Front tension ramp frame
                AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.48f, 2.1f), new Vector3(0.66f, 0.42f, 0.9f), Quaternion.Euler(-25f, 0f, 0f));
                // Rear drive sprocket frame
                AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.48f, -2.1f), new Vector3(0.66f, 0.42f, 0.9f), Quaternion.Euler(25f, 0f, 0f));

                // Front Idler Wheel (large drum)
                AddCylinder(verts, norms, uvs, tris, new Vector3(tx, 0.42f, 2.22f), 0.34f, 0.60f, 18, Quaternion.Euler(0f, 0f, 90f));
                // Rear Drive Sprocket Wheel (with gear hub)
                AddCylinder(verts, norms, uvs, tris, new Vector3(tx, 0.42f, -2.22f), 0.34f, 0.60f, 18, Quaternion.Euler(0f, 0f, 90f));

                // 6 Lower Track Road Rollers per side
                for (int r = 0; r < 6; r++)
                {
                    float rz = -1.65f + r * 0.66f;
                    AddCylinder(verts, norms, uvs, tris, new Vector3(tx, 0.18f, rz), 0.16f, 0.56f, 12, Quaternion.Euler(0f, 0f, 90f));
                }

                // 2 Top Carrier Return Rollers per side
                AddCylinder(verts, norms, uvs, tris, new Vector3(tx, 0.68f, 0.85f), 0.12f, 0.54f, 12, Quaternion.Euler(0f, 0f, 90f));
                AddCylinder(verts, norms, uvs, tris, new Vector3(tx, 0.68f, -0.85f), 0.12f, 0.54f, 12, Quaternion.Euler(0f, 0f, 90f));

                // Continuous Track Tread Shoes (Top run, Bottom run, Front ramp, Rear ramp)
                AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.78f, 0f), new Vector3(0.72f, 0.08f, 4.1f), Quaternion.identity);
                AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.04f, 0f), new Vector3(0.72f, 0.08f, 4.1f), Quaternion.identity);
                AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.42f, 2.45f), new Vector3(0.72f, 0.08f, 0.85f), Quaternion.Euler(-55f, 0f, 0f));
                AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.42f, -2.45f), new Vector3(0.72f, 0.08f, 0.85f), Quaternion.Euler(55f, 0f, 0f));

                // Heavy Track Shoe Grouser Cleats (ribs along the tread)
                for (int g = 0; g < 14; g++)
                {
                    float gz = -1.95f + g * 0.30f;
                    AddBox(verts, norms, uvs, tris, new Vector3(tx, 0.83f, gz), new Vector3(0.70f, 0.035f, 0.06f), Quaternion.identity);
                    AddBox(verts, norms, uvs, tris, new Vector3(tx, -0.01f, gz), new Vector3(0.70f, 0.035f, 0.06f), Quaternion.identity);
                }

                // Side track guide skirts
                AddBox(verts, norms, uvs, tris, new Vector3(tx - 0.34f * (t == 0 ? 1f : -1f), 0.35f, 0f), new Vector3(0.04f, 0.28f, 3.4f), Quaternion.identity);
            }

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Excavator_Undercarriage";
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 2. REALISTIC EXCAVATOR CAB & UPPERSTRUCTURE MESH
        // =========================================================================
        public static Mesh CreateExcavatorCabMesh()
        {
            Mesh res = LoadResourceMesh("excavator-cab");
            if (res != null) return res;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Revolving deck platform
            AddBox(verts, norms, uvs, tris, new Vector3(0f, 0.12f, 0f), new Vector3(3.25f, 0.20f, 4.4f), Quaternion.identity);
            // Slewing ring top flange
            AddCylinder(verts, norms, uvs, tris, new Vector3(0f, 0.02f, 0f), 1.05f, 0.10f, 24, Quaternion.identity);

            // --- OPERATOR CABIN (FRONT-LEFT) ---
            Vector3 cabCenter = new Vector3(-0.96f, 1.25f, 0.70f);
            // Cab lower frame
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(0f, -0.75f, 0f), new Vector3(1.18f, 0.35f, 1.85f), Quaternion.identity);
            // Cab roof plate
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(0f, 0.92f, 0.05f), new Vector3(1.24f, 0.10f, 1.95f), Quaternion.identity);
            // Cab ROPS A-pillars (front corners)
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(-0.55f, 0.10f, 0.90f), new Vector3(0.08f, 1.6f, 0.08f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3( 0.55f, 0.10f, 0.90f), new Vector3(0.08f, 1.6f, 0.08f), Quaternion.identity);
            // Cab B-pillars (rear corners)
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(-0.55f, 0.10f, -0.85f), new Vector3(0.08f, 1.6f, 0.08f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3( 0.55f, 0.10f, -0.85f), new Vector3(0.08f, 1.6f, 0.08f), Quaternion.identity);

            // Overhead FOPS sun-visor & steel wire safety guard
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(0f, 0.98f, 0.75f), new Vector3(1.26f, 0.06f, 0.55f), Quaternion.identity);
            for (int g = 0; g < 4; g++)
            {
                AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(0f, 1.02f, 0.55f + g * 0.12f), new Vector3(1.20f, 0.03f, 0.03f), Quaternion.identity);
            }

            // Interior Operator Ergonomic Seat
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(0f, -0.28f, -0.20f), new Vector3(0.55f, 0.18f, 0.55f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(0f, 0.18f, -0.45f), new Vector3(0.52f, 0.70f, 0.15f), Quaternion.identity);
            // Seat Headrest
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(0f, 0.62f, -0.44f), new Vector3(0.32f, 0.20f, 0.12f), Quaternion.identity);
            // Pilot Joystick Pods (Left & Right)
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(-0.35f, -0.15f, 0.15f), new Vector3(0.18f, 0.38f, 0.35f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3( 0.35f, -0.15f, 0.15f), new Vector3(0.18f, 0.38f, 0.35f), Quaternion.identity);
            // Joystick handles
            AddCylinder(verts, norms, uvs, tris, cabCenter + new Vector3(-0.35f, 0.12f, 0.22f), 0.025f, 0.16f, 8, Quaternion.Euler(15f, 0f, 0f));
            AddCylinder(verts, norms, uvs, tris, cabCenter + new Vector3( 0.35f, 0.12f, 0.22f), 0.025f, 0.16f, 8, Quaternion.Euler(15f, 0f, 0f));

            // Operator Cab Door Handle & Mirror
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(-0.62f, 0.05f, 0.05f), new Vector3(0.03f, 0.06f, 0.22f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(-0.70f, 0.72f, 0.95f), new Vector3(0.04f, 0.32f, 0.16f), Quaternion.identity);

            // --- ENGINE HOUSING & BONNET (RIGHT & REAR) ---
            Vector3 engineCenter = new Vector3(0.68f, 1.15f, -0.15f);
            AddBox(verts, norms, uvs, tris, engineCenter, new Vector3(1.75f, 1.55f, 3.4f), Quaternion.identity);
            // Sloped engine bonnet hood
            AddBox(verts, norms, uvs, tris, engineCenter + new Vector3(0f, 0.75f, 0.20f), new Vector3(1.68f, 0.15f, 2.8f), Quaternion.Euler(-6f, 0f, 0f));

            // Radiator Louver Slits (side grilles)
            for (int l = 0; l < 7; l++)
            {
                float lz = -0.7f + l * 0.22f;
                AddBox(verts, norms, uvs, tris, engineCenter + new Vector3(0.88f, 0.15f, lz), new Vector3(0.04f, 0.10f, 0.14f), Quaternion.identity);
            }

            // Hydraulic oil tank and filters block (center-front)
            AddBox(verts, norms, uvs, tris, new Vector3(0.65f, 0.85f, 1.45f), new Vector3(1.5f, 0.95f, 0.75f), Quaternion.identity);

            // --- HEAVY SCULPTED REAR COUNTERWEIGHT ---
            Vector3 cwCenter = new Vector3(0f, 1.18f, -2.05f);
            AddBox(verts, norms, uvs, tris, cwCenter, new Vector3(3.22f, 1.65f, 0.85f), Quaternion.identity);
            // Curved bottom chamfer
            AddBox(verts, norms, uvs, tris, cwCenter + new Vector3(0f, -0.68f, -0.12f), new Vector3(3.18f, 0.35f, 0.70f), Quaternion.Euler(28f, 0f, 0f));
            // Rear view camera pod
            AddBox(verts, norms, uvs, tris, cwCenter + new Vector3(0f, 0.78f, -0.45f), new Vector3(0.24f, 0.15f, 0.12f), Quaternion.identity);

            // Turbocharged Engine Exhaust Stack Pipe
            AddCylinder(verts, norms, uvs, tris, new Vector3(1.15f, 2.30f, -1.25f), 0.12f, 0.75f, 16, Quaternion.identity);
            // Rain cap bevel on exhaust
            AddCylinder(verts, norms, uvs, tris, new Vector3(1.15f, 2.68f, -1.25f), 0.15f, 0.12f, 16, Quaternion.identity);

            // High-power LED Worklight Pods (Dual lights on cab roof)
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3(-0.25f, 0.96f, 1.05f), new Vector3(0.25f, 0.14f, 0.14f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, cabCenter + new Vector3( 0.25f, 0.96f, 1.05f), new Vector3(0.25f, 0.14f, 0.14f), Quaternion.identity);

            // Safety Walkway & Access Catwalk with Diamond Tread
            AddBox(verts, norms, uvs, tris, new Vector3(1.64f, 0.25f, 0f), new Vector3(0.35f, 0.08f, 3.8f), Quaternion.identity);

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Excavator_Cab";
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 3. REALISTIC ARCHED MONO-BOOM MESH
        // =========================================================================
        public static Mesh CreateExcavatorBoomMesh()
        {
            Mesh res = LoadResourceMesh("excavator-boom");
            if (res != null) return res;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Foot pin boss at (0, 0, 0)
            AddCylinder(verts, norms, uvs, tris, Vector3.zero, 0.20f, 0.52f, 18, Quaternion.Euler(0f, 0f, 90f));

            // Continuous curved mono-boom box beam segments:
            // Segment 1: Foot pin (0,0,0) -> Elbow arch (0, 1.8f, 1.2f)
            Vector3 pFoot = Vector3.zero;
            Vector3 pElbow = new Vector3(0f, 1.8f, 1.2f);
            AddBoxBetween(verts, norms, uvs, tris, pFoot, pElbow, 0.50f, 0.65f);

            // Segment 2: Elbow arch -> Tip pin (0, 2.6f, 3.2f)
            Vector3 pTip = new Vector3(0f, 2.6f, 3.2f);
            AddBoxBetween(verts, norms, uvs, tris, pElbow, pTip, 0.44f, 0.58f);

            // Tip pin boss at (0, 2.6f, 3.2f)
            AddCylinder(verts, norms, uvs, tris, pTip, 0.18f, 0.46f, 18, Quaternion.Euler(0f, 0f, 90f));

            // Stiffener flanges along the boom sides
            float[] sx = new float[] { -0.26f, 0.26f };
            for (int i = 0; i < 2; i++)
            {
                AddBoxBetween(verts, norms, uvs, tris, new Vector3(sx[i], 0.2f, 0.15f), new Vector3(sx[i], 1.7f, 1.2f), 0.03f, 0.45f);
                AddBoxBetween(verts, norms, uvs, tris, new Vector3(sx[i], 1.7f, 1.2f), new Vector3(sx[i], 2.5f, 3.1f), 0.03f, 0.42f);
            }

            // Boom top worklight
            AddBox(verts, norms, uvs, tris, new Vector3(0.30f, 2.2f, 1.8f), new Vector3(0.18f, 0.16f, 0.14f), Quaternion.identity);

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Excavator_Boom";
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 4. REALISTIC DIPPER STICK MESH
        // =========================================================================
        public static Mesh CreateExcavatorStickMesh()
        {
            Mesh res = LoadResourceMesh("excavator-stick");
            if (res != null) return res;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            Vector3 pTop = Vector3.zero;
            Vector3 pBtm = new Vector3(0f, -2.4f, 1.8f);

            // Top hinge pin boss at (0, 0, 0)
            AddCylinder(verts, norms, uvs, tris, pTop, 0.18f, 0.44f, 18, Quaternion.Euler(0f, 0f, 90f));

            // Main dipper stick box beam
            AddBoxBetween(verts, norms, uvs, tris, pTop, pBtm, 0.42f, 0.48f);

            // Bottom bucket hinge pin boss at (0, -2.4f, 1.8f)
            AddCylinder(verts, norms, uvs, tris, pBtm, 0.16f, 0.42f, 18, Quaternion.Euler(0f, 0f, 90f));

            // Underside wear runner plate
            Vector3 pTopWear = new Vector3(0f, -0.22f, -0.05f);
            Vector3 pBtmWear = new Vector3(0f, -2.48f, 1.65f);
            AddBoxBetween(verts, norms, uvs, tris, pTopWear, pBtmWear, 0.44f, 0.06f);

            // Hydraulic curl cylinder attached on stick back
            AddBoxBetween(verts, norms, uvs, tris, new Vector3(0f, 0.45f, 0.4f), new Vector3(0f, -0.95f, 1.45f), 0.16f, 0.16f);
            AddCylinder(verts, norms, uvs, tris, new Vector3(0f, -0.95f, 1.45f), 0.08f, 0.35f, 12, Quaternion.Euler(0f, 0f, 90f));

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Excavator_Stick";
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 5. REALISTIC HEAVY EXCAVATION BUCKET MESH
        // =========================================================================
        public static Mesh CreateExcavatorBucketMesh()
        {
            Mesh res = LoadResourceMesh("excavator-bucket");
            if (res != null) return res;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Bucket hinge pin boss at (0, 0, 0)
            AddCylinder(verts, norms, uvs, tris, Vector3.zero, 0.15f, 0.40f, 16, Quaternion.Euler(0f, 0f, 90f));

            // Curved back shell of bucket
            Vector3[] curve = new Vector3[]
            {
                new Vector3(0f,  0.08f, -0.22f),
                new Vector3(0f, -0.25f, -0.36f),
                new Vector3(0f, -0.58f, -0.15f),
                new Vector3(0f, -0.62f,  0.25f),
                new Vector3(0f, -0.52f,  0.65f),
                new Vector3(0f, -0.42f,  0.88f)
            };
            float width = 1.55f;
            for (int i = 0; i < curve.Length - 1; i++)
            {
                AddBoxBetween(verts, norms, uvs, tris, curve[i], curve[i + 1], width, 0.08f);
            }

            // Left & Right cheek plates
            float[] cheeks = new float[] { -width * 0.5f - 0.02f, width * 0.5f + 0.02f };
            for (int c = 0; c < 2; c++)
            {
                float cx = cheeks[c];
                AddBoxBetween(verts, norms, uvs, tris, new Vector3(cx, -0.25f, 0.05f), new Vector3(cx, -0.25f, 0.55f), 0.05f, 0.70f);
                AddBoxBetween(verts, norms, uvs, tris, new Vector3(cx, -0.45f, 0.65f), new Vector3(cx, -0.45f, 0.88f), 0.07f, 0.16f);
            }

            // Front cutting lip
            AddBoxBetween(verts, norms, uvs, tris, new Vector3(0f, -0.42f, 0.82f), new Vector3(0f, -0.42f, 0.92f), width + 0.06f, 0.10f);

            // 5 Digging Teeth on front lip
            for (int t = 0; t < 5; t++)
            {
                float tx = -0.60f + t * 0.30f;
                Vector3 pRoot = new Vector3(tx, -0.42f, 0.90f);
                Vector3 pToothTip = new Vector3(tx, -0.46f, 1.15f);
                AddBoxBetween(verts, norms, uvs, tris, pRoot, pToothTip, 0.12f, 0.12f);
            }

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Excavator_Bucket";
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 6. REALISTIC RED CLAY BRICK MESH (ГОСТ 530-2012)
        // =========================================================================
        public static Mesh CreateClayBrickMesh()
        {
            Mesh res = LoadResourceMesh("brick-gost");
            if (res != null) return res;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Proportional brick dimensions: length 0.48m, height 0.22m, width 0.28m
            float lx = 0.48f;
            float ly = 0.22f;
            float lz = 0.28f;

            // Main body
            AddBox(verts, norms, uvs, tris, Vector3.zero, new Vector3(lx, ly, lz), Quaternion.identity);

            // Mortar frog depression on top face (+Y)
            AddBox(verts, norms, uvs, tris, new Vector3(0f, ly * 0.5f - 0.012f, 0f), new Vector3(lx * 0.72f, 0.024f, lz * 0.60f), Quaternion.identity);

            // Mortar frog depression on bottom face (-Y)
            AddBox(verts, norms, uvs, tris, new Vector3(0f, -ly * 0.5f + 0.012f, 0f), new Vector3(lx * 0.72f, 0.024f, lz * 0.60f), Quaternion.identity);

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Clay_Brick";
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 7. REALISTIC CONCRETE FOUNDATION BLOCK MESH (ФБС 9.4.6)
        // =========================================================================
        public static Mesh CreateConcreteBlockFBSMesh()
        {
            Mesh res = LoadResourceMesh("block-fbs");
            if (res != null) return res;

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            float lx = 0.96f;
            float ly = 0.46f;
            float lz = 0.44f;

            // Solid main block body
            AddBox(verts, norms, uvs, tris, Vector3.zero, new Vector3(lx, ly, lz), Quaternion.identity);

            // Interlocking end vertical mortar grooves (Left & Right ends)
            AddBox(verts, norms, uvs, tris, new Vector3(-lx * 0.5f + 0.02f, 0f, 0f), new Vector3(0.04f, ly * 0.85f, 0.16f), Quaternion.identity);
            AddBox(verts, norms, uvs, tris, new Vector3( lx * 0.5f - 0.02f, 0f, 0f), new Vector3(0.04f, ly * 0.85f, 0.16f), Quaternion.identity);

            // Steel rebar lifting loops (монтажные петли) on top face
            float[] loopX = new float[] { -0.28f, 0.28f };
            for (int l = 0; l < 2; l++)
            {
                Vector3 loopPos = new Vector3(loopX[l], ly * 0.5f + 0.06f, 0f);
                // Loop posts
                AddCylinder(verts, norms, uvs, tris, loopPos + new Vector3(-0.04f, -0.02f, 0f), 0.016f, 0.12f, 8, Quaternion.identity);
                AddCylinder(verts, norms, uvs, tris, loopPos + new Vector3( 0.04f, -0.02f, 0f), 0.016f, 0.12f, 8, Quaternion.identity);
                // Loop arch
                AddCylinder(verts, norms, uvs, tris, loopPos + new Vector3(0f, 0.04f, 0f), 0.016f, 0.08f, 8, Quaternion.Euler(0f, 0f, 90f));
            }

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Concrete_Block_FBS";
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 8. REALISTIC FACETED SOIL CLUMP MESH (NON-SPHERICAL IRREGULAR CLOD)
        // =========================================================================
        public static Mesh CreateSoilChunkMesh(int seed = 0, float baseSize = 0.45f)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed);

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Generate an irregular faceted polyhedron from an icosahedron with noise
            float phi = (1f + Mathf.Sqrt(5f)) * 0.5f;
            Vector3[] icoVerts = new Vector3[]
            {
                new Vector3(-1,  phi, 0).normalized, new Vector3( 1,  phi, 0).normalized,
                new Vector3(-1, -phi, 0).normalized, new Vector3( 1, -phi, 0).normalized,
                new Vector3(0, -1,  phi).normalized, new Vector3(0,  1,  phi).normalized,
                new Vector3(0, -1, -phi).normalized, new Vector3(0,  1, -phi).normalized,
                new Vector3( phi, 0, -1).normalized, new Vector3( phi, 0,  1).normalized,
                new Vector3(-phi, 0, -1).normalized, new Vector3(-phi, 0,  1).normalized
            };

            int[] icoTris = new int[]
            {
                0, 11, 5,   0, 5, 1,    0, 1, 7,    0, 7, 10,   0, 10, 11,
                1, 5, 9,    5, 11, 4,   11, 10, 2,  10, 7, 6,   7, 1, 8,
                3, 9, 4,    3, 4, 2,    3, 2, 6,    3, 6, 8,    3, 8, 9,
                4, 9, 5,    2, 4, 11,   6, 2, 10,   8, 6, 7,    9, 8, 1
            };

            // Displace vertices with organic pseudo-random noise
            Vector3[] noisyVerts = new Vector3[icoVerts.Length];
            for (int i = 0; i < icoVerts.Length; i++)
            {
                float radNoise = 1.0f + (Random.value - 0.5f) * 0.45f;
                Vector3 scaleFactors = new Vector3(
                    0.9f + (Random.value - 0.5f) * 0.35f,
                    0.7f + (Random.value - 0.5f) * 0.30f,
                    0.9f + (Random.value - 0.5f) * 0.35f
                );
                noisyVerts[i] = Vector3.Scale(icoVerts[i] * radNoise, scaleFactors) * (baseSize * 0.5f);
            }

            // Build flat-shaded faceted triangles (gives natural earthy clod facets)
            for (int t = 0; t < icoTris.Length; t += 3)
            {
                int baseI = verts.Count;
                Vector3 v0 = noisyVerts[icoTris[t]];
                Vector3 v1 = noisyVerts[icoTris[t + 1]];
                Vector3 v2 = noisyVerts[icoTris[t + 2]];
                Vector3 fn = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                verts.Add(v0); norms.Add(fn); uvs.Add(new Vector2(0f, 0f));
                verts.Add(v1); norms.Add(fn); uvs.Add(new Vector2(1f, 0f));
                verts.Add(v2); norms.Add(fn); uvs.Add(new Vector2(0.5f, 1f));

                tris.Add(baseI);
                tris.Add(baseI + 1);
                tris.Add(baseI + 2);
            }

            Random.state = oldState;

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Soil_Chunk_" + seed;
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 9. REALISTIC FRACTURED QUARRY ROCK MESH
        // =========================================================================
        public static Mesh CreateQuarryStoneMesh(int seed = 0, float baseSize = 0.65f)
        {
            Random.State oldState = Random.state;
            Random.InitState(seed + 100);

            List<Vector3> verts = new List<Vector3>();
            List<Vector3> norms = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();

            // Sharp cleavage planes: box with randomized chamfered corners
            Vector3 half = new Vector3(
                baseSize * (0.35f + Random.value * 0.25f),
                baseSize * (0.25f + Random.value * 0.20f),
                baseSize * (0.35f + Random.value * 0.25f)
            );

            // Generate 8 box vertices + 6 center displaced cap vertices
            Vector3[] p = new Vector3[14];
            p[0] = new Vector3(-half.x * (0.8f + Random.value * 0.4f), -half.y, -half.z * (0.8f + Random.value * 0.4f));
            p[1] = new Vector3( half.x * (0.8f + Random.value * 0.4f), -half.y, -half.z * (0.8f + Random.value * 0.4f));
            p[2] = new Vector3( half.x * (0.8f + Random.value * 0.4f), -half.y,  half.z * (0.8f + Random.value * 0.4f));
            p[3] = new Vector3(-half.x * (0.8f + Random.value * 0.4f), -half.y,  half.z * (0.8f + Random.value * 0.4f));

            p[4] = new Vector3(-half.x * (0.7f + Random.value * 0.4f),  half.y, -half.z * (0.7f + Random.value * 0.4f));
            p[5] = new Vector3( half.x * (0.7f + Random.value * 0.4f),  half.y, -half.z * (0.7f + Random.value * 0.4f));
            p[6] = new Vector3( half.x * (0.7f + Random.value * 0.4f),  half.y,  half.z * (0.7f + Random.value * 0.4f));
            p[7] = new Vector3(-half.x * (0.7f + Random.value * 0.4f),  half.y,  half.z * (0.7f + Random.value * 0.4f));

            p[8] = new Vector3(0f,  half.y * 1.35f, 0f); // Top apex
            p[9] = new Vector3(0f, -half.y * 1.25f, 0f); // Bottom apex
            p[10] = new Vector3(-half.x * 1.25f, 0f, 0f); // Left apex
            p[11] = new Vector3( half.x * 1.25f, 0f, 0f); // Right apex
            p[12] = new Vector3(0f, 0f,  half.z * 1.25f); // Front apex
            p[13] = new Vector3(0f, 0f, -half.z * 1.25f); // Back apex

            void AddRockTri(int a, int b, int c)
            {
                int baseI = verts.Count;
                Vector3 va = p[a];
                Vector3 vb = p[b];
                Vector3 vc = p[c];
                Vector3 fn = Vector3.Cross(vb - va, vc - va).normalized;

                verts.Add(va); norms.Add(fn); uvs.Add(new Vector2(0, 0));
                verts.Add(vb); norms.Add(fn); uvs.Add(new Vector2(1, 0));
                verts.Add(vc); norms.Add(fn); uvs.Add(new Vector2(0.5f, 1));

                tris.Add(baseI); tris.Add(baseI + 1); tris.Add(baseI + 2);
            }

            // Top facets
            AddRockTri(8, 4, 5); AddRockTri(8, 5, 6); AddRockTri(8, 6, 7); AddRockTri(8, 7, 4);
            // Bottom facets
            AddRockTri(9, 1, 0); AddRockTri(9, 2, 1); AddRockTri(9, 3, 2); AddRockTri(9, 0, 3);
            // Front facets
            AddRockTri(12, 3, 2); AddRockTri(12, 2, 6); AddRockTri(12, 6, 7); AddRockTri(12, 7, 3);
            // Back facets
            AddRockTri(13, 1, 0); AddRockTri(13, 0, 4); AddRockTri(13, 4, 5); AddRockTri(13, 5, 1);
            // Left facets
            AddRockTri(10, 0, 3); AddRockTri(10, 3, 7); AddRockTri(10, 7, 4); AddRockTri(10, 4, 0);
            // Right facets
            AddRockTri(11, 2, 1); AddRockTri(11, 1, 5); AddRockTri(11, 5, 6); AddRockTri(11, 6, 2);

            Random.state = oldState;

            Mesh mesh = new Mesh();
            mesh.name = "Mesh_Quarry_Stone_" + seed;
            mesh.SetVertices(verts);
            mesh.SetNormals(norms);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateBounds();
            return mesh;
        }

        // =========================================================================
        // 10. WAVEFRONT OBJ EXPORTER UTILITY
        // =========================================================================
        public static void ExportMeshToObj(Mesh mesh, string filePath)
        {
            if (mesh == null || string.IsNullOrEmpty(filePath)) return;
            string dir = System.IO.Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !System.IO.Directory.Exists(dir))
            {
                System.IO.Directory.CreateDirectory(dir);
            }

            using (var sw = new System.IO.StreamWriter(filePath))
            {
                sw.WriteLine("# Wavefront OBJ exported by ConstructionMeshFactory");
                sw.WriteLine("o " + mesh.name);
                Vector3[] verts = mesh.vertices;
                Vector3[] norms = mesh.normals;
                Vector2[] uvs = mesh.uv;
                int[] tris = mesh.triangles;

                for (int i = 0; i < verts.Length; i++)
                {
                    Vector3 v = verts[i];
                    sw.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "v {0:F4} {1:F4} {2:F4}", v.x, v.y, v.z));
                }
                for (int i = 0; i < norms.Length; i++)
                {
                    Vector3 n = norms[i];
                    sw.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "vn {0:F4} {1:F4} {2:F4}", n.x, n.y, n.z));
                }
                for (int i = 0; i < uvs.Length; i++)
                {
                    Vector2 uv = uvs[i];
                    sw.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "vt {0:F4} {1:F4}", uv.x, uv.y));
                }

                bool hasNormals = norms != null && norms.Length == verts.Length;
                bool hasUVs = uvs != null && uvs.Length == verts.Length;

                for (int i = 0; i < tris.Length; i += 3)
                {
                    int i0 = tris[i] + 1;
                    int i1 = tris[i + 1] + 1;
                    int i2 = tris[i + 2] + 1;

                    if (hasNormals && hasUVs)
                        sw.WriteLine(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", i0, i1, i2));
                    else if (hasNormals)
                        sw.WriteLine(string.Format("f {0}//{0} {1}//{1} {2}//{2}", i0, i1, i2));
                    else
                        sw.WriteLine(string.Format("f {0} {1} {2}", i0, i1, i2));
                }
            }
        }

        public static void ExportAllConstructionAssets(string folderPath)
        {
            ExportMeshToObj(CreateExcavatorUndercarriageMesh(), System.IO.Path.Combine(folderPath, "excavator-tracks.obj"));
            ExportMeshToObj(CreateExcavatorCabMesh(), System.IO.Path.Combine(folderPath, "excavator-cab.obj"));
            ExportMeshToObj(CreateExcavatorBoomMesh(), System.IO.Path.Combine(folderPath, "excavator-boom.obj"));
            ExportMeshToObj(CreateExcavatorStickMesh(), System.IO.Path.Combine(folderPath, "excavator-stick.obj"));
            ExportMeshToObj(CreateExcavatorBucketMesh(), System.IO.Path.Combine(folderPath, "excavator-bucket.obj"));
            ExportMeshToObj(CreateClayBrickMesh(), System.IO.Path.Combine(folderPath, "brick-gost.obj"));
            ExportMeshToObj(CreateConcreteBlockFBSMesh(), System.IO.Path.Combine(folderPath, "block-fbs.obj"));
            ExportMeshToObj(CreateSoilChunkMesh(42, 0.45f), System.IO.Path.Combine(folderPath, "soil-chunk.obj"));
            ExportMeshToObj(CreateQuarryStoneMesh(105, 0.60f), System.IO.Path.Combine(folderPath, "rock-chunk.obj"));
        }
    }
}

