using System;
using System.Collections.Generic;

namespace Tindro.Application.Chat
{
    public class ConversationDto
    {
        public Guid MatchId { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }

        // Frontend friendly
        public string? OtherUserId { get; set; }
        public string? OtherUserName { get; set; }
        public string? OtherUserPhoto { get; set; }
        public int UnreadCount { get; set; }
    }

}
