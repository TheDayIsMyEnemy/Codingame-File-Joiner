using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CgFileJoiner
{
    public class FileJoiner
    {
        private string projectPath = string.Empty;
        private string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\result.txt";

        private Regex usingReg = new Regex(@"using\s[A-Z][A-Za-z]+(\.[A-Z][A-Za-z]+)*;");
        private Regex namespaceReg = new Regex(@"namespace\s+[A-Za-z.]+\s*{((\s*.*)+)}");

        private ConsoleKeyInfo userInput;

        public void Run()
        {
            SetProjectFolder();
            MenuOptions();

            do
            {                
                userInput = Console.ReadKey(true);

                if (userInput.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    string result = MergeFiles();
                    Console.WriteLine(result);
                }
                else if(userInput.Key == ConsoleKey.D1)
                {
                    SetProjectFolder();
                    MenuOptions();
                }

            } while (userInput.Key != ConsoleKey.Escape);
        }

        private string MergeFiles()
        {
            var files = Directory.GetFiles
                (projectPath, "*.cs", SearchOption.AllDirectories)
                .Where(a => !a.Contains(@"obj\Debug\") && !a.Contains(@"\Properties\"));

            var text = files.Select(a => File.ReadAllText(a).Trim()).ToList();

            string usings = string.Join("\n", usingReg.Matches(string.Join("\n", text))
                .OfType<Match>()
                .Select(f => f.Groups[0].Value)
                .Distinct()
                .OrderByDescending(v => v)) + "\n\n";

            string result = usings + string.Join("\n\n", text.Select(v => usingReg.Replace(v, "").Trim()).Select(f => namespaceReg.Replace(f, m => m.Groups[1].Value.ToString().Trim())).ToList());

            // File.WriteAllText(outputPath, result);

            return result;
        }

        private void SetProjectFolder()
        {
            Console.Clear();
            Console.WriteLine("Provide full path of project folder:");
            // C:\Users\psycho realm\Documents\visual studio 2017\Projects\SomeSolution\SomeProject

            projectPath = Console.ReadLine();
        }

        private void MenuOptions()
        {
            Console.Clear();
            Console.WriteLine("Press:");
            Console.WriteLine("Enter = MergeFiles");
            Console.WriteLine("Key 1 = SetProjectFolder");
            Console.WriteLine("Esc = Exit");
        }
    }
}
