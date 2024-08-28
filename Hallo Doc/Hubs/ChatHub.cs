using Data_Access.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Business_Logic.Interface;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly ISessionUtils _sessionUtils;

        public ChatHub(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, ISessionUtils sessionUtils)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _sessionUtils = sessionUtils;
        }

        public async Task SendMessage(string requestId, string receiverId, string message)
        {
            var senderConnectionId = Context.ConnectionId;
            var senderId = _context.UserConnections.Where(x => x.ConnectionId == senderConnectionId).Select(x => x.UserId).FirstOrDefault();

            // Get the connection ID of the receiver
            var receiverConnectionId = _context.UserConnections
                .Where(x => x.UserId == receiverId && x.RequestId == requestId)
                .Select(x => x.ConnectionId)
                .FirstOrDefault();

            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, message);
            }
            else
            {
                await Clients.Caller.SendAsync("ReceiverNotAvailable", receiverId);
            }

            // Send the message to the sender as well
            await Clients.Client(senderConnectionId).SendAsync("ReceiveMessage", "You", message);

            // Send Notification
            var NotiConnectionid = _context.UserConnections.Where(x => x.UserId == receiverId && x.RequestId == "0").Select(x => x.ConnectionId).FirstOrDefault();

            await SendPushNotification(NotiConnectionid, message);
        }

        public async Task SendPushNotification(string receiverConnectionId, string message)
        {
            string? senderName = _sessionUtils.GetUser(_httpContextAccessor.HttpContext!.Session).UserName;

            await Clients.Client(receiverConnectionId).SendAsync("ReceivePushNotification", message, senderName);
        }

        public override Task OnConnectedAsync()
        {
            string? aspnetID = _sessionUtils.GetUser(_httpContextAccessor.HttpContext!.Session) == null ? null : _sessionUtils.GetUser(_httpContextAccessor.HttpContext!.Session).AspNetUserId.ToString();

            string Requestid = Context.GetHttpContext().Request.Query["Reqid"];

            if (!aspnetID.IsNullOrEmpty() && !Requestid.IsNullOrEmpty())
            {
                UserConnection connectedUSerId = _context.UserConnections.Where(x => x.UserId == aspnetID && x.RequestId == Requestid).FirstOrDefault();

                UserConnection connectedUSerId2 = _context.UserConnections.Where(x => x.UserId == aspnetID && x.RequestId == "0").FirstOrDefault();

                if (connectedUSerId == null)
                {
                    UserConnection userConnection = new UserConnection();
                    userConnection.ConnectionId = Context.ConnectionId;
                    userConnection.UserId = aspnetID;
                    userConnection.RequestId = Requestid;
                    _context.UserConnections.Add(userConnection);
                    _context.SaveChanges();
                }
                else
                {
                    connectedUSerId.ConnectionId = Context.ConnectionId;

                    if(connectedUSerId2 != null)
                    {
                        connectedUSerId2.ConnectionId = Context.ConnectionId;
                    }

                    _context.SaveChanges();
                }

            }
            else
            {
                Console.WriteLine("Warning: UserId is null on connection.");
            }
            return base.OnConnectedAsync();
        }



    }
}