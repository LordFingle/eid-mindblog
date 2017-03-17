

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
 
namespace eiddataentityframework
{
    public class EntityAttachments
    {
        public string AttachmentsFolderPath { get; set; }
        private bool foldersCreated = false;
        public EntityAttachments()
        {
            AttachmentsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments");
        }

        public Stream Get(string id, string key)
        {
            var filePath = getInternalFilePath(id, key);
            if (!File.Exists(filePath)) return null;

            using(var sourceStream = File.OpenRead(filePath) )
            {
                var memoryStream = new MemoryStream();
                sourceStream.CopyTo(memoryStream);
                sourceStream.Close();
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        public void Delete(string id, string key)
        {

            var newFilename = getInternalFilePath(id, key);

            if (File.Exists(newFilename))
            {
                var errcnt = 0;

                while (true)
                {
                    try
                    {
                        File.Delete(newFilename);
                        break;
                    }
                    catch (Exception e)
                    {
                        errcnt++;
                        System.Threading.Thread.Sleep(1000);
                        if (errcnt > 5) throw e;   
                    }
                }
            }
        }

        public void Put(string id, string key, string sourcePath)
        {

            var newFilename = getInternalFilePath(id, key);
            
            File.Copy(sourcePath, newFilename);
        }


        public void Put(string id, string key, Stream sourceStream)
        {
            var newFilename = getInternalFilePath(id, key);
            var memoryStream = new MemoryStream();
            sourceStream.CopyTo(memoryStream);
            File.WriteAllBytes(newFilename, memoryStream.ToArray());
            sourceStream.Close();
            memoryStream.Close();
            sourceStream.Dispose();
            memoryStream.Dispose();
 

        }

        private string getInternalFilePath(string id, string simpleFilename)
        {
            if (!foldersCreated) createFolders();

            var pureFilename = String.Format("{0}{1}", id.Replace("-", ""), simpleFilename);
            
            return Path.Combine(this.AttachmentsFolderPath, pureFilename);

        }

        private void createFolders()
        {
            if (!Directory.Exists(this.AttachmentsFolderPath)) Directory.CreateDirectory(this.AttachmentsFolderPath);
            foldersCreated = true;
        }

    }
}
