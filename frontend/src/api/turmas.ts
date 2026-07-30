import { apiClient } from "./client";
import type { Turma } from "../types";

export async function listarTurmas(): Promise<Turma[]> {
  const { data } = await apiClient.get<Turma[]>("/turmas");
  return data;
}

export async function criarTurma(nome: string): Promise<Turma> {
  const { data } = await apiClient.post<Turma>("/turmas", { nome });
  return data;
}

export async function removerTurma(id: number): Promise<void> {
  await apiClient.delete(`/turmas/${id}`);
}
