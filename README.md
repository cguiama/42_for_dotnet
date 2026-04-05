# FORMAÇÃO ENGENHARIA .NET
Estou construindo este roadmap para que eu possa continuar aprendendendo e também ensinar a qualquer um que vier a ter interesse em aprender .NET através de construção manual de conceitos fundamentais, com acesso aberto via GitHub, visando democratizar o ensino de programação.

## EQUIPANDO O ARSENAL (PRÉ-REQUISITOS)

Para entrar na Forja, o seu hardware precisa reconhecer a matriz de comandos do ecossistema Microsoft. O compilador e a máquina virtual (CLR) não vêm embutidos no sistema operacional por padrão.

### 1. O Motor Principal (.NET SDK 10)
Você precisa obrigatoriamente do **Software Development Kit (SDK)**, e não apenas do *Runtime*. O SDK contém o compilador Roslyn e as ferramentas de linha de comando (CLI) necessárias para construir a aplicação e rodar o nosso motor de testes (o Scouter).

* [Download Oficial do .NET SDK 10 (Windows, macOS, Linux)](https://dotnet.microsoft.com/download/dotnet/10.0)
* *Nota:* Baixe o instalador correspondente à arquitetura do seu processador (x64 ou ARM64).

### 2. Validação da Infraestrutura
Após a instalação, o SDK deve ser acoplado automaticamente às variáveis de ambiente (Path) da sua máquina. Para validar se a instalação foi bem-sucedida, abra o seu terminal e injete o comando de diagnóstico:

```bash
dotnet --version
```

*A saída térmica no terminal deve retornar algo como `10.0.xxx`. Se o sistema acusar "comando não reconhecido", o SDK não foi mapeado no Path. Reinicie o terminal ou a máquina*

### 3. A Bancada de Engenharia (IDE)
Escrever C# puro sem um analisador sintático é um gasto térmico desnecessário. Equipe um ambiente capaz de ler a árvore do código:
* **Leve e Ágil:** [Visual Studio Code](https://code.visualstudio.com/) + Extensão oficial **C# Dev Kit**.
* **Armaduras Pesadas:** Visual Studio 2022 (Community) ou JetBrains Rider.

## 1. O MOTOR DE EXECUÇÃO (A JORNADA)
Cada Level do curso é estritamente dividido em duas fases:
* **A Forja:** O ambiente hostil. Bibliotecas padrão e magias de framework (LINQ, Entity Framework, ASP.NET) são bloqueados. O aprendiz constrói a estrutura de dados e a arquitetura do zero, lidando com alocação na RAM, matemática bruta e a física do C# puro. O *Scouter* (Testes Automatizados xUnit) explodirá o build se detectar ferramentas proibidas.
* **O Arsenal:** A liberação das chaves da Microsoft. Após provar que entende a engenharia subjacente, o aprendiz descarta o motor manual e equipa a ferramenta nativa de mercado (ex: troca o laço manual pelo `.Where()`, troca o SQL puro pelo EF Core) consolidando a produtividade.

### 2. ARCO 1: A FUNDAÇÃO CLR (Stack vs Heap)
O foco é a memória e a fluência na sintaxe C#. Zero banco de dados. Zero internet.

* **Lv00 - A Física da Máquina:** Variáveis, tipos primitivos, tipagem estrita (proibido `var`). Operação 100% na Stack.
* **Lv01 - O Salto para a Heap:** Entendendo ponteiros e o Garbage Collector. Proibido coleções dinâmicas.
* **Lv02 - Blindagem de Estado:** Structs vs Classes, Entendendo Orientação a Objetos estrita. Modificadores de acesso, validação de estado. Proibido `set` público.
* **Lv03 - A Esteira de Dados:** Construção de coleções dinâmicas na mão. Introdução a ponteiros de função (`Action`, `Func`) e criação de métodos de extensão que imitam o LINQ.
* **Lv04 - Boss Fight (Arco 1):** Construção de uma ferramenta de CLI (Command Line Interface) robusta que processe arquivos, argumentos e trate exceções sem travar a *thread* principal.

### 3. ARCO 2: A CONEXÃO COM A MATRIZ (Arquitetura, Nuvem e IA)
O foco é a infraestrutura distribuída, escalabilidade, persistência avançada e integração cognitiva.

* **Lv05 - I/O de Alta Tensão:** Banco de dados relacional. Conexão TCP pura. Proibido ORMs. Escrita de SQL manual via Dapper/ADO.NET.
* **Lv06 - A Caixa Forte (EF Core):** O desbloqueio do ORM. Mapeamento de entidades, Migrations e a física térmica do *Tracking* na memória.
* **Lv07 - A Rede Restful:** Protocolo HTTP, Controllers, Injeção de Dependência na mão, Middlewares e blindagem de rotas com JWT.
* **Lv08 - A Fronteira Térmica (DDD & Clean Arch):** Desacoplamento. Isolar o Domínio (Core) da Infraestrutura. Inversão de controle total.
* **Lv09 - Separação de Vias (CQRS):** Rasgar o sistema de leitura e escrita. Implementação do MediatR.
* **Lv10 - Escala e Observabilidade:** Dockerização da aplicação. Introdução de filas assíncronas (RabbitMQ), cache de memória (Redis) e injeção de telemetria (Serilog/OpenTelemetry).
* **Lv11 - Injeção Cognitiva (IA Pura):** Conexão bruta via `HttpClient` com APIs de LLMs. Gestão manual de latência e controle de tokens.
* **Lv12 - Geometria de Dados (RAG):** Vetorização de texto (Embeddings). Bancos de dados vetoriais (pgvector/Redis) e busca por similaridade de cossenos.
* **Lv13 - Orquestração Sintética (Semantic Kernel):** Transformação dos motores do C# em agentes de IA. Automação de intenção do aprendiz.
* **Lv14 - A Interface do Usuário (Blazor):** Ciclo de vida de componentes no navegador (WebAssembly) ou no servidor (Server-side). Gerenciamento de estado visual e consumo assíncrono da API RESTful forjada no Lv07.
* **Final Boss - A Linha de Montagem (CI/CD):** A linha de montagem automatizada. O deploy manual é estritamente proibido. O aprendiz deve escrever um pipeline YAML de CI/CD (GitHub Actions). O servidor deve baixar o código, compilar, rodar as validações do Domínio e injetar os contêineres em produção automaticamente no `git push`. Se um único teste do Scouter falhar, o lançamento é abortado.