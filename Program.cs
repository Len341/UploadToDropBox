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
            var directoryPath = @"C:\Users\Leonard Botha\Documents\WORK\DROPBOXUPLOADS";

            foreach (string fileName in Directory.GetFiles(directoryPath))
            {
                if (File.Exists(fileName))
                {
                    var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                    byte[] content;
                    using (var reader = new BinaryReader(fileStream))
                    {
                        content = reader.ReadBytes((int)fileStream.Length);
                    }

                    using (var mem = new MemoryStream(content))
                    {
                        string name = fileName.Substring(fileName.LastIndexOf("\\")+1,
                            fileName.Length - fileName.LastIndexOf("\\")-1);

                        Console.WriteLine("Uploading "+name);
                        await dbx.Files.UploadAsync("/Uploads/" + name,
                            WriteMode.Overwrite.Instance, body: mem);
                    }
                }
            }
        }
    }
}
