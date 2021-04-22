using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy3 : MonoBehaviour
{
    private void Awake()
    {
        GameObject[] cardObject = GameObject.FindGameObjectsWithTag("Card3");

        if (cardObject.Length > 1)
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);

    }
}
