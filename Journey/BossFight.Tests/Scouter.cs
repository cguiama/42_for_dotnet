#nullable disable
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace BossFight.Tests;

public class ForjaException : Exception
{
    public ForjaException(string message) : base(message) { }
    public override string StackTrace => null;
}

public class Scouter : IDisposable
{
    private readonly StringWriter _consoleOutput;
    private readonly TextWriter _originalOutput;

    public Scouter()
    {
        // Sequestra a saída de vídeo do Sistema Operacional para auditoria na memória
        _consoleOutput = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_consoleOutput);
    }

    public void Dispose()
    {
        // Restaura a física original do hardware após cada teste
        Console.SetOut(_originalOutput);
        _consoleOutput.Dispose();
    }

    private void VerificarRegrasDaForja(string nomeArquivo)
    {
        string caminhoProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "BossFight"));
        string caminhoArquivo = Path.Combine(caminhoProjeto, nomeArquivo);

        if (!File.Exists(caminhoArquivo))
            throw new ForjaException($"[MISSÃO INATIVA] O arquivo físico {nomeArquivo} não existe.");

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        if (codigoFonte.Contains(" var ")) throw new ForjaException($"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains("for (") || codigoFonte.Contains("for(")) throw new ForjaException($"[VIOLAÇÃO] Laço 'for' detectado.");
        if (codigoFonte.Contains("foreach")) throw new ForjaException($"[VIOLAÇÃO] Iterador 'foreach' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains("using System.Linq;") || codigoFonte.Contains(".ToList()"))
            throw new ForjaException($"[VIOLAÇÃO] Magia do LINQ detectada. Use as extensões que você forjou no Lv03.");
        if (codigoFonte.Contains("List<"))
            throw new ForjaException($"[VIOLAÇÃO] O uso do List<T> nativo é proibido. Use o seu Lv03.GenericList<T>.");
    }

    private Type ObterPlantaBaixa(string nomeQuest, string nomeTipo)
    {
        VerificarRegrasDaForja($"{nomeQuest}.cs");
        Type tipo = Type.GetType($"BossFight.{nomeTipo}, BossFight");
        if (tipo == null)
            throw new ForjaException($"[MISSÃO INATIVA] O molde/classe {nomeTipo} não foi forjado em {nomeQuest}.");
        return tipo;
    }

    [Fact]
    public void Quest00_ChassiDeCLI_MainExitCode()
    {
        Type tipoProgram = ObterPlantaBaixa("Program", "Program");
        MethodInfo metodoMain = tipoProgram.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);

        if (metodoMain == null) throw new ForjaException("[FALHA DE ASSINATURA] O método Main não foi encontrado.");
        if (metodoMain.ReturnType != typeof(int)) throw new ForjaException("[FALHA ESTRUTURAL] O Main deve retornar um Exit Code (int).");

        ParameterInfo[] parametros = metodoMain.GetParameters();
        if (parametros.Length != 1 || parametros[0].ParameterType != typeof(string[]))
            throw new ForjaException("[FALHA DE ASSINATURA] O Main deve receber os argumentos (string[] args) do SO.");
    }

    [Fact]
    public void Quest01_InterpretadorDeSinais_ArgParser()
    {
        Type tipoParser = ObterPlantaBaixa("ArgParser", "ArgParser");
        MethodInfo motorExtract = tipoParser.GetMethod("ExtractFilePath", BindingFlags.Public | BindingFlags.Static);

        if (motorExtract == null) throw new ForjaException("[FALHA DE ASSINATURA] Motor ExtractFilePath não encontrado.");

        // Teste de Ignição Limpa
        string[] argsValidos = new string[] { "--verbose", "--target", "C:/dados.txt" };
        string resultado = (string)motorExtract.Invoke(null, new object[] { argsValidos });
        Assert.Equal("C:/dados.txt", resultado);

        // Teste de Colapso de Protocolo
        string[] argsSujos = new string[] { "--read", "C:/dados.txt" }; // Faltou a flag --target
        TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => motorExtract.Invoke(null, new object[] { argsSujos }));
        Assert.NotNull(ex.InnerException); // O seu código deve explodir uma exceção aqui
    }

    [Fact]
    public void Quest02_VarreduraDeDisco_FileScanner()
    {
        Type tipoScanner = ObterPlantaBaixa("FileScanner", "FileScanner");
        object scanner = Activator.CreateInstance(tipoScanner);
        MethodInfo motorRead = tipoScanner.GetMethod("ReadFile");

        if (motorRead == null) throw new ForjaException("[FALHA DE ASSINATURA] Motor ReadFile não encontrado.");

        // Criação de arquivo temporário na física do disco
        string tempPath = Path.GetTempFileName();
        File.WriteAllLines(tempPath, new string[] { "Linha 1", "Linha 2" });

        try
        {
            // Teste de Leitura Estável
            object listaValida = motorRead.Invoke(scanner, new object[] { tempPath });
            int countValido = (int)listaValida.GetType().GetProperty("Count").GetValue(listaValida);
            Assert.Equal(2, countValido);

            // Teste de Tratamento de Colapso (try-catch interno)
            object listaVazia = motorRead.Invoke(scanner, new object[] { "C:/caminho_inexistente_na_memoria_4242.txt" });
            int countVazio = (int)listaVazia.GetType().GetProperty("Count").GetValue(listaVazia);
            Assert.Equal(0, countVazio); // O programa não pode travar, deve devolver lista com Count 0

            Assert.Contains("FALHA DE LEITURA", _consoleOutput.ToString());
        }
        finally
        {
            // Aciona o Garbage Collector do disco rígido
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    [Fact]
    public void Quest03_EncadeamentoDeArquivo_DataProcessor()
    {
        Type tipoProcessor = ObterPlantaBaixa("DataProcessor", "DataProcessor");
        MethodInfo motorAnalyze = tipoProcessor.GetMethod("Analyze", BindingFlags.Public | BindingFlags.Static);

        if (motorAnalyze == null) throw new ForjaException("[FALHA DE ASSINATURA] Motor Analyze não encontrado.");

        // Carrega a matriz bruta do Lv03
        Type tipoLista = Type.GetType("Lv03.GenericList`1, Lv03").MakeGenericType(typeof(string));
        object fileData = Activator.CreateInstance(tipoLista);
        MethodInfo addMotor = tipoLista.GetMethod("Add");

        addMotor.Invoke(fileData, new object[] { "  Dado Sujo 1  " });
        addMotor.Invoke(fileData, new object[] { "" }); // Deve ser filtrado
        addMotor.Invoke(fileData, new object[] { "Dado Sujo 2 " });

        motorAnalyze.Invoke(null, new object[] { fileData });

        string saidaTerminal = _consoleOutput.ToString();

        // O Action forjado deve ter disparado os WriteLine com a matéria limpa
        Assert.Contains("Dado Sujo 1", saidaTerminal);
        Assert.Contains("Dado Sujo 2", saidaTerminal);
        Assert.DoesNotContain("  Dado", saidaTerminal); // Confirma a transmutação (Trim)
    }
}