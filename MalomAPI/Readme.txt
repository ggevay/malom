Using MalomAPI.dll

To use the MalomAPI.dll, the database files and Wrappers.dll should be in the current working directory. When building Malom (instead of using downloaded binaries), you can find Wrappers.dll under the x64 directory.

MalomAPI.dll is a 64-bit DLL.

MalomAPI.dll provides the MalomSolutionAccess class. There is no need to instantiate the class, because all methods are static.

In the following description, “white” denotes the starting player, and “black” denotes the player that moves second.

Functions:

GetBestMove(whiteBitboard As Integer, blackBitboard As Integer, whiteStonesToPlace As Integer, blackStonesToPlace As Integer, playerToMove As Integer, onlyStoneTaking As Boolean) As Integer

This function receives the current game state, and returns one of the optimal moves randomly.

Parameters:
- whiteBitboard: The white stones on the board, encoded as a bitboard: Each of the first 24 bits corresponds to one place on the board. For the mapping between bits, see Bitboard.png. For example, the integer number 131 means that there is a vertical mill on the left side of the board, because 131 = 1 + 2 + 128. 
- blackBitboard: The black stones on the board.
- whiteStonesToPlace: The number of stones the white player can still place on the board.
- blackStonesToPlace: The number of stones the black player can still place on the board.
- playerToMove: 0 if white is to move, 1 if black is to move.
- onlyStoneTaking: Always set this to false if you want to handle mill-closing and stone-removal as a single move. If you set it to true, it is assumed that a mill was just closed and only the stone to be removed is returned.

Return value:
The move is returned as a bitboard, which has a bit set for each change on the board:
- If the place corresponding to a set bit is empty, then a stone of the player to move appears there.
- If the place corresponding to a set bit currently has a stone, then that stone disappears. (If it’s a stone of the opponent, then this move involves a stone-removal. If it’s a stone of the player to move, then this is a sliding or jumping move, and that stone is being slided or jumped to a different place.)
If this increases the number of stones the player to move has, then that player will have one less stone to place after the move.

This function throws an exception if the arguments are illegal, or if the game has already ended.

GetBestMoveNoException is a similar function that is useful if you have trouble handling .NET exceptions (e.g., due to calling from a non-.NET language). It has the same parameters as GetBestMove, but it never throws an exception. Its return value is almost the same as that of GetBestMove, but it returns 0 if there is an error. In this case, you can call GetLastError, which will return the error as a .NET String.

(None of our functions are thread-safe. This means that your program should not call our functions from different threads.)

Our library can consume around 1-2 GB of memory.
