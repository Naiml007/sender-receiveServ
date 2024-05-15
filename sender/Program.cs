using System;
using System.IO;
using System.Net.Sockets;

class FileClient
{
    private const string ServerAddress = "127.0.0.1";
    private const int ServerPort = 5000;

    static void Main(string[] args)
    {
        Console.Write("Enter the file path: ");
        string filePath = Console.ReadLine();
        string fileName = Path.GetFileName(filePath);

        if (!File.Exists(filePath))
        {
            Console.WriteLine("File not found.");
            return;
        }

        byte[] fileData = File.ReadAllBytes(filePath);
        long fileLength = fileData.Length;

        using (TcpClient client = new TcpClient(ServerAddress, ServerPort))
        using (NetworkStream networkStream = client.GetStream())
        {
            try
            {
                // Send file name length
                byte[] fileNameBuffer = System.Text.Encoding.UTF8.GetBytes(fileName);
                byte[] fileNameLengthBuffer = BitConverter.GetBytes(fileNameBuffer.Length);
                networkStream.Write(fileNameLengthBuffer, 0, fileNameLengthBuffer.Length);

                // Send file name
                networkStream.Write(fileNameBuffer, 0, fileNameBuffer.Length);

                // Send file length
                byte[] fileLengthBuffer = BitConverter.GetBytes(fileLength);
                networkStream.Write(fileLengthBuffer, 0, fileLengthBuffer.Length);

                // Send file data
                networkStream.Write(fileData, 0, fileData.Length);
                Console.WriteLine($"File {fileName} sent to server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
