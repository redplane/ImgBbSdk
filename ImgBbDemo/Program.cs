using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ImgBbSdk.Services;

namespace ImgBbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var imgBbService = new ImgBbService();
                imgBbService.LoginAsync("<your imgbb username>", "<your imgbb password>", CancellationToken.None).Wait();

                var files = new[] {"D:\\Images\\1.png", "D:\\Images\\2.png", "D:\\Images\\3.jpg"};
                var tasks = new List<Task>();
                foreach (var file in files)
                {
                    var fileName = Guid.NewGuid().ToString("N");
                    var bytes = File.ReadAllBytes(file);
                    var task = imgBbService.UploadBinaryAsync(bytes, "image/png", fileName, CancellationToken.None);
                    
                    tasks.Add(task);
                }

                Task.WhenAll(tasks).Wait();
                Console.WriteLine("Done");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            Console.ReadLine();
        }
    }
}
