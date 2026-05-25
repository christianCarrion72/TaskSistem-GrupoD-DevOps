/**
 *  PRUEBAS DEL FRONTEND - TaskSis
 *
 * Tests:
 *  1. Header renderiza el título de la aplicación ("Tareas") y el menú de navegación
 *  2. TaskCard muestra nombre y descripción de la tarea
 *  3. Column renderiza sus tareas y el botón "Añada una tarea"
 *  4. Column - guardar tarea vacía no llama onAddTask (botón deshabilitado)
 */

import { render, screen, fireEvent, waitFor, act } from '@testing-library/react';
import { describe, it, expect, vi } from 'vitest';
import Header from '../components/Header';
import TaskCard from '../components/TaskCard';
import Column from '../components/Column';
import type { Tarea } from '../types/Tarea';

// ---------------------------------------------------------------------------
// Mock global de fetch para evitar llamadas reales a la API durante los tests
// ---------------------------------------------------------------------------
globalThis.fetch = vi.fn(() =>
  Promise.resolve({
    ok: true,
    json: () => Promise.resolve({ exito: true, datos: [], mensaje: '' }),
  } as Response)
);

// ---------------------------------------------------------------------------
// TEST 1: El Header muestra el título de la aplicación ("Tareas") y navegación
// Nota: No existe un logo de imagen. El nombre "Tareas" es el título de la app
// que aparece como texto en la esquina superior izquierda del header.
// ---------------------------------------------------------------------------
describe('Componente Header', () => {
  it('debe renderizar el título de la aplicación "Tareas" en la cabecera', () => {
    render(<Header />);

    // El Header muestra "Tareas" como nombre/título de la aplicación
    const titulo = screen.getByText('Tareas');
    expect(titulo).toBeInTheDocument();
  });

  it('debe mostrar los elementos del menú de navegación', () => {
    render(<Header />);

    // Verificar que los ítems del menú existen
    expect(screen.getByText('Espacios de trabajo')).toBeInTheDocument();
    expect(screen.getByText('Reciente')).toBeInTheDocument();
    expect(screen.getByText('Crear')).toBeInTheDocument();
  });
});

// ---------------------------------------------------------------------------
// TEST 2: TaskCard muestra los datos de la tarea correctamente
// ---------------------------------------------------------------------------
describe('Componente TaskCard', () => {
  const tareaEjemplo: Tarea = {
    id: 1,
    nombre: 'Implementar login',
    descripcion: 'Crear el formulario de autenticación con JWT',
    estado: 'pendiente',
    usuarioAsignadoId: null,
  };

  it('debe mostrar el nombre y la descripción de la tarea', async () => {
    await act(async () => {
      render(
        <TaskCard
          tarea={tareaEjemplo}
          onMoveLeft={undefined}
          onMoveRight={undefined}
        />
      );
    });

    // Verificar que el nombre aparece en la tarjeta
    expect(screen.getByText('Implementar login')).toBeInTheDocument();

    // Verificar que la descripción aparece en la tarjeta
    expect(
      screen.getByText('Crear el formulario de autenticación con JWT')
    ).toBeInTheDocument();
  });
});

// ---------------------------------------------------------------------------
// TEST 3: Column muestra la lista de tareas pasadas como prop
// ---------------------------------------------------------------------------
describe('Componente Column - renderizado de tareas', () => {
  const tareas: Tarea[] = [
    {
      id: 1,
      nombre: 'Diseñar base de datos',
      descripcion: 'Modelo ER del sistema',
      estado: 'pendiente',
    },
    {
      id: 2,
      nombre: 'Crear endpoints REST',
      descripcion: 'CRUD de tareas y usuarios',
      estado: 'pendiente',
    },
  ];

  it('debe renderizar todas las tareas de la columna y el botón de agregar', async () => {
    const mockAddTask = vi.fn();

    await act(async () => {
      render(
        <Column
          title="Lista de tareas"
          color="blue"
          tareas={tareas}
          onAddTask={mockAddTask}
        />
      );
    });

    // Esperar a que las tareas se rendericen
    await waitFor(() => {
      expect(screen.getByText('Diseñar base de datos')).toBeInTheDocument();
      expect(screen.getByText('Crear endpoints REST')).toBeInTheDocument();
    });

    // El botón para agregar debe estar presente
    expect(screen.getByText(/añada una tarea/i)).toBeInTheDocument();
  });
});

// ---------------------------------------------------------------------------
// TEST 4: Column - no se puede guardar una tarea con campos vacíos
// ---------------------------------------------------------------------------
describe('Componente Column - validación de formulario', () => {
  it('no debe llamar a onAddTask si el título o descripción están vacíos', async () => {
    const mockAddTask = vi.fn();

    render(
      <Column
        title="En proceso"
        color="yellow"
        tareas={[]}
        onAddTask={mockAddTask}
      />
    );

    // Abrir el formulario haciendo clic en el botón
    fireEvent.click(screen.getByText(/añada una tarea/i));

    // El botón Guardar debe aparecer deshabilitado con campos vacíos
    const botonGuardar = screen.getByText('Guardar');
    expect(botonGuardar).toBeDisabled();

    // Confirmar que NO se llamó onAddTask
    expect(mockAddTask).not.toHaveBeenCalled();
  });
});

