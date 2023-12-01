using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetPartController : MonoBehaviour
{

    private float y_delete_border;

    void Start()
    {
        y_delete_border = -3f;
    }


    void FixedUpdate()
    {
        IsDestroy();
    }

    private void IsDestroy()
    {
        if (transform.position.y < y_delete_border)
        {
            gameObject.SetActive(false);
        }
    }


}
