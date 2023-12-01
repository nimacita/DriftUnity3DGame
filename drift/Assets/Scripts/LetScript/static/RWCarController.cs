using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RWCarController : MonoBehaviour
{

    public GameObject conus;
    private float max_pos_translate;
    private Vector3 dir;
    private Transform start_conus_trans;

    void Start()
    {
        dir = new Vector3(0f, 0f, 1f);
        max_pos_translate = 1f;//0.3

        start_conus_trans = conus.transform;
        ConusMove(conus);
    }

    private void ConusMove(GameObject conus)
    {
        float rand_rot_y = Random.Range(0f, 359f);
        conus.transform.rotation = Quaternion.AngleAxis(conus.transform.rotation.eulerAngles.y + rand_rot_y, new Vector3(0f, 1f, 0f));
        float rand_move_koef = Random.Range(0f, max_pos_translate);
        conus.transform.Translate(dir * rand_move_koef);
    }

    public void ReSpawn()
    {
        conus.transform.position = start_conus_trans.position;
        conus.transform.rotation = start_conus_trans.rotation;

        ConusMove(conus);
    }
}
