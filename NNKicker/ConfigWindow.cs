using System;
using System.Threading.Tasks;
using Dalamud.Game.AddonLifecycle;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Colors;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using ClickLib.Clicks;
using Dalamud.Game.Config;
using Dalamud.Interface.Components;

namespace NNKicker;

public class ConfigWindow : Window, IDisposable
{
    private int kickTimes;
    private bool isOnKicking;
    private bool stopFlag;
    private uint originalVolume;
    private bool isMuteSystemSound = true;
    private int kickFrequency = 500;

    public ConfigWindow(Plugin plugin) : base(
        "Novice Network Kicker",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
    }

    public override void Draw()
    {
        ImGui.SetWindowFontScale(1.5f);
        if (!isOnKicking)
        {
            if (ImGui.Button("一键开挤"))
            {
                StartKickerHandler();
            }
        }
        else
        {
            if (ImGui.Button("停止"))
            {
                EndKickerHandler();
            }
        }
        ImGui.SetWindowFontScale(1f);

        ImGui.SameLine();
        ImGui.TextColored(ImGuiColors.DalamudYellow, "尝试次数:");

        ImGui.SameLine();
        ImGui.Text(kickTimes.ToString());

        ImGui.Separator();

        ImGui.Checkbox("过程中禁用系统音", ref isMuteSystemSound);
        ImGuiComponents.HelpMarker("成功挤入/停止后将会自动恢复原音量");

        ImGui.SetNextItemWidth(150f);
        ImGui.InputInt("尝试间隔 (单位: ms)", ref kickFrequency);
        ImGuiComponents.HelpMarker("过低的间隔不会让你的成功率变高\n" +
                                   "只会增加你的电脑负荷\n" +
                                   "推荐间隔: 500ms 至 1000ms");
    }

    private void StartKickerHandler()
    {
        isOnKicking = true;
        kickTimes = 0;
        stopFlag = false;

        Plugin.GameConfig.TryGet(SystemConfigOption.SoundSystem, out originalVolume);
        if (isMuteSystemSound) Plugin.GameConfig.System.Set("SoundSystem", 0);

        Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "SelectYesno", ClickYesButton);

        ClickNoviceNetworkButton();
    }

    private void EndKickerHandler()
    {
        stopFlag = true;
        isOnKicking = false;

        Plugin.GameConfig.System.Set("SoundSystem", originalVolume);

        Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "SelectYesno", ClickYesButton);
    }

    public unsafe void ClickNoviceNetworkButton()
    {
        var cln = Plugin.GameGui.GetAddonByName("ChatLog");
        var cl = (AtkUnitBase*)cln;
        var clickNnButton = new ClickNNButton(cln);

        if (cl != null && cl->RootNode != null && cl->RootNode->ChildNode != null &&
            cl->UldManager.NodeList != null)
        {
            var buttonNode = cl->GetComponentNodeById(12);
            if (buttonNode != null)
            {
                clickNnButton.PerformButtonClick(cln);
                kickTimes++;
            }
        }

        Task.Delay(kickFrequency).ContinueWith(t => CheckJoinState());
    }

    public void ClickYesButton(AddonEvent type, AddonArgs args)
    {
        var ui = args.Addon;
        if (ui != nint.Zero)
        {
            var clickSelectOk = new ClickSelectOk(ui);
            clickSelectOk.Ok();
        }
    }

    private void CheckJoinState()
    {
        if (stopFlag)
        {
            EndKickerHandler();
            stopFlag = false;
            return;
        }

        var BGNL = Plugin.GameGui.GetAddonByName("BeginnerChatList");
        if (BGNL != nint.Zero)
        {
            EndKickerHandler();
            stopFlag = false;
        }
        else ClickNoviceNetworkButton();
    }

    public void Dispose()
    {
        Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "SelectYesno", ClickYesButton);
    }
}
