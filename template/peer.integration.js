document.app.peerjs = {
    serverId: "avallach_ovh_contracts_server",
    apiKey: "bw06fw5af8qwipb9",
    server: undefined,
    peerConnections: undefined,
    peer: undefined,
    serverConnection: undefined,
    
    setupServer: function() {
        document.app.log(`document.app.peerjs.setup(): server mode`);
        document.app.peerjs.server = new Peer(
            document.app.peerjs.serverId, {
                key: document.app.peerjs.apiKey,
                debug: document.app.debug ? 3 : 0
            }
        );
        window.addEventListener('beforeunload', function(event) {
            document.app.peerjs.server.disconnect();
        }, false);
        document.app.peerjs.peerConnections = [];
        document.app.peerjs.server.on('connection', function (peerConnection) {
            document.app.log(`document.app.peerjs.server.onconnection(${peerConnection.peer})`);
            document.app.peerjs.peerConnections.push(peerConnection);
            peerConnection.on('open', function () {
                document.app.log(`document.app.peerjs.peerConnections[${peerConnection.peer}].open`);
                document.app.log(`document.app.peerjs.peerConnections[${peerConnection.peer}].send(${document.app.switcher.activeSlideId})`);
                peerConnection.send(document.app.switcher.activeSlideId);
            });
        });

        document.app.switcher.onswitch.push(function(id) {
            document.app.log(`document.app.switcher.onswitch(document.app.peerjs, ${id})`);
            document.app.peerjs.peerConnections.forEach(function (connection) {
                document.app.log(`document.app.peerjs.peerConnections[${connection.peer}].send(${document.app.switcher.activeSlideId})`);
                connection.send(id);
            });
        });
    },
    
    setupPeer: function() {
        document.app.log(`document.app.peerjs.setup(): peer mode`);
        document.app.peerjs.peer = new Peer({
            key: document.app.peerjs.apiKey,
            debug: document.app.debug ? 3 : 0
        });
        document.app.peerjs.peer.on('open', function (id) {
            document.app.log(`document.app.peerjs.peer.open(${id})`);
            document.app.peerjs.serverConnection = document.app.peerjs.peer.connect(document.app.peerjs.serverId, { reliable: true });
            document.app.log(`document.app.peerjs.serverConnection.peer = ${document.app.peerjs.serverConnection.peer}`);
            document.app.log(`document.app.peerjs.serverConnection.open = ${document.app.peerjs.serverConnection.open}`);
            document.app.peerjs.serverConnection.on('data', function (data) {
                document.app.log(`document.app.peerjs.serverConnection.ondata(${data})`);
                document.app.switcher.setSlide(data);
            });
            document.app.peerjs.serverConnection.on('open', function () {
                document.app.log(`document.app.peerjs.serverConnection.onopen()`);
            });
        });
    },
    
    setup: function() {
        var isServer = document.location.href.indexOf("server") >= 0 || document.app.parameters.indexOf("presenter-mode") >= 0;
        document.app.log(`document.app.peerjs.setup(${isServer ? "server" : "peer"})`);
        if (isServer) {
            document.app.peerjs.setupServer();
        } else {
            document.app.peerjs.setupPeer();
        }
    }
}

window.addEventListener("load", function() { document.app.peerjs.setup(); });
