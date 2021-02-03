using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutSwitch : MonoBehaviour
{
    public bool IsPro;
    public GameObject Pro, NoPro;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsPro = !IsPro;
            Pro.SetActive(IsPro);
            NoPro.SetActive(!IsPro);
        }
    }
}
