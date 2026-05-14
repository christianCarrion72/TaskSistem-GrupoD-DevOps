import { useEffect, useState } from "react";
import type { Tarea } from "./types/Tarea";
import "./styles/App.css";
import "./styles/Layout.css";
import Header from "./components/Header";
import Sidebar from "./components/Sidebar";
import Column from "./components/Column";

const API_URL = "http://localhost:5255/api/tareas";

type EstadoFront = "pendiente" | "proceso" | "hecho";
type EstadoApi = "Pendiente" | "Proceso" | "Hecho";

export default function App() {
  const [tareas, setTareas] = useState<Tarea[]>([]);

  useEffect(() => {
    cargarTareas();
  }, []);

  const cargarTareas = async () => {
    try {
      const res = await fetch(API_URL);
      if (!res.ok) throw new Error("Error cargando tareas");

      const data = await res.json();
      setTareas(data);
    } catch (error) {
      console.error(error);
    }
  };

  const pendientes = tareas.filter((t) => t.estado.toLowerCase() === "pendiente");
  const proceso = tareas.filter((t) => t.estado.toLowerCase() === "proceso");
  const hecho = tareas.filter((t) => t.estado.toLowerCase() === "hecho");

  const convertirEstadoApi = (estado: EstadoFront): EstadoApi => {
    if (estado === "pendiente") return "Pendiente";
    if (estado === "proceso") return "Proceso";
    return "Hecho";
  };

  const cambiarEstado = async (id: number, nuevoEstado: EstadoFront) => {
    const tarea = tareas.find((t) => t.id === id);
    if (!tarea) return;

    try {
      const res = await fetch(`${API_URL}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          Nombre: tarea.nombre,
          Descripcion: tarea.descripcion,
          Estado: convertirEstadoApi(nuevoEstado),
        }),
      });

      if (!res.ok) throw new Error("Error actualizando estado");

      setTareas((prev) =>
        prev.map((t) => (t.id === id ? { ...t, estado: nuevoEstado } : t))
      );
    } catch (error) {
      console.error(error);
    }
  };

  const agregarTarea = async (
    estado: EstadoFront,
    nombre: string,
    descripcion: string
  ) => {
    try {
      const res = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          nombre,
          descripcion,
          estado: convertirEstadoApi(estado),
        }),
      });

      if (!res.ok) throw new Error("Error creando tarea");

      const nueva = await res.json();
      setTareas((prev) => [...prev, nueva]);
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <>
      <Header />

      <div className="layout">
        <Sidebar />

        <div className="app-container">
          <div className="board-header">
            <div className="board-title">Realizar las tareas</div>
          </div>

          <div className="board-columns">
            <Column
              title="Lista de tareas"
              color="blue"
              tareas={pendientes}
              onMoveRight={(id) => cambiarEstado(id, "proceso")}
              onAddTask={(nombre, descripcion) =>
                agregarTarea("pendiente", nombre, descripcion)
              }
            />

            <Column
              title="En proceso"
              color="yellow"
              tareas={proceso}
              onMoveLeft={(id) => cambiarEstado(id, "pendiente")}
              onMoveRight={(id) => cambiarEstado(id, "hecho")}
              onAddTask={(nombre, descripcion) =>
                agregarTarea("proceso", nombre, descripcion)
              }
            />

            <Column
              title="Hecho"
              color="green"
              tareas={hecho}
              onMoveLeft={(id) => cambiarEstado(id, "proceso")}
              onAddTask={(nombre, descripcion) =>
                agregarTarea("hecho", nombre, descripcion)
              }
            />
          </div>
        </div>
      </div>
    </>
  );
}