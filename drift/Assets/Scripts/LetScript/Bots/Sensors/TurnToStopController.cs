using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToStopController : MonoBehaviour
{
    private bool stop_turn;

    void Start()
    {
        stop_turn = false;
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "dynamic_let_first" /*|| other.gameObject.tag == "let"*/)
        {
            stop_turn = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "dynamic_let_first" /*|| other.gameObject.tag == "let"*/)
        {
            stop_turn = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "dynamic_let_first" /*|| other.gameObject.tag == "let"*/)
        {
            stop_turn = true;
        }
    }

    public bool GetTurnToStop()
    {
        return stop_turn;
    }
}
