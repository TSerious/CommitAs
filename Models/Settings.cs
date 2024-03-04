using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace CommitAs.Models
{
    /// <summary>
    /// Stores the settings of the application.
    /// </summary>
    public class Settings
    {
        private const string SettingsFile = "config.json";
        private readonly IConfigurationRoot root;
        private readonly List<User> users;
        private string? email;
        private User? currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.root = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFile, true, false)
                .Build();

#pragma warning disable CS8601 // Possible null reference assignment.
            this.users = this.root.GetSection(nameof(this.Users)).Get<List<User>>();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
            this.users ??= [];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
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
            else if (!string.IsNullOrEmpty(Environment.CurrentDirectory) &&
                     Path.GetPathRoot(Environment.CurrentDirectory) is string drive)
            {
                drive = drive[..2];
                var dir = Path.GetFullPath(Environment.CurrentDirectory + "\\..");
                this.Command = $"{drive} \u0026\u0026 cd \"{dir}\" \u0026\u0026 git config user.name ";
            }

            this.Save();
        }

        /// <summary>
        /// Gets all users.
        /// </summary>
        public IReadOnlyList<User>? Users => this.users;

        /// <summary>
        /// Gets or sets the currently used email.
        /// If this and the <see cref="User.Email"/> of <see cref="CurrentUser"/>
        /// are empty the user.email field of git will not be changed.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the current user.
        /// </summary>
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

        /// <summary>
        /// Gets the Command that is executed to change the user name.
        /// </summary>
        public string? Command
        {
            get;
        }

        = string.Empty;

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="user">The user.</param>
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

        /// <summary>
        /// Removes all users.
        /// </summary>
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
