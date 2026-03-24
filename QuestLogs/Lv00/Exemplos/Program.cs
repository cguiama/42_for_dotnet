// =====================================================================
// LEVEL 00 - QUEST 00: O CHASSI DE CONTENÇÃO
// Arquivo: Program.cs
// Objetivo: Estabelecer o ponto de ignição físico para o compilador Roslyn.
// =====================================================================

// 1. A FÍSICA DO NAMESPACE:
// Na RAM, milhares de bibliotecas são carregadas simultaneamente pela CLR. 
// O 'namespace' cria um perímetro de blindagem lógica no arquivo binário (.dll).
// Se o sistema carregar outra biblioteca que também possua uma classe 'Program', 
// o compilador não sofrerá colapso por colisão de memória, pois o endereço absoluto 
// da sua estrutura será 'Lv00.Program'.
namespace Lv00;

// 2. A FÍSICA DA CLASSE:
// Diferente da linguagem C, no .NET nenhum código executa solto no vácuo da memória. 
// O compilador exige um contêiner estrutural (uma 'class' ou 'struct') para gerar 
// a tabela de metadados no Assembly.
// O modificador 'public' quebra o isolamento padrão, garantindo que agentes externos 
// (como o nosso motor de testes, o Scouter) consigam enxergar este chassi.
public class Program
{
    // 3. A FÍSICA DO PONTO DE IGNIÇÃO (MAIN):
    // Este é o 'Entry Point' do ecossistema. Quando o sistema operacional (Windows/Linux) 
    // executa a aplicação, ele varre a .dll procurando especificamente por esta assinatura.
    // 
    // - 'static': Aterra o método em um endereço fixo da memória (High Frequency Heap). 
    //   Isso significa que o SO não precisa gastar energia instanciando um objeto 'new Program()' 
    //   para conseguir invocar o método.
    // - 'void': Determina que, ao encerrar a esteira, a função não devolverá matéria (dados)
    //   para o processo que a chamou.
    public static void Main()
    {
        // [LABORATÓRIO DE TESTES LOCAIS]
        // No exato momento em que a agulha de leitura entra nesta chave '{', o sistema operacional 
        // empilha o primeiro 'Frame' de execução na Stack da sua máquina.

        // O Scouter ignora completamente a execução deste bloco. 
        // Use esta área livre de auditoria para invocar os motores que você forjará nas próximas Quests.

        // Exemplo de invocação de teste (descomente após forjar a Quest 01):
        // Lv00.Quest01.PrintChar('G');
    }
}