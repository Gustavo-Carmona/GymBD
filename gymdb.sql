-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Versión del servidor:         11.2.0-MariaDB - mariadb.org binary distribution
-- SO del servidor:              Win64
-- HeidiSQL Versión:             12.3.0.6589
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Volcando estructura de base de datos para gymdb
CREATE DATABASE IF NOT EXISTS `gymdb` /*!40100 DEFAULT CHARACTER SET latin1 COLLATE latin1_swedish_ci */;
USE `gymdb`;

-- Volcando estructura para tabla gymdb.administrador
CREATE TABLE IF NOT EXISTS `administrador` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(50) DEFAULT NULL,
  `Contrasena` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.administrador: ~4 rows (aproximadamente)
INSERT INTO `administrador` (`id`, `Nombre`, `Contrasena`) VALUES
	(1, 'Gustavo Carmona', 'Gus123'),
	(2, 'Josue Padilla', 'Wen2413'),
	(3, 'Victor Ayon', 'Vic789'),
	(5, 'Alonso De la torre', 'dltq123');

-- Volcando estructura para tabla gymdb.alimento
CREATE TABLE IF NOT EXISTS `alimento` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) NOT NULL,
  `descripcion` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.alimento: ~4 rows (aproximadamente)
INSERT INTO `alimento` (`id`, `nombre`, `descripcion`) VALUES
	(1, 'Pollo', 'Fuente de proteína magra'),
	(2, 'Arroz integral', 'Carbohidrato de bajo índice glucémico'),
	(3, 'Aguacate', 'Grasa saludable rica en ácidos grasos monoinsaturados'),
	(4, 'Manzana', 'Saludable');

-- Volcando estructura para tabla gymdb.area
CREATE TABLE IF NOT EXISTS `area` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) DEFAULT NULL,
  `descripcion` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.area: ~3 rows (aproximadamente)
INSERT INTO `area` (`id`, `nombre`, `descripcion`) VALUES
	(1, 'Cardio', 'Área con máquinas de cardio como cintas y bicicletas'),
	(2, 'Fuerza', 'Área con pesas y máquinas para entrenamiento de fuerza'),
	(4, 'Tren Inferior', 'Area para trabajar el tren inferior del cuerpo');

-- Volcando estructura para tabla gymdb.cliente
CREATE TABLE IF NOT EXISTS `cliente` (
  `id` int(11) NOT NULL,
  `fechaRegistro` date DEFAULT NULL,
  `id_plan` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id_plan` (`id_plan`),
  CONSTRAINT `cliente_ibfk_1` FOREIGN KEY (`id`) REFERENCES `persona` (`id`),
  CONSTRAINT `cliente_ibfk_2` FOREIGN KEY (`id_plan`) REFERENCES `plan` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.cliente: ~2 rows (aproximadamente)
INSERT INTO `cliente` (`id`, `fechaRegistro`, `id_plan`) VALUES
	(102, '2024-11-17', 3),
	(103, '2024-11-01', 2);

-- Volcando estructura para tabla gymdb.cliente_promocion
CREATE TABLE IF NOT EXISTS `cliente_promocion` (
  `id_cliente` int(11) NOT NULL,
  `id_promocion` int(11) NOT NULL,
  PRIMARY KEY (`id_cliente`,`id_promocion`),
  KEY `idx_cliente_promocion_cliente` (`id_cliente`),
  KEY `idx_cliente_promocion_promocion` (`id_promocion`),
  CONSTRAINT `cliente_promocion_ibfk_1` FOREIGN KEY (`id_cliente`) REFERENCES `cliente` (`id`),
  CONSTRAINT `cliente_promocion_ibfk_2` FOREIGN KEY (`id_promocion`) REFERENCES `promocion` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.cliente_promocion: ~0 rows (aproximadamente)

-- Volcando estructura para tabla gymdb.dieta
CREATE TABLE IF NOT EXISTS `dieta` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_cliente` int(11) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `fechaAsignacion` date NOT NULL,
  `descripcion` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id_cliente` (`id_cliente`),
  CONSTRAINT `dieta_ibfk_1` FOREIGN KEY (`id_cliente`) REFERENCES `cliente` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.dieta: ~4 rows (aproximadamente)
INSERT INTO `dieta` (`id`, `id_cliente`, `nombre`, `fechaAsignacion`, `descripcion`) VALUES
	(1, 102, 'Dieta alta en proteínas', '2024-01-02', 'Dieta enfocada en el aumento de masa muscular'),
	(2, 102, 'Dieta Balanceada', '2024-02-01', 'Dieta equilibrada para mantener peso y energía'),
	(3, 102, 'Dieta para pérdida de grasa', '2024-03-11', 'Dieta baja en carbohidratos para reducir porcentaje de grasa'),
	(6, 102, 'Dieta de Volumen', '2024-11-01', 'Dieta alta en calorias para aumentar volumen');

-- Volcando estructura para tabla gymdb.dieta_alimento
CREATE TABLE IF NOT EXISTS `dieta_alimento` (
  `id_dieta` int(11) NOT NULL,
  `id_alimento` int(11) NOT NULL,
  `cantidad` decimal(5,2) NOT NULL,
  PRIMARY KEY (`id_dieta`,`id_alimento`),
  KEY `idx_dieta_alimento_dieta` (`id_dieta`),
  KEY `idx_dieta_alimento_alimento` (`id_alimento`),
  CONSTRAINT `dieta_alimento_ibfk_1` FOREIGN KEY (`id_dieta`) REFERENCES `dieta` (`id`),
  CONSTRAINT `dieta_alimento_ibfk_2` FOREIGN KEY (`id_alimento`) REFERENCES `alimento` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.dieta_alimento: ~6 rows (aproximadamente)
INSERT INTO `dieta_alimento` (`id_dieta`, `id_alimento`, `cantidad`) VALUES
	(1, 1, 200.00),
	(1, 2, 150.00),
	(2, 1, 150.00),
	(2, 3, 100.00),
	(3, 2, 100.00),
	(3, 3, 80.00);

-- Volcando estructura para tabla gymdb.empleado
CREATE TABLE IF NOT EXISTS `empleado` (
  `id` int(11) NOT NULL,
  `puesto` varchar(50) DEFAULT NULL,
  `salario` decimal(10,2) DEFAULT NULL,
  `fechaContratacion` date DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_empleado_puesto` (`puesto`),
  KEY `idx_empleado_salario` (`salario`),
  CONSTRAINT `empleado_ibfk_1` FOREIGN KEY (`id`) REFERENCES `persona` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.empleado: ~4 rows (aproximadamente)
INSERT INTO `empleado` (`id`, `puesto`, `salario`, `fechaContratacion`) VALUES
	(1, 'Entrenador', 1500.00, '2020-06-01'),
	(2, 'Recepcionista', 1200.00, '2022-03-15'),
	(3, 'Gerente', 2500.00, '2019-09-20'),
	(5, 'conserje', 1700.00, '2024-11-17');

-- Volcando estructura para tabla gymdb.fichamedica
CREATE TABLE IF NOT EXISTS `fichamedica` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_cliente` int(11) DEFAULT NULL,
  `peso` decimal(5,2) DEFAULT NULL,
  `talla` decimal(5,2) DEFAULT NULL,
  `porcentajeGrasaCorporal` decimal(5,2) DEFAULT NULL,
  `fechaRegistro` date DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id_cliente` (`id_cliente`),
  CONSTRAINT `fichamedica_ibfk_1` FOREIGN KEY (`id_cliente`) REFERENCES `cliente` (`id`),
  CONSTRAINT `chk_peso` CHECK (`peso` > 0),
  CONSTRAINT `chk_talla` CHECK (`talla` > 0),
  CONSTRAINT `chk_grasa` CHECK (`porcentajeGrasaCorporal` >= 0 and `porcentajeGrasaCorporal` <= 100)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.fichamedica: ~2 rows (aproximadamente)
INSERT INTO `fichamedica` (`id`, `id_cliente`, `peso`, `talla`, `porcentajeGrasaCorporal`, `fechaRegistro`) VALUES
	(5, 102, 80.90, 1.90, 18.90, '2024-11-13'),
	(6, 103, 78.20, 1.84, 17.20, '2024-11-15');

-- Volcando estructura para tabla gymdb.mantenimiento
CREATE TABLE IF NOT EXISTS `mantenimiento` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `id_maquina` int(11) DEFAULT NULL,
  `descripcion` varchar(100) DEFAULT NULL,
  `fecha` date DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id_maquina` (`id_maquina`),
  CONSTRAINT `mantenimiento_ibfk_1` FOREIGN KEY (`id_maquina`) REFERENCES `maquina` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.mantenimiento: ~3 rows (aproximadamente)
INSERT INTO `mantenimiento` (`id`, `id_maquina`, `descripcion`, `fecha`) VALUES
	(1, 1, 'Revisión general', '2024-05-10'),
	(2, 2, 'Cambio de correas', '2024-04-05'),
	(3, 4, 'Mantenimiento preventivo', '2024-02-20');

-- Volcando estructura para tabla gymdb.maquina
CREATE TABLE IF NOT EXISTS `maquina` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) DEFAULT NULL,
  `id_area` int(11) DEFAULT NULL,
  `fechaAdquisicion` date DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id_area` (`id_area`),
  CONSTRAINT `maquina_ibfk_1` FOREIGN KEY (`id_area`) REFERENCES `area` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.maquina: ~4 rows (aproximadamente)
INSERT INTO `maquina` (`id`, `nombre`, `id_area`, `fechaAdquisicion`) VALUES
	(1, 'Cinta de correr', 1, '2023-11-15'),
	(2, 'Bicicleta estática', 1, '2023-10-10'),
	(3, 'Máquina de press de banca', 2, '2022-05-22'),
	(4, 'Máquina de remo', 2, '2021-08-13');

-- Volcando estructura para tabla gymdb.persona
CREATE TABLE IF NOT EXISTS `persona` (
  `id` int(11) NOT NULL,
  `Nombre` varchar(50) DEFAULT NULL,
  `Apellido` varchar(50) DEFAULT NULL,
  `FechaNacimiento` date DEFAULT NULL,
  `Direccion` varchar(100) DEFAULT NULL,
  `Telefono` varchar(15) DEFAULT NULL,
  `Email` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.persona: ~6 rows (aproximadamente)
INSERT INTO `persona` (`id`, `Nombre`, `Apellido`, `FechaNacimiento`, `Direccion`, `Telefono`, `Email`) VALUES
	(1, 'Carlos', 'López', '1990-05-10', 'Calle Ficticia 123', '555-1234567', 'carlos.lopez@email.com'),
	(2, 'María', 'Maldonado', '1985-08-22', 'Av. Libertad 456', '555-2345678', 'maria.perez@email.com'),
	(3, 'Luis', 'Martínez', '1992-12-01', 'Calle Luna 789', '555-3456789', 'luis.martinez@email.com'),
	(5, 'Josue', 'hernandez', '2024-11-17', 'lomas 234', '324234', 'jh@gmail.com'),
	(102, 'Bryan', 'Alanis', '2024-11-17', 'independencia', '3453452', 'aabl@gmail.com'),
	(103, 'Alberto', 'Fernandez', '2000-12-31', 'periferico', '4564323', 'alf@gmail.com');

-- Volcando estructura para tabla gymdb.plan
CREATE TABLE IF NOT EXISTS `plan` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) DEFAULT NULL,
  `costo` decimal(10,2) DEFAULT NULL,
  `duracionMeses` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.plan: ~3 rows (aproximadamente)
INSERT INTO `plan` (`id`, `nombre`, `costo`, `duracionMeses`) VALUES
	(1, 'Plan Básico', 30.00, 1),
	(2, 'Plan Avanzado', 50.00, 3),
	(3, 'Plan Premium', 80.00, 6);

-- Volcando estructura para tabla gymdb.promocion
CREATE TABLE IF NOT EXISTS `promocion` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) DEFAULT NULL,
  `descuento` decimal(5,2) DEFAULT NULL,
  `fechaInicio` date DEFAULT NULL,
  `fechaFin` date DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla gymdb.promocion: ~4 rows (aproximadamente)
INSERT INTO `promocion` (`id`, `descripcion`, `descuento`, `fechaInicio`, `fechaFin`) VALUES
	(1, 'Descuento por primer mes', 15.00, '2024-01-01', '2024-01-31'),
	(2, 'Descuento de verano', 20.00, '2024-06-01', '2024-06-30'),
	(3, 'Promoción por referir amigos', 10.00, '2024-03-01', '2024-03-31'),
	(4, 'Descuento de Estudiante', 50.00, '2024-01-01', '2024-12-31');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
