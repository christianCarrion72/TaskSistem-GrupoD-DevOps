import { useEffect, useState } from "react";
import type { Tarea } from "./types/Tarea";
import "./styles/App.css";
import "./styles/Layout.css";
import Header from "./components/Header";
import Sidebar from "./components/Sidebar";
import Column from "./components/Column";
import GestionMiembros from "./components/GestionMiembros";

import { ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const API_URL = `${import.meta.env.VITE_API_URL || "http://localhost:5255"}/api/tareas`;
type EstadoFront = "pendiente" | "proceso" | "hecho";
type EstadoApi = "Pendiente" | "Proceso" | "Hecho";

type ApiResponse = {
  exito: boolean;
  mensaje: string;
  datos: Tarea[];
  errores: string[] | null;
};

export default function App() {
  const [respuesta, setRespuesta] = useState<ApiResponse | null>(null);
  const [seccionActiva, setSeccionActiva] = useState<"tablero" | "miembros" | "configuracion">("tablero");

  useEffect(() => {
    cargarTareas();
  }, []);

  const cargarTareas = async () => {
    try {
      const res = await fetch(API_URL);
      if (!res.ok) throw new Error("Error cargando tareas");

      const data: ApiResponse = await res.json();
      setRespuesta(data);
      // ❌ quitamos toast.info aquí para evitar duplicados
    } catch (error) {
      console.error(error);
      toast.error("❌ Error al cargar las tareas");
    }
  };

  const tareas = respuesta?.datos || [];

  const pendientes = tareas.filter((t) => t.estado && t.estado.toLowerCase() === "pendiente");
  const proceso = tareas.filter((t) => t.estado && t.estado.toLowerCase() === "proceso");
  const hecho = tareas.filter((t) => t.estado && t.estado.toLowerCase() === "hecho");

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
          UsuarioAsignadoId: tarea.usuarioAsignadoId ?? null,
        }),
      });

      if (!res.ok) throw new Error("Error actualizando estado");

      setRespuesta((prev) =>
        prev
          ? {
              ...prev,
              datos: prev.datos.map((t) =>
                t.id === id ? { ...t, estado: nuevoEstado } : t
              ),
            }
          : prev
      );

      toast.success("Estado actualizado con éxito");
    } catch (error) {
      console.error(error);
      toast.error("❌ Error al actualizar la tarea");
    }
  };

  const agregarTarea = async (estado: EstadoFront, nombre: string, descripcion: string) => {
    if (!nombre.trim() || !descripcion.trim()) {
      toast.warn("⚠️ Rellene los campos requeridos: título y descripción");
      return;
    }

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
      setRespuesta((prev) =>
        prev
          ? { ...prev, datos: [...prev.datos, nueva] }
          : { exito: true, mensaje: "Tarea creada", datos: [nueva], errores: null }
      );

      toast.success(nueva.mensaje || "Tarea creada con éxito");
      cargarTareas();
    } catch (error) {
      console.error(error);
      toast.error("❌ Error al registrar la tarea");
    }
  };

  return (
    <>
      <Header />
      <div className="layout">
        <Sidebar seccionActiva={seccionActiva} onSeccionChange={setSeccionActiva} />
        
        {seccionActiva === "tablero" && (
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
                onAddTask={(nombre, descripcion, estado) =>
                  agregarTarea(estado, nombre, descripcion)
                }
              />
              <Column
                title="En proceso"
                color="yellow"
                tareas={proceso}
                onMoveLeft={(id) => cambiarEstado(id, "pendiente")}
                onMoveRight={(id) => cambiarEstado(id, "hecho")}
                onAddTask={(nombre, descripcion, estado) =>
                  agregarTarea(estado, nombre, descripcion)
                }
              />
              <Column
                title="Hecho"
                color="green"
                tareas={hecho}
                onMoveLeft={(id) => cambiarEstado(id, "proceso")}
                onAddTask={(nombre, descripcion, estado) =>
                  agregarTarea(estado, nombre, descripcion)
                }
              />
            </div>
          </div>
        )}

        {seccionActiva === "miembros" && (
          <div className="app-container">
            <GestionMiembros />
          </div>
        )}

        {seccionActiva === "configuracion" && (
          <div className="app-container">
            <div style={{ color: "white", padding: "20px", background: "rgba(0, 0, 0, 0.15)", borderRadius: "8px" }}>
              <h2 style={{ margin: "0 0 10px 0" }}>Configuración</h2>
              <p>Opciones de configuración del sistema.</p>
            </div>
          </div>
        )}
      </div>

      <ToastContainer
        position="top-right"
        autoClose={3000}
        hideProgressBar={false}
        closeOnClick
        pauseOnHover
        draggable
        theme="colored"
      />
    </>
  );
}
