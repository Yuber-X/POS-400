CREATE DATABASE  IF NOT EXISTS `pos401` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `pos401`;
-- MySQL dump 10.13  Distrib 8.0.45, for Win64 (x86_64)
--
-- Host: localhost    Database: pos401
-- ------------------------------------------------------
-- Server version	8.0.45

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `cliente`
--

DROP TABLE IF EXISTS `cliente`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cliente` (
  `idCliente` bigint NOT NULL AUTO_INCREMENT,
  `nombres` varchar(150) DEFAULT NULL,
  `telefono` varchar(20) DEFAULT NULL,
  `direccion` text,
  PRIMARY KEY (`idCliente`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cliente`
--

LOCK TABLES `cliente` WRITE;
/*!40000 ALTER TABLE `cliente` DISABLE KEYS */;
INSERT INTO `cliente` VALUES (1,'Cliente General','000000000','N/A');
/*!40000 ALTER TABLE `cliente` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalle`
--

DROP TABLE IF EXISTS `detalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalle` (
  `idDetalle` bigint NOT NULL AUTO_INCREMENT,
  `fkFactura` bigint DEFAULT NULL,
  `fkProducto` bigint DEFAULT NULL,
  `cantidad` int DEFAULT NULL,
  `precio` decimal(10,2) DEFAULT NULL,
  `subtotal` decimal(10,2) DEFAULT NULL,
  `precioVenta` decimal(10,2) DEFAULT NULL,
  PRIMARY KEY (`idDetalle`),
  KEY `fkFactura` (`fkFactura`),
  KEY `fkProducto` (`fkProducto`),
  CONSTRAINT `detalle_ibfk_1` FOREIGN KEY (`fkFactura`) REFERENCES `factura` (`idFactura`),
  CONSTRAINT `detalle_ibfk_2` FOREIGN KEY (`fkProducto`) REFERENCES `producto` (`idProducto`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalle`
--

LOCK TABLES `detalle` WRITE;
/*!40000 ALTER TABLE `detalle` DISABLE KEYS */;
INSERT INTO `detalle` VALUES (1,4,2,2,NULL,NULL,34.00),(2,5,1,2,NULL,NULL,50.00),(3,6,1,1,NULL,NULL,50.00),(4,7,1,2,NULL,NULL,50.00);
/*!40000 ALTER TABLE `detalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `factura`
--

DROP TABLE IF EXISTS `factura`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `factura` (
  `idFactura` bigint NOT NULL AUTO_INCREMENT,
  `fechaFactura` datetime DEFAULT NULL,
  `fkUsuario` bigint DEFAULT NULL,
  `fkCliente` bigint DEFAULT NULL,
  `total` decimal(10,2) DEFAULT NULL,
  `metodoPago` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`idFactura`),
  KEY `fkUsuario` (`fkUsuario`),
  KEY `fkCliente` (`fkCliente`),
  CONSTRAINT `factura_ibfk_1` FOREIGN KEY (`fkUsuario`) REFERENCES `usuario` (`idUsuario`),
  CONSTRAINT `factura_ibfk_2` FOREIGN KEY (`fkCliente`) REFERENCES `cliente` (`idCliente`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `factura`
--

LOCK TABLES `factura` WRITE;
/*!40000 ALTER TABLE `factura` DISABLE KEYS */;
INSERT INTO `factura` VALUES (1,'2026-03-08 17:27:07',1,1,NULL,'Efectivo'),(2,'2026-03-08 17:28:40',1,1,NULL,'Efectivo'),(3,'2026-03-08 17:29:51',1,1,NULL,'Efectivo'),(4,'2026-03-08 17:44:10',1,1,NULL,'Efectivo'),(5,'2026-03-08 17:47:58',1,1,NULL,'Efectivo'),(6,'2026-03-08 17:52:21',1,1,NULL,'Efectivo'),(7,'2026-03-09 18:40:10',1,1,NULL,'Efectivo');
/*!40000 ALTER TABLE `factura` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permiso`
--

DROP TABLE IF EXISTS `permiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permiso` (
  `idPermiso` int NOT NULL AUTO_INCREMENT,
  `nombrePermiso` varchar(100) DEFAULT NULL,
  `claveForm` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`idPermiso`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permiso`
--

LOCK TABLES `permiso` WRITE;
/*!40000 ALTER TABLE `permiso` DISABLE KEYS */;
INSERT INTO `permiso` VALUES (1,'Productos','FormProductos'),(2,'Ventas','FormVentas'),(3,'Clientes','FormClientes'),(4,'Usuarios','FormUsuarios'),(5,'Reportes','FormReportePorFechas'),(6,'AdminMode','FormAdminMode');
/*!40000 ALTER TABLE `permiso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `producto`
--

DROP TABLE IF EXISTS `producto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `producto` (
  `idProducto` bigint NOT NULL AUTO_INCREMENT,
  `nombre` varchar(150) DEFAULT NULL,
  `precioProducto` decimal(10,2) DEFAULT NULL,
  `stock` int DEFAULT NULL,
  `descripcionProducto` text,
  `fechaCaducidad` date DEFAULT NULL,
  PRIMARY KEY (`idProducto`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `producto`
--

LOCK TABLES `producto` WRITE;
/*!40000 ALTER TABLE `producto` DISABLE KEYS */;
INSERT INTO `producto` VALUES (1,'Producto Demo 1',50.00,95,'Producto prueba',NULL),(2,'iboprofen',34.00,0,'si','2026-05-14');
/*!40000 ALTER TABLE `producto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rol`
--

DROP TABLE IF EXISTS `rol`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rol` (
  `idRol` int NOT NULL AUTO_INCREMENT,
  `nombreRol` varchar(50) NOT NULL,
  PRIMARY KEY (`idRol`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rol`
--

LOCK TABLES `rol` WRITE;
/*!40000 ALTER TABLE `rol` DISABLE KEYS */;
INSERT INTO `rol` VALUES (1,'Administrador'),(2,'Empleado');
/*!40000 ALTER TABLE `rol` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usersession`
--

DROP TABLE IF EXISTS `usersession`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usersession` (
  `idSession` int NOT NULL AUTO_INCREMENT,
  `fkUsuario` bigint DEFAULT NULL,
  `loginTime` datetime DEFAULT NULL,
  `logoutTime` datetime DEFAULT NULL,
  PRIMARY KEY (`idSession`),
  KEY `fkUsuario` (`fkUsuario`),
  CONSTRAINT `usersession_ibfk_1` FOREIGN KEY (`fkUsuario`) REFERENCES `usuario` (`idUsuario`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usersession`
--

LOCK TABLES `usersession` WRITE;
/*!40000 ALTER TABLE `usersession` DISABLE KEYS */;
INSERT INTO `usersession` VALUES (1,1,'2026-03-06 21:25:25','2026-03-06 21:25:39'),(2,1,'2026-03-06 21:34:24','2026-03-06 21:34:34'),(3,1,'2026-03-06 21:34:45','2026-03-06 21:34:46'),(4,1,'2026-03-06 21:38:59','2026-03-06 21:39:06'),(5,1,'2026-03-06 21:39:17','2026-03-06 21:39:28'),(6,1,'2026-03-06 21:43:17','2026-03-06 21:43:38'),(7,1,'2026-03-06 21:46:05','2026-03-06 21:46:12'),(8,1,'2026-03-06 21:48:45','2026-03-07 10:22:48'),(9,1,'2026-03-07 10:22:45','2026-03-07 10:22:48'),(10,1,'2026-03-07 10:23:27','2026-03-07 10:23:31'),(11,1,'2026-03-07 10:28:29','2026-03-07 10:28:48'),(12,1,'2026-03-07 10:29:33','2026-03-07 10:30:33'),(13,1,'2026-03-07 10:33:08','2026-03-07 10:56:42'),(14,1,'2026-03-07 10:54:42','2026-03-07 10:55:42'),(15,1,'2026-03-07 17:27:53','2026-03-07 17:28:54'),(16,1,'2026-03-07 17:38:00','2026-03-07 17:39:00'),(17,1,'2026-03-07 17:41:04','2026-03-07 17:42:04'),(18,1,'2026-03-07 17:43:39','2026-03-07 17:44:39'),(19,1,'2026-03-07 17:50:02','2026-03-07 17:56:49'),(20,1,'2026-03-07 17:53:10','2026-03-07 17:54:10'),(21,1,'2026-03-07 17:54:49','2026-03-07 17:55:49'),(22,1,'2026-03-07 18:23:42','2026-03-07 18:23:54'),(23,1,'2026-03-07 18:40:33','2026-03-07 18:44:12'),(24,1,'2026-03-07 18:42:12','2026-03-07 18:43:12'),(25,1,'2026-03-07 18:48:53','2026-03-07 18:49:53'),(26,1,'2026-03-08 17:15:25','2026-03-08 17:22:01'),(27,1,'2026-03-08 17:20:01','2026-03-08 17:21:01'),(28,1,'2026-03-08 17:23:03','2026-03-08 17:24:01'),(29,1,'2026-03-08 17:26:53','2026-03-08 17:27:01'),(30,1,'2026-03-08 17:28:19','2026-03-08 17:28:47'),(31,1,'2026-03-08 17:29:25','2026-03-08 17:30:01'),(32,1,'2026-03-08 17:43:43','2026-03-08 17:44:01'),(33,1,'2026-03-08 17:47:45','2026-03-08 17:48:01'),(34,1,'2026-03-08 17:50:57','2026-03-08 17:53:48'),(35,1,'2026-03-08 17:51:48','2026-03-08 17:52:48'),(36,1,'2026-03-09 18:39:56','2026-03-09 18:40:46');
/*!40000 ALTER TABLE `usersession` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `idUsuario` bigint NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) DEFAULT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `nombreUsuario` varchar(50) DEFAULT NULL,
  `password` varchar(100) DEFAULT NULL,
  `fkRol` int DEFAULT NULL,
  PRIMARY KEY (`idUsuario`),
  KEY `fkRol` (`fkRol`),
  CONSTRAINT `usuario_ibfk_1` FOREIGN KEY (`fkRol`) REFERENCES `rol` (`idRol`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
INSERT INTO `usuario` VALUES (1,'Admin','Sistema','admin','admin',1);
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario_permiso`
--

DROP TABLE IF EXISTS `usuario_permiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario_permiso` (
  `idUsuarioPermiso` int NOT NULL AUTO_INCREMENT,
  `fkUsuario` bigint DEFAULT NULL,
  `fkPermiso` int DEFAULT NULL,
  PRIMARY KEY (`idUsuarioPermiso`),
  KEY `fkUsuario` (`fkUsuario`),
  KEY `fkPermiso` (`fkPermiso`),
  CONSTRAINT `usuario_permiso_ibfk_1` FOREIGN KEY (`fkUsuario`) REFERENCES `usuario` (`idUsuario`),
  CONSTRAINT `usuario_permiso_ibfk_2` FOREIGN KEY (`fkPermiso`) REFERENCES `permiso` (`idPermiso`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario_permiso`
--

LOCK TABLES `usuario_permiso` WRITE;
/*!40000 ALTER TABLE `usuario_permiso` DISABLE KEYS */;
INSERT INTO `usuario_permiso` VALUES (1,1,1),(2,1,2),(3,1,3),(4,1,4),(5,1,5),(6,1,6);
/*!40000 ALTER TABLE `usuario_permiso` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-03-09 18:51:10
