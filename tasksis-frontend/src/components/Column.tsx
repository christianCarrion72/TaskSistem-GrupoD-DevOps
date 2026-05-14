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
  onAddTask: (nombre: string, descripcion: string) => void;
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

  const guardar = () => {
    if (!nombre.trim()) return;

    onAddTask(nombre, descripcion);

    setNombre("");
    setDescripcion("");
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

          <div className="task-form-actions">
            <button className="task-save-btn" onClick={guardar}>
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
        <button className="add-card-btn" onClick={() => setMostrarFormulario(true)}>
          + Añada una tarjeta
        </button>
      )}
    </div>
  );
}