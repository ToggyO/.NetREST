using System;
using System.Collections.Generic;
using System.Linq;

namespace NetREST.Common.Models.Chat.ChatHubStorage
{
    public partial class ChatHubStorage<T>
    {
        private readonly Dictionary<string, ChatGroup> _groups = new Dictionary<string, ChatGroup>();

        public string AddGroup(string groupName)
        {
            if (String.IsNullOrEmpty(groupName))
                return null;
            
            lock (_groups)
            {
                string groupId = Guid.NewGuid().ToString();
                var group = new ChatGroup
                {
                    Id = groupId,
                    Name = groupName,
                    Users = new List<ChatUser>()
                };
                _groups.Add(groupId, group);
                return groupId;
            }
        }

        public void RemoveGroup(string groupId)
        {
            if (String.IsNullOrEmpty(groupId))
                return;

            lock (_groups)
            {
                _groups.Remove(groupId);
            }
        }
        
        public List<string> GetGroupIds()
        {
            lock (_groups)
            {
                return _groups.Keys.ToList();
            }
        }

        public List<ChatGroup> GetGroups()
        {
            lock (_groups)
            {
                return _groups.Values.ToList();
            }
        }

        public ChatGroup GetGroupById(string id)
        {
            lock (_groups)
            {
                ChatGroup group;
                _groups.TryGetValue(id, out group);
                return group;
            }
        }
    }
}