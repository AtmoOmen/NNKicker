using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickLib.Bases;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ClickLib.Enums;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Dalamud.Logging;
using ClickLib.Clicks;

namespace NNKicker
{
    public class ClickNNButton : ClickBase<ClickNNButton>
    {

        public ClickNNButton(IntPtr addon = default)
        : base("ChatLog", addon)
        {
        }

        public void PerformButtonClick(IntPtr addon)
        {
            Click(5);
        }

        public static ClickNNButton Using(IntPtr addon) => new(addon);

        public void Click(int index)
            => this.FireCallback(3, 5, 0, 0);
    }

}
