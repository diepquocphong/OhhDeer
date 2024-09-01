using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

[Version(1, 0, 0)]
[Title("is Apple Device")]
[Description("Returns true if running on an Apple device: iPhone, iPad or Mac.")]
[Category("Device")]
[Keywords("Apple", "Device", "OS")]
[Serializable]
public class isAppleDevice : Condition
{
    protected override bool Run(Args args)
    {
        var OS = SystemInfo.operatingSystem;

        if (OS.StartsWith("iPhone") || OS.StartsWith("iPad") || OS.StartsWith("Mac"))
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
