using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitAs.Models
{
    public class User
    {
        public User(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            this.Name = name;
        }

        public string Name { get; set; }

        public string Email { get; set; } = string.Empty;
    }
}
