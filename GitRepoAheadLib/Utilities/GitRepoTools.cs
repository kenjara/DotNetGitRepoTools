namespace GitRepoAheadLib.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    using GitRepoAheadLib.Structure;

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

                    if (UpdateRepoStatus(repo))
                    {
                        // Remove path from repo name
                        var repoName = GetRepoName(rootFolder, repo);
                        unpushedRepos.Add(repoName);
                    }
                }
            }

            return unpushedRepos;
        }

        private static bool UpdateRepoStatus(string repoPath)
        {
            var ahead = false;

            // Setup git command
            ProcessStartInfo startInfo =
                new ProcessStartInfo("cmd", "/c " + $"git -C {repoPath} branch -v")
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

            Process process = Process.Start(startInfo);

            if (process == null)
            {
                return false;
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
                            ahead = true;
                        }
                    }
                };
            process.BeginOutputReadLine();
            process.WaitForExit();

            return ahead;
        }

        private static void UpdateRepoStatus(RepositoryStatus repoStatus)
        {
            // Setup git command
            ProcessStartInfo startInfo =
                new ProcessStartInfo("cmd", "/c " + $"git -C {repoStatus.Path} remote update&git -C {repoStatus.Path} branch -v")
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

            Process process = Process.Start(startInfo);

            if (process == null)
            {
                return;
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
                            repoStatus.Ahead = true;

                            repoStatus.AheadCount = GetCount(e.Data, "ahead ");
                        }

                        // Check for ahead in string to see if ahead of remote.
                        if (e.Data.Contains("behind"))
                        {
                            // Flag unpushed changes
                            repoStatus.Behind = true;

                            repoStatus.BehindCount = GetCount(e.Data, "behind ");                          
                        }
                    }
                };
            process.BeginOutputReadLine();
            process.WaitForExit();
        }

        private static int GetCount(string responseString, string searchString)
        {
            int count = 0;

            var subString = responseString.Substring(responseString.IndexOf(searchString, StringComparison.Ordinal) + searchString.Length);

            var countString = string.Empty;

            foreach (var c in subString)
            {
                if (char.IsDigit(c))
                {
                    countString += c;
                }
                else
                {
                    break;
                }
            }

            if (int.TryParse(countString, out var parsedValue))
            {
                count = parsedValue;
            }

            return count;
        }

        private static string GetRepoName(string rootFolder, string repo)
        {
            var repoName = repo.Replace(rootFolder, string.Empty);
            repoName = repoName.Replace(@"\", string.Empty);
            return repoName;
        }

        public static List<string> GetGitReposInDirString(string rootFolder)
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

        public static List<RepositoryStatus> GetGitReposInDir(string rootFolder)
        {
            var detectedRepos = new List<RepositoryStatus>();

            if (Directory.Exists(rootFolder))
            {
                var repos = Directory.GetDirectories(rootFolder);

                foreach (var repo in repos)
                {
                    var path = $"{repo}\\.git";

                    if (Directory.Exists(path))
                    {
                        var repoName = GetRepoName(rootFolder, repo);

                        var repoStatus = new RepositoryStatus(repoName, path);

                        detectedRepos.Add(repoStatus);
                    }
                }
            }

            return detectedRepos;
        }

        public static List<RepositoryStatus> GetAllReposAndStatusFromRootDir(string rootFolder)
        {
            var repos = GetGitReposInDir(rootFolder);

            foreach (var repositoryStatus in repos)
            {
                // Update status of each repository
                UpdateRepoStatus(repositoryStatus);
            }

            return repos;
        }
    }
}
