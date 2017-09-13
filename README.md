# NET-Code-Contracts-Presentation
This is presentation about .NET Code Contracts I prepared for one IT conference. 

# Implementation details
It's handwritten in HTML+CSS+JS and offers some nice features:
 - displays well on desktop and mobile, horizontally and vertically
 - auto generates agenda and footnotes
 - highlights code (with highlight.js)
 - has dark (default) and light (add ?light to url) theme (GUI switch can be added later...) (it also reverts illustration colors where needed)
 - has slide counter in the corner
 - presenter mode ( add ?presenter-mode to url)
   - slide is scrolled by exactly one on any input (some change it one slide back, the rest forward) (touch is supported) (scrollbars are hidden)
   - when presenter changes slide, it's automatically scrolled also on all running clients with P2P WebRTC (with peer.js)
   - "smart" clock around slide counter (shows "time-dimension" progress and changes color if you're changing slides too slow (blue) or too fast (red))
   - "less important" pieces of slides and footnotes are hidden in this mode, but visible in default audience mode
