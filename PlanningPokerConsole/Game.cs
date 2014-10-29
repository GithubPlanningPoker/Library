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
        private bool host;
        private readonly Id id;
        private readonly User user;

        public static Game CreateGame(string username)
        {
            var json = JsonRequestHandler.Request("/game/", RequestMethods.POST, "{ \"name\" : \"" + username + "\" }");

            Id gameid = new Id(json["gameid"].Value<string>());
            User user = new User(username, new Id(json["userid"].Value<string>()));

            return new Game(true, gameid, user);
        }

        public static Game JoinGame(Id gameid, string username)
        {
            var json = JsonRequestHandler.Request("/game/" + gameid.Hash + "/user/", RequestMethods.POST, "{ \"name\" : \"" + username + "\" }");
            User user = new User(username, new Id(json["userid"].Value<string>()));

            return new Game(false, gameid, user);
        }

        private Game(bool host, Id id, User user)
        {
            this.host = host;

            if (id == null)
                throw new ArgumentNullException("id");
            this.id = id;

            if (user == null)
                throw new ArgumentNullException("user");
            this.user = user;
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
    }
}
