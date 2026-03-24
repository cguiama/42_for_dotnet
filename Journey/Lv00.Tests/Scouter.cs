#nullable disable
using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Lv00.Tests;

public class Scouter
{
    private string InspecionarMemoria(string nomeQuest, string nomeMetodo, object[] parametros = null)
    {
        VerificarRegrasDaForja($"{nomeQuest}.cs");

        Type tipoQuest = Type.GetType($"Lv00.{nomeQuest}, Lv00");
        if (tipoQuest == null)
            Assert.Fail($"[MISSÃO INATIVA] A classe {nomeQuest} não foi forjada.");

        MethodInfo motor = tipoQuest.GetMethod(nomeMetodo, BindingFlags.Public | BindingFlags.Static);
        if (motor == null)
            Assert.Fail($"[FALHA DE ASSINATURA] O motor {nomeMetodo} não foi forjado em {nomeQuest}.");

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
            Assert.Fail($"[MISSÃO INATIVA] O arquivo físico {nomeArquivo} não existe.");

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        Assert.False(codigoFonte.Contains(" var "), $"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}.");
        Assert.False(codigoFonte.Contains("for (") || codigoFonte.Contains("for("), $"[VIOLAÇÃO] Laço 'for' detectado no {nomeArquivo}.");
        Assert.False(codigoFonte.Contains(".ToString("), $"[VIOLAÇÃO] Magia .ToString() detectada no {nomeArquivo}.");
        Assert.False(codigoFonte.Contains("using System.Linq;"), $"[VIOLAÇÃO] Invocação do LINQ detectada no {nomeArquivo}.");
        Assert.False(codigoFonte.Contains("Console.WriteLine"), $"[VIOLAÇÃO] Console.WriteLine detectado no {nomeArquivo}.");
    }

    [Fact]
    public void Quest00_DeveConterChassiDeContencaoEMain()
    {
        Type tipoProgram = Type.GetType("Lv00.Program, Lv00");
        if (tipoProgram == null) Assert.Fail("[MISSÃO INATIVA] O Program.cs não possui o chassi exigido.");

        MethodInfo metodoMain = tipoProgram.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
        if (metodoMain == null) Assert.Fail("[FALHA DE ASSINATURA] O método Main não foi encontrado.");
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