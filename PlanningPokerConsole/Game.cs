using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningPokerConsole
{
    public class Game
    {
        private readonly bool host;
        private readonly Id id;
        private readonly User user;

        private readonly JsonRequestHandler jsonReq;

        public static Game CreateGame(string domainURL, string username)
        {
            JsonRequestHandler handler = new JsonRequestHandler(domainURL);

            var json = handler.Request("/game/", RequestMethods.POST, "{ \"name\" : \"" + username + "\" }");

            Id gameid = new Id(json["gameid"].Value<string>());
            User user = new User(username, new Id(json["userid"].Value<string>()));

            return new Game(true, gameid, user, handler);
        }

        public static Game JoinGame(string domainURL, Id gameid, string username)
        {
            JsonRequestHandler handler = new JsonRequestHandler(domainURL);

            string request = string.Format("/game/{0}/user/", gameid.Hash);
            var json = handler.Request(request, RequestMethods.POST, "{ \"name\" : \"" + username + "\" }");
            User user = new User(username, new Id(json["userid"].Value<string>()));

            return new Game(false, gameid, user, handler);
        }

        private Game(bool host, Id id, User user, JsonRequestHandler jsonRequestHandler)
        {
            this.host = host;

            if (id == null)
                throw new ArgumentNullException("id");
            this.id = id;

            if (user == null)
                throw new ArgumentNullException("user");
            this.user = user;

            if (jsonRequestHandler == null)
                throw new ArgumentNullException("jsonRequestHandler");
            this.jsonReq = jsonRequestHandler;
        }

        public bool Host
        {
            get { return host; }
        }

        public Id Id
        {
            get { return id; }
        }

        public User User
        {
            get { return user; }
        }

        public string Description
        {
            get
            {
                string request = string.Format("/game/{0}/description/", id.Hash);
                var json = jsonReq.Request(request, RequestMethods.GET);
                return json["description"].Value<string>();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                string request = string.Format("/game/{0}/description/", id.Hash);
                if (value == string.Empty)
                    jsonReq.Request(request, RequestMethods.DELETE, "{ \"userid\" : \"" + user.Id.Hash + "\" }");
                else
                    jsonReq.Request(request, RequestMethods.POST, "{ \"description\" : \"" + value.Replace("\"", "\\\"") + "\", \"userid\" : \"" + user.Id.Hash + "\" }");
            }
        }

        public void Vote(VoteTypes voteType)
        {
            string request = string.Format("/game/{0}/vote/{1}/", id.Hash, user.Id.Hash);
            jsonReq.Request(request, RequestMethods.POST, "{ \"vote\" : \"" + voteType.ToAPIString() + "\" }");
        }

        public IEnumerable<KeyValuePair<User, VoteTypes?>> GetVotes()
        {
            string request = string.Format("/game/{0}/vote/", id.Hash);
            var json = jsonReq.Request(request, RequestMethods.GET);

            var votes = json["votes"] as JArray;
            foreach (var i in votes)
            {
                string username = i["name"].Value<string>();
                string voteStr = i["vote"].Value<string>();

                yield return new KeyValuePair<User, VoteTypes?>(new User(username), VoteTypesExtension.Parse(voteStr));
            }
        }

        public void ClearVotes()
        {
            string request = string.Format("/game/{0}/vote/", id.Hash);
            jsonReq.Request(request, RequestMethods.DELETE, "{ \"userid\" : \"" + user.Id.Hash + "\" }");
        }

        public void ResetGame()
        {
            Description = "";
            ClearVotes();
        }
    }
}
