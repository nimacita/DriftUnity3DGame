using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T90LetController : MonoBehaviour
{
    [SerializeField]
    private GameObject l_conus, r_conus;
    private float max_pos_translate;
    private Vector3 dir;
    private Transform l_conus_trans, r_conus_trans;
    private bool is_restart;

    void Start()
    {
        dir = new Vector3(0f,0f,1f);
        max_pos_translate = 1.6f;
        is_restart = false;

        DefineConus();

        l_conus_trans = l_conus.transform;
        r_conus_trans = r_conus.transform;

        ConusMove(l_conus);
        ConusMove(r_conus);

        l_conus.SetActive(false);
        r_conus.SetActive(false);
    }
    

    private void ConusMove(GameObject conus)
    {
        float rand_rot_y = Random.Range(0f,359f);
        conus.transform.rotation =  Quaternion.AngleAxis(conus.transform.rotation.eulerAngles.y + rand_rot_y, new Vector3(0f, 1f, 0f));
        float rand_move_koef = Random.Range(0f,max_pos_translate);
        conus.transform.Translate(dir * rand_move_koef);
    }

    private void DefineConus()
    {
        l_conus = transform.Find("L_conus").gameObject;
        r_conus = transform.Find("R_conus").gameObject;
    }

    public void ReSpawn()
    {
        if (is_restart) {
            l_conus.transform.position = l_conus_trans.position;
            l_conus.transform.rotation = l_conus_trans.rotation;
            r_conus.transform.position = r_conus_trans.position;
            r_conus.transform.rotation = r_conus_trans.rotation;

            ConusMove(l_conus);
            ConusMove(r_conus);
        }
        is_restart = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "sphere_trig")
        {
            l_conus.SetActive(true);
            r_conus.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "sphere_trig")
        {
            l_conus.SetActive(false);
            r_conus.SetActive(false);
        }
    }
}
