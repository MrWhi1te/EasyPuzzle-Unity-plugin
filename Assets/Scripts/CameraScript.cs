using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    void Start()
    {
        float targetAspect = 9f / 16f; // Желаемое соотношение сторон (высота / ширина)
        float currentAspect = (float)Screen.height / Screen.width; // Текущее соотношение сторон

        // Расчет нового размера камеры, чтобы объекты оставались видимыми на всех устройствах
        float differenceInSize = currentAspect / targetAspect;
        Camera.main.orthographicSize *= differenceInSize;
    }
}
