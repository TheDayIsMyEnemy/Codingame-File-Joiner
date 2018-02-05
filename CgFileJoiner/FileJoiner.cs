using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CgFileJoiner
{
    public class FileJoiner
    {
        private string projectPath;
        private string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\result.txt";

        private Regex uReg = new Regex(@"using\s[A-Z][A-Za-z]+(\.[A-Z][A-Za-z]+)*;");
        private Regex nReg = new Regex(@"namespace\s+[A-Za-z.]+\s*{((\s*.*)+)}");

        private ConsoleKeyInfo cki;

        public void Run()
        {
            SetPath();
            Options();

            do
            {                
                cki = Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    Console.WriteLine(Merge());
                }
                else if(cki.Key == ConsoleKey.D1 || cki.Key == ConsoleKey.NumPad1)
                {
                    SetPath();
                    Options();
                }

            } while (cki.Key != ConsoleKey.Escape);
        }

        private string Merge()
        {
            var files = Directory.GetFiles
                (projectPath, "*.cs", SearchOption.AllDirectories)
                .Where(a => !a.Contains(@"obj\Debug\") && !a.Contains(@"\Properties\"));

            var text = files.Select(a => File.ReadAllText(a).Trim()).ToList();

            string usings = string.Join("\n", uReg.Matches(string.Join("\n", text))
                .OfType<Match>()
                .Select(f => f.Groups[0].Value)
                .Distinct()
                .OrderByDescending(v => v)) + "\n\n";

            string result = usings + string.Join("\n\n", text.Select(v => uReg.Replace(v, "").Trim())
                .Select(f => nReg.Replace(f, m => m.Groups[1].Value.ToString().Trim())));

            // File.WriteAllText(outputPath, result);

            return result;
        }

        private void SetPath()
        {
            Console.Clear();
            Console.WriteLine("Provide a project path:");
            // C:\Users\psycho realm\Documents\visual studio 2017\Projects\SomeSolution\SomeProject

            projectPath = Console.ReadLine();
        }

        private void Options()
        {
            Console.Clear();
            Console.WriteLine("Press:");
            Console.WriteLine("------ Enter to join files");
            Console.WriteLine("------ Key 1 to set a project path");
            Console.WriteLine("------ Esc to exit");
        }
    }
}
