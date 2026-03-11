# Guia de Autorização JWT para ColetaJá API

Para acessar os endpoints protegidos da API ColetaJá, você precisará de um **JSON Web Token (JWT)**. Este token é obtido após um login ou registro bem-sucedido e deve ser incluído no cabeçalho de `Authorization` de suas requisições.

## 1. Obtendo o Token JWT

Primeiro, você precisa fazer uma requisição para o endpoint de login ou registro:

-   **Registro:** `POST /api/Auth/register`
-   **Login:** `POST /api/Auth/login`

Ambos os endpoints retornarão um objeto JSON contendo o token JWT, similar a este:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
  "user": {
    "id": 1,
    "name": "João Silva",
    "email": "joao@email.com",
    "type": "Paciente"
  }
}
```

Copie o valor da propriedade `token` (a string longa que começa com `eyJ...`).

## 2. Usando o Token no Swagger UI

1.  Abra a interface do Swagger UI da sua API (geralmente em `http://localhost:5000/swagger`).
2.  Localize o botão **"Authorize"** (ou um ícone de cadeado) no canto superior direito da página.
3.  Clique no botão "Authorize". Uma janela pop-up será exibida.
4.  No campo de valor, insira o token JWT que você copiou, **precedido pela palavra `Bearer` e um espaço**. Por exemplo:
    ```
    Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
    ```
5.  Clique em **"Authorize"** e depois em **"Close"**.

Agora, todas as requisições que você fizer através do Swagger UI incluirão automaticamente o cabeçalho de autorização com o seu token.

## 3. Usando o Token em Requisições `curl`

Ao usar `curl`, você **deve** incluir o prefixo `Bearer` seguido de um espaço antes do token. O formato correto é:

```bash
curl -X 'GET' \
  'http://localhost:5152/api/Auth/me' \
  -H 'accept: text/plain' \
  -H 'Authorization: Bearer SEU_TOKEN_JWT_AQUI'
```

**Exemplo corrigido para sua requisição:**

```bash
curl -X 'GET' \
  'http://localhost:5152/api/Auth/me' \
  -H 'accept: text/plain' \
  -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwiZW1haWwiOiJqb3NlMTIzQGdtYWlsLmNvbSIsInJvbGUiOiJQYWNpZW50ZSIsIm5iZiI6MTc3MzI2NTM4NSwiZXhwIjoxNzczMzUxNzg1LCJpYXQiOjE3NzMyNjUzODUsImlzcyI6IkNvbGV0YUphQXBpIiwiYXVkIjoiQ29sZXRhSmFBcHAifQ.Ni903hb7AEB3wIjaNQo_EDW-RMwJjHPJCXWBrFn27eE'
```

---

## 4. Usando o Token no Postman

1.  Abra o Postman e crie uma nova requisição.
2.  Vá para a aba **"Authorization"**.
3.  No dropdown **"Type"**, selecione **"Bearer Token"**.
4.  No campo **"Token"** (ou "Token Value"), cole o token JWT que você copiou (não é necessário adicionar `Bearer` aqui, o Postman fará isso automaticamente).
5.  Agora, ao enviar a requisição, o Postman incluirá o cabeçalho `Authorization: Bearer [seu_token]` automaticamente.

--- 

Lembre-se de que o token JWT tem um tempo de expiração. Se você receber um erro de `401 Unauthorized` ou `403 Forbidden` após um tempo, pode ser necessário fazer login novamente para obter um novo token.
