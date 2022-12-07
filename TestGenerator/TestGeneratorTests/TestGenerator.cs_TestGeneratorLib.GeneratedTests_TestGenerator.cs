using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestGeneratorLib;
using NUnit.Framework;


namespace TestGeneratorLib.GeneratedTests
{
    [TestFixture]
    public class TestOfTestGenerator
    {

        [Test]
        public async Task GenerateForFile_EmptyFile_Test()
        {
            //Arrange
            TestGenerator generator = new TestGenerator();

            string content;
            string file = @"F:\5 sem\ñïï\test files\empty.cs";

            //act
            using (var sr = new StreamReader(file))
            {
                content = sr.ReadToEnd();
            }

            var tests = await generator.GenerateForFile(content);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(tests, Is.Not.EqualTo(null));
                Assert.That(tests.Count, Is.EqualTo(0));
            });

        }

        [Test]
        public async Task GenerateForFile_OneFileOneClass_Test()
        {
            //Arrange
            TestGenerator generator = new TestGenerator();

            string content;
            string file = @"F:\5 sem\ñïï\test files\BoolGenerator.cs";

            //act
            using (var sr = new StreamReader(file))
            {
                content = sr.ReadToEnd();
            }

            var tests = await generator.GenerateForFile(content);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(tests, Is.Not.EqualTo(null));
                Assert.That(tests.Count, Is.EqualTo(1));
                Assert.That(tests[0].ClassName, Is.EqualTo("BoolGenerator"));
            });
            
        }

        [Test]
        public async Task GenerateForFile_OneFileTwoClass_Test()
        {
            //Arrange
            TestGenerator generator = new TestGenerator();

            string content;
            string file = @"F:\5 sem\ñïï\test files\MultiFile.cs";

            //act
            using (var sr = new StreamReader(file))
            {
                content = sr.ReadToEnd();
            }

            var tests = await generator.GenerateForFile(content);

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(tests, Is.Not.EqualTo(null));
                Assert.That(tests.Count, Is.EqualTo(2));
                Assert.That(tests[0].ClassName, Is.EqualTo("BoolGenerator"));
                Assert.That(tests[1].ClassName, Is.EqualTo("CharGenerator"));
            });

        }

        [Test]
        public async Task CheckTestsSructure_Test()
        {
            //Arrange
            TestGenerator generator = new TestGenerator();

            string content;
            string file = @"F:\5 sem\ñïï\test files\BoolGenerator.cs";

            //act
            using (var sr = new StreamReader(file))
            {
                content = sr.ReadToEnd();
            }

            var tests = await generator.GenerateForFile(content);
            string res=tests[0].GetCode();

            var root = CSharpSyntaxTree.ParseText(res).GetCompilationUnitRoot();

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                .Where(_class => _class.Modifiers.Any(SyntaxKind.PublicKeyword));
            var classDeclaration = classes.ElementAt(0);

            var sourceMethods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(sourceMethod => sourceMethod.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();

            //assert
            Assert.Multiple(() =>
            {
                Assert.That(classDeclaration.AttributeLists.ElementAt(0).ToString(), Is.EqualTo("[TestFixture]"));
                Assert.That(sourceMethods.Count, Is.EqualTo(2));

                Assert.That(sourceMethods[0].AttributeLists.ElementAt(0).ToString(),Is.EqualTo("[Test]"));
            });

        }

    }
}