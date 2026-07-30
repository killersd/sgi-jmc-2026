import { apiClient } from "./client";
import type { Aluno, AlunoFormValues, PagedResult } from "../types";

export interface AlunosFiltro {
  anoSerie?: number;
  turma?: string;
  busca?: string;
  transferido?: boolean;
  page?: number;
  pageSize?: number;
}

export async function listarAlunos(filtro: AlunosFiltro): Promise<PagedResult<Aluno>> {
  const { data } = await apiClient.get<PagedResult<Aluno>>("/alunos", { params: filtro });
  return data;
}

export async function obterAluno(id: number): Promise<Aluno> {
  const { data } = await apiClient.get<Aluno>(`/alunos/${id}`);
  return data;
}

export async function criarAluno(valores: AlunoFormValues): Promise<Aluno> {
  const { data } = await apiClient.post<Aluno>("/alunos", valores);
  return data;
}

export async function atualizarAluno(id: number, valores: AlunoFormValues): Promise<Aluno> {
  const { data } = await apiClient.put<Aluno>(`/alunos/${id}`, valores);
  return data;
}

export async function removerAluno(id: number): Promise<void> {
  await apiClient.delete(`/alunos/${id}`);
}
