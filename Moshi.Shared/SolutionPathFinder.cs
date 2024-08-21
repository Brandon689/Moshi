namespace Moshi.Shared;

public static class SolutionPathFinder
{
    public static string GetSolutionDirectoryPath()
    {
        var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        while (directory != null && !IsSolutionDirectory(directory))
        {
            directory = directory.Parent;
        }
        return directory?.FullName;
    }

    private static bool IsSolutionDirectory(DirectoryInfo directory)
    {
        if (directory.GetFiles("*.sln").Length > 0)
        {
            return true;
        }
        return false;
    }
}