import type { CelulaHorario, Disciplina, GradeHorario, GradeTurno, Turma } from "../types";
import "./GradeHorarioEditor.css";

const DIAS: { chave: keyof GradeTurno; label: string }[] = [
  { chave: "seg", label: "Segunda" },
  { chave: "ter", label: "Terça" },
  { chave: "qua", label: "Quarta" },
  { chave: "qui", label: "Quinta" },
  { chave: "sex", label: "Sexta" },
  { chave: "sab", label: "Sábado" },
];

interface GradeHorarioEditorProps {
  grade: GradeHorario;
  onChange: (grade: GradeHorario) => void;
  turmas: Turma[];
  disciplinasDoProfessor: Disciplina[];
}

function CelulaEditor({
  celula, turmas, disciplinasDoProfessor, onChange,
}: {
  celula: CelulaHorario;
  turmas: Turma[];
  disciplinasDoProfessor: Disciplina[];
  onChange: (celula: CelulaHorario) => void;
}) {
  return (
    <div className="celula-editor">
      <select
        value={celula.turmaId ?? ""}
        onChange={(e) => onChange({ ...celula, turmaId: e.target.value ? Number(e.target.value) : null })}
      >
        <option value="">Turma</option>
        {turmas.map((t) => <option key={t.id} value={t.id}>{t.nome}</option>)}
      </select>
      <select
        value={celula.disciplinaId ?? ""}
        onChange={(e) => onChange({ ...celula, disciplinaId: e.target.value ? Number(e.target.value) : null })}
      >
        <option value="">Disciplina</option>
        {disciplinasDoProfessor.map((d) => <option key={d.id} value={d.id}>{d.nome}</option>)}
      </select>
    </div>
  );
}

function GradeTurnoTable({
  titulo, turno, turmas, disciplinasDoProfessor, onChangeCelula,
}: {
  titulo: string;
  turno: GradeTurno;
  turmas: Turma[];
  disciplinasDoProfessor: Disciplina[];
  onChangeCelula: (dia: keyof GradeTurno, periodo: number, celula: CelulaHorario) => void;
}) {
  return (
    <div className="grade-turno">
      <h3>{titulo}</h3>
      <div className="grade-turno__scroll">
        <table className="grade-turno__table">
          <thead>
            <tr>
              <th>Aula</th>
              {DIAS.map((d) => <th key={d.chave}>{d.label}</th>)}
            </tr>
          </thead>
          <tbody>
            {[0, 1, 2, 3, 4].map((periodo) => (
              <tr key={periodo}>
                <td className="grade-turno__periodo">{periodo + 1}ª</td>
                {DIAS.map((d) => (
                  <td key={d.chave}>
                    <CelulaEditor
                      celula={turno[d.chave][periodo]}
                      turmas={turmas}
                      disciplinasDoProfessor={disciplinasDoProfessor}
                      onChange={(celula) => onChangeCelula(d.chave, periodo, celula)}
                    />
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default function GradeHorarioEditor({
  grade, onChange, turmas, disciplinasDoProfessor,
}: GradeHorarioEditorProps) {
  function atualizarCelula(turno: "manha" | "tarde", dia: keyof GradeTurno, periodo: number, celula: CelulaHorario) {
    const novaLista = [...grade[turno][dia]];
    novaLista[periodo] = celula;
    onChange({
      ...grade,
      [turno]: { ...grade[turno], [dia]: novaLista },
    });
  }

  return (
    <div className="grade-editor">
      <GradeTurnoTable
        titulo="Manhã"
        turno={grade.manha}
        turmas={turmas}
        disciplinasDoProfessor={disciplinasDoProfessor}
        onChangeCelula={(dia, periodo, celula) => atualizarCelula("manha", dia, periodo, celula)}
      />
      <GradeTurnoTable
        titulo="Tarde"
        turno={grade.tarde}
        turmas={turmas}
        disciplinasDoProfessor={disciplinasDoProfessor}
        onChangeCelula={(dia, periodo, celula) => atualizarCelula("tarde", dia, periodo, celula)}
      />
    </div>
  );
}
