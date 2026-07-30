import { type FormEvent, useState } from "react";
import type { Disciplina, Professor, ProfessorFormValues } from "../types";
import DisciplinaMultiSelect from "./DisciplinaMultiSelect";
import "./ProfessorFormModal.css";

interface ProfessorFormModalProps {
  professor?: Professor | null;
  disciplinasDisponiveis: Disciplina[];
  onDisciplinaCriada: (disciplina: Disciplina) => void;
  onSave: (valores: ProfessorFormValues) => Promise<void>;
  onClose: () => void;
}

export default function ProfessorFormModal({
  professor, disciplinasDisponiveis, onDisciplinaCriada, onSave, onClose,
}: ProfessorFormModalProps) {
  const [nome, setNome] = useState(professor?.nome ?? "");
  const [cpf, setCpf] = useState(professor?.cpf ?? "");
  const [cargo, setCargo] = useState(professor?.cargo ?? "");
  const [cargaHorariaSemanal, setCargaHorariaSemanal] = useState(professor?.cargaHorariaSemanal ?? 0);
  const [disciplinaIds, setDisciplinaIds] = useState<number[]>(
    professor?.disciplinas.map((d) => d.id) ?? []
  );
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const editando = !!professor;

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);
    setSalvando(true);
    try {
      await onSave({ nome, cpf, cargo, cargaHorariaSemanal, disciplinaIds });
    } catch {
      setErro("Não foi possível salvar o professor.");
    } finally {
      setSalvando(false);
    }
  }

  return (
    <div className="dialog-overlay" onClick={onClose}>
      <form className="professor-form" onClick={(e) => e.stopPropagation()} onSubmit={handleSubmit}>
        <h3>{editando ? "Editar professor" : "Novo professor"}</h3>

        <div className="professor-form__grid">
          <label className="field professor-form__span2">
            <span>Nome do professor</span>
            <input required value={nome} onChange={(e) => setNome(e.target.value)} />
          </label>
          <label className="field">
            <span>CPF</span>
            <input required value={cpf} onChange={(e) => setCpf(e.target.value)} placeholder="000.000.000-00" />
          </label>
          <label className="field">
            <span>Cargo</span>
            <input required value={cargo} onChange={(e) => setCargo(e.target.value)} placeholder="Ex: Professor(a) efetivo(a)" />
          </label>
          <label className="field professor-form__span2">
            <span>Carga horária semanal</span>
            <input
              type="number" required min={0}
              value={cargaHorariaSemanal}
              onChange={(e) => setCargaHorariaSemanal(Number(e.target.value))}
            />
          </label>
        </div>

        <div className="professor-form__disciplinas">
          <span className="professor-form__label">Disciplinas que leciona</span>
          <DisciplinaMultiSelect
            disciplinas={disciplinasDisponiveis}
            selecionadas={disciplinaIds}
            onChange={setDisciplinaIds}
            onDisciplinaCriada={onDisciplinaCriada}
          />
        </div>

        {erro && <div className="professor-form__error">{erro}</div>}

        <div className="professor-form__actions">
          <button type="button" className="btn btn--ghost" onClick={onClose}>Cancelar</button>
          <button type="submit" className="btn btn--primary" disabled={salvando}>
            {salvando ? "Salvando..." : "Salvar"}
          </button>
        </div>
      </form>
    </div>
  );
}
