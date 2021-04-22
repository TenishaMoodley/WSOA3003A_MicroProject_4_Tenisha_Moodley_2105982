using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy4 : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] cardObject = GameObject.FindGameObjectsWithTag("Card4");

        if (cardObject.Length > 1)
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);

    }
}
