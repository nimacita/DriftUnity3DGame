using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampPostController : MonoBehaviour
{
    public GameObject sparks;
    private PlayerController player_controller;
    private DayOrNightController day_or_night_controller;
    private int min_time_of_day, max_time_of_day, time_of_day;
    private bool is_broke;
    private bool player_crush, bot_crush;
    private Animation anim;

    void Start()
    {
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        day_or_night_controller = GameObject.Find("DayCiclyController").GetComponent<DayOrNightController>();
        min_time_of_day = 0;
        max_time_of_day = day_or_night_controller.GetMaxTimeOfDay();
        time_of_day = day_or_night_controller.GetTimeOfDay();

        is_broke = false;
        player_crush = bot_crush = false;
        anim = GetComponent<Animation>();
        anim.enabled = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            anim.enabled = true;
            player_controller.SetEndGame();
            if (!player_crush) {
                LightCrushAnim();
                player_crush = true;
            }
        }
        if (other.gameObject.tag == "dynamic_let_first")
        {
            anim.enabled = true;
            other.gameObject.GetComponent<BotsController>().HasCrush();
            if (!bot_crush) {
                LightCrushAnim();
                bot_crush = true;
            }
        }
    }

    private void LightCrushAnim()
    {
        if (!is_broke) {
            anim.Play("lamp_broke");
            if (PlayerPrefs.GetInt("is_day") == 0 || 
                (PlayerPrefs.GetInt("is_day") > 0 && (time_of_day <= min_time_of_day || time_of_day >= max_time_of_day))) {
                sparks.GetComponent<ParticleSystem>().Play();
            }
            is_broke = true;
        }
        else
        {
            anim.Play("lamp_rebroke");
        }
    }
}
