using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LexTypeBot : BotsController
{

    public Material[] corpus_colors;

    private GameObject corpus, bampers;

    void Start()
    {
        DefineAllCarObjects();
        DefineAllSensors();
        DefineCorpusParts();
        ChooseCorpusColor();
        DefineAllLights();
        AllStartValues();
        CarStatValues();
    }

    private void CarStatValues()//начальные изменяемые статы автомобиля
    {
        min_speed = 4f;//1.2
        max_speed = 10.7f;//3.2
        speed_koef = 1.85f;//0.55
        speed_down_koef = 2.18f;//0.65

        min_rotatate_speed = 6f;//10
        max_rotate_speed = 100f;//90
        rotate_speed_koef = 50f;//40
        rot_to_speed_koef = 10f;//45

        min_rotatate_speed_let = 10f;//18
        max_rotate_speed_let = 100f;//90
        rotate_speed_koef_let = 55f;//45
        rot_to_speed_koef_let = 13f;//55

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
        int rand_color = Random.Range(0, corpus_colors.Length);
        corpus.GetComponent<Renderer>().material = corpus_colors[rand_color];
        bampers.GetComponent<Renderer>().material = corpus_colors[rand_color];
    }

    private void DefineCorpusParts()
    {
        corpus = transform.Find("Body").gameObject;
        bampers = transform.Find("Bumper").gameObject;
    }
}
