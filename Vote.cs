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
        private VoteTypes? votetype;

        public Vote(string name, VoteTypes? votetype)
        {
            this.name = name;
            this.votetype = votetype;
        }

        public string Name
        {
            get { return name; }
        }
        public VoteTypes? VoteType
        {
            get { return votetype; }
        }
    }
}
