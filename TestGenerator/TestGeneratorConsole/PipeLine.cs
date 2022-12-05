using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestGeneratorLib;

namespace TestGeneratorConsole
{
    public class PipeLine
    {
        TestGenerator _testGenerator;

        private int _maxReadingTask;
        private int _maxProcessingTask;
        private int _maxWritingTask;

        static readonly string extensionCS = ".cs";

        private struct StringPair
        {
            public string Name { get; init; }
            public string Value { get; init; }

            public StringPair(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }

        public PipeLine(int maxReadingTask, int maxProcessingTask, int maxWritingTask)
        {
            _testGenerator = new TestGenerator();
            _maxReadingTask = maxReadingTask;
            _maxProcessingTask = maxProcessingTask;
            _maxWritingTask = maxWritingTask;
        }
        public async Task Process(string srcDir, string resDir)
        {
            // prep file work
            if (!Directory.Exists(srcDir))
            {
                throw new ArgumentException(srcDir, "cannot find src directory");
            }

            if (!Directory.Exists(resDir))
            {
                Directory.CreateDirectory(resDir);
            }

            // prepare dataflow
            var readFiles = new TransformBlock<string, StringPair>(
                async path => new StringPair(Path.GetFileName(path), await FileRead(path)),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxReadingTask });

            var processFiles = new TransformBlock<StringPair, List<StringPair>>(
                content => FileProcess(content),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxProcessingTask });

            var writeFiles = new ActionBlock<List<StringPair>>(
                async content => await FileWrite(content, resDir),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxWritingTask });

            // link sep dataflows to sequence of dataflows
            var linkOptions = new DataflowLinkOptions();
            linkOptions.PropagateCompletion = true;

            readFiles.LinkTo(processFiles, linkOptions);
            processFiles.LinkTo(writeFiles, linkOptions);
            // launch for all files in the dir
            foreach (var filePath in Directory.GetFiles(srcDir))
            {
                readFiles.Post(filePath);
            }
            // task ready (no more changes)
            readFiles.Complete();
            // wait for completion
            await writeFiles.Completion;
        }

        private async Task<List<StringPair>> FileProcess(StringPair srcFile)
        {
            List<StringPair> results = new List<StringPair>();

            var tests = await _testGenerator.GenerateForFile(srcFile.Value);

            if (tests == null)
            {
                return results;
            }

            foreach (var testContent in tests)
            {
                var resultName = srcFile.Name + '_' + testContent.NamespaceName + "_" + testContent.ClassName;
                results.Add(new StringPair(resultName, testContent.GetCode()));
            }
            return results;
        }
        private async Task<string> FileRead(string file)
        {
            string result;
            using (var sr = new StreamReader(file))
            {
                result = await sr.ReadToEndAsync();
            }
            return result;
        }
        private async Task FileWrite(List<StringPair> files, string dirto)
        {
            foreach (var filedata in files)
            {
                var resultFilePath = dirto + Path.DirectorySeparatorChar + filedata.Name + extensionCS;
                Console.WriteLine(resultFilePath);
                using (var sw = new StreamWriter(resultFilePath))
                {
                    await sw.WriteAsync(filedata.Value);
                }
            }
        }
    }
}
