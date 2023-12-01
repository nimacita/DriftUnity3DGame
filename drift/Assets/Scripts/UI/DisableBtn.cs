using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableBtn : MonoBehaviour
{

    public bool is_anim;



    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void EndAnim()
    {
        is_anim = false;
    }

    public void StartAnim()
    {
        is_anim = true;
    }
}
