using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy2 : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] cardObject = GameObject.FindGameObjectsWithTag("Card2");

        if (cardObject.Length > 1)
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);

    }
}
