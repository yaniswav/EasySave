using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EasySave;  // Use your actual namespace where BackupManager is defined.

public class BackupServer
{
    private TcpListener listener;
    private readonly int port;
    private BackupManager backupManager;

    public BackupServer(int port)
    {
        this.port = port;
        backupManager = new BackupManager();  
        backupManager.LoadBackupJobs();
    }

    public async Task Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Server started on port {port}. Waiting for connections...");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");
            HandleClient(client);
        }
    }

    private async void HandleClient(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int byteCount = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (byteCount == 0)  // Client disconnected
                {
                    Console.WriteLine("Client disconnected.");
                    break;  // Exit the loop to handle client disconnection
                }

                string command = Encoding.UTF8.GetString(buffer, 0, byteCount);
                ProcessCommand(command);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            client.Close();
        }

        // After a client disconnects or an error occurs, the server continues to run,
        // waiting for new connections without exiting the HandleClient method.
    }

    private void ProcessCommand(string command)
    {
        string[] commandParts = command.Split(':');
        if (commandParts.Length < 2)
        {
            Console.WriteLine("Invalid command format.");
            return;
        }

        string action = commandParts[0];
        string[] jobNames = commandParts[1].Split(',');

        switch (action.ToLower())
        {
            case "start":
                backupManager.ExecuteJobs(jobNames);
                break;
            case "pause":
                foreach (var jobName in jobNames)
                {
                    backupManager.PauseJob(jobName);
                }
                break;
            case "resume":
                foreach (var jobName in jobNames)
                {
                    backupManager.ResumeJob(jobName);
                }
                break;
            case "stop":
                foreach (var jobName in jobNames)
                {
                    backupManager.StopJob(jobName);
                }
                break;
            default:
                Console.WriteLine("Unknown command received.");
                break;
        }
    }
}
