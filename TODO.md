# TODO

* Fix allow rotten filter
    Doesn't work. I think the patch syntax is fine, maybe the individual stockpile types are overriding the defaults here.

    Yeah, looks like that's the case in ThingFilter.cs line 651, though there is no specific mention of the special filters


* Fix tainted apparel filter
    I think both of these have the same issue -- the built in stockpiles override the special filters in code (though I'm not sure exactly where yet). I think they are applying correctly to things like the stockpile filters on shelves, etc.


* Make odyssey quest extension xml patch use the multiplier setting. Clean up the multiplier setting label.


# MAYBE

* Makes the Smooth Surface designator available from the Structure menu (I keep looking for it there).
* Makes the stockpile for shelves empty by default (I'm tired of pawns delivering weapons to shelf 1 while I'm waiting for shelf 2 to finish building before setting both of their stockpiles to what I actually want).

* Add links of various QOL/Tweak mods
    * link mehni's misc modifications?

# FIXME
