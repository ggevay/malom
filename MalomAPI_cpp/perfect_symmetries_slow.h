/*
Malom, a Nine Men's Morris (and variants) player and solver program.
Copyright(C) 2007-2016  Gabor E. Gevay, Gabor Danner
Copyright (C) 2023 The Sanmill developers (see AUTHORS file)

See our webpage (and the paper linked from there):
http://compalg.inf.elte.hu/~ggevay/mills/index.php


This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

#ifndef PERFECT_SYMMETRIES_SLOW_H_INCLUDED
#define PERFECT_SYMMETRIES_SLOW_H_INCLUDED

int id(int a);
int rot90(int a);
int rot180(int a);
int rot270(int a);
int tt_fuggoleges(int a);
int tt_vizszintes(int a);
int tt_bslash(int a);
int tt_slash(int a);
int swap(int a);
int swap_rot90(int a);
int swap_rot180(int a);
int swap_rot270(int a);
int swap_tt_fuggoleges(int a);
int swap_tt_vizszintes(int a);
int swap_tt_bslash(int a);
int swap_tt_slash(int a);

#endif // PERFECT_SYMMETRIES_SLOW_H_INCLUDED
