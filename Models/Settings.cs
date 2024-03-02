using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CommitAs.Models
{
    public class Settings
    {
        private const string SettingsFile = "config.json";
        private readonly IConfigurationRoot root;
        private readonly List<User> users;
        private string? email;
        private User? currentUser;

        public Settings()
        {
            this.root = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFile, true, false)
                .Build();

            this.users = this.root.GetSection(nameof(this.Users)).Get<List<User>>(); 
            this.users ??= [];
            this.users = this.users.Distinct().ToList();
            this.users.Sort();

            this.email = this.root.GetSection(nameof(this.Email)).Value;

            this.currentUser = this.root.GetSection(nameof(this.CurrentUser)).Get<User>();

            if (this.currentUser != null &&
                !this.users.Contains(this.currentUser))
            {
                this.AddUser(this.currentUser);
            }

            var cmdSection = this.root.GetSection(nameof(this.Command));
            if (cmdSection != null &&
                !string.IsNullOrWhiteSpace(cmdSection.Value))
            {
                this.Command = cmdSection.Value;
            }

            this.Save();
        }

        public IReadOnlyList<User>? Users => this.users;

        public string? Email 
        { 
            get => this.email;
            set
            {
                if (this.email == value)
                {
                    return;
                }

                this.email = value;
                this.Save();
            }
        }

        public User? CurrentUser
        {
            get => this.currentUser;
            set
            {
                if (this.currentUser == value)
                {
                    return;
                }

                this.currentUser = value;
                this.Save();
            }
        }

        public string? Command
        {
            get;
        }

        = "d: && cd d:\\CommitAs && git config user.name ";

        public void AddUser(User? user)
        {
            if (user == null ||
                this.users.Contains(user))
            {
                return;
            }

            this.users.Add(user);
            this.users.Sort();
            this.Save();
        }

        public void ClearUsers()
        {
            this.users.Clear();
            this.Save();
        }

        private (bool result, Exception? exception) Save(bool format = true)
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this, format ? options : null));

                return (true, null);
            }
            catch (Exception exception)
            {
                return (false, exception);
            }
        }
    }
}
