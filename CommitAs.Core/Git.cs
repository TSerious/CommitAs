namespace CommitAs.Core.Models
{
    using System.Diagnostics;

    /// <summary>
    /// This class contains some helper methods to interact with the git repository.
    /// </summary>
    public static class Git
    {
        /// <summary>
        /// Sets the user.name and user.email of a git repository.
        /// </summary>
        /// <param name="settings">The settings that hold some used properties.</param>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user. If this is null, the email will not be changed.</param>
        /// <returns>True if the command succeeded.</returns>
        public static bool WriteUserToGit(Settings settings, string name, string? email = null)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(settings.CommandConfigUserName))
            {
                return false;
            }

            string commandName = $"{settings.CommandPath} && {settings.CommandConfigUserName.Replace("{name}", $"{name}")}";
            bool ok = ExecuteCommand(commandName);

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(settings.CommandConfigUserEmail))
            {
                return ok;
            }

            string commandEmail = $"{settings.CommandPath} && {settings.CommandConfigUserEmail.Replace("{email}", $"{email}")}";
            ok &= ExecuteCommand(commandEmail);

            return ok;
        }

        /// <summary>
        /// Changes the autor of the most recent commit.
        /// </summary>
        /// <param name="settings">The settings that hold some used properties.</param>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>True if the command succeeded.</returns>
        public static bool AmendAuthor(Settings settings, string? name, string? email)
        {
            if (string.IsNullOrWhiteSpace(settings.CommandAmend) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string command = settings.CommandAmend.Replace("{name}", name);
            command = command.Replace("{email}", email);
            command = $"{settings.CommandPath} && {command}";
            return ExecuteCommand(command);
        }

        /// <summary>
        /// Executes a command line command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>True if the command succeded without errors.</returns>
        public static bool ExecuteCommand(string command)
        {
            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(processInfo);
            process?.WaitForExit();

            if (process == null)
            {
                return false;
            }

            return process.ExitCode == 0;
        }
    }
}
