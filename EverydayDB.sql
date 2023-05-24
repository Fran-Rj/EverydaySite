
create database EverydayDB

-- USUARIOS
create table Usuario(
idUser int primary key identity(1,1) not null,
photo varchar(max),
nameUser varchar(300) not null,
gender varchar(50) not null,
email varchar(300) unique not null,
keyUser varchar(300) unique not null,
roleUser varchar(25) default 'Usuario' not null,
createdAt datetime default GETDATE() not null,
state varchar(15) not null
);


-- MARCAS
create table Marca(
idMarc int primary key identity(1,1) not null,
nameMarc varchar(300) not null,
createdAt datetime default GETDATE() not null,
);

insert into Marca (nameMarc) values ('Hermès');

select * from Marca

-- CATEGORIAS
create table Categoria(
idCateg int primary key identity(1,1) not null,
nameCateg varchar(300) not null,
createdAt datetime default GETDATE() not null
);


insert into Categoria(nameCateg) values ('Camisa manga larga');

select * from Categoria


-- PRODUCTOS
create table Producto(
idProd int primary key identity(1,1) not null,
imagen image,
nameProd varchar(300),
description varchar(max),
quantity int not null,
price decimal(6,2) not null,
stock varchar(15) not null,
idMarc int foreign key references Marca not null,
idCateg int foreign key references Categoria not null,
createdAt datetime default GETDATE() not null
);


-- TIPOS - PRODUCTO
create table Tipo(
idType int primary key identity(1,1) not null,
nameType varchar(300),
idProd int foreign key references Producto not null,
createdAt datetime default GETDATE() not null
);


-- CLIENTES
create table Cliente(
idClient int primary key identity(1,1) not null,
nameClient varchar(300) not null,
lastnameClient varchar(300) not null,
nitClient int unique not null,
addressClient varchar(100) not null,
phone int unique not null,
idUser int foreign key references Usuario not null,
createdAt datetime default GETDATE() not null
);


-- TARJETAS
create table Tarjeta(
idTarjet int primary key identity(1,1) not null,
numTarjet int unique not null,
codeTarjet int unique not null,
cvv int unique not null,
nameTarjet varchar(300) not null,
saldo decimal(6,2) not null,
createdAt datetime default GETDATE() not null,
expireDate date not null,
state varchar(45) not null,
idUser int foreign key references Usuario not null
);


-- VENTA
create table Venta(
idVent int primary key identity(1,1) not null,
idClient int foreign key references Cliente not null,
total decimal(6,2) not null,
createdAt datetime default GETDATE() not null
);


-- DETALLES VENTA
create table DetallesVenta(
idDetail int primary key identity(1,1) not null,
idVent int foreign key references Venta not null,
iProd int foreign key references Producto not null,
price decimal(6,2),
quantity int not null,
subTotal decimal(6,2) not null,
createdAt datetime default GETDATE() not null
);


-- PAGOS
create table Pago(
idPay int primary key identity(1,1) not null,
idUser int foreign key references Usuario not null,
quantity int not null,
createdAt datetime default GETDATE() not null
);


select * from Usuario