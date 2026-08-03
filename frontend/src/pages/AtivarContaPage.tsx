import { type FormEvent, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { ativarConta } from "../api/auth";
import "./LoginPage.css";

export default function AtivarContaPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const userId = searchParams.get("userId") ?? "";
  const token = searchParams.get("token") ?? "";

  const [novaSenha, setNovaSenha] = useState("");
  const [confirmarSenha, setConfirmarSenha] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  const linkInvalido = !userId || !token;

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);

    if (novaSenha !== confirmarSenha) {
      setErro("As senhas não coincidem.");
      return;
    }

    setCarregando(true);
    try {
      await ativarConta(userId, token, novaSenha);
      navigate("/login?ativado=1", { replace: true });
    } catch {
      setErro("Não foi possível ativar a conta. O link pode ter expirado ou já ter sido utilizado.");
    } finally {
      setCarregando(false);
    }
  }

  return (
    <div className="login-screen">
      <aside className="login-brand">
        <div className="login-brand__mark">SGI</div>
        <h1 className="login-brand__title">SGI - Sistema de Gestão Institucionmal</h1>
        <p className="login-brand__subtitle">Defina sua senha para ativar seu acesso.</p>
      </aside>

      <main className="login-panel">
        {linkInvalido ? (
          <div className="login-form__error">Link de ativação inválido. Verifique o e-mail recebido.</div>
        ) : (
          <form className="login-form" onSubmit={handleSubmit}>
            <h2>Ativar conta</h2>
            <p className="login-form__hint">Defina uma senha para poder entrar no sistema.</p>

            <label className="field">
              <span>Nova senha</span>
              <input
                type="password"
                required
                autoFocus
                minLength={6}
                value={novaSenha}
                onChange={(e) => setNovaSenha(e.target.value)}
                placeholder="••••••••"
              />
            </label>

            <label className="field">
              <span>Confirmar senha</span>
              <input
                type="password"
                required
                minLength={6}
                value={confirmarSenha}
                onChange={(e) => setConfirmarSenha(e.target.value)}
                placeholder="••••••••"
              />
            </label>

            {erro && <div className="login-form__error">{erro}</div>}

            <button type="submit" className="btn btn--primary" disabled={carregando}>
              {carregando ? "Ativando..." : "Ativar conta"}
            </button>
          </form>
        )}
      </main>
    </div>
  );
}
