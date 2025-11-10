-- Crear BD
IF DB_ID('MiticaxDB') IS NULL
BEGIN
    CREATE DATABASE MiticaxDB;
END
GO
USE MiticaxDB;
GO

-- Limpieza segura (solo si re-ejecutas el script durante desarrollo)
IF OBJECT_ID('dbo.Ronda','U') IS NOT NULL DROP TABLE dbo.Ronda;
IF OBJECT_ID('dbo.Batalla','U') IS NOT NULL DROP TABLE dbo.Batalla;
IF OBJECT_ID('dbo.Equipo','U') IS NOT NULL DROP TABLE dbo.Equipo;
IF OBJECT_ID('dbo.InventarioJugador','U') IS NOT NULL DROP TABLE dbo.InventarioJugador;
IF OBJECT_ID('dbo.Jugador','U') IS NOT NULL DROP TABLE dbo.Jugador;
IF OBJECT_ID('dbo.Criatura','U') IS NOT NULL DROP TABLE dbo.Criatura;
GO

-- Tabla: Criatura (catalogo)
CREATE TABLE dbo.Criatura
(
    IdCriatura     INT        NOT NULL PRIMARY KEY,
    Nombre         VARCHAR(60) NOT NULL,
    Tipo           VARCHAR(10) NOT NULL,             -- agua | tierra | aire | fuego
    Nivel          INT         NOT NULL,             -- 1..5 (Iniciado..Maestro)
    Poder          INT         NOT NULL CHECK (Poder >= 0),
    Resistencia    INT         NOT NULL CHECK (Resistencia >= 0),
    Costo          INT         NOT NULL CHECK (Costo >= 0),
    CONSTRAINT CK_Criatura_Tipo CHECK (Tipo IN ('agua','tierra','aire','fuego')),
    CONSTRAINT CK_Criatura_Nivel CHECK (Nivel BETWEEN 1 AND 5),
    -- Validaciones de costo por nivel (reglas de rango)
    CONSTRAINT CK_Criatura_CostoNivel CHECK (
        (Nivel = 1 AND Costo < 100) OR
        (Nivel = 2 AND Costo >= 100 AND Costo < 300) OR
        (Nivel = 3 AND Costo >= 300 AND Costo < 600) OR
        (Nivel = 4 AND Costo >= 600 AND Costo < 1200) OR
        (Nivel = 5 AND Costo >= 1200)
    )
);
GO

-- Tabla: Jugador
CREATE TABLE dbo.Jugador
(
    IdJugador        INT         NOT NULL PRIMARY KEY,
    Nombre           VARCHAR(60) NOT NULL,
    FechaNacimiento  DATE        NOT NULL,
    Nivel            INT         NOT NULL,        -- 1 Novato, 2 Estudiante, 3 Maestro, 4 Supremo (segun victorias)
    Cristales        INT         NOT NULL DEFAULT 100 CHECK (Cristales >= 0),
    BatallasGanadas  INT         NOT NULL DEFAULT 0 CHECK (BatallasGanadas >= 0),
    CONSTRAINT CK_Jugador_Nivel CHECK (Nivel BETWEEN 1 AND 4)
);
GO

-- Tabla: Inventario del jugador (criaturas compradas)
CREATE TABLE dbo.InventarioJugador
(
    IdInventario   INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdJugador      INT NOT NULL,
    IdCriatura     INT NOT NULL,
    Poder          INT NOT NULL CHECK (Poder >= 0),       -- copia inicial + evoluciones por rondas
    Resistencia    INT NOT NULL CHECK (Resistencia >= 0), -- copia inicial - perdidas por batallas
    CONSTRAINT UQ_Inventario_JugadorCriatura UNIQUE (IdJugador, IdCriatura),
    CONSTRAINT FK_Inv_Jugador  FOREIGN KEY (IdJugador)  REFERENCES dbo.Jugador (IdJugador),
    CONSTRAINT FK_Inv_Criatura FOREIGN KEY (IdCriatura) REFERENCES dbo.Criatura (IdCriatura)
);
GO

-- Tabla: Equipo (3 criaturas por equipo de un jugador)
CREATE TABLE dbo.Equipo
(
    IdEquipo     INT NOT NULL PRIMARY KEY,
    IdJugador    INT NOT NULL,
    IdCriatura1  INT NOT NULL,
    IdCriatura2  INT NOT NULL,
    IdCriatura3  INT NOT NULL,
    CONSTRAINT FK_Equipo_Jugador FOREIGN KEY (IdJugador) REFERENCES dbo.Jugador (IdJugador),
    -- Deben existir en inventario del mismo jugador (se valida en logica/servidor)
    CONSTRAINT CK_Equipo_NoRepetidas CHECK (IdCriatura1 <> IdCriatura2 AND IdCriatura1 <> IdCriatura3 AND IdCriatura2 <> IdCriatura3)
);
GO

-- Tabla: Batalla
CREATE TABLE dbo.Batalla
(
    IdBatalla   INT NOT NULL PRIMARY KEY,
    IdJugador1  INT NOT NULL,
    IdEquipo1   INT NOT NULL,
    IdJugador2  INT NOT NULL,
    IdEquipo2   INT NOT NULL,
    Ganador     INT NULL,         -- IdJugador del ganador (se asigna al final)
    Fecha       DATE NOT NULL DEFAULT CAST(GETDATE() AS DATE),
    CONSTRAINT FK_Batalla_J1  FOREIGN KEY (IdJugador1) REFERENCES dbo.Jugador (IdJugador),
    CONSTRAINT FK_Batalla_J2  FOREIGN KEY (IdJugador2) REFERENCES dbo.Jugador (IdJugador),
    CONSTRAINT FK_Batalla_E1  FOREIGN KEY (IdEquipo1)  REFERENCES dbo.Equipo  (IdEquipo),
    CONSTRAINT FK_Batalla_E2  FOREIGN KEY (IdEquipo2)  REFERENCES dbo.Equipo  (IdEquipo),
    CONSTRAINT CK_Batalla_JugadoresDistintos CHECK (IdJugador1 <> IdJugador2),
    CONSTRAINT CK_Batalla_EquiposDistintos   CHECK (IdEquipo1 <> IdEquipo2)
);
GO

-- Tabla: Ronda (1..3 por batalla)
CREATE TABLE dbo.Ronda
(
    IdRonda       INT NOT NULL,      -- 1..3
    IdBatalla     INT NOT NULL,
    IdJugador1    INT NOT NULL,
    IdCriatura1   INT NOT NULL,
    IdJugador2    INT NOT NULL,
    IdCriatura2   INT NOT NULL,
    GanadorRonda  INT NOT NULL,      -- IdJugador
    PRIMARY KEY (IdBatalla, IdRonda),
    CONSTRAINT FK_Ronda_Batalla FOREIGN KEY (IdBatalla) REFERENCES dbo.Batalla (IdBatalla),
    CONSTRAINT CK_Ronda_Numero CHECK (IdRonda BETWEEN 1 AND 3)
);
GO

-- Semillas minimas opcionales (para pruebas rapidas)
INSERT INTO dbo.Criatura (IdCriatura,Nombre,Tipo,Nivel,Poder,Resistencia,Costo) VALUES
(1,'Flammar','fuego', 3, 40, 25, 400),
(2,'Aquarys','agua',  2, 22, 30, 150),
(3,'Terron','tierra',4, 65, 60, 800),
(4,'Aeron','aire',   1, 12, 10,  50);
GO
