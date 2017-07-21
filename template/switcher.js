document.app = {};

function setCurrentSlideVisibility(visible) {
    if (visible)
        document.app.slides[document.app.activeSlideId].scrollIntoView(true);
    console.log(`slide ${document.app.activeSlideId+1} shown`);
}

function changeSlide() {
    setCurrentSlideVisibility(false);
    document.app.activeSlideId++;
    if (document.app.activeSlideId >= document.app.slides.length)
        document.app.activeSlideId = 0;
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
