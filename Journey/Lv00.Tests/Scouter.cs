using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Lv00.Tests;

public class Scouter
{
    // ==========================================
    // MOTOR DE REFLECTION E I/O (O RADAR)
    // ==========================================
    private string InspecionarMemoria(string nomeQuest, string nomeMetodo, object[] parametros = null)
    {
        // 1. Auditoria da Forja (Análise de sintaxe no arquivo físico antes de rodar a memória)
        VerificarRegrasDaForja($"{nomeQuest}.cs");

        // 2. Radar Térmico: Procura a classe na memória da .dll
        Type tipoQuest = Type.GetType($"Lv00.{nomeQuest}, Lv00");
        Assert.True(tipoQuest != null, $"[MISSÃO INATIVA] A classe estática {nomeQuest} não foi encontrada na memória. Forje o arquivo.");

        // 3. Trava do Motor: Procura o método dentro da classe
        MethodInfo motor = tipoQuest.GetMethod(nomeMetodo, BindingFlags.Public | BindingFlags.Static);
        Assert.True(motor != null, $"[FALHA DE ASSINATURA] O motor 'public static void {nomeMetodo}' não foi forjado dentro de {nomeQuest}.");

        // 4. Sequestro de I/O na Heap
        TextWriter originalOut = Console.Out;
        using StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Dispara o código do recruta dinamicamente injetando os parâmetros na Stack
            motor.Invoke(null, parametros);
            return stringWriter.ToString();
        }
        finally
        {
            // Devolve a física do terminal para o sistema operacional
            Console.SetOut(originalOut);
        }
    }

    private void VerificarRegrasDaForja(string nomeArquivo)
    {
        // Navega da pasta bin/Debug/net10.0 até a raiz do projeto Lv00
        string caminhoProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Lv00"));
        string caminhoArquivo = Path.Combine(caminhoProjeto, nomeArquivo);

        if (!File.Exists(caminhoArquivo))
            Assert.Fail($"[MISSÃO INATIVA] O arquivo físico {nomeArquivo} não existe no disco. Crie-o para iniciar a Quest.");

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        Assert.False(codigoFonte.Contains(" var "), $"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}. Tipagem explícita é obrigatória na Forja.");
        Assert.False(codigoFonte.Contains("for (") || codigoFonte.Contains("for("), $"[VIOLAÇÃO] Laço 'for' detectado no {nomeArquivo}. O rotor deve ser manual (while).");
        Assert.False(codigoFonte.Contains(".ToString("), $"[VIOLAÇÃO] Magia .ToString() detectada no {nomeArquivo}. Conversão térmica proibida.");
        Assert.False(codigoFonte.Contains("using System.Linq;"), $"[VIOLAÇÃO] Invocação do LINQ detectada no {nomeArquivo}. Magia de alto nível bloqueada.");
        Assert.False(codigoFonte.Contains("Console.WriteLine"), $"[VIOLAÇÃO] Console.WriteLine detectado no {nomeArquivo}. Controle os bytes de quebra de linha manualmente.");
    }

    // ==========================================
    // QUEST LOG (BATERIA DE TESTES AUTOMATIZADA)
    // ==========================================

    [Fact]
    public void Quest00_DeveConterChassiDeContencaoEMain()
    {
        Type tipoProgram = Type.GetType("Lv00.Program, Lv00");
        Assert.True(tipoProgram != null, "[MISSÃO INATIVA] O arquivo Program.cs não possui o namespace Lv00 ou a classe Program.");

        MethodInfo metodoMain = tipoProgram.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
        Assert.True(metodoMain != null, "[FALHA DE ASSINATURA] O método Main obrigatório não foi encontrado na classe Program.");
    }

    [Fact]
    public void Quest01_DeveImprimirCaractereNaMemoria()
    {
        string resultado = InspecionarMemoria("Quest01", "PrintChar", new object[] { 'G' });
        Assert.Equal("G", resultado);
    }

    [Fact]
    public void Quest02_DeveImprimirAlfabetoAscii()
    {
        string resultado = InspecionarMemoria("Quest02", "PrintAlphabet");
        Assert.Equal("abcdefghijklmnopqrstuvwxyz", resultado);
    }

    [Fact]
    public void Quest03_DeveImprimirAlfabetoReverso()
    {
        string resultado = InspecionarMemoria("Quest03", "PrintReverseAlphabet");
        Assert.Equal("zyxwvutsrqponmlkjihgfedcba", resultado);
    }

    [Fact]
    public void Quest04_DeveImprimirNumerosAscii()
    {
        string resultado = InspecionarMemoria("Quest04", "PrintNumbers");
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
        string resultado = InspecionarMemoria("Quest05", "PrintSign", new object[] { energia });
        Assert.Equal(saidaEsperada, resultado);
    }

    [Fact]
    public void Quest06_DeveImprimirCombinacoesDeTresDigitos()
    {
        // Construção da expectativa usando o Arsenal (permitido apenas no Scouter)
        var expectativa = new StringBuilder();
        for (int i = 0; i <= 7; i++)
            for (int j = i + 1; j <= 8; j++)
                for (int k = j + 1; k <= 9; k++)
                    expectativa.Append($"{i}{j}{k}{(i == 7 && j == 8 && k == 9 ? "" : ", ")}");

        string resultado = InspecionarMemoria("Quest06", "PrintCombinations");
        Assert.Equal(expectativa.ToString(), resultado);
    }

    [Fact]
    public void Quest07_DeveImprimirCombinacoesDeDuasDezenas()
    {
        var expectativa = new StringBuilder();
        for (int i = 0; i <= 98; i++)
            for (int j = i + 1; j <= 99; j++)
            {
                expectativa.Append($"{i:D2} {j:D2}");
                if (!(i == 98 && j == 99)) expectativa.Append(", ");
            }

        string resultado = InspecionarMemoria("Quest07", "PrintDoubleCombinations");
        Assert.Equal(expectativa.ToString(), resultado);
    }

    [Theory]
    [InlineData(42, "42")]
    [InlineData(0, "0")]
    [InlineData(-42, "-42")]
    [InlineData(int.MaxValue, "2147483647")]
    [InlineData(int.MinValue, "-2147483648")] // O teste de estresse do Boss Fight
    public void Quest08_DeveImprimirInteiroComRecursao(int carga, string saidaEsperada)
    {
        string resultado = InspecionarMemoria("Quest08", "PrintInteger", new object[] { carga });
        Assert.Equal(saidaEsperada, resultado);
    }
}