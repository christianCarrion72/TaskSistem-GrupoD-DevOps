import type { Tarea } from "../types/Tarea";

const API_URL = `${import.meta.env.VITE_API_URL || "http://localhost:5255"}/api/tareas`;
export async function getTareas(): Promise<Tarea[]> {
  const res = await fetch(API_URL);
  if (!res.ok) throw new Error("Error obteniendo tareas");
  return res.json();
}
export async function crearTarea(data: Omit<Tarea, "id">): Promise<Tarea> {
  const res = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!res.ok) throw new Error("Error creando tarea");
  return res.json();
}

export async function actualizarTarea(id: number, data: Omit<Tarea, "id">): Promise<void> {
  const res = await fetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });

  if (!res.ok) throw new Error("Error actualizando tarea");
}

export async function eliminarTarea(id: number): Promise<void> {
  const res = await fetch(`${API_URL}/${id}`, {
    method: "DELETE",
  });

  if (!res.ok) throw new Error("Error eliminando tarea");
}