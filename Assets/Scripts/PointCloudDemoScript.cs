
using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;

public class PointCloudDemoScript : MonoBehaviour {

    [SerializeField]
    private ParticleSystem m_PointCloudParticlePrefab;

    [SerializeField]
    private int m_MaxPointsToShow = 300;

    [SerializeField]
    private float m_ParticleSize = 1.0f;

    private ParticleSystem m_ParticleSystem;
    private ParticleSystem.Particle[] m_Particles;
    private ParticleSystem.Particle[] m_NoParticles;
    private ARInterface.PointCloud m_PointCloud;

    private Transform pointsRoot = null;


    private bool particlesOn = true;

    private void OnDisable()
    {
        m_ParticleSystem.SetParticles(m_NoParticles, 1);


        CommandKeeper.SetPointCloudOn -= SetPointsVisible;
        CommandKeeper.GetPointCloudOn -= GetPointsVisible;
        CommandKeeper.GetPointCloudCoordsList -= GetPointsCloudList;
    }

    // Use this for initialization
    void Start()
    {
        pointsRoot = Camera.main.transform.parent;

        m_ParticleSystem = Instantiate(m_PointCloudParticlePrefab, pointsRoot);
        m_NoParticles = new ParticleSystem.Particle[1];
        m_NoParticles[0].startSize = 0f;

        CommandKeeper.SetPointCloudOn += SetPointsVisible;
        CommandKeeper.GetPointCloudOn += GetPointsVisible;
        CommandKeeper.GetPointCloudCoordsList += GetPointsCloudList;
    }

    private List<Vector3> GetPointsCloudList()
    {
        List<Vector3> coords = new List<Vector3>();

        for(int i = 0; i < m_Particles.Length; i++)
        {
            coords.Add(m_Particles[i].position);
        }

        return coords;
    }

    private bool GetPointsVisible ()
    {
        return particlesOn;
    }

    private void SetPointsVisible(bool on)
    {
        particlesOn = on;

        if (!particlesOn)
        {
            m_ParticleSystem.SetParticles(m_NoParticles, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (particlesOn && ARInterface.GetInterface().TryGetPointCloud(ref m_PointCloud))
        {
            var scale = pointsRoot.transform.localScale.x;

            //CommandKeeper.WriteLineDebug("scale = " + scale);

            var numParticles = Mathf.Min(m_PointCloud.points.Count, m_MaxPointsToShow);
            if (m_Particles == null || m_Particles.Length != numParticles)
                m_Particles = new ParticleSystem.Particle[numParticles];

            for (int i = 0; i < numParticles; ++i)
            {
                m_Particles[i].position = m_PointCloud.points[i] * scale;
                m_Particles[i].startColor = new Color(1.0f, 1.0f, 1.0f);
                m_Particles[i].startSize = m_ParticleSize * scale;
            }

            m_ParticleSystem.SetParticles(m_Particles, numParticles);
        }
        else
        {
            m_ParticleSystem.SetParticles(m_NoParticles, 1);
        }
    }


} // End Of Class //


