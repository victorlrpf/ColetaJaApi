# ColetaJá API

API REST desenvolvida em C# (.NET 8) para o aplicativo ColetaJá.

## Tecnologias Utilizadas

- **.NET 8 Web API**
- **Entity Framework Core**
- **PostgreSQL (Supabase)**
- **JWT Authentication**
- **BCrypt.Net-Next** (Hash de senhas)
- **Swagger/OpenAPI**

## Configuração do Banco de Dados

1. Crie um projeto no [Supabase](https://supabase.com/).
2. Obtenha a string de conexão do PostgreSQL em `Project Settings > Database`.
3. Atualize o arquivo `appsettings.json` com a sua string de conexão:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=db.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=SUA_SENHA_AQUI"
}
```

## Como Executar

1. Certifique-se de ter o .NET 8 SDK instalado.
2. Instale a ferramenta do Entity Framework:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
3. Execute as migrations para criar as tabelas no banco de dados:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
4. Inicie a aplicação:
   ```bash
   dotnet run
   ```
5. Acesse o Swagger em: `http://localhost:5000/swagger` (ou a porta configurada).

## Endpoints Principais

### Autenticação
- `POST /api/Auth/register`: Cadastro de novos usuários (Paciente, Coletador, Laboratório).
- `POST /api/Auth/login`: Login e obtenção do token JWT.
- `GET /api/Auth/me`: Informações do usuário logado.

### Paciente
- `POST /api/Patient/address`: Cadastrar endereço.
- `GET /api/Patient/addresses`: Listar endereços.
- `GET /api/Patient/exam-types`: Listar tipos de exames disponíveis.
- `POST /api/Patient/request-exam`: Solicitar um novo exame.
- `GET /api/Patient/my-exams`: Acompanhar status e ver resultados.

### Coletador
- `GET /api/Collector/available-requests`: Ver solicitações pendentes.
- `POST /api/Collector/accept/{id}`: Aceitar uma coleta.
- `POST /api/Collector/update-status/{id}`: Atualizar status da coleta.

### Laboratório
- `GET /api/Laboratory/pending-exams`: Ver exames recebidos/coletados.
- `POST /api/Laboratory/receive/{id}`: Confirmar recebimento do exame.
- `POST /api/Laboratory/insert-result/{id}`: Inserir resultado e finalizar.
