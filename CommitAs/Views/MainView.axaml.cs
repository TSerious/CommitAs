namespace CommitAs.Views;

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using CommitAs.ViewModels;
using ReactiveUI;

/// <summary>
/// This is the main view for the user interaction.
/// </summary>
public partial class MainView : ReactiveUserControl<MainViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    public MainView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.UserName.WhenAnyValue(x => x.SelectedIndex, x => x.SelectedItem)
                .Skip(3)
                .Subscribe(_ => this.SetCurrentUser())
                .DisposeWith(disposables);

            Observable.FromEventPattern<KeyEventArgs>(this.UserName, nameof(this.UserName.KeyDown))
            .Subscribe(k => System.Diagnostics.Debug.WriteLine($"pressed: {k}"))
            .DisposeWith(disposables);

            var k = new KeyBinding
            {
                Gesture = new KeyGesture(Key.Enter),
                Command = ReactiveCommand.Create(new Action(() =>
                {
                    this.SetCurrentUser();
                })),
            };
            this.UserName.KeyBindings.Add(k);

            this.SelectMostSimularUserAsCurrent();

            this.UserName.Focus();
        });
    }

    private void SelectMostSimularUserAsCurrent()
    {
        if (this.ViewModel == null ||
            this.ViewModel.CurrentUser == null)
        {
            return;
        }

        this.UserName.SelectedIndex = this.ViewModel.UserNames.IndexOf(this.ViewModel.CurrentUser.Name);
    }

    private void SetCurrentUser()
    {
        if (this.ViewModel == null ||
            string.IsNullOrWhiteSpace(this.UserName.Text))
        {
            return;
        }

        string? user = this.ViewModel.UserNames.FirstOrDefault(u => u.Equals(this.UserName.SelectedItem));

        if (user == null)
        {
            user = this.UserName.Text;
            this.ViewModel.SetCurrentUser(user);
            this.WriteAndShutdown();
            return;
        }

        this.ViewModel.SetCurrentUser(user);
        this.UserName.SelectedIndex = this.UserName.Items.IndexOf(user);
        this.UserName.SelectedValue = this.UserName.SelectedItem;
        this.WriteAndShutdown();
    }

    private void WriteAndShutdown()
    {
        if (this.ViewModel == null)
        {
            return;
        }

        bool ok = this.ViewModel.WriteCurrentUserToGit();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopApp)
        {
            desktopApp.Shutdown(ok ? 0 : 1);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}