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

#include "wrappers.h"


//Wrappers::Hash::Hash(int W, int B, Sector ^s){
//	hash_obj=new ::Hash(W,B,s->s);
//}
//
//int Wrappers::Hash::hash_count(){
//	return hash_obj->hash_count;
//}

//This manages the lookuptables of the hash function: it keeps them in memory for a few most recently accessed sectors.
Tuple<int, Wrappers::gui_eval_elem2>^ Wrappers::Sector::hash(board a){
	static set<pair<int, ::Sector*>> loaded_hashes;
	static map<::Sector*, int> loaded_hashes_inv;
	static int timestamp = 0;

	::Sector *tmp = s;

	if(s->hash == nullptr){
		// hash object is not present

		if(loaded_hashes.size()==8){
			// release one if there are too many
			::Sector *to_release = loaded_hashes.begin()->second;
			LOG("Releasing hash: %s\n", to_release->id.to_string().c_str());
			to_release->release_hash();
			loaded_hashes.erase(loaded_hashes.begin());
			loaded_hashes_inv.erase(to_release);
		}

		// load new one
		LOG("Loading hash: %s\n", s->id.to_string().c_str());
		s->allocate_hash();
	} else {
		// update access time
		loaded_hashes.erase(make_pair(loaded_hashes_inv[tmp], tmp));
	}
	loaded_hashes.insert(make_pair(timestamp, tmp));
	loaded_hashes_inv[tmp] = timestamp; // s doesn't work here, which is probably a compiler bug!

	timestamp++;

	auto e=s->hash->hash(a);
	return Tuple::Create(e.first, Wrappers::gui_eval_elem2(e.second, s));
}

//board Wrappers::Hash::inv_hash(int h){
//	return hash_obj->inv_hash(h);
//}


void Wrappers::id::negate(){
	int t=W;
	W=B;
	B=t;

	t=WF;
	WF=BF;
	BF=t;
}

Wrappers::id Wrappers::id::operator-(id s){
	id r = id(s);
	r.negate();
	return r;
}
