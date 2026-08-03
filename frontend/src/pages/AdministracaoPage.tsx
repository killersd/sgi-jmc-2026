import { type FormEvent, useEffect, useState } from "react";
import { obterMatrizPermissoes, atualizarPermissoes } from "../api/perfis";
import {
  listarUsuarios,
  atualizarPerfilUsuario,
  removerUsuario,
  criarUsuario,
  editarUsuario,
  atualizarStatusUsuario,
} from "../api/usuarios";
import { obterConfiguracaoNotificacoes, atualizarConfiguracaoNotificacoes } from "../api/configuracoes";
import { PERFIS_DISPONIVEIS, type MatrizPermissoes, type UsuarioAdmin } from "../types";
import { useAuth } from "../context/AuthContext";
import ConfirmDialog from "../components/ConfirmDialog";
import EditarUsuarioDialog from "../components/EditarUsuarioDialog";
import "./AdministracaoPage.css";

type Aba = "permissoes" | "usuarios" | "notificacoes";

function chavePermissao(perfil: string, moduloChave: string) {
  return `${perfil}:${moduloChave}`;
}

export default function AdministracaoPage() {
  const { isAdmin } = useAuth();
  const [aba, setAba] = useState<Aba>("permissoes");

  return (
    <div>
      <header className="page-header">
        <h1>Administração</h1>
        <p>Controle de acesso por perfil e gestão de usuários do sistema.</p>
      </header>

      <div className="ocorrencias-tabs">
        <button
          type="button"
          className={"ocorrencias-tab" + (aba === "permissoes" ? " ocorrencias-tab--ativa" : "")}
          onClick={() => setAba("permissoes")}
        >
          Permissões por perfil
        </button>
        {isAdmin && (
          <button
            type="button"
            className={"ocorrencias-tab" + (aba === "usuarios" ? " ocorrencias-tab--ativa" : "")}
            onClick={() => setAba("usuarios")}
          >
            Usuários
          </button>
        )}
        {isAdmin && (
          <button
            type="button"
            className={"ocorrencias-tab" + (aba === "notificacoes" ? " ocorrencias-tab--ativa" : "")}
            onClick={() => setAba("notificacoes")}
          >
            Notificações
          </button>
        )}
      </div>

      {aba === "permissoes" && <PainelPermissoes />}
      {aba === "usuarios" && isAdmin && <PainelUsuarios />}
      {aba === "notificacoes" && isAdmin && <PainelNotificacoes />}
    </div>
  );
}

function PainelPermissoes() {
  const [matriz, setMatriz] = useState<MatrizPermissoes | null>(null);
  const [estado, setEstado] = useState<Record<string, boolean>>({});
  const [carregando, setCarregando] = useState(true);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [sucesso, setSucesso] = useState(false);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      const dados = await obterMatrizPermissoes();
      setMatriz(dados);
      const mapa: Record<string, boolean> = {};
      for (const perfil of dados.perfisEditaveis) {
        for (const modulo of dados.modulos) {
          const encontrada = dados.permissoes.find(
            (p) => p.perfil === perfil.chave && p.moduloChave === modulo.chave
          );
          mapa[chavePermissao(perfil.chave, modulo.chave)] = encontrada?.permitido ?? false;
        }
      }
      setEstado(mapa);
    } catch {
      setErro("Não foi possível carregar as permissões.");
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  function alternar(perfil: string, moduloChave: string) {
    const chave = chavePermissao(perfil, moduloChave);
    setEstado((prev) => ({ ...prev, [chave]: !prev[chave] }));
    setSucesso(false);
  }

  async function salvar() {
    if (!matriz) return;
    setSalvando(true);
    setErro(null);
    try {
      const permissoes = matriz.perfisEditaveis.flatMap((perfil) =>
        matriz.modulos.map((modulo) => ({
          perfil: perfil.chave,
          moduloChave: modulo.chave,
          permitido: estado[chavePermissao(perfil.chave, modulo.chave)] ?? false,
        }))
      );
      await atualizarPermissoes(permissoes);
      setSucesso(true);
    } catch {
      setErro("Não foi possível salvar as permissões.");
    } finally {
      setSalvando(false);
    }
  }

  if (carregando) return <p className="horarios-page__estado">Carregando...</p>;
  if (erro && !matriz) return <div className="ocorrencias-page__error">{erro}</div>;
  if (!matriz) return null;

  return (
    <div className="permissoes-painel">
      <p className="permissoes-painel__aviso">
        Marque os módulos que cada perfil pode acessar. Perfis não listados aqui você não tem permissão para configurar.
      </p>

      <div className="permissoes-tabela__scroll">
        <table className="data-table permissoes-tabela">
          <thead>
            <tr>
              <th>Módulo</th>
              {matriz.perfisEditaveis.map((p) => <th key={p.chave}>{p.nome}</th>)}
            </tr>
          </thead>
          <tbody>
            {matriz.modulos.map((modulo) => (
              <tr key={modulo.chave}>
                <td>{modulo.nome}</td>
                {matriz.perfisEditaveis.map((perfil) => (
                  <td key={perfil.chave} className="permissoes-tabela__celula">
                    <input
                      type="checkbox"
                      checked={estado[chavePermissao(perfil.chave, modulo.chave)] ?? false}
                      onChange={() => alternar(perfil.chave, modulo.chave)}
                    />
                  </td>
                ))}
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {erro && <div className="ocorrencias-page__error">{erro}</div>}
      {sucesso && <div className="oficio-form__sucesso">Permissões salvas com sucesso.</div>}

      <div className="permissoes-painel__actions">
        <button className="btn btn--primary" onClick={salvar} disabled={salvando}>
          {salvando ? "Salvando..." : "Salvar alterações"}
        </button>
      </div>
    </div>
  );
}

function PainelUsuarios() {
  const [usuarios, setUsuarios] = useState<UsuarioAdmin[]>([]);
  const [carregando, setCarregando] = useState(true);
  const [erro, setErro] = useState<string | null>(null);
  const [paraExcluir, setParaExcluir] = useState<UsuarioAdmin | null>(null);
  const [paraEditar, setParaEditar] = useState<UsuarioAdmin | null>(null);

  const [nomeCompleto, setNomeCompleto] = useState("");
  const [email, setEmail] = useState("");
  const [perfil, setPerfil] = useState("secretario");
  const [criando, setCriando] = useState(false);
  const [erroCriar, setErroCriar] = useState<string | null>(null);
  const [sucessoCriar, setSucessoCriar] = useState<string | null>(null);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      setUsuarios(await listarUsuarios());
    } catch {
      setErro("Não foi possível carregar os usuários.");
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  async function handleMudarPerfil(usuario: UsuarioAdmin, novoPerfil: string) {
    await atualizarPerfilUsuario(usuario.id, novoPerfil);
    await carregar();
  }

  async function confirmarExclusao() {
    if (!paraExcluir) return;
    await removerUsuario(paraExcluir.id);
    setParaExcluir(null);
    await carregar();
  }

  async function handleSalvarEdicao(valores: { nomeCompleto: string; email: string; cpf: string; dataNascimento: string }) {
    if (!paraEditar) return;
    await editarUsuario(paraEditar.id, valores);
    setParaEditar(null);
    await carregar();
  }

  async function handleAlternarStatus(usuario: UsuarioAdmin) {
    await atualizarStatusUsuario(usuario.id, !usuario.ativo);
    await carregar();
  }

  async function handleCriar(event: FormEvent) {
    event.preventDefault();
    setErroCriar(null);
    setSucessoCriar(null);
    setCriando(true);
    try {
      const criado = await criarUsuario({ nomeCompleto, email, perfil });
      setNomeCompleto(""); setEmail("");
      setSucessoCriar(
        criado.emailEnviado
          ? `Usuário criado! Um e-mail de ativação foi enviado para ${criado.email}.`
          : `Usuário criado, mas não foi possível enviar o e-mail de ativação para ${criado.email}.`
      );
      await carregar();
    } catch {
      setErroCriar("Não foi possível criar o usuário. Verifique se o e-mail já está em uso.");
    } finally {
      setCriando(false);
    }
  }

  return (
    <div>
      <form className="usuario-novo-form" onSubmit={handleCriar}>
        <h3>Novo usuário</h3>
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
            <span>Perfil</span>
            <select value={perfil} onChange={(e) => setPerfil(e.target.value)}>
              {PERFIS_DISPONIVEIS.map((p) => (
                <option key={p.chave} value={p.chave}>{p.nome}</option>
              ))}
            </select>
          </label>
        </div>
        {erroCriar && <div className="ocorrencias-page__error">{erroCriar}</div>}
        {sucessoCriar && <div className="oficio-form__sucesso">{sucessoCriar}</div>}
        <div className="usuario-novo-form__actions">
          <button type="submit" className="btn btn--primary" disabled={criando}>
            {criando ? "Criando..." : "+ Criar usuário"}
          </button>
        </div>
      </form>

      {erro && <div className="ocorrencias-page__error">{erro}</div>}

      <div className="usuarios-lista">
        {carregando && <p className="horarios-page__estado">Carregando...</p>}
        {!carregando && usuarios.map((u) => (
          <div key={u.id} className="usuario-item">
            <div>
              <strong>{u.nomeCompleto}</strong>
              <p className="usuario-item__email">{u.email}</p>
            </div>
            <div className="usuario-item__badges">
              <span className={"badge " + (u.emailConfirmado ? "badge--success" : "badge--warning")}>
                {u.emailConfirmado ? "E-mail confirmado" : "E-mail pendente"}
              </span>
              <span className={"badge " + (u.ativo ? "badge--success" : "badge--muted")}>
                {u.ativo ? "Conta ativa" : "Conta inativa"}
              </span>
            </div>
            <div className="usuario-item__acoes">
              <select
                value={u.perfis[0] ?? ""}
                onChange={(e) => handleMudarPerfil(u, e.target.value)}
              >
                {PERFIS_DISPONIVEIS.map((p) => (
                  <option key={p.chave} value={p.chave}>{p.nome}</option>
                ))}
              </select>
              <button className="btn btn--ghost btn--sm" onClick={() => setParaEditar(u)}>Editar</button>
              <button className="btn btn--ghost btn--sm" onClick={() => handleAlternarStatus(u)}>
                {u.ativo ? "Inativar" : "Reativar"}
              </button>
              <button className="btn btn--ghost btn--sm" onClick={() => setParaExcluir(u)}>Excluir</button>
            </div>
          </div>
        ))}
      </div>

      {paraExcluir && (
        <ConfirmDialog
          title="Excluir usuário"
          message={`Tem certeza que deseja excluir "${paraExcluir.nomeCompleto}"?`}
          confirmLabel="Excluir"
          danger
          onConfirm={confirmarExclusao}
          onCancel={() => setParaExcluir(null)}
        />
      )}

      {paraEditar && (
        <EditarUsuarioDialog
          usuario={paraEditar}
          onSalvar={handleSalvarEdicao}
          onCancel={() => setParaEditar(null)}
        />
      )}
    </div>
  );
}

function PainelNotificacoes() {
  const [email, setEmail] = useState("");
  const [carregando, setCarregando] = useState(true);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [sucesso, setSucesso] = useState(false);

  useEffect(() => {
    (async () => {
      setCarregando(true);
      setErro(null);
      try {
        const config = await obterConfiguracaoNotificacoes();
        setEmail(config.emailNotificacaoTransferencia ?? "");
      } catch {
        setErro("Não foi possível carregar as configurações de notificação.");
      } finally {
        setCarregando(false);
      }
    })();
  }, []);

  async function salvar(event: FormEvent) {
    event.preventDefault();
    setSalvando(true);
    setErro(null);
    setSucesso(false);
    try {
      await atualizarConfiguracaoNotificacoes(email.trim());
      setSucesso(true);
    } catch {
      setErro("Não foi possível salvar a configuração.");
    } finally {
      setSalvando(false);
    }
  }

  if (carregando) return <p className="horarios-page__estado">Carregando...</p>;

  return (
    <form className="usuario-novo-form" onSubmit={salvar}>
      <h3>Notificações de transferência</h3>
      <p className="permissoes-painel__aviso">
        Sempre que uma declaração de transferência for emitida, uma notificação será enviada para este e-mail.
      </p>
      <div className="usuario-novo-form__grid">
        <label className="field">
          <span>E-mail de notificação</span>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="secretaria@escola.gov.br"
          />
        </label>
      </div>
      {erro && <div className="ocorrencias-page__error">{erro}</div>}
      {sucesso && <div className="oficio-form__sucesso">Configuração salva com sucesso.</div>}
      <div className="usuario-novo-form__actions">
        <button type="submit" className="btn btn--primary" disabled={salvando}>
          {salvando ? "Salvando..." : "Salvar"}
        </button>
      </div>
    </form>
  );
}
