import { useEffect, useState } from "react";
import { listarProfessores, criarProfessor, atualizarProfessor, removerProfessor } from "../api/professores";
import { listarDisciplinas } from "../api/disciplinas";
import type { Disciplina, Professor, ProfessorFormValues } from "../types";
import { useAuth } from "../context/AuthContext";
import ProfessorFormModal from "../components/ProfessorFormModal";
import ConfirmDialog from "../components/ConfirmDialog";
import "./ProfessoresPage.css";

export default function ProfessoresPage() {
  const { isAdmin } = useAuth();
  const [professores, setProfessores] = useState<Professor[]>([]);
  const [disciplinas, setDisciplinas] = useState<Disciplina[]>([]);
  const [busca, setBusca] = useState("");
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);

  const [modalAberto, setModalAberto] = useState(false);
  const [professorEmEdicao, setProfessorEmEdicao] = useState<Professor | null>(null);
  const [paraExcluir, setParaExcluir] = useState<Professor | null>(null);
  const [erroExclusao, setErroExclusao] = useState<string | null>(null);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      const [listaProfessores, listaDisciplinas] = await Promise.all([
        listarProfessores(busca || undefined),
        listarDisciplinas(),
      ]);
      setProfessores(listaProfessores);
      setDisciplinas(listaDisciplinas);
    } catch {
      setErro("Não foi possível carregar os professores.");
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  function abrirNovo() {
    setProfessorEmEdicao(null);
    setModalAberto(true);
  }

  function abrirEdicao(professor: Professor) {
    setProfessorEmEdicao(professor);
    setModalAberto(true);
  }

  async function salvar(valores: ProfessorFormValues) {
    if (professorEmEdicao) {
      await atualizarProfessor(professorEmEdicao.id, valores);
    } else {
      await criarProfessor(valores);
    }
    setModalAberto(false);
    await carregar();
  }

  async function confirmarExclusao() {
    if (!paraExcluir) return;
    setErroExclusao(null);
    try {
      await removerProfessor(paraExcluir.id);
      setParaExcluir(null);
      await carregar();
    } catch {
      setErroExclusao("Este professor já possui um horário cadastrado. Remova o horário primeiro.");
    }
  }

  return (
    <div>
      <header className="page-header page-header--row">
        <div>
          <h1>Professores</h1>
          <p>Cadastro central de professores e disciplinas que lecionam.</p>
        </div>
        <button className="btn btn--primary" onClick={abrirNovo}>+ Novo professor</button>
      </header>

      <form
        className="professores-busca"
        onSubmit={(e) => { e.preventDefault(); carregar(); }}
      >
        <label className="field">
          <span>Buscar por nome</span>
          <input value={busca} onChange={(e) => setBusca(e.target.value)} placeholder="Ex: João Silva" />
        </label>
        <button type="submit" className="btn btn--ghost">Filtrar</button>
      </form>

      {erro && <div className="professores-page__error">{erro}</div>}

      <div className="professores-grid">
        {carregando && <p className="professores-page__estado">Carregando...</p>}
        {!carregando && professores.length === 0 && (
          <p className="professores-page__estado">Nenhum professor cadastrado.</p>
        )}
        {!carregando && professores.map((p) => (
          <div key={p.id} className="professor-card">
            <div>
              <h3>{p.nome}</h3>
              <p className="professor-card__meta">{p.cargo} · {p.cargaHorariaSemanal}h semanais</p>
              <div className="professor-card__disciplinas">
                {p.disciplinas.length === 0 && <span className="professor-card__sem-disciplina">Sem disciplinas vinculadas</span>}
                {p.disciplinas.map((d) => (
                  <span key={d.id} className="badge badge--muted">{d.nome}</span>
                ))}
              </div>
            </div>
            <div className="professor-card__acoes">
              <button className="btn btn--ghost btn--sm" onClick={() => abrirEdicao(p)}>Editar</button>
              {isAdmin && (
                <button className="btn btn--ghost btn--sm" onClick={() => setParaExcluir(p)}>Excluir</button>
              )}
            </div>
          </div>
        ))}
      </div>

      {modalAberto && (
        <ProfessorFormModal
          professor={professorEmEdicao}
          disciplinasDisponiveis={disciplinas}
          onDisciplinaCriada={(d) => setDisciplinas((prev) => [...prev, d].sort((a, b) => a.nome.localeCompare(b.nome)))}
          onSave={salvar}
          onClose={() => setModalAberto(false)}
        />
      )}

      {paraExcluir && (
        <ConfirmDialog
          title="Excluir professor"
          message={erroExclusao ?? `Tem certeza que deseja excluir "${paraExcluir.nome}"?`}
          confirmLabel="Excluir"
          danger
          onConfirm={confirmarExclusao}
          onCancel={() => { setParaExcluir(null); setErroExclusao(null); }}
        />
      )}
    </div>
  );
}
