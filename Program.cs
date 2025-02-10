// See https://aka.ms/new-console-template for more information
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

class Program
{
    private static string vlcPath = @"C:\Program Files\VideoLAN\VLC\vlc.exe";
    private static string _sampleVideo1 = "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
    private static string _card1 = "-";
    private static string _card2 = "-";
    private static string _card3 = "-";
    private static Process vlcProcess;
    private static StreamWriter vlcStream;

    static void Main()
    {
        Console.WriteLine("Hello, World!");
        KillVLC();
        // Play video at startup
        StartVLC();
        Console.WriteLine($"Playing video: {_sampleVideo1}");
        ChangeVideo(_sampleVideo1);

        while (true)
        {
            Console.WriteLine("Selec your sensor (1, 2 or 3)");
            string sensor = Console.ReadLine();

            while (sensor != "1" && sensor != "2" && sensor != "3") {
                Console.WriteLine("Incorrect: Please selec your sensor from (1, 2 or 3)");
                sensor = Console.ReadLine();
            }

            Console.WriteLine(string.Format("You choose sensor {0}",sensor));

            Console.WriteLine("Pick your card");

            string card = Console.ReadLine();

            if (sensor == "1")
                _card1 = card;
            else if (sensor == "2")
                _card2 = card;
            else 
                _card3 = card;

            Console.WriteLine(string.Format("You pick Card {0} for sensor {1}", card, sensor));
            Console.WriteLine(string.Format("Card 1 = {0}", _card1));
            Console.WriteLine(string.Format("Card 2 = {0}", _card2));
            Console.WriteLine(string.Format("Card 3 = {0}", _card3));

            string combinedCard = _card1 + _card2 + _card3;

            string videoUrl = GetVideoUrl(combinedCard);

            if (!string.IsNullOrEmpty(videoUrl))
            {
                Console.WriteLine($"Playing video: {videoUrl}");
                ChangeVideo(videoUrl);
            }
            else
            {
                Console.WriteLine($"No video for: {combinedCard}");
            }


        }
    }

    static void StartVLC()
    {
        if (!System.IO.File.Exists(vlcPath))
        {
            Console.WriteLine("Error: VLC not found at " + vlcPath);
            return;
        }

        // Start VLC with RC mode on port 4212
        vlcProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = vlcPath,
                Arguments = "--extraintf rc --rc-host=localhost:4212",
                UseShellExecute = false,
                RedirectStandardInput = true,
                CreateNoWindow = true,     // Hides the command prompt window
            }
        };
        vlcProcess.Start();
        Thread.Sleep(1000); // Wait for VLC to start

        // Connect to VLC via TCP
        TcpClient client = new TcpClient("localhost", 4212);
        vlcStream = new StreamWriter(client.GetStream()) { AutoFlush = true };

        // Clear VLC startup text
        Thread.Sleep(500);
    }

    static void ChangeVideo(string videoUrl)
    {
        if (vlcStream != null)
        {
            vlcStream.WriteLine($"add {videoUrl}"); // Play new video
        }
    }

    static string GetVideoUrl(string combinedCard)
    {
        // Define video URLs based on card combinations
        return combinedCard switch
        {
            "A--" => "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
            "-A-" => "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4",
            "--A" => "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
            "B--" => "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4",
            "-B-" => "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
            "--B" => "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4",
            "AA-" => "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4",
            "AB-" =>  "http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4",
            _ => ""
        };
    }

    static void KillVLC()
    {
        foreach (var process in Process.GetProcessesByName("vlc"))
        {
            process.Kill();
        }
        Console.WriteLine("VLC process killed.");
    }
}
