using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMcdf;

namespace SrfInfoInner
{
    public static class GetSrfInfo
    {
        /// <summary>
        /// return array of two SrfInfo - [0] info about full compile, [1] - about current compile. It is possible that Current == full, it means [1].IsFullCompile = true, otherwise = false;
        /// </summary>
        /// <param name="SrfFile"></param>
        /// <returns></returns>
        public static SrfMetaData[] GetInfo(string SrfFile)
        {
            SrfMetaData fullCompile;
            SrfMetaData currentCompile;
            using (OpenMcdf.CompoundFile cf = new CompoundFile(SrfFile, CFSUpdateMode.ReadOnly, CFSConfiguration.Default))
            {
                CFStream stream = null;
                stream = cf.RootStorage.TryGetStream("Full Compile");
                fullCompile = ReadBuffer(stream.GetData());
                fullCompile.Srf = new FileInfo(SrfFile);
                fullCompile.IsFullCompile = true;

                stream = cf.RootStorage.TryGetStream("Last Incr. Compile");
                if (stream == null)
                    currentCompile = fullCompile;
                else
                {
                    currentCompile = ReadBuffer(stream.GetData());
                    currentCompile.Srf = new FileInfo(SrfFile);
                    currentCompile.IsFullCompile = false;
                }
            }
            return new SrfMetaData[2] { fullCompile, currentCompile };
        }
        private static SrfMetaData ReadBuffer(byte[] buffer)
        {
            /*string strUDT = Convert.ToString(buffer[7], 16).PadLeft(2, '0') + 
                                  Convert.ToString(buffer[6], 16).PadLeft(2, '0') + 
                                  Convert.ToString(buffer[5], 16).PadLeft(2, '0') + 
                                  Convert.ToString(buffer[4], 16).PadLeft(2, '0');
                unixDt = Convert.ToInt64(strUDT, 16);*/

            //Same as:
            /*unixDt = buffer[4] + 
                     buffer[5] * 16 * 16 + 
                     buffer[6] * 16 * 16 * 16 * 16 + 
                     buffer[7] * 16 * 16 * 16 * 16 * 16 * 16;*/
            //same as:
            double unixDt = buffer[4] + (buffer[5] << 8) + (buffer[6] << 16) + (buffer[7] << 24);
            DateTime winDt = UnixTimeStampToDateTime(unixDt);
            
            string mashineName = "";
            for (int i = 8; i < 0x87 && buffer[i] != 0; i++) mashineName += (char)buffer[i];

            string language = "";
            for (int i = 0x88; i < 0x91 && buffer[i] != 0; i++) language += (char)buffer[i];

            string login = "";
            int bsFolderSuffix = 0;
            for (int i = 0x8c; i < 0xAB && buffer[i] != 0; i++)
            {
                login += (char)buffer[i];
                bsFolderSuffix += buffer[i];
            }
            SrfMetaData si = new SrfMetaData("",false,winDt,login,mashineName,language,$"srf{unixDt}_{bsFolderSuffix}");
            return si;
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }
    }

    public struct SrfMetaData
    {
        
        public SrfMetaData(string Srf, bool IsFullCompile, DateTime CompilationDate, string CompiledBy, string MashineName, string Language, string BsFolder)
        {
            this.IsFullCompile = IsFullCompile;
            this.CompilationDate = CompilationDate;
            this.CompiledBy = CompiledBy;
            this.MashineName = MashineName;
            this.Language = Language;
            this.BsFolder = BsFolder;
            this.Srf = String.IsNullOrEmpty(Srf) ?  null : new FileInfo(Srf);
        }
        public bool IsFullCompile { get; internal set; }
        public DateTime CompilationDate { get; private set; }
        public string CompiledBy { get; private set; }
        public string MashineName { get; private set; }
        public string Language { get; private set; }
        public string BsFolder { get; private set; }
        public FileInfo Srf { get; internal set; }

        public override string ToString()
        {
            return
                $"IsFullCompile\t{IsFullCompile}\n" +
                $"CompilationDate\t{CompilationDate}\n" +
                $"CompiledBy\t{CompiledBy}\n" +
                $"MashineName\t{MashineName}\n" +
                $"Language\t{Language}\n" +
                $"BsFolder\t{BsFolder}";
        }

    }
}

