create database bdpos;
use bdpos;

create table producto(
idProducto bigint  auto_increment not null primary key, 
nombre varchar(100),
precioProducto decimal(5,2),
stock int,
descripcionProducto text(200)
);

/*Mostrando todos los productos*/
select idproducto, nombre, precioProducto, stock, descripcionProducto from producto;

/*Ingresando un registro de prueba*/
insert into producto(idProducto,nombre, precioProducto, stock, descripcionProducto) values (8543518723184,"Alcohol Isoprilico", 480, 15,"Alcohol para limpiar impuresas");

/*Actualizar registro*/
UPDATE producto SET producto.idProducto = 8543518723184 , producto.nombre = "Alcohol Isoprilico", producto.precioProducto = 480, producto.stock = 15, producto.descripcionProducto = "Alcohol para limpiar impuresas" where producto.idProducto = 1;

/*-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------*/

create table cliente(
idCliente bigint auto_increment not null primary key,
nombres varchar(100),
appaterno varchar(100),
appmaterno varchar(100)
);

/*Ingresando un registro de prueba*/
insert into cliente(nombres,appaterno,appmaterno) values ("Juan","Perez", "Cano");

/*Mostrando todos los clientes*/
select idcliente, nombres, appaterno, appmaterno from cliente;

/*Eliminar*/
delete from cliente where cliente.idCliente = 6;

/*Actualizar registro*/
UPDATE cliente SET cliente.nombres = "Scott", cliente.appaterno = "Soto", cliente.appmaterno = "Villalona" where cliente.idcliente = 5;

/*----------------------------------------------------------------------------------------------------------------------------------------*/

create table factura(
idFactura bigint auto_increment not null primary key,
fechaFactura date,
fkCliente bigint,
foreign key(fkCliente) references cliente(idCliente) 
);

/*Mostrar todas las facturas*/
select * from factura;

/*Creando Factura*/
insert into factura (fechaFactura, fkCliente) values (curdate(),1);

/*Mostrar Ultima Factura*/
Select max(idfactura) as UltimaFactura from factura;

/*Mostrar todos los datos Factura*/
select factura.idfactura, factura.fechaFactura, cliente.nombres, cliente.appaterno, cliente.appmaterno from factura
INNER JOIN cliente ON cliente.idCliente = factura.fkcliente WHERE factura.idFactura = 2; 

/*Mostrar Productos en base al numero de factura*/
select producto.nombre, detalle.cantidad, detalle.precioventa from detalle
INNER JOIN factura ON factura.idfactura = detalle.fkfactura
INNER JOIN producto ON producto.idproducto = detalle.fkproducto
WHERE factura.idfactura = 7;

/*Mostrar ingresos totales por fecha*/
select factura.idfactura, factura.fechaFactura, detalle.cantidad, detalle.precioVenta from detalle
INNER JOIN factura ON factura.idfactura = detalle.fkfactura
INNER JOIN producto ON producto.idproducto = detalle.fkproducto
WHERE factura.fechaFactura  between 10/03/2025 AND 24/03/2025;

SELECT factura.idfactura, factura.fechaFactura, producto.nombre, detalle.cantidad, detalle.precioventa 
FROM detalle 
INNER JOIN factura ON factura.idfactura = detalle.fkfactura 
INNER JOIN producto ON producto.idproducto = detalle.fkproducto 
WHERE factura.fechaFactura BETWEEN '2025-03-01' AND '2025-03-25';



create table detalle(
idDetalle bigint auto_increment not null primary key,
fkFactura bigint,
foreign key (fkFactura) references factura(idFactura),
fkProducto bigint,
foreign key (fkProducto) references producto(idProducto),
cantidad int,
precioVenta decimal(5,2)
);

/*Insertar Detalle de prueba*/
insert into detalle (fkFactura, fkProducto, cantidad, precioVenta) values ((select max(idFactura) from factura),?,?,?);

/*Reducir Stock*/
update producto set stock = stock - ? where idProducto = ?;

/*Mostrar Detalle*/
select * from detalle;

/*Consulta Dinamica*/
SELECT * FROM producto WHERE producto.nombre LIKE CONCAT("%","A","%");

/*-----------------------------------------------------------------------------------------------------------*/
/*Alterando tipos de datos*/
/*cambiar el tipo de dato idProducto por un BIGINT*/
SHOW CREATE TABLE producto;
ALTER TABLE producto MODIFY COLUMN idProducto BIGINT NOT NULL AUTO_INCREMENT; /*Cambiando el tipo de dato del primary key. NO FUNCIONÓ*/
ALTER TABLE producto DROP FOREIGN KEY fk_Producto_categoria; /**eliminando la clave foranea. NO FUNCIONÓ*/

SHOW CREATE TABLE detalle;
ALTER TABLE detalle DROP FOREIGN KEY detalle_ibfk_1;
ALTER TABLE producto MODIFY id BIGINT UNSIGNED;
ALTER TABLE detalle MODIFY fkProducto BIGINT UNSIGNED;

ALTER TABLE detalle ADD CONSTRAINT detalle_ibfk_2 foreign key (fkFactura) references factura(idFactura);
ALTER TABLE detalle ADD CONSTRAINT detalle_ibfk_1 foreign key (fkProducto) references producto(idProducto);
ALTER TABLE factura ADD CONSTRAINT detalle_ibfk_3 foreign key(fkCliente) references cliente(idCliente);







