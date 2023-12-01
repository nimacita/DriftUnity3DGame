using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCarFrontSens : MonoBehaviour
{

    private bool is_enter;

    void Start()
    {
        is_enter = false;
    }


    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "dynamic_let_first" || other.gameObject.tag == "Player")
        {
            is_enter = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "dynamic_let_first" || other.gameObject.tag == "Player")
        {
            is_enter = true;
        }
    }

    public bool GetIsEnter()
    {
        return is_enter;
    }
}
