using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

class FileServer
{
    private const int Port = 5000;

    static void Main(string[] args)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, Port);
        listener.Start();
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            using (TcpClient client = listener.AcceptTcpClient())
            using (NetworkStream networkStream = client.GetStream())
            {
                try
                {
                    // Read file name length
                    byte[] fileNameLengthBuffer = new byte[4];
                    networkStream.Read(fileNameLengthBuffer, 0, fileNameLengthBuffer.Length);
                    int fileNameLength = BitConverter.ToInt32(fileNameLengthBuffer, 0);

                    // Read file name
                    byte[] fileNameBuffer = new byte[fileNameLength];
                    networkStream.Read(fileNameBuffer, 0, fileNameBuffer.Length);
                    string fileName = System.Text.Encoding.UTF8.GetString(fileNameBuffer, 0, fileNameBuffer.Length);
                    Console.WriteLine($"Receiving file: {fileName}");

                    // Read file length
                    byte[] fileLengthBuffer = new byte[8];
                    networkStream.Read(fileLengthBuffer, 0, fileLengthBuffer.Length);
                    long fileLength = BitConverter.ToInt64(fileLengthBuffer, 0);

                    // Read file data
                    byte[] fileBuffer = new byte[fileLength];
                    int bytesRead = 0;
                    while (bytesRead < fileLength)
                    {
                        bytesRead += networkStream.Read(fileBuffer, bytesRead, fileBuffer.Length - bytesRead);
                    }

                    // Save the file
                    File.WriteAllBytes(fileName, fileBuffer);
                    Console.WriteLine($"File {fileName} received and saved.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
