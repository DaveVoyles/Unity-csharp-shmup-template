/* http://answers.unity3d.com/questions/19122/assert-function.html 
 * Use this to catch errors before they become an issue. Assert will throw an exception and say "Hey, something is wrong here!"
 * It's a sanity check to ensure that all variables are as they should be
 */

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
