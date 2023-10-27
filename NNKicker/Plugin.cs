using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using NNKicker.Windows;
using ClickLib.Bases;
using ClickLib.Clicks;

namespace NNKicker
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "NNKicker";
        private const string CommandName = "/NNK";

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        public static Plugin Instance = null!;
        [PluginService] public static Framework Framework { get; set; } = null!;
        [PluginService] public static GameGui GameGui { get; set; } = null!;
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("NNKicker");

        private ConfigWindow ConfigWindow { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            Instance = this;
            PluginInterface = pluginInterface;
            CommandManager = commandManager;

            Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            Configuration.Initialize(PluginInterface);

            ConfigWindow = new ConfigWindow(this);

            WindowSystem.AddWindow(ConfigWindow);

            CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "主界面"
            });

            PluginInterface.UiBuilder.Draw += DrawUI;
            PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
        }

        internal void OnFrameworkUpdate(Framework framework)
        {
            var SYN = Plugin.GameGui.GetAddonByName("SelectYesno");
            if (SYN != nint.Zero)
            {
                ClickSelectOk clickSelectOk = new(SYN);
                clickSelectOk.Ok();
            }
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();

            ConfigWindow.Dispose();

            CommandManager.RemoveHandler(CommandName);

            Framework.Update -= OnFrameworkUpdate;
        }

        private void OnCommand(string command, string args)
        {
            ConfigWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
    }
}
