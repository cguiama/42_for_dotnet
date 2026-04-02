#nullable disable
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Lv03.Tests;

public class ForjaException : Exception
{
    public ForjaException(string message) : base(message) { }
    public override string StackTrace => null;
}

public class Scouter
{
    private void VerificarRegrasDaForja(string nomeArquivo)
    {
        string caminhoProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Lv03"));
        string caminhoArquivo = Path.Combine(caminhoProjeto, nomeArquivo);

        if (!File.Exists(caminhoArquivo))
            throw new ForjaException($"[MISSÃO INATIVA] O arquivo físico {nomeArquivo} não existe.");

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        // Blindagem Universal e de Memória Herdada
        if (codigoFonte.Contains(" var ")) throw new ForjaException($"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains("for (") || codigoFonte.Contains("for(")) throw new ForjaException($"[VIOLAÇÃO] Laço 'for' detectado.");
        if (codigoFonte.Contains("foreach")) throw new ForjaException($"[VIOLAÇÃO] Iterador 'foreach' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains(".Length")) throw new ForjaException($"[VIOLAÇÃO] Propriedade .Length bloqueada.");

        // Blindagem Exclusiva do Lv03
        if (codigoFonte.Contains("using System.Linq;") || codigoFonte.Contains(".ToList()"))
            throw new ForjaException($"[VIOLAÇÃO] Magia do LINQ detectada. Você deve forjar a sua própria esteira.");
        if (codigoFonte.Contains("List<") && nomeArquivo != "Quest06.cs" && nomeArquivo != "Quest07.cs")
            throw new ForjaException($"[VIOLAÇÃO] O uso do List<T> nativo é proibido. Use o seu GenericList<T>.");
    }

    private Type ObterPlantaBaixa(string nomeQuest, string nomeTipo)
    {
        VerificarRegrasDaForja($"{nomeQuest}.cs");
        Type tipo = Type.GetType($"Lv03.{nomeTipo}, Lv03");
        if (tipo == null)
            throw new ForjaException($"[MISSÃO INATIVA] O molde/classe {nomeTipo} não foi forjado em {nomeQuest}.");
        return tipo;
    }

    [Fact]
    public void Quest00_DeveConterChassiDeContencaoEMain()
    {
        Type tipoProgram = ObterPlantaBaixa("Program", "Program");
        MethodInfo metodoMain = tipoProgram.GetMethod("Main", BindingFlags.Public | BindingFlags.Static);
        if (metodoMain == null) throw new ForjaException("[FALHA DE ASSINATURA] O método Main não foi encontrado.");
    }

    [Fact]
    public void Quest01_VetorDinamico_DynamicArray()
    {
        Type tipo = ObterPlantaBaixa("Quest01", "DynamicArray");
        object obj = Activator.CreateInstance(tipo);

        PropertyInfo countProp = tipo.GetProperty("Count");
        PropertyInfo capacityProp = tipo.GetProperty("Capacity");
        MethodInfo addMotor = tipo.GetMethod("Add", new Type[] { typeof(int) });

        Assert.Equal(0, countProp.GetValue(obj));
        Assert.Equal(2, capacityProp.GetValue(obj)); // Nasce com 2

        // Estressa a matriz para forçar o redimensionamento térmico
        addMotor.Invoke(obj, new object[] { 10 });
        addMotor.Invoke(obj, new object[] { 20 });
        addMotor.Invoke(obj, new object[] { 30 }); // Quebra da Heap (Redimensionamento)

        Assert.Equal(3, countProp.GetValue(obj));
        Assert.True((int)capacityProp.GetValue(obj) >= 4); // O Capacity deve ter dobrado
    }

    [Fact]
    public void Quest02_ContratoGenerico_GenericList()
    {
        VerificarRegrasDaForja("Quest02.cs");
        // O `1 indica que a classe exige 1 parâmetro genérico <T>
        Type tipoAberto = Type.GetType("Lv03.GenericList`1, Lv03");
        if (tipoAberto == null) throw new ForjaException("[MISSÃO INATIVA] GenericList<T> não foi forjada.");

        Type tipoFechado = tipoAberto.MakeGenericType(typeof(string));
        object obj = Activator.CreateInstance(tipoFechado);

        MethodInfo addMotor = tipoFechado.GetMethod("Add", new Type[] { typeof(string) });
        MethodInfo getMotor = tipoFechado.GetMethod("Get", new Type[] { typeof(int) });

        addMotor.Invoke(obj, new object[] { "Engenharia" });
        Assert.Equal("Engenharia", getMotor.Invoke(obj, new object[] { 0 }));

        // Validação da Blindagem de Leitura Cega
        TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => getMotor.Invoke(obj, new object[] { 5 }));
        Assert.IsType<IndexOutOfRangeException>(ex.InnerException);
    }

    [Fact]
    public void Quest03_PonteiroDeCondicao_FuncFilter()
    {
        VerificarRegrasDaForja("Quest03.cs");
        Type tipoAberto = Type.GetType("Lv03.GenericList`1, Lv03");
        Type tipoFechado = tipoAberto.MakeGenericType(typeof(int));
        object obj = Activator.CreateInstance(tipoFechado);

        MethodInfo addMotor = tipoFechado.GetMethod("Add");
        addMotor.Invoke(obj, new object[] { 1 });
        addMotor.Invoke(obj, new object[] { 2 });
        addMotor.Invoke(obj, new object[] { 3 });

        MethodInfo filterMotor = tipoFechado.GetMethod("Filter", new Type[] { typeof(Func<int, bool>) });
        if (filterMotor == null) throw new ForjaException("[FALHA DE ASSINATURA] O motor Filter(Func<T, bool>) não existe.");

        Func<int, bool> isEven = x => x % 2 == 0;
        object filteredList = filterMotor.Invoke(obj, new object[] { isEven });

        PropertyInfo countProp = tipoFechado.GetProperty("Count");
        Assert.Equal(1, countProp.GetValue(filteredList)); // Apenas o 2 deve ter passado
    }

    [Fact]
    public void Quest04_ExtensaoDaMateria_MeuWhere()
    {
        Type tipoExtensao = ObterPlantaBaixa("Quest04", "LinqExtensions");
        Type tipoAberto = Type.GetType("Lv03.GenericList`1, Lv03");
        Type tipoFechado = tipoAberto.MakeGenericType(typeof(int));

        // Reflection para caçar método estático genérico (Extension Method)
        MethodInfo whereAberto = tipoExtensao.GetMethod("MeuWhere");
        if (whereAberto == null) throw new ForjaException("[FALHA DE ASSINATURA] MeuWhere não forjado em LinqExtensions.");

        MethodInfo whereFechado = whereAberto.MakeGenericMethod(typeof(int));

        object sourceList = Activator.CreateInstance(tipoFechado);
        tipoFechado.GetMethod("Add").Invoke(sourceList, new object[] { 10 });
        tipoFechado.GetMethod("Add").Invoke(sourceList, new object[] { 15 });

        Func<int, bool> greaterThanTen = x => x > 10;
        object resultList = whereFechado.Invoke(null, new object[] { sourceList, greaterThanTen });

        Assert.Equal(1, tipoFechado.GetProperty("Count").GetValue(resultList));
    }

    [Fact]
    public void Quest05_MutacaoDeEstado_MeuSelect()
    {
        Type tipoExtensao = ObterPlantaBaixa("Quest05", "LinqExtensions");
        Type listaInt = Type.GetType("Lv03.GenericList`1, Lv03").MakeGenericType(typeof(int));
        Type listaString = Type.GetType("Lv03.GenericList`1, Lv03").MakeGenericType(typeof(string));

        MethodInfo selectAberto = tipoExtensao.GetMethod("MeuSelect");
        if (selectAberto == null) throw new ForjaException("[FALHA DE ASSINATURA] MeuSelect não forjado.");

        MethodInfo selectFechado = selectAberto.MakeGenericMethod(typeof(int), typeof(string));

        object sourceList = Activator.CreateInstance(listaInt);
        listaInt.GetMethod("Add").Invoke(sourceList, new object[] { 42 });

        Func<int, string> transformer = x => $"ID-{x}";
        object resultList = selectFechado.Invoke(null, new object[] { sourceList, transformer });

        Assert.Equal("ID-42", listaString.GetMethod("Get").Invoke(resultList, new object[] { 0 }));
    }

    [Fact]
    public void Quest06_MotorDeProcedimento_MeuForEach()
    {
        Type tipoExtensao = ObterPlantaBaixa("Quest06", "LinqExtensions");
        Type listaInt = Type.GetType("Lv03.GenericList`1, Lv03").MakeGenericType(typeof(int));

        MethodInfo forEachAberto = tipoExtensao.GetMethod("MeuForEach");
        if (forEachAberto == null) throw new ForjaException("[FALHA DE ASSINATURA] MeuForEach não forjado.");

        MethodInfo forEachFechado = forEachAberto.MakeGenericMethod(typeof(int));

        object sourceList = Activator.CreateInstance(listaInt);
        listaInt.GetMethod("Add").Invoke(sourceList, new object[] { 10 });
        listaInt.GetMethod("Add").Invoke(sourceList, new object[] { 20 });

        int accumulator = 0;
        Action<int> sumAction = x => accumulator += x;

        forEachFechado.Invoke(null, new object[] { sourceList, sumAction });

        Assert.Equal(30, accumulator); // A prova de que o Action disparou a energia para o sistema
    }

    [Fact]
    public void Quest07_BossFight_EncadeamentoDeTurbinas()
    {
        Type pipelineTipo = ObterPlantaBaixa("Quest07", "PipelineAnalyzer");
        MethodInfo processMotor = pipelineTipo.GetMethod("ProcessData", BindingFlags.Public | BindingFlags.Static);

        if (processMotor == null) throw new ForjaException("[FALHA DE ASSINATURA] O motor ProcessData não foi construído.");

        Type listaInt = Type.GetType("Lv03.GenericList`1, Lv03").MakeGenericType(typeof(int));
        object rawList = Activator.CreateInstance(listaInt);
        MethodInfo addMotor = listaInt.GetMethod("Add");

        // Injeta: 2 (ignorado, não vira length 5), 1000 (vira ID-1000 = length 7), 42 (vira ID-42 = length 5)
        addMotor.Invoke(rawList, new object[] { 2 });
        addMotor.Invoke(rawList, new object[] { 3 }); // ímpar, morre no where
        addMotor.Invoke(rawList, new object[] { 42 });
        addMotor.Invoke(rawList, new object[] { 1000 });

        object resultList = processMotor.Invoke(null, new object[] { rawList });
        Type listaString = Type.GetType("Lv03.GenericList`1, Lv03").MakeGenericType(typeof(string));

        int finalCount = (int)listaString.GetProperty("Count").GetValue(resultList);
        Assert.Equal(1, finalCount); // Apenas o 42 deve ter chegado ao fim do pipeline intacto.
        Assert.Equal("ID-42", listaString.GetMethod("Get").Invoke(resultList, new object[] { 0 }));
    }
}