using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VazTypeBot : BotsController
{
    public Material[] corpus_colors, glasses_colors;

    public GameObject corpus, hood;
    public GameObject glass, hood_glass;

    private int rand_color;

    void Start()
    {
        DefineAllCarObjects();
        DefineAllSensors();
        ChooseCorpusColor();
        DefineAllLights();
        AllStartValues();
        CarStatValues();
    }

    private void CarStatValues()//начальные изменяемые статы автомобиля
    {
        if (rand_color != 2) {
            max_speed = 9f;//9
            speed_koef = 1.85f;//1.85
            rot_to_speed_koef_let = 20f;//20
        }
        min_speed = 5f;//5
        speed_down_koef = 2.18f;//2.18

        min_rotatate_speed = 6f;//6
        max_rotate_speed = 100f;//100
        rotate_speed_koef = 50f;//50
        rot_to_speed_koef = 10f;//10

        min_rotatate_speed_let = 10f;//10
        max_rotate_speed_let = 100f;//100
        rotate_speed_koef_let = 55f;//55

        disk_speed_koef = 50f;
        wheel_rot_speed = 10f;
        wheel_rot_angle = 20f;
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
    }

    public override void ChooseCorpusColor()
    {
        rand_color = Random.Range(0, corpus_colors.Length);
        corpus.GetComponent<Renderer>().material = corpus_colors[rand_color];
        hood.GetComponent<Renderer>().material = corpus_colors[rand_color];
        glass.GetComponent<Renderer>().material = glasses_colors[rand_color];
        hood_glass.GetComponent<Renderer>().material = glasses_colors[rand_color];

        if (rand_color == 2)//если черная с тонировкой то быстрее)
        {
            max_speed = 15f;
            speed_koef = 2f;
            rot_to_speed_koef_let = 40f;
        }
        else
        {
            CarStatValues();
        }
    }
}
