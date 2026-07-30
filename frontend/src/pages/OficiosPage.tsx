import { type FormEvent, useState } from "react";
import { gerarOficioGeral, type OficioGeralValues } from "../api/oficios";
import "./OficiosPage.css";

const valoresIniciais: OficioGeralValues = {
  numeroOficio: 1,
  assunto: "",
  destinatario: "",
  cargoDoDestinatario: "",
  corpoDoOficio: "",
  remetente: "Vera",
  cidade: "Simão Dias - SE",
};

export default function OficiosPage() {
  const [valores, setValores] = useState<OficioGeralValues>(valoresIniciais);
  const [gerando, setGerando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [sucesso, setSucesso] = useState(false);

  function atualizarCampo<K extends keyof OficioGeralValues>(campo: K, valor: OficioGeralValues[K]) {
    setValores((prev) => ({ ...prev, [campo]: valor }));
    setSucesso(false);
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErro(null);
    setGerando(true);
    try {
      const { blob, nomeArquivo } = await gerarOficioGeral(valores);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.download = nomeArquivo;
      link.click();
      window.URL.revokeObjectURL(url);
      setSucesso(true);
    } catch {
      setErro("Não foi possível gerar o ofício. Confira os campos preenchidos.");
    } finally {
      setGerando(false);
    }
  }

  return (
    <div>
      <header className="page-header">
        <h1>Ofício Geral</h1>
        <p>Preencha os dados para emitir uma carta oficial em PDF.</p>
      </header>

      <form className="oficio-form" onSubmit={handleSubmit}>
        <div className="oficio-form__grid">
          <label className="field">
            <span>Número do ofício</span>
            <input
              type="number"
              required
              min={1}
              value={valores.numeroOficio}
              onChange={(e) => atualizarCampo("numeroOficio", Number(e.target.value))}
            />
          </label>

          <label className="field">
            <span>Remetente</span>
            <select
              value={valores.remetente}
              onChange={(e) => atualizarCampo("remetente", e.target.value)}
            >
              <option value="Vera">Vera Cristina Carvalho Oliveira — Diretora</option>
              <option value="Alex">Alex de Oliveira Souza — Secretário</option>
            </select>
          </label>

          <label className="field oficio-form__span2">
            <span>Assunto</span>
            <input
              required
              value={valores.assunto}
              onChange={(e) => atualizarCampo("assunto", e.target.value)}
              placeholder="Ex: Solicitação de material didático"
            />
          </label>

          <label className="field">
            <span>Destinatário</span>
            <input
              required
              value={valores.destinatario}
              onChange={(e) => atualizarCampo("destinatario", e.target.value)}
              placeholder="Nome do destinatário"
            />
          </label>

          <label className="field">
            <span>Cargo do destinatário</span>
            <input
              required
              value={valores.cargoDoDestinatario}
              onChange={(e) => atualizarCampo("cargoDoDestinatario", e.target.value)}
              placeholder="Ex: Coordenador(a) Regional"
            />
          </label>

          <label className="field oficio-form__span2">
            <span>Cidade do destinatário</span>
            <input
              required
              value={valores.cidade}
              onChange={(e) => atualizarCampo("cidade", e.target.value)}
            />
          </label>

          <label className="field oficio-form__span2">
            <span>Corpo do ofício</span>
            <textarea
              required
              rows={8}
              value={valores.corpoDoOficio}
              onChange={(e) => atualizarCampo("corpoDoOficio", e.target.value)}
              placeholder="Escreva o texto completo do ofício..."
            />
          </label>
        </div>

        {erro && <div className="oficio-form__error">{erro}</div>}
        {sucesso && <div className="oficio-form__sucesso">Ofício gerado e baixado com sucesso.</div>}

        <div className="oficio-form__actions">
          <button type="submit" className="btn btn--primary" disabled={gerando}>
            {gerando ? "Gerando PDF..." : "🖨️ Gerar e baixar ofício"}
          </button>
        </div>
      </form>
    </div>
  );
}
