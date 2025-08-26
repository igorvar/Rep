using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using SrfInfoInner;
using System.Xml.Linq;
using RedBlackTree;
using System.Security.Cryptography;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using OpenMcdf;
namespace SrfInfo
{
    internal class Program
    {

        static void Main(string[] args)
        {
            SrfMetaData[] res;// = null;
            string fileName = String.Empty;//= @"C:\oracle\siebel\Client\objects\enu\siebel_sia.srf";
            if (args.Length == 0)
            {
                Console.WriteLine("Return metadata of Siebel srf file.");
                Console.WriteLine("Parameters:");
                Console.WriteLine("\t srf file");
                Console.WriteLine("\t /fn: n number 0...15 - color of text. (https://learn.microsoft.com/dotnet/api/system.consolecolor)");
                Console.WriteLine("\t /bn: n number 0...15 - color of background");
                Console.WriteLine("Example: C:\\siebel\\client\\objects\\enu\\siebel_sia.srf /f2 /b15");
                Console.Write("Srf: ");
                fileName = Console.ReadLine();
            }
            else
            {
                int clr;
                ConsoleColor fColor = Console.ForegroundColor;
                ConsoleColor bColor = Console.BackgroundColor;
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("/f") || args[i].StartsWith("/F"))

                        if (int.TryParse(args[i].Substring(2), out clr))
                            try { fColor = (ConsoleColor)Enum.GetValues(typeof(ConsoleColor)).GetValue(clr); }
                            catch (System.IndexOutOfRangeException) { Console.WriteLine($"Value of {args[i].Substring(0, 2)} expected 0...15"); }
                        else
                            Console.WriteLine($"Invalid parameter {args[i]}. Expected /f and number 0...16 (/f9), instead of {args[i]}");
                    else if (args[i].StartsWith("/b") || args[i].StartsWith("/B"))
                        if (int.TryParse(args[i].Substring(2), out clr))
                            try { bColor = (ConsoleColor)Enum.GetValues(typeof(ConsoleColor)).GetValue(clr); }
                            catch (System.IndexOutOfRangeException) { Console.WriteLine($"Value of {args[i].Substring(0, 2)} expected 0...15"); }
                        //Console.BackgroundColor = (ConsoleColor)Enum.GetValues(typeof(ConsoleColor)).GetValue(clr);
                        //                    Console.Clear();
                        else
                            Console.WriteLine($"Invalid parameter {args[i]}. Expected /b and number 0...15 (/b10), instead of {args[i]}");
                    else
                        fileName = args[i];
                }
                if (fColor == bColor)
                    Console.WriteLine($"The font and background colors are the same. Not changed ({(int)bColor})");
                else
                {
                    Console.ForegroundColor = fColor;
                    if (Console.BackgroundColor != bColor)
                    {
                        Console.BackgroundColor = bColor;
                        Console.Clear();
                    }
                }
                Console.WriteLine(fileName);
            }
            //}
            if (!File.Exists(fileName))
                throw new FileNotFoundException($"Not found file '{fileName}'");

            res = GetSrfInfo.GetInfo(fileName);
            Console.WriteLine(res[1].ToString());
            if (!res[1].IsFullCompile)
            {
                Console.WriteLine("\n----------- First compile: -----------");
                Console.WriteLine(res[0]);
            }
            res[0].IsFullCompile = true;
            Console.ReadLine();
        }
        //static void Main1(string[] args)
        //{
        //    bool isFullCompile = false;
        //    byte[] buffer = null;
        //    string login = "";
        //    int bsFolderSuffix = 0;
        //    string bsFolderName;
        //    string mashineName = "";
        //    string language = "";
        //    double unixDt;
        //    DateTime winDt;
        //    string fileName;//= @"C:\Users\Igor_var\Desktop\1973.srf";
        //    try
        //    {

        //        if (args.Length == 0)
        //        {
        //            Console.Write("Srf: ");
        //            fileName = Console.ReadLine();
        //        }
        //        else
        //        {
        //            fileName = args[0];
        //            Console.WriteLine(fileName);
        //        }
        //        if (!File.Exists(fileName))
        //            throw new FileNotFoundException($"Not found file '{fileName}'");


        //        using (OpenMcdf.CompoundFile cf = new CompoundFile(fileName, CFSUpdateMode.ReadOnly, CFSConfiguration.Default))
        //        {
        //            CFStream stream = null;
        //            stream = cf.RootStorage.TryGetStream("Last Incr. Compile");
        //            if (stream == null)
        //            {
        //                isFullCompile = true;
        //                stream = cf.RootStorage.TryGetStream("Full Compile");
        //            }
        //            buffer = stream.GetData();
        //        }

        //        /*string strUDT = Convert.ToString(buffer[7], 16).PadLeft(2, '0') + 
        //                          Convert.ToString(buffer[6], 16).PadLeft(2, '0') + 
        //                          Convert.ToString(buffer[5], 16).PadLeft(2, '0') + 
        //                          Convert.ToString(buffer[4], 16).PadLeft(2, '0');
        //        unixDt = Convert.ToInt64(strUDT, 16);*/

        //        //Same as:
        //        /*unixDt = buffer[4] + 
        //                 buffer[5] * 16 * 16 + 
        //                 buffer[6] * 16 * 16 * 16 * 16 + 
        //                 buffer[7] * 16 * 16 * 16 * 16 * 16 * 16;*/
        //        //same as:
        //        unixDt = buffer[4] + (buffer[5] << 8) + (buffer[6] << 16) + (buffer[7] << 24);
        //        winDt = UnixTimeStampToDateTime(unixDt);

        //        for (int i = 8; i < 0x87 && buffer[i] != 0; i++) mashineName += (char)buffer[i];

        //        for (int i = 0x88; i < 0x91 && buffer[i] != 0; i++) language += (char)buffer[i];

        //        for (int i = 0x8c; i < 0xAB && buffer[i] != 0; i++)
        //        {
        //            login += (char)buffer[i];
        //            bsFolderSuffix += buffer[i];
        //        }
        //        bsFolderName = $"srf{unixDt}_{bsFolderSuffix}";
        //        Console.WriteLine($"IsFullCompile\t{isFullCompile}");
        //        Console.WriteLine($"CompilationDate\t{winDt}");
        //        Console.WriteLine($"CompiledBy\t{login}");
        //        Console.WriteLine($"MashineName\t{mashineName}");
        //        Console.WriteLine($"Language\t{language}");
        //        Console.WriteLine($"BsFolder\t{bsFolderName}");
        //    }
        //    catch (Exception ex)
        //    {

        //        Console.WriteLine(ex.Message);
        //    }

        //    Console.ReadLine();
        //}

        //public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        //{
        //    // Unix timestamp is seconds past epoch
        //    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        //    return epoch.AddSeconds(unixTimeStamp).ToLocalTime();
        //}

    }
}
