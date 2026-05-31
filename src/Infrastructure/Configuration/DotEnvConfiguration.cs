namespace TaskManager.Infrastructure.Configuration;

/// <summary>
/// Loads a root <c>.env</c> file into environment variables before configuration binding.
/// ASP.NET Core maps <c>SECTION__KEY</c> variables to nested configuration (e.g. Jwt__Secret).
/// </summary>
public static class DotEnvConfiguration
{
    public static void Load(string? startDirectory = null)
    {
        var envPath = FindEnvFilePath(startDirectory ?? Directory.GetCurrentDirectory());
        if (envPath is null)
            return;

        DotNetEnv.Env.Load(envPath);
    }

    public static string? FindEnvFilePath(string startDirectory)
    {
        var directory = new DirectoryInfo(startDirectory);

        while (directory is not null)
        {
            var envFile = Path.Combine(directory.FullName, ".env");
            if (File.Exists(envFile))
                return envFile;

            if (File.Exists(Path.Combine(directory.FullName, "TaskManager.sln")))
                return null;

            directory = directory.Parent;
        }

        return null;
    }
}
