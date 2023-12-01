using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusTypeBot : BotsController
{

    [SerializeField]
    private bool turn_to_bus_stop;
    private GameObject bus_stop_trigger;

    private GameObject full_stop_trigger;
    private bool full_bus_stop;
    private float stop_timer,max_stop_timer;
    private bool can_stop;

    public Material[] corpus_colors,corpus_up_colors;
    private GameObject corpus, up;

    void Start()
    {
        DefineAllCarObjects();
        DefineAllSensors();
        DefineAllLights();
        DefineCorpus();
        DefineBusStopTrigger();//метод автобуса
        ChooseCorpusColor();
        AllStartValues();
        CarStatValues();
        StartValues();//метод автобуса
    }

    private void StartValues()
    {
        turn_to_bus_stop = false;

        full_bus_stop = false;
        can_stop = true;
        max_stop_timer = 8f;
        stop_timer = max_stop_timer;
    }

    private void CarStatValues()//начальные изменяемые статы автомобиля
    {
        min_speed = 3.3f;//1
        max_speed = 5f;//2
        speed_koef = 1f;//0.3
        speed_down_koef = 4f;//2

        min_rotatate_speed = 1f;//1
        max_rotate_speed = 70f;//80
        rotate_speed_koef = 24f;//20
        rot_to_speed_koef = 1.2f;//5

        min_rotatate_speed_let = 2f;//2
        max_rotate_speed_let = 80f;//80
        rotate_speed_koef_let = 35f;//35
        rot_to_speed_koef_let = 2f;//6

        disk_speed_koef = 50f;
        wheel_rot_speed = 15f;
        wheel_rot_angle = 12f;

    }

    private void FixedUpdate()
    {
        StopTimer();//метод автобуса
        IsSpeedCalculate();//метод автобуса
        CarRotateBusStop();//метод автобуса
        GetTurns();
        GetBusStopTurn();//метод автобуса
        WheelRotate();

        AllLightController();

        IsDestroy();
    }

    private void IsSpeedCalculate()//считаем ли скорость
    {
        if (!full_bus_stop)
        {
            SpeedCalculate();
            CalculateRotateSpeed();
            CarMove();
        }
        else
        {
            speed = 0f;
        }
    }

    private void StopTimer()//таймер остановки
    {
        if (full_bus_stop)
        {
            can_stop = false;
            if (stop_timer > 0f)
            {
                stop_timer -= Time.deltaTime;
            }
            else
            {
                stop_timer = 0f;
                full_bus_stop = false;
            }
        }

        if (!can_stop && !full_bus_stop)
        {
            if (stop_timer < max_stop_timer)
            {
                stop_timer += Time.deltaTime;
            }
            else
            {
                stop_timer = max_stop_timer;
                can_stop = true;
            }
        }
    }

    protected new void CalculateRotateSpeed()//считаем скорость поворота автобуса
    {
        //turn
        if (is_turn_right || is_turn_left)
        {
            if (rotate_speed < max_rotate_speed)
            {
                rotate_speed += Time.deltaTime * (rotate_speed_koef + (speed * rot_to_speed_koef));
            }
        }
        else
        {
            rotate_speed = min_rotatate_speed;
        }

        //let
        if (is_turn_left_let || is_turn_right_let || turn_to_bus_stop)
        {
            if (rotate_speed_let < max_rotate_speed_let)
            {
                rotate_speed_let += Time.deltaTime * (rotate_speed_koef_let + (speed * rot_to_speed_koef_let));
                rotate_speed -= Time.deltaTime * rotate_speed_koef / 1.2f;
            }
        }
        else
        {
            rotate_speed_let = min_rotatate_speed_let;
        }
    }

    protected void CarRotateBusStop()//поворот автобуса от остановки
    {
        if (!has_crush && !left_stop_turn && !right_stop_turn && is_go)
        {
            if (turn_to_bus_stop && !is_turn_left_let)//вправо
            {
                is_turn_right_let = true;
                rotate_dir_let = 1;
                transform.Rotate(new Vector3(0, 1, 0) * rotate_speed_let * rotate_dir_let * Time.deltaTime);
            }
            if (!right_turn_to_let && !turn_to_bus_stop || full_bus_stop)
            {
                is_turn_left_let = false;
            }
            if (!left_turn_to_let && !turn_to_bus_stop || full_bus_stop)
            {
                is_turn_right_let = false;
            }
        }
    }

    private void GetBusStopTurn()//получаем значение автобусных сенсоров
    {
        turn_to_bus_stop = bus_stop_trigger.GetComponent<TurnToBusStopController>().GetTurnToBusStop();

        if (can_stop)
        {
            full_bus_stop = full_stop_trigger.GetComponent<FullBusStopController>().GetFullStop();
        }
    }

    private void DefineBusStopTrigger()//определяем автобусные сенсоры
    {
        bus_stop_trigger = sensors.transform.Find("BusStopTrigger").gameObject;
        full_stop_trigger = sensors.transform.Find("FullStopBusTrigger").gameObject;
    }

    public override void ChooseCorpusColor()
    {
        int rand_color = Random.Range(0, corpus_colors.Length);
        corpus.GetComponent<Renderer>().material = corpus_colors[rand_color];
        up.GetComponent<Renderer>().material = corpus_up_colors[rand_color];
    }


    private void DefineCorpus()
    {
        corpus = transform.Find("Down_body").gameObject;
        up = transform.Find("Up_body").gameObject;
    }
}
