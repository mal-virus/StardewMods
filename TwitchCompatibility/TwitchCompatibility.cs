using StardewModdingAPI;

namespace TwitchCompatibility {
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod {
        /// <summary>The mod configuration from the player.</summary>
        ModConfig Config;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper) {
            Config=this.Helper.ReadConfig<ModConfig>();
            TwitchConnector bot = new TwitchConnector(Config.BotUsername, Config.AuthToken, Config.Channel);

            bot.Connect();
        }
    }
}