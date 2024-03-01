using CommitAs.Models;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;

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
                this.HandleActivation();
                Disposable
                    .Create(() => this.HandleDeactivation())
                    .DisposeWith(disposables);
            });

            if (this.Settings.Users != null)
            {
                this.users.AddRange(this.Settings.Users);
            }
        }

        private void HandleActivation()
        {
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
