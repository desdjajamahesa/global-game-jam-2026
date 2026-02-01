using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private Transform shadowTarget;

    [Header("Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    
    [Header("Visual Debug")]
    [SerializeField] private bool showDetectionRadius = true;
    private bool isShadowActive = false;
    private Vector3 initialPosition;
    
    void Start()
    {
        initialPosition = transform.position;

        ToggleVisibility(false);
        PlayerStateManager.onAstralActive += OnShadowAppeared;
        PlayerStateManager.onAwakeActive += OnShadowDisappeared;
    }

    void Update()
    {
        if(!isShadowActive || shadowTarget == null) return;

        float distanceToShadow = Vector3.Distance(transform.position, shadowTarget.position);
        
        if (distanceToShadow <= detectionRadius)
        {
            MoveTowardsShadow();
        }
        else
        {
            LookAtShadow();
        }
    }

    private void MoveTowardsShadow()
    {
        Vector3 targetPos = new Vector3(shadowTarget.position.x, transform.position.y, shadowTarget.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed *Time.deltaTime);
        LookAtShadow();
    }

    private void LookAtShadow()
    {
        Vector3 direction = (shadowTarget.position - transform.position);
        direction.y = 0;
        if(direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnShadowAppeared()
    {
        isShadowActive = true;
        ToggleVisibility(true);
    }

    private void OnShadowDisappeared()
    {
        isShadowActive = false;
        ToggleVisibility(false);

        // transform.position = initialPosition;
    }

    private void ToggleVisibility(bool Visible)
    {
        if(enemy != null)
        {
            enemy.SetActive(Visible);
        }
    }

    private void OnDrawGizmos()
    {
        if(showDetectionRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
