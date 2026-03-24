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
        if (codigoFonte.Contains(".Equals") || codigoFonte.Contains(".CompareTo")) throw new ForjaException($"[VIOLAÇÃO] Comparação mágica de memória bloqueada no {nomeArquivo}. Compare byte a byte.");

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
        Assert.Equal(new int[] { 50, 40, 30, 20, 10 }, (int[])parametros[0]);
    }

    [Fact]
    public void Quest05_ClonagemDeMateria()
    {
        char[] fonte = new char[] { 'A', 'r', 'q', 'u', 'i', 't', 'e', 't', 'o' };
        var resultado = InspecionarMemoria("Quest05", "CloneBuffer", new object[] { fonte });

        char[] clone = (char[])resultado.Retorno;

        // 1. Audita se o conteúdo foi copiado corretamente
        Assert.Equal(fonte, clone);

        // 2. A Física Real: Audita se os endereços de memória SÃO DIFERENTES.
        // Se o recruta só fez "return fonte;", ele cai aqui.
        Assert.NotSame(fonte, clone);
    }

    [Theory]
    [InlineData("Fisica", "Fisica", 0)]
    [InlineData("Fisica", "Fisico", -1)] // 'a' (97) é menor que 'o' (111)
    [InlineData("Fisico", "Fisica", 1)]  // 'o' (111) é maior que 'a' (97)
    [InlineData("Fis", "Fisica", -1)]    // Tamanhos diferentes
    [InlineData("Fisica", "Fis", 1)]
    [InlineData("", "", 0)]
    public void Quest06_Espectrometro(string a, string b, int saidaEsperada)
    {
        var resultado = InspecionarMemoria("Quest06", "CompareMatter", new object[] { a, b });
        Assert.Equal(saidaEsperada, resultado.Retorno);
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData("   -42", -42)]
    [InlineData(" ---++42A5", -42)]
    [InlineData(" +++100", 100)]
    [InlineData("-2147483648", -2147483648)]
    [InlineData("2147483647", 2147483647)]
    public void Quest07_DecodificadorDeMateria(string pacote, int saidaEsperada)
    {
        var resultado = InspecionarMemoria("Quest07", "DecodeMatter", new object[] { pacote });
        Assert.Equal(saidaEsperada, resultado.Retorno);
    }
}