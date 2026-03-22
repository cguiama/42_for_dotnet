# FORMAÇÃO ENGENHARIA .NET
Estou construindo este curso para que eu possa continuar aprendendendo e também ajudar a qualquer um que vier a ter interesse em aprender .NET através de construção manual e adoção de ferramentas nativas.

## 1. O MOTOR DE EXECUÇÃO (A JORNADA)
Cada Level do curso é estritamente dividido em duas fases:
* **A Forja:** O ambiente hostil. Bibliotecas padrão e magias de framework (LINQ, Entity Framework, ASP.NET) são bloqueados. O jogador constrói a estrutura de dados e a arquitetura do zero, lidando com alocação na RAM, matemática bruta e a física do C# puro. O *Scouter* (Testes Automatizados xUnit) explodirá o build se detectar ferramentas proibidas.
* **O Arsenal:** A liberação das chaves da Microsoft. Após provar que entende a engenharia subjacente, o jogador descarta o motor manual e equipa a ferramenta nativa de mercado (ex: troca o laço manual pelo `.Where()`, troca o SQL puro pelo EF Core) consolidando a produtividade.

## 2. ARCO 1: A FUNDAÇÃO CLR (Stack vs Heap)
O foco é a memória e a fluência na sintaxe C#. Zero banco de dados. Zero internet.
* **Lv00 - A Física da Máquina:** Variáveis, tipos primitivos, tipagem estrita (proibido `var`). Operação 100% na Stack.
* **Lv01 - O Salto para a Heap:** Structs vs Classes. Entendendo o Garbage Collector. Proibido coleções dinâmicas.
* **Lv02 - Blindagem de Estado:** Orientação a Objetos estrita. Modificadores de acesso, validação de estado. Proibido `set` público.
* **Lv03 - A Esteira de Dados:** Construção de coleções dinâmicas na mão. Introdução a ponteiros de função (`Action`, `Func`) e criação de métodos de extensão que imitam o LINQ.
* **Boss Fight (Arco 1):** Construção de uma ferramenta de CLI (Command Line Interface) robusta que processe arquivos, argumentos e trate exceções sem travar a thread principal.

## 3. ARCO 2: ENGENHARIA DE SISTEMAS (Arquitetura e Nuvem)
O foco é a infraestrutura distribuída, escalabilidade e integração de IA.
* **Lv04 - I/O de Alta Tensão:** Banco de dados relacional. Conexão TCP pura. Proibido ORMs. Escrita de SQL manual via Dapper/ADO.NET.
* **Lv05 - A Rede Restful:** Protocolo HTTP, Controllers, Injeção de Dependência na mão, JWT e Middlewares.
* **Lv06 - A Fronteira Térmica (DDD & Clean Arch):** Desacoplamento. Isolar o Domínio (Core) da Infraestrutura. Inversão de controle total.
* **Lv07 - Separação de Vias (CQRS):** Rasgar o sistema de leitura e escrita. Implementação do MediatR.
* **Lv08 - Escala e Contêineres:** Dockerização da aplicação. Introdução de filas assíncronas (RabbitMQ) e cache de memória (Redis).
* **Lv09 - Injeção Cognitiva (IA Pura):** Conexão bruta via `HttpClient` com APIs de LLMs. Gestão de latência e tokens.
* **Lv10 - Geometria de Dados (RAG):** Vetorização de texto (Embeddings). Bancos de dados vetoriais (pgvector/Redis) e busca por similaridade.
* **Lv11 - Orquestração (Semantic Kernel):** Transformação dos Comandos do C# em agentes de IA. Automação de intenção do jogador.
* **Lv12 - A Interface do Usuário (Blazor):** Ciclo de vida de componentes no navegador (WebAssembly) ou no servidor (Server-side). Gerenciamento de estado visual e consumo assíncrono da API RESTful forjada no Lv05.
* **Final Boss:** A linha de montagem automatizada. O deploy manual é estritamente proibido. O jogador deve escrever um pipeline YAML de CI/CD (GitHub Actions). O servidor deve baixar o código, compilar, rodar as validações do Domínio e injetar os contêineres em produção automaticamente no `git push`. Se um único teste do Scouter falhar, o lançamento é abortado.