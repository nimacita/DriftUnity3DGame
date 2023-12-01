using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusStopController : MonoBehaviour
{

    public GameObject treshcan1, treshcan2;

    void Start()
    {
        treshcan1.SetActive(false);
        treshcan2.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "sphere_trig")
        {
            treshcan1.SetActive(true);
            treshcan2.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "sphere_trig")
        {
            treshcan1.SetActive(false);
            treshcan2.SetActive(false);
        }
    }


}
