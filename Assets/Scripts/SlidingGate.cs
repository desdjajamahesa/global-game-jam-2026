using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingGate : MonoBehaviour
{
    [Header("Sliding Gate Settings")]
    public float closedY = 2.5f;
    public float openY = -2.5f;
    public float moveSpeed = 5f;
    private float targetY;

    void Start()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, closedY, transform.localPosition.z);
        targetY = closedY;
    }

    void Update()
    {
        float newY = Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * moveSpeed);
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }

    public void OpenGate() => targetY = openY;
    public void CloseGate() => targetY = closedY;
}
