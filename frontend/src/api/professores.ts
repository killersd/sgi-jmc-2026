import { apiClient } from "./client";
import type { Professor, ProfessorFormValues } from "../types";

export async function listarProfessores(busca?: string): Promise<Professor[]> {
  const { data } = await apiClient.get<Professor[]>("/professores", { params: { busca } });
  return data;
}

export async function obterProfessor(id: number): Promise<Professor> {
  const { data } = await apiClient.get<Professor>(`/professores/${id}`);
  return data;
}

export async function criarProfessor(valores: ProfessorFormValues): Promise<Professor> {
  const { data } = await apiClient.post<Professor>("/professores", valores);
  return data;
}

export async function atualizarProfessor(id: number, valores: ProfessorFormValues): Promise<Professor> {
  const { data } = await apiClient.put<Professor>(`/professores/${id}`, valores);
  return data;
}

export async function removerProfessor(id: number): Promise<void> {
  await apiClient.delete(`/professores/${id}`);
}
