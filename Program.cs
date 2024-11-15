using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FromExcelToListClass.Models;

namespace FromExcelToListClass
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MiBenchmarckAnashei>();
            Console.WriteLine(summary);
            //MiBenchmarckAnashei.MiFuncion();
        }
    }

    public class MiBenchmarckAnashei
    {
        [Benchmark]
        public void MiFuncion()
        {
            var path = Path.GetFullPath("C:\\Users\\Santi\\source\\repos\\GenericImportExcelToClass\\FromExcelToListClass\\excels\\");
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                var resultado = new FromExcelToListClass<Persona>().ParseExcelToClass(file);

                //resultado.ForEach(p =>
                //{
                //    Console.WriteLine(p.ToString() + "\n");
                //});

            }
        }

        [Benchmark]
        public void MiFuncion2()
        {
            var path = Path.GetFullPath("C:\\Users\\Santi\\source\\repos\\GenericImportExcelToClass\\FromExcelToListClass\\excels\\");
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                var resultado = new FromExcelToListClass<Persona>().ParseExcelToClass(file);

                //resultado.ForEach(p =>
                //{
                //    Console.WriteLine(p.ToString() + "\n");
                //});

            }
        }
    }
}
