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

            var json = handler.Request("/game/" + gameid.Hash + "/user/", RequestMethods.POST, "{ \"name\" : \"" + username + "\" }");
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
                var json = jsonReq.Request("/game/" + id.Hash + "/description/", RequestMethods.GET);
                return json["description"].Value<string>();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value == string.Empty)
                    jsonReq.Request("/game/" + id.Hash + "/description/", RequestMethods.DELETE,
                        "{ \"userid\" : \"" + user.Id.Hash + "\" }");
                else
                    jsonReq.Request("/game/" + id.Hash + "/description/", RequestMethods.PUT,
                        "{ \"description\" : \"" + value.Replace("\"", "\\\"") + "\", \"userid\" : \"" + user.Id.Hash + "\" }");
            }
        }

        public void Vote(VoteTypes voteType)
        {
            jsonReq.Request("/game/" + id.Hash + "/vote/" + user.Id.Hash + "/", RequestMethods.POST, "{ \"vote\" : \"" + voteType.ToAPIString() + "\" }");
        }
    }
}
