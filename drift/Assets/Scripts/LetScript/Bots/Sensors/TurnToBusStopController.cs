using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToBusStopController : MonoBehaviour
{
    [SerializeField]
    private bool go_turn_to_bus_stop;

    void Start()
    {
        go_turn_to_bus_stop = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "bus_stop_enter")
        {
            go_turn_to_bus_stop = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "bus_stop_enter")
        {
            go_turn_to_bus_stop = true;
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "bus_stop_enter")
        {
            go_turn_to_bus_stop = true;
        }
    }

    public bool GetTurnToBusStop()
    {
        return go_turn_to_bus_stop;
    }
}
