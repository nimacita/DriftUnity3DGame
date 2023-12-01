using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTypeBots : BotsController
{

    private GameObject body_incline;
    private float max_incline_angle, incline_speed;
    private float incline_border_to_speed, body_incline_angle;

    public Material[] corpus_colors;
    private GameObject corpus, mirrors,wheel_up;

    void Start()
    {
        DefineAllCarObjects();
        DefineAllSensors();
        DefineAllLights();
        ChooseCorpusColor();
        AllStartValues();
        CarStatValues();


        PickupValues();//метод пикапа
    }

    private void CarStatValues()//начальные изменяемые статы автомобиля
    {
        min_speed = 2f;//0.6
        max_speed = 9f;//2.2
        speed_koef = 1.27f;//0.38
        speed_down_koef = 1f;//0.3

        min_rotatate_speed = 6f;//6
        max_rotate_speed = 80f;//80
        rotate_speed_koef = 45f;//45
        rot_to_speed_koef = 7f;//20

        min_rotatate_speed_let = 10f;//10
        max_rotate_speed_let = 90f;//90
        rotate_speed_koef_let = 50f;//50
        rot_to_speed_koef_let = 15f;//40

        disk_speed_koef = 50f;
        wheel_rot_speed = 15f;
        wheel_rot_angle = 12f;

    }

    private void PickupValues()
    {
        max_incline_angle = 1f;
        incline_speed = 0.4f;
        incline_border_to_speed = max_speed / 2f;
    }

    private void FixedUpdate()
    {
        SpeedCalculate();
        CalculateRotateSpeed();
        CarMove();
        GetTurns();
        WheelRotate();

        AllLightController();

        IsDestroy();

        BodyIncline();
        BodyInclineStraight();
    }

    private void BodyIncline()//наклон корпуса
    {
        body_incline_angle = body_incline.transform.localEulerAngles.x;
        if (body_incline_angle > 180f)
        {
            body_incline_angle -= 360f;
        }
        if (body_incline_angle > -max_incline_angle && speed >= incline_border_to_speed)
        {
            body_incline.transform.Rotate(new Vector3(1, 0, 0) * incline_speed * -1 * Time.deltaTime);
        }
    }

    private void BodyInclineStraight()//выпрямление корпуса после наклона
    {
        body_incline_angle = body_incline.transform.localEulerAngles.x;
        if (body_incline_angle > 180f)
        {
            body_incline_angle -= 360f;
        }
        if (body_incline_angle < 0f && speed < incline_border_to_speed)
        {
            body_incline.transform.Rotate(new Vector3(1, 0, 0) * incline_speed * 1 * Time.deltaTime);
        }
    }

    public override void ChooseCorpusColor()
    {
        int rand_color = Random.Range(0, corpus_colors.Length);
        corpus.GetComponent<Renderer>().material = corpus_colors[rand_color];
        mirrors.GetComponent<Renderer>().material = corpus_colors[rand_color];
        wheel_up.GetComponent<Renderer>().material = corpus_colors[rand_color];
    }

    protected new void DefineAllLights()
    {
        body_incline = transform.Find("Body_incline_back").gameObject;

        lights = body_incline.transform.Find("Lights").gameObject;
        front_light = lights.transform.Find("front_ligh").gameObject;
        back_light = lights.transform.Find("back_ligh").gameObject;
        front_glass = body_incline.transform.Find("Front_light").gameObject;
        back_glass = body_incline.transform.Find("Back_light").gameObject;
        front_glass_mat = front_glass.GetComponent<Renderer>().material;
        back_glass_mat = back_glass.GetComponent<Renderer>().material;

        corpus = body_incline.transform.Find("Body").gameObject;
        mirrors = body_incline.transform.Find("Body_mirror").gameObject;
        wheel_up = body_incline.transform.Find("Body_wheel_up").gameObject;

        l_turn_glass = body_incline.transform.Find("L_turn_signal").gameObject;
        r_turn_glass = body_incline.transform.Find("R_turn_signal").gameObject;
        l_turn_glass_mat = l_turn_glass.GetComponent<Renderer>().material;
        r_turn_glass_mat = r_turn_glass.GetComponent<Renderer>().material;
    }


}
