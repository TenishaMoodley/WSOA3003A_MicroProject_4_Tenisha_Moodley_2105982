using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverGateCard : MonoBehaviour
{
    public Animator Card1Anim;
    

    public void OnMouseOver()
    {
       
        Card1Anim.SetBool("isCard1Hovered", true);
       
    }

    public void OnMouseExit()
    {
        
        Card1Anim.SetBool("isCard1Hovered", false);
        
    }

}
