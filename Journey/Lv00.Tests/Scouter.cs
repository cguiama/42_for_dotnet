using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Lv00.Tests;

public class Scouter
{
    // ==========================================
    // RADAR DA FORJA (ANÁLISE ESTÁTICA)
    // ==========================================
    private void VerificarRegrasDaForja(string nomeArquivo)
    {
        string caminhoProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Lv00"));
        string caminhoArquivo = Path.Combine(caminhoProjeto, nomeArquivo);

        if (!File.Exists(caminhoArquivo))
        {
            Assert.Fail($"O arquivo {nomeArquivo} não foi encontrado no chassi. Forje-o primeiro.");
        }

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        Assert.False(codigoFonte.Contains(" var "), $"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}. Tipagem explícita é obrigatória.");
        Assert.False(codigoFonte.Contains("for (") || codigoFonte.Contains("for("), $"[VIOLAÇÃO] Laço 'for' detectado no {nomeArquivo}. Use apenas 'while'.");
        Assert.False(codigoFonte.Contains(".ToString("), $"[VIOLAÇÃO] Uso de .ToString() detectado no {nomeArquivo}. Formatação mágica é proibida.");
        Assert.False(codigoFonte.Contains("using System.Linq;"), $"[VIOLAÇÃO] Magia do LINQ detectada no {nomeArquivo}. Operação bloqueada.");
        Assert.False(codigoFonte.Contains("Console.WriteLine"), $"[VIOLAÇÃO] Console.WriteLine detectado no {nomeArquivo}. Controle a quebra de linha manualmente com Console.Write.");
    }

    // ==========================================
    // AUDITORIAS DE FLUXO (I/O REDIRECTION)
    // ==========================================
    private string InterceptarSaida(Action acaoMotor)
    {
        TextWriter originalOut = Console.Out;
        using StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            acaoMotor.Invoke();
            return stringWriter.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // ==========================================
    // QUESTS DO LV00
    // ==========================================

    [Fact]
    public void Quest00_DeveConterChassiDeContencaoEMain()
    {
        Type tipoProgram = Type.GetType("Lv00.Program, Lv00");
        Assert.True(tipoProgram != null, "A classe Program não foi encontrada no namespace Lv00.");

        MethodInfo metodoMain = tipoProgram.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
        Assert.True(metodoMain != null, "O método 'public static void Main()' não foi encontrado.");
    }

    [Fact]
    public void Quest01_DeveImprimirCaractereNaMemoria()
    {
        VerificarRegrasDaForja("Quest01.cs");
        string resultado = InterceptarSaida(() => Lv00.Quest01.PrintChar('G'));
        Assert.Equal("G", resultado);
    }

    [Fact]
    public void Quest02_DeveImprimirAlfabetoAscii()
    {
        VerificarRegrasDaForja("Quest02.cs");
        string resultado = InterceptarSaida(() => Lv00.Quest02.PrintAlphabet());
        Assert.Equal("abcdefghijklmnopqrstuvwxyz", resultado);
    }

    [Fact]
    public void Quest03_DeveImprimirAlfabetoReverso()
    {
        VerificarRegrasDaForja("Quest03.cs");
        string resultado = InterceptarSaida(() => Lv00.Quest03.PrintReverseAlphabet());
        Assert.Equal("zyxwvutsrqponmlkjihgfedcba", resultado);
    }

    [Fact]
    public void Quest04_DeveImprimirNumerosAscii()
    {
        VerificarRegrasDaForja("Quest04.cs");
        string resultado = InterceptarSaida(() => Lv00.Quest04.PrintNumbers());
        Assert.Equal("0123456789", resultado);
    }

    [Theory]
    [InlineData(42, "P")]
    [InlineData(0, "P")]
    [InlineData(-42, "N")]
    [InlineData(int.MinValue, "N")]
    [InlineData(int.MaxValue, "P")]
    public void Quest05_DeveValidarDesvioCondicional(int energia, string saidaEsperada)
    {
        VerificarRegrasDaForja("Quest05.cs");
        string resultado = InterceptarSaida(() => Lv00.Quest05.PrintSign(energia));
        Assert.Equal(saidaEsperada, resultado);
    }

    [Fact]
    public void Quest06_DeveImprimirCombinacoesDeTresDigitos()
    {
        VerificarRegrasDaForja("Quest06.cs");

        var expectativa = new StringBuilder();
        for (int i = 0; i <= 7; i++)
            for (int j = i + 1; j <= 8; j++)
                for (int k = j + 1; k <= 9; k++)
                    expectativa.Append($"{i}{j}{k}{(i == 7 && j == 8 && k == 9 ? "" : ", ")}");

        string resultado = InterceptarSaida(() => Lv00.Quest06.PrintCombinations());
        Assert.Equal(expectativa.ToString(), resultado);
    }

    [Fact]
    public void Quest07_DeveImprimirCombinacoesDeDuasDezenas()
    {
        VerificarRegrasDaForja("Quest07.cs");

        var expectativa = new StringBuilder();
        for (int i = 0; i <= 98; i++)
            for (int j = i + 1; j <= 99; j++)
            {
                expectativa.Append($"{i:D2} {j:D2}");
                if (!(i == 98 && j == 99)) expectativa.Append(", ");
            }

        string resultado = InterceptarSaida(() => Lv00.Quest07.PrintDoubleCombinations());
        Assert.Equal(expectativa.ToString(), resultado);
    }

    [Theory]
    [InlineData(42, "42")]
    [InlineData(0, "0")]
    [InlineData(-42, "-42")]
    [InlineData(int.MaxValue, "2147483647")]
    [InlineData(int.MinValue, "-2147483648")] // O teste de estresse do Boss
    public void Quest08_DeveImprimirInteiroComRecursao(int carga, string saidaEsperada)
    {
        VerificarRegrasDaForja("Quest08.cs");
        string resultado = InterceptarSaida(() => Lv00.Quest08.PrintInteger(carga));
        Assert.Equal(saidaEsperada, resultado);
    }
}