using System;
using System.Numerics;
using ClickLib.Enums;
using ClickLib.Bases;
using Dalamud.Interface.Windowing;
using Dalamud.Interface.Colors;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using System.Timers;
using ClickLib;
using ClickLib.Clicks;

namespace NNKicker.Windows;

public class ConfigWindow : Window, IDisposable
{
    private System.Timers.Timer KickTimer = new System.Timers.Timer(500);
    private int KickTimes = 0;
    private bool isOnKicking = false;

    public ConfigWindow(Plugin plugin) : base(
        "Novice Network Kicker",
        ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize)
    {
        this.Size = new Vector2(200, 50);
        this.SizeCondition = ImGuiCond.Once;

        KickTimer.Elapsed += KickTimerElapsed;

        KickTimer.AutoReset = false;
    }

    private void KickTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var BGNL = Plugin.GameGui.GetAddonByName("BeginnerChatList");
        if (BGNL != nint.Zero)
        {
            KickTimer.Stop();
            KickTimer.Dispose();
            Plugin.Framework.Update -= Plugin.Instance.OnFrameworkUpdate;
            isOnKicking = false;
        }
        else
        {
            PerformButtonClick();
        }
    }

    public void Dispose() 
    {
        KickTimer.Stop();
        KickTimer.Dispose();
    }

    public override void Draw()
    {
        if (!isOnKicking)
        {
            if (ImGui.Button("一键开挤"))
            {
                isOnKicking = true;
                KickTimes = 0;
                PerformButtonClick();
                Plugin.Framework.Update += Plugin.Instance.OnFrameworkUpdate;
            }
        }
        else
        {
            ImGui.Button("一键开挤");
        }

        ImGui.SameLine();

        ImGui.TextColored(ImGuiColors.DalamudYellow, "尝试次数:");
        ImGui.SameLine();
        ImGui.Text(KickTimes.ToString());
    }

    public void PerformButtonClick()
    {
        unsafe
        {
            var CL = (AtkUnitBase*)Plugin.GameGui.GetAddonByName("ChatLog");
            var CLN = Plugin.GameGui.GetAddonByName("ChatLog");
            ClickNNButton clickNNButton = new(CLN);

            if (CL != null && CL->RootNode != null && CL->RootNode->ChildNode != null && CL->UldManager.NodeList != null)
            {
                var buttonNode = CL->GetComponentNodeById(12);
                if (buttonNode != null)
                {
                    clickNNButton.PerformButtonClick(CLN);
                    KickTimes++;
                }
            }


            KickTimer.Stop();
            KickTimer.Start();
        }
    }
}
