import "../styles/TaskCard.css";
import type { Tarea } from "../types/Tarea";

interface Props {
  tarea: Tarea;
  onMoveLeft?: (id: number) => void;
  onMoveRight?: (id: number) => void;
}

export default function TaskCard({ tarea, onMoveLeft, onMoveRight }: Props) {
  return (
    <div className="task-card">
      <div className="task-title">{tarea.nombre}</div>

      {tarea.descripcion && (
        <div className="task-description">{tarea.descripcion}</div>
      )}

      <div className="task-actions">
        {onMoveLeft && (
          <button className="task-btn" onClick={() => onMoveLeft(tarea.id)}>
            ←
          </button>
        )}

        {onMoveRight && (
          <button className="task-btn" onClick={() => onMoveRight(tarea.id)}>
            →
          </button>
        )}
      </div>
    </div>
  );
}