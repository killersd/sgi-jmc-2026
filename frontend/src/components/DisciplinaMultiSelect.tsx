import { useState } from "react";
import type { Disciplina } from "../types";
import { criarDisciplina } from "../api/disciplinas";
import "./DisciplinaMultiSelect.css";

interface DisciplinaMultiSelectProps {
  disciplinas: Disciplina[];
  selecionadas: number[];
  onChange: (ids: number[]) => void;
  onDisciplinaCriada: (disciplina: Disciplina) => void;
}

export default function DisciplinaMultiSelect({
  disciplinas, selecionadas, onChange, onDisciplinaCriada,
}: DisciplinaMultiSelectProps) {
  const [novaDisciplina, setNovaDisciplina] = useState("");
  const [criando, setCriando] = useState(false);

  function alternar(id: number) {
    if (selecionadas.includes(id)) {
      onChange(selecionadas.filter((i) => i !== id));
    } else {
      onChange([...selecionadas, id]);
    }
  }

  async function handleCriarDisciplina() {
    const nome = novaDisciplina.trim();
    if (!nome) return;
    setCriando(true);
    try {
      const disciplina = await criarDisciplina(nome);
      onDisciplinaCriada(disciplina);
      onChange([...selecionadas, disciplina.id]);
      setNovaDisciplina("");
    } finally {
      setCriando(false);
    }
  }

  return (
    <div className="disciplina-select">
      <div className="disciplina-select__chips">
        {disciplinas.length === 0 && (
          <span className="disciplina-select__vazio">Nenhuma disciplina cadastrada ainda.</span>
        )}
        {disciplinas.map((d) => (
          <button
            key={d.id}
            type="button"
            className={"disciplina-chip" + (selecionadas.includes(d.id) ? " disciplina-chip--ativo" : "")}
            onClick={() => alternar(d.id)}
          >
            {d.nome}
          </button>
        ))}
      </div>

      <div className="disciplina-select__nova">
        <input
          value={novaDisciplina}
          onChange={(e) => setNovaDisciplina(e.target.value)}
          placeholder="Adicionar nova disciplina..."
          onKeyDown={(e) => {
            if (e.key === "Enter") { e.preventDefault(); handleCriarDisciplina(); }
          }}
        />
        <button
          type="button"
          className="btn btn--ghost btn--sm"
          onClick={handleCriarDisciplina}
          disabled={criando || !novaDisciplina.trim()}
        >
          + Adicionar
        </button>
      </div>
    </div>
  );
}
