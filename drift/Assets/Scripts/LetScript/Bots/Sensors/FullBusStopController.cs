using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBusStopController : MonoBehaviour
{
    private bool full_stop;

    void Start()
    {
        full_stop = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "full_stop_bus")
        {
            full_stop = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "full_stop_bus")
        {
            full_stop = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "full_stop_bus")
        {
            full_stop = true;
        }
    }

    public bool GetFullStop()
    {
        return full_stop;
    }
}
