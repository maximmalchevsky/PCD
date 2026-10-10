using System.Collections.Generic;
using UnityEngine;

namespace Task6_7
{
    [System.Serializable]
    public class CarRoute
    {
        public Transform[] waypoints;

        public CarRoute() { }
        public CarRoute(params Transform[] w)
        {
            waypoints = w;
        }
    }

    public class TrafficManager : MonoBehaviour
    {
        public static TrafficManager Instance;

        public GameObject[] carPrefabs;
        public Material carMaterial;
        public int maxActiveCars = 10;
        public float spawnInterval = 4.0f;

        public List<CarRoute> routes = new List<CarRoute>();

        private List<GameObject> activeCars = new List<GameObject>();
        private float spawnTimer = 0f;

        public static bool IsInConstructionZone(Vector3 pos)
        {
            return pos.x >= -74f && pos.x <= -26f && pos.z >= -4f && pos.z <= 44f;
        }

        public bool IsRouteValid(CarRoute cr)
        {
            if (cr == null || cr.waypoints == null || cr.waypoints.Length < 2) return false;
            foreach (var wp in cr.waypoints)
            {
                if (wp == null) return false;
                if (!wp.gameObject.activeInHierarchy) return false;
                if (wp.parent != null && !wp.parent.gameObject.activeInHierarchy) return false;
                if (IsInConstructionZone(wp.position)) return false;
            }
            return true;
        }

        public void PurgeInvalidRoutes()
        {
            if (routes != null)
            {
                routes.RemoveAll(r => !IsRouteValid(r));
            }
        }

        void Awake()
        {
            Instance = this;
            PurgeInvalidRoutes();
        }

        void Start()
        {
            PurgeInvalidRoutes();

            CarAgent[] existing = Object.FindObjectsByType<CarAgent>(FindObjectsInactive.Exclude);
            foreach (var ca in existing)
            {
                if (ca != null)
                {
                    if (IsInConstructionZone(ca.transform.position))
                    {
                        Destroy(ca.gameObject);
                    }
                    else if (!activeCars.Contains(ca.gameObject))
                    {
                        activeCars.Add(ca.gameObject);
                    }
                }
            }

            if (routes == null || routes.Count == 0 || carPrefabs == null || carPrefabs.Length == 0) return;

            int safetyLoop = 0;
            while (activeCars.Count < Mathf.Min(8, maxActiveCars) && routes.Count > 0 && safetyLoop < 40)
            {
                safetyLoop++;
                int rIdx = activeCars.Count % routes.Count;
                CarRoute cr = routes[rIdx];
                if (!IsRouteValid(cr))
                {
                    routes.RemoveAt(rIdx);
                    continue;
                }

                Transform[] route = cr.waypoints;
                int midIdx = Mathf.Clamp(route.Length / 3, 0, route.Length - 1);
                Vector3 spawnPos = route[midIdx].position;

                if (IsInConstructionZone(spawnPos))
                {
                    routes.RemoveAt(rIdx);
                    continue;
                }

                Collider[] cols = Physics.OverlapSphere(spawnPos, 5f);
                bool blocked = false;
                foreach (var c in cols)
                {
                    if (c.GetComponentInParent<CarAgent>() != null) { blocked = true; break; }
                }
                if (blocked) continue;

                GameObject prefab = carPrefabs[activeCars.Count % carPrefabs.Length];
                SpawnCar(prefab, route, midIdx);
            }
        }

        void Update()
        {
            for (int i = activeCars.Count - 1; i >= 0; i--)
            {
                if (activeCars[i] == null)
                {
                    activeCars.RemoveAt(i);
                }
                else if (IsInConstructionZone(activeCars[i].transform.position))
                {
                    Destroy(activeCars[i]);
                    activeCars.RemoveAt(i);
                }
            }

            spawnTimer += Time.deltaTime;
            if (activeCars.Count < maxActiveCars && spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                TrySpawnAtEntrance();
            }
        }

        private void TrySpawnAtEntrance()
        {
            PurgeInvalidRoutes();
            if (routes == null || routes.Count == 0 || carPrefabs == null || carPrefabs.Length == 0) return;

            int randomRouteIdx = Random.Range(0, routes.Count);
            CarRoute cr = routes[randomRouteIdx];
            if (!IsRouteValid(cr))
            {
                routes.RemoveAt(randomRouteIdx);
                return;
            }

            Transform[] route = cr.waypoints;
            Vector3 startPos = route[0].position;
            if (IsInConstructionZone(startPos)) return;

            Collider[] cols = Physics.OverlapSphere(startPos, 7f);
            foreach (var c in cols)
            {
                if (c.GetComponentInParent<CarAgent>() != null) return;
            }

            GameObject prefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
            SpawnCar(prefab, route, 0);
        }

        private void SpawnCar(GameObject prefab, Transform[] route, int startWaypointIndex)
        {
            if (prefab == null || route == null || route.Length == 0) return;

            Vector3 pos = route[startWaypointIndex].position;
            if (IsInConstructionZone(pos)) return;

            Vector3 nextPos = route[Mathf.Min(startWaypointIndex + 1, route.Length - 1)].position;
            Vector3 dir = (nextPos - pos).normalized;
            Quaternion rot = dir != Vector3.zero ? Quaternion.LookRotation(dir) : Quaternion.identity;

            GameObject carObj = Instantiate(prefab, pos, rot, transform);
            carObj.name = "Car_" + prefab.name;
            carObj.transform.localScale = new Vector3(0.70f, 0.70f, 0.70f);

            if (carMaterial != null)
            {
                Renderer[] rends = carObj.GetComponentsInChildren<Renderer>(true);
                foreach (var r in rends) r.sharedMaterial = carMaterial;
            }

            CapsuleCollider col = carObj.GetComponent<CapsuleCollider>();
            if (col == null) col = carObj.AddComponent<CapsuleCollider>();
            col.radius = 0.55f;
            col.height = 1.9f;
            col.center = new Vector3(0f, 0.95f, 0f);

            Rigidbody rb = carObj.GetComponent<Rigidbody>();
            if (rb == null) rb = carObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            CarAgent agent = carObj.GetComponent<CarAgent>();
            if (agent == null) agent = carObj.AddComponent<CarAgent>();
            agent.waypoints = route;
            agent.speed = Random.Range(4.6f, 5.4f);
            agent.currentWaypointIndex = Mathf.Min(startWaypointIndex + 1, route.Length - 1);
            agent.loopWaypoints = false;

            activeCars.Add(carObj);
        }

        public void OnCarExited(GameObject car)
        {
            if (activeCars.Contains(car))
            {
                activeCars.Remove(car);
            }
        }
    }
}
