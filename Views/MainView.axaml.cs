using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using CommitAs.Models;
using CommitAs.ViewModels;
using System;
using System.Linq;

namespace CommitAs.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();

        //this.UserName.PropertyChanged += UserName_PropertyChanged;
        //this.UserName.KeyDown += UserName_KeyDown;
    }

    private void UserName_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key != Avalonia.Input.Key.Tab)
        {
            return;
        }

        this.SelectMostSimularUserAsCurrent();
    }

    private void UserName_PropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"{this.UserName.Text}");

        var user = this.GetMostSimilarUser(this.UserName.Text);
        if (user == null)
        {
            this.UserName.PlaceholderText = string.Empty;
            return;
        }

        this.UserName.PlaceholderText = user.Name;
        this.UserName.Text = user.Name.Substring(0, this.UserName.Text.Length);
    }

    private void UserName_TextSubmitted(FluentAvalonia.UI.Controls.FAComboBox sender, FluentAvalonia.UI.Controls.FAComboBoxTextSubmittedEventArgs args)
    {
        System.Diagnostics.Debug.WriteLine("committed");
        //this.UserName.Items.Add(args.Text);
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
            return;
        }

        this.UserName.SelectedIndex = this.UserName.Items.IndexOf(user);
        this.UserName.Text = user.Name;
        (this.DataContext as MainViewModel).CurrentUser = user;
    }
}