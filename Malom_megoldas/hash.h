/*
Malom, a Nine Men's Morris (and variants) player and solver program.
Copyright(C) 2007-2016  Gabor E. Gevay, Gabor Danner

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


#pragma once

#include "sector.h"


//void init_hash_lookuptables();

class Hash{	
	int W,B; //ezeket cache-locality szempontbol talan erdemesebb lenne a nagy tombok utanra tenni

	int f_lookup[1<<24];
	char f_sym_lookup[1<<24]; //atirva introl charra
	int *f_inv_lookup;
	int *g_lookup;
	int *g_inv_lookup;

	int f_count;

	Sector *s;

public:
	Hash(int W, int B, Sector *s);

	pair<int, eval_elem2> hash(board a);
	board inv_hash(int h);

	int hash_count;

	unsigned short f_sym_lookup2[1<<24]; ///

	void check_hash_init_consistency();

	~Hash();
};


int collapse(board a);
board uncollapse(board a);
int next_choose(int x);

