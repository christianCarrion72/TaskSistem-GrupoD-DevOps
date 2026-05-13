import "../styles/Header.css";

export default function Header() {
  return (
    <header className="trello-header">
      <div className="header-left">
        <div className="header-logo">Tareas</div>

        <nav className="header-menu">
          <span>Espacios de trabajo</span>
          <span>Reciente</span>
          <span>Crear</span>
        </nav>
      </div>

      <div className="header-right">
        <input className="header-search" placeholder="Buscar" />
        <div className="header-avatar">D</div>
      </div>
    </header>
  );
}