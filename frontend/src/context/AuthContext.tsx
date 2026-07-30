import { createContext, useContext, useMemo, useState, type ReactNode } from "react";
import type { AuthUser } from "../types";
import { login as loginRequest } from "../api/auth";

interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (email: string, senha: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const TOKEN_KEY = "sgi_jmc_token";
const USER_KEY = "sgi_jmc_user";

function readStoredUser(): AuthUser | null {
  const raw = localStorage.getItem(USER_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as AuthUser;
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(readStoredUser());

  const value = useMemo<AuthContextValue>(() => ({
    user,
    isAuthenticated: !!user,
    isAdmin: !!user?.roles.includes("administrador"),
    async login(email: string, senha: string) {
      const resposta = await loginRequest(email, senha);
      const authUser: AuthUser = {
        id: resposta.id,
        nome: resposta.nome,
        email: resposta.email,
        roles: resposta.roles,
      };
      localStorage.setItem(TOKEN_KEY, resposta.token);
      localStorage.setItem(USER_KEY, JSON.stringify(authUser));
      setUser(authUser);
    },
    logout() {
      localStorage.removeItem(TOKEN_KEY);
      localStorage.removeItem(USER_KEY);
      setUser(null);
    },
  }), [user]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth deve ser usado dentro de um AuthProvider");
  return ctx;
}
