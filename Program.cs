// This is a quick way to generate all the project content/external links for a WAP project

using System;
using System.IO;

namespace GenerateWAP
{
    class Program
    {
        static void GetFiles(StreamWriter sw, Uri parent, DirectoryInfo folder)
        {
            foreach (FileInfo fi in folder.GetFiles())
            {
                Uri uri = new Uri(fi.FullName);
                String path = parent.MakeRelative(uri).ToString().Replace("/", "\\").Replace("%20", " ");
                sw.WriteLine(@"<Content Include=""{0}"">", fi.FullName);
                sw.WriteLine("\t<Link>{0}</Link>", path);
                sw.WriteLine("\t<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>");
                sw.WriteLine(@"</Content>");
            }
            foreach (DirectoryInfo di in folder.GetDirectories())
            {
                if (di.FullName != folder.FullName)
                {
                    GetFiles(sw, parent, di);
                }
            }
        }

        static void GetDirectories(StreamWriter sw, Uri parent, DirectoryInfo folder)
        {
            foreach (DirectoryInfo di in folder.GetDirectories())
            {
                if (di.FullName != folder.FullName)
                {
                    Uri uri = new Uri(di.FullName);
                    String path = parent.MakeRelative(uri).ToString().Replace("/", "\\").Replace("%20", " ");
                    sw.WriteLine(@"<Folder Include=""{0}\"" />", path);
                    GetDirectories(sw, parent, di);
                }
            }
        }

        static void Build(StreamWriter sw, string path)
        {
            Uri uri = new Uri(path);
            sw.WriteLine("<!-- Generated Content -->");
            GetFiles(sw, uri, new DirectoryInfo(path));
            sw.WriteLine("<!-- Generated Folders -->");
            GetDirectories(sw, uri, new DirectoryInfo(path));
        }

        static void Main(string[] args)
        {
            using (FileStream fs = File.Open("Output.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Build(sw, @"C:\Program Files (x86)\Razer\Synapse3\");
                    sw.Flush();
                }
            }
        }
    }
}
