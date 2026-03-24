#nullable disable
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Lv02.Tests;

public class ForjaException : Exception
{
    public ForjaException(string message) : base(message) { }
    public override string StackTrace => null;
}

public class Scouter
{
    private void VerificarRegrasDaForja(string nomeArquivo)
    {
        string caminhoProjeto = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Lv02"));
        string caminhoArquivo = Path.Combine(caminhoProjeto, nomeArquivo);

        if (!File.Exists(caminhoArquivo))
            throw new ForjaException($"[MISSÃO INATIVA] O arquivo físico {nomeArquivo} não existe.");

        string codigoFonte = File.ReadAllText(caminhoArquivo);

        if (codigoFonte.Contains(" var ")) throw new ForjaException($"[VIOLAÇÃO] Uso de 'var' detectado no {nomeArquivo}.");
        if (codigoFonte.Contains("Console.WriteLine")) throw new ForjaException($"[VIOLAÇÃO] Console.WriteLine detectado no {nomeArquivo}.");
    }

    private Type ObterPlantaBaixa(string nomeQuest, string nomeTipo)
    {
        VerificarRegrasDaForja($"{nomeQuest}.cs");
        Type tipo = Type.GetType($"Lv02.{nomeTipo}, Lv02");
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
    public void Quest01_MoldeDeValor_Struct()
    {
        Type tipo = ObterPlantaBaixa("Quest01", "Coordinate");
        if (!tipo.IsValueType) throw new ForjaException("[VIOLAÇÃO] Coordinate deve ser uma 'struct' (Stack), não uma 'class' (Heap).");

        object obj = Activator.CreateInstance(tipo);
        tipo.GetField("X").SetValue(obj, 10);
        tipo.GetField("Y").SetValue(obj, 20);
        tipo.GetField("Z").SetValue(obj, 30);

        Assert.Equal(10, tipo.GetField("X").GetValue(obj));
    }

    [Fact]
    public void Quest02_PlantaBaixa_Class()
    {
        Type tipo = ObterPlantaBaixa("Quest02", "Vehicle");
        if (!tipo.IsClass) throw new ForjaException("[VIOLAÇÃO] Vehicle deve ser uma 'class' (Heap).");

        object obj = Activator.CreateInstance(tipo);
        tipo.GetField("Wheels").SetValue(obj, 4);
        tipo.GetField("Brand").SetValue(obj, "Fiat");

        Assert.Equal("Fiat", tipo.GetField("Brand").GetValue(obj));
    }

    [Fact]
    public void Quest03_BlindagemDoNucleo_Encapsulamento()
    {
        Type tipo = ObterPlantaBaixa("Quest03", "FuelTank");

        FieldInfo campoPrivado = tipo.GetField("_capacity", BindingFlags.NonPublic | BindingFlags.Instance);
        if (campoPrivado == null) throw new ForjaException("[FALHA DE BLINDAGEM] A variável _capacity não existe ou não é private.");

        PropertyInfo prop = tipo.GetProperty("Capacity");
        object obj = Activator.CreateInstance(tipo);

        prop.SetValue(obj, 50); // Injeção válida
        Assert.Equal(50, prop.GetValue(obj));

        prop.SetValue(obj, -10); // Injeção hostil
        Assert.Equal(50, prop.GetValue(obj)); // A trava deve ter ignorado o -10
    }

    [Fact]
    public void Quest04_BlindagemImutavel_Readonly()
    {
        Type tipo = ObterPlantaBaixa("Quest04", "BlackBox");

        FieldInfo campoPrivado = tipo.GetField("_serialNumber", BindingFlags.NonPublic | BindingFlags.Instance);
        if (campoPrivado == null || !campoPrivado.IsInitOnly)
            throw new ForjaException("[FALHA DE BLINDAGEM] A variável _serialNumber não existe, não é private ou não é readonly.");

        object obj = Activator.CreateInstance(tipo, new object[] { "SN-4242" });
        Assert.Equal("SN-4242", campoPrivado.GetValue(obj));
    }

    [Fact]
    public void Quest05_ProtocoloDeIgnicao_Construtor()
    {
        Type tipo = ObterPlantaBaixa("Quest05", "Reactor");

        ConstructorInfo construtorVazio = tipo.GetConstructor(Type.EmptyTypes);
        if (construtorVazio != null)
            throw new ForjaException("[FALHA DE SEGURANÇA] O Reactor não pode nascer sem energia. Remova o construtor vazio.");

        object obj = Activator.CreateInstance(tipo, new object[] { 5000 });
        PropertyInfo prop = tipo.GetProperty("CoreTemperature");
        Assert.Equal(5000, prop.GetValue(obj));
    }

    [Fact]
    public void Quest06_AdaptabilidadeTermica_Sobrecarga()
    {
        Type tipo = ObterPlantaBaixa("Quest06", "PowerSupply");
        object obj = Activator.CreateInstance(tipo);

        MethodInfo motor1 = tipo.GetMethod("Connect", new Type[] { typeof(int) });
        MethodInfo motor2 = tipo.GetMethod("Connect", new Type[] { typeof(int), typeof(string) });

        if (motor1 == null || motor2 == null)
            throw new ForjaException("[FALHA DE ASSINATURA] Os dois motores 'Connect' (Overload) não foram forjados corretamente.");
    }

    [Fact]
    public void Quest07_MotoresDeEstado_This()
    {
        Type tipo = ObterPlantaBaixa("Quest07", "Battery");
        object obj = Activator.CreateInstance(tipo);
        PropertyInfo prop = tipo.GetProperty("Charge");
        MethodInfo motorDrain = tipo.GetMethod("Drain");

        Assert.Equal(100, prop.GetValue(obj)); // Deve nascer com 100

        motorDrain.Invoke(obj, new object[] { 30 });
        Assert.Equal(70, prop.GetValue(obj));

        motorDrain.Invoke(obj, new object[] { 100 }); // Dreno letal
        Assert.Equal(0, prop.GetValue(obj)); // A trava impede de cair abaixo de 0
    }

    [Fact]
    public void Quest08_MenteColetiva_Static()
    {
        Type tipo = ObterPlantaBaixa("Quest08", "Drone");
        PropertyInfo prop = tipo.GetProperty("FleetCount", BindingFlags.Public | BindingFlags.Static);
        if (prop == null) throw new ForjaException("[FALHA DE ASSINATURA] A propriedade FleetCount não é public static.");

        int frotaInicial = (int)prop.GetValue(null);
        Activator.CreateInstance(tipo);
        Activator.CreateInstance(tipo);

        Assert.Equal(frotaInicial + 2, prop.GetValue(null));
    }

    [Fact]
    public void Quest09_MotorDeCombustao_BossFight()
    {
        Type tipo = ObterPlantaBaixa("Quest09", "CombustionEngine");
        object obj = Activator.CreateInstance(tipo, new object[] { 20, 10 }); // Fuel: 20, Oil: 10

        MethodInfo motorAcelerar = tipo.GetMethod("Accelerate");

        // Primeira aceleração: Gasta 10 Fuel, 5 Oil (Sobram 10 Fuel, 5 Oil)
        motorAcelerar.Invoke(obj, null);
        Assert.Equal(10, tipo.GetProperty("Fuel").GetValue(obj));
        Assert.Equal(5, tipo.GetProperty("Oil").GetValue(obj));

        // Segunda aceleração: Gasta 10 Fuel, 5 Oil (Sobram 0 Fuel, 0 Oil)
        motorAcelerar.Invoke(obj, null);
        Assert.Equal(0, tipo.GetProperty("Fuel").GetValue(obj));
        Assert.Equal(0, tipo.GetProperty("Oil").GetValue(obj));

        // Terceira aceleração sem combustível: Ignora.
        motorAcelerar.Invoke(obj, null);

        // Teste Catastrófico: Injetar Fuel (20), mas tentar acelerar com Oil 0.
        tipo.GetMethod("Refuel").Invoke(obj, new object[] { 20 });

        TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => motorAcelerar.Invoke(obj, null));
        Assert.IsType<InvalidOperationException>(ex.InnerException);
        Assert.Equal("CATASTROPHIC FAILURE: NO OIL", ex.InnerException.Message);
    }
}