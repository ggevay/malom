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

#include "verify.h"
#include "movegen.h"
#include "sector.h"
#include "sector_graph.h"
#include "hash.h"


const int inf=2000000000;


eval_elem2 negate_eval_elem2(eval_elem2 v){
	if(v.cas() == eval_elem2::Count)
		return eval_elem2{ 0, 0 };
	else
		return eval_elem2{ (sec_val)(-v.key1), v.key2 + 1 };
}


void verify(id main_id){
	LOG("Verify sector %s\n", main_id.to_string().c_str());
	
	Sector *main_sec=sectors(main_id)=new Sector(main_id);
	main_sec->allocate(false);
	Hash *main_hash=main_sec->hash;


	vector<id> chd_sectors=graph_func(main_id);
	for(auto it=chd_sectors.begin(); it!=chd_sectors.end(); ++it)
		(sectors(*it)=new Sector(*it))->allocate(false, main_id);

	init_get_chd_sectors(main_id);

	LOG("Verifying\n");

	bool verification_ok=true;
	for(int h=0; h<main_hash->hash_count; h++)
		if(main_sec->get_eval_inner(h).cas() != eval_elem_sym2::Sym){
			//int ma=-inf;
			eval_elem2 ma = eval_elem2{ (sec_val)(virt_loss_val - main_sec->sval), 0 }; //a 0 tavolsag itt valoban minimalis, mert nem valt elojelet a key1, ha valahol virt_loss_val-bol indult, tehat a key2 nem lehet negativ

			board a=main_hash->inv_hash(h);
			get_chd(a);
			for(int i=0; i<num_chd; i++)
				//a get_parents, get_chd, can_close_mill-nek illeszkedniuk kell egymashoz es a win_conditionhoz is
				//pl. ld. std_can_close_mill elso sora, tovabba doc-ban "Nincs korong fönt problémák" szekcio
				if(chd[i].s!=nullptr){
					ma = max(ma, negate_eval_elem2(chd[i].s->hash->hash(chd[i].a).second));
				}else{
					//ez csakis egy egylepeses nyeres lehet
					//(a graph_func-ban is szerepel a B+BF feltetel, tehat azert lesz nullptr, mert nem hozunk letre teljesen vesztes szektort)
					REL_ASSERT(main_id.B+main_id.BF<=3 && can_close_mill(a,main_id.W,main_id.B,main_id.WF,main_id.BF)); //ez a win conditionunk
					//ma=inf-1;
					assert(virt_win_val);
					ma = eval_elem2{ (sec_val)(virt_win_val - main_sec->sval), 1 };
				}

#if FULL_BOARD_IS_DRAW
			if(main_sec->W == 12 && main_sec->B == 12){
				assert(main_sec->sval == 0);
				ma = eval_elem2{ 0, 0 };
			}
#endif

			auto shouldbe = main_sec->get_eval(h);
			if(shouldbe != ma){
				LOG("!!! Verification error !!!  Sector: %s, Hash: %d, Board: %lld, shouldbe: (%hd, %d), ma: (%hd, %d)\n", main_sec->id.to_string().c_str(), h, a, shouldbe.key1, shouldbe.key2, ma.key1, ma.key2);
				verification_ok=false;
				static int verif_error_count = 0;
				verif_error_count++;
				if(verif_error_count > 1000){
					LOG("!!! verif_error_count reached maximum, exiting.\n");
					break;
				}
			}
		}

	
	if(verification_ok)
		LOG("Verification OK!\n");
	else{
		exit(3);
	}
}