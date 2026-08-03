import { apiClient } from "./client";
import type { AlunoParaDeclaracao } from "../types";

export async function buscarAlunoPorCodigo(codigoSeed: string): Promise<AlunoParaDeclaracao> {
  const { data } = await apiClient.get<AlunoParaDeclaracao>(
    `/declaracoes/buscar-aluno/${encodeURIComponent(codigoSeed)}`
  );
  return data;
}

export async function imprimirDeclaracaoFrequencia(
  codigoSeed: string,
  qtdFaltas: number
): Promise<{ blob: Blob; nomeArquivo: string }> {
  const response = await apiClient.post(
    "/declaracoes/frequencia",
    { codigoSeed, qtdFaltas },
    { responseType: "blob" }
  );

  const disposicao: string = response.headers["content-disposition"] ?? "";
  const match = disposicao.match(/filename="?([^"]+)"?/);
  const nomeArquivo = match?.[1] ?? "declaracao.pdf";

  return { blob: response.data as Blob, nomeArquivo };
}

export async function imprimirDeclaracaoTransferencia(
  codigoSeed: string,
  escolaDestino: string,
  motivo: string
): Promise<{ blob: Blob; nomeArquivo: string }> {
  const response = await apiClient.post(
    "/declaracoes/transferencia",
    { codigoSeed, escolaDestino, motivo: motivo || null },
    { responseType: "blob" }
  );

  const disposicao: string = response.headers["content-disposition"] ?? "";
  const match = disposicao.match(/filename="?([^"]+)"?/);
  const nomeArquivo = match?.[1] ?? "declaracao-transferencia.pdf";

  return { blob: response.data as Blob, nomeArquivo };
}
