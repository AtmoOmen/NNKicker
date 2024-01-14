using System;
using ClickLib.Bases;

namespace NNKicker;

public class ClickNNButton(IntPtr addon = default) : ClickBase<ClickNNButton>("ChatLog", addon)
{
    public void PerformButtonClick()
    {
        Click(3);
    }

    public void Click(int index)
    {
        FireCallback(index);
    }
}
