namespace CommitAs
{
    using System;
    using Avalonia.Controls;
    using Avalonia.Controls.Templates;
    using CommitAs.ViewModels;

    /// <summary>
    /// Implements the <see cref="IDataTemplate"/> to find the correct view model for a view.
    /// </summary>
    public class ViewLocator : IDataTemplate
    {
        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly", Justification = "'!' Operator")]
        public Control? Build(object? data)
        {
            if (data is null)
            {
                return null;
            }

            var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
            var type = Type.GetType(name);

            if (type != null)
            {
                var control = (Control)Activator.CreateInstance(type)!;
                control.DataContext = data;
                return control;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        /// <inheritdoc/>
        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
