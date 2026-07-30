# SGI-JMC · Frontend

Frontend em **React 19 + TypeScript + Vite**, consumindo a `SGI-JMC.Api`.

## Paleta de cores

Azul moderno, sóbrio (não preto/navy pesado), definida em `src/styles/theme.css`
como variáveis CSS:

| Token | Uso |
|---|---|
| `--navy-900` / `--navy-950` | Sidebar e tela de login (fundo escuro) |
| `--blue-600` | Cor primária (botões, links, foco) |
| `--blue-400` | Acento claro (destaques, hover) |
| `--bg-app` | Fundo geral da aplicação (cinza-azulado bem claro) |
| `--bg-surface` | Cards, tabelas, modais (branco) |

Tipografia: **Sora** (títulos) + **Inter** (corpo/UI) + **IBM Plex Mono** (códigos
de matrícula/aluno).

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
  api/          # client axios + chamadas (auth, alunos)
  components/   # componentes reutilizáveis (modal de aluno, badge, confirm dialog)
  context/       # AuthContext (JWT, usuário logado)
  layouts/       # layout com sidebar + navegação
  pages/         # Login, Painel, Alunos
  routes/        # proteção de rotas autenticadas
  styles/        # tokens de design (theme.css) + estilos utilitários (ui.css)
```

## Módulos implementados neste MVP
- Login com JWT (persistido em `localStorage`, anexado automaticamente nas
  requisições via interceptor do axios).
- Painel inicial.
- Alunos: listagem com busca/filtro (ano/série, turma), paginação, criação, edição
  e exclusão (exclusão restrita ao papel `administrador`).

## Próximos passos sugeridos
- Repetir o padrão de `pages/AlunosPage.tsx` + `api/alunos.ts` para os demais
  módulos (Declarações, Ofícios, Horários, Contratos...).
- Upload de foto do aluno (campo `UrlFoto` já existe no modelo).
- Tela de administração de usuários (papéis, redefinição de senha).
