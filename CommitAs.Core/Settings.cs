namespace CommitAs.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Stores the settings of the application.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets the name of the file that contains the settings.
        /// </summary>
        public static readonly string SettingsFile = "CommitAsConfig.json";

        /// <summary>
        /// Gets the placeholder for the name of a user.
        /// </summary>
        public static readonly string NamePlaceholder = "{name}";

        /// <summary>
        /// Gets the placeholder for the email of a user.
        /// </summary>
        public static readonly string EmailPlaceholder = "{email}";

        private readonly string settingsPath = string.Empty;
        private readonly IConfigurationRoot root;
        private readonly List<User> users;
        private string email = "user@mail.com";
        private User? currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            rootPath ??= Directory.GetCurrentDirectory();
            this.settingsPath = $"{rootPath}\\{SettingsFile}";

            this.root = new ConfigurationBuilder()
                .SetBasePath(rootPath)
                .AddJsonFile($"{rootPath}\\{SettingsFile}", true, false)
                .Build();

#pragma warning disable CS8601 // Possible null reference assignment.
            this.users = this.root.GetSection(nameof(this.Users)).Get<List<User>>();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
            this.users ??= [];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly
            this.users = this.users.Distinct().ToList();
            this.users.Sort();

            if (this.root.GetSection(nameof(this.Email)) is IConfigurationSection emailSection &&
                emailSection.Value != null)
            {
                this.email = emailSection.Value;
            }

            this.currentUser = this.root.GetSection(nameof(this.CurrentUser)).Get<User>();

            if (this.currentUser != null &&
                !this.users.Contains(this.currentUser))
            {
                this.AddUser(this.currentUser);
            }

            var commandPath = this.root.GetSection(nameof(this.CommandPath));
            if (commandPath != null &&
                !string.IsNullOrWhiteSpace(commandPath.Value))
            {
                this.CommandPath = commandPath.Value;
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var drive = Path.GetPathRoot(rootPath)[..2];
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                var dir = Path.GetFullPath(rootPath + "\\..");
                this.CommandPath = $"{drive} \u0026\u0026 cd \"{dir}\"";
            }

            var commandConfigUserName = this.root.GetSection(nameof(this.CommandConfigUserName));
            if (commandConfigUserName != null &&
                !string.IsNullOrWhiteSpace(commandConfigUserName.Value))
            {
                this.CommandConfigUserName = commandConfigUserName.Value;
            }
            else
            {
                this.CommandConfigUserName = $"git config user.name \"{NamePlaceholder}\"";
            }

            var commandConfigUserEmail = this.root.GetSection(nameof(this.CommandConfigUserEmail));
            if (commandConfigUserEmail != null &&
                !string.IsNullOrWhiteSpace(commandConfigUserEmail.Value))
            {
                this.CommandConfigUserEmail = commandConfigUserEmail.Value;
            }
            else
            {
                this.CommandConfigUserEmail = $"git config user.email \"{EmailPlaceholder}\"";
            }

            var commandAmend = this.root.GetSection(nameof(this.CommandAmend));
            if (commandAmend != null &&
                !string.IsNullOrWhiteSpace(commandAmend.Value))
            {
                this.CommandAmend = commandAmend.Value;
            }
            else
            {
                this.CommandAmend = $"git commit --no-verify --amend --author \"{NamePlaceholder} <{EmailPlaceholder}>\" --no-edit";
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
        public string Email
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
        /// Gets the command that is used to point to the folder containing the repository.
        /// </summary>
        public string? CommandPath
        {
            get;
        }

        = string.Empty;

        /// <summary>
        /// Gets the command that is used to change the user.name of a repository.
        /// </summary>
        public string? CommandConfigUserName
        {
            get;
        }

        = string.Empty;

        /// <summary>
        /// Gets the command that is used to change the user.email of a repository.
        /// </summary>
        public string? CommandConfigUserEmail
        {
            get;
        }

        = string.Empty;

        /// <summary>
        /// Gets the command that is used to change the author of the last commit.
        /// </summary>
        public string? CommandAmend
        {
            get;
        }

        = string.Empty;

        /// <summary>
        /// Gets the email that should currently be used.
        /// </summary>
        /// <returns>The email.</returns>
        public string GetCurrentEmail()
        {
            if (this.CurrentUser != null &&
                !string.IsNullOrEmpty(this.CurrentUser.Email))
            {
                return this.CurrentUser.Email;
            }

            return this.Email;
        }

        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The added user.</returns>
        public User? AddUser(User? user)
        {
            if (user == null ||
                this.users.Contains(user))
            {
                return this.Users?.FirstOrDefault(u => u.Equals(user));
            }

            this.users.Add(user);
            this.users.Sort();
            this.Save();

            return user;
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
                File.WriteAllText(this.settingsPath, JsonSerializer.Serialize(this, format ? options : null));

                return (true, null);
            }
            catch (Exception exception)
            {
                return (false, exception);
            }
        }
    }
}
