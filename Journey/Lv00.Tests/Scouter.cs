#nullable disable
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Lv00.Tests;

// 1. O SILENCIADOR TÉRMICO
// Hackeamos a classe base de erros do .NET. Quando o xUnit tentar 
// ler o rastro de sangue, nós devolvemos um ponteiro nulo.
public class ForjaException : Exception
{
    public ForjaException(string message) : base(message) { }
    public override string StackTrace => null;
}

public class Scouter
{
    private string InspecionarMemoria(string nomeQuest, string nomeMetodo, object[] parametros = null)
    {
        VerificarRegrasDaForja($"{nomeQuest}.cs");

        Type tipoQuest = Type.GetType($"Lv00.{nomeQuest}, Lv00");
        if (tipoQuest == null)
            throw new ForjaException($"[MISSÃO INATIVA] A classe estática {nomeQuest} não foi forjada.");

        MethodInfo motor = tipoQuest.GetMethod(nomeMetodo, BindingFlags.Public | BindingFlags.Static);
        if (motor == null)
            throw new ForjaException($"[FALHA DE ASSINATURA] O motor {nomeMetodo} não foi encontrado em {nomeQuest}.");

        TextWriter originalOut = Console.Out;
        using StringWriter stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            motor.Invoke(null, parametros);
            return stringWriter.ToString();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    private void VerificarRegrasDaForja(string nomeArquivo)
    {
        string caminhoProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Lv00"));
        string caminhoArquivo = Path.Combine(caminhoProjeto, nomeArquivo);

        if (!File.Exists(caminhoArquivo))
            throw new ForjaException($"[MISSÃO INATIVA] O arquivo físico {nomeArquivo} não existe.");

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        if (codigoFonte.Contains(" var ")) throw new ForjaException($"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains("for (") || codigoFonte.Contains("for(")) throw new ForjaException($"[VIOLAÇÃO] Laço 'for' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains(".ToString(")) throw new ForjaException($"[VIOLAÇÃO] Magia .ToString() detectada no {nomeArquivo}.");
        if (codigoFonte.Contains("using System.Linq;")) throw new ForjaException($"[VIOLAÇÃO] Invocação do LINQ detectada no {nomeArquivo}.");
        if (codigoFonte.Contains("Console.WriteLine")) throw new ForjaException($"[VIOLAÇÃO] Console.WriteLine detectado no {nomeArquivo}.");
    }

    [Fact]
    public void Quest00_DeveConterChassiDeContencaoEMain()
    {
        Type tipoProgram = Type.GetType("Lv00.Program, Lv00");
        if (tipoProgram == null) throw new ForjaException("[MISSÃO INATIVA] O Program.cs não possui o chassi exigido.");

        MethodInfo metodoMain = tipoProgram.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
        if (metodoMain == null) throw new ForjaException("[FALHA DE ASSINATURA] O método Main não foi encontrado.");
    }

    [Fact]
    public void Quest01_PulsoDeTransmissao()
    {
        string resultado = InspecionarMemoria("Quest01", "TransmitSignal", new object[] { 'G' });
        Assert.Equal("G", resultado);
    }

    [Fact]
    public void Quest02_CalibragemDialHexadecimal()
    {
        string resultado = InspecionarMemoria("Quest02", "CalibrateHexDial");
        Assert.Equal("0123456789ABCDEF", resultado);
    }

    [Fact]
    public void Quest03_FiltroDeFrequencia()
    {
        string resultado = InspecionarMemoria("Quest03", "FilterFrequency");
        Assert.Equal("bcdfghjklmnpqrstvwxyz", resultado);
    }

    [Theory]
    [InlineData(85, "C")]
    [InlineData(80, "C")]
    [InlineData(50, "S")]
    [InlineData(1, "S")]
    [InlineData(0, "F")]
    [InlineData(-42, "F")]
    public void Quest04_DiagnosticoDeTermostato(int energia, string saidaEsperada)
    {
        string resultado = InspecionarMemoria("Quest04", "SystemDiagnostics", new object[] { energia });
        Assert.Equal(saidaEsperada, resultado);
    }

    [Fact]
    public void Quest05_MatrizDeCoordenadasSeguras()
    {
        var expectativa = new StringBuilder();
        for (int i = 0; i <= 8; i++)
        {
            for (int j = i + 1; j <= 9; j++)
            {
                expectativa.Append($"[{i},{j}]");
                if (!(i == 8 && j == 9)) expectativa.Append(" ");
            }
        }

        string resultado = InspecionarMemoria("Quest05", "PrintSafeCoordinates");
        Assert.Equal(expectativa.ToString(), resultado);
    }

    [Theory]
    [InlineData(42, "42")]
    [InlineData(0, "0")]
    [InlineData(-42, "-42")]
    [InlineData(int.MaxValue, "2147483647")]
    [InlineData(int.MinValue, "-2147483648")]
    public void Quest06_DesfragmentacaoDeSetor(int carga, string saidaEsperada)
    {
        string resultado = InspecionarMemoria("Quest06", "DecodeSectorAddress", new object[] { carga });
        Assert.Equal(saidaEsperada, resultado);
    }
}