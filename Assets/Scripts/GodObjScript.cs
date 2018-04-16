using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;

public class GodObjScript : MonoBehaviour {


    protected ARInterface m_ARInterface;

    [SerializeField]
    protected Camera m_ARCamera;
    public Camera arCamera { get { return m_ARCamera; } }


    [SerializeField]
    private bool m_PlaneDetection;

    //[SerializeField]
    //private bool m_LightEstimation;

    //[SerializeField]
    //private bool m_PointCloud;

    public bool IsRunning
    {
        get
        {
            if (m_ARInterface == null)
                return false;
            return m_ARInterface.IsRunning;
        }
    }


    [SerializeField]
    private bool m_BackgroundRendering = true;

    public virtual bool BackgroundRendering
    {
        get { return m_BackgroundRendering; }

        set
        {
            if (m_ARInterface != null)
            {
                m_ARInterface.BackgroundRendering = m_BackgroundRendering = value;
            }
        }
    }


    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {

    }



    void OnBeforeRender()
    {
        m_ARInterface.UpdateCamera(m_ARCamera);

        Pose pose = new Pose();
        if (m_ARInterface.TryGetPose(ref pose))
        {
            m_ARCamera.transform.localPosition = pose.position;
            m_ARCamera.transform.localRotation = pose.rotation;
            var parent = m_ARCamera.transform.parent;
            //if (parent != null)
            //    parent.localScale = Vector3.one * scale;
        }
    }

    protected virtual void SetupARInterface()
    {
        m_ARInterface = ARInterface.GetInterface();
    }

    private void OnEnable()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Input.simulateMouseWithTouches = true;

        if (m_ARInterface == null)
            SetupARInterface();

        // See if we are on a camera
        if (m_ARCamera == null)
            m_ARCamera = GetComponent<Camera>();

        // Fallback to main camera
        if (m_ARCamera == null)
            m_ARCamera = Camera.main;

        StopAllCoroutines();
        StartCoroutine(StartServiceRoutine());

    }


    IEnumerator StartServiceRoutine()
    {
        yield return m_ARInterface.StartService(GetSettings());
        if (IsRunning)
        {
            m_ARInterface.SetupCamera(m_ARCamera);
            m_ARInterface.BackgroundRendering = BackgroundRendering;
            Application.onBeforeRender += OnBeforeRender;
        }
        else
        {
            enabled = false;
        }
    }


    void OnDisable()
    {
        StopAllCoroutines();
        if (IsRunning)
        {
            m_ARInterface.StopService();
            Application.onBeforeRender -= OnBeforeRender;
        }
    }

    void Update()
    {
        m_ARInterface.Update();
    }

    public ARInterface.Settings GetSettings()
    {
        return new ARInterface.Settings()
        {
            enablePointCloud = true,// m_PointCloud,
            enablePlaneDetection = m_PlaneDetection,
            enableLightEstimation = false // m_LightEstimation
        };
    }

} // end of class //


