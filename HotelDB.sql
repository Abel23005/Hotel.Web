-- Esquema de practica creado a partir de la guia; no es el script oficial del docente.
IF DB_ID(N'HotelDB') IS NULL CREATE DATABASE HotelDB;
GO
USE HotelDB;
GO
CREATE TABLE TipoHabitacion (
 IdTipoHabitacion INT IDENTITY PRIMARY KEY,
 Nombre NVARCHAR(60) NOT NULL
);
CREATE TABLE Habitacion (
 IdHabitacion INT IDENTITY PRIMARY KEY,
 Numero NVARCHAR(10) NOT NULL UNIQUE,
 IdTipoHabitacion INT NOT NULL REFERENCES TipoHabitacion(IdTipoHabitacion),
 PrecioPorNoche DECIMAL(10,2) NOT NULL CHECK (PrecioPorNoche > 0),
 Activo BIT NOT NULL DEFAULT 1
);
CREATE TABLE Huesped (
 IdHuesped INT IDENTITY PRIMARY KEY,
 Nombres NVARCHAR(100) NOT NULL,
 Documento NVARCHAR(20) NOT NULL,
 Activo BIT NOT NULL DEFAULT 1
);
CREATE TABLE Reserva (
 IdReserva INT IDENTITY PRIMARY KEY,
 IdHuesped INT NOT NULL REFERENCES Huesped(IdHuesped),
 IdHabitacion INT NOT NULL REFERENCES Habitacion(IdHabitacion),
 FechaIngreso DATE NOT NULL,
 FechaSalida DATE NOT NULL,
 Estado NVARCHAR(20) NOT NULL DEFAULT N'Pendiente',
 CHECK (FechaSalida > FechaIngreso),
 CHECK (Estado IN (N'Pendiente',N'Confirmada',N'Cancelada',N'Finalizada'))
);
INSERT TipoHabitacion(Nombre) VALUES (N'Simple'),(N'Doble'),(N'Suite');
INSERT Habitacion(Numero,IdTipoHabitacion,PrecioPorNoche) VALUES
 (N'101',1,100),(N'102',1,100),(N'201',2,180),(N'202',2,180),(N'301',3,300);
INSERT Huesped(Nombres,Documento) VALUES (N'Ana Ejemplo',N'DEMO001'),(N'Luis Ejemplo',N'DEMO002');
GO
