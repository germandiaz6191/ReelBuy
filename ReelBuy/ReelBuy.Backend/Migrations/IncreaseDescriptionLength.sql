-- Aumentar el tamaño del campo Description en la tabla Products
ALTER TABLE Products 
ALTER COLUMN Description NVARCHAR(4000) NOT NULL; 