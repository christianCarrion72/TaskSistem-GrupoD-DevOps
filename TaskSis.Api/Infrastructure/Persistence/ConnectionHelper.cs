using System;
using System.IO;

namespace TaskSis.Api.Infrastructure.Persistence;

public static class ConnectionHelper
{
    public static string GetConnectionString(string? defaultConnectionString = null)
    {
        LoadEnvFile();

        string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if (!string.IsNullOrEmpty(connectionString))
        {
            return connectionString;
        }

        string? host = Environment.GetEnvironmentVariable("DB_HOST");
        if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" && host == "localhost")
        {
            host = "db";
        }

        string? port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        string? database = Environment.GetEnvironmentVariable("DB_DATABASE");
        string? username = Environment.GetEnvironmentVariable("DB_USERNAME");
        string? password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        if (!string.IsNullOrEmpty(host) && 
            !string.IsNullOrEmpty(database) && 
            !string.IsNullOrEmpty(username) && 
            !string.IsNullOrEmpty(password))
        {
            return $"Host={host};Port={port};Database={database};Username={username};Password={password};";
        }

        return defaultConnectionString ?? "Host=localhost;Port=5432;Database=tasksis;Username=tasksis;Password=tasksis123";
    }

    private static void LoadEnvFile()
    {
        try
        {
            string currentDir = AppDomain.CurrentDomain.BaseDirectory;
            string? envPath = null;
            
            for (int i = 0; i < 5; i++)
            {
                string checkPath = Path.Combine(currentDir, ".env");
                if (File.Exists(checkPath))
                {
                    envPath = checkPath;
                    break;
                }
                var parent = Directory.GetParent(currentDir);
                if (parent == null) break;
                currentDir = parent.FullName;
            }

            if (string.IsNullOrEmpty(envPath))
            {
                string cwd = Directory.GetCurrentDirectory();
                for (int i = 0; i < 5; i++)
                {
                    string checkPath = Path.Combine(cwd, ".env");
                    if (File.Exists(checkPath))
                    {
                        envPath = checkPath;
                        break;
                    }
                    var parent = Directory.GetParent(cwd);
                    if (parent == null) break;
                    cwd = parent.FullName;
                }
            }

            if (!string.IsNullOrEmpty(envPath))
            {
                foreach (var line in File.ReadAllLines(envPath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                        continue;

                    var parts = line.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();
                        if (value.StartsWith("\"") && value.EndsWith("\""))
                            value = value.Substring(1, value.Length - 2);
                        else if (value.StartsWith("'") && value.EndsWith("'"))
                            value = value.Substring(1, value.Length - 2);

                        if (!string.IsNullOrEmpty(key) && string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                        {
                            Environment.SetEnvironmentVariable(key, value);
                        }
                    }
                }
            }
        }
        catch
        {
            // Ignorar errores al cargar .env
        }
    }
}
