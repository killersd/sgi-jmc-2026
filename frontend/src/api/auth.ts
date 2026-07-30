import { apiClient } from "./client";
import type { AuthResponse } from "../types";

export async function login(email: string, senha: string): Promise<AuthResponse> {
  const { data } = await apiClient.post<AuthResponse>("/auth/login", { email, senha });
  return data;
}
