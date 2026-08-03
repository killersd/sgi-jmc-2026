import { apiClient } from "./client";
import type { MatrizPermissoes, Permissao } from "../types";

export async function obterMatrizPermissoes(): Promise<MatrizPermissoes> {
  const { data } = await apiClient.get<MatrizPermissoes>("/perfis/matriz");
  return data;
}

export async function atualizarPermissoes(permissoes: Permissao[]): Promise<void> {
  await apiClient.put("/perfis/matriz", { permissoes });
}
