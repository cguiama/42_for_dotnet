using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Lv00.Tests
{
    public class Ex00_Tests
    {
        [Fact]
        public void PrintChar_DeveImprimirLetraZNaTela()
        {
            TextWriter originalOut = Console.Out;
            using StringWriter stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            Lv00.Program.PrintChar('Z');

            string resultado = stringWriter.ToString();

            Console.SetOut(originalOut);

            Assert.Equal("Z", resultado);
        }
    }
}