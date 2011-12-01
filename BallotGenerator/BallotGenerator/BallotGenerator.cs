using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Xml.Serialization;

namespace BallotGenerator
{
    /// <summary>
    /// Ballot Generator<br></br>
    /// This application provides secure ballots by an unique identifier.
    /// </summary>
    class BallotGenerator
    {

        private static Configuration _configuration = null;
        private static List<string> _listOfRandomStrings = new List<string>();
        // usepackage[utf8]{inputenc} only works with utf8 without byte order mark
        private static Encoding utf8EmitBOM = new UTF8Encoding(false);




        private static void CheckForDuplicates()
        {
            if (_listOfRandomStrings.Count == _listOfRandomStrings.Distinct().Count())
            {
                Console.WriteLine("No duplicate values");
            }
            else
            {
                Console.WriteLine("Duplicate entries foud, exiting ...");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            foreach (var item in _listOfRandomStrings)
            {
                if (item.Length < 20)
                {
                    Console.WriteLine("Weak entry with <20 numbers foud, exiting ...");
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
        }

        /// <summary>
        /// Generates Latex files.
        /// A single page is a latex command with the path to the barcode as command
        /// </summary>
        private static void GenerateLaTeXFiles()
        {
            string template = File.ReadAllText("BallotTemplate.tex");

            int counter = 0;
            int innerCounter = 0;
            StringBuilder sb = new StringBuilder(template);
            foreach (string barcodeName in _listOfRandomStrings)
            {
                if (counter == _configuration.packageSize)
                {
                    sb.AppendLine("\\end{document}");

                    File.WriteAllText(@"BallotTexFile" + innerCounter++ + ".tex ", sb.ToString(), utf8EmitBOM);

                    // Reset
                    counter = 0;
                    sb.Clear();
                    sb.Append(template);
                }
                sb.AppendLine("\\ballot{Barcodes/" + barcodeName + ".png}");
                counter++;
            }
            if (counter != 0)
            {
                sb.AppendLine("\\end{document}");

                File.WriteAllText(@"BallotTexFile" + innerCounter++ + ".tex ", sb.ToString(), utf8EmitBOM);
            }
        }

        /// <summary>
        /// Generate PDF files with the pdfLatex compiler by a given list of te files
        /// </summary>
        /// <param name="texFiles"></param>
        private static void GeneratePdfFiles(string[] texFiles)
        {
            string file;
            int pdfCounter = 0;
            foreach (string texFile in texFiles)
            {
                file = Path.GetFileName(texFile);
                if (!file.Equals("BallotTemplate.tex"))
                {
                    Console.WriteLine("PDF file: " + pdfCounter++);

                    Process p1 = new Process();
                    p1.StartInfo.FileName = _configuration.pdfLatex;
                    p1.StartInfo.Arguments = file;
                    p1.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    p1.StartInfo.RedirectStandardOutput = true;
                    p1.StartInfo.UseShellExecute = false;

                    p1.Start();
                    var output = p1.StandardOutput.ReadToEnd();
                    p1.WaitForExit();
                }
            }
        }

        static void Main(string[] args)
        {
            // Obtain configuration
            XmlSerializer serialiser = new XmlSerializer(typeof(Configuration));
            _configuration = (Configuration)serialiser.Deserialize(File.OpenRead("Configuration.xml"));

            Console.WriteLine("Reading lines of random data");
            ReadFileWithRandomData();

            Console.WriteLine("Checking for duplicate or weak items");
            CheckForDuplicates();

            Console.WriteLine(_listOfRandomStrings.Count + "unique random values");

            Console.WriteLine("Generating barcodes");

            // Image.Save throws an exception if the path dosn't exist
            System.IO.Directory.CreateDirectory("Barcodes");
            foreach (string code in _listOfRandomStrings)
            {
                System.Drawing.Image myimg = Code128Rendering.MakeBarcodeImage(code, 1, true);
                myimg.Save("Barcodes/" + code + ".png", ImageFormat.Png);
            }
            Console.WriteLine("Barcodes generated");

            Console.WriteLine("Generating LaTeX files");
            GenerateLaTeXFiles();

            string[] texFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.tex");
            Console.WriteLine("Generating " + (texFiles.Length-1) + " pdf files");

            GeneratePdfFiles(texFiles);



            Console.WriteLine("Success! :-)");
            Console.ReadLine();
        }

        /// <summary>
        /// Reads a file with data and adds them to a list
        /// </summary>
        private static void ReadFileWithRandomData()
        {
            string line;
            StreamReader myFile = new StreamReader("random.txt", System.Text.Encoding.ASCII);
            while ((line = myFile.ReadLine()) != null)
            {
                _listOfRandomStrings.Add(line);
            }
            myFile.Close();
        }
    }
}
