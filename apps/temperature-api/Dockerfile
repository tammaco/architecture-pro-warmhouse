# Build stage
FROM python:3.11-slim

# Set working directory
WORKDIR /app

# Install necessary runtime dependencies
RUN pip install --no-cache-dir flask

# Copy the binary from the builder stage
COPY main.py .

# Expose the port the app runs on
EXPOSE 8081

# Command to run the application
CMD ["python", "main.py"]