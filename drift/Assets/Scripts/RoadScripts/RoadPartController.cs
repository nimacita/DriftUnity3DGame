using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPartController : MonoBehaviour
{

    private RoadController road_controller;
    private PlayerController player_controller;

    public GameObject start_pos,end_pos;
    public GameObject start_let_border, end_let_border;
    public Houses houses;

    [SerializeField]
    private float timer;
    [SerializeField]
    private bool is_time;
    private bool is_delete;
    private bool on_player;
    [SerializeField]
    private float over_timer;
    [SerializeField]
    private int road_num;


    void Awake()
    {
        StartValues();
    }

    public void StartValues()
    {
        road_controller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<RoadController>();
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        road_num = road_controller.GetRoadNum();
        timer = 2f;
        is_time = is_delete = false;
        on_player = false;

        CalcOverTime();
    }

    
    void FixedUpdate()
    {
        CountToDestroy();
    }

    private void CountToDestroy()//отсчет до дизактивации
    {
        if (is_time && !player_controller.GetIsEndGame() && !player_controller.GetIsPause() && !on_player)
        {
            timer -= Time.deltaTime;
        }

        if (over_timer >= 0f && !is_time && !player_controller.GetIsEndGame() && !player_controller.GetIsPause() && !on_player)
        {
            over_timer -= Time.deltaTime;
        }

        if ((timer <= 0f || over_timer <= 0f) && !is_delete)
        {
            road_controller.ChooseRoadPart();
            road_controller.MinusRoadNum();
            gameObject.SetActive(false);
            is_delete = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            is_time = true;
            on_player = false;
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            on_player = true;
            is_time = false;
        }
    }

    private void CalcOverTime()
    {
        //if (road_num > 5)
        //{
        //    over_timer = road_num * 4;
        //}
        //else
        //{
        //    over_timer = 20f;
        //}
        over_timer = 30f;
    }

    public Transform GetEndPos()
    {
        return end_pos.transform;
    }

    public bool GetIsDelete()
    {
        return is_delete;
    }

    public void ReSpawn()
    {
        timer = 5f;
        is_time = is_delete = false;
        on_player = false;
        road_num = road_controller.GetRoadNum();
        CalcOverTime();
        houses.ReSpawn();
    }

    public Transform GetStartPos()
    {
        return start_pos.transform;
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public Vector3 GetStartBorderTransform()
    {
        if (start_let_border != null) {
            return start_let_border.transform.position;
        }
        else
        {
            start_let_border = transform.Find("start_let_s_border").gameObject;
            return start_let_border.transform.position;
        }
    }

    public Vector3 GetEndBorderTransform()
    {
        if (end_let_border != null)
        {
            return end_let_border.transform.position;
        }
        else
        {
            end_let_border = transform.Find("end_let_s_border").gameObject;
            return end_let_border.transform.position;
        }
    }

}
