export interface AuthUser {
  id: string;
  nome: string;
  email: string;
  roles: string[];
}

export interface AuthResponse {
  token: string;
  expiraEm: string;
  id: string;
  nome: string;
  email: string;
  roles: string[];
}

export interface Aluno {
  id: number;
  nome: string;
  pai?: string | null;
  mae: string;
  dataNascimento: string;
  endereco: string;
  telefone?: string | null;
  codigoSeed: string;
  anoLetivo: number;
  anoSerie?: number | null;
  turma?: string | null;
  numeroDoNis?: string | null;
  correcaoDeFluxo: string;
  transferido: boolean;
  urlFoto?: string | null;
}

export type AlunoFormValues = Omit<Aluno, "id">;

export interface AlunoParaDeclaracao {
  id: number;
  nome: string;
  pai?: string | null;
  mae: string;
  dataNascimento: string;
  codigoSeed: string;
  anoLetivo: number;
  anoSerie?: number | null;
  turma?: string | null;
  numeroDoNis?: string | null;
}

export interface Disciplina {
  id: number;
  nome: string;
}

export interface Professor {
  id: number;
  nome: string;
  cpf: string;
  cargo: string;
  cargaHorariaSemanal: number;
  disciplinas: Disciplina[];
}

export type ProfessorFormValues = Omit<Professor, "id" | "disciplinas"> & { disciplinaIds: number[] };

export interface Turma {
  id: number;
  nome: string;
}

export interface CelulaHorario {
  turmaId: number | null;
  disciplinaId: number | null;
}

export interface GradeTurno {
  seg: CelulaHorario[];
  ter: CelulaHorario[];
  qua: CelulaHorario[];
  qui: CelulaHorario[];
  sex: CelulaHorario[];
  sab: CelulaHorario[];
}

export interface GradeHorario {
  manha: GradeTurno;
  tarde: GradeTurno;
}

export interface HorarioProfessor {
  id: number;
  professorId: number;
  professorNome: string;
  cpf: string;
  cargo: string;
  cargaHorariaSemanal: number;
  disciplinas: string[];
  grade: GradeHorario;
}

export interface HorarioProfessorFormValues {
  professorId: number;
  grade: GradeHorario;
}

export function gradeVazia(): GradeHorario {
  const celulaVazia = (): CelulaHorario => ({ turmaId: null, disciplinaId: null });
  const diaVazio = (): CelulaHorario[] => Array.from({ length: 5 }, celulaVazia);
  const turnoVazio = (): GradeTurno => ({
    seg: diaVazio(), ter: diaVazio(), qua: diaVazio(),
    qui: diaVazio(), sex: diaVazio(), sab: diaVazio(),
  });
  return { manha: turnoVazio(), tarde: turnoVazio() };
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}
