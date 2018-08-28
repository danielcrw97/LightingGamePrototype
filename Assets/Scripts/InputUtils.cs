using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class InputUtils
{
    public static bool CheckForMultipleInputs(params KeyCode[] keys)
    {
        bool flag = false;
        foreach(KeyCode key in keys)
        {
            if(Input.GetKey(key))
            {
                flag = true;
                break;
            }
        }
        return flag;
    }
}

