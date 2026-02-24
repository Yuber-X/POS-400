-- bdposV2_limpio.sql
-- Script: creación limpia de bdposV2 con roles, permisos, mappings y triggers
-- ADVERTENCIA: DROP DATABASE IF EXISTS (BORRARÁ datos previos)

DROP DATABASE IF EXISTS bdposV2;
CREATE DATABASE bdposV2 CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE bdposV2;

-- =================================================================
-- TABLAS PRINCIPALES
-- =================================================================

-- Tabla Rol
CREATE TABLE rol (
    idRol INT AUTO_INCREMENT PRIMARY KEY,
    nombreRol VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB;

-- Tabla Permiso (catalogo de permisos / forms)
CREATE TABLE permiso (
    idPermiso INT AUTO_INCREMENT PRIMARY KEY,
    nombrePermiso VARCHAR(100) NOT NULL,
    claveForm VARCHAR(50) NOT NULL UNIQUE
) ENGINE=InnoDB;

-- Tabla que mapea rol -> permiso (definimos permisos por rol)
CREATE TABLE rol_permiso (
    fkRol INT NOT NULL,
    fkPermiso INT NOT NULL,
    PRIMARY KEY (fkRol, fkPermiso),
    FOREIGN KEY (fkRol) REFERENCES rol(idRol) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (fkPermiso) REFERENCES permiso(idPermiso) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Tabla Usuario
CREATE TABLE usuario (
    idUsuario BIGINT AUTO_INCREMENT PRIMARY KEY,
    nombreUsuario VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    fkRol INT NULL,
    nombre VARCHAR(100) NULL,
    apellido VARCHAR(100) NULL,
    FOREIGN KEY (fkRol) REFERENCES rol(idRol) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Tabla usuario_permiso (permiso asignado a usuario) - uso operativo
CREATE TABLE usuario_permiso (
    fkUsuario BIGINT NOT NULL,
    fkPermiso INT NOT NULL,
    PRIMARY KEY (fkUsuario, fkPermiso),
    FOREIGN KEY (fkUsuario) REFERENCES usuario(idUsuario) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (fkPermiso) REFERENCES permiso(idPermiso) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Tabla Cliente
CREATE TABLE cliente (
    idCliente BIGINT AUTO_INCREMENT PRIMARY KEY,
    nombres VARCHAR(100),
    appaterno VARCHAR(100),
    appmaterno VARCHAR(100)
) ENGINE=InnoDB;

-- Tabla Producto
CREATE TABLE producto (
    idProducto BIGINT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100),
    precioProducto DECIMAL(10,2),
    stock INT,
    descripcionProducto TEXT,
    fechaCaducidad DATE
) ENGINE=InnoDB;

-- Tabla Factura
CREATE TABLE factura (
    idFactura BIGINT AUTO_INCREMENT PRIMARY KEY,
    fechaFactura DATETIME,
    metodoPago ENUM('Efectivo','Tarjeta') NOT NULL DEFAULT 'Efectivo',
    fkCliente BIGINT NULL,
    fkUsuario BIGINT NULL,
    FOREIGN KEY (fkCliente) REFERENCES cliente(idCliente) ON DELETE SET NULL ON UPDATE CASCADE,
    FOREIGN KEY (fkUsuario) REFERENCES usuario(idUsuario) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Tabla Detalle
CREATE TABLE detalle (
    idDetalle BIGINT AUTO_INCREMENT PRIMARY KEY,
    fkFactura BIGINT,
    fkProducto BIGINT,
    cantidad INT,
    precioVenta DECIMAL(10,2),
    FOREIGN KEY (fkFactura) REFERENCES factura(idFactura) ON DELETE CASCADE ON UPDATE CASCADE,
    FOREIGN KEY (fkProducto) REFERENCES producto(idProducto) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Tabla userSession
CREATE TABLE userSession (
    idSession BIGINT AUTO_INCREMENT PRIMARY KEY,
    fkUsuario BIGINT NOT NULL,
    loginTime DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    logoutTime DATETIME NULL,
    FOREIGN KEY (fkUsuario) REFERENCES usuario(idUsuario) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB;

-- =================================================================
-- POBLADO INICIAL (roles + permisos + mapping rol->permiso)
-- =================================================================

-- Insertar roles base
INSERT INTO rol (nombreRol) VALUES ('ADMIN'), ('USER'), ('CAJERO'), ('VENDEDOR'), ('SUPERVISOR');

-- Insertar permisos (ajusta claves si tu app espera otras)
INSERT INTO permiso (nombrePermiso, claveForm) VALUES
('Administrador de Ventas','formVentas'),
('Administrador de Clientes','formClientes'),
('Administrador de Productos','formProductos'),
('Administrador de Caducidad','formCaducidad'),
('Administrador de Comprobante','formBuscarComprobante'),
('Administrador de Registro por día','formCuadre'),
('Administrador de Registro por fecha','formRegistroPorFecha'),
('Administrador de Almacén','formAlmacen'),
('Administrador de Usuarios','formAdminMode');

-- Mapear permisos por rol (rol_permiso)
-- ADMIN => todos los permisos
INSERT INTO rol_permiso (fkRol, fkPermiso)
SELECT r.idRol, p.idPermiso
FROM rol r
CROSS JOIN permiso p
WHERE r.nombreRol = 'ADMIN';

-- USER => permisos limitados (ejemplo)
INSERT INTO rol_permiso (fkRol, fkPermiso)
SELECT r.idRol, p.idPermiso
FROM rol r
JOIN permiso p ON p.claveForm IN ('formVentas','formClientes','formBuscarComprobante')
WHERE r.nombreRol = 'USER';

-- CAJERO => ventas + clientes
INSERT INTO rol_permiso (fkRol, fkPermiso)
SELECT r.idRol, p.idPermiso
FROM rol r
JOIN permiso p ON p.claveForm IN ('formVentas','formClientes')
WHERE r.nombreRol = 'CAJERO';

-- VENDEDOR => ventas, clientes, buscar comprobante, cuadre
INSERT INTO rol_permiso (fkRol, fkPermiso)
SELECT r.idRol, p.idPermiso
FROM rol r
JOIN permiso p ON p.claveForm IN ('formVentas','formClientes','formBuscarComprobante','formCuadre')
WHERE r.nombreRol = 'VENDEDOR';

-- SUPERVISOR => muchos permisos
INSERT INTO rol_permiso (fkRol, fkPermiso)
SELECT r.idRol, p.idPermiso
FROM rol r
JOIN permiso p ON p.claveForm IN ('formVentas','formClientes','formBuscarComprobante','formCuadre',
                                  'formCaducidad','formProductos','formAdminMode','formAlmacen','formRegistroPorFecha')
WHERE r.nombreRol = 'SUPERVISOR';

-- =================================================================
-- USUARIO ADMIN INICIAL (contraseña en texto plano por compatibilidad con tu app actual)
-- =================================================================
-- (Si tu app usa hashing, reemplaza la contraseña por el hash correspondiente)
INSERT INTO usuario (nombreUsuario, password, fkRol, nombre, apellido)
VALUES ('admin', 'admin123', (SELECT idRol FROM rol WHERE nombreRol = 'ADMIN'), 'Administrador', 'Local');

-- Asignar permisos iniciales a usuarios basados en rol (para usuarios ya creados)
INSERT INTO usuario_permiso (fkUsuario, fkPermiso)
SELECT u.idUsuario, rp.fkPermiso
FROM usuario u
JOIN rol_permiso rp ON rp.fkRol = u.fkRol
WHERE u.fkRol IS NOT NULL;

-- =================================================================
-- TRIGGERS: al insertar/actualizar usuario, sincronizamos usuario_permiso con rol_permiso
-- =================================================================

DELIMITER $$

CREATE TRIGGER trg_usuario_after_insert
AFTER INSERT ON usuario
FOR EACH ROW
BEGIN
    -- Asignar al usuario los permisos del rol asignado (si tiene rol)
    IF NEW.fkRol IS NOT NULL THEN
        INSERT IGNORE INTO usuario_permiso (fkUsuario, fkPermiso)
        SELECT NEW.idUsuario, rp.fkPermiso
        FROM rol_permiso rp
        WHERE rp.fkRol = NEW.fkRol;
    END IF;
END$$

CREATE TRIGGER trg_usuario_after_update
AFTER UPDATE ON usuario
FOR EACH ROW
BEGIN
    -- Si cambió el rol, actualizar permisos del usuario
    IF (OLD.fkRol IS NULL AND NEW.fkRol IS NOT NULL) OR (OLD.fkRol IS NOT NULL AND NEW.fkRol IS NULL) OR (OLD.fkRol <> NEW.fkRol) THEN
        -- Eliminar permisos previos (los que vinieron por rol)
        DELETE FROM usuario_permiso WHERE fkUsuario = NEW.idUsuario;

        -- Asignar los permisos correspondientes al nuevo rol (si tiene)
        IF NEW.fkRol IS NOT NULL THEN
            INSERT IGNORE INTO usuario_permiso (fkUsuario, fkPermiso)
            SELECT NEW.idUsuario, rp.fkPermiso
            FROM rol_permiso rp
            WHERE rp.fkRol = NEW.fkRol;
        END IF;
    END IF;
END$$

DELIMITER ;

-- =================================================================
-- INDICES ÚTILES
-- =================================================================
CREATE INDEX idx_factura_fecha ON factura (fechaFactura);
CREATE INDEX idx_producto_nombre ON producto (nombre);

-- =================================================================
-- FIN - base creada y poblada
-- =================================================================

