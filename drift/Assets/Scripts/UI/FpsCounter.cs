using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [SerializeField]
    private int frame_range = 60;

    private int[] fps_buffer;
    private int fps_buffer_index;

    public int AverageFPS { get; private set; }

    private void Update()
    {
        //AverageFPS = (int)(1f / Time.unscaledDeltaTime);//фактический фпс
        AverageFpsCount();//средний фпс
    }

    private void AverageFpsCount()
    {
        if (fps_buffer == null || frame_range != fps_buffer.Length)
        {
            InitializeBuffer();
        }
        UpdateBuffer();
        CalculateFps();
    }

    private void InitializeBuffer()
    {
        if (frame_range <=0)
        {
            frame_range = 1;
        }
        fps_buffer = new int[frame_range];
        fps_buffer_index = 0;
    }

    private void UpdateBuffer()
    {
        fps_buffer[fps_buffer_index++] = (int)(1f / Time.unscaledDeltaTime);
        if (fps_buffer_index >= frame_range)
        {
            fps_buffer_index = 0;
        }
    }

    private void CalculateFps()
    {
        int sum = 0;
        for (int i = 0;i<frame_range;i++)
        {
            sum += fps_buffer[i];
        }

        AverageFPS = sum / frame_range;
    }
}
