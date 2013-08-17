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


#include "stdafx.h"

#include "common.h"
#include "abstract_queue.h"
#include "retrograde.h"


abstract_queue::abstract_queue() : q1first(brm.pop()) {}

void abstract_queue::push(int hash, signed char wms){
	assert(wms!=-1);

	q2.push(hash^(((int)wms)<<30));
	assert(q2.front()>=0);
}

queue_elem abstract_queue::pop(){
	val q2val;
	int q2hash;
	short_id q2sid;
	if(!q2.empty()){
		q2hash=q2.front();
		assert(q2hash>=0);
		int wms=q2hash>>30;
		q2hash&=((1<<30)-1);
		q2sid=main_secs[wms]->sid;
		q2val=main_secs[wms]->get_eval(q2hash).value();
	}
	if(q2.empty() || q1first.val<q2val){
		queue_elem ret=q1first;
		q1first=brm.pop();
		assert(ret.hash>=0);
		return ret;
	}else{
		q2.pop();
		return queue_elem{ q2sid, q2hash, q2val };
	}
}

bool abstract_queue::empty(){
	return q2.empty() && q1first == queue_elem::max;
}
