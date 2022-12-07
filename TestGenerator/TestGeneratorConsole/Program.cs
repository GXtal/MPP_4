using TestGeneratorLib;
namespace TestGeneratorConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string srcDir,resDir;
            Console.WriteLine("Доброе утро! Введите папку, из которой взять файлы классов для генерации тестов");
            srcDir=Console.ReadLine();
            Console.WriteLine("Введите папку, куда сохранить итоговые файлы тестов");
            resDir=Console.ReadLine();
            int maxReadingTask;
            int maxProcessingTask;
            int maxWritingTask;
            Console.WriteLine("количесвто потоков чтения файлов");
            maxReadingTask = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("количесвто потоков обработки файлов");
            maxProcessingTask = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("количесвто потоков записи файлов");
            maxWritingTask = Convert.ToInt32(Console.ReadLine());

            PipeLine pipeLine = new PipeLine(maxReadingTask, maxProcessingTask, maxWritingTask);
            await pipeLine.Process(srcDir, resDir);


        }
    }
}