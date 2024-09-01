using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine.UI;
using UnityEngine;

[Version(0, 0, 1)]

[Title("Change Slider Min Value ")]
[Description("Change Slider Min Value")]
[Category("UI/Change Slider Min Value")]
[Image(typeof(IconUISlider), ColorTheme.Type.TextLight)]

[Serializable]
public class InstructionSliderMinValue : Instruction
{

    // MEMBERS: -------------------------------------------------------------------------------
    [SerializeField] private Slider slider;

    [SerializeField] private PropertyGetDecimal _minValue = new PropertyGetDecimal();


    // TITLE: ---------------------------------------------------------------------------------

    public override string Title => $"Change Slider Min Value";


    // RUN METHOD: ----------------------------------------------------------------------------

    protected override Task Run(Args args)
    {
        slider.minValue = (float)_minValue.Get(args);

        return DefaultResult;
    }
}
