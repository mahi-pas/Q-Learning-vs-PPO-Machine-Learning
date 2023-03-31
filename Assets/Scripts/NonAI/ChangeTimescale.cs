using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeTimescale : MonoBehaviour
{
    public void ChangeScale(string value){
        try{
            Time.timeScale = float.Parse(value);
        }
        catch{}
    }
}
