using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        static string directoryPath = "HAM";
        static string directoryPath2 = "OLMUŞ";
        static void Main()
        {
            string[] proFiles = Directory.GetFiles(directoryPath, "*.pro");

            foreach (var file in proFiles)
            {
                Donustur(file);
            }
        }

        static void Donustur(string inputFile)
        {
            try
            {
                #region OKU
                //string[] lines = File.ReadAllLines(inputFile);
                string[] lines = File.ReadAllLines(inputFile, System.Text.Encoding.GetEncoding("windows-1254"));
                int startLine = -1, endLine = -1;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("<PIECESLIST>"))
                        startLine = i + 1; // Satır numarası (0 tabanlı değil)

                    if (lines[i].Contains("</PIECESLIST>"))
                        endLine = i + 1;
                }

                if (startLine != -1)
                {
                    //Console.WriteLine($"<PIECESLIST> etiketi {startLine}. satırda.");
                }
                else
                {
                    Console.WriteLine("<PIECESLIST> etiketi bulunamadı.");
                    throw new Exception("Format yanlış");
                }
                if (endLine != -1)
                {
                    //Console.WriteLine($"</PIECESLIST> etiketi {endLine}. satırda.");
                }
                else
                {
                    Console.WriteLine("</PIECESLIST> etiketi bulunamadı.");
                    throw new Exception("Format yanlış");
                }
                #endregion
                #region tanımlı verileri yerlerine yaz
                for (int i = startLine; i < endLine - 1; i++)
                {
                    //Console.WriteLine(lines[i]);
                    string[] pieces = lines[i].Split(new string[] {"<PIECE Code=\""},StringSplitOptions.None);
                    string[] pieces2 = pieces[1].Split(new string[] { "\" " },StringSplitOptions.None);
                    var key = pieces2[0];
                    var value = pieces2[1];
                    //Console.WriteLine($"Key: {key}, Value: {value}");

                    for (int j = endLine; j < lines.Count(); j++)
                    {
                        if (lines[j].Contains("\"" + key + "\""))
                        {
                            lines[j] = lines[j].Replace("\"" + key + "\"", value);
                        }
                    }

                }
                #endregion
                #region tanımsız verileri temizler
                for (int j = endLine; j < lines.Count(); j++)
                {
                    if (lines[j].Contains("\""))
                    {
                        lines[j] = lines[j].Split('\"')[0];
                    }
                }
                #endregion
                #region piecelist bölümünü sil

                var newLines = new List<string>();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i < (startLine - 1) || i >= endLine)
                    {
                        newLines.Add(lines[i]);
                    }
                }
                lines = newLines.ToArray();

                #endregion
                File.WriteAllLines(directoryPath2 + "\\" + inputFile.Replace(directoryPath, ""), lines);
                File.Delete(inputFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata: {ex.Message}");
            }
        }
    }
}
