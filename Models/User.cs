using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CommitAs.Models
{
    public class User : IComparable<User>, IEquatable<User>, IEqualityComparer<User>
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

        public int CompareTo(User? other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Name.CompareTo(other.Name);
        }

        public bool Equals(User? other)
        {
            if (other == null)
            {
                return false;
            }

            return string.Equals(this.Name, other.Name);
        }

        public bool Equals(User? x, User? y)
        {
            if (x == null && y == null)
            {
                return true;
            }

            if (x != null)
            {
                return x.Equals(y);
            }

            return y!.Equals(x);
        }

        public int GetHashCode([DisallowNull] User obj)
        {
            return  obj.Name.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.GetHashCode(this);
        }

        public override bool Equals(object? obj)
        {
            return this.Equals(obj as User);
        }

        public static int GetInsertIndex(IList<User> users, User newUser)
        {
            int insertIndex = 0;
            while (newUser.CompareTo(users[insertIndex]) > 0)
            {
                insertIndex++;
            }
            
            return insertIndex;
        }

        public static int GetInsertIndex(IList<string> users, string newUser)
        {
            if (string.IsNullOrEmpty(newUser))
            {
                return 0;
            }

            int insertIndex = 0;
            while (newUser.CompareTo(users[insertIndex]) > 0)
            {
                insertIndex++;
            }

            return insertIndex;
        }
    }
}
