#nullable disable
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Lv01.Tests;

public class ForjaException : Exception
{
    public ForjaException(string message) : base(message) { }
    public override string StackTrace => null;
}

public class Scouter
{
    // ==========================================
    // SONDA DE MEMÓRIA DUPLA (HEAP E I/O)
    // ==========================================
    private (object Retorno, string SaidaTerminal) InspecionarMemoria(string nomeQuest, string nomeMetodo, object[] parametros = null)
    {
        VerificarRegrasDaForja($"{nomeQuest}.cs");

        Type tipoQuest = Type.GetType($"Lv01.{nomeQuest}, Lv01");
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
            // O Invoke alterará fisicamente o conteúdo do array 'parametros' se a função usar 'ref'
            object retorno = motor.Invoke(null, parametros);
            return (retorno, stringWriter.ToString());
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    private void VerificarRegrasDaForja(string nomeArquivo)
    {
        // Aponta o radar para a pasta Lv01
        string caminhoProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Lv01"));
        string caminhoArquivo = Path.Combine(caminhoProjeto, nomeArquivo);

        if (!File.Exists(caminhoArquivo))
            throw new ForjaException($"[MISSÃO INATIVA] O arquivo físico {nomeArquivo} não existe.");

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        // Bloqueios Universais
        if (codigoFonte.Contains(" var ")) throw new ForjaException($"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains("for (") || codigoFonte.Contains("for(")) throw new ForjaException($"[VIOLAÇÃO] Laço 'for' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains(".ToString(")) throw new ForjaException($"[VIOLAÇÃO] Magia .ToString() detectada no {nomeArquivo}.");
        if (codigoFonte.Contains("using System.Linq;")) throw new ForjaException($"[VIOLAÇÃO] Invocação do LINQ detectada no {nomeArquivo}.");
        if (codigoFonte.Contains("Console.WriteLine")) throw new ForjaException($"[VIOLAÇÃO] Console.WriteLine detectado no {nomeArquivo}.");

        // Bloqueios Exclusivos da Heap (Lv01)
        if (codigoFonte.Contains(".Length")) throw new ForjaException($"[VIOLAÇÃO] Propriedade .Length bloqueada no {nomeArquivo}. Provoque o colapso térmico manualmente.");
        if (codigoFonte.Contains("foreach")) throw new ForjaException($"[VIOLAÇÃO] Iterador 'foreach' bloqueado no {nomeArquivo}.");
        if (codigoFonte.Contains("int.Parse") || codigoFonte.Contains("Convert.ToInt")) throw new ForjaException($"[VIOLAÇÃO] Magia de conversão numérica bloqueada no {nomeArquivo}.");
        if (codigoFonte.Contains("Array.Reverse") || codigoFonte.Contains(".Reverse()")) throw new ForjaException($"[VIOLAÇÃO] Inversão mágica detectada no {nomeArquivo}.");

        // Regra específica do Quest04 (ReversePolarity): Proíbe alocação de novos blocos
        if (nomeArquivo == "Quest04.cs" && codigoFonte.Contains(" new "))
            throw new ForjaException($"[VIOLAÇÃO DE MEMÓRIA] Instanciação ('new') bloqueada no {nomeArquivo}. Você deve rotacionar a matéria in-place.");
    }

    // ==========================================
    // BATERIA DE TESTES (QUEST LOG LV01)
    // ==========================================

    [Fact]
    public void Quest00_DeveConterChassiDeContencaoEMain()
    {
        Type tipoProgram = Type.GetType("Lv01.Program, Lv01");
        if (tipoProgram == null) throw new ForjaException("[MISSÃO INATIVA] O Program.cs não possui o chassi Lv01 exigido.");

        MethodInfo metodoMain = tipoProgram.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
        if (metodoMain == null) throw new ForjaException("[FALHA DE ASSINATURA] O método Main não foi encontrado.");
    }

    [Fact]
    public void Quest01_PermutaTermica()
    {
        // A Física: Passamos um array de objetos. Se o método usar 'ref', a CLR alterará os valores originais dentro deste array.
        object[] energia = new object[] { 42, 99 };
        InspecionarMemoria("Quest01", "Swap", energia);

        Assert.Equal(99, energia[0]);
        Assert.Equal(42, energia[1]);
    }

    [Fact]
    public void Quest02_MedicaoDoBloco()
    {
        char[] bufferFisico = new char[] { 'S', 'y', 's', 't', 'e', 'm' };
        var resultado = InspecionarMemoria("Quest02", "MeasureBlock", new object[] { bufferFisico });

        Assert.Equal(6, resultado.Retorno);
    }

    [Fact]
    public void Quest03_TransmissorDeMassa()
    {
        string pacoteDeDados = "Engenharia de Memoria Contigua";
        var resultado = InspecionarMemoria("Quest03", "TransmitBlock", new object[] { pacoteDeDados });

        Assert.Equal(pacoteDeDados, resultado.SaidaTerminal);
    }

    [Fact]
    public void Quest04_InversaoDePolaridade()
    {
        int[] blocoMatriz = new int[] { 10, 20, 30, 40, 50 };
        object[] parametros = new object[] { blocoMatriz };

        InspecionarMemoria("Quest04", "ReversePolarity", parametros);

        // Verifica se o array injetado sofreu inversão in-place
        Assert.Equal(new int[] { 50, 40, 30, 20, 10 }, (int[])parametros[0]);
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData("   -42", -42)]
    [InlineData(" ---++42A5", -42)] // Extrai apenas até o primeiro erro (A), 3 negativos invertem pra -
    [InlineData(" +++100", 100)]
    [InlineData("-2147483648", -2147483648)] // Teste de estresse
    [InlineData("2147483647", 2147483647)]
    public void Quest05_DecodificadorDeMateria(string pacote, int saidaEsperada)
    {
        var resultado = InspecionarMemoria("Quest05", "DecodeMatter", new object[] { pacote });
        Assert.Equal(saidaEsperada, resultado.Retorno);
    }
}