using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Vote
    {
        private string name;
        private VoteTypes votetype;
        private bool hasvoted;

        public Vote(string name, VoteTypes votetype)
        {
            this.name = name;
            this.votetype = votetype;
            this.hasvoted = true;
        }

        public Vote(string name, bool hasvoted)
        {
            this.name = name;
            this.votetype = default(VoteTypes);
            this.hasvoted = hasvoted;
        }

        public string Name
        {
            get { return name; }
        }
        public VoteTypes VoteType
        {
            get { return votetype; }
        }
        public bool HasVoted
        {
            get { return hasvoted; }
        }

        public override string ToString()
        {
            if (!hasvoted)
                return string.Format("{0}: No vote", name);
            else
                return string.Format("{0}: {1}", name, votetype.ToAPIString());
        }
    }
}
