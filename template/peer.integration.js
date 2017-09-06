document.app.peerjs = {};
document.app.peerjs.setup = function() {
    console.log(`document.app.peerjs.setup()`);
    document.app.peerjs.serverId = "avallach_ovh_contracts_server";
    document.app.peerjs.apiKey = "bw06fw5af8qwipb9";
    
    if (document.location.href.indexOf("server") >= 0) {
        console.log(`setupPeerJs: server mode`);
        document.app.peerjs.server = new Peer(
            document.app.peerjs.serverId, {
                key: document.app.peerjs.apiKey,
                debug: 3
            }
        );
        document.app.peerjs.peerConnections = [];
        document.app.peerjs.server.on('connection', function (peerConnection) {
            console.log(`document.app.peerjs.server.onconnection(${peerConnection.peer})`);
            document.app.peerjs.peerConnections.push(peerConnection);
            peerConnection.on('open', function () {
                console.log(`document.app.peerjs.peerConnections[${peerConnection.peer}].open`);
                console.log(`document.app.peerjs.peerConnections[${peerConnection.peer}].send(${document.app.activeSlideId})`);
                peerConnection.send(document.app.activeSlideId);
            });
            peerConnection.on('data', function (data) {
                console.log(`document.app.peerjs.peerConnection.ondata(${data})`);
                document.app.setSlide(data);
            });
            peerConnection.on('error', function (err) {
                console.log(`document.app.peerjs.peerConnection.onerror(${err})`);
            });
            peerConnection.on('close', function () {
                console.log(`document.app.peerjs.peerConnection.onclose()`);
            });
        });
        document.app.peerjs.server.on('close', function () {
            console.log(`document.app.peerjs.server.onclose()`);
        });
        document.app.peerjs.server.on('disconnected', function () {
            console.log(`document.app.peerjs.server.ondisconnected()`);
        });
        document.app.peerjs.server.on('error', function (err) {
            console.log(`document.app.peerjs.server.onerror(${err})`);
        });
        document.app.peerjs.server.on('open', function () {
            console.log(`document.app.peerjs.server.onopen()`);
        });

        document.app.onswitch = function(id) {
            console.log(`document.app.onswitch(${id})`);
            document.app.peerjs.peerConnections.forEach(function (connection) {
                console.log(`document.app.peerjs.peerConnections[${connection.peer}].send(${document.app.activeSlideId})`);
                connection.send(id);
            });
        };
    } else {
        console.log(`setupPeerJs: peer mode`);
        document.app.peerjs.peer = new Peer({
            key: document.app.peerjs.apiKey,
            debug: 3
        });
        document.app.peerjs.peer.on('open', function (id) {
            console.log(`document.app.peerjs.peer.onopen(${id})`);
            document.app.peerjs.serverConnection = document.app.peerjs.peer.connect(document.app.peerjs.serverId, { reliable: true });
            console.log(`document.app.peerjs.serverConnection.peer = ${document.app.peerjs.serverConnection.peer}`);
            console.log(`document.app.peerjs.serverConnection.open = ${document.app.peerjs.serverConnection.open}`);
            document.app.peerjs.serverConnection.on('data', function (data) {
                console.log(`document.app.peerjs.serverConnection.ondata(${data})`);
                document.app.setSlide(data);
            });
            document.app.peerjs.serverConnection.on('error', function (err) {
                console.log(`document.app.peerjs.serverConnection.onerror(${err})`);
            });
            document.app.peerjs.serverConnection.on('open', function () {
                console.log(`document.app.peerjs.serverConnection.onopen()`);
            });
            document.app.peerjs.serverConnection.on('close', function () {
                console.log(`document.app.peerjs.serverConnection.onclose()`);
            });
        });
        document.app.peerjs.peer.on('close', function () {
            console.log(`document.app.peerjs.peer.onclose()`);
        });
        document.app.peerjs.peer.on('disconnected', function () {
            console.log(`document.app.peerjs.peer.ondisconnected()`);
        });
        document.app.peerjs.peer.on('error', function (err) {
            console.log(`document.app.peerjs.peer.onerror(${err})`);
        });
        document.app.peerjs.peer.on('connection', function () {
            console.log(`document.app.peerjs.peer.onconnection()`);
        });
    }
}

window.addEventListener("load", function() { document.app.peerjs.setup(); });
