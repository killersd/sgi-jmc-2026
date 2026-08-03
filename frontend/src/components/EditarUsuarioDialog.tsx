import { type FormEvent, useState } from "react";
import type { UsuarioAdmin } from "../types";
import "./ConfirmDialog.css";

interface EditarUsuarioDialogProps {
  usuario: UsuarioAdmin;
  onSalvar: (valores: { nomeCompleto: string; email: string; cpf: string; dataNascimento: string }) => Promise<void>;
  onCancel: () => void;
}

export default function EditarUsuarioDialog({ usuario, onSalvar, onCancel }: EditarUsuarioDialogProps) {
  const [nomeCompleto, setNomeCompleto] = useState(usuario.nomeCompleto);
  const [email, setEmail] = useState(usuario.email);
  const [cpf, setCpf] = useState(usuario.cpf ?? "");
  const [dataNascimento, setDataNascimento] = useState(usuario.dataNascimento?.slice(0, 10) ?? "");
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);
    setSalvando(true);
    try {
      await onSalvar({ nomeCompleto, email, cpf, dataNascimento });
    } catch {
      setErro("Não foi possível salvar. Verifique se o e-mail já está em uso.");
    } finally {
      setSalvando(false);
    }
  }

  return (
    <div className="dialog-overlay" onClick={onCancel}>
      <div className="dialog-card" onClick={(e) => e.stopPropagation()}>
        <h3>Editar usuário</h3>
        <form onSubmit={handleSubmit}>
          <div className="usuario-novo-form__grid">
            <label className="field">
              <span>Nome completo</span>
              <input required value={nomeCompleto} onChange={(e) => setNomeCompleto(e.target.value)} />
            </label>
            <label className="field">
              <span>E-mail</span>
              <input required type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
            </label>
            <label className="field">
              <span>CPF</span>
              <input value={cpf} onChange={(e) => setCpf(e.target.value)} />
            </label>
            <label className="field">
              <span>Data de nascimento</span>
              <input type="date" value={dataNascimento} onChange={(e) => setDataNascimento(e.target.value)} />
            </label>
          </div>

          {erro && <div className="ocorrencias-page__error">{erro}</div>}

          <div className="dialog-card__actions">
            <button type="button" className="btn btn--ghost" onClick={onCancel}>Cancelar</button>
            <button type="submit" className="btn btn--primary" disabled={salvando}>
              {salvando ? "Salvando..." : "Salvar"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
