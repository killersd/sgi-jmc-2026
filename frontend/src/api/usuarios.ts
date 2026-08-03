import { apiClient } from "./client";
import type { UsuarioAdmin, UsuarioCriado } from "../types";

export async function listarUsuarios(): Promise<UsuarioAdmin[]> {
  const { data } = await apiClient.get<UsuarioAdmin[]>("/usuarios");
  return data;
}

export async function atualizarPerfilUsuario(id: string, novoPerfil: string): Promise<void> {
  await apiClient.put(`/usuarios/${id}/perfil`, { novoPerfil });
}

export async function removerUsuario(id: string): Promise<void> {
  await apiClient.delete(`/usuarios/${id}`);
}

export interface EditarUsuarioValues {
  nomeCompleto: string;
  email: string;
  cpf?: string | null;
  dataNascimento?: string | null;
}

export async function editarUsuario(id: string, valores: EditarUsuarioValues): Promise<void> {
  await apiClient.put(`/usuarios/${id}`, {
    nomeCompleto: valores.nomeCompleto,
    email: valores.email,
    cpf: valores.cpf ?? null,
    dataNascimento: valores.dataNascimento ?? null,
  });
}

export async function atualizarStatusUsuario(id: string, ativo: boolean): Promise<void> {
  await apiClient.put(`/usuarios/${id}/status`, { ativo });
}

export interface NovoUsuarioValues {
  nomeCompleto: string;
  email: string;
  perfil: string;
  cpf?: string;
}

export async function criarUsuario(valores: NovoUsuarioValues): Promise<UsuarioCriado> {
  const { data } = await apiClient.post<UsuarioCriado>("/auth/registrar", {
    nomeCompleto: valores.nomeCompleto,
    email: valores.email,
    perfil: valores.perfil,
    cpf: valores.cpf ?? null,
  });
  return data;
}
