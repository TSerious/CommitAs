using Avalonia.Data.Converters;
using CommitAs.Models;

namespace CommitAs.Converters
{
    public static class UserConverters
    {
        public static FuncValueConverter<User?, string> UserName { get; } =
            new FuncValueConverter<User?, string>(user => user == null ? string.Empty : user.Name);

        public static FuncValueConverter<User?, string> UserEmail { get; } =
            new FuncValueConverter<User?, string>(user => user == null ? string.Empty : user.Email);
    }
}
