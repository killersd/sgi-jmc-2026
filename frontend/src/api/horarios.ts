import { apiClient } from "./client";
import type { HorarioProfessor, HorarioProfessorFormValues } from "../types";

export async function listarHorarios(busca?: string): Promise<HorarioProfessor[]> {
  const { data } = await apiClient.get<HorarioProfessor[]>("/horarios", { params: { busca } });
  return data;
}

export async function obterHorario(id: number): Promise<HorarioProfessor> {
  const { data } = await apiClient.get<HorarioProfessor>(`/horarios/${id}`);
  return data;
}

export async function criarHorario(valores: HorarioProfessorFormValues): Promise<HorarioProfessor> {
  const { data } = await apiClient.post<HorarioProfessor>("/horarios", valores);
  return data;
}

export async function atualizarHorario(id: number, valores: HorarioProfessorFormValues): Promise<HorarioProfessor> {
  const { data } = await apiClient.put<HorarioProfessor>(`/horarios/${id}`, valores);
  return data;
}

export async function removerHorario(id: number): Promise<void> {
  await apiClient.delete(`/horarios/${id}`);
}

export async function imprimirHorario(id: number): Promise<{ blob: Blob; nomeArquivo: string }> {
  const response = await apiClient.get(`/horarios/${id}/pdf`, { responseType: "blob" });

  const disposicao: string = response.headers["content-disposition"] ?? "";
  const match = disposicao.match(/filename="?([^"]+)"?/);
  const nomeArquivo = match?.[1] ?? "horario.pdf";

  return { blob: response.data as Blob, nomeArquivo };
}
