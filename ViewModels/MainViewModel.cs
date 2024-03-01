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
        private readonly ObservableCollection<User> users = [];

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
                this.users.AddRange(this.Settings.Users);
            }
        }

        private void HandleActivation(CompositeDisposable disposables)
        {
            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(this.Users, nameof(this.Users.CollectionChanged))
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

            if (!this.Users.Any())
            {
                this.Settings.ClearUsers();
                return;
            }

            if (args.NewItems == null ||
                args.NewItems.Count <= 0)
            {
                return;
            }

            foreach (var user in args.NewItems)
            {
                this.Settings.AddUser(user as User);
            }
        }

        private void HandleDeactivation()
        {

        }

        public ViewModelActivator Activator { get; }

        public Settings Settings { get; } = new Settings();

        public ObservableCollection<User> Users => this.users;

        [Reactive]
        public User? CurrentUser { get; set; }
    }
}
