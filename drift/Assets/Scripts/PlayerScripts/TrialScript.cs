using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialScript : MonoBehaviour
{
    private PlayerController player_controller;
    private DayOrNightController day_or_night;
    private TrailRenderer trai_rend;
    public GameObject smoke;
    private ParticleSystem ps;
    private bool is_ground;
    private float time_to_delete;
    private float delete_koef;
    private Ray ray;



    void Start()
    {
        AllStartValue();
    }

    private void AllStartValue()
    {
        trai_rend = GetComponent<TrailRenderer>();
        trai_rend.emitting = false;
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        day_or_night = GameObject.FindGameObjectWithTag("don").GetComponent<DayOrNightController>();
        ps = smoke.GetComponent<ParticleSystem>();
        ps.Stop();
        is_ground = true;
        time_to_delete = 5f;
        trai_rend.time = time_to_delete;
        delete_koef = 30f;

    }
    
    void FixedUpdate()
    {
        TrialOnDrift();
        AfterEndGame();
        RaySettings();
    }

    private void RaySettings()//нстройки луча
    {
        ray = new Ray(transform.position, -transform.right);

        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,1)) {
            if (hit.collider.gameObject.CompareTag("road"))
            {
                is_ground = true;
            }
            else
            {
                is_ground = false;
            }
        }
        else
        {
            is_ground = false;
        }
    }

    private void TrialOnDrift()//триал и дым во время дрифта и ручника
    {
        if((player_controller.GetIsDrift() || player_controller.GetIsHandbrake()) && is_ground)
        {
            trai_rend.emitting = true;
        }
        else
        {
            trai_rend.emitting = false;
        }

        if (player_controller.GetIsDrift() && is_ground && !player_controller.GetIsEndGame() && !day_or_night.GetIsRain())
        {
            ps.Play();
        }
        else
        {
            ps.Stop();
        }
    }

    private void AfterEndGame()//на паузе и в оканчании игры
    {
        if (player_controller.GetIsEndGame() || player_controller.GetIsPause())
        {
            trai_rend.time = float.PositiveInfinity;
        }
        else
        {
            if (trai_rend.time == float.PositiveInfinity)
            {
                trai_rend.time = 99f;
            }
            if (trai_rend.time > time_to_delete)
            {
                trai_rend.time -= Time.deltaTime * delete_koef;
            }
            if (trai_rend.time < time_to_delete)
            {
                trai_rend.time = time_to_delete;
            }
            //trai_rend.time = time_to_delete;
        }

        if (player_controller.GetIsPause())
        {
            ps.Pause();
        }
    }

}
