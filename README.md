# NEXO Presupuesto

Sistema de Control de Presupuesto para el curso **Base de Datos I**
(Universidad Politécnica Internacional).

- **Integrantes:** Tayron Salas Núñez y Erick Delgado Araya
- **Docente:** Julio Sánchez Lobo
- **Motor de base de datos:** Microsoft SQL Server 2019 o superior
- **Herramienta:** SQL Server Management Studio (SSMS)

## Estructura del repositorio

| Carpeta | Contenido |
|---|---|
| `base-de-datos/` | Scripts SQL numerados, se ejecutan en orden |
| `documentacion/` | Trabajo escrito, presentación y diccionario de datos |
| `aplicacion/` | Código de la aplicación (se agrega más adelante) |

## Cómo crear la base de datos

1. Abrir SSMS y conectarse a la instancia de SQL Server.
2. Abrir y ejecutar los scripts de `base-de-datos/` en orden numérico
   (`01_...`, `02_...`, etc.).
3. Revisar que el script `01` termine con 6 tablas y 6 llaves foráneas.

> Ojo: el script `01` borra y vuelve a crear las tablas, así que
> no lo ejecuten sobre datos que quieran conservar.

## Cómo trabajar en equipo

- Antes de empezar: `git pull`
- Al terminar un cambio: `git add .`, `git commit -m "mensaje"` y `git push`
- Los archivos Word y PowerPoint no se pueden combinar si los editan
  los dos a la vez: avísense quién los está editando.
