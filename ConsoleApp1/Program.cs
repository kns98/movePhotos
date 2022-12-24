using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace MovePics
{
    
    class Program
    {
        static readonly string folder = @"D:\OneDrive\Pictures\\by_date";

        static void RunPass(List<FileInfo> files, string prefix)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var x = Convert.ToDouble(files.Count);
                var index = i;

                var file = files[(int)index];
                string dirname = Path.GetDirectoryName(file.FullName);
                string dest = Path.Combine(dirname, prefix + index + file.Extension);

                try
                {
                    File.Move(file.FullName, dest,false);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }





            }
        }


        public static void RenameAll(string[] args)
        {
            List<FileInfo> files1 = new List<FileInfo>();
            DirSearch_ex3_Rename(folder, files1);

            RunPass(files1, Guid.NewGuid().ToString());

            List<FileInfo> files2 = new List<FileInfo>();
            DirSearch_ex3_Rename(folder, files2);
            RunPass(files2, "");


        }
        public static void DirSearch_ex3_Rename(string sDir, List<FileInfo> files)
        {
            //Console.WriteLine("DirSearch..(" + sDir + ")");
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {

                    files.Add(new FileInfo(f));

                }

                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch_ex3_Rename(d, files);
                }
            }
            catch (System.Exception excpt)
            {
                //Console.WriteLine(excpt.Message);
            }


        }



        private static Regex r = new Regex(":");
        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime? GetDateTakenFromImage(string path)
        {
            try
            {
                if ((path.ToLower().EndsWith(".jpg")) || (path.ToLower().EndsWith(".jpeg")))
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    using (Image myImage = Image.FromStream(fs, false, false))
                    {
                        PropertyItem propItem;
                        if (myImage.PropertyIdList.Contains(36867))
                        {
                            propItem = myImage.GetPropertyItem(36867);
                            string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                            return DateTime.Parse(dateTaken);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                return null;
            }

            return null;
        }

        public static void DirSearch_ex3(string sDir, List<FileInfo> files)
        {
            //Console.WriteLine("DirSearch..(" + sDir + ")");
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {

                    files.Add(new FileInfo(f));

                }

                foreach (string d in Directory.GetDirectories(sDir))
                {
                    DirSearch_ex3(d, files);
                }
            }
            catch (System.Exception excpt)
            {
                //Console.WriteLine(excpt.Message);
            }


        }


        static void Main(string[] args)

        { 
            RenameAll(args);
            
            DirectoryInfo d = new DirectoryInfo(folder);//Assuming Test is your Folder

            List<FileInfo> files = new List<FileInfo>();

            DirSearch_ex3(folder, files);

            foreach (FileInfo file in files.ToArray())
            {
                var fd = GetDateTakenFromImage(file.FullName);
                if (fd == null)
                    fd = GetTimeFromFileName(file.Name);
                if (fd == null)
                    fd = file.LastWriteTime;
                if (fd != null)
                {
                    string inFolder = Path.Combine(folder, ((DateTime)fd).ToString("yyyy-MM"));
                    Console.WriteLine(inFolder);
                    Directory.CreateDirectory(inFolder);
                    var dest = Path.Combine(inFolder, file.Name);

                    if (!System.IO.File.Exists(dest))
                    {
                        file.MoveTo(dest,false);
                        Console.WriteLine("Moved file:" + dest);
                    }
                    else
                    {
                        //if (FileCompare(file.FullName, dest))
                        //{
                            Console.WriteLine("Skipped file :" + dest);
                        //    file.Delete();
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Warning different file :" + dest);
                        //}
                    }
                }
            }
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }


        private static bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read);
            fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }

        private static DateTime? GetTimeFromFileName(string name)
        {
            try
            {
                DateTime outp = DateTime.ParseExact(name.Substring(0, 10), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                return DateTime.Parse(name.Substring(0, 10));
            } catch
            {
                return null;
            }
        }
    }
}
