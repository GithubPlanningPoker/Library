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
        private readonly Id id;

        public static Game CreateGame(string username, out Id userID)
        {
            var json = JsonRequestHandler.Request("/game/", RequestMethods.POST, "{ \"name\" : \"" + username + "\" }");

            Id gameid = new Id((json["gameid"] as JValue).Value<string>());
            userID = new Id((json["userid"] as JValue).Value<string>());

            return new Game(gameid);
        }

        public Game(Id id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            this.id = id;
        }
    }
}
