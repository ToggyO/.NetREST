using System.Collections.Generic;
using System.Linq;

namespace NetREST.Common.Models.Chat.ChatHubStorage
{
    public partial class ChatHubStorage<T>
    {
        private readonly Dictionary<T, ChatClientMeta> _connections = new Dictionary<T, ChatClientMeta>();

        public void AddConnection(T key, ChatUser user)
        {
            if (user == null)
                return;

            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out ChatClientMeta meta))
                {
                    meta = new ChatClientMeta { User = user };
                    _connections.Add(key, meta);
                }   
            }
        }

        public void AddConnection(T key, ChatUser user, string groupId)
        {
            if (user == null)
                return;

            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out ChatClientMeta meta))
                {
                    meta = new ChatClientMeta
                    {
                        User = user,
                        GroupId = groupId
                    };
                    _connections.Add(key, meta);
                }  
            }
        }

        public void UpdateConnection(T key, ChatUser user, string groupId)
        {
            if (user == null)
                return;

            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out ChatClientMeta meta))
                    return;
            
                _connections[key] = new ChatClientMeta
                {
                    User = user,
                    GroupId = groupId
                };
            }
        }

        public void RemoveConnection(T key)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out ChatClientMeta meta))
                    return;
                _connections.Remove(key);
            }
        }
                
        public List<T> GetConnectionIds()
        {
            lock (_connections)
                return _connections.Keys.ToList();
        }
        
        public List<ChatUser> GetUsers()
        {
            lock (_connections)
                return _connections.Values.Select(u => u.User).ToList();
        }

        public ChatClientMeta GetUserByConnectionId(T connectionId)
        {
            lock (_connections)
            {
                if (_connections.TryGetValue(connectionId, out ChatClientMeta meta))
                    return meta;
            }
            return null;
        }
    }
}