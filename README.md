# Sistema de Cafetería - Universidad Nacional de Colombia

Este es un software de escritorio hecho en C# (.NET 8, WinForms) para la gestión integral de la cafetería de la Universidad Nacional de Colombia, Sede de La Paz. El sistema nos permitirá solucionar el problema actual de la gestión de almuerzos mediante las funciones de administrar usuarios, productos, ventas y reportes, facilitando la operación y la toma de decisiones diariamente.

## Características principales

- **Gestión de Usuarios**
  - Alta, edición y baja de usuarios del sistema.
- **Gestión de Productos**
  - Administración del inventario de productos: agregar, editar y eliminar productos.
- **Ventas**
  - Registro de nuevas ventas.
  - Visualización del historial de ventas (en desarrollo).
- **Reportes**
  - Reporte de ventas del día (en desarrollo).
  - Consulta de subsidios y productos más vendidos (en desarrollo).
- **Interfaz amigable**
  - Menú principal con acceso rápido a todas las funciones.
  - Barra de estado con usuario, fecha y hora actualizada en tiempo real.
  - Soporte para ventanas MDI (interfaz de documentos múltiples).
- **Arquitectura moderna**
  - Inyección de dependencias con `Microsoft.Extensions.DependencyInjection`.
  - Acceso a datos mediante Entity Framework Core y Sqlite.
  - Configuración centralizada en `appsettings.json`.
  - Separación clara entre lógica de negocio, datos y presentación.

## Menú principal

- **Archivo**
  - Cerrar sesión (camellando)
  - Salir
- **Gestión**
  - Usuarios
  - Productos
  - Fichos del Día (camellando)
- **Ventas**
  - Nueva Venta
  - Historial de Ventas (camellando)
- **Reportes**
  - Ventas del Día (camellando)
  - Subsidios (camellando)
  - Productos más Vendidos (camellando)
- **Ayuda**
  - Acerca de

## Requisitos

- .NET 8 SDK
- Visual Studio 2022 o superior
- Sqlite (la base de datos se crea automáticamente) Se planea la opción de poder agregar una base de datos desde un archivo `bd`

## Instalación y ejecución

1. Clona este repositorio:
   
