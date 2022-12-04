using TestGeneratorLib;
namespace TestGeneratorConsole
{
    internal class Program
    {
        private static async Task<string> FileRead(string file)
        {
            string result;
            using (var sr = new StreamReader(file))
            {
                result = await sr.ReadToEndAsync();
            }
            return result;
        }

        static async Task Main(string[] args)
        {
            var a = await FileRead(@"F:\5 sem\спп\MPP_1\Tracer\TracerLib\Tracer.cs");
            var b = new TestGenerator();
            var c = await b.GenerateForFile(a);
            foreach (var d in c)
            {
                Console.WriteLine(d.GetCode());
            }
        }
    }
}