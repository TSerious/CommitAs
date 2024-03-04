using System.Diagnostics;

namespace CommitAs.Models
{
    /// <summary>
    /// This class contains some helper methods to interact with the git repository.
    /// </summary>
    public static class Git
    {
        /// <summary>
        /// Sets the user.name and user.email of a git repository.
        /// </summary>
        /// <param name="command">The command to set name and email.</param>
        /// <param name="name">The name of the user.</param>
        /// <param name="email">The email of the user. If this is null, the email will not be changed.</param>
        /// <returns>True if the command succeeded.</returns>
        public static bool WriteUserToGit(string command, string name, string? email = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            command += $"\"{name}\"";

            if (!string.IsNullOrWhiteSpace(email))
            {
                command += $" user.email \"{email}\"";
            }

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
