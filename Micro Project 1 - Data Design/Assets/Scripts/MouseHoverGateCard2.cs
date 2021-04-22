using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverGateCard2 : MonoBehaviour
{
   
    public Animator Card2Anim;
    

    public void OnMouseOver()
    {
        
        Card2Anim.SetBool("isCard2Hovered", true);
       
    }

    public void OnMouseExit()
    {
       
        Card2Anim.SetBool("isCard2Hovered", false);
       
    }

}
