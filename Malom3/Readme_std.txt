Malom, a Nine Men's Morris (and variants) player and solver program.
Copyright (C) 2007-2018  Gabor E. Gevay, Gabor Danner

See our webpage (and the paper linked from there):
http://compalg.inf.elte.hu/~ggevay/mills/index.php

For license information, see the “License (gpl-3.0).txt” file.


*** Note: Although we have included this Readme file with the binaries, it mainly concerns the databases. ***


This is the database of the ultra-strong solution of the standard
variant, and an executable file (_Malom.exe) which plays using the
database. No installation is required, you can just run the program
directly. It expects the database files to be in the working directory
(where the exe is).

You can read about the theory behind the program in our paper linked
from our webpage. You may also want to check for newer versions of the
program there.

0. System requirements
-64-bit Windows
-.Net framework 4.5
-3 GB memory
-77.8 GB free space (unzipped)

1. Rules

We used the same rules as described here:
http://www.flyordie.com/games/help/mill/en/contents.html or in our
paper.

2. Playing with the program
The default mode of the program is perfect play, which means that with
the ultra-strong database it never makes a mistake (always achieves at
least the game-theoretical value of the position), and also tries to
get into positions which are hard for the opponent (the latter is the
“ultra-” part, which you can turn off from the Settings menu with the
checkbox titled “Ignore draw distinguishing info in the databases”)

The game-theoretic value of standard Nine Men’s Morris is draw. This
means that the program will always achieve at least a draw, and if you
make mistakes, then it may win.

The program prints the evaluation of the current position in the
bottom right corner (you can turn this off from the Settings).
“NTESC” roughly means, that the position is deemed equal. What it
actually means is that with perfect play (with regard to distinguished
draws), the game will end with a draw where the players have an equal
number of stones (that is, in a non-transient ESC subspace in the
terminology of our paper). If it prints “L”, you are lost (the second
number in the parenthesis shows how many moves you can delay the loss
at best). If it prints “W”, you are winning. In other cases it prints
things like “-59 (std_8_9_0_0)”. The four numbers identify the
subspace in which the game will be drawn by perfect play: you will
have 8 stones on the board, the computer will have 9, and none of you
will have more stones to place. The first number (-59) is the value of
the subspace. The center of the range of this value is 0, and a
negative number means that you are in a bad situation. This number
won’t increase, since the program is playing perfectly. (If you make a
mistake, then it decreases, otherwise it stays the same.) For example,
if the evaluation is 3_6_0_0, then you are very close to losing. The
number after the “NGM:“ shows the number of optimal moves in the
current position.

If the bottom right corner shows "No database file", then either
- The database is not found. You should check that the directory
  of the exe file contains the .sec2 files for the appropriate
  variant.
- The solution database doesn't contain an entry for the current
  position. This can happen when you use the "Set Up Position" feature
  to set up such a position that cannot be reached from the starting
  position of the game.

If you activate the “Advisor” from the menu bar, the program will also
print the values of all possible moves, and mark optimal moves yellow.
(If there are multiple possible moves (slide and/or placement) to a
free point, then it prints the value of the best of them.)

In the players menu you can set the types of the players: “Human”
means that the user controls the player from the user interface,
“Heuristic” refers to the heuristic engine (this only works for the
standard variant), which uses alpha-beta pruning (that is, looking
only a few moves ahead), and “Perfect” uses the database. “Combined”
uses both: the heuristic player chooses between moves deemed equally
good by the databases. This makes it even harder for the opponent to
not make a mistake, and achieve the game-theoretic value.