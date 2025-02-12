# Developer Evaluation Project

**Thiago Augusto Borges**
<br/>
Disponível em [GitHub](https://github.com/Wenemy/thiago-borges-developer-evaluation)

## Use Case
**DeveloperStore Team - Sale API**

### Sale API Fields

| Field           | Type               | Description                                                                 |
|-----------------|--------------------|---------------------------------------------------------------------------|
| **`saleNumber`** | `string`           | Número único da venda.                                                    |
| **`saleDate`**   | `string (date-time)`| Data e hora da venda.                                                     |
| **`customer`**   | `object`           | Informações do cliente.                                                   |
| → `customerId`   | `string (UUID)`    | Identificador único do cliente (External Identity).                        |
| → `name`         | `string`           | Nome do cliente (denormalizado).                                          |
| → `email`        | `string`           | E-mail do cliente (denormalizado).                                        |
| **`branch`**     | `object`           | Informações da filial.                                                    |
| → `branchId`     | `string (UUID)`    | Identificador único da filial (External Identity).                         |
| → `name`         | `string`           | Nome da filial (denormalizado).                                           |
| → `address`      | `string`           | Endereço da filial (denormalizado).                                       |
| **`items`**      | `array`            | Lista de itens da venda.                                                  |
| → `productId`    | `string (UUID)`    | Identificador único do produto (External Identity).                        |
| → `quantity`     | `integer`          | Quantidade do produto.                                                    |
| → `unitPrice`    | `number`           | Preço unitário do produto.                                                |
| → `discount`     | `number`           | Desconto aplicado ao item (calculado com base nas regras de negócio).      |
| → `totalAmount`  | `number`           | Valor total do item (`quantity * unitPrice - discount`).                   |
| **`totalAmount`**| `number`           | Valor total da venda (soma dos valores dos itens).                         |
| **`isCancelled`**| `boolean`          | Indica se a venda foi cancelada.                                          |

---

### Observações

1. **External Identities**:
   - `customerId`, `branchId` e `productId` são identificadores externos, referenciando entidades de outros domínios.

2. **Denormalização**:
   - Campos como `customer.name`, `customer.email`, `branch.name` e `branch.address` são denormalizados.

3. **Regras de Negócio**:
   - Descontos são aplicados automaticamente com base na quantidade de itens:
     - **4+ itens**: 10% de desconto.
     - **10-20 itens**: 20% de desconto.
     - **Mais de 20 itens**: Não permitido (lança uma exceção).
   - O valor total de cada item (`totalAmount`) é calculado como `quantity * unitPrice - discount`.

4. **Cancelamento de Venda**:
   - Uma venda pode ser cancelada por meio do endpoint `PATCH /sales/{id}/cancel`.
   - O campo `isCancelled` é atualizado para `true` quando uma venda é cancelada.

5. **Paginação e Ordenação**:
   - O endpoint `GET /sales` suporta paginação e ordenação:
     - `_page`: Número da página (padrão: 1).
     - `_size`: Número de itens por página (padrão: 10).
     - `_order`: Ordenação dos resultados (exemplo: `saleNumber desc, saleDate asc`).

6. **Validações**:
   - Todos os campos obrigatórios são validados antes de processar a requisição.
   - Quantidades inválidas (ex.: acima de 20 itens) resultam em erro.

---

### Sales

#### GET /sales
- Description: Retrieve a list of all sales
- Query Parameters:
  - `_page` (optional): Page number for pagination (default: 1)
  - `_size` (optional): Number of items per page (default: 10)
  - `_order` (optional): Ordering of results (e.g., "saleNumber desc, saleDate asc")
- Response: 
  ```json
  {
  "data": [
    {
      "id": "string (UUID)",
      "saleNumber": "string",
      "saleDate": "string (date-time)",
      "customer": {
        "customerId": "string (UUID)",
        "name": "string",
        "email": "string"
      },
      "branch": {
        "branchId": "string (UUID)",
        "name": "string",
        "address": "string"
      },
      "items": [
        {
          "productId": "string (UUID)",
          "quantity": "integer",
          "unitPrice": "number",
          "discount": "number",
          "totalAmount": "number"
        }
      ],
      "totalAmount": "number",
      "isCancelled": "boolean"
    }
  ],
  "totalItems": "integer",
  "currentPage": "integer",
  "totalPages": "integer"
  }
  ```


#### POST /sales
- Description: Create a new sale
- Request Body:
  ```json
  {
  "saleNumber": "string",
  "saleDate": "string (date-time)",
  "customerId": "string (UUID)",
  "branchId": "string (UUID)",
  "items": [
      {
         "productId": "string (UUID)",
         "quantity": "integer",
         "unitPrice": "number"
      }
    ]
  }
  ```
- Response: 
  ```json
  {
   "id": "string (UUID)",
   "saleNumber": "string",
   "saleDate": "string (date-time)",
   "customer": {
      "customerId": "string (UUID)",
      "name": "string",
      "email": "string"
   },
   "branch": {
      "branchId": "string (UUID)",
      "name": "string",
      "address": "string"
   },
   "items": [
      {
         "productId": "string (UUID)",
         "quantity": "integer",
         "unitPrice": "number",
         "discount": "number",
         "totalAmount": "number"
      }
   ],
   "totalAmount": "number",
   "isCancelled": "boolean"
  }
  ```

#### GET /sales/{id}
- Description: Retrieve a specific sale by ID
- Path Parameters:
  - `id`: Sale ID (UUID)
- Response: 
  ```json
  {
   "id": "string (UUID)",
   "saleNumber": "string",
   "saleDate": "string (date-time)",
   "customer": {
      "customerId": "string (UUID)",
      "name": "string",
      "email": "string"
   },
   "branch": {
      "branchId": "string (UUID)",
      "name": "string",
      "address": "string"
   },
   "items": [
      {
         "productId": "string (UUID)",
         "quantity": "integer",
         "unitPrice": "number",
         "discount": "number",
         "totalAmount": "number"
      }
   ],
   "totalAmount": "number",
   "isCancelled": "boolean"
  }
  ```

#### PUT /sales/{id}
- Description: Update a specific sale
- Path Parameters:
  - `id`: Sale ID (UUID)
- Request Body:
  ```json
  {
   "saleNumber": "string",
   "saleDate": "string (date-time)",
   "customerId": "string (UUID)",
   "branchId": "string (UUID)",
   "items": [
         {
            "productId": "string (UUID)",
            "quantity": "integer",
            "unitPrice": "number"
         }
      ]
   }
  ```
- Response: 
  ```json
  {
   "id": "string (UUID)",
   "saleNumber": "string",
   "saleDate": "string (date-time)",
   "customer": {
      "customerId": "string (UUID)",
      "name": "string",
      "email": "string"
   },
   "branch": {
      "branchId": "string (UUID)",
      "name": "string",
      "address": "string"
   },
   "items": [
      {
         "productId": "string (UUID)",
         "quantity": "integer",
         "unitPrice": "number",
         "discount": "number",
         "totalAmount": "number"
      }
   ],
   "totalAmount": "number",
   "isCancelled": "boolean"
   }
  ```

#### DELETE /sales/{id}
- Description: Delete a specific sale
- Path Parameters:
  - `id`: Sale ID (UUID)
- Response: 
  ```json
  {
    "message": "string"
  }
  ```

#### PATCH /sales/{id}/cancel
- Description: Cancel a specific sale
- Path Parameters:
  - `id`: Sale ID (UUID)
- Response: 
  ```json
  {
   "id": "string (UUID)",
   "saleNumber": "string",
   "saleDate": "string (date-time)",
   "customer": {
      "customerId": "string (UUID)",
      "name": "string",
      "email": "string"
   },
   "branch": {
      "branchId": "string (UUID)",
      "name": "string",
      "address": "string"
   },
   "items": [
      {
         "productId": "string (UUID)",
         "quantity": "integer",
         "unitPrice": "number",
         "discount": "number",
         "totalAmount": "number"
      }
   ],
   "totalAmount": "number",
   "isCancelled": "boolean"
  }
  ```