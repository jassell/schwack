using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Configuration;
using Entities;
using Newtonsoft.Json;

namespace schwackd.Helpers
{
    public class SchwackHelper
    {
        public List<User> GetUsers()
        {
            var server = ConfigurationManager.AppSettings["SchwackServer"];
            var client = new RestClient(server);
            var request = new RestRequest("schwack/schwack/GetUsers", Method.GET);

            var queryResults = client.Execute<System.Collections.Generic.Dictionary<string, object>>(request).Data;

            string json = null;
            if (queryResults.Count == 2)
            {
                if (Convert.ToBoolean(queryResults["success"]))
                {
                    json = queryResults["users"].ToString();
                }
            }

            return ConvertGetUsers(json);

        }
        public List<User> ConvertGetUsers(string json)
        {
            var users = JsonConvert.DeserializeObject<List<User>>(json);

            return users;
        }

        public User SignIn(string who)
        {
            var server = ConfigurationManager.AppSettings["SchwackServer"];
            var client = new RestClient(server);
            var request = new RestRequest("schwack/schwack/SignIn", Method.GET, DataFormat.Json);
            request.AddQueryParameter("who", who);

            var queryResults = client.Execute<System.Collections.Generic.Dictionary<string, object>>(request).Data;

            if (queryResults != null)
            {
                if (Convert.ToBoolean(queryResults["success"]))
                {
                    return ConvertSignInUser(JsonConvert.SerializeObject(queryResults["user"]));
                }
                else
                {
                    throw new ApplicationException(queryResults["message"].ToString());
                }
            }

            throw new ApplicationException("Something went really wrong");
        }

        private User ConvertSignInUser(string json)
        {
            return JsonConvert.DeserializeObject<User>(json);
        }

        private string ConvertSignInMessage(string json)
        {
            return JsonConvert.DeserializeObject<string>(json);
        }


    }
}
