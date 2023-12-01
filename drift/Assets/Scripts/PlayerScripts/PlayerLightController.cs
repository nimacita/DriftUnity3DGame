using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightController : MonoBehaviour
{

    private PlayerController play_controller;

    private GameObject lights;
    private GameObject back_glass, front_glass;
    private GameObject front_light, back_light;
    private GameObject l_turn_glass, r_turn_glass;
    private Renderer front_glass_ren, back_glass_ren, r_turn_glass_ren, l_turn_glass_ren;
    private float turn_speed;
    private bool anim_value;

    public Material front_glass_day, front_glass_night;
    public Material back_glass_all_time, back_glass_stop;
    public Material turn_glass_all_time, turn_glass_active;

    private DayOrNightController day_or_night_controller;

    private bool is_day;

    void Start()
    {
        AllStartValues();
        FrontLightColor();
    }

    private void AllStartValues()
    {
        play_controller = GetComponent<PlayerController>();

        day_or_night_controller = GameObject.Find("DayCiclyController").GetComponent<DayOrNightController>();

        is_day = day_or_night_controller.GetIsDay();

        anim_value = false;
        turn_speed = 0.4f;
        InvokeRepeating("TicBoolValue", 0.6f, turn_speed);

        back_light.SetActive(false);
        front_light.SetActive(false);

        r_turn_glass_ren.material = turn_glass_all_time;
        l_turn_glass_ren.material = turn_glass_all_time;
    }

    void FixedUpdate()
    {
        BackLightColor();
        if (play_controller.GetIsEndGame())
        {
            EmergencyLightColor();
        }
    }

    private void FrontLightColor()//контрль передних фонарей
    {
        if (is_day)
        {
            front_glass_ren.material = front_glass_day;
            front_light.SetActive(false);
        }
        else
        {
            front_glass_ren.material = front_glass_night;
            front_light.SetActive(true);
        }
    }

    private void BackLightColor()//контрль задних фонарей
    {
        if (play_controller.GetIsHandbrake())
        {
            back_glass_ren.material = back_glass_stop;
            if (!is_day)
            {
                back_light.SetActive(true);
            }
        }
        else
        {
            back_glass_ren.material = back_glass_all_time;
            if (!is_day)
            {
                back_light.SetActive(false);
            }
        }
    }

    protected void EmergencyLightColor()//аварийка
    {
        if (anim_value)
        {
            r_turn_glass_ren.material = turn_glass_active;
            l_turn_glass_ren.material = turn_glass_active;
        }
        else
        {
            r_turn_glass_ren.material = turn_glass_all_time;
            l_turn_glass_ren.material = turn_glass_all_time;
        }
    }

    protected void TicBoolValue()//возвращение переменной для анимации
    {
        anim_value = !anim_value;
    }

    public void DefineObjects(GameObject body_incline)
    {
        lights = body_incline.transform.Find("Lights").gameObject;

        front_light = lights.transform.Find("front_ligh").gameObject;
        back_light = lights.transform.Find("back_ligh").gameObject;

        front_glass = body_incline.transform.Find("Front_light").gameObject;
        back_glass = body_incline.transform.Find("Back_light").gameObject;

        r_turn_glass = body_incline.transform.Find("R_turn_signal").gameObject;
        l_turn_glass = body_incline.transform.Find("L_turn_signal").gameObject;

        DefineLightsMaterials();
    }

    private void DefineLightsMaterials()//кеширование матерьялов
    {
        front_glass_ren = front_glass.GetComponent<Renderer>();
        back_glass_ren = back_glass.GetComponent<Renderer>();
        r_turn_glass_ren = r_turn_glass.GetComponent<Renderer>();
        l_turn_glass_ren = l_turn_glass.GetComponent<Renderer>();
    }
}
