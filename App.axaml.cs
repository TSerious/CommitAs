using Avalonia;
using Avalonia.Markup.Xaml;

namespace CommitAs
{
    /// <summary>
    /// The app allows to quickly change the user of a git repository.
    /// </summary>
    public partial class App : Application
    {
        /// <inheritdoc/>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}