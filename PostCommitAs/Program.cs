// See https://aka.ms/new-console-template for more information
using CommitAs.Core.Models;

Settings settings = new();
if (Git.AmendAuthor(settings, settings.CurrentUser?.Name, settings.GetCurrentEmail()))
{
    Environment.Exit(0);
    return;
}

Environment.Exit(3);
