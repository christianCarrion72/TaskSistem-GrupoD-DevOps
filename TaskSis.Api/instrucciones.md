cd TaskSis.Api

dotnet run

#Docker

Generar la imagen
docker build -t tasksis:latest .

Ejecutar el contenedor
docker run --rm -p 5255:5255 tasksis:latest

