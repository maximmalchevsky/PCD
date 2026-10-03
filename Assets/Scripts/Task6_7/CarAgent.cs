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

            float targetSpeed = speed;
            bool isStoppedAtRedLight = false;
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
                    if (ped != null)
                    {
                        if (ped.transform.position.y < 0.10f || hit.distance < 2.0f)
                        {
                            isObstacle = true;
                        }
                    }
                    if (player != null)
                    {
                        isObstacle = true;
                    }
                    if (otherCar != null)
                    {
                        if (Vector3.Dot(transform.forward, otherCar.transform.forward) > 0.2f || hit.distance < 2.4f)
                        {
                            isObstacle = true;
                        }
                    }
                    if (lightCtrl != null && lightCtrl.IsRedOrYellow())
                    {
                        isObstacle = true;
                        isStoppedAtRedLight = true;
                    }

                    if (isObstacle)
                    {
                        if (hit.distance < 2.2f)
                        {
                            targetSpeed = 0f;
                        }
                        else
                        {
                            float factor = (hit.distance - 2.2f) / (detectionDistance - 2.2f);
                            targetSpeed = Mathf.Lerp(0f, speed, factor);
                        }
                    }
                }
            }

            Collider[] pedsAhead = Physics.OverlapSphere(transform.position, 5f);
            for (int pi = 0; pi < pedsAhead.Length; pi++)
            {
                Collider pc = pedsAhead[pi];
                if (pc.gameObject != gameObject && !pc.transform.IsChildOf(transform))
                {
                    PedestrianAgent p = pc.GetComponentInParent<PedestrianAgent>();
                    CharacterController pl = pc.GetComponentInParent<CharacterController>();
                    Transform targetPed = p != null ? p.transform : (pl != null ? pl.transform : null);
                    if (targetPed != null)
                    {
                        if (targetPed.position.y < 0.10f)
                        {
                            Vector3 toPed = targetPed.position - transform.position;
                            float fwdDist = Vector3.Dot(toPed, transform.forward);
                            float latDist = Mathf.Abs(Vector3.Dot(toPed, transform.right));
                            if (fwdDist > 0.3f && fwdDist < 4.5f && latDist < 1.3f)
                            {
                                targetSpeed = 0f;
                                break;
                            }
                        }
                    }
                }
            }

            Vector3[] intersections = new Vector3[] {
                new Vector3(0f, 0.05f, -16f),
                new Vector3(0f, 0.05f, 16f),
                new Vector3(-22f, 0.05f, -16f),
                new Vector3(-22f, 0.05f, 16f),
                new Vector3(22f, 0.05f, -16f),
                new Vector3(22f, 0.05f, 16f)
            };

            for (int ii = 0; ii < intersections.Length; ii++)
            {
                Vector3 inter = intersections[ii];
                float distToInter = Vector3.Distance(transform.position, inter);
                if (distToInter >= 2.6f && distToInter < 5.5f)
                {
                    Collider[] inInter = Physics.OverlapSphere(inter, 1.8f);
                    for (int ci = 0; ci < inInter.Length; ci++)
                    {
                        Collider c = inInter[ci];
                        if (c.gameObject != gameObject && !c.transform.IsChildOf(transform))
                        {
                            CarAgent ic = c.GetComponentInParent<CarAgent>();
                            if (ic != null && ic != this && ic.currentSpeed > 0.3f)
                            {
                                targetSpeed = 0f;
                                break;
                            }
                        }
                    }
                }
            }

            Collider[] nearby = Physics.OverlapSphere(transform.position, 1.8f);
            for (int ni = 0; ni < nearby.Length; ni++)
            {
                Collider col = nearby[ni];
                if (col.gameObject != gameObject && !col.transform.IsChildOf(transform))
                {
                    CarAgent other = col.GetComponentInParent<CarAgent>();
                    if (other != null && other != this)
                    {
                        Vector3 toOther = other.transform.position - transform.position;
                        if (Vector3.Dot(transform.forward, toOther.normalized) > 0.35f)
                        {
                            targetSpeed = 0f;
                            break;
                        }
                    }
                }
            }

            if (currentSpeed < 0.15f && !isStoppedAtRedLight)
            {
                stuckTimer += Time.deltaTime;
                if (stuckTimer > 4.0f)
                {
                    targetSpeed = Mathf.Max(targetSpeed, 1.6f);
                }
                if (stuckTimer > 12.0f)
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
