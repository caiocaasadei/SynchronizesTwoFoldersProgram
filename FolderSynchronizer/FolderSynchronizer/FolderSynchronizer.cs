using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;

class FolderSynchronizer
{
    // Paths for source and replica folders, synchronization interval, and log file path
    private readonly string sourcePath;
    private readonly string replicaPath;
    private readonly int syncInterval;
    private readonly string logFilePath;
    private static System.Timers.Timer syncTimer;

    // Constructor initializes folder paths, interval, and log file path
    public FolderSynchronizer(string sourcePath, string replicaPath, int syncInterval, string logFilePath)
    {
        this.sourcePath = sourcePath;
        this.replicaPath = replicaPath;
        this.syncInterval = syncInterval;
        this.logFilePath = logFilePath;
    }

    // Start method sets up the timer for periodic synchronization
    public void Start()
    {
        syncTimer = new System.Timers.Timer(syncInterval * 1000); // Timer interval in milliseconds
        syncTimer.Elapsed += (sender, e) => SynchronizeFolders(); // Event triggered on each interval
        syncTimer.AutoReset = true; // Timer resets after each interval
        syncTimer.Start(); // Start the timer
        Console.WriteLine($"Synchronization started. Interval: {syncInterval} seconds.");
    }

    // Method to perform folder synchronization
    private void SynchronizeFolders()
    {
        try
        {
            Console.WriteLine("\nStarting synchronization...");
            SyncDirectories(new DirectoryInfo(sourcePath), new DirectoryInfo(replicaPath));
            Console.WriteLine("Synchronization completed.\n");
        }
        catch (Exception ex)
        {
            Log($"Error during synchronization: {ex.Message}");
        }
    }

    // Recursive method to synchronize directories and their contents
    private void SyncDirectories(DirectoryInfo sourceDir, DirectoryInfo replicaDir)
    {
        // Create replica directory if it doesn't exist
        if (!replicaDir.Exists)
        {
            replicaDir.Create();
            Log($"Created directory: {replicaDir.FullName}");
        }

        // Get files in source and replica directories
        var sourceFiles = sourceDir.GetFiles();
        var replicaFiles = replicaDir.GetFiles();

        // Copy or update files from source to replica
        foreach (var file in sourceFiles)
        {
            var targetFilePath = Path.Combine(replicaDir.FullName, file.Name);
            // Copy file if it doesn't exist or if it's different in the replica
            if (!File.Exists(targetFilePath) || !FilesAreEqual(file.FullName, targetFilePath))
            {
                file.CopyTo(targetFilePath, true);
                Log($"Copied file: {file.FullName} -> {targetFilePath}");
            }
        }

        // Remove files from replica if they don't exist in source
        foreach (var file in replicaFiles)
        {
            if (!sourceFiles.Any(f => f.Name == file.Name))
            {
                file.Delete();
                Log($"Deleted file: {file.FullName}");
            }
        }

        // Get subdirectories in source and replica directories
        var sourceSubDirs = sourceDir.GetDirectories();
        var replicaSubDirs = replicaDir.GetDirectories();

        // Recursively synchronize subdirectories
        foreach (var subDir in sourceSubDirs)
        {
            var targetSubDirPath = Path.Combine(replicaDir.FullName, subDir.Name);
            SyncDirectories(subDir, new DirectoryInfo(targetSubDirPath));
        }

        // Remove subdirectories in replica that don't exist in source
        foreach (var subDir in replicaSubDirs)
        {
            if (!sourceSubDirs.Any(d => d.Name == subDir.Name))
            {
                Directory.Delete(subDir.FullName, true);
                Log($"Deleted directory: {subDir.FullName}");
            }
        }
    }

    // Method to check if two files are identical using MD5 hash
    private bool FilesAreEqual(string filePath1, string filePath2)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream1 = File.OpenRead(filePath1))
            using (var stream2 = File.OpenRead(filePath2))
            {
                var hash1 = md5.ComputeHash(stream1);
                var hash2 = md5.ComputeHash(stream2);
                return hash1.SequenceEqual(hash2); // True if hashes are equal
            }
        }
    }

    // Log method to record messages to both console and log file
    private void Log(string message)
    {
        string logEntry = $"{DateTime.Now}: {message}";
        Console.WriteLine(logEntry); // Write to console
        File.AppendAllText(logFilePath, logEntry + Environment.NewLine); // Write to log file
    }

    // Main method for handling command-line arguments and starting synchronization
    static void Main(string[] args)
    {
        // Check if there are enough arguments
        if (args.Length < 4)
        {
            Console.WriteLine("Usage: FolderSynchronizer <sourcePath> <replicaPath> <syncIntervalSeconds> <logFilePath>");
            return;
        }

        // Assign command-line arguments to variables
        string sourcePath = args[0];
        string replicaPath = args[1];
        int syncInterval = int.Parse(args[2]);
        string logFilePath = args[3];

        // Initialize FolderSynchronizer and start the sync process
        var synchronizer = new FolderSynchronizer(sourcePath, replicaPath, syncInterval, logFilePath);
        synchronizer.Start();

        // Wait for user input to stop synchronization
        Console.WriteLine("Press 'Enter' to stop the synchronization...");
        Console.ReadLine();
        syncTimer.Stop();
        syncTimer.Dispose();
    }
}
