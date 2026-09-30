# SGI-JMC · Frontend

Frontend em **React 19 + TypeScript + Vite**, consumindo a `SGI-JMC.Api`.

## Tema visual

As cores da interface são definidas como variáveis CSS em `src/styles/theme.css`:

| Token | Uso |
|---|---|
| `--navy-900` / `--navy-950` | Sidebar e tela de login (fundo escuro) |
| `--blue-600` | Cor primária (botões, links, foco) |
| `--blue-400` | Acento claro (destaques, hover) |
| `--bg-app` | Fundo geral da aplicação (cinza-azulado bem claro) |
| `--bg-surface` | Cards, tabelas, modais (branco) |

Tipografia: **Sora** para títulos, **Inter** para textos e interface e
**IBM Plex Mono** para códigos de matrícula e aluno.

## Como rodar

```bash
npm install
cp .env.example .env   # ajuste VITE_API_URL para a URL da sua API
npm run dev
```

Acesse `http://localhost:5173`. Login padrão do seed da API:
`admin@sgi-jmc.local` / `admin123`.

## Estrutura

```
src/
  api/          # cliente HTTP e módulos de acesso à API
  components/   # componentes reutilizáveis
  context/      # contexto de autenticação
  layouts/      # estrutura e navegação da aplicação
  pages/        # páginas dos módulos
  routes/       # rotas e proteção de acesso
  styles/       # tema e estilos da interface
```

## Funcionalidades disponíveis

- Login com JWT (persistido em `localStorage`, anexado automaticamente nas
  requisições via interceptor do axios).
- Ativação de conta e painel inicial.
- Páginas de Alunos, Declarações, Horários, Ocorrências, Ofícios, Professores,
  Turmas e Administração.
