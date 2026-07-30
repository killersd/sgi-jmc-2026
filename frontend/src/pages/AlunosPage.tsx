import { useEffect, useState } from "react";
import { listarAlunos, criarAluno, atualizarAluno, removerAluno } from "../api/alunos";
import type { Aluno, AlunoFormValues } from "../types";
import { useAuth } from "../context/AuthContext";
import CodigoBadge from "../components/CodigoBadge";
import AlunoFormModal from "../components/AlunoFormModal";
import ConfirmDialog from "../components/ConfirmDialog";
import "./AlunosPage.css";

const PAGE_SIZE = 10;

export default function AlunosPage() {
  const { isAdmin } = useAuth();
  const [alunos, setAlunos] = useState<Aluno[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [busca, setBusca] = useState("");
  const [anoSerie, setAnoSerie] = useState("");
  const [turma, setTurma] = useState("");
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const [modalAberto, setModalAberto] = useState(false);
  const [alunoEmEdicao, setAlunoEmEdicao] = useState<Aluno | null>(null);
  const [alunoParaExcluir, setAlunoParaExcluir] = useState<Aluno | null>(null);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      const resultado = await listarAlunos({
        busca: busca || undefined,
        anoSerie: anoSerie ? Number(anoSerie) : undefined,
        turma: turma || undefined,
        page,
        pageSize: PAGE_SIZE,
      });
      setAlunos(resultado.items);
      setTotalCount(resultado.totalCount);
    } catch {
      setErro("Não foi possível carregar a lista de alunos.");
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page]);

  function handleFiltrar(event: React.FormEvent) {
    event.preventDefault();
    setPage(1);
    carregar();
  }

  function abrirNovo() {
    setAlunoEmEdicao(null);
    setModalAberto(true);
  }

  function abrirEdicao(aluno: Aluno) {
    setAlunoEmEdicao(aluno);
    setModalAberto(true);
  }

  async function salvar(valores: AlunoFormValues) {
    if (alunoEmEdicao) {
      await atualizarAluno(alunoEmEdicao.id, valores);
    } else {
      await criarAluno(valores);
    }
    setModalAberto(false);
    await carregar();
  }

  async function confirmarExclusao() {
    if (!alunoParaExcluir) return;
    await removerAluno(alunoParaExcluir.id);
    setAlunoParaExcluir(null);
    await carregar();
  }

  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  return (
    <div>
      <header className="page-header page-header--row">
        <div>
          <h1>Alunos</h1>
          <p>{totalCount} aluno(s) cadastrado(s)</p>
        </div>
        <button className="btn btn--primary" onClick={abrirNovo}>
          + Novo aluno
        </button>
      </header>

      <form className="alunos-filtros" onSubmit={handleFiltrar}>
        <label className="field alunos-filtros__busca">
          <span>Buscar por nome</span>
          <input value={busca} onChange={(e) => setBusca(e.target.value)} placeholder="Ex: Maria Silva" />
        </label>
        <label className="field">
          <span>Ano/série</span>
          <select value={anoSerie} onChange={(e) => setAnoSerie(e.target.value)}>
            <option value="">Todas</option>
            {[2, 3, 4, 5, 6, 7, 8, 9].map((ano) => (
              <option key={ano} value={ano}>{ano}º ano</option>
            ))}
          </select>
        </label>
        <label className="field">
          <span>Turma</span>
          <select value={turma} onChange={(e) => setTurma(e.target.value)}>
            <option value="">Todas</option>
            {["A", "B", "U"].map((t) => (
              <option key={t} value={t}>{t}</option>
            ))}
          </select>
        </label>
        <button type="submit" className="btn btn--ghost">Filtrar</button>
      </form>

      {erro && <div className="alunos-page__error">{erro}</div>}

      <div className="alunos-page__table-wrapper">
        <table className="data-table">
          <thead>
            <tr>
              <th>Aluno</th>
              <th>Código</th>
              <th>Série / Turma</th>
              <th>Mãe</th>
              <th>Situação</th>
              <th aria-label="Ações"></th>
            </tr>
          </thead>
          <tbody>
            {carregando && (
              <tr><td colSpan={6} className="alunos-page__estado">Carregando...</td></tr>
            )}
            {!carregando && alunos.length === 0 && (
              <tr><td colSpan={6} className="alunos-page__estado">Nenhum aluno encontrado.</td></tr>
            )}
            {!carregando && alunos.map((aluno) => (
              <tr key={aluno.id}>
                <td>{aluno.nome}</td>
                <td><CodigoBadge codigo={aluno.codigoSeed} /></td>
                <td>{aluno.anoSerie ? `${aluno.anoSerie}º ${aluno.turma ?? ""}` : "—"}</td>
                <td>{aluno.mae}</td>
                <td>
                  <span className={"badge " + (aluno.transferido ? "badge--muted" : "badge--success")}>
                    {aluno.transferido ? "Transferido" : "Ativo"}
                  </span>
                </td>
                <td className="alunos-page__acoes">
                  <button className="btn btn--ghost btn--sm" onClick={() => abrirEdicao(aluno)}>
                    Editar
                  </button>
                  {isAdmin && (
                    <button
                      className="btn btn--ghost btn--sm"
                      onClick={() => setAlunoParaExcluir(aluno)}
                    >
                      Excluir
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="alunos-page__paginacao">
        <button
          className="btn btn--ghost btn--sm"
          disabled={page <= 1}
          onClick={() => setPage((p) => p - 1)}
        >
          Anterior
        </button>
        <span>Página {page} de {totalPages}</span>
        <button
          className="btn btn--ghost btn--sm"
          disabled={page >= totalPages}
          onClick={() => setPage((p) => p + 1)}
        >
          Próxima
        </button>
      </div>

      {modalAberto && (
        <AlunoFormModal
          aluno={alunoEmEdicao}
          onSave={salvar}
          onClose={() => setModalAberto(false)}
        />
      )}

      {alunoParaExcluir && (
        <ConfirmDialog
          title="Excluir aluno"
          message={`Tem certeza que deseja excluir "${alunoParaExcluir.nome}"? Esta ação não pode ser desfeita.`}
          confirmLabel="Excluir"
          danger
          onConfirm={confirmarExclusao}
          onCancel={() => setAlunoParaExcluir(null)}
        />
      )}
    </div>
  );
}
