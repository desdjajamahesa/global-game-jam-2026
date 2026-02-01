using UnityEngine;
using System.Collections;

public class LightingObs : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private bool isLightOn = true;
    [SerializeField] private float lightOnDuration = 3f;
    [SerializeField] private float lightOffDuration = 2f;
    [SerializeField] private float transitionSpeed = 5f;

    [Header("Torch Flicker Settings")]
    [SerializeField] private float minIntensity = 3f;
    [SerializeField] private float maxIntensity = 15f;
    [SerializeField] private float flickerSpeed = 10f;
    [SerializeField] private float particleTransitionSpeed = 20f;

    [Header("Component References")]
    [SerializeField] private Light lightComponent;
    [SerializeField] private ParticleSystem particleFX;

    private float originalLightIntensity;
    private float originalEmissionRate;
    private ParticleSystem.EmissionModule emissionModule;

    private float targetLightIntensity;
    private float targetEmissionRate;
    private float currentLightIntensity;
    private float currentEmissionRate;

    void Start()
    {
        // Store original values
        if (lightComponent != null)
        {
            originalLightIntensity = lightComponent.intensity;
            currentLightIntensity = originalLightIntensity;
        }

        if (particleFX != null)
        {
            emissionModule = particleFX.emission;
            originalEmissionRate = emissionModule.rateOverTime.constant;
            currentEmissionRate = originalEmissionRate;
        }

        // Start the light interval behavior
        StartCoroutine(LightIntervalRoutine());
    }

    void Update()
    {
        UpdateVisualization();
    }

    private IEnumerator LightIntervalRoutine()
    {
        while (true)
        {
            // Light ON phase
            isLightOn = true;
            yield return new WaitForSeconds(lightOnDuration);

            // Light OFF phase
            isLightOn = false;
            yield return new WaitForSeconds(lightOffDuration);
        }
    }

    private void UpdateVisualization()
    {
        if (isLightOn)
        {
            // Light is ON - create torch flicker effect with intensity range
            float flicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            
            targetLightIntensity = Mathf.Lerp(minIntensity, maxIntensity, flicker);
            targetEmissionRate = originalEmissionRate;

            gameObject.layer = LayerMask.NameToLayer("PlayerLight");
        }
        else
        {
            // Light is OFF - set targets to 0
            targetLightIntensity = 0f;
            targetEmissionRate = 0f;

            gameObject.layer = LayerMask.NameToLayer("ShadowLight");
        }

        // Smooth transition for light intensity
        currentLightIntensity = Mathf.Lerp(currentLightIntensity, targetLightIntensity, Time.deltaTime * transitionSpeed);
        
        // Fast transition for particles when turning off, normal when turning on
        float currentParticleSpeed = isLightOn ? transitionSpeed : particleTransitionSpeed;
        currentEmissionRate = Mathf.Lerp(currentEmissionRate, targetEmissionRate, Time.deltaTime * currentParticleSpeed);

        // Apply values
        if (lightComponent != null)
        {
            lightComponent.intensity = currentLightIntensity;
        }

        if (particleFX != null)
        {
            var emission = particleFX.emission;
            emission.rateOverTime = currentEmissionRate;
        }
    }
}
