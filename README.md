# API de Antecipação de Recebíveis

## 📖 Sobre

Esta API foi desenvolvida para apoiar o fluxo de caixa de criadores de conteúdo, oferecendo a opção de antecipação de recebíveis. Através dela, um criador pode solicitar a liberação antecipada de parte de seus valores futuros, mediante a aplicação de uma taxa.

---

## 🛠️ Tecnologias

O projeto foi construído utilizando as seguintes tecnologias:

-   **.NET 9**

---

## 🚀 Começando

Siga os passos abaixo para executar a aplicação em seu ambiente local.

### Pré-requisitos

-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) instalado.

### Executando a Aplicação

1.  Clone o repositório para a sua máquina.
2.  Navegue até o diretório onde o projeto foi clonado e execute o seguinte comando no seu terminal:

    ```bash
    dotnet Global.Anticipation.API.dll
    ```

3.  Após a execução, a API estará disponível. Você pode acessar a documentação interativa via Swagger no seguinte endereço:
    [https://localhost:7115/swagger](https://localhost:7115/swagger)

---

## 🧪 Testando com Postman

Na raiz do repositório, você encontrará uma pasta `postman` com a collection e os ambientes necessários para testar os endpoints.

```bash
meu-projeto/
├── src/
├── postman/
│   ├
└── ...

```

Importe a collection e o ambiente desejado no seu Postman para começar a testar.

Endpoints da API
Aqui está a lista de endpoints disponíveis na versão 1 da API.

1. Simular Antecipação
Calcula e retorna o valor líquido de uma antecipação com base no valor solicitado.

Método: GET

URL: /api/v1/anticipations/simulate?requestedAmount={valor}

Exemplo:

```bash

curl -X 'GET' \
  'https://localhost:7115/api/v1/anticipations/simulate?requestedAmount=101' \
  -H 'accept: */*' \
  -H 'api-version: 1'

```

2. Criar Solicitação de Antecipação
Cria uma nova solicitação de antecipação para um criador.

Método: POST

URL: /api/v1/anticipations

Exemplo:

```bash

curl -X 'POST' \
  'https://localhost:7115/api/v1/anticipations' \
  -H 'accept: */*' \
  -H 'api-version: 1' \
  -H 'Content-Type: application/json' \
  -d '{
  "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "requestedAmount": 110,
  "requestDate": "2025-08-20T00:03:51.716Z"
}'

```

3. Aprovar ou Rejeitar Antecipação
Altera o status de uma solicitação de antecipação existente (ex: para "Approved" ou "Rejected").

Método: PATCH

URL: /api/v1/anticipations/{id}/status

Exemplo:

```bash

curl -X 'PATCH' \
  'https://localhost:7115/api/v1/anticipations/74b3fea1-e295-4ec7-bc97-fc095946d57b/status' \
  -H 'accept: */*' \
  -H 'api-version: 1' \
  -H 'Content-Type: application/json' \
  -d '{
  "status": "Approved"
}'

```

4. Obter Antecipação por ID
Busca os detalhes de uma solicitação de antecipação específica pelo seu ID.

Método: GET

URL: /api/v1/anticipations/{id}

Exemplo:

```bash

curl -X 'GET' \
  'https://localhost:7115/api/v1/anticipations/74b3fea1-e295-4ec7-bc97-fc095946d57b' \
  -H 'accept: */*' \
  -H 'api-version: 1'

```

5. Listar Antecipações por Criador
Retorna uma lista de todas as solicitações de antecipação feitas por um criador específico.

Método: GET

URL: /api/v1/creators/{creatorId}/anticipations

Exemplo:

```bash

curl -X 'GET' \
  'https://localhost:7115/api/v1/creators/3fa85f64-5717-4562-b3fc-2c963f66afa6/anticipations' \
  -H 'accept: */*' \
  -H 'api-version: 1'

```
