using System.Collections.Generic;
using UnityEngine;

namespace Task6_7
{
    public class CityExpander : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AutoExpandOnPlay()
        {
            if (GameObject.Find("City_Clone_West") != null) return;
            GameObject holder = new GameObject("CityExpander_Runtime");
            holder.AddComponent<CityExpander>();
        }

        void Awake()
        {
            ExpandCity();
        }

        public void ExpandCity()
        {
            if (GameObject.Find("City_Clone_West") != null) return;

            ExpandGround();

            Vector3[] blockOffsets = new Vector3[]
            {
                new Vector3(-68f, 0f, 0f),
                new Vector3(0f, 0f, 144f),
                new Vector3(-68f, 0f, 144f)
            };

            string[] blockNames = new string[]
            {
                "City_Clone_West",
                "City_Clone_North",
                "City_Clone_NorthWest"
            };

            string[] rootsToClone = new string[]
            {
                "--- ROADS & SIDEWALKS ---",
                "--- COMMERCIAL BUILDINGS & SKYSCRAPERS ---",
                "--- GREENERY (TREES & BUSHES) ---",
                "--- STREET DETAILS & PROPS ---",
                "--- TRAFFIC LIGHTS ---"
            };

            for (int b = 0; b < blockOffsets.Length; b++)
            {
                Vector3 offset = blockOffsets[b];
                GameObject blockParent = new GameObject(blockNames[b]);

                for (int r = 0; r < rootsToClone.Length; r++)
                {
                    GameObject origRoot = GameObject.Find(rootsToClone[r]);
                    if (origRoot != null)
                    {
                        GameObject clone = Instantiate(origRoot, blockParent.transform);
                        clone.name = origRoot.name + "_" + blockNames[b];
                        clone.transform.position = origRoot.transform.position + offset;

                        StripHousePortalsFromClone(clone);
                    }
                }

                CloneCarRoutes(offset, blockParent);
                ClonePedestrians(offset, blockParent);
            }

            ExpandTrafficSystem();
        }

        private void ExpandGround()
        {
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                ground.transform.position = new Vector3(-33f, -0.5f, 72f);
                ground.transform.localScale = new Vector3(220f, 1f, 380f);
            }
        }

        private void StripHousePortalsFromClone(GameObject clone)
        {
            Transform[] all = clone.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] == null) continue;
                string n = all[i].name;
                if (n.Contains("Portal_") || n.Contains("Threshold") || n.Contains("Glowing_Accent") || n.Contains("Entrance_Porch") || n.Contains("Doormat"))
                {
                    Destroy(all[i].gameObject);
                }
            }
        }

        private void CloneCarRoutes(Vector3 offset, GameObject parent)
        {
            GameObject origCarWaypoints = GameObject.Find("--- CAR WAYPOINTS ---");
            if (origCarWaypoints == null) return;

            GameObject clonedWpRoot = Instantiate(origCarWaypoints, parent.transform);
            clonedWpRoot.name = origCarWaypoints.name + "_Clone";
            clonedWpRoot.transform.position = origCarWaypoints.transform.position + offset;

            TrafficManager tm = Object.FindAnyObjectByType<TrafficManager>();
            if (tm == null) return;

            Transform[] clonedRoutes = clonedWpRoot.GetComponentsInChildren<Transform>();
            for (int i = 0; i < clonedRoutes.Length; i++)
            {
                Transform r = clonedRoutes[i];
                if (r == clonedWpRoot.transform || r.parent != clonedWpRoot.transform) continue;

                List<Transform> wpList = new List<Transform>();
                for (int w = 0; w < r.childCount; w++)
                {
                    wpList.Add(r.GetChild(w));
                }
                if (wpList.Count >= 2)
                {
                    tm.routes.Add(new CarRoute(wpList.ToArray()));
                }
            }
        }

        private void ClonePedestrians(Vector3 offset, GameObject parent)
        {
            GameObject origChars = GameObject.Find("--- CHARACTERS (MEMES) ---");
            GameObject origPedWp = GameObject.Find("--- PEDESTRIAN WAYPOINTS (SIDEWALKS & CROSSWALKS) ---");
            if (origChars == null) return;

            GameObject clonedPedWpRoot = null;
            if (origPedWp != null)
            {
                clonedPedWpRoot = Instantiate(origPedWp, parent.transform);
                clonedPedWpRoot.name = origPedWp.name + "_Clone";
                clonedPedWpRoot.transform.position = origPedWp.transform.position + offset;
            }

            GameObject clonedCharsRoot = Instantiate(origChars, parent.transform);
            clonedCharsRoot.name = origChars.name + "_Clone";
            clonedCharsRoot.transform.position = origChars.transform.position + offset;

            if (clonedPedWpRoot != null)
            {
                PedestrianAgent[] agents = clonedCharsRoot.GetComponentsInChildren<PedestrianAgent>();
                for (int i = 0; i < agents.Length; i++)
                {
                    PedestrianAgent pa = agents[i];
                    if (pa.waypoints != null && pa.waypoints.Length > 0)
                    {
                        List<Transform> newWps = new List<Transform>();
                        for (int w = 0; w < pa.waypoints.Length; w++)
                        {
                            if (pa.waypoints[w] == null) continue;
                            string wpName = pa.waypoints[w].name;
                            Transform found = FindChildRecursive(clonedPedWpRoot.transform, wpName);
                            if (found != null) newWps.Add(found);
                            else newWps.Add(pa.waypoints[w]);
                        }
                        pa.waypoints = newWps.ToArray();
                        pa.currentWaypointIndex = 0;
                    }
                }
            }
        }

        private Transform FindChildRecursive(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform c = parent.GetChild(i);
                if (c.name == name) return c;
                Transform f = FindChildRecursive(c, name);
                if (f != null) return f;
            }
            return null;
        }

        private void ExpandTrafficSystem()
        {
            TrafficManager tm = Object.FindAnyObjectByType<TrafficManager>();
            if (tm != null)
            {
                tm.maxActiveCars = 35;
                tm.spawnInterval = 1.6f;
            }
        }
    }
}
