/* http://answers.unity3d.com/questions/19122/assert-function.html */

using System;
using System.Diagnostics;
using UnityEngine;
using System.Collections;
 
public class DebugUtils
{
    [Conditional("DEBUG")]
    public static void Assert(bool condition)
    {
        if (!condition) throw new Exception();
    }
}
