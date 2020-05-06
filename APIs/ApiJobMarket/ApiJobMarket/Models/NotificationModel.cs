namespace ApiJobMarket.Models
{
    public class NotificationModel
    {
        public string ActionType { get; set; }
        public int OwnerId { get; set; }
        public int ActorId { get; set; }
        public int ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string Description { get; set; }
    }
}