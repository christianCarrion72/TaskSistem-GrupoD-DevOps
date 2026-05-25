import type { Miembro } from "../types/Miembro";

const API_URL = `${import.meta.env.VITE_API_URL || "http://localhost:5255"}/api/usuarios`;
interface ApiRespuesta<T> {
  exito: boolean;
  mensaje: string;
  datos: T;
  errores: string[] | null;
}

// Obtiene todos los usuarios registrados en el backend
export async function getUsuarios(): Promise<Miembro[]> {
  const res = await fetch(API_URL);
  if (!res.ok) throw new Error("Error obteniendo usuarios");
  const json: ApiRespuesta<Miembro[]> = await res.json();
  return json.datos ?? [];
}

export async function getUsuarioPorId(id: number): Promise<Miembro> {
  const res = await fetch(`${API_URL}/${id}`);
  if (!res.ok) throw new Error(`Error obteniendo usuario con id: ${id}`);
  const json: ApiRespuesta<Miembro> = await res.json();
  return json.datos;
}

export async function crearUsuario(nombre: string, correo: string): Promise<Miembro> {
  const res = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ nombre, correo }),
  });
  if (!res.ok) throw new Error("Error creando usuario");
  const json: ApiRespuesta<Miembro> = await res.json();
  return json.datos;
}

export async function actualizarUsuario(id: number, nombre: string, correo: string): Promise<Miembro> {
  const res = await fetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ nombre, correo }),
  });
  if (!res.ok) throw new Error("Error actualizando usuario");
  const json: ApiRespuesta<Miembro> = await res.json();
  return json.datos;
}

export async function eliminarUsuario(id: number): Promise<void> {
  const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
  if (!res.ok) throw new Error("Error eliminando usuario");
}
