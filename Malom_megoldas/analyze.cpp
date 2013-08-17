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
#include "analyze.h"
#include "histogram.h"
#include "sector.h"
#include "hash.h"
#include "debug.h"
#include "auto_grow_vector.h"


extern id run_params;


void analyze(id id){
	LOG("Analyze sector %s\n", id.to_string().c_str());

	Sector *sec=sectors(id)=new Sector(id);
	sec->allocate(false);
	Hash *hash=sec->hash;

	LOG("Analyzing\n");

	int sym_count=0, win_count=0, loss_count=0;

	int ma=-1;
	int mh=0;
	pn_vector<int> dist;
	pn_vector<int> akey1_dist;
	akey1_dist[virt_loss_val];  akey1_dist[virt_win_val]; //force range
	for(int h=0; h<hash->hash_count; h++){
		auto eval=sec->get_eval_inner(h);
		if(eval.cas()!=eval_elem_sym2::Sym){
			sec_val akey1 = eval.key1 + sec->sval;

			int distance = (eval.cas() == eval_elem_sym2::Val && abs(akey1) == virt_win_val) ? eval.value().key2 : -1;

			if(distance > ma){
				ma = distance;
				mh = h;
			}

			assert(distance >= -1);
			dist[distance]++;

			akey1_dist[akey1]++;

			/*if(akey1 == 5){
				LOG("a draw: %s\n", toclp3(sec->hash->inv_hash(h), sec->id).c_str());
			}*/

			if(distance >= 0){
				if(eval.value().key1 > 0)
					win_count++;
				else
					loss_count++;
			}
		}else{
			sym_count++;
		}
	}

	board maxval_board=hash->inv_hash(mh);

	int tot_count = hash->hash_count - sym_count;
	int draw_count = dist[-1];

	LOG("maxval: %d\n", ma);
	LOG("maxval_board: %lld, toclp: %s\n", maxval_board, toclp3(maxval_board, id).c_str());
	LOG("sym_count: %d  (ratio: %f)\n", sym_count, (double)sym_count/hash->hash_count);
	LOG("win/draw/loss ratios: %f/%f/%f\n", (double)win_count/tot_count, (double)draw_count/tot_count, (double)loss_count/tot_count);
	LOG("\ndistance distribution:\n");
	for(auto it = dist.begin(); it != dist.end(); ++it)
		LOG("%d %d\n", it.ind(), *it);
	LOG("\nakey1 distribution:\n");
	for(auto it = akey1_dist.begin(); it != akey1_dist.end(); ++it)
		if(*it)
			LOG("%s %d\n", sec_val_to_sec_name(it.ind()).c_str(), *it);
	LOG("\n");
	
	assert(sym_count + win_count + draw_count + loss_count == hash->hash_count);
	FILE *out;
	fopen_s(&out, (run_params.to_string()+".analyze"FNAME_SUFFIX).c_str(), "w");
	fprintf(out, "%d\n%lld\n%d\n%d %d %d\n", ma, maxval_board, sym_count, win_count, draw_count, loss_count);
	fprintf(out, "%d\n", dist.size());
	for(auto d: dist)
		fprintf(out, "%d\n", d);
#ifdef DD
	fprintf(out, "%d %d\n", virt_loss_val, virt_win_val);
	assert(akey1_dist.size() == virt_win_val - virt_loss_val + 1);
	fprintf(out, "%d\n", akey1_dist.size());
	for(auto x : akey1_dist)
		fprintf(out, "%d\n", x);
#endif
	fclose(out);
	
	histogram(dist.to_vector(), -1, id.to_string()+"_disthist").gnuplot(false);
	
	histogram(akey1_dist.to_vector(), virt_loss_val, id.to_string() + "_akey1hist").gnuplot(false);
}