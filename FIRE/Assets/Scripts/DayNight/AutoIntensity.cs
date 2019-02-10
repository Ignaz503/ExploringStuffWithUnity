using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoIntensity : MonoBehaviour
{

    [Header("Real Time cycle length")]
    [SerializeField] [Range(0, 23)] int hours = 0;
    [SerializeField] [Range(0, 59)] int minutes = 10;
    [SerializeField] [Range(0, 59)] int seconds = 0;
    [SerializeField] [Range(0, 1)] float startPercent = 0;

    float fSec;
    float curTime;
    float percent;

    public Gradient nightDayColor;

    [SerializeField] float maxIntensity = 3f;
    [SerializeField] float minIntensity = 0f;
    [SerializeField] float minPoint = -0.2f;

    [SerializeField] float maxAmbient = 1f;
    [SerializeField] float minAmbient = 0f;
    [SerializeField] float minAmbientPoint = -0.2f;


    [SerializeField] Gradient nightDayFogColor= null;
    [SerializeField] AnimationCurve fogDensityCurve = null;
    [SerializeField] float fogScale = 1f;

    [SerializeField] float dayAtmosphereThickness = 0.4f;
    [SerializeField] float nightAtmosphereThickness = 0.87f;

    Vector3 rotationHelper = new Vector3(-360,0,0);
       
    Light mainLight;
    Skybox sky;
    Material skyMat;

    void Start()
    {

        mainLight = GetComponent<Light>();
        skyMat = RenderSettings.skybox;
        fSec = (hours * 60 * 60) + (minutes * 60) + seconds;
        curTime = fSec * startPercent;
        StartCoroutine(KeepTime());
    }

    void Update()
    {

        float tRange = 1 - minPoint;
        float dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);
        float i = ((maxIntensity - minIntensity) * dot) + minIntensity;

        mainLight.intensity = i;

        tRange = 1 - minAmbientPoint;
        dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
        i = ((maxAmbient - minAmbient) * dot) + minAmbient;
        RenderSettings.ambientIntensity = i;

        mainLight.color = nightDayColor.Evaluate(dot);
        RenderSettings.ambientLight = mainLight.color;

        RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

        i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
        skyMat.SetFloat("_AtmosphereThickness", i);

        transform.rotation = Quaternion.Euler(rotationHelper* percent);
    }

    IEnumerator KeepTime()
    {
        while (true)
        {
            curTime += Time.deltaTime;

            curTime = curTime > fSec ? 0 : curTime;

            percent = curTime / fSec;

            yield return null;
        }
    }

}
