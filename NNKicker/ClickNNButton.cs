using System;
using ClickLib.Bases;

namespace NNKicker
{
    public class ClickNNButton(IntPtr addon = default) : ClickBase<ClickNNButton>("ChatLog", addon)
    {
        public void PerformButtonClick(IntPtr addon)
        {
            Click(5);
        }

        public void Click(int index)
            => this.FireCallback(3, 5, 0, 0);
    }

}
