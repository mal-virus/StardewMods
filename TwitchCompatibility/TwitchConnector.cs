using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using TwitchLib.Models.API.v5.Users;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;


namespace TwitchCompatibility
{
    internal class TwitchConnector
    {

        readonly ConnectionCredentials credentials = new ConnectionCredentials(TwitchInfo.BotUsername, TwitchInfo.BotToken);
        TwitchClient client;

        public ModEntry modRef;

        public void TwitchWrapper()
        {

            Connect();

            //Console.ReadLine();

            if (modRef.Monitor.IsExiting == true)
            {
                Disconnect();
            }
        }

        internal void Connect()
        {
            modRef.Monitor.Log("connecting...");

            client = new TwitchClient(credentials, TwitchInfo.ChannelName, logging: false);
            client.OnLog += Client_OnLog;
            client.OnConnectionError += Client_OnConnectionError;

            client.OnMessageReceived += Client_OnMessageReceived;

            client.Connect();
        }

        internal void Disconnect()
        {
            modRef.Monitor.Log("disconnecting...");
        }

        /// <summary>Method controlling interaction with Twitch Chat Messages for Stardew Mod</summary>
        /// <param name="message">The message passed from the Client_OnMessageRecieved Method.</param>
        public void OnChatMessage(TwitchLib.Models.Client.ChatMessage message)
        {
            // modRef.Monitor.Log($"[{message.DisplayName.ToString()}] {message.Message.ToString()}");
            
            if (Context.IsWorldReady)
            {
                Game1.chatBox.addMessage($"[{message.DisplayName.ToString()}] {message.Message.ToString()}", ColorConverter(message.Color));
            }
        }

        // Helper function to convert System.Drawing.Color object to Xna.Framework.color object
        // !TODO: Proper documentation for this method
        private Microsoft.Xna.Framework.Color ColorConverter(System.Drawing.Color color)
        {
            return new Microsoft.Xna.Framework.Color(color.R, color.G, color.B);
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            OnChatMessage(e.ChatMessage);

            //if (e.ChatMessage.Message.StartsWith("hi", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    client.SendMessage($"Hey there {e.ChatMessage.DisplayName}");
            //}
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //Console.WriteLine(e.Data);
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            Console.WriteLine($"Error!! {e.Error}");
        }
    }
}