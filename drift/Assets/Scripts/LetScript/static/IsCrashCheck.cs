using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCrashCheck : MonoBehaviour
{

    private PlayerController player_controller;

    void Start()
    {
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            player_controller.SetEndGame();
        }
        if (other.gameObject.tag == "dynamic_let_first")
        {
            other.gameObject.GetComponent<BotsController>().HasCrush();
        }
    }

}
