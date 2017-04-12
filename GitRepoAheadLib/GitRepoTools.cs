namespace GitRepoAheadLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public class GitRepoTools
    {
        /// <summary>
        /// Looks for git repositories within the root folder and checks to see if they are ahead of the remote repository.
        /// </summary>
        /// <param name="rootFolder">
        /// The root folder.
        /// </param>
        /// <returns>
        /// The list or repositories that are ahead <see cref="List"/>.
        /// </returns>
        public static List<string> GetAheadReposFromRootDir(string rootFolder)
        {
            var unpushedRepos = new List<string>();

            if (Directory.Exists(rootFolder))
            {
                var repos = Directory.GetDirectories(rootFolder);

                foreach (var repo in repos)
                {
                    if (!Directory.Exists($"{repo}\\.git"))
                    {
                        continue;
                    }

                    // Setup git command
                    ProcessStartInfo startInfo =
                        new ProcessStartInfo("cmd", "/c " + $"git -C {repo} branch -v")
                            {
                                WindowStyle =
                                    ProcessWindowStyle
                                        .Hidden,
                                UseShellExecute = false,
                                RedirectStandardOutput =
                                    true,
                                CreateNoWindow = true
                            };

                    Process process = Process.Start(startInfo);


                    if (process == null)
                    {
                        continue;
                    }

                    // Create handler for output event
                    process.OutputDataReceived += (s, e) =>
                        {
                            if (e.Data != null)
                            {
                                // Check for ahead in string to see if ahead of remote.
                                if (e.Data.Contains("ahead"))
                                {
                                    // Flag unpushed changes

                                    // Remove path from repo name
                                    var repoName = GetRepoName(rootFolder, repo);
                                    unpushedRepos.Add(repoName);
                                }
                            }
                        };
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                }
            }

            return unpushedRepos;
        }

        private static string GetRepoName(string rootFolder, string repo)
        {
            var repoName = repo.Replace(rootFolder, string.Empty);
            repoName = repoName.Replace(@"\", string.Empty);
            return repoName;
        }

        public static List<string> GetGitReposInDir(string rootFolder)
        {
            var detectedRepos = new List<string>();

            if (Directory.Exists(rootFolder))
            {
                var repos = Directory.GetDirectories(rootFolder);

                foreach (var repo in repos)
                {
                    if (Directory.Exists($"{repo}\\.git"))
                    {
                        var repoName = GetRepoName(rootFolder, repo);
                        detectedRepos.Add(repoName);
                    }
                }
            }

            return detectedRepos;
        }
    }
}
