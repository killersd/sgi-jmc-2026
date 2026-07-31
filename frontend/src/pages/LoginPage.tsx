import { type FormEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import "./LoginPage.css";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [erro, setErro] = useState<string | null>(null);
  const [carregando, setCarregando] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);
    setCarregando(true);
    try {
      await login(email, senha);
      navigate("/", { replace: true });
    } catch {
      setErro("E-mail ou senha inválidos.");
    } finally {
      setCarregando(false);
    }
  }

  return (
    <div className="login-screen">
      <aside className="login-brand">
        <div className="login-brand__mark">SGI</div>
        <h1 className="login-brand__title">SGI - Sistema de Gestão Institucionmal</h1>
        <p className="login-brand__subtitle">
          Matrículas, declarações e documentos
          escolares em um só lugar.
        </p>
        <ul className="login-brand__list">
          <li>Cadastro de alunos</li>
          <li>Emissão de declarações e ofícios</li>
          <li>Controle de turmas</li>
          <li>Controle de horários</li>
          <li>Controle de professores</li>
          <li>Advertências e suspensões</li>
        </ul>
      </aside>

      <main className="login-panel">
        <form className="login-form" onSubmit={handleSubmit}>
          <h2>Entrar</h2>
          <p className="login-form__hint">Acesse com sua conta institucional.</p>

          <label className="field">
            <span>E-mail</span>
            <input
              type="email"
              required
              autoFocus
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="seuusuario@escola.gov.br"
            />
          </label>

          <label className="field">
            <span>Senha</span>
            <input
              type="password"
              required
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              placeholder="••••••••"
            />
          </label>

          {erro && <div className="login-form__error">{erro}</div>}

          <button type="submit" className="btn btn--primary" disabled={carregando}>
            {carregando ? "Entrando..." : "Entrar"}
          </button>
        </form>
      </main>
    </div>
  );
}
