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

export interface OficioFuncaoValues {
  tipo: "Professor" | "Servidor" | "ApoioEscolar";
  numeroOficio: number;
  assunto: string;
  destinatario: string;
  cargoDestinatario: string;
  cidadeDestinatario: string;
  saudacaoGenero: "Senhor" | "Senhora";
  nome: string;
  cpf: string;
  vinculo: string;
  dataAssumiuFuncao: string;
  cargaHoraria: number;
  cargo?: string;
  disciplina?: string;
  fonteRecursos?: number;
}

export async function gerarOficioFuncao(
  valores: OficioFuncaoValues
): Promise<{ blob: Blob; nomeArquivo: string }> {
  const response = await apiClient.post("/oficios/funcao", valores, { responseType: "blob" });

  const disposicao: string = response.headers["content-disposition"] ?? "";
  const match = disposicao.match(/filename="?([^"]+)"?/);
  const nomeArquivo = match?.[1] ?? "oficio.pdf";

  return { blob: response.data as Blob, nomeArquivo };
}
