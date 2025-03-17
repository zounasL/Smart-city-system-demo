//Define animation options.
const loopLength = 600; // unit corresponds to the timestamp in source data
let animationSpeed = 1;
let time = 0;
var deckOverlay;

class DeckGLOverlay {
    constructor(options) {
        this.id = options.id;
        this._mbOverlay = new deck.MapboxOverlay(options);
    }

    onAdd(map, options) {
        this.map = map;
        return this._mbOverlay.onAdd(map["map"]);
    }

    onRemove() {
        this._mbOverlay.onRemove();
    }

    getCanvas() {
        this._mbOverlay.getCanvas();
    }

    getId() {
        return this.id;
    }

    pickObject(params) {
        return this._mbOverlay.pickObject(params);
    }

    pickMultipleObjects(params) {
        return this._mbOverlay.pickMultipleObjects(params);
    }

    pickObjects(params) {
        return this._mbOverlay.pickObjects();
    }

    setProps(props) {
        this._mbOverlay.setProps(props);
    }

    finalize() {
        this._mbOverlay.finalize();
    }
}

var map = new atlas.Map("map", {
    center: [25.747357, 62.242635], // Jyväskylä, Finland
    zoom: 14,
    pitch: 45,
    style: "grayscale_dark",
    antialias: true,
    authOptions: {
        authType: 'subscriptionKey',
        clientId: 'AZURE_MAP_CLIENT_ID',
        subscriptionKey: 'AZURE_MAP_KEY'
    }
});

// Wait until the map resources are ready
map.events.add("ready", function () {

    //Add the overlay to the map.
    deckOverlay = new DeckGLOverlay({
        layers: []
    });

    // Add the overlay to the map
    map.controls.add(deckOverlay);
    // Add controls
    map.controls.add(
        [
            new atlas.control.ZoomControl(),
            new atlas.control.PitchControl(),
            new atlas.control.CompassControl(),
            new atlas.control.StyleControl({
                mapStyles: "all"
            })
        ],
        {
            position: "top-right"
        }
    );
});


function onMapLayerChange(layerName) {
    if (layerName) {
        if (layerName.includes(".geojson")) {
            //Create a data source and add it to the map.
            var datasource = new atlas.source.DataSource();
            map.sources.add(datasource);

            //Load a dataset of points, in this case earthquake data from the USGS.
            datasource.importDataFromUrl(`/api/layer/heatmap/${layerName}`);

            //Create a heat map and add it to the map.
            map.layers.add(new atlas.layer.HeatMapLayer(datasource, layerName, {
                radius: 10,
                opacity: 0.8
            }));
        } else {
            //Initialize the trip layer options.
            let tripLayerOptions = {
                id: "trips",
                data: `/api/layer/traffic/${layerName}`,
                getPath: (d) => d.path,
                getTimestamps: (d) => d.timestamps,
                getColor: (d) => [50, 230, 50],
                opacity: 0.3,
                widthMinPixels: 2,
                trailLength: 180,
                currentTime: time,
                shadowEnabled: false
            };

            function animate() {
                time = (time + animationSpeed) % loopLength;
                tripLayerOptions.currentTime = time;
                deckOverlay.setProps({
                    layers: [
                        new deck.TripsLayer(tripLayerOptions)
                    ]
                });

                window.requestAnimationFrame(animate);
            };
            animate();
        }
    }
}