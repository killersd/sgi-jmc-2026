import { apiClient } from "./client";
import type { ConfiguracaoNotificacoes } from "../types";

export async function obterConfiguracaoNotificacoes(): Promise<ConfiguracaoNotificacoes> {
  const { data } = await apiClient.get<ConfiguracaoNotificacoes>("/configuracoes/notificacoes");
  return data;
}

export async function atualizarConfiguracaoNotificacoes(email: string): Promise<void> {
  await apiClient.put("/configuracoes/notificacoes", {
    emailNotificacaoTransferencia: email || null,
  });
}
