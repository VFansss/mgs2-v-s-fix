using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace mgs2_v_s_fix
{
    class Unzip
    {
        public static string ApplicationPath;

        // Homemade Unzipper

        // Target Required .net > 4.5

        public static void UnZippa(string fileToUnzip, bool OverWrite = false)
        {
            // NB: Watch out for 'overwrite' optional parameter
            //  only if true it would overwrite a file,if already exist
            //   not all call to 'Unzippa' method has this optional parameter
            //    Why? mainly to performance purpouse. Only sometimes you need really to overwrite!

            // Get a stream by internal resource name
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "mgs2_v_s_fix.Resources." + fileToUnzip;
            Stream stream = assembly.GetManifestResourceStream(resourceName);

            ZipArchive archive = new ZipArchive(stream);

            // foreach entry in my .zip file
            // NB: an entry can be also a folder so someone must check for that case
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                // Set working directory
                String path = ApplicationPath + "\\" + entry.FullName;

                // entry (file or directory) already exist?
                if (!File.Exists(path) || OverWrite == true)
                {
                    if (!(entry.FullName.ToString().EndsWith("/")))
                    {
                        //it's a file
                        entry.ExtractToFile(path, OverWrite);
                    }

                    else
                    {
                        //it's a folder

                        //must I have to create it?

                        if (!(Directory.Exists(entry.FullName.ToString())))
                        {
                            Directory.CreateDirectory(ApplicationPath + "\\" + entry.FullName.ToString());
                        }
                    }
                }
            }
        }

        //END CLASS
    }
}