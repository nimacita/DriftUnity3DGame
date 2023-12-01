using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToLetController : MonoBehaviour
{
    [SerializeField]
    private bool go_turn_to_let;

    void Start()
    {
        go_turn_to_let = false;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "let" || other.gameObject.tag == "dynamic_let_first"
            || other.gameObject.tag == "Player")
        {
            go_turn_to_let = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "let" || other.gameObject.tag == "dynamic_let_first"
            || other.gameObject.tag == "Player")
        {
            go_turn_to_let = true;
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "let" || other.gameObject.tag == "dynamic_let_first"
            || other.gameObject.tag == "Player")
        {
            go_turn_to_let = true;
        }
    }

    public bool GetTurnToLet()
    {
        return go_turn_to_let;
    }
}
