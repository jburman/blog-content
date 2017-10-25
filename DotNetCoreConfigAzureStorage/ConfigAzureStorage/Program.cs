using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace ConfigAzureStorage
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json");
                var config = builder.Build();

                string connStr = config.GetConnectionString("azureStorageConnStr");

                var task = DoBlobStuff(connStr);
                task.GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("Done. Press <Enter> to exit");
            Console.ReadLine();
        }

        public static async Task DoBlobStuff(string connStr)
        {

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connStr);
            CloudBlobClient blobs = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobs.GetContainerReference("mycontainer");
            await container.CreateIfNotExistsAsync();

            CloudBlockBlob randomBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString("N"));
            await randomBlob.UploadTextAsync("demo blob");
        }
    }
}