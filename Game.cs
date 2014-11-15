using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Game
    {
        private readonly bool host;
        private readonly Id id;
        private readonly User user;

        private const string userIdString = "userId";
        private const string usernameString = "username";
        private const string descriptionString = "description";
        private const string titleString = "title";
        private const string voteString = "vote";
        private const string usersString = "users";
        private const string votedString = "voted";
        private const string gameIdString = "gameId";

        private readonly JsonRequestHandler jsonReq;

        public static Game CreateGame(string domainURL, string username)
        {
            JsonRequestHandler handler = new JsonRequestHandler(domainURL);

            var json = handler.Request("/game/", RequestMethods.POST, new JObject(new JProperty(usernameString, username)));

            Id gameid = new Id(json[gameIdString].Value<string>());
            User user = new User(username, new Id(json[userIdString].Value<string>()));

            return new Game(true, gameid, user, handler);
        }

        public static Game JoinGame(string domainURL, Id gameid, string username)
        {
            JsonRequestHandler handler = new JsonRequestHandler(domainURL);

            string request = string.Format("/game/{0}/user/", gameid.Hash);
            var json = handler.Request(request, RequestMethods.POST, new JObject(new JProperty(usernameString, username)));
            User user = new User(username, new Id(json[userIdString].Value<string>()));

            return new Game(false, gameid, user, handler);
        }

        public static bool Exists(string domainURL, Id gameid)
        {
            JsonRequestHandler handler = new JsonRequestHandler(domainURL);

            string request = string.Format("/game/{0}/", gameid.Hash);
            var json = handler.Request(request, RequestMethods.GET, true);

            return json["success"].Value<bool>();
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
                return json[descriptionString].Value<string>();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                string request = string.Format("/game/{0}/description/", id.Hash);
                jsonReq.Request(request, RequestMethods.PUT, new JObject(new JProperty(descriptionString, value), new JProperty(userIdString, user.Id.Hash)));
            }
        }
        public string Title
        {
            get
            {
                string request = string.Format("/game/{0}/title/", id.Hash);
                var json = jsonReq.Request(request, RequestMethods.GET);
                return json[titleString].Value<string>();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                string request = string.Format("/game/{0}/title/", id.Hash);
                jsonReq.Request(request, RequestMethods.PUT, new JObject(new JProperty(titleString, value), new JProperty(userIdString, user.Id.Hash)));
            }
        }

        public void Kick(string username)
        {
            string request = string.Format("/game/[gameid]/user/[username]/", id.Hash, username);
            jsonReq.Request(request, RequestMethods.DELETE, new JObject(new JProperty(userIdString, user.Id.Hash)));
        }
        public Vote[] Votes
        {
            get
            {
                string request = string.Format("/game/{0}/user/", id.Hash);
                var json = jsonReq.Request(request, RequestMethods.GET);

                var jvotes = json[usersString] as JArray;
                Vote[] votes = new Vote[jvotes.Count];

                bool allVoted = jvotes.All(node => node[votedString].Value<bool>());

                for (int i = 0; i < jvotes.Count; i++)
                {
                    string user = jvotes[i][usernameString].Value<string>();
                    var v = jvotes[i][voteString];

                    if (allVoted)
                        votes[i] = new Library.Vote(user, VoteTypesExtension.Parse(v.Value<string>()));
                    else
                        votes[i] = new Library.Vote(user, jvotes[i][votedString].Value<bool>());
                }

                return votes;
            }
        }

        /// <summary>
        /// Estimates a result for the vote
        /// </summary>
        /// <returns></returns>
        public int VoteResultEstimate()
        {
            double average = Votes.Average(x => x.VoteType.ToDouble());

            return (int)Math.Round(average, 0, MidpointRounding.ToEven);
        }
        public void Vote(VoteTypes voteType)
        {
            string request = string.Format("/game/{0}/user/{1}/", id.Hash, user.Name);
            jsonReq.Request(request, RequestMethods.PUT, new JObject(new JProperty(voteString, voteType.ToAPIString()), new JProperty(userIdString, user.Id.Hash)));
        }

        public void ClearVotes()
        {
            if (!Host)
                throw new InvalidOperationException("Only the host can clear votes.");

            string request = string.Format("/game/{0}/user/", id.Hash);
            jsonReq.Request(request, RequestMethods.PUT, new JObject(new JProperty(userIdString, user.Id.Hash)));
        }

        public void ResetGame()
        {
            Description = "";
            Title = "";
            ClearVotes();
        }
    }
}
