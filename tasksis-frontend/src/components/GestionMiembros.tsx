import { useState, useEffect } from "react";
import type { Miembro } from "../types/Miembro";
import "../styles/GestionMiembros.css";
import { toast } from "react-toastify";
import { getUsuarios, crearUsuario, actualizarUsuario, eliminarUsuario } from "../services/UsuariosApi";

type TipoModal = "ninguno" | "crear" | "editar" | "ver";

interface EstadoModal {
  tipo: TipoModal;
  miembro?: Miembro;
}

interface ConfirmacionEliminar {
  visible: boolean;
  miembro?: Miembro;
}

export default function GestionMiembros() {
  const [miembros, setMiembros] = useState<Miembro[]>([]);
  const [cargando, setCargando] = useState(true);
  const [buscar, setBuscar] = useState("");
  const [modalState, setModalState] = useState<EstadoModal>({ tipo: "ninguno" });
  const [confirmacion, setConfirmacion] = useState<ConfirmacionEliminar>({ visible: false });

  const [nombre, setNombre] = useState("");
  const [correo, setCorreo] = useState("");

  useEffect(() => {
    cargarMiembros();
  }, []);

  const cargarMiembros = async () => {
    setCargando(true);
    try {
      const datos = await getUsuarios();
      setMiembros(datos);
    } catch (error) {
      console.error(error);
      toast.error("Error al cargar los miembros");
    } finally {
      setCargando(false);
    }
  };

  const abrirModal = (tipo: TipoModal, miembro?: Miembro) => {
    setModalState({ tipo, miembro });
    if (tipo === "editar" && miembro) {
      setNombre(miembro.nombre);
      setCorreo(miembro.correo);
    } else if (tipo === "crear") {
      setNombre("");
      setCorreo("");
    }
  };

  const cerrarModal = () => {
    setModalState({ tipo: "ninguno" });
    setNombre("");
    setCorreo("");
  };

  const guardar = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!nombre.trim() || !correo.trim()) {
      toast.warn("⚠️ Por favor rellene todos los campos requeridos");
      return;
    }

    try {
      if (modalState.tipo === "crear") {
        await crearUsuario(nombre.trim(), correo.trim());
        toast.success("Miembro creado con éxito");
      } else if (modalState.tipo === "editar" && modalState.miembro) {
        await actualizarUsuario(modalState.miembro.id, nombre.trim(), correo.trim());
        toast.success("Miembro actualizado con éxito");
      }
      cerrarModal();
      cargarMiembros();
    } catch (error) {
      console.error(error);
      toast.error("Error al guardar los cambios");
    }
  };

  const pedirConfirmacionEliminar = (miembro: Miembro) => {
    setConfirmacion({ visible: true, miembro });
  };

  const cancelarEliminar = () => {
    setConfirmacion({ visible: false });
  };

  const confirmarEliminar = async () => {
    if (!confirmacion.miembro) return;
    const { id, nombre: nombreMiembro } = confirmacion.miembro;
    setConfirmacion({ visible: false });
    try {
      await eliminarUsuario(id);
      toast.success(`Miembro "${nombreMiembro}" eliminado con éxito`);
      cargarMiembros();
    } catch (error) {
      console.error(error);
      toast.error("Error al eliminar el miembro");
    }
  };

  const miembrosFiltrados = miembros.filter((m) =>
    m.nombre.toLowerCase().includes(buscar.toLowerCase()) ||
    m.correo.toLowerCase().includes(buscar.toLowerCase())
  );

  const tituloPorTipo: Record<TipoModal, string> = {
    ninguno: "",
    crear: "Nuevo Miembro",
    editar: "Editar Miembro",
    ver: "Detalles de Miembro",
  };

  return (
    <div className="gestion-miembros">
      <div className="gestion-miembros-header">
        <h2>Gestión de Miembros</h2>
        <button className="nuevo-btn" onClick={() => abrirModal("crear")}>
          <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
            <line x1="12" y1="5" x2="12" y2="19"></line>
            <line x1="5" y1="12" x2="19" y2="12"></line>
          </svg>
          Nuevo
        </button>
      </div>

      <div className="search-container">
        <input
          type="text"
          className="search-input"
          placeholder="Buscar..."
          value={buscar}
          onChange={(e) => setBuscar(e.target.value)}
        />
      </div>

      <div className="table-responsive">
        {cargando ? (
          <div className="no-members">Cargando miembros...</div>
        ) : miembrosFiltrados.length > 0 ? (
          <table className="members-table">
            <thead>
              <tr>
                <th>Nro</th>
                <th>Nombre</th>
                <th>Correo</th>
                <th>Fecha de Creación</th>
                <th style={{ width: "120px" }}>Acción</th>
              </tr>
            </thead>
            <tbody>
              {miembrosFiltrados.map((m, index) => (
                <tr key={m.id}>
                  <td>{index + 1}</td>
                  <td style={{ fontWeight: "600", color: "white" }}>{m.nombre}</td>
                  <td>{m.correo}</td>
                  <td>{new Date(m.fechaCreacion).toLocaleDateString("es-ES")}</td>
                  <td className="actions-cell">
                    <button className="action-btn view" title="Ver Detalles" onClick={() => abrirModal("ver", m)}>
                      <svg viewBox="0 0 24 24">
                        <path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"/>
                      </svg>
                    </button>
                    <button className="action-btn edit" title="Editar" onClick={() => abrirModal("editar", m)}>
                      <svg viewBox="0 0 24 24">
                        <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/>
                      </svg>
                    </button>
                    <button className="action-btn delete" title="Eliminar" onClick={() => pedirConfirmacionEliminar(m)}>
                      <svg viewBox="0 0 24 24">
                        <path d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"/>
                      </svg>
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        ) : (
          <div className="no-members">No se encontraron miembros registrados.</div>
        )}
      </div>

      {/* */}
      {confirmacion.visible && confirmacion.miembro && (
        <div className="modal-overlay" onClick={cancelarEliminar}>
          <div className="modal-container modal-confirmacion" onClick={(e) => e.stopPropagation()}>
            <div className="confirmacion-icono">
              <svg viewBox="0 0 24 24" width="40" height="40">
                <path fill="#ff6b6b" d="M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 4h-3.5l-1-1h-5l-1 1H5v2h14V4z"/>
              </svg>
            </div>
            <h3 className="confirmacion-titulo">Eliminar Miembro</h3>
            <p className="confirmacion-mensaje">
              ¿Está seguro que desea eliminar a{" "}
              <strong>"{confirmacion.miembro.nombre}"</strong>?
              <br />
              <span className="confirmacion-advertencia">Esta acción no se puede deshacer.</span>
            </p>
            <div className="confirmacion-acciones">
              <button className="btn-cancelar" onClick={cancelarEliminar}>
                Cancelar
              </button>
              <button className="btn-eliminar-confirm" onClick={confirmarEliminar}>
                Eliminar
              </button>
            </div>
          </div>
        </div>
      )}

      {/*  */}
      {modalState.tipo !== "ninguno" && (
        <div className="modal-overlay" onClick={cerrarModal}>
          <div className="modal-container" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{tituloPorTipo[modalState.tipo]}</h3>
              <button className="modal-close-btn" onClick={cerrarModal}>&times;</button>
            </div>

            <form onSubmit={guardar}>
              <div className="modal-body">
                <div className="form-group">
                  <label htmlFor="nombre">Nombre *</label>
                  <input
                    type="text"
                    id="nombre"
                    className="form-control"
                    placeholder="Ingrese el nombre completo"
                    value={modalState.tipo === "ver" ? modalState.miembro?.nombre ?? "" : nombre}
                    onChange={(e) => setNombre(e.target.value)}
                    disabled={modalState.tipo === "ver"}
                    required
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="correo">Correo *</label>
                  <input
                    type="email"
                    id="correo"
                    className="form-control"
                    placeholder="ejemplo@correo.com"
                    value={modalState.tipo === "ver" ? modalState.miembro?.correo ?? "" : correo}
                    onChange={(e) => setCorreo(e.target.value)}
                    disabled={modalState.tipo === "ver"}
                    required
                  />
                </div>

                {/*  */}
                {modalState.tipo === "ver" && modalState.miembro && (
                  <div className="form-group">
                    <label>Fecha de Creación</label>
                    <input
                      type="text"
                      className="form-control"
                      value={new Date(modalState.miembro.fechaCreacion).toLocaleString("es-ES")}
                      disabled
                    />
                  </div>
                )}
              </div>

              <div className="modal-footer">
                <button type="button" className="btn-cancelar" onClick={cerrarModal}>
                  {modalState.tipo === "ver" ? "Cerrar" : "Cancelar"}
                </button>
                {modalState.tipo !== "ver" && (
                  <button
                    type="submit"
                    className="btn-guardar"
                    disabled={!nombre.trim() || !correo.trim()}
                  >
                    {modalState.tipo === "editar" ? "Actualizar" : "Guardar"}
                  </button>
                )}
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
