# ServiceRemote

API REST desarrollada con **ASP.NET Core / .NET** para gestionar usuarios mediante una arquitectura por capas. El proyecto combina **base de datos local, caché, una API REST externa, sincronización automática y un sistema de notificaciones reactivas**.

El objetivo principal es separar las responsabilidades de cada parte de la aplicación para que el código sea más mantenible, testeable y fácil de ampliar.

---

## Índice

- [Descripción](#-descripción)
- [Arquitectura](#-arquitectura)
- [Estructura del proyecto](#-estructura-del-proyecto)
- [Flujo de la aplicación](#-flujo-de-la-aplicación)
- [Capas y directorios](#-capas-y-directorios)
  - [Models](#models)
  - [Entity](#entity)
  - [Dto](#dto)
  - [Repositories](#repositories)
  - [Services](#services)
  - [Controllers](#controllers)
  - [Errors](#errors)
  - [Cache](#cache)
  - [API](#api)
  - [Notifications](#notifications)
  - [Validations](#validations)
  - [Sync](#sync)
  - [Dependency Injection](#dependency-injection)
  - [Test](#test)
- [Caché](#-caché)
- [Persistencia](#-persistencia)
- [Manejo de errores](#-manejo-de-errores)
- [Notificaciones reactivas](#-notificaciones-reactivas)
- [Sincronización automática](#-sincronización-automática)
- [Pruebas](#-pruebas)
- [Tecnologías utilizadas](#-tecnologías-utilizadas)
- [Ventajas de la arquitectura](#-ventajas-de-la-arquitectura)

---

# Descripción

`ServiceRemote` es un servicio web que permite realizar operaciones CRUD sobre usuarios.

La aplicación trabaja con diferentes niveles de almacenamiento:

1. **Caché**
2. **Base de datos local**
3. **API REST externa**

Además, dispone de:

- DTOs para separar los datos de entrada y salida.
- Mappers para transformar objetos entre capas.
- Repositorio genérico para las operaciones CRUD y especifico (para probar si se puede con ambos) .
- Servicio encargado de la lógica de negocio.
- Gestión de errores de dominio mediante `Result<T, TError>`.
- Validación de datos.
- Sistema de notificaciones basado en programación reactiva.
- Sincronización periódica con la API externa.
- Inyección de dependencias.
- Pruebas automatizadas.
- Pruebas de integración del repositorio mediante Testcontainers.

---

# Arquitectura

La aplicación utiliza una arquitectura por capas en la que cada directorio tiene una responsabilidad concreta.

```text
                    ┌──────────────────────┐
                    │       Cliente        │
                    │  Bruno / HTTP Client │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │     Controllers      │
                    │    UserController    │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │       Services       │
                    │      UserService     │
                    └──────┬────────┬──────┘
                           │        │
                ┌──────────┘        └──────────┐
                ▼                              ▼
       ┌────────────────┐             ┌─────────────────┐
       │   Repositories │             │      Cache      │
       │                │             │ Memory / Redis  │
       └───────┬────────┘             └─────────────────┘
               │
               ▼
       ┌────────────────┐
       │   AppDbContext │
       │   EF Core      │
       └───────┬────────┘
               │
               ▼
       ┌────────────────┐
       │  Base de datos │
       └────────────────┘

       ┌──────────────────────────────┐
       │       API externa            │
       │ JSONPlaceholder / Refit      │
       └──────────────▲───────────────┘
                      │
                      │
              ┌───────┴────────┐
              │      API       │
              └────────────────┘

       ┌──────────────────────────────┐
       │       Notifications          │
       │   Subject → Observer         │
       └──────────────────────────────┘

       ┌──────────────────────────────┐
       │          Sync                │
       │    BackgroundService         │
       │       cada 60 segundos       │
       └──────────────────────────────┘
```

La idea fundamental es que **cada capa conoce únicamente las responsabilidades que necesita**.

Por ejemplo:

- El controlador recibe peticiones HTTP.
- El servicio contiene la lógica de negocio.
- El repositorio se ocupa de la persistencia.
- La caché se ocupa de almacenar temporalmente usuarios.
- La API se ocupa de la comunicación externa.
- Los DTOs representan los datos que entran y salen de la API.

---

# Estructura del proyecto

Una estructura aproximada del proyecto es:

```text
ServiceRemote/
│
├── API/
│   └── IJsonPlaceholderApi.cs
│
├── Cache/
|   ├── IDistributedCache.cs
│   ├── IUserCache.cs
│   ├── IUserCacheKeyProvider.cs
│   ├── RedisUserCacheKeyProvider.cs
│   └── UserCacheService.cs
│
├── Config/
|   ├── AppUserConfig.cs
│   └── ConfigRedis.cs
│
├── Controller/
│   └── UserController.cs
│
├── Decorator/
│   └── WebAplicationOptionCacheExtension
│
├── Dto/
│   ├── UserCreateDto.cs
│   ├── UserResponseDto.cs
│   ├── UserResponseExtensionMapper.cs
│   └── UserUpdateDto.cs 
│
├── Entity/
│   ├── AppDbContext.cs
│   └── UserEntity.cs
│
├── Errors/
│   ├── UserDomainError.cs
│   └── DomainErrorExtensions.cs
│
├── Models/
│   └── User.cs
│
├── Notifications/
│   ├── Emiter/
│   │   ├── INotificationService.cs
│   │   └── NotificacionService.cs
│   │
│   └── Observer/
│       ├── INotifyObserverService.cs
│       └── NotifyObserverService.cs
│
├── Repositories/
│   ├── ICrudRepository.cs
│   ├── CrudRepository.cs
│   ├── IUserRepository.cs
│   └── UserRepository.cs
│
├── Services/
│   ├── IService.cs
│   ├── IUserService.cs
│   └── UserService.cs
│
├── Sync/
│   └── UserSyncBackgroundService.cs
│
├── Validations/
│   └── IUserValidator.cs
│
├── Infraestructure/
│   └── DependenciesProvider.cs
│
├── Test/
│   └── Test con Bruno
│
├── Program.cs
├── Docker-compose.yaml
└── appsettings.json

ServiceRemote.Test/
│
├── Mapper
│   └── UserDtoExtensionMapperTests.cs
├── Repository
│   └── CrudRepositoryIntegrationTest.cs
├── Cache
│   └── UserCacheService_Test.cs
├── Errors
│   ├── DomainErrorExtensionsTests.cs
│   ├── DomainErrorsTests.cs
│   └── UserDomainErrorTests.cs
└── Notifications
│   ├── NotificationObserberServiceTest.cs
│   └── NotificationServiceTest.cs
├── Api.Test
│   └── JSonPlaceholderApiTest.cs
├── Services.Test
│   └── UserServiceTet.cs
├── ValidatorsTest.Test
│   └── UserValidatorTest.cs
```

---

# Capas y directorios

## `Models`

Contiene los **modelos de dominio utilizados por la aplicación**.

Actualmente el modelo principal es:

```text
User
```
Representa un usuario de forma independiente de cómo se almacena en la base de datos o de cómo se expone mediante HTTP.

Esto permite evitar que el modelo utilizado internamente tenga que ser exactamente igual que el modelo de persistencia o los DTOs.

---

## `Entity`

Contiene las clases relacionadas con la **persistencia mediante Entity Framework Core**.

```text
Entity/
├── AppDbContext.cs
└── UserEntity.cs
```

### `UserEntity`

Representa cómo se almacena un usuario en la base de datos.

Además de los datos básicos:

- Id
- Name
- UserName
- Email

incluye información de persistencia como:

- `CreatedAt`
- `UpdatedAt`
- `IsDeleted`
- `DeletedAt`

También se utilizan restricciones para:

- campos obligatorios;
- longitud máxima;
- formato del email;
- índices únicos para `UserName` y `Email`.

### `AppDbContext`

Es el punto de entrada de Entity Framework Core a la base de datos.

También contiene reglas globales, como el filtro:

```text
IsDeleted == false
```

De esta manera, los usuarios eliminados lógicamente no aparecen en las consultas normales, aunque esta opción finalmente no ha visto la luz. Se añadirá a futuras actuaizaciones.

---

# DTO

El directorio `Dto` contiene los **Data Transfer Objects**.

```text
Dto/
├── UserCreateDto
├── UserUpdateDto
├── UserResponseDto
└── UserDtoExtensionMapper
```

Se utilizan DTOs diferentes para las distintas operaciones:

### `UserCreateDto`

Representa los datos necesarios para crear un usuario.

No necesita el `Id`, ya que este será generado durante la creación.

### `UserUpdateDto`

Representa los datos utilizados para modificar un usuario.

Incluye el identificador y permite trabajar con propiedades opcionales.

### `UserResponseDto`

Representa la información que la API devuelve al cliente.

Incluye:

- Id
- Name
- UserName
- Email
- CreateAt
- UpdateAt

---

## Mappers

Los mappers permiten transformar objetos entre diferentes representaciones.

Por ejemplo:

```text
User
  ↓
UserResponseDto
```

o:

```text
UserCreateDto
  ↓
User
```

También se dispone de conversiones a JSON.

La utilización de mappers evita realizar manualmente estas conversiones dentro de los controladores o servicios y mantiene separada la transformación de datos de la lógica de negocio.

---

# Repositories

El directorio `Repositories` contiene la lógica de acceso a la base de datos.

```text
Repositories/
├── ICrudRepository.cs
├── CrudRepository.cs
├── IUserRepository.cs
└── UserRepository.cs
```

## Repositorio CRUD genérico

`ICrudRepository<T>` define las operaciones comunes:

```text
GetAll
GetById
Create
Update
Delete
```

`CrudRepository<T>` implementa estas operaciones utilizando Entity Framework Core.

La ventaja de utilizar un repositorio genérico es evitar repetir la misma implementación para cada entidad.

Por ejemplo, las operaciones CRUD básicas son prácticamente iguales para:

```text
User
Product
Order
...
```

Por tanto, pueden reutilizarse mediante:

```text
CrudRepository<T>
```

## Repositorio específico

También existe actualmente:

```text
IUserRepository
UserRepository
```

para las operaciones específicas de usuarios.

Esto permite disponer de un punto específico para el usuario si posteriormente aparecen operaciones que no sean CRUD genérico.

---

# Services

El directorio `Services` contiene la **lógica de negocio**.

```text
Services/
├── IService.cs
├── IUserService.cs
└── UserService.cs
```

Esta capa es una de las partes más importantes de la arquitectura.

El controlador no debería encargarse directamente de:

- acceder a Entity Framework;
- gestionar la caché;
- comunicarse con la API externa;
- transformar todos los modelos;
- gestionar las notificaciones;
- decidir cómo tratar los errores.

Estas responsabilidades se coordinan desde `UserService`.

---

## Servicio genérico

`IService` define una estructura común para operaciones CRUD:

```text
GetAll
GetById
Create
Update
Delete
```

Además, utiliza:

```text
Result<T, TDomainError>
```

para representar tanto operaciones correctas como errores de dominio.

---

## `UserService`

Es la implementación concreta de la lógica de usuarios.

Coordina diferentes componentes:

```text
UserService
    │
    ├── Cache
    ├── Repository
    ├── Notifications
    ├── Mappers
    └── Error handling
```

Por tanto, el servicio funciona como el punto donde se decide **qué debe ocurrir cuando se realiza una operación sobre un usuario**.

---

# Controllers

El controlador expone las operaciones mediante HTTP.

Actualmente se plantea:

```text
UserController
```

con las operaciones:

| HTTP   | Ruta             | Operación      |
| ------ | ---------------- | -------------- |
| GET    | `/api/User`      | Obtener todos  |
| GET    | `/api/User/{id}` | Obtener por ID |
| POST   | `/api/User`      | Crear          |
| PUT    | `/api/User/{id}` | Actualizar     |
| DELETE | `/api/User/{id}` | Eliminar       |

El controlador tiene una responsabilidad limitada:

```text
HTTP Request
     ↓
Controller
     ↓
Service
     ↓
HTTP Response
```

No debería contener la lógica de negocio.

Por ejemplo, al recibir un `GET`, el controlador llama al servicio y transforma el resultado de dominio en una respuesta HTTP.

---

# Errors

El directorio `Errors` centraliza los errores relacionados con el dominio.

Se utiliza `CSharpFunctionalExtensions` para trabajar con:

```text
Result<T, UserDomainError>
```

en lugar de depender exclusivamente de excepciones para controlar errores esperables.

Entre los errores contemplados se encuentran:

- `NotFound`
- `Validation`
- `Storage`
- otros errores internos

La extensión:

```text
ToHttpResult()
```

transforma un error de dominio en una respuesta HTTP apropiada.

Por ejemplo:

```text
NotFound
    ↓
HTTP 404

Validation
    ↓
HTTP 400

Storage
    ↓
HTTP 500
```

Esto permite mantener separadas las dos responsabilidades:

```text
Service
    ↓
Error de dominio

Controller
    ↓
Error de dominio → HTTP
```

---

# Cache

El directorio `Cache` contiene la lógica de caché.

```text
Cache/
├── IUserCache.cs
├── IUserCacheKeyProvider.cs
├── UserCacheService.cs
└── RedisUserCacheKeyProvider.cs
```

Se utiliza la abstracción:

```text
IDistributedCache
```

y se contempla la posibilidad de utilizar diferentes implementaciones de caché.

La configuración permite seleccionar entre:

```text
Memory
```

o:

```text
Distributed / Redis
```

En el caso de Redis se utiliza:

```text
StackExchange.Redis
```

Los usuarios se almacenan utilizando claves con el formato:

```text
user:{id}
```

Por ejemplo:

```text
user:1
user:2
user:25
```

La caché permite:

- obtener un usuario;
- guardar un usuario;
- eliminar un usuario;
- eliminar todos los usuarios almacenados.

---

# API

El directorio `API` contiene la comunicación con la API REST externa.

El proyecto está preparado para comunicarse con:

```text
JSONPlaceholder
```

mediante una interfaz como:

```text
IJsonPlaceholderApi
```

La finalidad de separar esta comunicación en un directorio propio es evitar que el servicio tenga que conocer directamente los detalles de HTTP.

La arquitectura queda:

```text
UserService
      ↓
IJsonPlaceholderApi
      ↓
API REST externa
```

De esta manera, la implementación externa puede cambiar sin tener que modificar toda la lógica de negocio.

---

# Notifications

El sistema de notificaciones utiliza **System.Reactive** y el patrón:

```text
Subject → Observer
```

La estructura es:

```text
Notifications/
│
├── Emiter/
│   ├── INotificationService
│   └── NotificacionService
│
└── Observer/
    ├── INotifyObserverService
    └── NotifyObserverService
```

## Emitter

`NotificacionService` actúa como emisor.

Cuando ocurre una acción:

```text
Usuario creado
Usuario actualizado
Usuario eliminado
Error
Fin de comunicación
```

se publica un mensaje mediante un `Subject<string>`.

Por ejemplo:

```text
Usuario creado: {...}
```

## Observer

`NotifyObserverService` se suscribe al `Subject`.

Cuando recibe un mensaje, lo muestra por consola.

El flujo es:

```text
UserService
     │
     ▼
NotificationService
     │
     ▼
   Subject
     │
     ▼
 Observer
     │
     ▼
 Console
```

Esto permite desacoplar la generación de eventos de la forma en la que estos eventos son consumidos.

---

# Validations

El directorio `Validations` contiene las reglas relacionadas con la validación de los datos recibidos.

La interfaz:

```text
IUserValidator
```

centraliza las comprobaciones necesarias antes de realizar determinadas operaciones.

Separar la validación de los controladores y servicios evita que estas comprobaciones se repitan en diferentes lugares.

---

# Sync

El directorio `Sync` contiene el servicio encargado de la **sincronización periódica con la API externa**.

Se utiliza un `BackgroundService` que se ejecuta automáticamente cada **60 segundos**.

El proceso consiste en:

```text
Cada 60 segundos
       │
       ▼
Vaciar caché
       │
       ▼
Vaciar BD local
       │
       ▼
Consultar API REST
       │
       ▼
Obtener usuarios
       │
       ▼
Guardar usuarios en BD local
```

El objetivo es mantener la información almacenada localmente sincronizada con la fuente remota.

Este proceso se ejecuta en segundo plano y no bloquea las peticiones HTTP normales de la aplicación.

---

# Dependency Injection

El proyecto utiliza **inyección de dependencias** para conectar las diferentes capas.

En lugar de que una clase cree directamente sus dependencias:

```text
new Repository(...)
new Cache(...)
new NotificationService(...)
```

estas son proporcionadas por el contenedor de dependencias de ASP.NET Core.

Por ejemplo:

```text
Controller
    ↓
UserService
    ↓
Repository
    ↓
DbContext
```

Cada componente recibe las dependencias que necesita.

Esto facilita:

- sustituir implementaciones;
- realizar pruebas unitarias;
- reducir el acoplamiento;
- mantener una arquitectura modular.

La configuración de estas dependencias se concentra en el directorio destinado a la inyección de dependencias.

---

# Test

El directorio `Test` contiene las pruebas del proyecto.

Se utilizan herramientas como:

- **NUnit**
- **Moq**
- **FluentAssertions**
- **Testcontainers**

La estructura contempla pruebas para diferentes partes del sistema:

```text
Test/
├── Mapper/
├── Repository/
├── Cache/
└── Notifications/
```

---

## Tests de Mapper

Comprueban que las conversiones entre:

```text
Model ↔ DTO
```

funcionan correctamente.

Esto es importante porque los mappers se utilizan constantemente entre las diferentes capas.

---

## Tests de Repository

Los repositorios se prueban mediante **Testcontainers**.

Esto permite ejecutar las pruebas contra un entorno de base de datos real dentro de un contenedor, en lugar de limitarse a comprobar el código mediante mocks.

De esta manera se pueden comprobar operaciones como:

```text
Create
GetAll
GetById
Update
Delete
```

y su interacción real con la persistencia.

---

## Tests de Cache

Comprueban el comportamiento de:

```text
Get
Set
Remove
RemoveAll
```

de la capa de caché.

---

## Tests de Notifications

Se prueban tanto el emisor como el observador.

Entre las situaciones comprobadas se encuentran:

- publicación de mensajes de creación;
- publicación de mensajes de actualización;
- publicación de mensajes de eliminación;
- publicación de errores;
- finalización del `Subject`;
- suscripción del observador;
- desuscripción del observador;
- escritura de mensajes en consola.

---

# Bruno

Además de las pruebas automatizadas del proyecto, existe una carpeta destinada a las pruebas realizadas con **Bruno**.

Estas pruebas permiten comprobar la API desde el punto de vista del cliente HTTP.

Se pueden probar las diferentes operaciones:

```text
GET
POST
PUT
DELETE
```

sobre los endpoints expuestos por `UserController`.

La diferencia respecto a las pruebas unitarias es que Bruno permite comprobar el comportamiento de la API mediante peticiones HTTP reales.

---

# Flujo de una petición

## GET por ID

Una petición:

```http
GET /api/User/1
```

sigue conceptualmente el siguiente flujo:

```text
Cliente
   │
   ▼
Controller
   │
   ▼
UserService
   │
   ▼
Cache
   │
   ├── Encontrado → devuelve usuario
   │
   └── No encontrado
            │
            ▼
        Repository
            │
            ├── Encontrado → guarda en cache
            │
            └── No encontrado
                    │
                    ▼
              API externa
                    │
                    ▼
              Base de datos
                    │
                    ▼
                  Cache
                    │
                    ▼
                Cliente
```

El objetivo es utilizar primero las fuentes más rápidas y evitar acceder innecesariamente a recursos externos.

---

# Flujo de creación

Una creación sigue conceptualmente:

```text
Cliente
   │
   ▼
Controller
   │
   ▼
Validator
   │
   ▼
UserService
   │
   ├── API externa
   │
   ├── Repository
   │
   ├── Cache
   │
   └── Notification
   │
   ▼
Response DTO
   │
   ▼
Cliente
```

El servicio coordina las diferentes operaciones y devuelve un `Result` indicando si la operación terminó correctamente o produjo un error de dominio.

---

# Flujo de actualización

```text
Cliente
   │
   ▼
Controller
   │
   ▼
UserService
   │
   ├── Validación
   ├── API externa
   ├── Repository
   ├── Cache
   └── Notification
   │
   ▼
UserResponseDto
   │
   ▼
Cliente
```

---

# Flujo de eliminación

La eliminación se gestiona desde el servicio y posteriormente se actualizan las diferentes fuentes implicadas.

Además, la entidad dispone de propiedades para trabajar con **soft delete**:

```text
IsDeleted
DeletedAt
```

En lugar de depender únicamente de eliminar físicamente un registro, se puede marcar como eliminado.

El `DbContext` utiliza un filtro global para que estos registros no aparezcan en las consultas normales.

---

# Sincronización

La sincronización periódica tiene como objetivo que la información local no quede desactualizada respecto a la API remota.

Cada 60 segundos:

```text
┌──────────────────────────────┐
│ BackgroundService            │
└──────────────┬───────────────┘
               │
               ▼
        Eliminar caché
               │
               ▼
        Limpiar BD local
               │
               ▼
        Consultar API externa
               │
               ▼
        Obtener usuarios
               │
               ▼
        Guardar en BD local
```

Al centralizar este proceso en un `BackgroundService`, las peticiones HTTP no tienen que encargarse de realizar la sincronización.

---

# Separación Model / Entity / DTO

Una de las decisiones importantes del proyecto es no utilizar una única clase `User` para todo.

Se distinguen:

```text
Model
   ↓
Representación interna

Entity
   ↓
Representación de persistencia

DTO
   ↓
Representación de entrada/salida HTTP
```

Por ejemplo:

```text
User
UserEntity
UserCreateDto
UserUpdateDto
UserResponseDto
```

Aunque algunos tengan propiedades similares, tienen responsabilidades diferentes.

Esto permite modificar la estructura de la base de datos o de la API pública sin tener que modificar necesariamente todo el sistema.

---

# ¿Por qué utilizar esta arquitectura?

La principal razón es **separar responsabilidades**.

Sin separación de capas, una única clase podría terminar encargándose de:

```text
HTTP
↓
Validación
↓
Base de datos
↓
Caché
↓
API externa
↓
Errores
↓
Notificaciones
```

Esto produciría una aplicación difícil de mantener y probar.

Con la arquitectura utilizada:

```text
Controller
    ↓
Service
    ↓
Repository / Cache / API
    ↓
Infrastructure
```

cada componente tiene una función concreta.

---

# Responsabilidad de cada directorio

| Directorio            | Responsabilidad                             |
| --------------------- | ------------------------------------------- |
| `Models`              | Modelos utilizados por la aplicación        |
| `Entity`              | Persistencia mediante Entity Framework Core |
| `Dto`                 | Datos de entrada y salida de la API         |
| `Repositories`        | Acceso y operaciones CRUD sobre la BD       |
| `Services`            | Lógica de negocio y coordinación            |
| `Controller`          | Endpoints HTTP                              |
| `Errors`              | Errores de dominio y conversión a HTTP      |
| `Cache`               | Gestión de caché                            |
| `API`                 | Comunicación con la API REST externa        |
| `Notifications`       | Sistema reactivo de notificaciones          |
| `Validations`         | Validación de datos                         |
| `Sync`                | Sincronización periódica                    |
| `DependencyInjection` | Configuración de dependencias               |
| `Test`                | Pruebas automatizadas y de integración      |

---

# Tecnologías utilizadas

| Tecnología                     | Uso                                       |
| ------------------------------ | ----------------------------------------- |
| **.NET / ASP.NET Core**        | Desarrollo de la API REST                 |
| **C#**                         | Lenguaje principal                        |
| **Entity Framework Core**      | Persistencia                              |
| **IDistributedCache**          | Abstracción de caché                      |
| **Redis**                      | Caché distribuida                         |
| **System.Reactive**            | Sistema de notificaciones                 |
| **CSharpFunctionalExtensions** | Gestión funcional de resultados y errores |
| **NUnit**                      | Framework de pruebas                      |
| **Moq**                        | Mocking en pruebas                        |
| **FluentAssertions**           | Asserts más expresivos                    |
| **Testcontainers**             | Pruebas de integración con contenedores   |
| **Bruno**                      | Pruebas manuales de la API                |
| **JSONPlaceholder**            | API REST externa                          |

---

# Principios aplicados

El proyecto intenta aplicar varios principios habituales en el desarrollo backend:

### Separación de responsabilidades

Cada capa tiene una responsabilidad concreta.

### Inyección de dependencias

Las clases reciben sus dependencias desde el contenedor de ASP.NET Core.

### Reutilización

El repositorio CRUD genérico permite reutilizar las operaciones comunes.

### Bajo acoplamiento

Las interfaces permiten sustituir implementaciones sin modificar las capas que las utilizan.

### DTOs

La API no depende directamente de las entidades de persistencia.

### Programación reactiva

Las notificaciones utilizan el patrón `Subject/Observer`.

### Tratamiento explícito de errores

Los errores esperables se representan mediante `Result` y `DomainError`.

### Testabilidad

La separación por capas permite probar individualmente mappers, repositorios, caché y notificaciones.

---

# Resumen de la arquitectura

El funcionamiento general puede resumirse así:

```text
                         CLIENTE
                            │
                            ▼
                    ┌───────────────┐
                    │   Controller  │
                    └───────┬───────┘
                            │
                            ▼
                    ┌───────────────┐
                    │    Service    │
                    └───────┬───────┘
                            │
             ┌──────────────┼──────────────┐
             │              │              │
             ▼              ▼              ▼
         ┌───────┐    ┌───────────┐   ┌─────────┐
         │ Cache │    │ Repository│   │   API   │
         └───────┘    └─────┬─────┘   │ externa │
                             │         └─────────┘
                             ▼
                       ┌──────────┐
                       │ DbContext│
                       └────┬─────┘
                            │
                            ▼
                        ┌───────┐
                        │   DB  │
                        └───────┘


             ┌────────────────────────┐
             │ BackgroundService      │
             │ sincronización 60s     │
             └───────────┬────────────┘
                         │
                         ▼
                    API externa


             ┌────────────────────────┐
             │     Notifications      │
             │   Subject / Observer   │
             └────────────────────────┘
```

El resultado es una API organizada en capas donde el **Controller recibe las peticiones, el Service coordina la lógica de negocio y las diferentes infraestructuras se encargan de persistencia, caché, comunicación externa y notificaciones**.

Esta separación permite que el proyecto pueda crecer incorporando nuevas entidades, operaciones o infraestructuras sin tener que concentrar toda la lógica en una única parte de la aplicación.
