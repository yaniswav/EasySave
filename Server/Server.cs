using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using EasySave;


public class Server
{
    private TcpListener listener;
    private readonly int port;
    private BackupManager backupManager;
    private ConfigModel config = ConfigModel.Instance;

    private bool isRunning = false;
    private CancellationTokenSource cts = new CancellationTokenSource();

    public Server(int port)
    {
        this.port = port;
        backupManager = new BackupManager();
        backupManager.LoadBackupJobs();
    }

    public async Task Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        isRunning = true;
        Console.WriteLine($"Server started on port {port}. Waiting for connections...");

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (listener.Pending())
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(cts.Token);
                    Console.WriteLine("Client connected.");
                    HandleClient(client);
                }
                else
                {
                    await Task.Delay(100); // Reduce CPU usage
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the cancellation is requested
            Console.WriteLine("Server stopping...");
        }
        finally
        {
            listener.Stop();
            isRunning = false;
        }
    }

    public void Stop()
    {
        cts.Cancel();
        listener?.Stop(); // Ensure listener is stopped to break out of the waiting state
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
                if (byteCount == 0) // Client disconnected
                {
                    Console.WriteLine("Client disconnected.");
                    break; // Exit the loop to handle client disconnection
                }

                string command = Encoding.UTF8.GetString(buffer, 0, byteCount);
                ProcessCommand(command, stream); // Include 'stream' as the second argument
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
    }

    private void ProcessCommand(string command, NetworkStream stream)
    {
        string[] commandParts = command.Split(':');
        string action = commandParts[0];
        string[] jobNames = commandParts.Length > 1 ? commandParts[1].Split(',') : Array.Empty<string>();

        switch (action.ToLower())
        {
            case "list":
                Console.WriteLine("Sending list of backup jobs to the client...");
                var backupJobs = config.LoadBackupJobs();
                SendToClient(backupJobs, stream);
                break;
            case "start":
                backupManager.ExecuteJobs(jobNames);
                SendProgressUpdates(stream);
                break;
            case "pause":
                foreach (var jobName in jobNames)
                {
                    backupManager.PauseJob(jobName); // Adjusted to call PauseJob for each jobName
                }

                break;
            case "resume":
                foreach (var jobName in jobNames)
                {
                    backupManager.ResumeJob(jobName); // Adjusted to call ResumeJob for each jobName
                }

                break;
            case "stop":
                foreach (var jobName in jobNames)
                {
                    backupManager.StopJob(jobName); // Adjusted to call StopJob for each jobName
                }

                break;
            default:
                Console.WriteLine("Unknown command received.");
                break;
        }
    }


    private async void SendProgressUpdates(NetworkStream stream)
    {
        while (!backupManager.AllJobsCompleted) // Assumes a method to check if all jobs are completed.
        {
            var progress = backupManager.GetBackupProgress();
            foreach (var kvp in progress)
            {
                var message = $"Progress for {kvp.Key}: {kvp.Value}%\n";
                Console.WriteLine($"Sending progress update: {message}");
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            }

            await Task.Delay(1);
        }
    }


    private void SendToClient(string message, NetworkStream stream)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private void SendToClient(List<BackupJobConfig> backupJobs, NetworkStream stream)
    {
        StringBuilder stringBuilder = new StringBuilder();
        if (backupJobs.Count == 0)
        {
            stringBuilder.AppendLine("No backups configured.");
        }
        else
        {
            foreach (var job in backupJobs)
            {
                stringBuilder.AppendLine(
                    $"Name: {job.Name}, Source: {job.SourceDir}, Destination: {job.DestinationDir}, Type: {job.Type}");
            }
        }

        byte[] data = Encoding.UTF8.GetBytes(stringBuilder.ToString());
        stream.Write(data, 0, data.Length);
    }
}