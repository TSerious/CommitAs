using System.Diagnostics;

namespace CommitAs.Models
{
    public static class Git
    {
        public static bool WriteUserToGit(string command, string name, string email)
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

            var processInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            // *** Redirect the output ***
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;

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
