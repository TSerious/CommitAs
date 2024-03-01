using Avalonia;
using Avalonia.Controls;
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
            this.UserName.WhenAnyValue(x => x.Text)
                .Skip(1)
                .Subscribe(_ => this.UpdateUserNamePlaceholderAndText())
                .DisposeWith(disposables);

            this.UserName.WhenAnyValue(x => x.SelectionBoxItem)
                .Subscribe(item =>
                {
                    System.Diagnostics.Debug.WriteLine(item);

                    if (item == null ||
                        item is not User user)
                    {
                        this.UserName.Text = string.Empty;
                        return;
                    }

                    this.UserName.Text = user.Name;
                })
                .DisposeWith(disposables);

            Observable.FromEventPattern<KeyEventArgs>(this.UserName, nameof(this.UserName.KeyDown))
                .Select(k => k.EventArgs.Key)
                .Where(k => k == Key.Tab)
                .Subscribe(_ => this.SelectMostSimularUserAsCurrent())
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

            Observable.FromEventPattern<KeyEventArgs>(this.UserName, nameof(this.UserName.KeyDown))
                .Select(k => k.EventArgs.Key)
                .Where(k => k == Key.Enter)
                .Subscribe(_ => this.SetCurrentUser())
                .DisposeWith(disposables);

            this.UserName.IsTextSearchEnabled = true;

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
        var user = this.GetMostSimilarUser(this.UserName.Text);

        if (user == null)
        {
            this.UserName.Text = string.Empty;
            this.ViewModel.CurrentUser = user;
            return;
        }

        this.UserName.PlaceholderText = string.Empty;
        this.UserName.Text = user.Name;

        if (this.ViewModel == null)
        {
            return;
        }

        //this.UserName.SelectedValue = user;
        this.ViewModel.CurrentUser = user;
        this.UserName.SelectedIndex = this.UserName.Items.IndexOf(user);
    }

    private void SetCurrentUser()
    {
        if (this.ViewModel == null ||
            string.IsNullOrWhiteSpace(this.UserName.Text))
        {
            return;
        }

        var user = this.ViewModel.Users.FirstOrDefault(u => u.Name.Equals(this.UserName.Text));

        if (user == null)
        {
            user = new User(this.UserName.Text);
            this.ViewModel.Users.Insert(User.GetInsertIndex(this.ViewModel.Users, user), user);
        }

        this.ViewModel.CurrentUser = user;
        this.UserName.SelectedIndex = this.UserName.Items.IndexOf(user);
    }
}