using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MitsTruckTypeBot : BotsController
{
    public Material[] corpus_colors, carcas_colors;

    public GameObject corpus, mirrors, l_door, r_door, carcase;

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
        min_speed = 2.5f;//2
        max_speed = 8f;//7
        speed_koef = 1.1f;//1.27
        speed_down_koef = 1f;//1

        min_rotatate_speed = 6f;//6
        max_rotate_speed = 80f;//80
        rotate_speed_koef = 45f;//45
        rot_to_speed_koef = 7f;//7

        min_rotatate_speed_let = 10f;//10
        max_rotate_speed_let = 90f;//90
        rotate_speed_koef_let = 50f;//50
        rot_to_speed_koef_let = 25f;//15

        disk_speed_koef = 50f;
        wheel_rot_speed = 15f;
        wheel_rot_angle = 12f;
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
        mirrors.GetComponent<Renderer>().material = corpus_colors[rand_color];
        //int rand_corpus_color = Random.Range(0, carcas_colors.Length);
        //carcase.GetComponent<Renderer>().material = carcas_colors[rand_corpus_color];
        //l_door.GetComponent<Renderer>().material = carcas_colors[rand_corpus_color];
        //r_door.GetComponent<Renderer>().material = carcas_colors[rand_corpus_color];
    }
}
