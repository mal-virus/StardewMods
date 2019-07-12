using System;
using System.Drawing;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TwitchLib;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.Communication.Interfaces;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Extensions;
using TwitchLib.Api;
using System.Threading.Tasks;
using TwitchLib.PubSub;

namespace TwitchCompatibility
{
    internal class TwitchConnector
    {
        TwitchClient client;
        private TwitchPubSub pubsub;
        public ModEntry modRef;

        public void TwitchWrapper()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(TwitchInfo.BotUsername, TwitchInfo.BotToken);

            // Chatbot Creation
            client = new TwitchClient();
            client.Initialize(credentials, TwitchInfo.ChannelName);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.Connect();

            // PubSub creation
            pubsub = new TwitchPubSub();
            pubsub.OnPubSubServiceConnected += Pubsub_OnPubSubServiceConnected;
            pubsub.OnListenResponse += Pubsub_OnListenResponse;
            pubsub.OnBitsReceived += Pubsub_OnBitsReceived;

            pubsub.Connect();



            if (modRef.Monitor.IsExiting == true)
            {
                client.Disconnect();
            }
        }  

        internal void Connect()
        {
            modRef.Monitor.Log("connecting...");
        }
        internal void Disconnect()
        {
            modRef.Monitor.Log("disconnecting...");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            modRef.Monitor.Log("Hey guys! I am a bot connected via TwitchLib!");
            client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            OnChatMessage(e.ChatMessage);

            if (e.ChatMessage.Message.Contains("badword"))
                client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }

        /// <summary>Method controlling interaction with Twitch Chat Messages for Stardew Mod</summary>
        /// <param name="message">The message passed from the Client_OnMessageRecieved Method.</param>
        public void OnChatMessage(TwitchLib.Client.Models.ChatMessage message)
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

        
        /******************************************************************************************************************/


        private void Pubsub_OnPubSubServiceConnected(object sender, System.EventArgs e)
        {
            pubsub.ListenToWhispers(TwitchInfo.ChannelName);
        }

        private void Pubsub_OnBitsReceived(object sender, TwitchLib.PubSub.Events.OnBitsReceivedArgs e)
        {
            modRef.Monitor.Log($"Just received {e.BitsUsed} bits from {e.Username}. That brings their total to {e.TotalBitsUsed} bits!");
        }

        private void Pubsub_OnListenResponse(object sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
        {
            if (e.Successful)
                modRef.Monitor.Log($"Successfully verified listening to topic: {e.Topic}");
            else
                modRef.Monitor.Log($"Failed to listen! Error: {e.Response.Error}");
        }

        /******************************************************************************************************************/

    }
}