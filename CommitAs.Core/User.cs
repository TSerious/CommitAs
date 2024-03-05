namespace CommitAs.Core.Models
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Holds all relevant information of a user.
    /// </summary>
    public class User : IComparable<User>, IEquatable<User>, IEqualityComparer<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">Sets <see cref="Name"/>.</param>
        public User(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets the index at which <paramref name="newUser"/>
        /// should be inserted into <paramref name="users"/>.
        /// </summary>
        /// <param name="users">The current users.</param>
        /// <param name="newUser">A new user.</param>
        /// <returns>The index.</returns>
        public static int GetInsertIndex(IList<User> users, User newUser)
        {
            int insertIndex = 0;
            while (newUser.CompareTo(users[insertIndex]) > 0)
            {
                insertIndex++;
            }

            return insertIndex;
        }

        /// <summary>
        /// Gets the index at which <paramref name="newUser"/>
        /// should be inserted into <paramref name="users"/>.
        /// </summary>
        /// <param name="users">The current users.</param>
        /// <param name="newUser">A new user.</param>
        /// <returns>The index.</returns>
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

        /// <inheritdoc/>
        public int CompareTo(User? other)
        {
            if (other == null)
            {
                return -1;
            }

            return this.Name.CompareTo(other.Name);
        }

        /// <inheritdoc/>
        public bool Equals(User? other)
        {
            if (other == null)
            {
                return false;
            }

            return string.Equals(this.Name, other.Name);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] User obj)
        {
            return obj.Name.GetHashCode();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.GetHashCode(this);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return this.Equals(obj as User);
        }
    }
}
