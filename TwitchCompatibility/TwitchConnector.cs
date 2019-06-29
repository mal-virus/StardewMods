using System;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using TwitchLib.Models.API.v5.Users;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


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

            Console.ReadLine();

            Disconnect();
        }

        internal void Connect()
        {
            Console.WriteLine("connecting...");

            client = new TwitchClient(credentials, TwitchInfo.ChannelName, logging: false);
            client.OnLog += Client_OnLog;
            client.OnConnectionError += Client_OnConnectionError;

            client.OnMessageReceived += Client_OnMessageReceived;

            client.Connect();
        }

        internal void Disconnect()
        {
            Console.WriteLine("disconnecting...");
        }

        public void SVEventCaller(String message)
        {
            modRef.Monitor.Log(message);
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            SVEventCaller(e.ChatMessage.Message.ToString());

            if (e.ChatMessage.Message.StartsWith("hi", StringComparison.InvariantCultureIgnoreCase))
            {
                client.SendMessage($"Hey there {e.ChatMessage.DisplayName}");
            }
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