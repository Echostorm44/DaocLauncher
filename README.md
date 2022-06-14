This is a game launcher for the MMO Dark Age of Camelot

I'm writing this because existing launchers I've seen are either buggy or are using libs I can't decompile to kill the mutex handles and it makes me nervous.  Also it's better to have everything in C#.

Currently the launcher can rename game instance windows and kill the mutex the game uses to prevent playing more than 2 accounts at once.

Plans are to add hotkey management, window size and screen management and anything else that seems fun.  Maybe some packet reading to do stats.
