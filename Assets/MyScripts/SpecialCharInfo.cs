using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCharInfo : DigitInfo
{
    [SerializeField] string named;
     public enum SpecialChar
     {
        None,
        BackSpace,
        Del,
        CapsToggle,
        ArrowUp,
        ArrowDown,
        ArrowLeft,
        ArrowRight,
        Enter
     }

    [SerializeField] SpecialChar specialCharacter;
}
