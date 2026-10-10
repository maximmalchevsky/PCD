using UnityEngine;

namespace Task6_7
{
    public class CarAgent : MonoBehaviour
    {
        public Transform[] waypoints;
        public float speed = 5.2f;
        public float rotationSpeed = 6f;
        public float stoppingDistance = 1.3f;
        public float detectionDistance = 5.5f;
        public int currentWaypointIndex = 0;
        public bool loopWaypoints = false;

        private Rigidbody rb;
        private float currentSpeed = 0f;
        private float stuckTimer = 0f;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            currentSpeed = speed;
        }

        public void SetInitialWaypointIndex(int index)
        {
            if (waypoints != null && waypoints.Length > 0)
            {
                currentWaypointIndex = (index + 1) % waypoints.Length;
            }
        }

        void Update()
        {
            if (transform.position.x >= -75f && transform.position.x <= -25f && transform.position.z >= -5f && transform.position.z <= 45f)
            {
                if (TrafficManager.Instance != null)
                {
                    TrafficManager.Instance.OnCarExited(gameObject);
                }
                Destroy(gameObject);
                return;
            }

            if (waypoints == null || waypoints.Length == 0) return;

            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loopWaypoints)
                {
                    currentWaypointIndex = 0;
                }
                else
                {
                    if (TrafficManager.Instance != null)
                    {
                        TrafficManager.Instance.OnCarExited(gameObject);
                    }
                    Destroy(gameObject);
                    return;
                }
            }

            Transform target = waypoints[currentWaypointIndex];
            if (target == null) return;

            Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 direction = (targetPosition - transform.position).normalized;

            if (currentSpeed < 0.25f)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > 7.5f)
                {
                    if (TrafficManager.Instance != null)
                    {
                        TrafficManager.Instance.OnCarExited(gameObject);
                    }
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                stuckTimer = 0f;
            }

            float targetSpeed = speed;
            bool forceCreep = (stuckTimer > 3.0f);

            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.4f;

            if (Physics.SphereCast(rayOrigin, 0.32f, transform.forward, out hit, detectionDistance))
            {
                if (hit.collider.gameObject != gameObject && !hit.collider.transform.IsChildOf(transform))
                {
                    CarAgent otherCar = hit.collider.GetComponentInParent<CarAgent>();
                    PedestrianAgent ped = hit.collider.GetComponentInParent<PedestrianAgent>();
                    CharacterController player = hit.collider.GetComponentInParent<CharacterController>();
                    TrafficLightController lightCtrl = hit.collider.GetComponentInParent<TrafficLightController>();

                    bool isObstacle = false;

                    if (ped != null || player != null)
                    {
                        if (!forceCreep)
                        {
                            isObstacle = true;
                        }
                    }

                    if (otherCar != null)
                    {
                        float dot = Vector3.Dot(transform.forward, otherCar.transform.forward);
                        if (dot > 0.5f)
                        {
                            if (hit.distance < 2.2f)
                            {
                                isObstacle = true;
                            }
                        }
                        else
                        {
                            if (gameObject.GetHashCode() < otherCar.gameObject.GetHashCode() && !forceCreep)
                            {
                                if (hit.distance < 2.5f)
                                {
                                    isObstacle = true;
                                }
                            }
                        }
                    }

                    if (lightCtrl != null && lightCtrl.IsRedOrYellow())
                    {
                        if (!forceCreep)
                        {
                            isObstacle = true;
                        }
                    }

                    if (isObstacle)
                    {
                        if (hit.distance < 1.8f)
                        {
                            targetSpeed = 0f;
                        }
                        else
                        {
                            float factor = (hit.distance - 1.8f) / Mathf.Max(0.1f, detectionDistance - 1.8f);
                            targetSpeed = Mathf.Lerp(0f, speed, factor);
                        }
                    }
                }
            }

            Collider[] pedsAhead = Physics.OverlapSphere(transform.position, 4.5f);
            foreach (var pc in pedsAhead)
            {
                if (pc.gameObject != gameObject && !pc.transform.IsChildOf(transform))
                {
                    PedestrianAgent p = pc.GetComponentInParent<PedestrianAgent>();
                    CharacterController pl = pc.GetComponentInParent<CharacterController>();
                    Transform targetPed = p != null ? p.transform : (pl != null ? pl.transform : null);
                    if (targetPed != null)
                    {
                        Vector3 toPed = targetPed.position - transform.position;
                        float fwdDist = Vector3.Dot(toPed, transform.forward);
                        float latDist = Mathf.Abs(Vector3.Dot(toPed, transform.right));
                        if (fwdDist > 0.6f && fwdDist < 4.0f && latDist < 1.1f)
                        {
                            if (!forceCreep)
                            {
                                targetSpeed = 0f;
                                break;
                            }
                        }
                    }
                }
            }

            if (!forceCreep)
            {
                Vector3[] intersections = new Vector3[] {
                    new Vector3(0f, 0.05f, -16f),
                    new Vector3(0f, 0.05f, 16f),
                    new Vector3(-22f, 0.05f, -16f),
                    new Vector3(-22f, 0.05f, 16f),
                    new Vector3(22f, 0.05f, -16f),
                    new Vector3(22f, 0.05f, 16f)
                };

                foreach (var inter in intersections)
                {
                    float distToInter = Vector3.Distance(transform.position, inter);
                    if (distToInter > 2.2f && distToInter < 5.2f)
                    {
                        Collider[] inInter = Physics.OverlapSphere(inter, 2.5f);
                        foreach (var c in inInter)
                        {
                            if (c.gameObject != gameObject && !c.transform.IsChildOf(transform))
                            {
                                CarAgent ic = c.GetComponentInParent<CarAgent>();
                                if (ic != null && ic != this)
                                {
                                    if (gameObject.GetHashCode() < ic.gameObject.GetHashCode())
                                    {
                                        targetSpeed = 0f;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            Collider[] nearby = Physics.OverlapSphere(transform.position, 1.4f);
            foreach (var col in nearby)
            {
                if (col.gameObject != gameObject && !col.transform.IsChildOf(transform))
                {
                    CarAgent other = col.GetComponentInParent<CarAgent>();
                    if (other != null && other != this)
                    {
                        Vector3 toOther = other.transform.position - transform.position;
                        float dot = Vector3.Dot(transform.forward, toOther.normalized);
                        if (dot > 0.4f)
                        {
                            if (gameObject.GetHashCode() < other.gameObject.GetHashCode())
                            {
                                targetSpeed = 0f;
                                currentSpeed = 0f;
                                break;
                            }
                        }
                    }
                }
            }

            if (forceCreep && targetSpeed < 1.6f)
            {
                targetSpeed = 1.6f;
            }

            float brakeRate = (targetSpeed < currentSpeed) ? 14f : 8f;
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, brakeRate * Time.deltaTime);

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            if (currentSpeed > 0.02f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
            }

            if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance)
            {
                currentWaypointIndex++;
            }
        }
    }
}
