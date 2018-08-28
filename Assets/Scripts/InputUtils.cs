using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class InputUtils
{
    public static KeyCode CheckForMultipleInputs(params KeyCode[] keys)
    {
        KeyCode code = KeyCode.None;
        foreach(KeyCode key in keys)
        {
            if(Input.GetKey(key))
            {
                code = key;
                break;
            }
        }
        return code;
    }
}

