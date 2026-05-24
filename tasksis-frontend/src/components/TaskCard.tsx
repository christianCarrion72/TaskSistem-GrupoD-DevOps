import { useState, useEffect, useRef } from "react";
import { createPortal } from "react-dom";
import "../styles/TaskCard.css";
import type { Tarea } from "../types/Tarea";
import type { Miembro } from "../types/Miembro";
import { getUsuarios } from "../services/UsuariosApi";

const API_URL = "http://localhost:5255/api/tareas";
const CLAVE_ASIGNACIONES = "tasksis_asignaciones";

const COLORES = ["#1976d2", "#388e3c", "#f57c00", "#7b1fa2", "#d32f2f", "#0097a7", "#e64a19", "#5d3b66"];

interface Props {
  tarea: Tarea;
  onMoveLeft?: (id: number) => void;
  onMoveRight?: (id: number) => void;
}

export default function TaskCard({ tarea, onMoveLeft, onMoveRight }: Props) {
  const [miembros, setMiembros] = useState<Miembro[]>([]);
  const [miembroAsignado, setMiembroAsignado] = useState<Miembro | null>(null);
  const [showDropdown, setShowDropdown] = useState(false);
  const [coords, setCoords] = useState<{ top: number; left: number } | null>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    const cargar = async () => {
      try {
        const lista = await getUsuarios();
        setMiembros(lista);

        // Prioridad 1: el campo que viene directamente de la BD
        if (tarea.usuarioAsignadoId) {
          const encontrado = lista.find((m) => m.id === tarea.usuarioAsignadoId);
          if (encontrado) {
            setMiembroAsignado(encontrado);
            return;
          }
        }

        // Prioridad 2: localStorage (fallback)
        const asignacionesGuardadas = localStorage.getItem(CLAVE_ASIGNACIONES);
        if (asignacionesGuardadas) {
          try {
            const mapa = JSON.parse(asignacionesGuardadas);
            const idAsignado = mapa[tarea.id];
            if (idAsignado) {
              const encontrado = lista.find((m) => m.id === idAsignado);
              if (encontrado) setMiembroAsignado(encontrado);
            }
          } catch (e) {
            console.error("Error al leer asignaciones guardadas", e);
          }
        }
      } catch (error) {
        console.error("Error al cargar miembros para asignación", error);
      }
    };

    cargar();
  }, [tarea.id, tarea.usuarioAsignadoId]);

  useEffect(() => {
    if (!showDropdown) return;
    const cerrar = () => setShowDropdown(false);
    document.addEventListener("click", cerrar);
    window.addEventListener("scroll", cerrar, true);
    return () => {
      document.removeEventListener("click", cerrar);
      window.removeEventListener("scroll", cerrar, true);
    };
  }, [showDropdown]);

  const toggleDropdown = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (!showDropdown && buttonRef.current) {
      const rect = buttonRef.current.getBoundingClientRect();
      const anchoDropdown = 190;
      let left = rect.right - anchoDropdown;
      if (left < 4) left = 4;

      const cantidadItems = miembros.length + 1;
      const alturaDropdown = Math.min(cantidadItems * 36 + 8, 220);

      let top = rect.bottom + 4;
      if (top + alturaDropdown > window.innerHeight - 8) {
        top = rect.top - alturaDropdown - 4;
      }
      if (top < 4) top = rect.bottom + 4;

      setCoords({ top, left });
    }
    setShowDropdown((prev) => !prev);
  };

  const asignarMiembro = async (miembro: Miembro | null) => {
    setMiembroAsignado(miembro);
    setShowDropdown(false);

    // Guardar en localStorage
    const asignacionesGuardadas = localStorage.getItem(CLAVE_ASIGNACIONES);
    let mapa: Record<number, number> = {};
    if (asignacionesGuardadas) {
      try { mapa = JSON.parse(asignacionesGuardadas); }
      catch { mapa = {}; }
    }
    if (miembro) { mapa[tarea.id] = miembro.id; }
    else { delete mapa[tarea.id]; }
    localStorage.setItem(CLAVE_ASIGNACIONES, JSON.stringify(mapa));

    // Persistir en la base de datos via API
    try {
      await fetch(`${API_URL}/${tarea.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          Nombre: tarea.nombre,
          Descripcion: tarea.descripcion,
          Estado: tarea.estado
            ? tarea.estado.charAt(0).toUpperCase() + tarea.estado.slice(1).toLowerCase()
            : "Pendiente",
          UsuarioAsignadoId: miembro ? miembro.id : null,
        }),
      });
    } catch (error) {
      console.error("Error al guardar asignación en la BD", error);
    }
  };

  const obtenerInicial = (nombre: string) => nombre.trim().charAt(0).toUpperCase();

  const obtenerColorAvatar = (nombre: string) => {
    let hash = 0;
    for (let i = 0; i < nombre.length; i++) {
      hash = nombre.charCodeAt(i) + ((hash << 5) - hash);
    }
    return COLORES[Math.abs(hash) % COLORES.length];
  };

  const iconoSinAsignar = (
    <svg viewBox="0 0 24 24" width="13" height="13">
      <path fill="currentColor" d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 3c1.66 0 3 1.34 3 3s-1.34 3-3 3-3-1.34-3-3 1.34-3 3-3zm0 14.2c-2.5 0-4.71-1.28-6-3.22.03-1.99 4-3.08 6-3.08 1.99 0 5.97 1.09 6 3.08-1.29 1.94-3.5 3.22-6 3.22z" />
    </svg>
  );

  const dropdown = showDropdown && coords
    ? createPortal(
        <div
          className="assignee-dropdown"
          style={{ position: "fixed", top: `${coords.top}px`, left: `${coords.left}px` }}
          onClick={(e) => e.stopPropagation()}
        >
          {/* quitar la asignación */}
          <div className="dropdown-item" onClick={() => asignarMiembro(null)}>
            <span className="assignee-avatar unassigned">{iconoSinAsignar}</span>
            <span style={{ fontStyle: "italic", color: "#5e6c84" }}>Sin asignar</span>
          </div>
          {miembros.map((m) => (
            <div key={m.id} className="dropdown-item" onClick={() => asignarMiembro(m)}>
              <span className="assignee-avatar" style={{ backgroundColor: obtenerColorAvatar(m.nombre) }}>
                {obtenerInicial(m.nombre)}
              </span>
              <span>{m.nombre}</span>
            </div>
          ))}
        </div>,
        document.body
      )
    : null;

  return (
    <div className="task-card">
      <div className="task-title">{tarea.nombre}</div>
      {tarea.descripcion && <div className="task-description">{tarea.descripcion}</div>}

      <div className="task-card-footer">
        <div className="task-actions">
          {onMoveLeft && (
            <button className="task-btn" onClick={() => onMoveLeft(tarea.id)}>←</button>
          )}
          {onMoveRight && (
            <button className="task-btn" onClick={() => onMoveRight(tarea.id)}>→</button>
          )}
        </div>

        {/* asignación de miembro*/}
        <div className="task-assignment-container" onClick={(e) => e.stopPropagation()}>
          <button
            ref={buttonRef}
            className="assignee-btn"
            title={miembroAsignado ? `Asignado a: ${miembroAsignado.nombre}` : "Asignar miembro"}
            onClick={toggleDropdown}
          >
            {miembroAsignado ? (
              <span className="assignee-avatar" style={{ backgroundColor: obtenerColorAvatar(miembroAsignado.nombre) }}>
                {obtenerInicial(miembroAsignado.nombre)}
              </span>
            ) : (
              <span className="assignee-avatar unassigned">{iconoSinAsignar}</span>
            )}
          </button>
        </div>
      </div>

      {dropdown}
    </div>
  );
}