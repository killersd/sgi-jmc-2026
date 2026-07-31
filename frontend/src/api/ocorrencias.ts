import { apiClient } from "./client";

function extrairNomeArquivo(headers: Record<string, unknown>, padrao: string): string {
  const disposicao = String(headers["content-disposition"] ?? "");
  const match = disposicao.match(/filename="?([^"]+)"?/);
  return match?.[1] ?? padrao;
}

export interface AdvertenciaValues {
  codigoSeed: string;
  turno: string;
  descricaoDoFato: string;
  numero: number;
}

export interface SuspensaoValues {
  codigoSeed: string;
  turno: string;
  descricaoDoFato: string;
  dias: number;
  numeroSuspensao: number;
}

export async function gerarAdvertencia(valores: AdvertenciaValues): Promise<{ blob: Blob; nomeArquivo: string }> {
  const response = await apiClient.post("/ocorrencias/advertencia", valores, { responseType: "blob" });
  return { blob: response.data as Blob, nomeArquivo: extrairNomeArquivo(response.headers, "advertencia.pdf") };
}

export async function gerarSuspensao(valores: SuspensaoValues): Promise<{ blob: Blob; nomeArquivo: string }> {
  const response = await apiClient.post("/ocorrencias/suspensao", valores, { responseType: "blob" });
  return { blob: response.data as Blob, nomeArquivo: extrairNomeArquivo(response.headers, "suspensao.pdf") };
}
