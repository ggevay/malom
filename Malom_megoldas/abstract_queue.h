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
#include "stdafx.h"

#include "bucket.h"
#include "biglinked_queue.h"

using namespace std;


//csak a 3. menetben hozzuk letre, mert a brm letrehozasakor mar zarva kell, hogy legyen a bwm
//(tehat iraskor mindig a q2-be ir, es olvasasnal fesuli ossze a ketfele sort)
class abstract_queue{
	//queue<int> q2;
	queue<int, biglinked_queue<int>> q2;

	bucket_reader_mgr brm;

	queue_elem q1first;

public:
	abstract_queue();
	void push(int hash, signed char wms);
	queue_elem pop();
	bool empty();
};