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

//#include "sector.h" //korkoros dependencia lenne, ugyhogy inkabb forward declarationnel vezetjuk be a Sector-t es a sectors-t (ld. http://stackoverflow.com/questions/625799/resolve-circular-dependencies-in-c)
#include "sector_graph.h"

void write_movegen();
void read_movegen();


void get_parents(board a, id id);
void get_parents(board a, int w, int b, int wf, int bf);
//void get_symparents(board a, int w, int b, int wf, int bf);

class Sector;
extern Sector* sectors[max_ksz+1][max_ksz+1][max_ksz+1][max_ksz+1];

struct Parent{
	board a;
	signed char wms;
	Parent(board a, signed char wms):a(a),wms(wms){}
	Parent(){}
};

const int max_parents=2048; //std-ben 3,3,0,0-ban vannak 900 koruli ertekek, tovabba laskerben lattunk 1026-osat, valamint 1500 koruli felulbecslesunk volt
extern Parent parents[max_parents]; //ebbe general a get_parents
extern int num_parents; //a parentsben levo elemek szama
//const int max_symparents=16384;
//extern Parent symparents[max_symparents];
//extern int num_symparents;



int std_child_count(board a, int w, int b, int wf, int bf);



bool can_close_mill(board a, int w, int b, int wf, int bf);


//unsigned int orbitsize(board a);



struct Child{
	board a;
	Sector *s;
	Child(board a,Sector *s);
	Child();
};
void get_chd(board a);
void init_get_chd_sectors(id a_id);
extern Child chd[1024];
extern int num_chd;
