# Build Docker
docker build -t blazorstatic .

# Run Docker
docker run -p 8081:80 blazorstatic