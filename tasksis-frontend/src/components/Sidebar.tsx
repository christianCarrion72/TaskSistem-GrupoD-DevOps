import "../styles/Sidebar.css";

interface SidebarProps {
  seccionActiva: "tablero" | "miembros" | "configuracion";
  onSeccionChange: (seccion: "tablero" | "miembros" | "configuracion") => void;
}

export default function Sidebar({ seccionActiva, onSeccionChange }: SidebarProps) {
  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <h3>AREA DE TRABAJO</h3>
        <p>Resolver tareas</p>
      </div>

      <div className="sidebar-section">
        <div
          className={`sidebar-item ${seccionActiva === "tablero" ? "active" : ""}`}
          onClick={() => onSeccionChange("tablero")}
        >
          Tablero
        </div>
        <div
          className={`sidebar-item ${seccionActiva === "miembros" ? "active" : ""}`}
          onClick={() => onSeccionChange("miembros")}
        >
          Miembros
        </div>
        <div
          className={`sidebar-item ${seccionActiva === "configuracion" ? "active" : ""}`}
          onClick={() => onSeccionChange("configuracion")}
        >
          Configuración
        </div>
      </div>
    </aside>
  );
}