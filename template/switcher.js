document.app = {};
document.app.onswitch = function (id) {};

document.app.isScrollingBack = function(event) {
    var backScrollingKeys = ["ArrowUp", "ArrowLeft", "PageUp"];
    return (event instanceof WheelEvent && event.deltaY < 0) ||
        (event instanceof KeyboardEvent && backScrollingKeys.indexOf(event.key) >= 0) ||
        (event instanceof TouchEvent &&
            (document.app.touchStart.y - event.changedTouches[0].clientY > 0 ||
                document.app.touchStart.x - event.changedTouches[0].clientX < 0));
}

document.app.setSlide = function(id) {
    document.app.activeSlideId = id;
    document.app.slides[id].scrollIntoView(true);
    document.app.onswitch(id);
}

document.app.onSlideChange = function(event) {
    var newSlide = document.app.activeSlideId + (document.app.isScrollingBack(event) ? -1 : 1);
    if (newSlide >= document.app.slides.length)
        newSlide = 0;
    else if (newSlide < 0)
        newSlide = document.app.slides.length - 1;
    document.app.setSlide(newSlide);
}

document.app.setupDebugging = function() {
    if (document.location.href.indexOf("debug") >= 0) {
        var consoleView = document.createElement("div");
        consoleView.id = "consoleView";
        consoleView.style.position = "fixed";
        consoleView.style.bottom = "0";
        consoleView.style.left = "0";
        consoleView.style.padding = "2em";
        consoleView.style.font = "14px monospace";
        consoleView.style.color = "black";
        consoleView.style.background = "rgba(255,255,255,0.8)";
        document.body.appendChild(consoleView);
        var oldLog = console.log;
        console.log = function () {
            consoleView.innerHTML += Array.prototype.slice.call(arguments).join(" ") + "<br/>";
            oldLog.apply(console, arguments);
        };
    }
}

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
        document.app.peerjs.connections = [];
        document.app.peerjs.server.on('connection', function (connection) {
            console.log(`document.app.peerjs.server.connection(${connection.peer})`);
            document.app.peerjs.connections.push(connection);
            connection.on('open', function () {
                console.log(`document.app.peerjs.connections[${connection.peer}].open`);
                console.log(`document.app.peerjs.connections[${connection.peer}].send(${document.app.activeSlideId})`);
                connection.send(document.app.activeSlideId);
            });
        });

        document.app.onswitch = function(id) {
            console.log(`document.app.onswitch(${id})`);
            document.app.peerjs.connections.forEach(function (connection) {
                console.log(`document.app.peerjs.connections[${connection.peer}].send(${document.app.activeSlideId})`);
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
            console.log(`document.app.peerjs.peer.open(${id})`);
            document.app.peerjs.peer
                .connect(document.app.peerjs.serverId)
                .on('data', function (data) {
                    console.log(`document.app.peerjs.peer.data(${data})`);
                    document.app.setSlide(data);
                });
        });
    }
}

window.addEventListener("load", function () {
    document.app.slides = document.getElementsByTagName("section");
    document.app.activeSlideId = 0;
    document.body.addEventListener("click", document.app.onSlideChange);
    document.body.addEventListener("wheel", document.app.onSlideChange);
    document.body.addEventListener("keydown", document.app.onSlideChange);
    document.body.addEventListener("touchstart", function (event) {
        document.app.touchStart = {
            x: event.touches[0].clientX,
            y: event.touches[0].clientY
        }
    });
    document.body.addEventListener("touchend", document.app.onSlideChange);
    document.app.setupDebugging();
    document.app.peerjs.setup();
});
