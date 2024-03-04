using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CommitAs.Models;
using CommitAs.ViewModels;
using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace CommitAs.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.UserName.WhenAnyValue(x => x.SelectedIndex, x => x.SelectedItem)
                .Skip(3)
                .Subscribe(_ => this.SetCurrentUser())
                .DisposeWith(disposables);

            Observable.FromEventPattern<KeyEventArgs>(this.UserName, nameof(this.UserName.KeyDown))
            .Subscribe(k => System.Diagnostics.Debug.WriteLine($"pressed: {k}"))
            .DisposeWith(disposables);

            var k = new KeyBinding();
            k.Gesture = new KeyGesture(Key.Enter);
            k.Command = ReactiveCommand.Create(new Action(() => { 
                this.SetCurrentUser();
            }));
            this.UserName.KeyBindings.Add(k);

            /*
            Observable.FromEventPattern<KeyEventArgs>(this.UserName, nameof(this.UserName.KeyDown))
                .Select(k => k.EventArgs.Key)
                .Where(k => k == Key.Enter)
                .Subscribe(_ => this.SetCurrentUser())
                .DisposeWith(disposables);
            */

            /*
            Observable.FromEventPattern<EventArgs>(this.UserName, nameof(this.UserName.GotFocus))
                .Subscribe(_ =>
                {
                    if (this.ViewModel.CurrentUser == null)
                    {
                        return;
                    }

                    this.UserName.Text = this.ViewModel.CurrentUser.Name;
                })
                .DisposeWith(disposables);

            Observable.FromEventPattern<EventArgs>(this.UserName, nameof(this.UserName.SelectionChanged))
                .Subscribe(_ =>
                {
                    if (this.ViewModel.CurrentUser == null)
                    {
                        return;
                    }

                    this.UserName.Text = this.ViewModel.CurrentUser.Name;
                })
                .DisposeWith(disposables);
            */

            this.SelectMostSimularUserAsCurrent();

            /*
            var b = new Avalonia.Data.Binding(nameof(this.ViewModel.CurrentUser), Avalonia.Data.BindingMode.OneWay);
            b.Converter = Converters.UserConverters.UserName;
            this.UserName.SelectedValueBinding = b;
            */

            this.UserName.Focus();
        });
    }


    private void UpdateUserNamePlaceholderAndText()
    {
        /*
        var user = this.GetMostSimilarUser(this.UserName.Text);
        
        if (user == null &&
            this.UserName.SelectionBoxItem is User selectedUser)
        {
            this.UserName.Text = selectedUser.Name;
            return;
        }

        if (user == null)
        {
            this.UserName.PlaceholderText = string.Empty;
            return;
        }

        if (!user.Name.Equals(this.UserName.Text, StringComparison.CurrentCultureIgnoreCase))
        {
            this.UserName.PlaceholderText = $"{user.Name}\t[Tab to select]";
        }
        
        //this.UserName.Text = user.Name[..this.UserName.Text.Length];
        */
    }

    private User? GetMostSimilarUser(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return (this.DataContext as MainViewModel)?.Settings.Users?.FirstOrDefault(x => x.Name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase));
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
            desktopApp.Shutdown(ok ? 10 : 0);
        }
    }
}