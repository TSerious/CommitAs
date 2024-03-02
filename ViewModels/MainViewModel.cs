using CommitAs.Models;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace CommitAs.ViewModels
{
    public class MainViewModel : ViewModelBase, IActivatableViewModel
    {
        private readonly ObservableCollection<string> userNames = [];

        public MainViewModel() 
        {
            Activator = new ViewModelActivator();

            this.WhenActivated((CompositeDisposable disposables) =>
            {
                this.HandleActivation(disposables);
                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);
            });

            if (this.Settings.Users != null)
            {
                this.userNames.AddRange(this.Settings.Users.Select(u => u.Name));
            }
        }

        public void SetCurrentUser(string userName)
        {
            if (this.Settings.Users == null)
            {
                this.CurrentUser = null;
                return;
            }

            this.CurrentUser = this.Settings.Users.FirstOrDefault(u => u.Name == userName);
        }

        public bool WriteCurrentUserToGit()
        {
            return Git.WriteUserToGit(
                this.Settings.Command,
                this.CurrentUser.Name,
                string.IsNullOrWhiteSpace(this.CurrentUser.Email) ? this.Settings.Email : this.CurrentUser.Email);
        }

        private void HandleActivation(CompositeDisposable disposables)
        {
            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(this.UserNames, nameof(this.UserNames.CollectionChanged))
                .Select(e => e.EventArgs)
                .Subscribe(args => UpdateUsers(args))
                .DisposeWith(disposables);

            this.CurrentUser = this.Settings.CurrentUser;

            this.WhenAnyValue(x => x.CurrentUser)
                .Subscribe(user => this.Settings.CurrentUser = user)
                .DisposeWith(disposables);
        }

        private void UpdateUsers(NotifyCollectionChangedEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            if (!this.UserNames.Any())
            {
                this.Settings.ClearUsers();
                return;
            }

            if (args.NewItems == null ||
                args.NewItems.Count <= 0)
            {
                return;
            }

            foreach (string userName in args.NewItems)
            {
                this.Settings.AddUser(new User(userName));
            }
        }

        private void HandleDeactivation()
        {

        }

        public ViewModelActivator Activator { get; }

        public Settings Settings { get; } = new Settings();

        public ObservableCollection<string> UserNames => this.userNames;

        [Reactive]
        public User? CurrentUser { get; set; }
    }
}
