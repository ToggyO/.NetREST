using System;
using System.Collections.Generic;

namespace NetREST.Common.Models.Chat
{
    public class ChatGroup
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ChatUser> Users { get; set; }
    }
}