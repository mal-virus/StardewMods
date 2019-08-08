using System;
using TwitchLib;
using TwitchLib.Models.Client;
using TwitchLib.Events.Client;
using StardewModdingAPI;
using StardewValley;

namespace TwitchCompatibility {
    internal class TwitchConnector {

        static string BotUsername;
        static string AuthToken;
        static string Channel;
        TwitchClient client;

        public TwitchConnector(string botUsername,string authToken,string channel) {
            BotUsername=botUsername;
            AuthToken=authToken;
            Channel=channel;
        }

        internal void Connect() {
            Console.WriteLine("connecting...");

            client=new TwitchClient(new ConnectionCredentials(BotUsername,AuthToken),Channel,logging: false);
            client.OnLog+=Client_OnLog;
            client.OnConnectionError+=Client_OnConnectionError;

            client.OnMessageReceived+=Client_OnMessageReceived;

            client.Connect();
        }

        private void Client_OnMessageReceived(object sender,OnMessageReceivedArgs e) {
            ChatMessage message = e.ChatMessage;

            if(Context.IsWorldReady) {
                Game1.chatBox.addInfoMessage($"[{message.DisplayName.ToString()}] {message.Message.ToString()}");
                if(message.Message.StartsWith("hi",StringComparison.InvariantCultureIgnoreCase)) {
                    client.SendMessage($"Hey there {e.ChatMessage.DisplayName}");
                }
            }

        }

        private void Client_OnLog(object sender,OnLogArgs e) {
            //Console.WriteLine(e.Data);
        }

        private void Client_OnConnectionError(object sender,OnConnectionErrorArgs e) {
            Console.WriteLine($"Error!! {e.Error}");
        }


        internal void Disconnect() {
            Console.WriteLine("disconnecting...");
        }
    }
}