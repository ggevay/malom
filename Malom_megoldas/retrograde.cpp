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
#include "retrograde.h"

#include "sector_graph.h"
#include "sector.h"
#include "movegen.h"
#include "bucket.h"
#include "hash.h"
#include "abstract_queue.h"
#include "debug.h"


bool twin; //(amugy van olyan, hogy nem EKS, de megse twin, pl. standard felrakas paratlan szinten)
vector<Sector*> main_secs;

id id0;

set<id> wsectors;
map<id, id> parent_ids;
void collect_sector_family(id u){ //berakja a wsectorsba az id-t es a gyerekeit
	wsectors.insert(u);
	auto e=graph_func(u);
	wsectors.insert(e.begin(),e.end());
	for(auto v : e){
		assert(!parent_ids.count(v));
		parent_ids[v] = u;
	}
}

void create_and_collect_sectors(){
	collect_sector_family(id0);
	if(twin)
		collect_sector_family(-id0);

	for(auto it=wsectors.begin(); it!=wsectors.end(); ++it)
		sectors(*it)=new Sector(*it);

	main_secs.push_back(sectors(id0));
	if(twin)
		main_secs.push_back(sectors(-id0));

	for(auto it=main_secs.begin(); it!=main_secs.end(); ++it){
		(*it)->allocate(true);
		(*it)->wms=(signed char)(it-main_secs.begin());
	}
}


bucket_writer_mgr *bwm;

void phase1(){
	/*1. minden szektorban végigiterálunk a (nem-sym) állásokon:
		-foszektorbeli szülo állások count-ját inceljük
		-queue-ba rakás (ha nem döntetlen (azaz val (foszektorban mindig olyan, hogy a key1=win-sval, key2=1)))
	*/
	LOG("***phase1***\n");
	long long counter=0;

	bwm = new bucket_writer_mgr();

	for(auto it=sector_objs.begin(); it!=sector_objs.end(); ++it){
		Sector &s=**it;

		bool alloc=s.wms==-1;
		if(alloc) s.allocate(false, parent_ids[s.id]);

		LOG("p1 processing %s", s.id.to_string().c_str());

		for(int h=0; h<s.hash->hash_count; h++){
			eval_elem_sym2 e=s.get_eval_inner(h);
			if(e.cas() != eval_elem_sym2::Sym){

				get_parents(s.hash->inv_hash(h), s.id);
				for(int i=0; i<num_parents; i++){
					counter++;
					Parent &p=parents[i];
					Sector &ps=*main_secs[p.wms];
					auto ph=ps.hash->hash(p.a);
					if(ph.second.cas() == eval_elem2::Count)
						ps.set_eval(ph.first, ph.second.count() + 1);
				}
				
				if(e.cas() == eval_elem_sym2::Val)
					bwm->push(queue_elem{ s.sid, h, e.value() });
			}
		}

		LOG(".\n");

		if(alloc) s.release();
	}

	LOG("%lld edges\n", counter);
}

void phase2(){
	//2. ahol a foszektorokban count-0 maradt, átírjuk vesztés valra, és berakjuk a sorba

	LOG("***phase2***\n");
	long long counter=0;

	if(!(FULL_BOARD_IS_DRAW && main_secs[0]->W == 12 && main_secs[0]->B == 12)){
		for(auto it = main_secs.begin(); it != main_secs.end(); ++it){
			Sector &s = **it;
			val loss(virt_loss_val - s.sval, 0);
			for(int h = 0; h < s.hash->hash_count; h++){
				counter++;
				eval_elem_sym2 e = s.get_eval_inner(h);
				if(e.cas() == eval_elem_sym2::Count && e.count() == 0){
					s.set_eval(h, loss);
					bwm->push(queue_elem{ s.sid, h, loss });
				}
			}
		}
	} else {
		LOG("Skipping phase2 main loop, because board is full.\n");
	}

	bwm->close();
	LOG("%lld nodes\n", counter);
}

void phase3(){
	//3. a sor feldolgozása (a sorból kivett állások némelyike gyerekszektorbeli, némelyike foszektorbeli)
	LOG("***phase3***\n");
	{
		long long counter = 0;

		abstract_queue aq;
		while(!aq.empty()){
			queue_elem qe = aq.pop();
			Sector *s = qe.sid;

			get_parents(s->hash->inv_hash(qe.hash), s->id);
			for(int i = 0; i < num_parents; i++){
				counter++;
				Parent &p = parents[i];
				Sector &ps = *main_secs[p.wms];
				auto ph = ps.hash->hash(p.a);

				if(ph.second.cas() == eval_elem2::Count){ //count-ba terjesztunk
					if(qe.val.key1 > 0){ //nyeresbol terjesztunk
						if(ph.second.count() == 1){ //elfogyott a count
							//ps.set_eval(ph.first, eval_elem(eval_elem::val, e.val + 1));
							ps.set_eval(ph.first, qe.val.undo_negate());
							aq.push(ph.first, p.wms);
						} else { //nem fogyott el a count
							//ps.set_eval(ph.first, eval_elem(eval_elem::count, ph.second.x - 1));
							ps.set_eval(ph.first, ph.second.count() - 1);
						}
					} else { //vesztesbol terjesztunk
						//ps.set_eval(ph.first, eval_elem(eval_elem::val, e.val + 1));
						ps.set_eval(ph.first, qe.val.undo_negate());
						aq.push(ph.first, p.wms);
					}
				} else { //val-ba terjesztunk
					//assert(ph.second.x & 1); //csak nyeres lehet
					//assert(ph.second.x <= e.val + 1);
					assert(ph.second.value().key1 > 0); //csak nyeres lehet
					assert(ph.second.value() <= qe.val.undo_negate() || ph.second.value().key1 == virt_win_val - ps.sval && qe.val.key1 > 0);
				}
			}
		}

		LOG("%lld edges\n", counter);
	}
}

void print_statistics(){
#ifdef STATISTICS
	for(auto it=main_secs.begin(); it!=main_secs.end(); ++it)
		LOG("Statistics of sector  %s:  max_val: %d, max_count: %d\n", (*it)->id.to_string().c_str(), (*it)->max_val, (*it)->max_count);
#endif
}

void solve(id id0in){
	id0=id0in;
	twin = wus[id0]->twine;  REL_ASSERT(id0.twine() == twin);


	create_and_collect_sectors();

	phase1();

	phase2();

	phase3();

	print_statistics();

	for(auto it=main_secs.begin(); it!=main_secs.end(); ++it)
		(*it)->save();
}

