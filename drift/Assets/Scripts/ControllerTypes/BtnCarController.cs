using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BtnCarController : MonoBehaviour
{

    private PlayerController player_controller;
    private DayOrNightController day_or_night;
    public MainUiController main_ui;

    private int direction;
    private float rotation_speed;
    [SerializeField]
    private float speed_koef;
    private bool is_touch;
    [SerializeField]
    private bool is_touch_left, is_touch_right;
    private bool is_calculate;
    private float min_rot_speed, max_rot_speed;
    private float rain_speed_koef_add;//прибавление к скорости поворота во время дождя
    private float buildup_koef;//коэфициент раскачки остаток снижеие рот спиад
    private bool is_add, is_down;
    private bool pause_disble;


    public GameObject left_btn, right_btn;
    public GameObject handbrake_btn;


    void Start()
    {
        StartValues();
    }

    private void StartValues()
    {
        player_controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        day_or_night = GameObject.FindGameObjectWithTag("don").GetComponent<DayOrNightController>();

        if (player_controller.is_btn_controll) {
            left_btn.SetActive(true);
            right_btn.SetActive(true);
            handbrake_btn.SetActive(true);
        }

        is_touch = false;
        is_touch_left = is_touch_right = false;
        direction = 1;
        rotation_speed = 0;
        is_calculate = false;
        pause_disble = false;

        is_add = false;
        is_down = true;
        
    }

    private void FixedUpdate()
    {
        IsCalculate();
        SpeedCalculate();
        RaindAdd();
        IsPause();
    }

    private void RaindAdd()
    {
        rain_speed_koef_add = 15;
        if (day_or_night.GetIsRain())
        {
            if (!is_add)
            {
                speed_koef += rain_speed_koef_add;
                is_down = false;
                is_add = true;
            }
        }
        else
        {
            if (!is_down)
            {
                speed_koef -= rain_speed_koef_add;
                is_add = false;
                is_down = true;
            }
        }
        
    }

    private void SpeedCalculate()
    {
        if (is_calculate)
        {
            if (rotation_speed < min_rot_speed)
            {
                rotation_speed = min_rot_speed;
            }
            if (rotation_speed < max_rot_speed)
            {
                rotation_speed += Time.deltaTime * speed_koef;
            }
            else
            {
                rotation_speed = max_rot_speed;
            }
        }
        else
        {
            if (rotation_speed > 0f)
            {
                rotation_speed -= Time.deltaTime * speed_koef;
            }
            else
            {
                rotation_speed = 0f;
            }
            //rotation_speed = 0f;
        }
    }

    private void IsCalculate()
    {
        if ((is_touch_left || is_touch_right) && main_ui.GetIsEndCount())
        {
            is_calculate = true;
        }
        else
        {
            is_calculate = false;
            //rotation_speed = 0;
        }
    }

    public void LeftBtnDown()
    {
        direction = -1;
        //rotation_speed = 0;
        is_touch_left = true;

        is_touch_right = false;
    }

    public void RightBtnDown()
    {
        direction = 1;
        //rotation_speed = 0;
        is_touch_right = true;

        is_touch_left = false;
    }

    public void LeftBtnUp()
    {
        is_touch_left = false;
        //rotation_speed = 0;
        direction = 1;

        BtnUp();
    }

    public void RightBtnUp()
    {
        is_touch_right = false;
        //rotation_speed = 0;
        direction = -1;

        BtnUp();
    }

    private void BtnUp()
    {
        if (rotation_speed > buildup_koef)
        {
            rotation_speed = buildup_koef;
        }
    }

    private void IsPause()
    {
        if (main_ui.GetIsPause() ) 
        {
            if (!pause_disble) {
                is_touch_left = is_touch_right = false;
                rotation_speed = 0;
                pause_disble = true;
            }
        }
        else
        {
            pause_disble = false;
        }
    }

    public bool GetIsTouch()
    {
        if (is_touch_left || is_touch_right)
        {
            is_touch = true;
        }
        else
        {
            is_touch = false;
        }
        return is_touch;
    }

    public int GetDirection()
    {
        return direction;
    }

    public float GetRotateSpeed()
    {
        return rotation_speed;
    }

    public void SetCarsValue(float set_min_speed, float set_max_speed, float set_speed_koef, float set_buildup_koef)
    {
        min_rot_speed = set_min_speed;
        max_rot_speed = set_max_speed;
        speed_koef = set_speed_koef;
        buildup_koef = set_buildup_koef;
    }

}
