# Bitácora API

API REST en **C# / ASP.NET Core 8** para administrar los registros de una bitácora. Está organizada por capas según la arquitectura limpia, usa **Entity Framework Core con SQLite**, se documenta con **OpenAPI / Swagger UI** y se prueba con **Postman**.

## Estructura de la solución

```
Bitacora.sln
├── src/
│   ├── Bitacora.Domain           Entidad Registro, reglas de negocio y excepción de dominio (no depende de nadie)
│   ├── Bitacora.Application      DTO, IRegistroRepository, casos de uso (RegistroService), validación
│   ├── Bitacora.Infrastructure   DbContext de EF Core (SQLite), repositorio y migraciones
│   └── Bitacora.Api              Controladores, Program.cs (DI), manejador global de errores, Swagger
└── postman/
    ├── Bitacora.postman_collection.json
    └── Bitacora.postman_environment.json
```

Dependencias (siempre apuntan hacia el dominio; ver también `docs/diagrama_capas.png`):

```
Api ──► Application ──► Domain
 │            ▲
 └──► Infrastructure ──┘
```

## Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0) (`dotnet --version` debe mostrar 8.x)
- Herramienta de EF Core: `dotnet tool install --global dotnet-ef --version 8.0.11`
- Postman (para la colección de pruebas)

## Cómo ejecutar

```bash
# 1. Clonar y entrar en la carpeta
git clone <URL-DEL-REPOSITORIO>
cd BitacoraApi

# 2. Restaurar paquetes y compilar
dotnet restore
dotnet build

# 3. Aplicar las migraciones (crea el archivo bitacora.db)
dotnet ef database update --project src/Bitacora.Infrastructure --startup-project src/Bitacora.Api

# 4. Ejecutar la API
dotnet run --project src/Bitacora.Api
```

La API queda en **http://localhost:5080** y Swagger UI en **http://localhost:5080/swagger**.

> Si más adelante cambian la entidad, se crea una nueva migración con:
> `dotnet ef migrations add NombreDelCambio --project src/Bitacora.Infrastructure --startup-project src/Bitacora.Api --output-dir Persistence/Migrations`

### Alternativa: ejecutar con Docker

No necesita el SDK de .NET instalado. La base de datos se crea sola al arrancar (aplica las migraciones) y se guarda en un volumen.

```bash
docker compose up --build
```

La API queda igualmente en **http://localhost:5080/swagger**. Para detenerla: `Ctrl + C` y luego `docker compose down` (agregar `-v` para borrar también la base de datos).

### Probar desde Android

- **Emulador de Android Studio:** usar `http://10.0.2.2:5080` como URL base (equivale al `localhost` del PC).
- **Teléfono físico en la misma red Wi-Fi:** ejecutar con el perfil que escucha en todas las interfaces y usar la IP del PC (por ejemplo `http://192.168.1.50:5080`):
  ```bash
  dotnet run --project src/Bitacora.Api --launch-profile red-local
  ```
  Puede ser necesario permitir el puerto 5080 en el firewall de Windows.
- Como la API usa HTTP, la app necesita `android:usesCleartextTraffic="true"` en el `AndroidManifest.xml` (o una `network_security_config`).

## Endpoints

| Método | Ruta | Acción | Respuestas |
|---|---|---|---|
| GET | `/api/registros` | Listar registros (sin la foto completa; incluye `tieneFoto`) | 200 |
| GET | `/api/registros/{id}` | Consultar un registro con su foto | 200, 404 |
| POST | `/api/registros` | Crear un registro | 201 + `Location`, 400 |
| PUT | `/api/registros/{id}` | Actualizar un registro | 200, 400, 404 |
| DELETE | `/api/registros/{id}` | Eliminar un registro | 204, 404 |
| GET | `/api/diagnostico/error` | Solo en Development: provoca un error para demostrar la respuesta 500 | 500 |

Ejemplo de cuerpo para POST / PUT:

```json
{
  "titulo": "Inspección del área norte",
  "descripcion": "Se revisaron los extintores y todos están vigentes.",
  "fotoBase64": null
}
```

`fotoBase64` es opcional; si se envía debe ser Base64 puro (sin el prefijo `data:image/...;base64,`) y no superar 2 MB decodificado. `fechaCreacion` la asigna el servidor (en UTC); si el cliente la envía se ignora.

## Validación y errores

| Situación | Código | Cuerpo |
|---|---|---|
| Campos inválidos (Data Annotations en los DTO) | 400 | `ValidationProblemDetails` con `errors` por campo |
| Regla de negocio incumplida (validada también en la entidad) | 400 | `ValidationProblemDetails` con el campo afectado |
| Registro inexistente | 404 | `ProblemDetails` |
| Error no controlado | 500 | `ProblemDetails` genérico con `traceId`; el detalle solo se escribe en el log |

El manejo de errores está centralizado en `src/Bitacora.Api/Middleware/ManejadorGlobalExcepciones.cs` (implementa `IExceptionHandler`).

## Pruebas con Postman

1. En Postman: **Import** → seleccionar los dos archivos de la carpeta `postman/`.
2. Elegir el entorno **Bitacora - Local** (variable `baseUrl` = `http://localhost:5080`).
3. Con la API en ejecución, abrir la colección **Bitácora API** → **Run** y ejecutar todo en orden.

La colección tiene 15 solicitudes (al menos un caso exitoso y uno fallido por endpoint) y 40 aserciones. Desde la terminal también se puede correr con Newman:

```bash
npx newman run postman/Bitacora.postman_collection.json -e postman/Bitacora.postman_environment.json
```

## Integrantes y participación

| Integrante | Rama | Responsabilidades |
|---|---|---|
| Ovidio Isai Castillo | `main` (estructura inicial) | Estructura de la solución, capa de dominio (entidad `Registro` y reglas), capa de aplicación (DTO, `IRegistroRepository`, `RegistroService`), capa de presentación (controlador, `Program.cs`, manejador global de errores, Swagger) e infraestructura con EF Core y SQLite. |
| Oliver Claudio Avila | `feature/pruebas-docs` | Colección y entorno de Postman (`postman/`), pruebas de cada endpoint, diagrama de capas e informe en PDF (`docs/`). |

## Flujo de trabajo en GitHub

- Rama principal `main` protegida; cada integrante trabaja en su rama (`feature/dominio`, `feature/infraestructura`, etc.) y abre un *pull request*.
- Mensajes de commit descriptivos, por ejemplo `feat(domain): agrega entidad Registro y reglas`.
- `bin/`, `obj/` y la base de datos `*.db` están excluidos por `.gitignore`.
