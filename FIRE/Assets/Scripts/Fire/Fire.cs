using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Fire : MonoBehaviour {



    public Light pointLight;
    public SphereCollider sphereCollider;

    [SerializeField] float minIntensity = 0f;

    [SerializeField] float maxIntensity = 1f;

    [Header("Smoothing")]
    [Range(1, 50)][SerializeField] int smoothing = 5;
    [Range(1, 50)][SerializeField] int minSmoothing = 5;
    int baseSmoothing;

    [Header("Light Decrease Over Time")]
    [Tooltip("Time in seconds until next decrease")]
    [SerializeField] float intensityDecreaseTime = 0f;
    [SerializeField][Range(0,1f)] float intensityDecreaseAmount = 0f;

    [SerializeField] bool decreaseIntensityOverTime = true;

    [Header("Particle System")]
    [SerializeField] ParticleSystem flames = null;
    MainModule flamesMainModule;
    [SerializeField] ParticleSystem fire = null;
    MainModule fireMainModule;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> smoothQueue;
    float lastSum = 0;

    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;


    }

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
        baseSmoothing = smoothing;
        // External or internal light?
        if (decreaseIntensityOverTime)
        {
            StartCoroutine(DecreaseIntensityTimer());
        }

        ConfigureParticleSystems();
    }

    void ConfigureParticleSystems()
    {
        flamesMainModule = flames.main;

        fireMainModule = fire.main;
    }

    void Update()
    {
        HandleFlicker();
    }

    void HandleFlicker()
    {
        if (pointLight == null)
            return;

        // pop off an item if too big
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        // Calculate new smoothed average
        pointLight.intensity = lastSum / (float)smoothQueue.Count;
        //sphereCollider.radius = Mathf.Max(.1f,pointLight.intensity*.25f);
        UpdateParticleSystemSize(pointLight.intensity);
    }

    void UpdateParticleSystemSize(float lightIntensity)
    {
        flamesMainModule.startSizeMultiplier = lightIntensity;
        if (lightIntensity < .8)
            flames.gameObject.SetActive(false);
        else
            flames.gameObject.SetActive(true);

        fireMainModule.startSizeMultiplier = lightIntensity;
    }

    public void AddFireWood(FireWood log)
    {
        maxIntensity += log.Value;
        StartCoroutine(TemporailyHigherFlicker(.5f, 10));
    }

    IEnumerator DecreaseIntensityTimer()
    {
        yield return new WaitForSeconds(intensityDecreaseTime);
        while (true)
        {
            maxIntensity -= intensityDecreaseAmount;
            maxIntensity = Mathf.Max(0f, maxIntensity);
            yield return new WaitForSeconds(intensityDecreaseTime);
        }
    }

    IEnumerator TemporailyHigherFlicker(float seconds, int smoothingChange)
    {

        smoothing -= smoothingChange;
        smoothing = Mathf.Max(minSmoothing, smoothing);
        yield return new WaitForSeconds(seconds);

        for(int i = smoothing; i <= baseSmoothing; i++)
        {
            smoothing = i;
            yield return new WaitForSeconds(.1f);
        }
    }
}
