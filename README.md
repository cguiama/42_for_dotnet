# ⚙️ ARQUITETURA PEDAGÓGICA: FORMAÇÃO .NET (ESTILO 42)
**Objetivo:** Estou construindo este curso para que eu possa continuar aprendendendo e também ajudar a qualquer um que vier a ter interesse em aprender através de construção manual e adoção de ferramentas nativas.

## 1. O MOTOR DE EXECUÇÃO (MÉTODO POR MÓDULO)
Cada módulo do curso é estritamente dividido em duas fases de engenharia:
* **A Forja:** O ambiente hostil. Bibliotecas padrão e frameworks mágicos (LINQ, Entity Framework, ASP.NET) são bloqueados. O aluno constrói a estrutura de dados e a arquitetura do zero, lidando com alocação na RAM, matemática de ponteiros (quando aplicável) e a física do C# puro.
* **O Arsenal:** A liberação das chaves da Microsoft. Após provar que entende a engenharia subjacente, o aluno descarta o motor manual e aprende a utilizar a ferramenta nativa de mercado (ex: troca o laço manual pelo `.Where()`, troca o SQL puro pelo EF Core) consolidando a produtividade.

## 2. ROADMAP: FORMAÇÃO 1 - A PISCINA DE C# (FUNDAMENTOS E CLR)
O foco é a memória (Stack vs Heap) e a fluência na linguagem C#. Zero banco de dados. Zero internet.
* **C00 - A Física da Máquina:** Variáveis, tipos de dados primitivos, tipagem estrita (proibido `var`).
* **C01 - O Salto para a Heap:** Structs vs Classes. Entendendo o Garbage Collector. Proibido coleções dinâmicas.
* **C02 - Blindagem de Estado:** Orientação a Objetos estrita. Modificadores de acesso, validação de estado. Proibido `set` público.
* **C03 - A Esteira de Dados:** Construção de coleções dinâmicas na mão. Introdução a ponteiros de função (`Action`, `Func`) e criação de métodos de extensão que imitam o LINQ.
* **Boss Fight (Exame F1):** Construção de uma ferramenta de CLI (Command Line Interface) robusta que processe arquivos, argumentos e trate exceções sem travar a thread principal.

## 3. ROADMAP: FORMAÇÃO 2 - ENGENHARIA DE SISTEMAS (ARQUITETURA E NUVEM)
O foco é a infraestrutura distribuída, escalabilidade e integração de IA.
* **M00 - I/O de Alta Tensão:** Banco de dados relacional. Conexão TCP pura. Proibido ORMs. Escrita de SQL manual via Dapper/ADO.NET.
* **M01 - A Rede Restful:** Protocolo HTTP, Controllers, Injeção de Dependência na mão, JWT e Middlewares.
* **M02 - A Fronteira Térmica (DDD & Clean Arch):** Desacoplamento. Isolar o Domínio (Core) da Infraestrutura. Inversão de controle total.
* **M03 - Separação de Vias (CQRS):** Rasgar o sistema de leitura e escrita. Implementação do MediatR.
* **M04 - Escala e Contêineres:** Dockerização da aplicação. Introdução de filas assíncronas (RabbitMQ) e cache de memória (Redis).
* **M05 - IA Pura (Injeção Cognitiva):** Conexão bruta via `HttpClient` com APIs de LLMs. Gestão de latência e tokens.
* **M06 - Geometria de Dados (RAG):** Vetorização de texto (Embeddings). Bancos de dados vetoriais (pgvector/Redis) e busca por similaridade.
* **M07 - Orquestração (Semantic Kernel):** Transformação dos Comandos do C# em agentes de IA. Automação de intenção do usuário.
* **M08 - A Interface do Usuário (Blazor):** Ciclo de vida de componentes no navegador (WebAssembly) ou no servidor (Server-side). Gerenciamento de estado visual e consumo assíncrono da API RESTful forjada no M01.
* **Boss Fight (Exame F2):** A linha de montagem automatizada. O deploy manual é estritamente proibido. O aluno deve escrever um pipeline YAML de CI/CD (GitHub Actions). A esteira da nuvem deve baixar o código, instanciar a CLR, compilar, rodar os testes de domínio e injetar os contêineres em produção automaticamente no `git push`. Se um único teste falhar, o build quebra e o lançamento é abortado.