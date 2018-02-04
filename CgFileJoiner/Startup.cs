using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CgFileJoiner
{
    public class Startup
    {
        private static void Main()
        {
            string outputPath = Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\result.txt";

            Regex usingReg = new Regex(@"using\s[A-Z][A-Za-z]+(\.[A-Z][A-Za-z]+)*;");
            Regex namespaceReg = new Regex(@"namespace\s+[A-Za-z.]+\s*{((\s*.*)+)}");

            Console.Write("Provide full path of project folder: ");
            //C:\Users\psycho realm\Documents\visual studio 2017\Projects\SomeSolution\SomeProject

            string path = Console.ReadLine();
            Console.Clear();

            var files = Directory.GetFiles
                (path, "*.cs", SearchOption.AllDirectories)
                .Where(a => !a.Contains(@"obj\Debug\") && !a.Contains(@"\Properties\"));

            var text = files.Select(a => File.ReadAllText(a).Trim()).ToList();

            string usings = string.Join("\n", usingReg.Matches(string.Join("\n", text))
                .OfType<Match>()
                .Select(f => f.Groups[0].Value)
                .Distinct()
                .OrderByDescending(v => v)) + "\n\n";

            string result = usings + string.Join("\n\n", text.Select(v => usingReg.Replace(v, "").Trim()).Select(f => namespaceReg.Replace(f, m => m.Groups[1].Value.ToString().Trim())).ToList());

            Console.WriteLine(result);
            Console.ReadKey();

            // File.WriteAllText(outputPath, result);

        }
    }
}
