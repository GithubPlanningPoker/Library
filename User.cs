using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class User
    {
        private readonly Id id;
        private string name;

        public User(string name, Id id)
            : this(name)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            this.id = id;
        }

        public User(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            this.name = name;
            this.id = null;
        }

        public Id Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
