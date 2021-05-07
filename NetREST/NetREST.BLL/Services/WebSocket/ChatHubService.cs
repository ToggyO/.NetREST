using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NetREST.Common.Chat;
using NetREST.Common.Extensions;
using NetREST.Common.Models;
using NetREST.Common.Models.Chat;
using NetREST.Common.Models.Chat.ChatHubStorage;
using NetREST.DAL.Repository.Users;
using NetREST.Domain.User;

namespace NetREST.BLL.Services.WebSocket
{
    [Authorize]
    public class ChatHubService : Hub
    {
        private readonly ILogger<ChatHubService> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ChatHubStorage<string> _storage;

        public ChatHubService(ILogger<ChatHubService> logger,
            IMapper mapper, IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
            _storage = ChatHubStorage<string>.GetInstance();
            _logger.LogInformation("ChatHubService constructor work done");
        }

        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var userEntity = await _userRepository.GetById((int)user.GetUserId());
            var chatUser = _mapper.Map<UserModel, ChatUser>(userEntity);
            _storage.AddConnection(Context.ConnectionId, chatUser);
            _logger.LogInformation($"Welcome, {chatUser.FirstName}! Join to group if you want to chat with someone");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var meta = GetCleanClientMeta();
            if (!String.IsNullOrEmpty(meta.GroupId))
            {
                NofifyAboutLeaving(meta.GroupId, meta.UserName);
            }
            await base.OnDisconnectedAsync(ex);
        }

        public async Task EnterGroup(string groupId)
        {
            ChatClientMeta meta = _storage.GetUserByConnectionId(Context.ConnectionId);
            var group = _storage.GetGroupById(groupId);
            if (group == null)
                throw new Exception("Group with the same id doesnt exists");

            _storage.UpdateConnection(Context.ConnectionId, meta.User, group.Id);
            await Groups.AddToGroupAsync(Context.ConnectionId, group.Id);
            await Clients.Caller.SendAsync(ChatEvents.GetGroupById, group);
            await Clients.Caller.SendAsync(ChatEvents.Notify, $"You joined to '{group.Name}' group!");
            await Clients.OthersInGroup(group.Id)
                .SendAsync(ChatEvents.Notify, $"{meta.User.FirstName} joined to group!");
        }

        public async Task LeaveGroup(string groupId)
        {
            ChatClientMeta meta = _storage.GetUserByConnectionId(Context.ConnectionId);
            var group = _storage.GetGroupById(groupId);
            if (group == null)
                throw new Exception("Group with the same id doesnt exists");
            
            _storage.UpdateConnection(Context.ConnectionId, meta.User, null);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.Id);
            await Clients.Caller.SendAsync(ChatEvents.GroupLeft, group);
            NofifyAboutLeaving(group.Id, meta.User.FirstName);
        }

        public async Task SendMessage(string message)
        {
            ChatClientMeta meta = _storage.GetUserByConnectionId(Context.ConnectionId);
            if (meta.GroupId == null)
                throw new HubException("Please, join group if you want to send message");

            await Clients.Group(meta.GroupId).SendAsync(ChatEvents.SendMessage,
                new SimpleMessage { User = meta.User.FirstName, Message = message });
        }

        public async Task GetGroupsList() =>
            await Clients.Caller.SendAsync(ChatEvents.GetGroupsList, _storage.GetGroups());

        public async Task CreateGroup(string groupName)
        {
            var meta = GetCleanClientMeta();
            var groupId = _storage.AddGroup(groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            await Clients.Caller.SendAsync($"Welcome, {meta.UserName}!");
        }

        private CleanClientMeta GetCleanClientMeta(string groupId = "")
        {
            ChatClientMeta meta = _storage.GetUserByConnectionId(Context.ConnectionId);
            return new CleanClientMeta
            {
                UserName = meta.User.FirstName,
                GroupId = !String.IsNullOrEmpty(groupId) ? groupId : meta.GroupId
            };
        }

        private async void NofifyAboutLeaving(string groupId, string userName)
        {
            await Clients.Group(groupId)
                .SendAsync(ChatEvents.Notify, $"{userName} left the group!");
        }
    }
}