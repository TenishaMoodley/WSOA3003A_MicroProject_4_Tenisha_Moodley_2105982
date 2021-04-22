using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy5 : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] cardObject = GameObject.FindGameObjectsWithTag("Card5");

        if (cardObject.Length > 1)
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);

    }
}
