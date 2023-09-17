namespace GodotUtils.Deprecated;

using Godot;
using System;
using System.Linq;

public static class UIHelpers
{
    /// <summary>
    /// Create several sliders for debugging purposes
    /// </summary>
    public static void CreateDebugSliders(Node parent,
        Action<float>[] valueChanged, int minValue = -1000, int maxValue = 1000)
    {
        var vbox = new VBoxContainer();
        vbox.Name = "VBox";
        vbox.AddThemeConstantOverride("separation", 0);

        parent.AddChild(vbox);

        for (int i = 0; i < valueChanged.Count(); i++)
        {
            var slider = new UISlider(new SliderOptions
            {
                Name = "Debug",
                HSlider = new HSlider
                {
                    MinValue = minValue,
                    MaxValue = maxValue
                },
                HideBackPanel = false
            });
            slider.ValueChanged += valueChanged[i];
            vbox.AddChild(slider);
        }
    }
}
