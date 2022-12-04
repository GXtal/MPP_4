using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestGeneratorLib
{
    public class TestGenerator
    {
        private readonly string NamespaceName = "tempnamespace";
        private Test GenerateTest(ClassDeclarationSyntax classDeclaration, List<UsingDirectiveSyntax> _usings)
        {
            var className = classDeclaration.Identifier.Text;
            // methods

            var resultMethods = new List<MemberDeclarationSyntax>();

            var sourceMethods = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(sourceMethod => sourceMethod.Modifiers.Any(SyntaxKind.PublicKeyword)).ToList();

            Dictionary<string, int> map = new Dictionary<string, int>();
            foreach (var method in sourceMethods)
            {
                //creating name for method
                string name = method.Identifier.ValueText + "_Test";
                if (map.ContainsKey(method.Identifier.ValueText))
                {
                    map[method.Identifier.ValueText]++;
                    name += map[method.Identifier.ValueText];
                }
                else
                {
                    map.Add(method.Identifier.ValueText, 0);
                }

                //add atribute [Test]
                var memAttrList = SyntaxFactory.SingletonList(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("Test")))));
                // public method
                var memModfList = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                
                
                resultMethods.Add(SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), name)
                    .WithModifiers(memModfList)
                    .WithAttributeLists(memAttrList));
            }

            // creating classdecl with all madeup methods, public, [TestFixture] atrubte 
            ClassDeclarationSyntax classDecl = SyntaxFactory.ClassDeclaration($"TestOf{className}")
                                    .WithAttributeLists(
                                        SyntaxFactory.SingletonList(
                                            SyntaxFactory.AttributeList(
                                                SyntaxFactory.SingletonSeparatedList(
                                                    SyntaxFactory.Attribute(
                                                        SyntaxFactory.IdentifierName("TestFixture"))))))
                                    .WithModifiers(
                                        SyntaxFactory.TokenList(
                                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
                                    .WithMembers(
                                        SyntaxFactory.List(resultMethods));

            // using ...
            

            SyntaxTree tree; string namespaceName;

            NamespaceDeclarationSyntax? currNamespace = classDeclaration.Parent as NamespaceDeclarationSyntax;

            if (currNamespace != null)
            {
                namespaceName = currNamespace.Name.ToString() + '.' + NamespaceName;
            }else
            {
                namespaceName = NamespaceName;
            }

            tree = CSharpSyntaxTree.Create(
                SyntaxFactory.CompilationUnit()
                    .WithUsings(SyntaxFactory.List(_usings))
                    .AddMembers(SyntaxFactory.NamespaceDeclaration(
                            SyntaxFactory.IdentifierName(namespaceName))
                        .AddMembers(classDecl))
                    .NormalizeWhitespace()
                );


            //
            return new Test(className, namespaceName, tree);
        }
    }
}