import { apiClient } from "./client";
import type { Disciplina } from "../types";

export async function listarDisciplinas(): Promise<Disciplina[]> {
  const { data } = await apiClient.get<Disciplina[]>("/disciplinas");
  return data;
}

export async function criarDisciplina(nome: string): Promise<Disciplina> {
  const { data } = await apiClient.post<Disciplina>("/disciplinas", { nome });
  return data;
}
