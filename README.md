[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/0jLIDEDY)
# Guía de Despliegue a Azure para el Proyecto Vehículo

Esta guía te ayudará a desplegar tu aplicación completa (API .NET, Base de Datos SQL y aplicación Web React) en Azure utilizando GitHub Actions.

## 📋 Requisitos Previos

- Una cuenta de Azure (puedes usar la versión gratuita)
- Una cuenta de GitHub con tu repositorio
- El proyecto Vehículo completo en tu repositorio

## 🎯 Resumen de Pasos

1. Crear recursos en Azure (App Service y SQL Database)
2. Descargar los perfiles de publicación
3. Configurar GitHub (Environments y Secrets)
4. Crear GitHub Actions workflows

---

## 1️⃣ Crear Recursos en Azure

### 1.1 Crear App Service para la API

1. Inicia sesión en [Azure Portal](https://portal.azure.com)
2. Haz clic en **"Crear un recurso"**
3. Busca y selecciona **"Aplicación web"**
4. Configura los siguientes valores:
   - **Suscripción**: Selecciona tu suscripción
   - **Grupo de recursos**: Crea uno nuevo o usa uno existente (ej: `rg-vehiculo`)
   - **Nombre**: `vehiculo-api` (debe ser único globalmente)
   - **Publicar**: Código
   - **Pila del entorno en tiempo de ejecución**: .NET 8 (LTS)
   - **Sistema operativo**: Linux
   - **Región**: Elige la más cercana (ej: East US)
   - **Plan de Linux**: Crea uno nuevo
   - **Plan de precios**: F1 (Gratis) - En la pestaña "Cambiar tamaño"
5. Haz clic en **"Revisar y crear"** y luego en **"Crear"**

### 1.2 Crear App Service para la Web (React)

1. Repite los pasos anteriores con estos cambios:
   - **Nombre**: `vehiculo-web` (debe ser único globalmente)
   - **Pila del entorno en tiempo de ejecución**: Node 18 LTS
   - **Sistema operativo**: Linux
   - **Plan de precios**: F1 (Gratis)
2. Haz clic en **"Revisar y crear"** y luego en **"Crear"**

### 1.3 Crear Base de Datos SQL

1. En Azure Portal, haz clic en **"Crear un recurso"**
2. Busca y selecciona **"Base de datos SQL"**
3. Configura los siguientes valores:
   - **Suscripción**: Selecciona tu suscripción
   - **Grupo de recursos**: Usa el mismo (ej: `rg-vehiculo`)
   - **Nombre de base de datos**: `vehiculo-db`
   - **Servidor**: Haz clic en "Crear nuevo"
     - **Nombre del servidor**: `vehiculo-db-server` (debe ser único)
     - **Ubicación**: La misma región que tus App Services
     - **Autenticación**: Usar autenticación SQL
     - **Inicio de sesión del administrador**: `sqladmin`
     - **Contraseña**: Crea una contraseña segura (¡guárdala!)
     - Marca la casilla: **"Permitir que los servicios de Azure accedan a este servidor"**
   - **¿Desea usar grupo elástico de SQL?**: No
   - **Proceso y almacenamiento**: Haz clic en "Configurar base de datos"
     - Selecciona **"Basic"** (es el más económico/gratuito para desarrollo)
4. En la pestaña **"Redes"**:
   - Método de conectividad: **Punto de conexión público**
   - Reglas de firewall: 
     - Marca: **"Permitir que los servicios y recursos de Azure accedan a este servidor"**
     - Marca: **"Agregar dirección IP de cliente actual"** (para desarrollo local)
5. Haz clic en **"Revisar y crear"** y luego en **"Crear"**

---

## 2️⃣ Descargar Perfiles de Publicación

### 2.1 Perfil de Publicación de la API

1. En Azure Portal, navega a tu App Service de la API (`vehiculo-api`)
2. En el menú izquierdo, haz clic en **"Información general"**
3. En la barra superior, haz clic en **"Obtener perfil de publicación"**
4. Se descargará un archivo `.PublishSettings`
5. Abre el archivo con un editor de texto y copia todo su contenido
6. **Guarda este contenido** - lo usarás en GitHub Secrets como `AZURE_WEBAPP_PUBLISH_PROFILE`

### 2.2 Perfil de Publicación de la Web

1. Repite los mismos pasos para el App Service de la web (`vehiculo-web`)
2. **Guarda este contenido** - lo usarás en GitHub Secrets como `AZURE_WEBAPP_PUBLISH_PROFILE_WEB`

### 2.3 Obtener Cadena de Conexión de SQL

1. En Azure Portal, navega a tu Base de datos SQL (`vehiculo-db`)
2. En el menú izquierdo, haz clic en **"Cadenas de conexión"**
3. Copia la cadena de conexión **"ADO.NET"**
4. Reemplaza `{your_password}` con la contraseña que creaste
5. **Formato final**:
   ```
   Server=tcp:vehiculo-db-server.database.windows.net,1433;Initial Catalog=vehiculo-db;Persist Security Info=False;User ID=sqladmin;Password=TU_CONTRASEÑA;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
   ```
6. **Guarda esta cadena** - la usarás en GitHub Secrets como `AZURE_SQL_CONNECTION_STRING`

---

## 3️⃣ Configurar GitHub

### 3.1 Crear Environments

1. Ve a tu repositorio en GitHub
2. Haz clic en **"Settings"** (Configuración)
3. En el menú izquierdo, haz clic en **"Environments"**
4. Crea tres environments:
   - Haz clic en **"New environment"**
   - Nombre: `Production` → **"Configure environment"**
   - Repite para: `Production-Web` y `Production-Database`

### 3.2 Crear Secrets

1. En tu repositorio de GitHub, ve a **"Settings"** → **"Secrets and variables"** → **"Actions"**
2. Haz clic en **"New repository secret"** y crea los siguientes secrets:

   **Secret 1: Perfil de Publicación de la API**
   - Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
   - Value: Pega el contenido completo del archivo `.PublishSettings` de la API
   - Haz clic en **"Add secret"**

   **Secret 2: Perfil de Publicación de la Web**
   - Name: `AZURE_WEBAPP_PUBLISH_PROFILE_WEB`
   - Value: Pega el contenido completo del archivo `.PublishSettings` de la Web
   - Haz clic en **"Add secret"**

   **Secret 3: Cadena de Conexión de SQL**
   - Name: `AZURE_SQL_CONNECTION_STRING`
   - Value: Pega la cadena de conexión completa de SQL (con tu contraseña)
   - Haz clic en **"Add secret"**

---

## 4️⃣ Crear GitHub Actions Workflows

### 4.1 Estructura de Carpetas

Crea la siguiente estructura en tu repositorio:

```
.github/
  workflows/
    deploy-api.yml
    deploy-database.yml
    deploy-webapp.yml
```

### 4.2 Crear los Archivos de Workflow

**Paso 1: Crear la carpeta `.github/workflows/`**

Si usas Git Bash o terminal:
```bash
mkdir -p .github/workflows
```

O créala manualmente en tu editor de código.

**Paso 2: Copiar los archivos YML**

Copia los tres archivos desde la carpeta `Azure/Yml/` de este repositorio a `.github/workflows/`:

1. Copia `Azure/Yml/deploy-api.yml` a `.github/workflows/deploy-api.yml`
2. Copia `Azure/Yml/deploy-database.yml` a `.github/workflows/deploy-database.yml`
3. Copia `Azure/Yml/deploy-webapp.yml` a `.github/workflows/deploy-webapp.yml`

**Paso 3: Actualizar los nombres de recursos en los archivos**

Abre cada archivo y actualiza los nombres de recursos con los que creaste en Azure:

**En `deploy-api.yml`:**
```yaml
env:
  AZURE_WEBAPP_NAME: vehiculo-api    # Cambia esto si usaste otro nombre
```

**En `deploy-database.yml`:**
```yaml
env:
  AZURE_SQL_SERVER: vehiculo-db-server.database.windows.net    # Tu servidor
  AZURE_SQL_DATABASE: vehiculo-db    # Tu base de datos
```

**En `deploy-webapp.yml`:**
```yaml
env:
  AZURE_WEBAPP_NAME: vehiculo-web    # Cambia esto si usaste otro nombre
```

---

## 5️⃣ Desplegar a Azure

### 5.1 Primer Despliegue - Base de Datos

1. Realiza un commit y push de los cambios:
   ```bash
   git add .
   git commit -m "Add Azure deployment workflows"
   git push origin main
   ```

2. Ve a tu repositorio en GitHub → **"Actions"**
3. Deberías ver los workflows ejecutándose
4. **Primero** asegúrate de que el workflow **"Deploy Vehiculo Database to Azure SQL"** se complete exitosamente
5. Esto creará las tablas y estructura de la base de datos en Azure

### 5.2 Segundo Despliegue - API

1. Una vez que la base de datos esté desplegada, verifica que el workflow **"Deploy Vehiculo API to Azure"** se ejecute
2. Este workflow debería completarse exitosamente después de la base de datos

### 5.3 Tercer Despliegue - Web

1. Finalmente, el workflow **"Deploy Vehiculo React Web App to Azure"** se ejecutará
2. Una vez completado, tu aplicación web estará disponible

### 5.4 Verificar los Despliegues

Prueba tus aplicaciones:

1. **API**: `https://vehiculo-api.azurewebsites.net`
2. **Web**: `https://vehiculo-web.azurewebsites.net`

(Reemplaza con tus nombres reales de App Services)

---

## 🔧 Configuración Adicional

### Configurar la Cadena de Conexión en el App Service de la API

1. Ve a tu App Service de la API en Azure Portal
2. En el menú izquierdo, selecciona **"Configuración"** → **"Cadenas de conexión"**
3. Haz clic en **"+ Nueva cadena de conexión"**
4. Configura:
   - **Nombre**: `DefaultConnection`
   - **Valor**: Tu cadena de conexión de SQL
   - **Tipo**: SQLAzure
5. Haz clic en **"Aceptar"** y luego en **"Guardar"**

### Configurar la URL de la API en la Web

1. Ve a tu App Service de la Web en Azure Portal
2. En el menú izquierdo, selecciona **"Configuración"** → **"Configuración de la aplicación"**
3. Haz clic en **"+ Nueva configuración de aplicación"**
4. Configura:
   - **Nombre**: `VITE_API_URL`
   - **Valor**: `https://vehiculo-api.azurewebsites.net`
5. Haz clic en **"Aceptar"** y luego en **"Guardar"**

---

## 🚀 Despliegues Automáticos

Una vez configurado todo, los despliegues se realizarán automáticamente:

- **API**: Cada vez que hagas push y cambies archivos en `Semana 05-API y WEB/Vehiculo.API/` (excepto BD)
- **Base de Datos**: Cada vez que hagas push y cambies archivos en `Semana 05-API y WEB/Vehiculo.API/BD/`
- **Web**: Cada vez que hagas push y cambies archivos en `Semana 05-API y WEB/Vehiculo.React/`

También puedes ejecutarlos manualmente:
1. Ve a **"Actions"** en GitHub
2. Selecciona el workflow que deseas ejecutar
3. Haz clic en **"Run workflow"** → **"Run workflow"**

---

## ❓ Solución de Problemas

### Error: "404 Not Found" en la Web

- Verifica que el archivo `web.config` esté en la carpeta `dist/` con la configuración SPA correcta
- Añadir o verificar que exista el archivo en tu proyecto React

### Error: "Database connection failed" en la API

- Verifica que la cadena de conexión esté correctamente configurada en App Service
- Asegúrate de que el firewall de SQL permita conexiones desde Azure

### Error en el build de la API

- Verifica que todos los proyectos estén referenciados correctamente
- Asegúrate de que no haya errores de compilación local primero

### Error en GitHub Actions

- Revisa los logs detallados en la pestaña "Actions"
- Verifica que todos los Secrets estén creados correctamente (nombres y valores)
- Asegúrate de que los nombres de recursos coincidan con los de Azure

---

## 📚 Recursos Adicionales

- [Documentación de Azure App Service](https://docs.microsoft.com/azure/app-service/)
- [Documentación de Azure SQL Database](https://docs.microsoft.com/azure/azure-sql/)
- [Documentación de GitHub Actions](https://docs.github.com/actions)
- [Azure for Students](https://azure.microsoft.com/free/students/)

---

## ✅ Checklist de Despliegue

Usa este checklist para asegurarte de que completaste todos los pasos:

- [ ] Crear App Service para la API (vehiculo-api)
- [ ] Crear App Service para la Web (vehiculo-web)
- [ ] Crear SQL Database (vehiculo-db)
- [ ] Descargar perfil de publicación de la API
- [ ] Descargar perfil de publicación de la Web
- [ ] Obtener cadena de conexión de SQL
- [ ] Crear Environments en GitHub (Production, Production-Web, Production-Database)
- [ ] Crear Secret: AZURE_WEBAPP_PUBLISH_PROFILE
- [ ] Crear Secret: AZURE_WEBAPP_PUBLISH_PROFILE_WEB
- [ ] Crear Secret: AZURE_SQL_CONNECTION_STRING
- [ ] Crear carpeta .github/workflows/
- [ ] Copiar deploy-api.yml
- [ ] Copiar deploy-database.yml
- [ ] Copiar deploy-webapp.yml
- [ ] Actualizar nombres de recursos en los YML
- [ ] Hacer commit y push
- [ ] Verificar que los workflows se ejecuten exitosamente
- [ ] Configurar cadena de conexión en App Service de la API
- [ ] Configurar URL de API en App Service de la Web
- [ ] Probar la API en el navegador
- [ ] Probar la Web en el navegador

¡Felicidades! 🎉 Tu aplicación ahora está desplegada en Azure y se actualizará automáticamente con cada push a tu repositorio.
