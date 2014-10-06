using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RAFlibPlus;

namespace RAFViewer
{
    class Program
    {
        static void PrintHelp()
        {
            Console.WriteLine("RAF Extractor: Usage Instructions");
            Console.WriteLine("./RAFExtractor PATH_TO_FILEARCHIVES SEARCH_REGEX DISABLE_OUTPUT");
            Console.WriteLine("DISABLE_OUTPUT: 0/1");
        }

        static void Main(string[] args)
        {
            if (args.Length < 3) {
                PrintHelp();
                return;
            }
            string leaguePath = args[0];
            Regex regex = new Regex(args[1]);
            bool disableOutput = (args[2] == "1");

            // Go through all folders and make a list of all RAF files.
            List<RAFArchive> RAFFileNames = new List<RAFArchive>();
            string[] archiveDirs = Directory.GetDirectories(leaguePath);
            foreach (string subDir in archiveDirs) {
                string path = subDir + "\\";
                
                // We are in some 0.0.0.XX or whatever folder...there should be a RAF file in here
                string[] fileNames = Directory.GetFiles(path);
                foreach (string fileName in fileNames) {
                    if (fileName.EndsWith("raf"))
                    {
                        RAFArchive archv = new RAFArchive(fileName);
                        Console.WriteLine(fileName);
                        RAFFileNames.Add(archv);

                        Dictionary<string, RAFFileListEntry> entries = archv.FileDictFull;
                        foreach (KeyValuePair<string, RAFFileListEntry> entry in entries)
                        {
                            Match match = regex.Match(entry.Key);
                            if (match.Success)
                            {
                                Console.WriteLine(entry.Key);

                                if (disableOutput)
                                    continue;
                                // Export the contents that we found out to an appropriately named file.
                                byte[] byteData = entry.Value.GetContent();
                                // Dump out.
                                string outputFile = entry.Value.FileName;

                                // If directory doesn't exist make it.
                                string dirName = Path.GetDirectoryName(outputFile);
                                if (!Directory.Exists(dirName))
                                {
                                    Directory.CreateDirectory(dirName);
                                }

                                int suffix = 0;
                                while (File.Exists(outputFile))
                                {
                                    outputFile += suffix;
                                    ++suffix;
                                }
                                FileStream fs = new System.IO.FileStream(outputFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                                fs.Write(byteData, 0, byteData.Length);
                                fs.Close();
                            }
                        }
                    }
                }
            }


            int exit = Console.Read();
            return;
        }
    }
}
