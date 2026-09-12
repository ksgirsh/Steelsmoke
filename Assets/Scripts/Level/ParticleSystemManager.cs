using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class ParticleSystemManager : MonoBehaviour
{
    [SerializeField] int[] scenes;

    [System.Serializable]
    public class SceneParticles
    {
        public ParticleSystem[] particles;
    }
    [field:SerializeField] public SceneParticles[] particleSystemsAllScenes = new SceneParticles[2];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Update is called once per frame


    void Start()
    {
        SceneCheck();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetParticleSystemActive(ParticleSystem part, bool active = false)
    {
        part.gameObject.SetActive(active);
    }

    public void SetParticleSystemEmission(ParticleSystem part, bool emit = false)
    {
        var emission = part.emission;
        emission.enabled = emit;

    }

    void SceneCheck()
    {

        for (int i = 0; i < particleSystemsAllScenes.Length; i++)
        {
           
            for (int j = 0; j < particleSystemsAllScenes[i].particles.Length; j++)
            {
                ParticleSystem[] disable = particleSystemsAllScenes[i].particles;
                if (disable[j] != null)
                {
                    SetParticleSystemActive(disable[j], false);
                }
            }
        }

        int s = SceneManager.GetActiveScene().buildIndex;
        ParticleSystem[] particlesToEnable = particleSystemsAllScenes[s].particles;

        foreach (ParticleSystem particle in particlesToEnable)
        {
            SetParticleSystemActive(particle, true);
        }


    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneCheck();
    }
}
