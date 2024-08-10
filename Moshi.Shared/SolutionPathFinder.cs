namespace Moshi.Shared;

public static class SolutionPathFinder
{
    public static string GetSolutionDirectoryPath()
    {
        // Start with the current directory
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (directory != null && !IsSolutionDirectory(directory))
        {
            directory = directory.Parent;
        }

        return directory?.FullName;
    }

    private static bool IsSolutionDirectory(DirectoryInfo directory)
    {
        // Check for the existence of a .sln file
        if (directory.GetFiles("*.sln").Length > 0)
        {
            return true;
        }

        // Check for other known folders or files that are always present in your solution
        // For example, if you always have a 'src' folder in your solution:
        if (directory.GetDirectories("src").Length > 0)
        {
            return true;
        }

        // Add more checks as needed based on your project structure

        return false;
    }
}
