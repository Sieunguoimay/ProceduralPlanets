using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Text fps;
    void Update()
    {
        float f = 1 / Time.deltaTime;
        fps.text = "FPS: "+f.ToString();
    }
}
