using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHoverGateCard3 : MonoBehaviour
{
    
    public Animator Card3Anim;
    
    public void OnMouseOver()
    {
        
        Card3Anim.SetBool("isCard3Hovered", true);
        
    }

    public void OnMouseExit()
    {
        
        Card3Anim.SetBool("isCard3Hovered", false);
       
    }

}
