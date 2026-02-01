using UnityEngine;
using Cinemachine;

/// <summary>
/// Manages the player camera using Cinemachine with smooth follow and offset
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform shadowTransform;
    [SerializeField] private PlayerStateManager playerStateManager;
    
    [Header("Follow Settings")]
    [Tooltip("Offset from the player position")]
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 2f, -5f);
    
    [Tooltip("How quickly camera follows on X axis (0 = instant, higher = slower)")]
    [SerializeField] private float dampingX = 1f;
    
    [Tooltip("How quickly camera follows on Y axis (0 = instant, higher = slower)")]
    [SerializeField] private float dampingY = 1f;
    
    [Tooltip("How quickly camera follows on Z axis (0 = instant, higher = slower)")]
    [SerializeField] private float dampingZ = 1f;
    
    [Header("Look At Settings")]
    [Tooltip("Offset from player for camera to look at")]
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 1.5f, 0f);
    
    [Tooltip("Damping for camera rotation")]
    [SerializeField] private float aimDamping = 2f;
    
    [Header("Optional: Mouse Look")]
    [SerializeField] private bool enableMouseLook = false;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;
    
    private CinemachineTransposer transposer;
    private CinemachineComposer composer;
    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    
    private void Start()
    {
        InitializeCamera();
        SubscribeToStateChanges();
    }

    private void OnDestroy()
    {
        UnsubscribeFromStateChanges();
    }

    private void SubscribeToStateChanges()
    {
        PlayerStateManager.onAwakeActive += SwitchToPlayerFollow;
        PlayerStateManager.onAstralActive += SwitchToShadowFollow;
    }

    private void UnsubscribeFromStateChanges()
    {
        PlayerStateManager.onAwakeActive -= SwitchToPlayerFollow;
        PlayerStateManager.onAstralActive -= SwitchToShadowFollow;
    }
    
    private void InitializeCamera()
    {
        // If virtual camera isn't assigned, try to find it
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            if (virtualCamera == null)
            {
                Debug.LogError("No CinemachineVirtualCamera found! Please assign one.");
                return;
            }
        }
        
        // Get or add Transposer (for follow)
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer == null)
        {
            transposer = virtualCamera.AddCinemachineComponent<CinemachineTransposer>();
        }
        
        // Get or add Composer (for aim)
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        if (composer == null)
        {
            composer = virtualCamera.AddCinemachineComponent<CinemachineComposer>();
        }
        
        // Set follow and look at targets
        if (playerTransform != null)
        {
            virtualCamera.Follow = playerTransform;
            virtualCamera.LookAt = playerTransform;
        }
        
        // Apply settings
        UpdateCameraSettings();
    }
    
    private void Update()
    {
        if (enableMouseLook)
        {
            HandleMouseLook();
        }
    }

    private void SwitchToPlayerFollow()
    {
        if (virtualCamera != null && playerTransform != null)
        {
            virtualCamera.Follow = playerTransform;
            virtualCamera.LookAt = playerTransform;
        }
    }

    private void SwitchToShadowFollow()
    {
        if (virtualCamera != null && shadowTransform != null)
        {
            virtualCamera.Follow = shadowTransform;
            virtualCamera.LookAt = shadowTransform;
        }
    }
    
    /// <summary>
    /// Updates camera settings based on inspector values
    /// </summary>
    public void UpdateCameraSettings()
    {
        if (transposer != null)
        {
            // Set follow offset
            transposer.m_FollowOffset = followOffset;
            
            // Set damping for smooth follow
            transposer.m_XDamping = dampingX;
            transposer.m_YDamping = dampingY;
            transposer.m_ZDamping = dampingZ;
            
            // Binding mode - use world space for following
            transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        }
        
        if (composer != null)
        {
            // Set look at offset
            composer.m_TrackedObjectOffset = lookAtOffset;
            
            // Set aim damping
            composer.m_HorizontalDamping = aimDamping;
            composer.m_VerticalDamping = aimDamping;
        }
    }
    
    /// <summary>
    /// Optional: Handle mouse look rotation
    /// </summary>
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        currentYRotation += mouseX;
        currentXRotation -= mouseY;
        currentXRotation = Mathf.Clamp(currentXRotation, minVerticalAngle, maxVerticalAngle);
        
        // Apply rotation to the virtual camera
        if (virtualCamera != null)
        {
            virtualCamera.transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0f);
        }
    }
    
    /// <summary>
    /// Change follow offset at runtime
    /// </summary>
    public void SetFollowOffset(Vector3 newOffset)
    {
        followOffset = newOffset;
        if (transposer != null)
        {
            transposer.m_FollowOffset = followOffset;
        }
    }
    
    /// <summary>
    /// Change damping values at runtime
    /// </summary>
    public void SetDamping(float x, float y, float z)
    {
        dampingX = x;
        dampingY = y;
        dampingZ = z;
        
        if (transposer != null)
        {
            transposer.m_XDamping = dampingX;
            transposer.m_YDamping = dampingY;
            transposer.m_ZDamping = dampingZ;
        }
    }
    
    /// <summary>
    /// Shake the camera
    /// </summary>
    public void ShakeCamera(float intensity, float duration)
    {
        // You can implement camera shake using Cinemachine Impulse
        // This is a placeholder for that functionality
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }
    
    private System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin noise = 
            virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        if (noise != null)
        {
            noise.m_AmplitudeGain = intensity;
            yield return new WaitForSeconds(duration);
            noise.m_AmplitudeGain = 0f;
        }
    }
    
    // Call this in the inspector to apply changes during edit mode
    private void OnValidate()
    {
        if (Application.isPlaying && virtualCamera != null)
        {
            UpdateCameraSettings();
        }
    }
}
