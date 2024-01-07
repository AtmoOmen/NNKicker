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
    private uint originalVolume;
    public bool IsMuteSystemSound = true;
    public int KickFrequency = 500;

    public ConfigWindow(Plugin plugin) : base(
        "Novice Network Kicker",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
    }

    public override void Draw()
    {
        ImGui.SetWindowFontScale(1.5f);
        if (ImGui.Button(isOnKicking ? "停止" : "一键开挤"))
        {
            isOnKicking = !isOnKicking;
            HandleKickerState();
        }
        ImGui.SetWindowFontScale(1f);

        ImGui.SameLine();
        ImGui.TextColored(ImGuiColors.DalamudYellow, "尝试次数:");
        ImGui.SameLine();
        ImGui.Text(kickTimes.ToString());

        ImGui.Separator();

        ImGui.Checkbox("过程中禁用系统音", ref IsMuteSystemSound);
        ImGuiComponents.HelpMarker("成功挤入/停止后将会自动恢复原音量");

        ImGui.SetNextItemWidth(150f);
        ImGui.InputInt("尝试间隔 (单位: ms)", ref KickFrequency);
        ImGuiComponents.HelpMarker("过低的间隔不会让你的成功率变高\n" +
                                   "只会增加你的电脑负荷\n" +
                                   "推荐间隔: 500ms 至 1000ms");
    }

    private void HandleKickerState()
    {
        if (isOnKicking)
        {
            StartKickerHandler();
        }
        else
        {
            EndKickerHandler();
        }
    }

    private void StartKickerHandler()
    {
        kickTimes = 0;
        Plugin.GameConfig.TryGet(SystemConfigOption.SoundSystem, out originalVolume);
        if (IsMuteSystemSound) Plugin.GameConfig.System.Set("SoundSystem", 0);

        Plugin.AddonLifecycle.RegisterListener(AddonEvent.PostSetup, "SelectYesno", ClickYesButton);

        ClickNoviceNetworkButton();
    }

    private void EndKickerHandler()
    {
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

        Task.Delay(KickFrequency).ContinueWith(t => CheckJoinState());
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
        if (!isOnKicking)
        {
            EndKickerHandler();
            return;
        }

        var BGNL = Plugin.GameGui.GetAddonByName("BeginnerChatList");
        if (BGNL != nint.Zero)
        {
            EndKickerHandler();
        }
        else
        {
            ClickNoviceNetworkButton();
        }
    }

    public void Dispose()
    {
        if (isOnKicking)
        {
            EndKickerHandler();
        }
        Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PostSetup, "SelectYesno", ClickYesButton);
    }
}
