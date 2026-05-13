export interface Tarea {
  id: number;
  nombre: string;
  descripcion: string;
  estado: "pendiente" | "proceso" | "hecho";
}