using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TestGeneratorLib
{
    public class Test
    {
        private SyntaxTree? _finalCode;
        public string ClassName { get; private set; }
        public string NamespaceName { get; private set; }

        public Test(string className, string namespaceName, SyntaxTree? tree)
        {
            ClassName = className;
            NamespaceName = namespaceName;
            _finalCode = tree;
        }
        public string? GetCode()
        {
            if(_finalCode==null)
            {
                return null;
            }
            return _finalCode.ToString();
        }
        
    }
}
