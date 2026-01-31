using MediatR;

namespace Tindro.Application.Match.Events
{
    public class MatchCreatedEvent : INotification
    {
        public string MatchId { get; }
        public string User1Id { get; }
        public string User2Id { get; }

        public MatchCreatedEvent(string matchId, string user1Id, string user2Id)
        {
            MatchId = matchId;
            User1Id = user1Id;
            User2Id = user2Id;
        }
    }
}
