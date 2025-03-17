#!/usr/bin/env python
from geojson import MultiPoint
import json
import os
import sys
import optparse

if 'SUMO_HOME' in os.environ:
    tools = os.path.join(os.environ['SUMO_HOME'], 'tools')
    sys.path.append(tools)
else:
    sys.exit("please declare environment variable 'SUMO_HOME'")
   
from sumolib import checkBinary
import traci

def get_options():
    opt_parser = optparse.OptionParser()
    opt_parser.add_option("--nogui", action="store_true",
                         default=False, help="run the commandline version of sumo")
    options, args = opt_parser.parse_args()
    return options

# TraCI control loop
def run():
    step = 0
    vehicles_routes = {}
    while traci.simulation.getMinExpectedNumber() > 0:
        traci.simulationStep()
        for vehID in traci.vehicle.getIDList():
            x, y = traci.vehicle.getPosition(vehID)
            lon, lat = traci.simulation.convertGeo(x, y)
            if vehID in vehicles_routes:
                vehicles_routes[vehID]["path"].append([lon, lat])
                vehicles_routes[vehID]["timestamps"].append(step)
            else:
                vehicles_routes[vehID] = {"vendor": vehID,"path": [[lon, lat]], "timestamps": [step]}
            
        with open('data.json', 'w') as convert_file:
            convert_file.write(json.dumps(list(vehicles_routes.values())))
        step += 1

    traci.close()
    sys.stdout.flush()

if __name__ == "__main__":
    options = get_options()

    if options.nogui:
        sumoBinary = checkBinary('sumo')
    else:
        sumoBinary = checkBinary('sumo-gui')

    traci.start([sumoBinary, "-c", "./sumo-simulation/osm.sumocfg"])
    run()