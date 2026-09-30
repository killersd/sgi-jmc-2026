# SGI-JMC.Api

API REST em **.NET 8** do sistema `SGI-JMC`, com autenticação **JWT** e endpoints
JSON consumidos pelo frontend em React. O acesso a dados usa ASP.NET Core Identity,
Entity Framework Core e **PostgreSQL**.

## Recursos disponíveis

- **Auth** (`/api/auth`): login, registro (somente admin) e perfil do usuário logado.
- **Alunos** (`/api/alunos`): CRUD completo do módulo `AlunoAtual`, com filtros por
  ano/série, turma, texto de busca e paginação.

A API também contém controladores para Configurações, Declarações, Disciplinas,
Horários, Ocorrências, Ofícios, Perfis, Professores, Turmas e Usuários.

## Como rodar

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download).
2. Inicie um servidor **PostgreSQL** local ou use Docker:

   ```bash
   docker run --name sgi-jmc-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16
   ```

3. Ajuste a connection string em `appsettings.json` (`ConnectionStrings:DefaultConnection`)
   com host/porta/usuário/senha/nome do banco do seu Postgres:

   ```
   Host=localhost;Port=5432;Database=sgi-jmc;Username=postgres;Password=postgres
   ```

4. Troque o valor de `Jwt:Key` por uma chave secreta forte (mín. 32 caracteres) — nunca
   suba a chave real para o controle de versão em produção; use `dotnet user-secrets`
   ou variáveis de ambiente.
5. Aplique as migrations existentes ao banco:

   ```bash
   cd SGI-JMC.Api
   dotnet tool install --global dotnet-ef   # se ainda não tiver
   dotnet ef database update
   ```

   No primeiro início, `DbSeeder` cria os papéis `administrador` e `usuario`, além
   do usuário administrador configurado em `AdminSeed` no `appsettings.json`.

6. Rode a API:

   ```bash
   dotnet run
   ```

7. Acesse o Swagger em `https://localhost:7080/swagger` (a porta pode variar —
   veja o console ao rodar `dotnet run`).

## Credenciais de desenvolvimento

- **E-mail:** `admin@sgi-jmc.local`
- **Senha:** `admin123`

> Troque essas credenciais imediatamente em produção (via `appsettings` ou variáveis
> de ambiente `AdminSeed__Email` / `AdminSeed__Senha`).

## CORS
A origem do frontend React é liberada via `Frontend:Origins` em `appsettings.json`
(padrão: `http://localhost:5173`, porta padrão do Vite).

