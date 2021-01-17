using System;
using System.Collections.Generic;
using System.Linq;

namespace NetREST.Common.Models.Chat.ChatHubStorage
{
    public partial class ChatHubStorage<T>
    {
        private static readonly ChatHubStorage<T> Instance = new ChatHubStorage<T>();
        public static ChatHubStorage<T> GetInstance() => Instance;
        
        private ChatHubStorage() {}
    }
}