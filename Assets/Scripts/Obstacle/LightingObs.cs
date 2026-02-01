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
    [SerializeField] private ParticleSystem playerLight;
    [SerializeField] private ParticleSystem shadowLight;

    private float originalLightIntensity;
    private float originalPlayerEmissionRate;
    private float originalShadowEmissionRate;
    private ParticleSystem.EmissionModule playerEmissionModule;
    private ParticleSystem.EmissionModule shadowEmissionModule;

    private float targetLightIntensity;
    private float targetPlayerEmissionRate;
    private float targetShadowEmissionRate;
    private float currentLightIntensity;
    private float currentPlayerEmissionRate;
    private float currentShadowEmissionRate;

    void Start()
    {
        // Store original values
        if (lightComponent != null)
        {
            originalLightIntensity = lightComponent.intensity;
            currentLightIntensity = originalLightIntensity;
        }

        if (playerLight != null)
        {
            playerEmissionModule = playerLight.emission;
            originalPlayerEmissionRate = playerEmissionModule.rateOverTime.constant;
            currentPlayerEmissionRate = originalPlayerEmissionRate;
        }

        if (shadowLight != null)
        {
            shadowEmissionModule = shadowLight.emission;
            originalShadowEmissionRate = shadowEmissionModule.rateOverTime.constant;
            currentShadowEmissionRate = 0f; // Start with shadow particles off
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
            targetPlayerEmissionRate = originalPlayerEmissionRate;
            targetShadowEmissionRate = 0f; // Turn off shadow particles

            gameObject.layer = LayerMask.NameToLayer("PlayerLight");
        }
        else
        {
            // Light is OFF - set targets to 0 and enable shadow particles
            targetLightIntensity = 0f;
            targetPlayerEmissionRate = 0f; // Turn off player particles
            targetShadowEmissionRate = originalShadowEmissionRate; // Turn on shadow particles

            gameObject.layer = LayerMask.NameToLayer("ShadowLight");
        }

        // Smooth transition for light intensity
        currentLightIntensity = Mathf.Lerp(currentLightIntensity, targetLightIntensity, Time.deltaTime * transitionSpeed);
        
        // Fast transition for particles when turning off, normal when turning on
        float currentParticleSpeed = isLightOn ? transitionSpeed : particleTransitionSpeed;
        currentPlayerEmissionRate = Mathf.Lerp(currentPlayerEmissionRate, targetPlayerEmissionRate, Time.deltaTime * currentParticleSpeed);
        currentShadowEmissionRate = Mathf.Lerp(currentShadowEmissionRate, targetShadowEmissionRate, Time.deltaTime * currentParticleSpeed);

        // Apply values
        if (lightComponent != null)
        {
            lightComponent.intensity = currentLightIntensity;
        }

        if (playerLight != null)
        {
            var emission = playerLight.emission;
            emission.rateOverTime = currentPlayerEmissionRate;
        }

        if (shadowLight != null)
        {
            var emission = shadowLight.emission;
            emission.rateOverTime = currentShadowEmissionRate;
        }
    }
}
