import "../styles/Sidebar.css";

export default function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <h3>AREA DE TRABAJO</h3>
        <p>Resolver tareas</p>
      </div>

      <div className="sidebar-section">
        <div className="sidebar-item active">Tablero</div>
        <div className="sidebar-item">Miembros</div>
        <div className="sidebar-item">Configuración</div>
      </div>
    </aside>
  );
}