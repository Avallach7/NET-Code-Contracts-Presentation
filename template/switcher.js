document.app = {};

function setCurrentSlideVisibility(visible) {
    if (visible)
        document.app.slides[document.app.activeSlideId].scrollIntoView(true);
    console.log(`slide ${document.app.activeSlideId+1} shown`);
}

function isScrollingBack(event) {
    var backScrollingKeys = ["ArrowUp", "ArrowLeft", "PageUp"];
    return (event instanceof WheelEvent && event.deltaY < 0) ||
           (event instanceof KeyboardEvent && backScrollingKeys.indexOf(event.key) >= 0);
}

function changeSlide(event) {
    setCurrentSlideVisibility(false);
    document.app.activeSlideId += isScrollingBack(event) ? -1 : 1;
    if (document.app.activeSlideId >= document.app.slides.length)
        document.app.activeSlideId = 0;
    else if (document.app.activeSlideId < 0)
        document.app.activeSlideId = document.app.slides.length-1;
    setCurrentSlideVisibility(true);
}

window.addEventListener("load", function() {
    document.app.slides = document.getElementsByTagName("section");
    document.app.activeSlideId = 0;
    setCurrentSlideVisibility(true);
    document.body.addEventListener("click", changeSlide);
    document.body.addEventListener("wheel", changeSlide);
    document.body.addEventListener("keydown", changeSlide);
});
