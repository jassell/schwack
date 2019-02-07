using System;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace Service
{
    public class SchwackService
    {
        private static IList<Entities.User> Users = new List<Entities.User>();
        private static IList<Entities.Message> Messages = new List<Entities.Message>();
        public IList<Entities.User> SignIn(string who)
        {
            if (Users.Any() && Users.Where(w => w.Name.ToLower() == who.ToLower()).ToList().Count > 0)
            {
                throw new System.ApplicationException("100");
            }

            Users.Add(new Entities.User(who));
            return Users;
        }

        public Message PostMessage(System.Guid from, System.Guid to, string message)
        {
            var postMessage = new Message();
            postMessage.From = Users.SingleOrDefault(w => w.Id == from);
            postMessage.To = Users.SingleOrDefault(w => w.Id == to);
            postMessage.MessageText = message;
            postMessage.PostTime = DateTime.Now;

            Messages.Add(postMessage);

            return postMessage;
        }

        public void SignOut(System.Guid id)
        {
            var user = Users.SingleOrDefault(w => w.Id.ToString().ToLower() == id.ToString().ToLower());
            if (user != null)
                Users.Remove(user);
        }

        public IList<Entities.User> GetUsers()
        {
            return Users;
        }

        public IList<Message> GetMessages(Guid forId)
        {
            var messages = Messages.Where(w => w.To.Id == forId).OrderByDescending(o => o.PostTime).ToList();

            foreach (var m in messages)
                Messages.Remove(m);

            return messages;
        }
    }
}
