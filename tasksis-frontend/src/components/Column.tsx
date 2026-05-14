import { useState } from "react";
import "../styles/Column.css";
import type { Tarea } from "../types/Tarea";
import TaskCard from "./TaskCard.tsx";

interface Props {
  title: string;
  color: "blue" | "yellow" | "green";
  tareas: Tarea[];
  onMoveLeft?: (id: number) => void;
  onMoveRight?: (id: number) => void;
  onAddTask: (nombre: string, descripcion: string, estado: "pendiente" | "proceso" | "hecho") => void;
}

export default function Column({
  title,
  color,
  tareas,
  onMoveLeft,
  onMoveRight,
  onAddTask,
}: Props) {
  const [mostrarFormulario, setMostrarFormulario] = useState(false);
  const [nombre, setNombre] = useState("");
  const [descripcion, setDescripcion] = useState("");
  const [estado, setEstado] = useState<"pendiente" | "proceso" | "hecho">("pendiente");

  const guardar = () => {
    if (!nombre.trim() || !descripcion.trim()) return;

    onAddTask(nombre, descripcion, estado);

    setNombre("");
    setDescripcion("");
    setEstado("pendiente");
    setMostrarFormulario(false);
  };

  return (
    <div className="column">
      <div className={`column-header ${color}`}>{title}</div>

      {tareas.map((tarea) => (
        <TaskCard
          key={tarea.id}
          tarea={tarea}
          onMoveLeft={onMoveLeft}
          onMoveRight={onMoveRight}
        />
      ))}
      {mostrarFormulario ? (
        <div className="task-form">
          <input
            className="task-input"
            placeholder="Título"
            value={nombre}
            onChange={(e) => setNombre(e.target.value)}
          />

          <textarea
            className="task-textarea"
            placeholder="Descripción"
            value={descripcion}
            onChange={(e) => setDescripcion(e.target.value)}
          />

          {/* Selector de estado */}
          <select
            className="task-select"
            value={estado}
            onChange={(e) => setEstado(e.target.value as "pendiente" | "proceso" | "hecho")}
          >
            <option value="pendiente">Pendiente</option>
            <option value="proceso">En proceso</option>
            <option value="hecho">Hecho</option>
          </select>

          <div className="task-form-actions">
            <button
              className="task-save-btn"
              onClick={guardar}
              disabled={!nombre.trim() || !descripcion.trim()}
            >
              Guardar
            </button>

            <button
              className="task-cancel-btn"
              onClick={() => setMostrarFormulario(false)}
            >
              Cancelar
            </button>
          </div>
        </div>
      ) : (
        <button
          className="add-card-btn"
          onClick={() => setMostrarFormulario(true)}
        >
          + Añada una tarea
        </button>
      )}
    </div>
  );
}
