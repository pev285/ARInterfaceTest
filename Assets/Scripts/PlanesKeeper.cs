using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;

public class PlanesKeeper : MonoBehaviour {

    [SerializeField]
    private GameObject m_PlanePrefab;

    [SerializeField]
    private int m_PlaneLayer;

    public int planeLayer { get { return m_PlaneLayer; } }

    private Transform planesRoot = null;

    private Dictionary<string, GameObject> m_Planes = new Dictionary<string, GameObject>();

    private Dictionary<string, BoundedPlane> planesCreationBuffer = new Dictionary<string, BoundedPlane>();
    private Dictionary<string, float> planesRequestTime = new Dictionary<string, float>();

    private bool planesAreEnabled = true;

    private object lockObject = new object();

    private bool working = false;
    private float planesCreationPause = 0.2f;
    private float timeForPlaneToBeCreated = 0.3f;

    private void Awake()
    {
        StartCoroutine(PlanesCreationCoroutine());
    }

    void OnEnable()
    {
        m_PlaneLayer = LayerMask.NameToLayer("ARGameObject");
        planesRoot = gameObject.transform;

        ARInterface.planeAdded += PlaneAddedHandler;
        ARInterface.planeUpdated += PlaneUpdatedHandler;
        ARInterface.planeRemoved += PlaneRemovedHandler;

        CommandKeeper.SetPlanesOn += SetPlanesActive;
        CommandKeeper.GetPlanesOn += ArePlanesActive;

        working = true;
    } // OnEnable() //

    void OnDisable()
    {
        working = false;

        ARInterface.planeAdded -= PlaneAddedHandler;
        ARInterface.planeUpdated -= PlaneUpdatedHandler;
        ARInterface.planeRemoved -= PlaneRemovedHandler;

        CommandKeeper.SetPlanesOn -= SetPlanesActive;
        CommandKeeper.GetPlanesOn -= ArePlanesActive;
    } // OnDisable() //

    private bool ArePlanesActive()
    {
        return planesAreEnabled;
    }


    private IEnumerator PlanesCreationCoroutine()
    {
        while(true)
        {
            if (working && planesCreationBuffer.Count > 0)
            {
                lock(lockObject)
                {
                    KeyValuePair<string, float>[] array = new KeyValuePair<string, float>[planesRequestTime.Count];
                    ((ICollection<KeyValuePair<string, float>>)planesRequestTime).CopyTo(array, 0);

                    for (int i = 0; i < array.Length; i++)
                    //var keysList = planesRequestTime.Keys;
                    //var ar = keysList.
                    //foreach (string id in keysList)
                    {
                        string id = array[i].Key;
                        if (planesRequestTime[id] < Time.time)
                        {
                            CreatePlane(id);
                            planesRequestTime.Remove(id);
                            planesCreationBuffer.Remove(id);
                        }
                    }

                }// lock //
            }// if (planesBuffer.count > 0) //

            yield return new WaitForSeconds(planesCreationPause);
        } // while(true)
    } // PlanesCreationCoroutine() //

    private void CreatePlane(string id)
    {
        BoundedPlane plane = planesCreationBuffer[id];

        GameObject go;
        go = Instantiate(m_PlanePrefab, planesRoot);

        // Make sure we can pick them later
        foreach (var collider in go.GetComponentsInChildren<Collider>())
        {
            collider.gameObject.layer = m_PlaneLayer;
        }

        m_Planes.Add(plane.id, go);
        go.SetActive(planesAreEnabled);

        CommandKeeper.WriteLineDebug("Plane added: " + plane.id);

        SetPlanePosition(go, plane);
    }

    private void SetPlanePosition(GameObject go, BoundedPlane plane)
    {
        go.transform.localPosition = plane.center;
        go.transform.localRotation = plane.rotation;
        go.transform.localScale = new Vector3(plane.extents.x, 1f, plane.extents.y);
    }

    protected virtual void CreateOrUpdateGameObject(BoundedPlane plane)
    {

        lock(lockObject)
        {
            GameObject go;
            if (!m_Planes.TryGetValue(plane.id, out go))
            {
                //CommandKeeper.WriteLineDebug("Create or update");

                planesRequestTime[plane.id] = Time.time + timeForPlaneToBeCreated;
                planesCreationBuffer[plane.id] = plane;

                //go = Instantiate(m_PlanePrefab, planesRoot);

                //// Make sure we can pick them later
                //foreach (var collider in go.GetComponentsInChildren<Collider>())
                //{
                //    collider.gameObject.layer = m_PlaneLayer;
                //}

                //m_Planes.Add(plane.id, go);
                //go.SetActive(planesAreEnabled);

                //CommandKeeper.WriteLineDebug("Plane added: " + plane.id);
            }
            else
            {
                SetPlanePosition(go, plane);
            }

        } // lock //
    } // CreateOrUpdateGameObject() //


    protected virtual void PlaneAddedHandler(BoundedPlane plane)
    {
        if (m_PlanePrefab)
        {
            CreateOrUpdateGameObject(plane);
        }
    } // PlaneAddedHandler() //

    protected virtual void PlaneUpdatedHandler(BoundedPlane plane)
    {
        if (m_PlanePrefab)
        {
            CreateOrUpdateGameObject(plane);
        }
    } // PlaneUpdatedHandler() //

    protected virtual void PlaneRemovedHandler(BoundedPlane plane)
    {
        lock(lockObject)
        {
            BoundedPlane bufferedPlane;

            if (planesCreationBuffer.TryGetValue(plane.id, out bufferedPlane))
            {
                planesRequestTime.Remove(plane.id);
                planesCreationBuffer.Remove(plane.id);
            }
            else
            {
                GameObject go;
                if (m_Planes.TryGetValue(plane.id, out go))
                {
                    Destroy(go);
                    m_Planes.Remove(plane.id);

                    CommandKeeper.WriteLineDebug("Plane deleted: " + plane.id);
                }
            }
        }
    } // PlaneRemovedHandler() //



    private void SetPlanesActive(bool value)
    {
        lock(lockObject)
        {
            planesAreEnabled = value;

            foreach (string key in m_Planes.Keys)
            {
                m_Planes[key].SetActive(value);
            }

        }
    } // SetPlanesActive() //


} // End Of Class //


