using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FromExcelToListClass.Models;

namespace FromExcelToListClass
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<MiBenchmarckAnashei>();
            //Console.WriteLine(summary);
            MiBenchmarckAnashei.MiFuncion();
        }
    }

    public class MiBenchmarckAnashei
    {
        [Benchmark]
        public static void MiFuncion()
        {
            var path = Path.GetFullPath("C:\\Users\\Santi\\source\\repos\\GenericImportExcelToClass\\FromExcelToListClass\\excels\\");
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                var resultado = GenericImportExcel.ParseExcelToClass<Persona>(file);

                resultado.ForEach(p =>
                {
                    Console.WriteLine(p.ToString() + "\n");
                });

            }
        }
    }
}
