import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { imprimirHorario, listarHorarios, removerHorario } from "../api/horarios";
import type { HorarioProfessor } from "../types";
import { useAuth } from "../context/AuthContext";
import ConfirmDialog from "../components/ConfirmDialog";
import "./HorariosPage.css";

export default function HorariosPage() {
  const { isAdmin } = useAuth();
  const navigate = useNavigate();
  const [horarios, setHorarios] = useState<HorarioProfessor[]>([]);
  const [busca, setBusca] = useState("");
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [paraExcluir, setParaExcluir] = useState<HorarioProfessor | null>(null);
  const [imprimindoId, setImprimindoId] = useState<number | null>(null);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      setHorarios(await listarHorarios(busca || undefined));
    } catch {
      setErro("Não foi possível carregar os horários.");
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function confirmarExclusao() {
    if (!paraExcluir) return;
    await removerHorario(paraExcluir.id);
    setParaExcluir(null);
    await carregar();
  }

  async function handleImprimir(horario: HorarioProfessor) {
    setImprimindoId(horario.id);
    try {
      const { blob, nomeArquivo } = await imprimirHorario(horario.id);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = nomeArquivo;
      link.click();
      window.URL.revokeObjectURL(url);
    } catch {
      setErro("Não foi possível gerar o PDF deste horário.");
    } finally {
      setImprimindoId(null);
    }
  }

  return (
    <div>
      <header className="page-header page-header--row">
        <div>
          <h1>Horários</h1>
          <p>Grade semanal de aulas por professor.</p>
        </div>
        <button className="btn btn--primary" onClick={() => navigate("/horarios/novo")}>
          + Novo horário
        </button>
      </header>

      <form
        className="horarios-busca"
        onSubmit={(e) => { e.preventDefault(); carregar(); }}
      >
        <label className="field">
          <span>Buscar por professor</span>
          <input value={busca} onChange={(e) => setBusca(e.target.value)} placeholder="Ex: João Silva" />
        </label>
        <button type="submit" className="btn btn--ghost">Filtrar</button>
      </form>

      {erro && <div className="horarios-page__error">{erro}</div>}

      <div className="horarios-grid">
        {carregando && <p className="horarios-page__estado">Carregando...</p>}
        {!carregando && horarios.length === 0 && (
          <p className="horarios-page__estado">Nenhum horário cadastrado.</p>
        )}
        {!carregando && horarios.map((h) => (
          <div key={h.id} className="horario-card">
            <div>
              <h3>{h.professorNome}</h3>
              <p className="horario-card__meta">{h.cargo}</p>
              <p className="horario-card__meta">{h.cargaHorariaSemanal}h semanais</p>
              <p className="horario-card__meta">
                {h.disciplinas.length > 0 ? h.disciplinas.join(", ") : "Sem disciplinas vinculadas"}
              </p>
            </div>
            <div className="horario-card__acoes">
              <button className="btn btn--ghost btn--sm" onClick={() => navigate(`/horarios/${h.id}`)}>
                Ver / Editar
              </button>
              <button
                className="btn btn--ghost btn--sm"
                onClick={() => handleImprimir(h)}
                disabled={imprimindoId === h.id}
              >
                {imprimindoId === h.id ? "Gerando..." : "🖨️ Imprimir"}
              </button>
              {isAdmin && (
                <button className="btn btn--ghost btn--sm" onClick={() => setParaExcluir(h)}>
                  Excluir
                </button>
              )}
            </div>
          </div>
        ))}
      </div>

      {paraExcluir && (
        <ConfirmDialog
          title="Excluir horário"
          message={`Tem certeza que deseja excluir o horário de "${paraExcluir.professorNome}"?`}
          confirmLabel="Excluir"
          danger
          onConfirm={confirmarExclusao}
          onCancel={() => setParaExcluir(null)}
        />
      )}
    </div>
  );
}
