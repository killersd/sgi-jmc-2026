import { type FormEvent, useState } from "react";
import { buscarAlunoPorCodigo } from "../api/declaracoes";
import { gerarAdvertencia, gerarSuspensao } from "../api/ocorrencias";
import type { AlunoParaDeclaracao } from "../types";
import CodigoBadge from "../components/CodigoBadge";
import "./OcorrenciasPage.css";

type Tipo = "advertencia" | "suspensao";

function baixarBlob(blob: Blob, nomeArquivo: string) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = nomeArquivo;
  link.click();
  window.URL.revokeObjectURL(url);
}

export default function OcorrenciasPage() {
  const [codigo, setCodigo] = useState("");
  const [aluno, setAluno] = useState<AlunoParaDeclaracao | null>(null);
  const [buscando, setBuscando] = useState(false);
  const [erroBusca, setErroBusca] = useState<string | null>(null);

  const [tipo, setTipo] = useState<Tipo>("advertencia");
  const [turno, setTurno] = useState("Matutino");
  const [descricaoDoFato, setDescricaoDoFato] = useState("");
  const [numero, setNumero] = useState(1);
  const [dias, setDias] = useState(1);
  const [numeroSuspensao, setNumeroSuspensao] = useState(1);

  const [gerando, setGerando] = useState(false);
  const [erroGerar, setErroGerar] = useState<string | null>(null);

  async function handleBuscar(event: FormEvent) {
    event.preventDefault();
    setErroBusca(null);
    setAluno(null);
    setBuscando(true);
    try {
      const resultado = await buscarAlunoPorCodigo(codigo.trim());
      setAluno(resultado);
    } catch {
      setErroBusca("Nenhum aluno encontrado com esse código.");
    } finally {
      setBuscando(false);
    }
  }

  async function handleGerar(event: FormEvent) {
    event.preventDefault();
    if (!aluno) return;
    setErroGerar(null);
    setGerando(true);
    try {
      if (tipo === "advertencia") {
        const { blob, nomeArquivo } = await gerarAdvertencia({
          codigoSeed: aluno.codigoSeed, turno, descricaoDoFato, numero,
        });
        baixarBlob(blob, nomeArquivo);
      } else {
        const { blob, nomeArquivo } = await gerarSuspensao({
          codigoSeed: aluno.codigoSeed, turno, descricaoDoFato, dias, numeroSuspensao,
        });
        baixarBlob(blob, nomeArquivo);
      }
    } catch {
      setErroGerar("Não foi possível gerar o documento. Confira os campos preenchidos.");
    } finally {
      setGerando(false);
    }
  }

  return (
    <div>
      <header className="page-header">
        <h1>Advertências e Suspensões</h1>
        <p>Busque o aluno pelo código de matrícula para emitir o documento.</p>
      </header>

      <form className="ocorrencias-busca" onSubmit={handleBuscar}>
        <label className="field ocorrencias-busca__campo">
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

      {erroBusca && <div className="ocorrencias-page__error">{erroBusca}</div>}

      {aluno && (
        <div className="aluno-card">
          <div className="aluno-card__header">
            <div>
              <h2>{aluno.nome}</h2>
              <CodigoBadge codigo={aluno.codigoSeed} />
            </div>
            <span className="badge badge--success">Encontrado</span>
          </div>

          <div className="ocorrencias-tabs">
            <button
              type="button"
              className={"ocorrencias-tab" + (tipo === "advertencia" ? " ocorrencias-tab--ativa" : "")}
              onClick={() => setTipo("advertencia")}
            >
              Advertência
            </button>
            <button
              type="button"
              className={"ocorrencias-tab" + (tipo === "suspensao" ? " ocorrencias-tab--ativa" : "")}
              onClick={() => setTipo("suspensao")}
            >
              Suspensão
            </button>
          </div>

          <form onSubmit={handleGerar}>
            <div className="ocorrencias-form__grid">
              <label className="field">
                <span>Turno</span>
                <select value={turno} onChange={(e) => setTurno(e.target.value)}>
                  <option value="Matutino">Matutino</option>
                  <option value="Vespertino">Vespertino</option>
                </select>
              </label>

              {tipo === "advertencia" && (
                <label className="field">
                  <span>Número da advertência</span>
                  <input type="number" min={1} required value={numero} onChange={(e) => setNumero(Number(e.target.value))} />
                </label>
              )}

              {tipo === "suspensao" && (
                <>
                  <label className="field">
                    <span>Número da suspensão</span>
                    <input type="number" min={1} required value={numeroSuspensao} onChange={(e) => setNumeroSuspensao(Number(e.target.value))} />
                  </label>
                  <label className="field">
                    <span>Quantidade de dias</span>
                    <input type="number" min={1} required value={dias} onChange={(e) => setDias(Number(e.target.value))} />
                  </label>
                </>
              )}

              <label className="field ocorrencias-form__span2">
                <span>Descrição do fato</span>
                <textarea
                  required
                  rows={6}
                  value={descricaoDoFato}
                  onChange={(e) => setDescricaoDoFato(e.target.value)}
                  placeholder="Descreva o ocorrido..."
                />
              </label>
            </div>

            {erroGerar && <div className="ocorrencias-page__error">{erroGerar}</div>}

            <div className="ocorrencias-form__actions">
              <button type="submit" className="btn btn--primary" disabled={gerando}>
                {gerando ? "Gerando PDF..." : `🖨️ Emitir ${tipo === "advertencia" ? "advertência" : "suspensão"}`}
              </button>
            </div>
          </form>
        </div>
      )}
    </div>
  );
}
