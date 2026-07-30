import { type FormEvent, useEffect, useState } from "react";
import { listarTurmas, criarTurma, removerTurma } from "../api/turmas";
import type { Turma } from "../types";
import { useAuth } from "../context/AuthContext";
import ConfirmDialog from "../components/ConfirmDialog";
import "./TurmasPage.css";

export default function TurmasPage() {
  const { isAdmin } = useAuth();
  const [turmas, setTurmas] = useState<Turma[]>([]);
  const [novaTurma, setNovaTurma] = useState("");
  const [carregando, setCarregando] = useState(true);
  const [salvando, setSalvando] = useState(false);
  const [erro, setErro] = useState<string | null>(null);
  const [paraExcluir, setParaExcluir] = useState<Turma | null>(null);
  const [erroExclusao, setErroExclusao] = useState<string | null>(null);

  async function carregar() {
    setCarregando(true);
    setErro(null);
    try {
      const lista = await listarTurmas();
      setTurmas(lista);
    } catch {
      setErro("Não foi possível carregar as turmas.");
    } finally {
      setCarregando(false);
    }
  }

  useEffect(() => {
    carregar();
  }, []);

  async function handleAdicionar(event: FormEvent) {
    event.preventDefault();
    const nome = novaTurma.trim();
    if (!nome) return;
    setSalvando(true);
    setErro(null);
    try {
      await criarTurma(nome);
      setNovaTurma("");
      await carregar();
    } catch {
      setErro("Não foi possível adicionar a turma.");
    } finally {
      setSalvando(false);
    }
  }

  async function confirmarExclusao() {
    if (!paraExcluir) return;
    setErroExclusao(null);
    try {
      await removerTurma(paraExcluir.id);
      setParaExcluir(null);
      await carregar();
    } catch {
      setErroExclusao("Não foi possível excluir esta turma. Ela pode estar em uso em algum horário.");
    }
  }

  return (
    <div>
      <header className="page-header">
        <h1>Turmas</h1>
        <p>Catálogo de turmas usado no seletor da grade de horários.</p>
      </header>

      <form className="turmas-form" onSubmit={handleAdicionar}>
        <label className="field turmas-form__campo">
          <span>Nova turma</span>
          <input
            value={novaTurma}
            onChange={(e) => setNovaTurma(e.target.value)}
            placeholder="Ex: 6º Ano A"
          />
        </label>
        <button type="submit" className="btn btn--primary" disabled={salvando || !novaTurma.trim()}>
          {salvando ? "Adicionando..." : "+ Adicionar turma"}
        </button>
      </form>

      {erro && <div className="turmas-page__error">{erro}</div>}

      <div className="turmas-lista">
        {carregando && <p className="turmas-page__estado">Carregando...</p>}
        {!carregando && turmas.length === 0 && (
          <p className="turmas-page__estado">Nenhuma turma cadastrada ainda.</p>
        )}
        {!carregando && turmas.map((t) => (
          <div key={t.id} className="turma-item">
            <span>{t.nome}</span>
            {isAdmin && (
              <button className="btn btn--ghost btn--sm" onClick={() => setParaExcluir(t)}>
                Excluir
              </button>
            )}
          </div>
        ))}
      </div>

      {paraExcluir && (
        <ConfirmDialog
          title="Excluir turma"
          message={erroExclusao ?? `Tem certeza que deseja excluir "${paraExcluir.nome}"?`}
          confirmLabel="Excluir"
          danger
          onConfirm={confirmarExclusao}
          onCancel={() => { setParaExcluir(null); setErroExclusao(null); }}
        />
      )}
    </div>
  );
}
