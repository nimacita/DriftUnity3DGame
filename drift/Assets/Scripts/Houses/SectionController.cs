using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionController : MonoBehaviour
{

    public GameObject[] windows_frame;
    public GameObject glasses, hous_body;

    public void EnableWindow(int window_ind)//включаем определенные фасады по переданому значению
    {
        windows_frame[window_ind].SetActive(true);
    }

    public void DisableAllObjects()//отключаем все объекты
    {
        for (int i =0;i<windows_frame.Length;i++)
        {
            windows_frame[i].SetActive(false);
        }
    }

    public int GetWindowsLength()
    {
        return windows_frame.Length;
    }
}
