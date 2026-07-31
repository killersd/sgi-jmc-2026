import { type FormEvent, useState } from "react";
import { gerarOficioGeral, gerarOficioFuncao, type OficioGeralValues, type OficioFuncaoValues } from "../api/oficios";
import "./OficiosPage.css";

type Aba = "geral" | "funcao";
type TipoFuncao = "Professor" | "Servidor" | "ApoioEscolar";

const geralInicial: OficioGeralValues = {
  numeroOficio: 1,
  assunto: "",
  destinatario: "",
  cargoDoDestinatario: "",
  corpoDoOficio: "",
  remetente: "Vera",
  cidade: "Simão Dias - SE",
};

function funcaoInicial(): OficioFuncaoValues {
  return {
    tipo: "Professor",
    numeroOficio: 1,
    assunto: "",
    destinatario: "",
    cargoDestinatario: "",
    cidadeDestinatario: "Simão Dias - SE",
    saudacaoGenero: "Senhora",
    nome: "",
    cpf: "",
    vinculo: "",
    dataAssumiuFuncao: "",
    cargaHoraria: 0,
    cargo: "",
    disciplina: "",
    fonteRecursos: 0,
  };
}

function baixarBlob(blob: Blob, nomeArquivo: string) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = nomeArquivo;
  link.click();
  window.URL.revokeObjectURL(url);
}

export default function OficiosPage() {
  const [aba, setAba] = useState<Aba>("geral");

  // --- Ofício Geral ---
  const [geral, setGeral] = useState<OficioGeralValues>(geralInicial);
  const [gerandoGeral, setGerandoGeral] = useState(false);
  const [erroGeral, setErroGeral] = useState<string | null>(null);
  const [sucessoGeral, setSucessoGeral] = useState(false);

  function atualizarGeral<K extends keyof OficioGeralValues>(campo: K, valor: OficioGeralValues[K]) {
    setGeral((prev) => ({ ...prev, [campo]: valor }));
    setSucessoGeral(false);
  }

  async function handleSubmitGeral(event: FormEvent) {
    event.preventDefault();
    setErroGeral(null);
    setGerandoGeral(true);
    try {
      const { blob, nomeArquivo } = await gerarOficioGeral(geral);
      baixarBlob(blob, nomeArquivo);
      setSucessoGeral(true);
    } catch {
      setErroGeral("Não foi possível gerar o ofício. Confira os campos preenchidos.");
    } finally {
      setGerandoGeral(false);
    }
  }

  // --- Ofício Assumiu Função ---
  const [funcao, setFuncao] = useState<OficioFuncaoValues>(funcaoInicial());
  const [gerandoFuncao, setGerandoFuncao] = useState(false);
  const [erroFuncao, setErroFuncao] = useState<string | null>(null);
  const [sucessoFuncao, setSucessoFuncao] = useState(false);

  function atualizarFuncao<K extends keyof OficioFuncaoValues>(campo: K, valor: OficioFuncaoValues[K]) {
    setFuncao((prev) => ({ ...prev, [campo]: valor }));
    setSucessoFuncao(false);
  }

  async function handleSubmitFuncao(event: FormEvent) {
    event.preventDefault();
    setErroFuncao(null);
    setGerandoFuncao(true);
    try {
      const { blob, nomeArquivo } = await gerarOficioFuncao(funcao);
      baixarBlob(blob, nomeArquivo);
      setSucessoFuncao(true);
    } catch {
      setErroFuncao("Não foi possível gerar o ofício. Confira os campos preenchidos.");
    } finally {
      setGerandoFuncao(false);
    }
  }

  const tipo: TipoFuncao = funcao.tipo;

  return (
    <div>
      <header className="page-header">
        <h1>Ofícios</h1>
        <p>Emita ofícios oficiais em PDF.</p>
      </header>

      <div className="ocorrencias-tabs">
        <button
          type="button"
          className={"ocorrencias-tab" + (aba === "geral" ? " ocorrencias-tab--ativa" : "")}
          onClick={() => setAba("geral")}
        >
          Ofício Geral
        </button>
        <button
          type="button"
          className={"ocorrencias-tab" + (aba === "funcao" ? " ocorrencias-tab--ativa" : "")}
          onClick={() => setAba("funcao")}
        >
          Assumiu Função
        </button>
      </div>

      {aba === "geral" && (
        <form className="oficio-form" onSubmit={handleSubmitGeral}>
          <div className="oficio-form__grid">
            <label className="field">
              <span>Número do ofício</span>
              <input
                type="number" required min={1}
                value={geral.numeroOficio}
                onChange={(e) => atualizarGeral("numeroOficio", Number(e.target.value))}
              />
            </label>

            <label className="field">
              <span>Remetente</span>
              <select value={geral.remetente} onChange={(e) => atualizarGeral("remetente", e.target.value)}>
                <option value="Vera">Vera Cristina Carvalho Oliveira — Diretora</option>
                <option value="Alex">Alex de Oliveira Souza — Secretário</option>
              </select>
            </label>

            <label className="field oficio-form__span2">
              <span>Assunto</span>
              <input required value={geral.assunto} onChange={(e) => atualizarGeral("assunto", e.target.value)} placeholder="Ex: Solicitação de material didático" />
            </label>

            <label className="field">
              <span>Destinatário</span>
              <input required value={geral.destinatario} onChange={(e) => atualizarGeral("destinatario", e.target.value)} placeholder="Nome do destinatário" />
            </label>

            <label className="field">
              <span>Cargo do destinatário</span>
              <input required value={geral.cargoDoDestinatario} onChange={(e) => atualizarGeral("cargoDoDestinatario", e.target.value)} placeholder="Ex: Coordenador(a) Regional" />
            </label>

            <label className="field oficio-form__span2">
              <span>Cidade do destinatário</span>
              <input required value={geral.cidade} onChange={(e) => atualizarGeral("cidade", e.target.value)} />
            </label>

            <label className="field oficio-form__span2">
              <span>Corpo do ofício</span>
              <textarea
                required rows={8}
                value={geral.corpoDoOficio}
                onChange={(e) => atualizarGeral("corpoDoOficio", e.target.value)}
                placeholder="Escreva o texto completo do ofício..."
              />
            </label>
          </div>

          {erroGeral && <div className="oficio-form__error">{erroGeral}</div>}
          {sucessoGeral && <div className="oficio-form__sucesso">Ofício gerado e baixado com sucesso.</div>}

          <div className="oficio-form__actions">
            <button type="submit" className="btn btn--primary" disabled={gerandoGeral}>
              {gerandoGeral ? "Gerando PDF..." : "🖨️ Gerar e baixar ofício"}
            </button>
          </div>
        </form>
      )}

      {aba === "funcao" && (
        <form className="oficio-form" onSubmit={handleSubmitFuncao}>
          <div className="oficio-form__grid">
            <label className="field">
              <span>Vínculo</span>
              <select value={funcao.tipo} onChange={(e) => atualizarFuncao("tipo", e.target.value as TipoFuncao)}>
                <option value="Professor">Professor</option>
                <option value="Servidor">Servidor</option>
                <option value="ApoioEscolar">Apoio Escolar</option>
              </select>
            </label>

            <label className="field">
              <span>Número do ofício</span>
              <input
                type="number" required min={1}
                value={funcao.numeroOficio}
                onChange={(e) => atualizarFuncao("numeroOficio", Number(e.target.value))}
              />
            </label>

            <label className="field oficio-form__span2">
              <span>Assunto</span>
              <input required value={funcao.assunto} onChange={(e) => atualizarFuncao("assunto", e.target.value)} placeholder="Ex: Comunicado de assunção de função" />
            </label>

            <label className="field">
              <span>Nome</span>
              <input required value={funcao.nome} onChange={(e) => atualizarFuncao("nome", e.target.value)} />
            </label>

            <label className="field">
              <span>CPF</span>
              <input required value={funcao.cpf} onChange={(e) => atualizarFuncao("cpf", e.target.value)} placeholder="000.000.000-00" />
            </label>

            <label className="field">
              <span>Vínculo (efetivo, contrato...)</span>
              <input required value={funcao.vinculo} onChange={(e) => atualizarFuncao("vinculo", e.target.value)} />
            </label>

            <label className="field">
              <span>Data que assumiu função</span>
              <input
                type="date" required
                value={funcao.dataAssumiuFuncao}
                onChange={(e) => atualizarFuncao("dataAssumiuFuncao", e.target.value)}
              />
            </label>

            <label className="field">
              <span>Carga horária</span>
              <input
                type="number" required min={1}
                value={funcao.cargaHoraria}
                onChange={(e) => atualizarFuncao("cargaHoraria", Number(e.target.value))}
              />
            </label>

            {tipo === "Servidor" && (
              <label className="field">
                <span>Cargo</span>
                <input required value={funcao.cargo ?? ""} onChange={(e) => atualizarFuncao("cargo", e.target.value)} placeholder="Ex: Auxiliar de Serviços Gerais" />
              </label>
            )}

            {tipo === "Professor" && (
              <>
                <label className="field">
                  <span>Disciplina</span>
                  <input required value={funcao.disciplina ?? ""} onChange={(e) => atualizarFuncao("disciplina", e.target.value)} />
                </label>
                <label className="field">
                  <span>Fonte de recursos FUNDEB (FRC) — opcional</span>
                  <input
                    type="number" min={0}
                    value={funcao.fonteRecursos ?? 0}
                    onChange={(e) => atualizarFuncao("fonteRecursos", Number(e.target.value))}
                  />
                </label>
              </>
            )}

            <label className="field">
              <span>Saudação</span>
              <select value={funcao.saudacaoGenero} onChange={(e) => atualizarFuncao("saudacaoGenero", e.target.value as "Senhor" | "Senhora")}>
                <option value="Senhora">Senhora Diretora</option>
                <option value="Senhor">Senhor Diretor</option>
              </select>
            </label>

            <label className="field">
              <span>Destinatário</span>
              <input required value={funcao.destinatario} onChange={(e) => atualizarFuncao("destinatario", e.target.value)} />
            </label>

            <label className="field">
              <span>Cargo do destinatário</span>
              <input required value={funcao.cargoDestinatario} onChange={(e) => atualizarFuncao("cargoDestinatario", e.target.value)} />
            </label>

            <label className="field oficio-form__span2">
              <span>Cidade do destinatário</span>
              <input required value={funcao.cidadeDestinatario} onChange={(e) => atualizarFuncao("cidadeDestinatario", e.target.value)} />
            </label>
          </div>

          {erroFuncao && <div className="oficio-form__error">{erroFuncao}</div>}
          {sucessoFuncao && <div className="oficio-form__sucesso">Ofício gerado e baixado com sucesso.</div>}

          <div className="oficio-form__actions">
            <button type="submit" className="btn btn--primary" disabled={gerandoFuncao}>
              {gerandoFuncao ? "Gerando PDF..." : "🖨️ Gerar e baixar ofício"}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}
