using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace NNKicker
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "NNKicker";
        private const string CommandName = "/NNK";
        private const string CommandName2 = "/nnk";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private ConfigWindow ConfigWindow { get; init; }

        public static Plugin Instance = null!;

        [PluginService] public static GameGui GameGui { get; set; } = null!;
        [PluginService] public static IAddonLifecycle AddonLifecycle { get; set; } = null!;
        [PluginService] public static IGameConfig GameConfig { get; set; } = null!;

        public WindowSystem WindowSystem = new("NNKicker");


        public Plugin(DalamudPluginInterface pluginInterface, CommandManager commandManager)
        {
            Instance = this;
            PluginInterface = pluginInterface;
            CommandManager = commandManager;

            ConfigWindow = new ConfigWindow(this);
            WindowSystem.AddWindow(ConfigWindow);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "主界面"
            });

            CommandManager.AddHandler(CommandName2, new CommandInfo(OnCommand)
            {
                HelpMessage = "主界面"
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        private void DrawUI()
        {
            WindowSystem.Draw();
        }

        private void OnCommand(string command, string args)
        {
            ConfigWindow.IsOpen = true;
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            ConfigWindow.Dispose();
            CommandManager.RemoveHandler(CommandName);
            CommandManager.RemoveHandler(CommandName2);
        }
    }
}
