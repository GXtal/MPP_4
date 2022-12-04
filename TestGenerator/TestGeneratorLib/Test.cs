using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace TestGeneratorLib
{
    public class Test
    {
        private SyntaxTree _finalCode;

        public string GetCode()
        {
            return _finalCode.ToString();
        }
        
    }
}
