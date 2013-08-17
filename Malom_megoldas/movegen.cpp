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
#include "movegen.h"
#include "symmetries.h"
#include "sector.h"



Parent parents[max_parents]; //get_parents generates into this
int num_parents; //number of elements in parents
//Parent symparents[max_symparents];
//int num_symparents;

//These are generated from the vb code
#if VARIANT == STANDARD || VARIANT == LASKER
const int millpos[16]={14,56,224,131,3584,14336,57344,33536,917504,3670016,14680064,8585216,65793,263172,1052688,4210752};
const board slide_adjmasks[24]={386,5,1034,20,4136,80,16544,65,98817,1280,264708,5120,1058832,20480,4235328,16640,8519936,327680,656384,1310720,2625536,5242880,10502144,4259840}; //mezok szomszedainak maskjai
#else //Morabaraba
const int millpos[20]={14,56,224,131,3584,14336,57344,33536,917504,3670016,14680064,8585216,65793,263172,1052688,4210752,131586,526344,2105376,8421504};
const board slide_adjmasks[24]={386,517,1034,2068,4136,8272,16544,32833,98817,132354,264708,529416,1058832,2117664,4235328,8405376,8519936,328192,656384,1312768,2625536,5251072,10502144,4292608}; //mezok szomszedainak maskjai
#endif
const board fly_adjmasks[24]={0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF,0xFFFFFF};

int millmasks[1<<24], //lookup table a malomban levo korongok maskjaira
	untakemasks[1<<24]; //lookup table arra, hogy hova lehet visszatenni korongot
int millclosemasks[1<<24]; //lookup table arra, hogy mely poziciokra rakva zarodna be malom

//int invar[1<<24]; //tarolja mindegyik tablarol, hogy mely szimmterimuveletek hagyjak invariansan; a mutatott 0. elem azt tarolja, hogy hany ilyen muvelet van
//const int invar_size=33792256;
//char invar0[invar_size]; //az elobbi ebbe mutato indexeket tarol

void write_movegen(){
	LOG("init_movegen\n");

	memset(millmasks,0,sizeof(millmasks));
	for(int a=0; a<1<<24; a++)
		for(int i=0; i<MILL_POS_CNT; i++)
			if((a & millpos[i]) == millpos[i])
				millmasks[a] |= millpos[i];

	memset(untakemasks,0,sizeof(untakemasks));
	for(int a=0; a<1<<24; a++){
		int r = (~a)&mask24; //alapbol ures helyekre lehet visszatenni
		for(int i=0; i<MILL_POS_CNT; i++){
			int mp=millpos[i];
			int amp=a&mp;

			if(__popcnt(amp)==2){ //ket korongos malompozicio
				//azt kell meg ellenorizni, hogy csak akkor ussuk ki a mezot, ha nem lenne minden malomban, ha oda visszatennenk a korongot
				board mm=amp^mp; //a mezo maskja
				if(!(millmasks[a|mm]==(a|mm)))
					r&=~mp;
			}
		}

		untakemasks[a] = r;
	}

	memset(millclosemasks,0,sizeof(millclosemasks));
	for(int a=0; a<1<<24; a++){
		for(int i=0; i<MILL_POS_CNT; i++){
			int m=millpos[i];
			int am=a&m;
			if(__popcnt(am)==2)
				millclosemasks[a] |= m^am;
		}
	}


	/*memset(invar0,0,invar_size);
	int c=0;
	for(int a=0; a<1<<24; a++){
		int start=c;
		invar[a]=start;
		c++;
		for(int op=0; op<MILL_POS_CNT; op++){
			if(sym48(op,a)==a)
				invar0[c++]=op;
		}
		invar0[start]=c-start-1;
	}*/

	LOG("Writing to file\n");
	FILE *f;
	errno_t e = fopen_s(&f, movegen_file.c_str(), "wb");
	if(e){
		LOG("!!! Could not create movegen file\n");
		exit(4);
	}
	fwrite(millmasks, sizeof(millmasks), 1, f);
	fwrite(untakemasks, sizeof(untakemasks), 1, f);
	fwrite(millclosemasks, sizeof(millclosemasks), 1, f);
	fclose(f);
}

void read_movegen(){
	LOG("Reading movegen lookuptables.\n");
	FILE *f;
	errno_t e = fopen_s(&f, movegen_file.c_str(), "rb");
	if(e){
		LOG("!!! Could not open movegen file\n");
		exit(6);
	}
	fread(millmasks, sizeof(millmasks), 1, f);
	fread(untakemasks, sizeof(untakemasks), 1, f);
	fread(millclosemasks, sizeof(millclosemasks), 1, f);
	fclose(f);
}


#if DANNER
	#define _BitScanReverse64 _BitScanReverse
#else
	//ez egy intellisense-es hiba eltuntetesere van
	#ifndef _M_AMD64
		unsigned char _BitScanReverse64(unsigned long * Index, unsigned __int64 Mask);
	#endif
#endif

template<class T> __forceinline T lssb(T x){return x&-x;} //returns mask of least significant set bit

void board_negate(board& a){
	a=(a>>24) | ((a&mask24)<<24);
}

void get_parents(board a, int w, int b, int wf, int bf){
	board_negate(a);
	swap(w,b); swap(wf,bf);

	num_parents=0;

	//mindig B kovetkezik, merthogy a fuggveny elejen negalunk (tehat a feher jatekos (mask24) lepeset csinaljuk visszafele)

	board free=(~(a|(a>>24)))&mask24;

	board cpmills = millmasks[a&mask24]; //cp malomban levo korongjai
	board can_untake_mask; //azok a poziciok, ahova rakhatunk vissza korongot
	if(cpmills)
		can_untake_mask=(untakemasks[a>>24]&free)<<24; //erre csak akkor lesz szukseg, ha van malom, amit esetleg kinyithatunk (itt meg lehetne gyorsitani, ha eltarolnank egy lookup table-ben azt is, hogy lehet-e malmot kinyitni (merthogy a blokkolasok miatt ez nem biztos jelenleg))
	
	board cp=a&mask24; //cp korongjai, kiveve ld. az alabbi sort  (a ciklus megeszi!)
		  
	if(b+bf==max_ksz)
		cp^=cpmills; //ha b+bf max_ksz fole menne, akkor nem csinalhatunk vissza malombecsukast

	const board *adjmasks = w+wf>3 ? slide_adjmasks : fly_adjmasks; //javitottuk b-rol w-re  //(tovabba nem valtozik meg a lepesed soran)

	//csak azokat a szuloket generaljuk, amelyek benne vannak a fo WU-ban
	#define WMS(x) ((x) ? (x)->wms : -1)
	signed char wms;
	signed char 
		/*wms_w_b_wf_bf=WMS(sectors[w][b][wf][bf]), wms_wm_b_wfp_bf=WMS(sectors[w-1][b][wf+1][bf]),
		wms_w_bp_wf_bf=WMS(sectors[w][b+1][wf][bf]), wms_wm_bp_wfp_bf=WMS(sectors[w-1][b+1][wf+1][bf]);*/
		v_mozg = WMS(sectors[w][b][wf][bf]), //wms_w_b_wf_bf
		v_felrak = w==0 || wf==max_ksz ? -1 : WMS(sectors[w-1][b][wf+1][bf]), //wms_wm_b_wfp_bf
		v_mozg_kle = b==max_ksz ? -1 : WMS(sectors[w][b+1][wf][bf]), //wms_w_bp_wf_bf
		v_felrak_kle = w==0 || b==max_ksz || wf==max_ksz ? -1 : WMS(sectors[w-1][b+1][wf+1][bf]); //wms_wm_bp_wfp_bf

	#if VARIANT==STANDARD || VARIANT==MORABARABA
		//#define P_MOZG_COND (wf==0 && bf==0) //bf==0 was to avoid generating parents in non-existing sectors, but this is not really needed, and it would break FULL_SECTOR_GRAPH
		#define P_MOZG_COND (wf==0)
	#else //Lasker
		#define	P_MOZG_COND true
	#endif
	//(P_FELRAK_COND azert nincs, mert ha visszacsinalunk egy felrakast, akkor lesz mit folrakni.)
	
	board kor; //az aktualis korong maskja
	while(kor=lssb(cp)){ cp^=kor;
		unsigned long kor_i; _BitScanReverse64(&kor_i, kor); //az aktualis korong indexe
		board adjmask = adjmasks[kor_i] & free; //szabad szomszedos mezok  (a ciklus megeszi)
		board akor=a^kor;
		
		board moveso_mask; //move source mask
		if(!(cpmills&kor)){ //ha nem malombol lepunk
			if(P_MOZG_COND){ //mozgatas
				if((wms=v_mozg)!=-1){
					while(moveso_mask=lssb(adjmask)){ adjmask^=moveso_mask;
						parents[num_parents++] = Parent(akor^moveso_mask, wms);
					}
				}
			}

			//felrakas
			if((wms=v_felrak)!=-1){
				parents[num_parents++] = Parent(akor, wms);
			}
		}else{ //ha malombol lepunk
			if((wms=v_mozg_kle)!=-1){
				if(P_MOZG_COND){ //mozgatas
					while(moveso_mask=lssb(adjmask)){ adjmask^=moveso_mask;
						board unt_mask; //untake mask
						board can_untake_mask0=can_untake_mask &~(moveso_mask<<24); //onnan sem vehetunk le korongot, ahonnan leptunk
						board akormoveso_mask=akor^moveso_mask;
						while(unt_mask=lssb(can_untake_mask0)){ can_untake_mask0^=unt_mask;
							parents[num_parents++] = Parent(akormoveso_mask^unt_mask, wms);
						}
					}
				}
			}

			//felrakas
			if((wms=v_felrak_kle)!=-1){
				board unt_mask; //untake mask
				board can_untake_mask0=can_untake_mask; //mert a can_untake_mask-ot nem eheti meg a ciklus
				while(unt_mask=lssb(can_untake_mask0)){ can_untake_mask0^=unt_mask;
					parents[num_parents++] = Parent(akor^unt_mask, wms);
				}
			}			
		}
	}

	if(num_parents>=max_parents){
		LOG("\n!!! num_parents>=max_parents,  num_parents: %d, a: %d\n", num_parents, a);
		REL_ASSERT(false);
	}
}

//void get_parents(board a, int w, int b, int wf, int bf){
//	board_negate(a);
//	GET_PARENTS(a, b,w,bf,wf);
//}

void get_parents(board a, id id){
	get_parents(a, id.W,id.B,id.WF,id.BF);
}



//void get_symparents(board a, int w, int b, int wf, int bf){
//	num_symparents=0;
//	get_parents(a,w,b,wf,bf);
//	for(int i=0; i<num_parents; i++){
//		/*for(int op=0; op<16; op++){
//			Parent &p=symparents[num_symparents++];
//			p=parents[i];
//			p.a=sym48(op,p.a);
//		}*/
//		Parent &p=parents[i];
//		int a=p.a&mask24;
//		char *iv=&invar0[invar[a]], n=iv[0];
//		for(int j=0; j<n; j++){
//			int op=iv[j+1];
//			Parent &sp=symparents[num_symparents++];
//			sp=parents[i];
//			sp.a=sym48(op,sp.a);
//		}
//	}
//}










//int std_child_count(board a, int w, int b, int wf, int bf){
//	
//	board free=(~(a|(a>>24)))&mask24;
//
//	int opp_notinmill=(int)(a>>24)^millmasks[a>>24]; //az ellenfel nem malomban levo korongjai
//	int opp_takeable_cnt=(opp_notinmill ?
//					__popcnt(opp_notinmill) :
//					__popcnt((unsigned int)(a>>24))); //barmelyik korongjat levehetjuk
//		
//	if(wf){ //felrakas
//		int mill_close=millclosemasks[a&mask24]; //malombezarasos poziciok
//		return  __popcnt(free & ~mill_close) +
//				__popcnt(free & mill_close) * opp_takeable_cnt;
//	}else{ //mozgatas
//		int r=0;
//		const board *adjmasks = w+wf>3 ? slide_adjmasks : fly_adjmasks;
//		board cp=a&mask24; //cp korongjai (a ciklus megeszi!)
//		board kor; //az aktualis korong maskja
//		while(kor=lssb(cp)){
//			cp^=kor;
//			unsigned long kor_i; _BitScanReverse64(&kor_i, kor); //az aktualis korong indexe
//			board adjmask = adjmasks[kor_i] & free; //szomszedos szabad mezok
//			
//			board akorm24=(a^kor)&mask24;
//			board moveend;
//			while(moveend=lssb(adjmask)){ adjmask^=moveend;
//				if(millmasks[akorm24^moveend]&moveend)
//					r+=opp_takeable_cnt;
//				else
//					r++;
//			}
//		}
//
//		return r;
//	}
//}



bool can_close_mill(board a, int w, int b, int wf, int bf){
	if(!(a&(mask24<<24))) return false; //ha nincs az ellenfelnek korongja
	//Valojaban erre nem lenne szukseg, ld. doc-ban "Nincs korong fönt problémák" szekcio

	board free=(~(a|(a>>24)))&mask24; //az also 24 biten mutatja, hogy se feher, se fekete nincs-e ott
		
	#if VARIANT==STANDARD || VARIANT==MORABARABA
		#define CCM_MOZG_COND (!CCM_FELRAK_COND)
		#define CCM_FELRAK_COND wf
	#else //Lasker
		#define CCM_MOZG_COND w
		#define CCM_FELRAK_COND wf
	#endif

	if(CCM_FELRAK_COND){
		if(millclosemasks[a&mask24] & free) //malombezarasos poziciok
			return true;
	}

	if(CCM_MOZG_COND){
		const board *adjmasks = w+wf>3 ? slide_adjmasks : fly_adjmasks;
		board cp=a&(mask24); //cp korongjai (a ciklus megeszi!)
		board kor; //az aktualis korong maskja
		while(kor=lssb(cp)){
			cp^=kor;
			unsigned long kor_i; _BitScanReverse64(&kor_i, kor); //az aktualis korong indexe
			board adjmask = adjmasks[kor_i] & free; //szomszedos szabad mezok
			
			board akorm24=(a^kor)&mask24;
			board moveend;
			while(moveend=lssb(adjmask)){ adjmask^=moveend;
				if(millmasks[akorm24^moveend]&moveend)
					return true;
			}
		}
	}

	return false;
}






//unsigned int orbitsize(board a){
//	set<board> s;
//	for(int op=0; op<16; op++)
//		s.insert(sym48(op,a));
//	return s.size();
//}








Child::Child(board a,Sector *s):a(a),s(s){}
Child::Child(){}

Child chd[1024]; //(uj variansoknal atgondolni (Std, Lasker, Morabaraba-ban atgondolva))
int num_chd;
id a_id;

Sector *felrakas,*felrakas_kle,*mozg,*mozg_kle;

void init_get_chd_sectors(id a_id0){
	LOG("init_get_chd_sectors \n");
	a_id=a_id0;

	//meg vannak cserelve az indexek
	felrakas = /*a_id.W<max_ksz &&*/ a_id.WF>0 ? sectors[a_id.B][a_id.W+1][a_id.BF][a_id.WF-1] : nullptr;
	felrakas_kle = a_id.B>0 && /*a_id.W<max_ksz &&*/ a_id.WF>0 ? sectors[a_id.B-1][a_id.W+1][a_id.BF][a_id.WF-1] : nullptr;
	mozg = sectors[a_id.B][a_id.W][a_id.BF][a_id.WF];
	mozg_kle = a_id.B>0 ? sectors[a_id.B-1][a_id.W][a_id.BF][a_id.WF] : nullptr;
}

void get_chd(board a){
	num_chd=0;
	
	board free=(~(a|(a>>24)))&mask24; //a felrakasos agban megesszuk

	board opp_notinmill=(a>>24)^millmasks[a>>24]; //az ellenfel nem malomban levo korongjai
	board opp_takeable=opp_notinmill ?
					(opp_notinmill<<24) :
					(a&(mask24<<24)); //barmelyik korongjat levehetjuk
	
	#if VARIANT==STANDARD || VARIANT==MORABARABA
		#define C_MOZG_COND (!C_FELRAK_COND)
		#define C_FELRAK_COND (a_id.WF)
	#else //Lasker
		#define C_MOZG_COND (a_id.W) //ez lehetne akar true is, mert ugyse menne bele a while-ba
		#define C_FELRAK_COND (a_id.WF)
	#endif

	if(C_MOZG_COND){
		const board *adjmasks = a_id.W+a_id.WF>3 ? slide_adjmasks : fly_adjmasks;
		board cp=a&(mask24); //cp korongjai (a ciklus megeszi!)
		board kor; //az aktualis korong maskja
		while(kor=lssb(cp)){
			cp^=kor;
			unsigned long kor_i; _BitScanReverse64(&kor_i, kor); //az aktualis korong indexe
			board adjmask = adjmasks[kor_i] & free; //szomszedos szabad mezok
			
			board akorm24=(a^kor)&mask24;
			board moveend;
			while(moveend=lssb(adjmask)){ adjmask^=moveend;
				if(millmasks[akorm24^moveend]&moveend){ //koronglevetel
					board akormoveend=a^kor^moveend;
					board opp_takeable0=opp_takeable; //megesszuk
					board taking;
					while(taking=lssb(opp_takeable0)){ opp_takeable0^=taking;
						chd[num_chd++]=Child(akormoveend^taking, mozg_kle);
					}
				}else
					chd[num_chd++]=Child(a^kor^moveend, mozg);
			}
		}
	}

	if(C_FELRAK_COND){
		board kor;
		while(kor=lssb(free)){ free^=kor;
			board akor=a^kor;
			if(millmasks[(akor)&mask24]&kor){ //koronglevetel
				board opp_takeable0=opp_takeable; //megesszuk
				board taking;
				while(taking=lssb(opp_takeable0)){ opp_takeable0^=taking;
					chd[num_chd++]=Child(akor^taking, felrakas_kle);
				}
			}else{
				chd[num_chd++]=Child(akor, felrakas);
			}
		}
	}

	for(int i=0; i<num_chd; i++)
		board_negate(chd[i].a);
}
