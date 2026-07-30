import { type FormEvent, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { atualizarHorario, criarHorario, obterHorario } from "../api/horarios";
import { listarProfessores, obterProfessor } from "../api/professores";
import { listarTurmas, criarTurma } from "../api/turmas";
import { gradeVazia, type Disciplina, type HorarioProfessorFormValues, type Professor, type Turma } from "../types";
import GradeHorarioEditor from "../components/GradeHorarioEditor";
import "./HorarioEditorPage.css";

export default function HorarioEditorPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const editando = id !== undefined && id !== "novo";

  const [professores, setProfessores] = useState<Professor[]>([]);
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [professorId, setProfessorId] = useState<number | null>(null);
  const [disciplinasDoProfessor, setDisciplinasDoProfessor] = useState<Disciplina[]>([]);
  const [grade, setGrade] = useState(gradeVazia());
  const [novaTurma, setNovaTurma] = useState("");

  const [carregando, setCarregando] = useState(true);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  useEffect(() => {
    async function iniciar() {
      setCarregando(true);
      try {
        const [listaProfessores, listaTurmas] = await Promise.all([listarProfessores(), listarTurmas()]);
        setProfessores(listaProfessores);
        setTurmas(listaTurmas);

        if (editando) {
          const horario = await obterHorario(Number(id));
          setProfessorId(horario.professorId);
          setGrade(horario.grade);
          const professor = listaProfessores.find((p) => p.id === horario.professorId);
          setDisciplinasDoProfessor(professor?.disciplinas ?? []);
        }
      } catch {
        setErro("Não foi possível carregar os dados necessários.");
      } finally {
        setCarregando(false);
      }
    }
    iniciar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id, editando]);

  async function handleSelecionarProfessor(idSelecionado: string) {
    if (!idSelecionado) {
      setProfessorId(null);
      setDisciplinasDoProfessor([]);
      return;
    }
    const novoId = Number(idSelecionado);
    setProfessorId(novoId);
    const professor = await obterProfessor(novoId);
    setDisciplinasDoProfessor(professor.disciplinas);
  }

  async function handleAdicionarTurma() {
    const nome = novaTurma.trim();
    if (!nome) return;
    const turma = await criarTurma(nome);
    setTurmas((prev) => [...prev, turma].sort((a, b) => a.nome.localeCompare(b.nome)));
    setNovaTurma("");
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (!professorId) {
      setErro("Selecione um professor.");
      return;
    }
    setErro(null);
    setSalvando(true);
    try {
      const valores: HorarioProfessorFormValues = { professorId, grade };
      if (editando) {
        await atualizarHorario(Number(id), valores);
      } else {
        await criarHorario(valores);
      }
      navigate("/horarios");
    } catch {
      setErro("Não foi possível salvar. Verifique se este professor já não possui um horário cadastrado.");
    } finally {
      setSalvando(false);
    }
  }

  if (carregando) {
    return <p className="horarios-page__estado">Carregando...</p>;
  }

  return (
    <div>
      <header className="page-header">
        <h1>{editando ? "Editar horário" : "Novo horário"}</h1>
        <p>Escolha o professor e clique em cada célula para definir a turma e a disciplina daquele horário.</p>
      </header>

      <form onSubmit={handleSubmit}>
        <div className="horario-editor__dados">
          <label className="field horario-editor__professor">
            <span>Professor</span>
            <select
              required
              value={professorId ?? ""}
              onChange={(e) => handleSelecionarProfessor(e.target.value)}
              disabled={editando}
            >
              <option value="">Selecione um professor...</option>
              {professores.map((p) => (
                <option key={p.id} value={p.id}>{p.nome}</option>
              ))}
            </select>
          </label>

          {professorId && (
            <div className="horario-editor__resumo">
              {(() => {
                const professor = professores.find((p) => p.id === professorId);
                if (!professor) return null;
                return (
                  <>
                    <span><strong>CPF:</strong> {professor.cpf}</span>
                    <span><strong>Cargo:</strong> {professor.cargo}</span>
                    <span><strong>Carga horária:</strong> {professor.cargaHorariaSemanal}h</span>
                    <span><strong>Disciplinas:</strong> {professor.disciplinas.map((d) => d.nome).join(", ") || "nenhuma vinculada"}</span>
                  </>
                );
              })()}
            </div>
          )}
        </div>

        <div className="horario-editor__turmas">
          <span className="horario-editor__label">Adicionar nova turma ao catálogo (aparece no seletor de cada célula)</span>
          <div className="horario-editor__turmas-add">
            <input
              value={novaTurma}
              onChange={(e) => setNovaTurma(e.target.value)}
              placeholder="Ex: 6º Ano A"
              onKeyDown={(e) => { if (e.key === "Enter") { e.preventDefault(); handleAdicionarTurma(); } }}
            />
            <button type="button" className="btn btn--ghost btn--sm" onClick={handleAdicionarTurma}>
              + Adicionar turma
            </button>
          </div>
        </div>

        <div className="horario-editor__grade">
          {!professorId && <p className="horarios-page__estado">Selecione um professor para habilitar a grade.</p>}
          {professorId && (
            <GradeHorarioEditor
              grade={grade}
              onChange={setGrade}
              turmas={turmas}
              disciplinasDoProfessor={disciplinasDoProfessor}
            />
          )}
        </div>

        {erro && <div className="horario-editor__error">{erro}</div>}

        <div className="horario-editor__actions">
          <button type="button" className="btn btn--ghost" onClick={() => navigate("/horarios")}>
            Cancelar
          </button>
          <button type="submit" className="btn btn--primary" disabled={salvando || !professorId}>
            {salvando ? "Salvando..." : "Salvar horário"}
          </button>
        </div>
      </form>
    </div>
  );
}
