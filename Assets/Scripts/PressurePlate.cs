using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlate : MonoBehaviour
{
    [Header("Detection Settings")]
    public LayerMask targetLayer;
    public bool stayPressed = true;

    [Header("Interaction Events")]
    public UnityEvent onPlatePressed;
    public UnityEvent onPlateReleased;
    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if(((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            if(stayPressed && isActivated) return;
            
            isActivated = true;
            onPlatePressed.Invoke();
            Debug.Log("Objek Player Berhasil Menekan Pressure Plate. Plate Terkunci.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            if(!stayPressed)
            {
                onPlateReleased.Invoke();
                Debug.Log("Objek Player Berhasil Melepas Pressure Plate");
            }
        }
    }
}