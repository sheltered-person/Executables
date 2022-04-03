using System.Security.Cryptography.X509Certificates;

namespace Executables.Check
{
    /// <summary>
    /// Allows to list files by pattern and sort then by publisher.
    /// </summary>
    public static class Program
    {
        /// <param name="path">Start directory path.</param>
        /// <param name="pattern">Wildcard file name or type pattern.</param>
        /// <param name="recurse">Recurse subdirectories or not.</param>
        public static void Main(
            string path = null,
            string pattern = "*.dll",
            bool recurse = false)
        {
            var directory = path ?? Directory.GetCurrentDirectory();
            var directoryInfo = new DirectoryInfo(directory);

            var options = new EnumerationOptions()
            {
                RecurseSubdirectories = recurse
            };

            var publishersQuery = directoryInfo
                .GetFiles(pattern, options)
                .GroupBy(x => x.GetSigner())
                .OrderByDescending(group => group.Count())
                .Select(group => new Publisher()
                {
                    Name = group.Key,
                    Files = group
                        .Select(file => new File() 
                        { 
                            Name = file.Name, 
                            Length = file.Length
                        })
                });;

            var totalSpace = 0L;

            foreach (var publisher in publishersQuery) 
            {
                Console.WriteLine($"\t{publisher}");
                totalSpace += publisher.Files.Sum(fileInfo => fileInfo.Length);
            }

            Console.WriteLine($"\n\tTotal used space, bytes: {totalSpace}");
        }

        private static string GetSigner(this FileInfo fileInfo)
        {
            var signer = "Unknown";

            try
            {
                var theSigner = X509Certificate.CreateFromSignedFile(fileInfo.FullName);
                var theCertificate = new X509Certificate2(theSigner);

                if (!string.IsNullOrEmpty(theCertificate.Issuer))
                {
                    signer = theCertificate.Issuer;
                }
            }
            catch { }

            return signer;
        }
    }
}