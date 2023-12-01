using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Houses : MonoBehaviour
{

    public GameObject[] structures;

    public void ReSpawn()
    {
        for (int i = 0; i < structures.Length; i++) 
        {
            structures[i].SetActive(true);
            structures[i].GetComponent<HouseController>().RespawnStructure();
        }
    }

}
