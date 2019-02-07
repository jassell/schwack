namespace Entities
{
    public class User
    {
        public User() { }
        public User(string who)
        {
            Id = System.Guid.NewGuid();
            Name = who;
        }

        public System.Guid Id { get; set; }
        public string Name { get; set; }

    }
}
