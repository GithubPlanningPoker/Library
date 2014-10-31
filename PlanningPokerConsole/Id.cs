using System;

namespace PlanningPokerConsole
{
    public class Id
    {
        private readonly string hash;

        public Id(string hash)
        {
            if (hash == null)
                throw new ArgumentNullException("hash");

            hash = hash.Trim();
            if (hash.Length == 0)
                throw new ArgumentException("Empty hash string.");

            this.hash = hash;
        }

        public string Hash
        {
            get { return hash; }
        }
    }
}
