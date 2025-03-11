window.initializeMap = (options) => {
    return L.map('map', {
        center: options.center,
        zoom: options.zoom,
        zoomControl: true
    });
};

window.addTileLayer = (map, url) => {
    L.tileLayer(url, {
        maxZoom: 18,
        id: 'mapbox/streets-v11',
        tileSize: 512,
        zoomOffset: -1
    }).addTo(map);
};

window.addMarker = (map, coordinates, name) => {
    L.marker(coordinates).addTo(map).bindPopup(name);
};

window.addGeoJsonLayer = (map, geoJsonData) => {
    L.geoJSON(JSON.parse(geoJsonData)).addTo(map);
};

window.zoomIn = (map) => {
    map.zoomIn();
};

window.zoomOut = (map) => {
    map.zoomOut();
};
