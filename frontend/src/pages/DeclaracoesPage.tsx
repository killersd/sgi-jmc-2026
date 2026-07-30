import { type FormEvent, useState } from "react";
import { buscarAlunoPorCodigo, imprimirDeclaracaoFrequencia } from "../api/declaracoes";
import type { AlunoParaDeclaracao } from "../types";
import CodigoBadge from "../components/CodigoBadge";
import "./DeclaracoesPage.css";

function formatarData(iso: string): string {
  const data = new Date(iso);
  return data.toLocaleDateString("pt-BR", { timeZone: "UTC" });
}

export default function DeclaracoesPage() {
  const [codigo, setCodigo] = useState("");
  const [aluno, setAluno] = useState<AlunoParaDeclaracao | null>(null);
  const [qtdFaltas, setQtdFaltas] = useState(0);
  const [buscando, setBuscando] = useState(false);
  const [imprimindo, setImprimindo] = useState(false);
  const [erro, setErro] = useState<string | null>(null);

  async function handleBuscar(event: FormEvent) {
    event.preventDefault();
    setErro(null);
    setAluno(null);
    setBuscando(true);
    try {
      const resultado = await buscarAlunoPorCodigo(codigo.trim());
      setAluno(resultado);
      setQtdFaltas(0);
    } catch {
      setErro("Nenhum aluno encontrado com esse código.");
    } finally {
      setBuscando(false);
    }
  }

  async function handleImprimir() {
    if (!aluno) return;
    setErro(null);
    setImprimindo(true);
    try {
      const { blob, nomeArquivo } = await imprimirDeclaracaoFrequencia(aluno.codigoSeed, qtdFaltas);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = nomeArquivo;
      link.click();
      window.URL.revokeObjectURL(url);
    } catch {
      setErro("Não foi possível gerar a declaração. Verifique se o aluno tem ano/série e turma cadastrados.");
    } finally {
      setImprimindo(false);
    }
  }

  return (
    <div>
      <header className="page-header">
        <h1>Declarações</h1>
        <p>Busque o aluno pelo código de matrícula para emitir a declaração de frequência.</p>
      </header>

      <form className="declaracoes-busca" onSubmit={handleBuscar}>
        <label className="field declaracoes-busca__campo">
          <span>Código do aluno</span>
          <input
            required
            autoFocus
            value={codigo}
            onChange={(e) => setCodigo(e.target.value)}
            placeholder="Ex: MAT-2026-0001"
          />
        </label>
        <button type="submit" className="btn btn--primary" disabled={buscando}>
          {buscando ? "Buscando..." : "Buscar aluno"}
        </button>
      </form>

      {erro && <div className="declaracoes-page__error">{erro}</div>}

      {aluno && (
        <div className="aluno-card">
          <div className="aluno-card__header">
            <div>
              <h2>{aluno.nome}</h2>
              <CodigoBadge codigo={aluno.codigoSeed} />
            </div>
            <span className="badge badge--success">Encontrado</span>
          </div>

          <dl className="aluno-card__grid">
            <div><dt>Mãe</dt><dd>{aluno.mae}</dd></div>
            <div><dt>Pai</dt><dd>{aluno.pai || "—"}</dd></div>
            <div><dt>Data de nascimento</dt><dd>{formatarData(aluno.dataNascimento)}</dd></div>
            <div><dt>NIS</dt><dd>{aluno.numeroDoNis || "—"}</dd></div>
            <div><dt>Ano letivo</dt><dd>{aluno.anoLetivo}</dd></div>
            <div><dt>Série / Turma</dt><dd>{aluno.anoSerie ? `${aluno.anoSerie}º ${aluno.turma ?? ""}` : "—"}</dd></div>
          </dl>

          <div className="aluno-card__acao">
            <label className="field aluno-card__faltas">
              <span>Quantidade de faltas (carga horária anual: 833h)</span>
              <input
                type="number"
                min={0}
                value={qtdFaltas}
                onChange={(e) => setQtdFaltas(Number(e.target.value))}
              />
            </label>
            <button className="btn btn--primary" onClick={handleImprimir} disabled={imprimindo}>
              {imprimindo ? "Gerando PDF..." : "🖨️ Imprimir declaração"}
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
