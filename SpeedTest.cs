using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace SpeedTest
{
    internal class SpeedTest
    {
        private const string TARGET_FILE = "download.dummy";

        private readonly string url;
        private readonly List<double> updatedSizes = new List<double>();
        private long fileSize;

        public SpeedTest(string url)
        {
            this.url = url;
        }

        public void Run()
        {
            fileSize = GetContentLength();

            if (fileSize <= 0)
            {
                Console.WriteLine("URL ist ungültig");
                return;
            }

            StartTest();
        }

        private long GetContentLength()
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = "HEAD";

            var response = request.GetResponse();

            return response.ContentLength;
        }

        private void StartTest()
        {
            DownloadFile();

            PrintFinalResult();

            Cleanup();
        }

        private void DownloadFile()
        {
            var client = new WebClient();
            client.DownloadFileAsync(new Uri(url), TARGET_FILE);

            while (client.IsBusy)
            {
                CalculateSize(out var newSize, out var updatedSize);
                PrintCurrentResult(newSize, updatedSize);
            }
        }

        private void CalculateSize(out long newSize, out double updatedSize)
        {
            var oldSize = GetFileSize();
            var oldTime = DateTime.Now;

            Thread.Sleep(1000);

            newSize = GetFileSize();
            var newTime = DateTime.Now;

            var sizeDiff = newSize - oldSize;
            var timeDiff = newTime - oldTime;

            updatedSize = sizeDiff / timeDiff.TotalSeconds;
            updatedSizes.Add(updatedSize);
        }

        private static long GetFileSize()
        {
            if (!File.Exists(TARGET_FILE))
            {
                return 0;
            }

            var fileInfo = new FileInfo(TARGET_FILE);

            return fileInfo.Length;
        }

        private void PrintCurrentResult(long newSize, double updatedSize)
        {
            Console.Clear();
            Console.WriteLine("Fortschritt: {0:N2} MB von {1:N2} MB", GetByteToMegabyte(newSize),
                GetByteToMegabyte(fileSize));
            Console.WriteLine("Aktuelle Geschwindigkeit: {0:N2} MB/s", GetByteToMegabyte(updatedSize));
        }

        private void PrintFinalResult()
        {
            var average = updatedSizes.Average();
            var max = updatedSizes.Max();
            var min = updatedSizes.Min();

            Console.WriteLine("*****************************************************************************");
            Console.WriteLine("* === Ergebnis === ");
            Console.WriteLine("*");
            Console.WriteLine("* Geringste Geschwindigkeit: {0:N2} MB/s", GetByteToMegabyte(min));
            Console.WriteLine("* Höchste Geschwindigkeit: {0:N2} MB/s", GetByteToMegabyte(max));
            Console.WriteLine("* Durchschnittliche Geschwindigkeit: {0:N2} MB/s", GetByteToMegabyte(average));
            Console.WriteLine("*");
            Console.WriteLine("*****************************************************************************");
        }

        private static double GetByteToMegabyte(double byteAmount)
        {
            return byteAmount / 1024 / 1024;
        }

        private void Cleanup()
        {
            if (File.Exists(TARGET_FILE))
            {
                File.Delete(TARGET_FILE);
            }
        }
    }
}
