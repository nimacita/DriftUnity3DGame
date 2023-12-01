using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadStartController : MonoBehaviour
{

    private RoadController road_controller;
    private PlayerController player_controller;
    [SerializeField]
    private float timer;
    private bool is_time;
    private bool is_delete;

    void Start()
    {
        StartValues();
    }

    public void StartValues()
    {
        road_controller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RoadController>();
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        timer = 3f;
        is_delete = false;
        is_time = true;
    }


    void FixedUpdate()
    {
        if (is_time && !player_controller.GetIsEndGame() && !player_controller.GetIsPause())
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0f && !is_delete)
        {
            road_controller.ChooseRoadPart();
            Destroy(gameObject);
            is_delete = true;
        }
    }

}
