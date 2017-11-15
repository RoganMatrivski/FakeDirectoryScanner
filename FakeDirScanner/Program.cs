using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace FakeDirScanner
{
    class Program
    {
        public static string folderPath { get; set; }

        static void Main(string[] args)
        {
            randomInit();

            int counter = 0;

            if (args.Length != 0)
            {
                if (argsCheck(args[0]))
                {
                    folderPath = args[0];
                }
            }
            else
            {
                Console.WriteLine("Pls insert dir");
                folderPath = "";
                folderPath = Console.ReadLine();
            }

            int loop = 1;

            Console.WriteLine("Wanna make it loops?");
            int.TryParse(Console.ReadLine(), out loop);

            string testStr = randomStrings();

            if (testStr == null)
            {
                Environment.Exit(0);
            }
            
            List<string> getDirectory = new List<string>();

            /*
            try
            {
                Console.WriteLine("Please wait while we indexing the directories...");
                string[] getDir = Directory.GetFiles(folderPDath, "*", SearchOption.AllDirectories);
                getDirectory = getDir.OfType<string>().ToList();


            }

            catch (Exception ex) when(
                ex is UnauthorizedAccessException
                || ex is IOException
            )
            
            {
                Console.WriteLine("Error occured. Probably denied access to directory.");
                Console.ReadKey();

                Environment.Exit(0);
            }
            
            */

            //foreach (string fileName in SafeFileEnumerator.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
            Parallel.ForEach(SafeFileEnumerator.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories), (fileName, loopState) =>
            {
                getDirectory.Add(fileName);
                //Console.WriteLine("Found {0}", fileName);
                Console.Title = string.Format("Directory Scanner | Status : Found {1} | Avg. Speed : {2} file/sec, {3}/sec | DEBUG {4}, {5}", counter, getDirectory.Count, 0, 0, 0, 0);
                if (getDirectory.Count > int.MaxValue)
                {
                    loopState.Stop();
                }
            });


            string[] fileList = getDirectory.ToArray();

            Console.Clear();

            string strContainer;

            Stopwatch timer = new Stopwatch();
            int currentTime = 0, fileSpeed = 0, fileSpeedCounter = 0, currentTime2 = 0;
            long totalBytes = 0, countBytes = 0;
            timer.Start();



            //while (loop > 1)
            //{
                Parallel.ForEach(fileList, currentFile =>
                    {
                        if ((timer.ElapsedMilliseconds - currentTime) > 500)
                        {
                            fileSpeed = fileSpeedCounter * 2;
                            fileSpeedCounter = 0;
                            //Console.WriteLine("1 sec elapsed");
                            currentTime += 500;
                        }
                        else
                        {
                            fileSpeedCounter++;
                            //Console.WriteLine("filespeedcounter : {0}", fileSpeedCounter);
                        }


                        strContainer = randomStrings();
                        //Console.WriteLine("Thread {0} : Reading {1}", Thread.CurrentThread.ManagedThreadId, currentFile);
                        while (string.IsNullOrWhiteSpace(strContainer))
                        {
                            strContainer = randomStrings();
                        }
                        FileInfo fileinfo = new FileInfo(currentFile);

                        Console.WriteLine(string.Format("Thread {0} : " + strContainer, Thread.CurrentThread.ManagedThreadId, fileinfo.Name));
                        //Thread.Sleep(rand.Next(20, 300));

                        //int timeSize = Convert.ToInt32(fileinfo.Length / 1048576 * 1000); //1048576 = 1024*1024
                        //Thread.Sleep((timeSize));

                        long maxBytes = Convert.ToInt64(fileinfo.Length);
                        long currentBytes = 0;

                        while (currentBytes < maxBytes)
                        {
                            int randomBytes = 1024 * rand.Next(512, 2048);
                            long byteTrim;
                            currentBytes += randomBytes;
                            if (currentBytes > maxBytes)
                            {
                                byteTrim = currentBytes - maxBytes;
                                currentBytes -= byteTrim;
                                countBytes += byteTrim;
                            }
                            else
                            {
                                countBytes += randomBytes;
                            }
                            Thread.Sleep(100);
                        }

                        if ((timer.ElapsedMilliseconds - currentTime2) > 1000)
                        {
                            totalBytes = countBytes;
                            countBytes = 0;
                            //Console.WriteLine("1 sec elapsed");
                            currentTime2 += 1000;
                        }
                        counter++;
                        Console.Title = string.Format("Directory Scanner | Status : {0} out of {1} scanned | Avg. Speed : {2} file/sec, {3}/sec | DEBUG {4}, {5}", counter, fileList.Length, fileSpeed, bytesConverter(totalBytes), totalBytes, fileinfo.Length);
                    }
                );
                counter = 0;
           
            //}

            Console.ReadKey();
        }

        private static void AddFiles(string path, IList<string> files)
        {
            try
            {
                Directory.GetFiles(path)
                    .ToList()
                    .ForEach(s => files.Add(s));

                Directory.GetDirectories(path)
                    .ToList()
                    .ForEach(s => AddFiles(s, files));
            }
            catch (UnauthorizedAccessException ex)
            {
                // ok, so we are not allowed to dig into that directory. Move on.
            }
        }

        public static string bytesConverter(long bytes)
        {
            if (bytes > 1073741824)
            {
                double result = bytes / 1073741824;
                return result.ToString()+" KB";
            }

            if (bytes > 1048576)
            {
                double result = bytes / 1048576;
                return result.ToString()+" MB";
            }

            if (bytes > 1024)
            {
                double result = bytes / 1024;
                return result.ToString() + " GB";
            }

            if (bytes <= 1024)
            {
                double result = bytes;
                return result.ToString() + " B";
            }

            return null;
        }

        public static bool argsCheck(string filepath)
        {
            if (filepath.Length > 0 && Directory.Exists(filepath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Random rand = new Random();


        public static List<string> randomStuff = new List<string>();

        public static void randomInit()
        {

            try
            {
                foreach (string line in File.ReadLines("list.txt", Encoding.UTF8))
                {
                    if (!line.StartsWith(@"//") || String.IsNullOrWhiteSpace(line))
                    {
                        randomStuff.Add(line);
                    }
                }
            }

            catch (IOException)
            {
                Console.WriteLine("Can't find file. Creating a new one and closing...");

                // Create the file.
                using (FileStream fs = File.Create("list.txt"))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes(@"//Please add your line here. Line starts with double slash will not be added.");
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                    Environment.Exit(0);
                }
            }
        }

        public static string randomStrings()
        {
            try
            {
                //Console.WriteLine(rand.Next(0, randomStuff.Count));
                return randomStuff[rand.Next(0, randomStuff.Count)];
            }

            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine("Error! Can't select any sentences. Make sure there is a line to choose in list.txt on the app directory");
                return null;
            }
        }
    }
}
