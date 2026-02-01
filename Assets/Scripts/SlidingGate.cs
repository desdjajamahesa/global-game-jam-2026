using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingGate : MonoBehaviour
{
    [Header("Sliding Gate Settings")]
    public float closedY = 0f;
    public float openY = 6f;
    public float moveSpeed = 5f;
    private float targetY;

    [Header("Collider Settings")]
    [SerializeField] private Collider gateCollider;

    void Start()
    {
        // Get collider if not assigned
        if (gateCollider == null)
        {
            gateCollider = GetComponent<Collider>();
        }

        transform.localPosition = new Vector3(transform.localPosition.x, closedY, transform.localPosition.z);
        targetY = closedY;
    }

    void Update()
    {
        float newY = Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * moveSpeed);
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

        // Check if gate has reached open position and disable collider
        if (Mathf.Abs(transform.localPosition.y - openY) < 0.1f && targetY == openY)
        {
            if (gateCollider != null && gateCollider.enabled)
            {
                gateCollider.enabled = false;
            }
        }
    }

    public void OpenGate()
    {
        targetY = openY;
    }

    public void CloseGate()
    {
        targetY = closedY;
        // Re-enable collider when closing
        if (gateCollider != null)
        {
            gateCollider.enabled = true;
        }
    }
}
