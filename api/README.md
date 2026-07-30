# SGI-JMC.Api

API REST em **.NET 8** migrada do projeto MVC original (`SGI-JMC`), usando os mesmos
modelos de dados (Identity + EF Core, agora sobre **PostgreSQL**) porém expondo
endpoints JSON com autenticação via **JWT**, para serem consumidos pelo frontend em React.

## Módulos migrados neste MVP
- **Auth** (`/api/auth`): login, registro (somente admin) e perfil do usuário logado.
- **Alunos** (`/api/alunos`): CRUD completo do módulo `AlunoAtual`, com filtros por
  ano/série, turma, texto de busca e paginação.

Os demais ~28 controllers do sistema original (Declarações, Ofícios, Horários,
Contratos, Advertências, etc.) seguem o mesmo padrão e podem ser migrados na
sequência — o `AlunosController` serve de referência de arquitetura.

## Como rodar

1. Instale o [.NET 8 SDK](https://dotnet.microsoft.com/download).
2. Tenha um **PostgreSQL** rodando localmente. Se não tiver instalado, o jeito
   mais rápido é via Docker:

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
5. Crie a migration inicial e o banco:

   ```bash
   cd SGI-JMC.Api
   dotnet tool install --global dotnet-ef   # se ainda não tiver
   dotnet ef migrations add Inicial
   dotnet ef database update
   ```

   (A `DbSeeder` cria as roles `administrador`/`usuario` e um usuário admin padrão
   automaticamente no primeiro start — veja `AdminSeed` em `appsettings.json`.)

6. Rode a API:

   ```bash
   dotnet run
   ```

7. Acesse o Swagger em `https://localhost:7080/swagger` (a porta pode variar —
   veja o console ao rodar `dotnet run`).

## Login padrão (seed)
- **E-mail:** `admin@sgi-jmc.local`
- **Senha:** `admin123`

> Troque essas credenciais imediatamente em produção (via `appsettings` ou variáveis
> de ambiente `AdminSeed__Email` / `AdminSeed__Senha`).

## CORS
A origem do frontend React é liberada via `Frontend:Origins` em `appsettings.json`
(padrão: `http://localhost:5173`, porta padrão do Vite).

## Próximos passos sugeridos
- Migrar os demais controllers (Declaracao*, Oficio*, Horario*, Contratos, etc.)
  seguindo o padrão: Model → DTOs → Controller `[ApiController]` com `[Authorize]`.
- Geração de PDF: os controllers originais usam `iText7`/`PdfSharpCore` — pode-se
  manter a geração no backend (endpoint retornando `FileContentResult`) ou mover
  para o frontend.
- Versionamento de API (`/api/v1/...`) se o sistema crescer.
