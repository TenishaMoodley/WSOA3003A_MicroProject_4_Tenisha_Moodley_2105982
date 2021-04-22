using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverInfo : MonoBehaviour
{
    public GameObject Info;


    public void OnMouseOver()
    {

        Info.SetActive(true);
    }

    public void OnMouseExit()
    {
        Info.SetActive(false);
    }
}
