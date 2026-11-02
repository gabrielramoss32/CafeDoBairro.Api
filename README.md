# ☕ Café do Bairo - API

API desenvolvida como parte de um processo seletivo para a Empresa Júnior de Ciência da Computação da minha universidade, com o objetivo de organizar e facilitar o controle de encomendas recebidas pela cafeteria.

## 💻 Stack utilizada

- C#
- .NET 10
- ASP.NET Core Web API
- MinimalAPIS
- HTTP/JSON
- Git/Github

## 📄 Funcionalidades

- Cadastro de pedidos;
- Listagem de pedidos;
- Edição de pedidos;
- Exclusão de pedidos;
- Validações de pedidos;
- Limite de cinco encomendas por dia.

## ❗ Limites existentes

- Os dados são armazenados apenas na memória durante a execução, ao encerrar a aplicação, eles não são salvos;
- Não há tratamento de erro para entrada de data ou horário inválidos;
- Os produtos disponíveis no catálogo são fixos e não podem ser alterados.

## 🗂️ Estrutura do projeto

```text
CafeDoBairro.Api/
│
├── src/                                   # Pasta-fonte do projeto, com a maior parte das aplicações
│   ├── Config/                            # Pasta com arquivos de configuração JSON
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── Models/                            # Modelos do projeto de classe    
│   │   └── Pedido.cs                      # Declaração da classe e elementos que compõem o pedido
│   │
│   ├── Endpoints/                         # Endpoints do projeto
│   │   └── PedidoEndpoints.cs             # Validações e mapeamento das 4 endpoints do projeto
│   │
│   ├── Properties/                        # Arquivos de execução JSON
│   │   └── launchSettings.json
│   │
│   ├── Program.cs                         # Arquivos de compilação e construção do .NET
│   └── CafeDoBairro.Api.csproj            # Arquivo de versão do .NET
│
├── pedidos.http                           # Base de teste de requisições da API
├── README.md                              # Documentação do projeto
└── .gitignore                             # Gitignore com arquivos de compilação
```

## 🤖 Validações e tratamento de erros

| Validação | Condição para rejeição | Código HTTP |
|---|---|---|
| Campos obrigatórios | Nome, telefone, produto ou status vazios. | 400 – Bad Request |
| Telefone | Não possui exatamente 11 dígitos numéricos ou não possui o número 9 após o DDD. | 400 – Bad Request |
| Data de entrega | Data anterior ao dia atual. | 400 – Bad Request |
| Status | Valor diferente de `Recebida`, `Em preparo` ou `Entregue`. | 400 – Bad Request |
| Produto | Produto não pertence ao catálogo disponível. | 400 – Bad Request |
| Limite diário | Tentativa de cadastrar ou atualizar um pedido quando o dia já possui cinco encomendas. | 409 – Conflict |
| Pedido inexistente | ID não encontrado durante uma atualização ou exclusão. | 404 – Not Found |
| Data de entrega | Não ser informada ou faltar horário. | Sem tratamento |

**Obs1:** O campo status possui valor padrão Recebida. Portanto, quando omitido no JSON, o pedido recebe esse status automaticamente.

**Obs2:** A data de entrega deve obedecer ao formato ISO 8601 (ano-mes-diaThora:minuto:segundo)

## 🥖 Cardápio disponível

| Produto |
|---|
| Café coado |
| Pão de queijo |
| Bolo de chocolate |
| Bolo de cenoura |
| Bolo de milho |
| Bolo de aipim |

Caso o campo "produto" não ser um desses produtos, a API irá tratar como situação inválida, como dito em [🤖 Validações e tratamento de erros](Validações-e-tratamento-de-erros).

## 🖋️ Modelo do pedido

```text
{
  "id": 1,
  "nomeCliente": "Arthur",
  "telefone": "73999990000",
  "produto": "café coado",
  "dataEntrega": "2030-01-10T15:00:00",
  "status": "Recebida"
}
```

## 🪢 Endpoints da API

| Método HTTP | Rota | Descrição | Resposta de sucesso |
|---|---|---|---|
| POST | `/pedidos` | Cadastra um novo pedido. | 201 – Created |
| GET | `/pedidos` | Lista todos os pedidos cadastrados. | 200 – OK |
| PUT | `/pedidos/{id}` | Atualiza os dados de um pedido existente. | 200 – OK |
| DELETE | `/pedidos/{id}` | Exclui um pedido existente. | 200 – OK |

## 📖 Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- Git
- Visual Studio Code (opcional, para utilizar o REST Client).

## 🔧 Como executar

1. Clone o repositório e entre na branch:
```bash
   git clone https://github.com/gabrielramoss32/CafeDoBairro.Api.git
   cd CafeDoBairro.Api
   git checkout refactor/organizacao-pastas
```

2. Restaure as dependências:
```bash
   dotnet restore src/CafeDoBairro.Api.csproj
```

3. Inicie a aplicação:
```bash
   dotnet run --project src/CafeDoBairro.Api.csproj
```
A API estará disponível em **http://localhost:5062**. Para encerrar, pressione `Ctrl + C`.

### Método 1: VS Code + REST Client

1. Instale a extensão REST Client: No Visual Studio Code, acesse a aba de extensões (Ctrl + Shift + X), pesquise por REST Client e instale a extensão desenvolvida por Huachao Mao.

2. Inicie a aplicação: abra o terminal integrado do VS Code e execute:
```bash
   dotnet run --project src/CafeDoBairro.Api.csproj
```
   Mantenha o terminal aberto durante os testes.

3. Envie as requisições
Abra o arquivo pedidos.http, localizado na raiz do projeto.

   A extensão REST Client disponibilizará a opção Send Request acima de cada requisição HTTP.
  
   Clique nessa opção para enviar a requisição e visualizar a resposta da API diretamente no editor.
  
   O arquivo contém exemplos para testar as operações de cadastro, listagem, atualização e exclusão de pedidos, execute Ctrl + C para parar.

### Método 2: Terminal/PowerShell

Com a API já em execução conforme [🔧 Como executar](como-executar), abra um segundo terminal para realizar as requisições HTTP.

1. Cadastrar um pedido (POST)
```bash
    $body = @{
        nomeCliente = "Jose"
        telefone = "73987654321"
        produto = "bolo de chocolate"
        dataEntrega = "2026-11-05T14:00:00"
        status = "Recebida"
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "http://localhost:5062/pedidos" -Method POST -ContentType "application/json" -Body $body
```

2. Listar pedidos (GET)
```bash
    Invoke-RestMethod -Uri "http://localhost:5062/pedidos" -Method GET
```

3. Atualizar um pedido (PUT)
    ```bash
        $body = @{
        nomeCliente = "Jose"
        telefone = "73987654321"
        produto = "bolo de chocolate"
        dataEntrega = "2026-11-06T14:00:00"
        status = "Em preparo"
    } | ConvertTo-Json

    Invoke-RestMethod -Uri "http://localhost:5062/pedidos/{id}" -Method PUT -ContentType "application/json" -Body $body
```

4. Excluir um pedido (DELETE)
```bash
    Invoke-RestMethod -Uri "http://localhost:5062/pedidos/{id}" -Method DELETE
```

**Observações:**
- Mantenha a API em execução no primeiro terminal enquanto realiza os testes no segundo;
- Certifique-se de preencher o id solicitado nas requisições PUT e DELETE.