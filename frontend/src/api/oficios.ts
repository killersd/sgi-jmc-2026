import { apiClient } from "./client";

export interface OficioGeralValues {
  numeroOficio: number;
  assunto: string;
  destinatario: string;
  cargoDoDestinatario: string;
  corpoDoOficio: string;
  remetente: string;
  cidade: string;
}

export async function gerarOficioGeral(
  valores: OficioGeralValues
): Promise<{ blob: Blob; nomeArquivo: string }> {
  const response = await apiClient.post("/oficios/geral", valores, { responseType: "blob" });

  const disposicao: string = response.headers["content-disposition"] ?? "";
  const match = disposicao.match(/filename="?([^"]+)"?/);
  const nomeArquivo = match?.[1] ?? "oficio.pdf";

  return { blob: response.data as Blob, nomeArquivo };
}
