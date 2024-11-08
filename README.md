# SynchronizesTwoFoldersProgram
Program that synchronizes two folders: source and replica. The program maintain a full, identical copy of source folder at replica folder.

1. Synchronization must be one-way: after the synchronization content of the replica folder should be modified to exactly match content of the source folder;

2. Synchronization should be performed periodically;

3. File creation/copying/removal operations should be logged to a file and to the console output;

4. Folder paths, synchronization interval and log file path should be provided using the command line arguments;

5. It is undesirable to use third-party libraries that implement folder synchronization;

6. It is allowed (and recommended) to use external libraries implementing other well-known algorithms. For example, there is no point in implementing yet another function that calculates MD5 if you need it for the task – it is perfectly acceptable to use a third-party (or built-in) library;



How to Use:

Build or Download the Program:

  If you're building it yourself, compile the provided C# code into an executable (FolderSynchronizer.exe). If you have the executable file already, move to the next step.

Run the Program: 

  Open a Command Prompt or PowerShell window and navigate to the folder where FolderSynchronizer.exe is located.

To run the program, use the following command:

FolderSynchronizer.exe "C:\Path\To\Source" "C:\Path\To\Replica" 600 "C:\Path\To\LogFile.txt"

Example:
FolderSynchronizer.exe "C:\MySourceFolder" "C:\MyReplicaFolder" 600 "C:\SyncLog.txt"
  "C:\MySourceFolder": The source folder path.
  "C:\MyReplicaFolder": The replica folder path.
  600: Synchronization interval in seconds (e.g., 600 seconds = 10 minutes).
  "C:\SyncLog.txt": Path to the log file where actions will be recorded.
