namespace Entities
{
    public class Message
    {
        public User From { get; set; }
        public User To { get; set; }
        public string MessageText { get; set; }
        public System.DateTime PostTime { get; set; }
    }
}
