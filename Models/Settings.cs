using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

namespace CommitAs.Models
{
    public class Settings
    {
        private const string SettingsFile = "config.json";
        private IConfigurationRoot root;

        public Settings()
        {
            this.root = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFile, true, true)
                .Build();

            this.Users = this.root.GetSection(nameof(this.Users)).Get<List<User>>();
            this.Users ??= new List<User>();

            this.Email = this.root.GetSection(nameof(this.Email)).Value;
        }

        public List<User>? Users { get; }

        public string? Email { get; set; }
    }
}
