# Gestión de Proyectos ONG

## Contexto General
La organización que gestiona proyectos en áreas rurales necesita llevar un control de los fondos que recibe de donantes y cómo estos se ejecutan en órdenes de compra para diversos rubros del proyecto. El sistema debe permitir gestionar proyectos, rubros, donaciones y órdenes de compra de manera eficiente y transparente.

## Entidades y Relaciones Clave

### 1. Proyecto
Un **proyecto** es un conjunto de actividades y metas que la ONG llevará a cabo en una región específica. Un proyecto tiene varias características:
- **Código de Proyecto**: Identificador único generado automáticamente (por ejemplo, P-0001, P-0002, ...).
- **Nombre del Proyecto**: Nombre descriptivo del proyecto.
- **Municipio y Departamento**: Localización del proyecto.
- **Fechas de Inicio y Fin**: Tiempo de ejecución del proyecto.

#### Relación con otras entidades:
Un proyecto puede tener varios rubros asociados, cada uno destinado a un tipo de gasto específico dentro del proyecto (por ejemplo, alimentación, materiales, etc.).

### 2. Rubro (o "renglón de presupuesto")
Un **rubro** es un tipo de gasto específico dentro de un proyecto. Ejemplos comunes incluyen **Alimentación**, **Honorarios**, **Materiales**, etc. Cada rubro tiene un nombre único dentro de un proyecto y contiene un monto asignado a él.

#### Relación con otros elementos:
Un rubro pertenece a un proyecto y puede recibir donaciones de los donantes. También puede ser utilizado en órdenes de compra que reflejan los gastos del proyecto.

### 3. Donación
Una **donación** es un aporte monetario que un donante (persona u organización) realiza para financiar un rubro específico de un proyecto. Cada donación tiene:
- **Monto**: Cantidad de dinero donado.
- **Fecha**: Fecha de la donación.
- **Donante**: Nombre del donante.

#### Relación con otros elementos:
Las donaciones se asignan a un rubro de un proyecto específico.

### 4. Orden de Compra
Una **orden de compra** representa el gasto de los fondos asignados a un rubro. Una orden de compra tiene:
- **Monto gastado**: Cuánto dinero se gasta.
- **Proveedor**: Entidad o persona que recibe el pago (por ejemplo, un hotel, proveedor de alimentos, etc.).
- **Fecha**: Fecha de la compra.

#### Relación con otros elementos:
La orden de compra consume los fondos de un rubro (por ejemplo, una compra de alimentos consume los fondos asignados al rubro "Alimentación").

---

## Flujo Lógico del Proyecto
1. **Creación del Proyecto**:
   El usuario crea un proyecto, asignando el nombre, fechas de inicio y fin, municipio y departamento. Este proyecto tendrá un código generado automáticamente (por ejemplo, P-0001).
2. **Creación de Rubros**:
   Una vez creado el proyecto, el usuario puede agregar varios rubros asociados a este proyecto, como **Alimentación**, **Honorarios**, **Materiales**, etc.
3. **Recepción de Donaciones**:
   Los donantes pueden hacer donaciones a un rubro específico de un proyecto. Se registra el monto de la donación, la fecha y el nombre del donante.
4. **Ejecutar Fondos (Órdenes de Compra)**:
   Cuando el proyecto empieza a gastar los fondos, se generan órdenes de compra que reflejan el gasto de los rubros del proyecto.
5. **Control de Fondos**:
   La ONG puede verificar la disponibilidad de fondos en cualquier momento, consultando la relación entre las donaciones recibidas y las órdenes de compra ejecutadas.

---

## Cómo Funcionan las APIs y Vistas (Flujo de Interacciones)

### API de Proyectos

Permite al usuario crear y listar proyectos. Al crear un proyecto, se genera su código automáticamente, y el usuario puede asociar varios rubros a este proyecto.

- **Endpoint**: 
  - `GET /api/proyectos` (para listar proyectos)
  - `POST /api/proyectos` (para crear un proyecto)

### API de Rubros

Permite la creación y gestión de los rubros dentro de un proyecto específico. Un rubro debe tener un nombre único dentro del proyecto.

- **Endpoint**: 
  - `GET /api/rubros/{idProyecto}` (para listar rubros de un proyecto)
  - `POST /api/rubros` (para crear un rubro)

### API de Donaciones

Permite registrar donaciones que se asignan a un rubro específico de un proyecto.

- **Endpoint**: 
  - `POST /api/donaciones` (para registrar una donación)

### API de Órdenes de Compra

Gestiona las órdenes de compra, las cuales reflejan el gasto de los fondos asignados a un rubro de un proyecto.

- **Endpoint**: 
  - `POST /api/ordenesCompra` (para crear una orden de compra)

---

## Backend Estructura Lógica

### API Proyectos (ProyectosController)

- **GET**: Para listar los proyectos (`/api/proyectos`).
- **POST**: Para crear un proyecto (`/api/proyectos`).
- **PUT**: Para actualizar un proyecto (`/api/proyectos/{id}`).
- **DELETE**: Para eliminar un proyecto (`/api/proyectos/{id}`).

### API Rubros (RubrosController)

- **GET**: Para listar los rubros de un proyecto (`/api/rubros/{idProyecto}`).
- **POST**: Para crear un rubro en un proyecto (`/api/rubros`).
- **PUT**: Para actualizar un rubro (`/api/rubros/{id}`).
- **DELETE**: Para eliminar un rubro (`/api/rubros/{id}`).

### API Donaciones (DonacionesController)

- **GET**: Para listar las donaciones de un rubro (`/api/donaciones/{idRubro}`).
- **POST**: Para registrar una donación a un rubro (`/api/donaciones`).
- **PUT**: Para actualizar una donación (`/api/donaciones/{id}`).
- **DELETE**: Para eliminar una donación (`/api/donaciones/{id}`).

### API Órdenes de Compra (OrdenesCompraController)

- **GET**: Para listar las órdenes de compra de un rubro (`/api/ordenesCompra/{idRubro}`).
- **POST**: Para registrar una orden de compra (`/api/ordenesCompra`).
- **PUT**: Para actualizar una orden de compra (`/api/ordenesCompra/{id}`).
- **DELETE**: Para eliminar una orden de compra (`/api/ordenesCompra/{id}`).

---

## Stack Completo

### Backend

- **Tecnología**: .NET Core 8
- **Tipo**: API RESTful
- **Servidor**: Somee Server (Réplica tipo Azure para tecnologías Microsoft)

### Frontend

- **Tecnología**: Bootstrap, JavaScript
- **Despliegue**: Netlify

### Control de Versiones

- **Plataforma**: GitHub

---

# Información de la Base de Datos y Configuración del Servidor

## Base de Datos
- **Nombre de la base de datos**: `fullstackdeveloperservirprueba`
- **Nombre completo de dominio**: `fullstackdeveloperservirprueba.mssql.somee.com`
- **Versión de SQL**: MS SQL 2022 Express

## Credenciales de Acceso
- **Nombre de usuario**: `drivash1_SQLLogin_1`
- **Contraseña de acceso**: `eyup3jf9pf`
- **Cadena de Conexión**: `workstation id=fullstackdeveloperservirprueba.mssql.somee.com;packet size=4096;user id=drivash1_SQLLogin_1;pwd=eyup3jf9pf;data source=fullstackdeveloperservirprueba.mssql.somee.com;persist security info=False;initial catalog=fullstackdeveloperservirprueba;TrustServerCertificate=True`

## Hosting y Dominio
- **Nombre del dominio principal**: `fullstackdeveloperservirprueba.somee.com`
- **Entorno**: `Windows Server 2022 (IIS 10.0, ASP.Net v2.0-4.8, ASP.Net Core)`
- **URL**: `https://fullstackdeveloperservirprueba.somee.com/swagger/index.html`

## Acceso FTP
- **Servidor FTP**: `ftp://fullstackdeveloperservirprueba.somee.com o ftp://155.254.244.27`
- **Directorio**: `/www.fullstackdeveloperservirprueba.somee.com`
- **Usuario**: `drivash1`
- **Contraseña**: `QQQ$Blade26gt`







