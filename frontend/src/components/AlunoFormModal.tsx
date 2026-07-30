import { type FormEvent, useState } from "react";
import type { Aluno, AlunoFormValues } from "../types";
import "./AlunoFormModal.css";

interface AlunoFormModalProps {
  aluno?: Aluno | null;
  onSave: (valores: AlunoFormValues) => Promise<void>;
  onClose: () => void;
}

const valoresIniciais = (aluno?: Aluno | null): AlunoFormValues => ({
  nome: aluno?.nome ?? "",
  pai: aluno?.pai ?? "",
  mae: aluno?.mae ?? "",
  dataNascimento: aluno?.dataNascimento?.slice(0, 10) ?? "",
  endereco: aluno?.endereco ?? "",
  telefone: aluno?.telefone ?? "",
  codigoSeed: aluno?.codigoSeed ?? "",
  anoLetivo: aluno?.anoLetivo ?? new Date().getFullYear(),
  anoSerie: aluno?.anoSerie ?? undefined,
  turma: aluno?.turma ?? "",
  numeroDoNis: aluno?.numeroDoNis ?? "",
  correcaoDeFluxo: aluno?.correcaoDeFluxo ?? "Não",
  transferido: aluno?.transferido ?? false,
});

export default function AlunoFormModal({ aluno, onSave, onClose }: AlunoFormModalProps) {
  const [valores, setValores] = useState<AlunoFormValues>(valoresIniciais(aluno));
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const editando = !!aluno;

  function atualizarCampo<K extends keyof AlunoFormValues>(campo: K, valor: AlunoFormValues[K]) {
    setValores((prev) => ({ ...prev, [campo]: valor }));
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);
    setSalvando(true);
    try {
      await onSave(valores);
    } catch (err: unknown) {
      const mensagem =
        (err as { response?: { data?: { mensagem?: string } } })?.response?.data?.mensagem
        ?? "Não foi possível salvar o aluno.";
      setErro(mensagem);
    } finally {
      setSalvando(false);
    }
  }

  return (
    <div className="dialog-overlay" onClick={onClose}>
      <form
        className="aluno-form"
        onClick={(e) => e.stopPropagation()}
        onSubmit={handleSubmit}
      >
        <h3>{editando ? "Editar aluno" : "Novo aluno"}</h3>

        <div className="aluno-form__grid">
          <label className="field aluno-form__span2">
            <span>Nome do aluno</span>
            <input
              required
              value={valores.nome}
              onChange={(e) => atualizarCampo("nome", e.target.value)}
            />
          </label>

          <label className="field">
            <span>Nome da mãe</span>
            <input
              required
              value={valores.mae}
              onChange={(e) => atualizarCampo("mae", e.target.value)}
            />
          </label>

          <label className="field">
            <span>Nome do pai</span>
            <input
              value={valores.pai ?? ""}
              onChange={(e) => atualizarCampo("pai", e.target.value)}
            />
          </label>

          <label className="field">
            <span>Data de nascimento</span>
            <input
              type="date"
              required
              value={valores.dataNascimento}
              onChange={(e) => atualizarCampo("dataNascimento", e.target.value)}
            />
          </label>

          <label className="field">
            <span>Telefone</span>
            <input
              value={valores.telefone ?? ""}
              onChange={(e) => atualizarCampo("telefone", e.target.value)}
              placeholder="(00) 00000-0000"
            />
          </label>

          <label className="field aluno-form__span2">
            <span>Endereço</span>
            <input
              required
              value={valores.endereco}
              onChange={(e) => atualizarCampo("endereco", e.target.value)}
            />
          </label>

          <label className="field">
            <span>Código do aluno</span>
            <input
              required
              value={valores.codigoSeed}
              onChange={(e) => atualizarCampo("codigoSeed", e.target.value)}
              placeholder="Matrícula"
            />
          </label>

          <label className="field">
            <span>Número do NIS</span>
            <input
              value={valores.numeroDoNis ?? ""}
              onChange={(e) => atualizarCampo("numeroDoNis", e.target.value)}
            />
          </label>

          <label className="field">
            <span>Ano letivo</span>
            <input
              type="number"
              required
              value={valores.anoLetivo}
              onChange={(e) => atualizarCampo("anoLetivo", Number(e.target.value))}
            />
          </label>

          <label className="field">
            <span>Ano/série</span>
            <input
              type="number"
              min={1}
              max={9}
              value={valores.anoSerie ?? ""}
              onChange={(e) =>
                atualizarCampo("anoSerie", e.target.value ? Number(e.target.value) : undefined)
              }
            />
          </label>

          <label className="field">
            <span>Turma</span>
            <input
              maxLength={1}
              value={valores.turma ?? ""}
              onChange={(e) => atualizarCampo("turma", e.target.value.toUpperCase())}
              placeholder="A"
            />
          </label>

          <label className="field">
            <span>Correção de fluxo?</span>
            <select
              value={valores.correcaoDeFluxo}
              onChange={(e) => atualizarCampo("correcaoDeFluxo", e.target.value)}
            >
              <option value="Não">Não</option>
              <option value="Sim">Sim</option>
            </select>
          </label>

          <label className="field aluno-form__checkbox">
            <input
              type="checkbox"
              checked={valores.transferido}
              onChange={(e) => atualizarCampo("transferido", e.target.checked)}
            />
            <span>Aluno transferido</span>
          </label>
        </div>

        {erro && <div className="aluno-form__error">{erro}</div>}

        <div className="aluno-form__actions">
          <button type="button" className="btn btn--ghost" onClick={onClose}>
            Cancelar
          </button>
          <button type="submit" className="btn btn--primary" disabled={salvando}>
            {salvando ? "Salvando..." : "Salvar"}
          </button>
        </div>
      </form>
    </div>
  );
}
