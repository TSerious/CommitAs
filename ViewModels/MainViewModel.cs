using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CommitAs.Models;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace CommitAs.ViewModels
{
    /// <summary>
    /// The model for the main view.
    /// </summary>
    public class MainViewModel : ViewModelBase, IActivatableViewModel
    {
        private readonly ObservableCollection<string> userNames = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
        {
            this.Activator = new ViewModelActivator();

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

        /// <inheritdoc/>
        public ViewModelActivator Activator { get; }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public Settings Settings { get; } = new Settings();

        /// <summary>
        /// Gets the names of all users.
        /// </summary>
        public ObservableCollection<string> UserNames => this.userNames;

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
        [Reactive]
        public User? CurrentUser { get; set; }

        /// <summary>
        /// Sets <see cref="CurrentUser"/> by name.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        public void SetCurrentUser(string userName)
        {
            if (this.Settings.Users == null)
            {
                this.CurrentUser = null;
                return;
            }

            this.CurrentUser = this.Settings.Users.FirstOrDefault(u => u.Name == userName);
        }

        /// <summary>
        /// Configures the git reposiroty to use the <see cref="CurrentUser"/>.
        /// </summary>
        /// <returns>True if everything went well.</returns>
        public bool WriteCurrentUserToGit()
        {
            if (string.IsNullOrEmpty(this.Settings.Command) ||
                this.CurrentUser == null)
            {
                return false;
            }

            return Git.WriteUserToGit(
                this.Settings.Command,
                this.CurrentUser.Name,
                string.IsNullOrWhiteSpace(this.CurrentUser.Email) ? this.Settings.Email : this.CurrentUser.Email);
        }

        private void HandleActivation(CompositeDisposable disposables)
        {
            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(this.UserNames, nameof(this.UserNames.CollectionChanged))
                .Select(e => e.EventArgs)
                .Subscribe(args => this.UpdateUsers(args))
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
    }
}
