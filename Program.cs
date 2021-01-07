using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadToDropBox
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run(Upload);
            task.Wait();
            Console.WriteLine("All files have been uploaded to to your dropbox");
        }

        static async Task Upload()
        {
            var dbx = new DropboxClient(ConfigurationManager.AppSettings["AccessToken"]);
            var directoryPath = @"C:\YOUR\DIRECTORY\CONTAINING\UPLOAD\FILES";

            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                if (File.Exists(filePath))
                {
                    var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                    byte[] content;
                    using (var reader = new BinaryReader(fileStream))
                    {
                        content = reader.ReadBytes((int)fileStream.Length);
                    }

                    using (var mem = new MemoryStream(content))
                    {
                        string filename = filePath.Substring(filePath.LastIndexOf("\\")+1,
                            filePath.Length - filePath.LastIndexOf("\\")-1);

                        Console.WriteLine("Uploading "+filename);

                        //You should have a Uploads folder in your dropbox
                        //otherwise change the folder name or upload the files to root
                        // e.g. /filename (root) instead of /Uploads/filename (you can name your folders differently)
                        await dbx.Files.UploadAsync("/Uploads/" + filename,
                            WriteMode.Overwrite.Instance, body: mem);
                    }
                }
            }
        }
    }
}
