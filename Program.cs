using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;

class Program
{
    static void Main()
    {
        string inputFile = "MONOLA01.pro";
        string outputFile = "MONOLA01_düzeltilmiş.pro";

        try
        {
            // Dosya içeriğini oku
            string content = File.ReadAllText(inputFile);

            // XML içeriğini bul ve işle
            var piecesListMatch = Regex.Match(content, @"<PIECESLIST>(.*?)</PIECESLIST>", RegexOptions.Singleline);
            if (piecesListMatch.Success)
            {
                string piecesListXml = "<PIECESLIST>" + piecesListMatch.Groups[1].Value + "</PIECESLIST>";
                var xmlDoc = XDocument.Parse(piecesListXml);

                // <PIECE Code="X"> olan tüm kodları al
                var pieces = xmlDoc.Descendants("PIECE")
                    .Select(p => p.Attribute("Code")?.Value)
                    .Where(code => !string.IsNullOrEmpty(code))
                    .ToList();

                // Her PIECE Code değerini dosya içinde ilgili yerlere yaz
                for (int i = 0; i < pieces.Count; i++)
                {
                    string pattern = $@"(<PIECE Code="".*?"")";
                    content = Regex.Replace(content, pattern, $@"$1 Value=""{pieces[i]}""", RegexOptions.Multiline);
                }
            }

            // Yeni dosyayı oluştur
            File.WriteAllText(outputFile, content);
            Console.WriteLine($"Düzenlenmiş dosya oluşturuldu: {outputFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata: {ex.Message}");
        }
    }
}
