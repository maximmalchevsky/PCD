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

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            CarAgent[] existing = Object.FindObjectsByType<CarAgent>(FindObjectsInactive.Exclude);
            foreach (var ca in existing)
            {
                if (ca != null && !activeCars.Contains(ca.gameObject))
                {
                    activeCars.Add(ca.gameObject);
                }
            }

            if (routes == null || routes.Count == 0 || carPrefabs == null || carPrefabs.Length == 0) return;

            while (activeCars.Count < Mathf.Min(8, maxActiveCars))
            {
                int rIdx = activeCars.Count % routes.Count;
                CarRoute cr = routes[rIdx];
                if (cr == null || cr.waypoints == null || cr.waypoints.Length < 2) break;

                Transform[] route = cr.waypoints;
                int midIdx = Mathf.Clamp(route.Length / 3, 0, route.Length - 1);
                Vector3 spawnPos = route[midIdx].position;

                Collider[] cols = Physics.OverlapSphere(spawnPos, 5f);
                bool blocked = false;
                foreach (var c in cols)
                {
                    if (c.GetComponentInParent<CarAgent>() != null) { blocked = true; break; }
                }
                if (blocked) break;

                GameObject prefab = carPrefabs[activeCars.Count % carPrefabs.Length];
                SpawnCar(prefab, route, midIdx);
            }
        }

        void Update()
        {
            activeCars.RemoveAll(c => c == null);

            spawnTimer += Time.deltaTime;
            if (activeCars.Count < maxActiveCars && spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                TrySpawnAtEntrance();
            }
        }

        private void TrySpawnAtEntrance()
        {
            if (routes == null || routes.Count == 0 || carPrefabs == null || carPrefabs.Length == 0) return;

            int randomRouteIdx = Random.Range(0, routes.Count);
            CarRoute cr = routes[randomRouteIdx];
            if (cr == null || cr.waypoints == null || cr.waypoints.Length < 2) return;

            Transform[] route = cr.waypoints;
            Vector3 startPos = route[0].position;
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
