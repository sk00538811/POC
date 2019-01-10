using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileZipUnzip
{
    class Program
    {
        static void Main(string[] args)
        {
            string loop = "n";

            do
            {

                Console.Clear();
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("1 - Zip files");
                Console.WriteLine("2 - Upzip file");
                Console.WriteLine("3 - split zip file");
                Console.WriteLine("4 - Compress text");
                Console.WriteLine("5 - Decompress text");
                Console.WriteLine("6 -downlod file and output file");
                Console.WriteLine("0 - Exist");
                Console.WriteLine("-----------------------------------");
                Console.Write("Enter your choice: ");
                ConsoleKeyInfo input = Console.ReadKey();
                Console.WriteLine("");
                string filesfolder = @"C:\DocumentSecurity\Logs\test";
                string zipfilepath = string.Empty;
                switch (input.KeyChar.ToString())
                {
                    case "1":
                        {
                            Console.Write("\nEnter folder path to zip all file into one: ");
                            filesfolder = Console.ReadLine();
                            Console.WriteLine(string.Format("\nZip File Generate Start time:{0}, files folder", DateTime.Now.ToString("o"), filesfolder));
                            zipfilepath = FnZipFiles(filesfolder);
                            Console.WriteLine(string.Format("Zip File Generate End time:{0} , file path:{1}", DateTime.Now.ToString("o"), zipfilepath));
                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;

                    case "2":
                        {
                            Console.Write("\nEnter zip file path: ");
                            zipfilepath = Console.ReadLine();
                            Console.WriteLine(string.Format("\nFile Unzipping Start time:{0}, files folder", DateTime.Now.ToString("o"), zipfilepath));

                            filesfolder = FnUnZipFiles(zipfilepath);
                            Console.WriteLine(string.Format("File Unzipping End time:{0} , unzip files folder:{1}", DateTime.Now.ToString("o"), filesfolder));

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;

                    case "3":
                        {
                            Console.Write("\nEnter zip file path that you want to split: ");
                            zipfilepath = Console.ReadLine();
                            Console.WriteLine(string.Format("\nZip file spliting Start time:{0}, files folder", DateTime.Now.ToString("o"), zipfilepath));

                            filesfolder = FnSplitZipFiles(zipfilepath);
                            Console.WriteLine(string.Format("Zip file spliting  End time:{0} , unzip files folder:{1}", DateTime.Now.ToString("o"), filesfolder));

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "4":
                        {
                            Console.Write("\nEnter file path that text you want to compress: ");
                            zipfilepath = Console.ReadLine();

                            FileInfo fi = new FileInfo(zipfilepath);
                            if (fi.Exists)
                            {
                                Console.WriteLine(string.Format("\nFile Reading Start time:{0}, files path:{1}", DateTime.Now.ToString("o"), zipfilepath));
                                string outputtext = TextCompressDecompress.Compress(File.ReadAllText(zipfilepath, Encoding.ASCII));
                                string outputFilePath = Path.Combine(fi.DirectoryName, Guid.NewGuid() + ".txt");
                                File.WriteAllText(outputFilePath, outputtext);
                                Console.WriteLine(string.Format("\nOutput File with Compress text,End time:{0} , files path:{1}", DateTime.Now.ToString("o"), outputFilePath));

                            }
                            else { Console.WriteLine("\nFile not exist or invalid file path."); }

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "5":
                        {
                            Console.Write("\nEnter file path that text you want to decompress: ");
                            zipfilepath = Console.ReadLine();

                            FileInfo fi = new FileInfo(zipfilepath);
                            if (fi.Exists)
                            {
                                Console.WriteLine(string.Format("\nFile Reading Start time:{0}, files path:{1}", DateTime.Now.ToString("o"), zipfilepath));
                                string outputtext = TextCompressDecompress.Decompress(File.ReadAllText(zipfilepath, Encoding.ASCII));
                                string outputFilePath = Path.Combine(fi.DirectoryName, Guid.NewGuid() + ".txt");
                                File.WriteAllText(outputFilePath, outputtext);
                                Console.WriteLine(string.Format("\nOutput File with Compress text,End time:{0} , files path:{1}", DateTime.Now.ToString("o"), outputFilePath));

                            }
                            else { Console.WriteLine("\nFile not exist or invalid file path."); }

                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "6":
                        {
                            string userid = "1111";
                            string inputfileurl = @"https://www.tineye.com//images/imageurl-ie.jpg";
                            string tempFile = DownloadFileFromServer(inputfileurl, userid);
                            FileInfo fi = new FileInfo(tempFile);

                            if (!string.IsNullOrEmpty(tempFile) && fi.Exists)
                            {
                                string outputFileName = GetOutputFilePath(tempFile, userid);
                                if (new FileInfo(outputFileName).Exists)
                                {
                                    Console.WriteLine("output file: " + outputFileName);
                                }
                                else { Console.WriteLine("output file not exist"); }
                            }
                            else { Console.WriteLine("Invalid temp file"); }
                            Console.WriteLine("Press Enter key to return main menu...");
                            Console.ReadLine();
                        }
                        break;
                    case "0":
                        Console.Write("\nAre you sure you want to exit.(y/n)?");
                        input = Console.ReadKey();
                        loop = input.KeyChar.ToString();
                        break;
                    default:
                        return;

                }
            } while (loop != "y");
        }
        /// <summary>
        /// Description: this method zip all file and return single zip file path
        /// </summary>
        /// <param name="dirpath"></param>
        /// <returns></returns>
        static string FnZipFiles(string dirpath)
        {
            string zipfilepath = string.Empty;
            DirectoryInfo di = new DirectoryInfo(dirpath);
            if (!di.Exists) throw new Exception($"Dir path'{dirpath}' not exist.");
            //zip all file of di
            using (ZipFile zip = new ZipFile())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Zip file contain only below list of file, Please check all files are present in the folder");
                sb.Append(string.Format("{0}{1}", Environment.NewLine, string.Concat(Enumerable.Repeat("-", 79))));
                sb.Append(string.Format("{0}Sno#\tFileName\tFile Size", Environment.NewLine));

                zip.UseZip64WhenSaving = Zip64Option.AsNecessary;
                //zip.CompressionMethod = CompressionMethod.BZip2;
                //zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                int cnt = 1;
                foreach (FileInfo fi in di.GetFiles("*.*", SearchOption.AllDirectories))
                {
                    // add   file  in the zip archive
                    zip.AddFile(fi.FullName, RemoveInvalidCharaterFromFolderName(fi.Directory.Name));
                    sb.Append(string.Format("{0}{1}\t{2}\t{3} byte", Environment.NewLine, cnt, fi.Name, fi.Length));
                    cnt++;
                }
                ZipEntry e = zip.AddEntry("log.txt", sb.ToString());
                e.Comment = "This entry in the zip archive was created to verify all file with its size";
                zipfilepath = Path.Combine(di.FullName, Generate.AlphabateNumber(10, true) + ".zip");
                zip.Save(Path.Combine(di.FullName, zipfilepath));
            }
            return zipfilepath;
        }

        private static string RemoveInvalidCharaterFromFolderName(string str)
        {
            // avoid "\\", "/", ":", "*", "?", "\"", "<", ">", "|"  
            //allow  range a-z, A-Z, 0-9, _ and . only
            return Regex.Replace(str, "[^(a-z)(A-Z)(0-9)_.-]+", "", RegexOptions.Compiled);
        }

        /// <summary>
        /// Description: this method use given zip file path and unzip all file and return unzip file folder path
        /// </summary>
        /// <returns></returns>
        static string FnUnZipFiles(string zipfilepath)
        {
            string unzipfilepath = string.Empty;
            byte[] buffer = new byte[2048];
            int n;
            FileInfo fi = new FileInfo(zipfilepath);
            if (fi.Exists)
            {
                unzipfilepath = Path.Combine(fi.DirectoryName, Generate.AlphabateNumber(10, true));
                DirectoryInfo di = new DirectoryInfo(unzipfilepath);
                if (!di.Exists) di.Create();

                using (var raw = File.Open(zipfilepath, FileMode.Open, FileAccess.Read))
                {
                    using (var input = new ZipInputStream(raw))
                    {
                        ZipEntry e;
                        while ((e = input.GetNextEntry()) != null)
                        {
                            if (e.IsDirectory) continue;
                            string outputPath = Path.Combine(di.FullName, e.FileName.Replace("/", "\\"));
                            DirectoryInfo di_ = new DirectoryInfo(outputPath.Substring(0, outputPath.LastIndexOf("\\")));
                            if (!di_.Exists) di_.Create();
                            using (var output = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite))
                            {
                                while ((n = input.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    output.Write(buffer, 0, n);
                                }
                            }
                        }
                    }
                }
            }
            else { throw new Exception($"Zip file path'{zipfilepath }' not exist."); }

            return unzipfilepath;

        }
        /// <summary>
        /// Description: this method use given zip file path and unzip all file and return unzip file folder path
        /// </summary>
        /// <returns></returns>
        static string FnSplitZipFiles(string zipfilepath)
        {
            string unzipfilepath = string.Empty;
            byte[] buffer = new byte[2048];
            int n;
            FileInfo fi = new FileInfo(zipfilepath);
            if (fi.Exists)
            {
                unzipfilepath = Path.Combine(fi.DirectoryName, Generate.AlphabateNumber(10, true));
                DirectoryInfo di = new DirectoryInfo(unzipfilepath);
                if (!di.Exists) di.Create();

                int SegmentsCreated;
                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncoding = Encoding.UTF8;  // utf-8
                    zip.AddFile(fi.FullName);
                    zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                    string filesize = GetSizeString(fi.Length);
                    int size = 1024;
                    if (filesize.Contains("MB"))
                    {
                        size = 100 * 1024 * 1024;

                        zip.MaxOutputSegmentSize = size; // 100 * 1024=100k segments
                    }
                    zip.Save(Path.Combine(di.FullName, fi.Name));

                    SegmentsCreated = zip.NumberOfSegmentsForMostRecentSave;
                }
            }
            else { throw new Exception($"Zip file path'{zipfilepath }' not exist."); }

            return unzipfilepath;

        }
        static string GetSizeString(long length)
        {
            long B = 0, KB = 1024, MB = KB * 1024, GB = MB * 1024, TB = GB * 1024;
            double size = length;
            string suffix = nameof(B);

            if (length >= TB)
            {
                size = Math.Round((double)length / TB * 1024 * 1024, 2);
                suffix = nameof(MB);
            }
            else if (length >= GB)
            {
                size = Math.Round((double)length / GB * 1024, 2);
                suffix = nameof(MB);
            }
            else if (length >= MB)
            {
                size = Math.Round((double)length / MB, 2);
                suffix = nameof(MB);
            }
            else if (length >= KB)
            {
                size = Math.Round((double)length / KB, 2);
                suffix = nameof(KB);
            }

            return $"{size} {suffix}";
        }
        static string GetOutputFilePath(string tempFile, string userId)
        {
            string TempFileoutPath = @"C:\DocumentSecurity\Files\Output\"; //CommonSettings.AppSetting.TempOutputFilePath;
            string[] s3SubDir = tempFile.Split('/');
            string subFolder = TempFileoutPath + userId;

            if (!Directory.Exists(subFolder))
            {
                Directory.CreateDirectory(subFolder);
            }

            for (int ctr = 0; ctr < s3SubDir.Length - 1; ctr++)
            {
                subFolder = subFolder + " / " + s3SubDir[ctr];

                if (!Directory.Exists(subFolder))
                {
                    Directory.CreateDirectory(subFolder);
                }
            }

            string outputFileName = subFolder + "\\" + tempFile.Split('\\').LastOrDefault();
            File.Copy(tempFile, outputFileName, true);
            return outputFileName;
        }
        static string DownloadFileFromServer(string url, string userId)
        {
            string TempFilePath = @"C:\DocumentSecurity\Files\Input\";// CommonSettings.AppSetting.TempFilePath;
            string fileName = string.Empty;

            try
            {
                fileName = url.Split('/').LastOrDefault();

                using (WebClient wc = new WebClient())
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileName = TempFilePath + userId + "\\" + fileName;
                        if (!Directory.Exists(TempFilePath + userId))
                        {
                            Directory.CreateDirectory(TempFilePath + userId);
                        }

                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);

                        }
                        wc.DownloadFile(url, fileName);
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            catch(Exception ex)
            {
                fileName = string.Empty;
            }

            return fileName;
        }
    }
}
