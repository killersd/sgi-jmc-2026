# SGI-JMC — API (C#) + Frontend (React)

Este pacote contém a divisão do sistema **SGI-JMC** original (ASP.NET Core MVC
monolítico) em:

- **`api/`** — API REST em .NET 8 com autenticação JWT (`SGI-JMC.Api`)
- **`frontend/`** — SPA em React + TypeScript + Vite, com paleta azul moderna

## Status: MVP validado

Para garantir que a arquitetura e o visual estejam certos antes de migrar os
~28 módulos restantes do sistema original, este MVP cobre:

1. **Autenticação** (login com JWT, papéis `administrador`/`usuario`)
2. **Módulo Alunos** completo (listar com filtros/paginação, criar, editar, excluir)
3. **Layout e tema** definitivos (sidebar, cores, tipografia)

Veja o `README.md` de cada pasta para instruções de instalação e execução.

## Rodar no VS Code

**Pré-requisitos** (instale antes de abrir o projeto):
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org)
- [PostgreSQL](https://www.postgresql.org/download/) local, ou via Docker:
  ```bash
  docker run --name sgi-jmc-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16
  ```
- Extensão **C# Dev Kit** no VS Code (o `.vscode/extensions.json` já sugere ela ao abrir a pasta)

**Passo a passo:**

1. Abra a pasta `sgi-jmc` inteira no VS Code (`code sgi-jmc`) — isso já traz as
   tasks e o launch configurados.
2. Ajuste `api/SGI-JMC.Api/appsettings.json`: connection string do PostgreSQL e
   a chave `Jwt:Key`.
3. Abra a paleta de comandos (`Ctrl+Shift+P` / `Cmd+Shift+P`) → **Tasks: Run Task**
   → `API: restore`.
4. No terminal integrado (`Ctrl+` `` ` ``), rode as migrations (só na primeira vez):
   ```bash
   cd api/SGI-JMC.Api
   dotnet tool install --global dotnet-ef   # se ainda não tiver
   dotnet ef migrations add Inicial
   dotnet ef database update
   ```
5. Pressione **F5** (usa o `launch.json`) para subir a API com debug — ou rode a
   task `API: rodar (watch)`. O Swagger abre em algo como
   `https://localhost:7080/swagger`.
6. Em outro terminal, rode a task `Frontend: instalar dependências` e depois
   `Frontend: rodar (dev)` (ou simplesmente `cd frontend && npm install && npm run dev`).
7. Acesse `http://localhost:5173`, faça login com `admin@sgi-jmc.local` / `admin123`.

Também dá pra rodar tudo de uma vez com a task **"Rodar tudo (API + Frontend)"**.

> O visual do frontend já foi validado de forma isolada (prints em anexo na
> conversa) — sem a API rodando, a tela de Alunos aparece com um aviso de erro
> ao carregar os dados, o que é esperado.

## Arquitetura

```
Frontend (React/Vite, porta 5173)
        │  HTTP + JWT no header Authorization
        ▼
API (.NET 8, ASP.NET Core Web API)
        │  Entity Framework Core (Npgsql)
        ▼
PostgreSQL
```

## Próximos passos

Depois de validar este MVP, o próximo lote de trabalho é migrar os módulos
restantes (Declarações, Ofícios, Horários, Contratos, Advertências, Comunicados
etc.), sempre seguindo o mesmo padrão usado em `AlunosController` /
`AlunosPage.tsx`. Me avise quando quiser seguir para o próximo lote — posso
priorizar pelos módulos mais usados no dia a dia da secretaria.
