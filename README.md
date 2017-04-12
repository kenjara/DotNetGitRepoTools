# DotNetGitRepoTools
A C# library to run query git and return results. 

I built this project to help developers coming from TF and SVN to remember to push changes to the remote repository.

Example usage


class Program
    {
        
        private const string DevDirectory = @"C:\DevelopmentFolder";

        public static void Main(string[] args)
        {
            var unpushedRepos = GitRepoTools.GetAheadReposFromRootDir(DevDirectory);

            foreach (var unpushedRepo in unpushedRepos)
            {
                Console.WriteLine($"Repo {unpushedRepo} has unpushed changes!");
            }
            
            Console.ReadLine();
        }
    }

