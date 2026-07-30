import "./CodigoBadge.css";

export default function CodigoBadge({ codigo }: { codigo: string }) {
  return <span className="codigo-badge">{codigo}</span>;
}
