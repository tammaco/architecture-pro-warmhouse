from flask import Flask, jsonify, request
import random
import time
from datetime import datetime, timezone

app = Flask(__name__)

telemetry_store = {}
device_last_update = {}

METRICS = ['temperature', 'brightness', 'battery']
UNITS = {
    'temperature': '°C',
    'brightness': '%',
    'battery': '%'
}

def generate_telemetry(device_id, metric='temperature'):
    if metric:
        metrics_to_generate = [metric]
    else:
        metrics_to_generate = METRICS
    
    telemetry = []
    timestamp = datetime.now(timezone.utc).isoformat()
    
    for m in metrics_to_generate:
        if m == 'temperature':
            value = round(random.uniform(18.0, 28.0), 1)
        elif m == 'brightness':
            value = round(random.uniform(30.0, 70.0), 1)
        elif m == 'battery':
            value = random.randint(10, 100)
        else:
            value = round(random.uniform(0, 100), 1)
        
        telemetry.append({
            'sensor_id': device_id,
            'metric': m,
            'value': value,
            'unit': UNITS.get(m, ''),
            'timestamp': timestamp, 
            'status': 'active'
        })
    
    return telemetry

@app.route('/api/v2/telemetry/<int:sensor_id>', methods=['GET'])
def get_telemetry(sensor_id):
    metric = "temperature"
    
    # Generate telemetry data
    telemetry_data = generate_telemetry(sensor_id)
    telemetry_store[sensor_id] = telemetry_data
    
    # Find the temperature data point
    temperature_data = next((item for item in telemetry_data if item['metric'] == 'temperature'), None)
    
    if temperature_data:
        return jsonify({
            'sensor_id': sensor_id,
            'unit': temperature_data['unit'],
            'value': temperature_data['value'],
            'sensor_type': 'temperature',
            'status': temperature_data['status'],
            'timestamp': temperature_data['timestamp'],
            'description': 'description'
        })
    else:
        return jsonify({
            'error': 'Temperature data not found'
        }), 404

@app.route('/api/v2/telemetry/health', methods=['GET'])
def health_check():
    return jsonify({
        'status': 'healthy'
    })

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5001, debug=True)