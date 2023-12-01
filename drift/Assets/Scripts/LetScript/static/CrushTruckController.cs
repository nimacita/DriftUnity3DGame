using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushTruckController : MonoBehaviour
{

    private GameObject body;
    private GameObject left_door;
    private GameObject boxes;
    private GameObject planks;
    private GameObject dyn_box1, dyn_box2;
    private GameObject emergency_sign;
    private GameObject right_door;
    private GameObject carcass;
    private Animation left_door_anim;

    public Material[] carcas_color;

    private Vector3 boxes_dir;
    private float max_pos_translate;

    private Transform start_box1, start_box2, start_em_sign;

    void Awake()
    {
        DefineAllObjects();
        AllStartValues();
        DefineRandCargo();
        ChooseCarcasColor();
    }

    private void AllStartValues()
    {
        max_pos_translate = 0.7f;//для смещения коробок
        boxes_dir = new Vector3(0f, 0f, 1f);


        start_box1 = dyn_box1.transform;
        start_box2 = dyn_box2.transform;
        start_em_sign = emergency_sign.transform;

        planks.SetActive(false);
        boxes.SetActive(false);
        BoxMove(emergency_sign);
    }

    private void DefineRandCargo()
    {
        int rand_cargo = Random.Range(0, 2);
        switch (rand_cargo)
        {
            case 0:
                PlanksCargo();
                break;
            case 1:
                BoxesCargo();
                break;
            default:
                BoxesCargo();
                break;
        }
    }

    private void PlanksCargo()
    {
        planks.SetActive(true);
    }

    private void BoxesCargo()
    {
        boxes.SetActive(true);
        BoxMove(dyn_box1);
        BoxMove(dyn_box2);
    }

    private void ChooseCarcasColor()
    {
        int rand_color = Random.Range(0, carcas_color.Length);
        right_door.GetComponent<Renderer>().material = carcas_color[rand_color];
        left_door.GetComponent<Renderer>().material = carcas_color[rand_color];
        carcass.GetComponent<Renderer>().material = carcas_color[rand_color];
    }

    private void WooblyDoor()
    {
        left_door_anim.Play("woobly_door");
    }

    private void DefineAllObjects()
    {
        body = transform.Find("Body").gameObject;
        left_door = body.transform.Find("Left_door").gameObject;
        right_door = body.transform.Find("Right_door").gameObject;
        carcass = body.transform.Find("Сarcass").gameObject;
        planks = body.transform.Find("Planks").gameObject;
        boxes = body.transform.Find("Boxes").gameObject;

        left_door_anim = left_door.GetComponent<Animation>();

        dyn_box1 = boxes.transform.Find("DynBox1").gameObject;
        dyn_box2 = boxes.transform.Find("DynBox2").gameObject;
        emergency_sign = transform.Find("EmergencySign").gameObject;
    }

    private void BoxMove(GameObject box)//рандомное передвижение коробок
    {
        float rand_rot_y = Random.Range(-30f, 30f);
        float rand_move_koef = Random.Range(0f, max_pos_translate);
        box.transform.Rotate(new Vector3(0f, 1f, 0f) * rand_rot_y);
        box.transform.Translate(boxes_dir * rand_move_koef);
    }

    public void ReSpawn()
    {
        planks.SetActive(false);
        boxes.SetActive(false);

        dyn_box1.transform.position = start_box1.position;
        dyn_box1.transform.rotation = start_box1.rotation;
        dyn_box2.transform.position = start_box2.position;
        dyn_box2.transform.rotation = start_box2.rotation;
        emergency_sign.transform.position = start_em_sign.position;
        emergency_sign.transform.rotation = start_em_sign.rotation;

        BoxMove(emergency_sign);
        DefineRandCargo();
        ChooseCarcasColor();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "dynamic_let_first")
        {
            WooblyDoor();
        }

    }
}
