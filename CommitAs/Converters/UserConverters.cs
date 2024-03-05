namespace CommitAs.Converters
{
    using Avalonia.Data.Converters;
    using CommitAs.Core.Models;

    /// <summary>
    /// Converters for <see cref="User"/>s.
    /// </summary>
    public static class UserConverters
    {
        /// <summary>
        /// Gets a converter to get the <see cref="User.Name"/>.
        /// </summary>
        public static FuncValueConverter<User?, string> UserName { get; } =
            new FuncValueConverter<User?, string>(user => user == null ? string.Empty : user.Name);

        /// <summary>
        /// Gets a converter to get the <see cref="User.Email"/>.
        /// </summary>
        public static FuncValueConverter<User?, string> UserEmail { get; } =
            new FuncValueConverter<User?, string>(user => user == null ? string.Empty : user.Email);
    }
}
