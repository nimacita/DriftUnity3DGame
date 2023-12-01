using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoCamSens : MonoBehaviour
{
    private bool is_sens;

    void Start()
    {
        is_sens = false;
    }

    void OnTriggerExit(Collider other)
    {
        is_sens = false;
    }

    void OnTriggerStay(Collider other)
    {
        is_sens = true;
    }

    void OnTriggerEnter(Collider other)
    {
        is_sens = false;
    }

    public bool GetSens()
    {
        return is_sens;
    }

}
