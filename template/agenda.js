window.addEventListener("load", function () {
    var slides = document.getElementsByTagName("section");
    [].forEach.call(document.getElementsByClassName("agenda"), function (agenda) {
        function getTitle(slideIndex) {
            var slide = slides[slideIndex];
            if (slide.dataset.title != undefined && !slide.dataset.title.endsWith(" cont."))
                return (slideIndex + 1) + ". " + slide.dataset.title;
            else {
                return undefined;
            }
        }
        var list = document.createElement("ol");
        agenda.appendChild(list);
        [].forEach.call(slides, function (slide, index) {
            if (slide.classList.contains("agenda") ||
                slide.childElementCount == 0)
                return;
            var title = getTitle(index)
            if (title == undefined)
                return;
            var entry = document.createElement("li");
            entry.textContent = title;
            list.appendChild(entry);
        });
    });
});