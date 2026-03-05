from flask import Flask, request, jsonify
from datetime import datetime, timezone
import random
import logging

app = Flask(__name__)

class TemperatureData:
    def __init__(self, value, unit, timestamp, location, status, sensor_id, sensor_type, description):
        self.value = value
        self.unit = unit
        self.timestamp = timestamp
        self.location = location
        self.status = status
        self.sensor_id = sensor_id
        self.sensor_type = sensor_type
        self.description = description
    
    def to_dict(self):
        return {
            "value": self.value,
            "unit": self.unit,
            "timestamp": self.timestamp.isoformat(),
            "location": self.location,
            "status": self.status,
            "sensor_id": self.sensor_id,
            "sensor_type": self.sensor_type,
            "description": self.description
        }

def generate_temperature_data(location, sensor_id):
    # Generate a random temperature between 18 and 28 degrees Celsius
    value = 18.0 + random.uniform(0, 10)
    
    # If no location is provided, use a default based on sensor ID
    if not location:
        location_map = {
            "1": "Living Room",
            "2": "Bedroom", 
            "3": "Kitchen"
        }
        location = location_map.get(sensor_id, "Unknown")
    
    # If no sensor ID is provided, generate one based on location
    if not sensor_id:
        sensor_map = {
            "Living Room": "1",
            "Bedroom": "2", 
            "Kitchen": "3"
        }
        sensor_id = sensor_map.get(location, "0")
    
    return TemperatureData(
        value=round(value, 2),
        unit="°C",
        timestamp=datetime.now(timezone.utc),
        location=location,
        status="active",
        sensor_id=sensor_id,
        sensor_type="temperature",
        description=f"Temperature sensor in {location}"
    )

@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({"status": "ok"})

@app.route('/temperature', methods=['GET'])
def get_temperature_by_location():
    location = request.args.get('location')
    if not location:
        return jsonify({"error": "Location is required"}), 400
    
    data = generate_temperature_data(location, "")
    return jsonify(data.to_dict())

@app.route('/temperature/<sensor_id>', methods=['GET'])
def get_temperature_by_sensor(sensor_id):
    if not sensor_id:
        return jsonify({"error": "Sensor ID is required"}), 400
    
    data = generate_temperature_data("", sensor_id)
    return jsonify(data.to_dict())

if __name__ == '__main__':
    logging.basicConfig(level=logging.INFO)
    logging.info("Temperature API starting on :8081")
    app.run(host='0.0.0.0', port=8081, debug=True)