using TestGeneratorLib;
namespace TestGeneratorConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            PipeLine a = new PipeLine(4, 4, 4);

            await a.Process(@"F:\5 sem\спп\MPP_1\Tracer\TracerLib", @"F:\5 sem\спп\res");
            
        }
    }
}