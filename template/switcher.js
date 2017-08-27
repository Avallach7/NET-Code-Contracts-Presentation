document.app = {};
document.app.onswitch = function (id) {};
document.app.peerjs = {};
document.app.peerjs.serverId = "avallach_ovh_contracts_server";
document.app.peerjs.apiKey = "lwjd5qra8257b9";

function isScrollingBack(event) {
    var backScrollingKeys = ["ArrowUp", "ArrowLeft", "PageUp"];
    return (event instanceof WheelEvent && event.deltaY < 0) ||
        (event instanceof KeyboardEvent && backScrollingKeys.indexOf(event.key) >= 0);
}

function setSlide(id) {
    //    console.log(`setSlide(${id}})`);
    document.app.activeSlideId = id;
    document.app.slides[id].scrollIntoView(true);
    document.app.onswitch(id);
    //    console.log(`document.app.activeSlideId = ${document.app.activeSlideId}`);
}

function changeSlide(event) {
    var newSlide = document.app.activeSlideId + (isScrollingBack(event) ? -1 : 1);
    //    console.log(`newSlide = ${newSlide}`);
    if (newSlide >= document.app.slides.length)
        newSlide = 0;
    else if (newSlide < 0)
        newSlide = document.app.slides.length - 1;
    setSlide(newSlide);
}

function setupPeerJs() {
    if (document.location.href.indexOf("server") >= 0) {
        var server = new Peer(
            document.app.peerjs.serverId, {
                key: document.app.peerjs.apiKey
            }
        );
        var connections = [];
        server.on('connection', function (connection) {
            connections.push(connection);
            connection.on('open', function () {
                connection.send(document.app.activeSlideId);
            });
        });

        document.app.onswitch = function () {
            connections.forEach(function (connection) {
                connection.send(document.app.activeSlideId);
            });
        };
    } else {
        peer = new Peer({
            key: document.app.peerjs.apiKey
        });
        peer.on('open', function (id) {
            peer
                .connect(document.app.peerjs.serverId)
                .on('data', function (data) {
                    setSlide(data);
                });
        });
    }
}

window.addEventListener("load", function () {
    document.app.slides = document.getElementsByTagName("section");
    document.app.activeSlideId = 0;
    document.body.addEventListener("click", changeSlide);
    document.body.addEventListener("wheel", changeSlide);
    document.body.addEventListener("keydown", changeSlide);
    setupPeerJs();
});
