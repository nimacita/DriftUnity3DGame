using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    [SerializeField]
    private bool go_turn_to_road;

    void Start()
    {
        go_turn_to_road = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "road" || other.gameObject.tag == "road_for_bots" )
        {
            go_turn_to_road = true;
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "road" || other.gameObject.tag == "road_for_bots")
        {
            go_turn_to_road = false;
        }
        //if (other.gameObject.tag == "house")
        //{
        //    go_turn_to_road = true;
        //}
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "house")
    //    {
    //        go_turn_to_road = true;
    //    }
    //}

    public bool GetTurnToRoad()
    {
        return go_turn_to_road;
    }
}
