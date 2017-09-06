document.app = {
    debug: false,
    parameters: [],
    
    log: function(message) {
        if (document.app.debug)
            console.log(message);
    },
    
    createAgenda: function() {
        [].forEach.call(document.getElementsByClassName("agenda"), function (agenda) {
            function getTitle(slideIndex) {
                var slide = document.app.slides[slideIndex];
                if (slide.dataset.title != undefined && !slide.dataset.title.endsWith(" cont."))
                    return (slideIndex + 1) + ". " + slide.dataset.title;
                else {
                    return undefined;
                }
            }
            var list = document.createElement("ol");
            agenda.appendChild(list);
            [].forEach.call(document.app.slides, function (slide, index) {
                if (slide.classList.contains("agenda") ||
                    slide.classList.contains("title") ||
                    slide.childElementCount == 0)
                    return;
                var title = getTitle(index)
                if (title == undefined)
                    return;
                var entry = document.createElement("a");
                entry.href = "javascript:document.app.slides[" + index + "].scrollIntoView(true)";
                entry.textContent = title;
                list.appendChild(entry);
            });
        });
    },

    initCodeHighlighting: function() {
        hljs.configure({ languages: [ "csharp "] });
        if (document.location.host.startsWith("127.0.0.1"))
            return;
        var codes = document.getElementsByTagName("code");
        for (i = 0; i < codes.length; i++) {
            codes[i].classList.add("language-csharp");
            hljs.highlightBlock(codes[i]);
            codes[i].classList.remove("hljs");
        }
    },

    addTotalNumberOfSlidesToCounter: function() {
        var style = document.createElement('style');
        style.textContent = `section::after { content: counter(slide) "/${document.app.slides.length}"; }`;
        document.head.appendChild(style);
    },

    addFootnotes: function() {
        document.app.slides.forEach(function(slide) {
            var footnotes = document.createElement("footer");
            footnotes.classList.add("footnotes");
            Array.prototype.slice.call(slide.querySelectorAll("*[data-footnote]")).forEach(function(hyperlink) {
                var footnote = document.createElement("a");
                footnote.href = hyperlink.href;
                footnote.textContent = hyperlink.dataset.footnote;
                footnotes.appendChild(footnote);
            });
            slide.appendChild(footnotes);
        });
    },

    loadParameters: function() {
        document.location.search.substr(1).split("&").forEach(function(param) {
            if (param.length > 0)
                document.app.parameters.push(param);
        });
    },
    
    exposeParametersToStyles: function() {
        document.app.parameters.forEach(function(param) {
            document.body.classList.add(param);
        });
    },

    setupDebugging: function() {
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
        onswitch: function (id) {},
        
        activeSlideId: 0,

        isScrollingBack: function(event) {
            var backScrollingKeys = ["ArrowUp", "ArrowLeft", "PageUp"];
            return (event instanceof WheelEvent && event.deltaY < 0) ||
                (event instanceof KeyboardEvent && backScrollingKeys.indexOf(event.key) >= 0) ||
                (event instanceof TouchEvent &&
                    (document.app.switcher.touchStart.y - event.changedTouches[0].clientY > 0 ||
                        document.app.switcher.touchStart.x - event.changedTouches[0].clientX < 0));
        },

        setSlide: function(id) {
            document.app.switcher.activeSlideId = id;
            document.app.slides[id].scrollIntoView(true);
            document.app.switcher.onswitch(id);
        },

        onSlideChange: function(event) {
            document.app.log(`document.app.switcher.onSlideChange(${event.type})`);
            var newSlide = document.app.switcher.activeSlideId + (document.app.switcher.isScrollingBack(event) ? -1 : 1);
            if (newSlide >= document.app.slides.length)
                newSlide = 0;
            else if (newSlide < 0)
                newSlide = document.app.slides.length - 1;
            document.app.switcher.setSlide(newSlide);
        },

        init: function() {
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

    init: function() {
        document.app.slides = Array.prototype.slice.call(document.getElementsByTagName("section"));
        document.app.createAgenda();
        document.app.initCodeHighlighting();
        document.app.addTotalNumberOfSlidesToCounter();
        document.app.addFootnotes();
        document.app.loadParameters();
        document.app.exposeParametersToStyles();
        document.app.setupDebugging();
        document.app.switcher.init();
    }
}

window.addEventListener("load", document.app.init);