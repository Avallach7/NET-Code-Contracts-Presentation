document.app = {
    debug: false,
    parameters: [],

    log: function (message) {
        if (document.app.debug)
            console.log(message);
    },

    createAgenda: function () {
        [].forEach.call(document.getElementsByClassName("agenda"), function (agenda) {
            function getTitle(slideIndex) {
                var slide = document.app.slides[slideIndex];
                if (slide.dataset.title != undefined &&
                    !slide.dataset.title.endsWith(" cont.") &&
                    !(slideIndex > 0 && slide.dataset.title == document.app.slides[slideIndex - 1].dataset.title))
                    return (slideIndex + 1) + ". " + slide.dataset.title;
                else {
                    return undefined;
                }
            }
            var list = document.createElement("ol");
            agenda.appendChild(list);
            var indexOfAgenda = document.app.slides.indexOf(agenda);
            [].forEach.call(document.app.slides, function (slide, index) {
                if (index <= indexOfAgenda ||
                    slide.classList.contains("title") ||
                    slide.childElementCount == 0)
                    return;
                var title = getTitle(index)
                if (title == undefined)
                    return;
                var entry = document.createElement("a");
                entry.href = "javascript:document.app.switcher.setSlide(" + index + ")";
                entry.textContent = title;
                list.appendChild(entry);
            });
        });
    },

    initCodeHighlighting: function () {
        hljs.configure({
            languages: ["csharp "]
        });
        if (document.location.host.startsWith("127.0.0.1"))
            return;
        var codes = document.getElementsByTagName("code");
        for (i = 0; i < codes.length; i++) {
            codes[i].classList.add("language-csharp");
            hljs.highlightBlock(codes[i]);
            codes[i].classList.remove("hljs");
        }
    },

    addTotalNumberOfSlidesToCounter: function () {
        var style = document.createElement('style');
        style.textContent = `section::after { content: counter(slide) "/${document.app.slides.length}"; }`;
        document.head.appendChild(style);
    },


    switchToDarkStyle: function () {
        if (document.app.parameters.indexOf("light") < 0) {
            Array.prototype.slice.call(document.getElementsByTagName("img")).forEach(function (img) {
                if (!img.classList.contains("preserve-colors"))
                    img.style.filter = "invert(100%)";
            });
        }
    },

    addFootnotes: function () {
        document.app.slides.forEach(function (slide) {
            var footnotes = document.createElement("footer");
            footnotes.classList.add("footnotes");
            Array.prototype.slice.call(slide.querySelectorAll("*[data-footnote]")).forEach(function (hyperlink) {
                var footnote = document.createElement("a");
                footnote.href = hyperlink.href;
                footnote.textContent = hyperlink.dataset.footnote;
                footnotes.appendChild(footnote);
            });
            if (footnotes.childElementCount > 0)
                slide.appendChild(footnotes);
        });
    },

    loadParameters: function () {
        document.location.search.substr(1).split("&").forEach(function (param) {
            if (param.length > 0) {
                document.app.parameters.push(param);
                document.body.classList.add(param);
            }
        });
    },

    setupDebugging: function () {
        if (document.app.parameters.indexOf("debug") >= 0) {
            document.app.debug = true;
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
    },

    switcher: {
        onswitch: [],

        activeSlideId: 0,

        isScrollingBack: function (event) {
            var backScrollingKeys = ["ArrowUp", "ArrowLeft", "PageUp"];
            return (event instanceof WheelEvent && event.deltaY < 0) ||
                (event instanceof KeyboardEvent && backScrollingKeys.indexOf(event.key) >= 0) ||
                (event instanceof TouchEvent &&
                    (event.changedTouches[0].clientY - document.app.switcher.touchStart.y > 0 ||
                        document.app.switcher.touchStart.x - event.changedTouches[0].clientX < 0));
        },

        setSlide: function (id) {
            document.app.switcher.activeSlideId = id;
            document.app.slides[id].scrollIntoView(true);
            document.app.switcher.onswitch.forEach(function (handler) {
                handler(id);
            });
        },

        onSlideChange: function (event) {
            document.app.log(`document.app.switcher.onSlideChange(${event.type})`);
            var newSlide = document.app.switcher.activeSlideId + (document.app.switcher.isScrollingBack(event) ? -1 : 1);
            if (newSlide >= document.app.slides.length)
                newSlide = 0;
            else if (newSlide < 0)
                newSlide = document.app.slides.length - 1;
            document.app.switcher.setSlide(newSlide);
        },

        init: function () {
            document.body.addEventListener("click", document.app.switcher.onSlideChange);
            document.body.addEventListener("wheel", document.app.switcher.onSlideChange);
            document.body.addEventListener("keydown", document.app.switcher.onSlideChange);
            document.body.addEventListener("touchstart", function (event) {
                document.app.switcher.touchStart = {
                    x: event.touches[0].clientX,
                    y: event.touches[0].clientY
                }
            });
            document.body.addEventListener("touchend", document.app.switcher.onSlideChange);
        }
    },

    progressbar: undefined,

    timeLengthMinutes: 30,
    slideLengths: [0, 55, 45, 50, 60, 60, 55, 105, 35, 40, 195, 50, 75, 90, 50, 65, 65, 65, 85, 170, 85, 75, 70, 50, 95, 75, 75],
    startTime: undefined,

    initProgressbar: function () {
        document.app.slides.forEach(function (slide) {
            // var svgNS = "http://www.w3.org/2000/svg";
            // var progressbar = document.createElementNS(svgNS, "svg");
            // progressbar.classList.add("progressbar");
            // progressbar.setAttributeNS(svgNS, "viewBox", "0 0 100 100");
            // var path = document.createElementNS(svgNS, "path");
            // progressbar.appendChild(path);
            // slide.appendChild(progressbar);
            // ABOVE DOES NOT RENDER IN CHROME, UGLY HACK INSTEAD 
            slide.innerHTML += '<svg class="progressbar" viewBox="0 0 100 100"><path></path></svg>';
        });
        document.app.startTime = new Date().getTime();
        document.app.updateProgressbar = function () {
            var minutesElapsed = (new Date().getTime() - document.app.startTime) / 1000 / 60;
            var progress = minutesElapsed / document.app.timeLengthMinutes;
            var timeProgress = 0;
            var slidesThasShouldHaveBeenPassed = 0;
            document.app.slideLengths.forEach(function (l) {
                if (minutesElapsed * 60 > timeProgress) {
                    timeProgress += l;
                    slidesThasShouldHaveBeenPassed++;
                }
            });
            progress = slidesThasShouldHaveBeenPassed / document.app.slides.length;
            if (progress >= 1) {
                progress = 1;
                clearInterval(loop);
            }
            document.body.style.setProperty("--progress", progress);

            var slideCount = document.app.slides.length;
            var slideProgress = document.app.switcher.activeSlideId / slideCount;
            var progressColor;
            if (slideProgress < progress - (1 / slideCount))
                progressColor = "blue";
            else if (slideProgress > progress + (1 / slideCount))
                progressColor = "red";
            else
                progressColor = "rgba(128,128,128, 0.2)";
            document.body.style.setProperty("--progress-color", progressColor);
        }
        var loop = setInterval(document.app.updateProgressbar, 60 * 1000);
        document.app.switcher.onswitch.push(document.app.updateProgressbar);
    },

    startPresentationMode: function () {
        if (document.app.parameters.indexOf("presenter-mode") < 0)
            return;
        document.app.switcher.init();
        document.app.initProgressbar();
        document.body.classList.add("hideIrrelevant");
    },

    init: function () {
        document.app.slides = Array.prototype.slice.call(document.getElementsByTagName("section"));
        document.app.createAgenda();
        document.app.initCodeHighlighting();
        document.app.addTotalNumberOfSlidesToCounter();
        document.app.addFootnotes();
        document.app.loadParameters();
        document.app.setupDebugging();
        document.app.switchToDarkStyle();
        document.app.startPresentationMode();
    }
}

window.addEventListener("load", document.app.init);
