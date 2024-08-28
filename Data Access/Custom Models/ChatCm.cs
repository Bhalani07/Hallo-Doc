using Data_Access.Models;

namespace Data_Access.Custom_Models
{
    public class ChatCm
    {
        public int? AspId { get; set; }

        public string? SenderName { get; set; }

        public string? ReceiverName { get; set; }

        public List<ChatHistory> chatHistories { get; set;}

    }
}