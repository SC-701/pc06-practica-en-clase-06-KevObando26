CREATE   PROCEDURE ObtenerModelos
    @IdMarca UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        IdMarca,
        Nombre
    FROM Modelos
    WHERE (IdMarca = @IdMarca)
    
END