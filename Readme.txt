Malom, a Nine Men's Morris (and variants) player and solver program.
Copyright (C) 2007-2023  Gabor E. Gevay, Gabor Danner

See our webpage:
http://compalg.inf.elte.hu/~ggevay/mills/index.php

For license information, see the “License (gpl-3.0).txt” file.

You can read about the theory behind the program in our paper linked
from our webpage.

This is a Visual Studio 2017 solution. If you have a newer Visual Studio,
then we recommend upgrading the solution and the projects when opening the
solution. The program works on 64-bit Windows.


0. Rules

We used the same rules as described here:
http://www.flyordie.com/games/help/mill/en/contents.html or in our
paper.


1. Overview of the projects

Malom_megoldas (C++): the solver program that produces solution (.sec
or .sec2 files) for a single work unit (one or two subspaces),
provided that the subspaces on which the current work unit directly
depends on are available. It also does (based on command line options)
the verification of single subspaces, and can calculate various
statistics.

Controller (C#): starts the solver for different subspaces
automatically, taking the dependencies between them into account.

Malom3 (VB.Net): the GUI which you can play a game with, using the
databases. (And it also contains a heuristic (alpha-beta) AI.)

Wrappers (C++/CLI): Produces a dll which is used by the Controller and
the GUI to access the databases, and to keep track of the dependencies
between them (termed sector_graph in the code).

Analyzer (C++): Aggregates data from the .analyze files produced by
the solver for individual subspaces. (This part is a mess, because we
were adding features quite randomly. If you would like to extract some
statistics that we didn't compute, then you should probably drop us an
email.) It also calculates the values of subspaces (.secval file) from
the strong solution's statistics, which is required for the
ultra-strong solution.

MalomAPI (VB.Net): Produces a dll that you can use to access the databases
from an other program. See the other Readme.txt in the MalomAPI directory for
more details.


2. common.h

You can set various macros and constants in Malom_megoldas/common.h.
For example, DD means Draw Distinguishing, i.e. whether we are
“ultra”. The const string movegen_file is also important. This file
contains lookup tables for the move generation. You should make sure
that it’s directory exists.

Before compiling and launching something, go through the following checklist:
[ ] Debug/Release mode compilation (Release mode is faster, but no
asserts, etc.)
[ ] Variant
[ ] DD/legacy (i.e. ultra-strong or strong)
[ ] FULL_BOARD_IS_DRAW
[ ] FULL GRAPH (extended solution)
[ ] Controller.Mode (If you are launching the controller): in
Controller.cs, you can set the controller mode. Keeping it on “solve”
is ok in most cases, because it automatically switches (when there is
at least one thread) to the next mode, if there is no work left in the
current mode.
[ ] Databases are created (or looked for) in the working directory.
[ ] If you are launching from Visual Studio, then you should check the
Working Directory setting (in the project properties, under
debugging), to make sure that it points to the databases.
[ ] If you are launching or compiling the Controller, set
Controller.ExeToStart: If you are launching the Controller from VS,
then the first option is needed; if you are launching it by just
clicking on the exe in the directory where the solver is, then the
second.
[ ] The directory specified by  const string movegen_file  in common.h
should exist.
[ ] The solver needs the “buckets” directory to exist in the working
directory and the Controller needs the “lockfiles” directory.
[ ] .secval file: If you are launching anything in DD mode, then you
need this in the current directory. It contains the values of the
sectors, and can be calculated by the Analyzer from the strong
solutions (or can be downloaded from our website).
[ ] Project: If you are launching from VS, you should specify the
StartUp project (right click on it in the Solution Explorer, Set as
StartUp Project)
[ ] Switches: If you are launching the Analyzer, then there are
multiple possible switches. (and also, if you are launching the solver
directly (not through the Controller))
[ ] If you are launching the Controller from VS in debug mode, then
make sure Controller.ExeToStart specifies the exe you want, and it is
built.

Note that which database is needed for perfect playing is dependent on
the following settings:
-variant
-DD/legacy (.sec2/.sec files)
-FULL_BOARD_IS_DRAW


3. Malom_megoldas

The main function is in Malom_megoldas.cpp. It examines the command
line arguments, initializes logging and some lookup tables, then calls
the solve, verify, or analyze functions. For the solution algorithm,
see our paper (linked from our website).


4. Controller

The Mode variable controls whether we are solving, verifying, or
analyzing. The program switches automatically between them in this
order, upon completion of one.

The first progress bar counts the work units (or sectors, if we are in
verification mode or analyze mode), whereas the second counts the game
states. You can set the number of solver instances manually, or let
the program do it automatically, based on free memory: it checks free
memory every minute, and increases or decreases the number of
instances, if there is too much (more than Inc) or too little (less
than dec) free memory. The max field should be set to at most the
available hardware parallelism, that your machine has (number of
physical CPU cores in the simplest case, but hyperthreading can double
this).

In the main part of the form you can see the outputs of the currently
running (and recently finished) solver instances (these are also
recorded in the log files).

If you want to launch the Controller, go through the checklist in 2.


5. Wrapper

This provides .net wrappers for some native C++ parts of the solution:
-the hash function
-reading from the database files
-sector_graph (the dependencies between the subspaces)
The Wrapper project includes many of the source files from the
Malom_megoldas (solver) project. However, there is a macro called
WRAPPER, on which lots of #ifdefs depend in several files. So, many of
the functionalities of the shared source files are actually different
in the two projects (for example, see sector.cpp).
The hash function is a little tricky (see Ralph Gasser’s PhD thesis
cited in our paper) and uses large lookup tables. These don’t fit in
memory for all the sectors at once, which is a problem when playing.
We decided to keep the lookup tables for the 8 most recently accessed
sector in memory (this is ~1 GB). The code for this is in
Wrappers.cpp.


6. User Interface

The default mode of the program is perfect play, which means that with
the ultra-strong database it never makes a mistake (always achieves at
least the game-theoretical value of the position), and also tries to
get into positions which are hard for the opponent (the latter is the
“ultra-” part, which you can turn off from the Settings menu with the
checkbox titled “Ignore draw distinguishing info in the databases”)

The program prints the evaluation of the current position in the
bottom right corner (you can turn this off from the Settings). “NTESC”
roughly means, that the position is deemed equal. What it actually
means is that with perfect play (with regard to distinguished draws),
the game will end with a draw where the players have an equal number
of stones (that is, in a non-transient ESC subspace in the terminology
of our paper). If it prints “L”, you are lost (the second number in
the parenthesis shows how many moves you can delay the loss at best).
If it prints “W”, you are winning. In other cases it prints things
like “-59 (std_8_9_0_0)”. The four numbers identify the subspace in
which the game will be drawn by perfect play: you will have 8 stones
on the board, the computer will have 9, and none of you will have more
stones to place. The first number (-59) is the value of the subspace.
The center of the range of this value is 0, and a negative number
means that you are in a bad situation. This number won’t increase,
since the program is playing perfectly. (If you make a mistake, then
it decreases, otherwise it stays the same.) For example, if the
evaluation is 3_6_0_0, then you are very close to losing. The number
after the “NGM:“ shows the number of optimal moves in the current
position.

If you activate the “Advisor” from the menu bar, the program will also
print the values of all possible moves, and mark optimal moves yellow.

In the players menu you can set the types of the players: “Human”
means that the user controls the player from the user interface,
“Computer” refers to the heuristic engine (this only works for the
standard variant), which uses alpha-beta pruning (that is, looking
only a few moves ahead), and “Perfect” uses the database. “Combined”
uses both: the heuristic player chooses between moves deemed equally
good by the databases. This makes it even harder for the opponent, to
not make a mistake, and achieve the game-theoretic value.


7. Running computation

Running the computations requires at least 4 GB RAM for the strong
solutions, and 8 GB for ultra-strong. (We used a 16 GB and a 20 GB
machine, which makes the computations run faster, because more
instances of the solver can be run in parallel.) (You can exit Visual
Studio to save RAM after launching the Controller, if you click Detach
All in the Debug menu.)

We used a few utility programs to monitor the computations: Process
Explorer, Resource Monitor, Core Temp.
