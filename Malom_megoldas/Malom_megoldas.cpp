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
#include "symmetries.h"
#include "hash.h"
#include "movegen.h"
#include "sector.h"
#include "retrograde.h"
#include "sector_graph.h"
#include "verify.h"
#include "analyze.h"

#include "abstract_queue.h"
#include "bucket.h"
#include "debug.h"




const int bar_width=80;

void progressbar(int a, int b, int sum) {
	static char t[bar_width+1];
	int ha=int(a*(__int64)(bar_width-2)/sum), hb=int((a+b)*(__int64)(bar_width-2)/sum), i=1;
	t[0]='[';
	while (i<=ha) t[i++]='=';
	while (i<=hb) t[i++]='!';
	while (i<=bar_width-2) t[i++]='-';
	t[bar_width-1]=']';
	t[bar_width]=0;
	cout << t; //inkabb printf()?
}


void exp(); void exp2(); void exp3();

id run_params;
modes mode = uninit;


int main(int argc, char* argv[])
{
	//exp3(); system("pause"); return 0;

	////
	//argc = 6;
	//argv = new char*[argc];
	//argv[1] = "3"; argv[2] = "5"; argv[3] = "1"; argv[4] = "0";
	//argv[5] = "-solve";
	//#pragma message("Warning: params hardcoded!!!")
	////


	int W, B, WF, BF;
	if(argc<6){
		if(argc == 2 && !strcmp(argv[1], "-writemovegenlookups")){
			write_movegen();
			return 0;
		} else {
			LOG("Invalid arguments; argc=%d\n", argc);
			exit(5);
		}
	}else{
		sscanf_s(argv[1],"%d",&W);
		sscanf_s(argv[2],"%d",&B);
		sscanf_s(argv[3],"%d",&WF);
		sscanf_s(argv[4],"%d",&BF);
		run_params=id(W,B,WF,BF);
		if(!strcmp(argv[5], "-solve")){
			mode=solution_mode;
			Log::setup_logfile(run_params.to_string(), "solutionlog"); //ide nem kell a suffix, mert azt a Log osztaly intezi
		}else{
			if(!strcmp(argv[5], "-verify")){
				mode=verification_mode;
				Log::setup_logfile(run_params.to_string(), "verificationlog");
				LOG("Verification mode\n");
			}else
				if(!strcmp(argv[5], "-analyze")){
					mode=analyze_mode;
					Log::setup_logfile(run_params.to_string(), "analyzelog");
					LOG("Analyze mode\n");
				}else{
					LOG("Unknown argument: %s\n",argv[5]);
					exit(4);
				}
		}
	}
	LOG("Parameters: %d, %d, %d, %d\n", W,B,WF,BF);

#ifdef DD
	LOG("Running in DD mode.\n");
#else
	LOG("Running in legacy mode.\n");
#endif


	//ezeknek mindenkeppen bent kell lenniuk!
	init_sym_lookuptables();
	if(mode != analyze_mode) read_movegen();
	init_sector_graph();
	init_sec_vals();

	//exp(); return 0;

	time_t start_time = time(0);
	
	switch(mode){
	case solution_mode:
		solve(id(W,B,WF,BF));
		break;
	case verification_mode:
		verify(id(W,B,WF,BF));
		break;
	case analyze_mode:
		analyze(id(W,B,WF,BF));
		break;
	default:
		assert(0);
	}

	LOG("Time: %lld s\n", time(0)-start_time);

	Log::close();
	
	return 0;
}







struct tmps{
	char id;
	int hash;
};

extern bucket_writer_mgr *bwm;

void exp3(){
	vector<queue_elem> qes;
	for(int i = 0; i < 1000; i++){
		short_id sid = rand() % 3;
		int hash = rand() % 1000000;
		val v = val((rand() % 100 + 1) * ((rand() % 2) * 2 - 1), rand() % 300);

		qes.push_back(queue_elem{ sid, hash, v });
	}
	{
		bwm = new bucket_writer_mgr();
		for(auto qe : qes){
			bwm->push(qe);
		}
		bwm->close();
	}
	cout << endl;

	vector<queue_elem> rqes;
	{
		bucket_reader_mgr brm;
		while(1){
			queue_elem l = brm.pop();
			if(l == queue_elem::max)
				break;
			rqes.push_back(l);
			cout << ".";
		}
	}

	stable_sort(qes.begin(), qes.end(), [](queue_elem a, queue_elem b){return a.val < b.val; });
	assert(qes.size() == rqes.size());
	for(unsigned int i = 0; i < qes.size(); i++){
		assert(rqes[i] == qes[i]);
	}
}


void exp2(){
	/*Log<<"proba";
	system("pause");
	return 0;*/


	////Log::setup_logfile("proba.txt");
	//LOG("alma\n");
	//LOG("korte\n");
	//system("pause");
	//return 0;


	//toclp(820338753577);


	init_sym_lookuptables();
	Hash *h=new Hash(12,9,nullptr);





	/*bucket_writer_mgr bwm;
	auto tmp=queue_elem((char)0, 0,0);
	bwm.push(tmp);*/




	/*bucket_writer b(0);
	b.write(bucket_elem((char)0, 0));
	b.close();*/

	//bucket_reader br(0);
	//br.re

	
	
	//auto f=fopen((tostring(0)+".bucket").c_str(),"wb");
	//bucket_elem e;
	//e.hash=532642523;
	//e.id=(char)2;
	////fwrite(&e, sizeof(e),1,f);
	//fwrite(&e.id, sizeof(e.id),1,f);
	//fwrite(&e.hash, sizeof(e.hash),1,f);
	//fclose(f);
	////cout<<sizeof(e)<<endl;
	////system("pause");

	//{
	//	auto f=fopen((tostring(0)+".bucket").c_str(),"rb");
	//	bucket_elem e;
	//	fread(&e, sizeof(e), 1, f);
	//	cout<<(int)((char)(e.id.sid))<<endl;
	//	cout<<e.hash<<endl;
	//	system("pause");
	//}




	//auto f=fopen((tostring(0)+".bucket").c_str(),"wb");
	//tmps e;
	//e.hash=0;
	//e.id=0;
	//fwrite(&e, sizeof(e),1,f);
	//fclose(f);
	////cout<<sizeof(e)<<endl;
	////system("pause");

	/*{
		auto f=fopen((tostring(0)+".bucket").c_str(),"rb");
		tmps e;
		fread(&e, sizeof(e), 1, f);
		cout<<(int)e.id<<endl;
		cout<<e.hash<<endl;
		system("pause");
	}*/

}

void exp(){



	/*Sector s(id(3,3,0,0));
	for(int i=0; i<1000; i++){
		auto e=eval_elem(eval_elem::val,i);
		s.set_eval(0,e);
		assert(s.get_eval(0)==e);
	}
	for(int i=0; i<1000; i++){
		auto e=eval_elem(eval_elem::count,i);
		s.set_eval(0,e);
		auto k=s.get_eval(0);
		assert(k==e);
	}
	for(int i=0; i<1000; i++){
		auto e=eval_elem_sym(eval_elem_sym::val,i);
		s.set_eval_inner(0,e);
		assert(s.get_eval_inner(0)==e);
	}
	for(int i=0; i<1000; i++){
		auto e=eval_elem_sym(eval_elem_sym::count,i);
		s.set_eval_inner(0,e);
		auto k=s.get_eval_inner(0);
		assert(k==e);
	}
	for(int i=0; i<15; i++){
		auto e=eval_elem_sym(eval_elem_sym::sym,i);
		s.set_eval_inner(0,e);
		assert(s.get_eval_inner(0)==e);
	}*/





	////Hash-fv tesztelese: hash(inv_hash(h))==h
	//init_sym_lookuptables();
	//int W, B, WF, BF;
	//W=3; B=3; WF=0; BF=0;
	//id id(W,B,WF,BF);
	////Hash *hash=new Hash(W,B);
	//Sector *s=new Sector(id);
	//s->allocate(true);
	//Hash *hash=s->hash;

	//int tmp=0;
	//for(int h=0; h<hash->hash_count; h++){
	//	if(s->get_eval_inner(h).c!=eval_elem_sym::sym){
	//		//cout<<h<<endl;
	//		board a=hash->inv_hash(h);
	//		int h2=hash->hash(a).first;
	//		if(h!=h2){
	//			cerr<<"Baj!!!: "<<h<<" "<<h2<<" a: "<<a<<endl;
	//			//system("PAUSE");
	//		}
	//	}
	//}
	//cout<<tmp<<endl;


	
	////Hash-fv tesztelese, hogy minden orbit minden elemenek azonos-e a hash-e
	//int W, B, WF, BF;
	//W=3; B=3; WF=0; BF=0;
	//id id(W,B,WF,BF);
	////Hash *hash=new Hash(W,B);
	//Sector *s=new Sector(id);
	//Hash *hash=s->hash;

	//int tmp=0;
	//for(int h=0; h<hash->hash_count; h++){
	//	if(s->get_eval(h).c!=eval_elem_sym::sym){
	//		//cout<<h<<endl;
	//		board a=hash->inv_hash(h);
	//		for(int op=0; op<16; op++){
	//			board a2=sym48(op,a);
	//			auto h2=hash->hash(a2);
	//			if(h!=h2.first){
	//				cerr<<"Baj!!!: "<<h<<" "<<h2.first<<endl;
	//				system("PAUSE");
	//			}
	//		}
	//	}
	//}
	//cout<<tmp<<endl;







	//progressbar(10,30,100);

	//int op=0;cin>>op; int a=0;cin>>a;
	//cout<<sym48(op,a)<<endl;
	////cout<<sym48(11,2)<<endl;

	
	/*W=8; B=8; WF=0; BF=0;
	Hash *hash=new Hash(W,B);
	cout<<"Meres indul, hash_count="<<hash->hash_count<<endl;
	board a;*/

	////
	//int h=603332730;
	//for(int i=0; i<10; i++){
	//	a=hash->inv_hash(h);
	//	cerr<<h<<" "<<a<<endl;
	//	h=hash->hash(a);
	//}
	////

	
	//a = 1112916655328;
	//a=(a>>24) | ((a&mask24)<<24);
	//get_parents(a,W,B,WF,BF);


	
	////2:04.176 (get_parents-cel)
	//for(int i=0; i<hash->hash_count; i++){
	//	//cout<<i<<endl;
	//	a=hash->inv_hash(i);
	//	int h=hash->hash(a);
	//	if(h!=i){
	//		cerr<<"Baj!!!: "<<h<<" "<<i<<endl;
	//		board a2=hash->inv_hash(h);
	//		cerr<<a2<<" "<<hash->hash(h)<<endl;
	//		system("PAUSE");
	//	}
	//	//get_parents(a,W,B,WF,BF);
	//}

	////11:23 (8 8 0 0)
	//int tmp=0;
	//for(int i=0; i<hash->hash_count; i++){
	//	//cout<<i<<endl;
	//	a=hash->inv_hash(i);
	//	int h=hash->hash(a);
	//	if(h!=i){
	//		cerr<<"Baj!!!: "<<h<<" "<<i<<endl;
	//		board a2=hash->inv_hash(h);
	//		cerr<<a2<<" "<<hash->hash(h)<<endl;
	//		system("PAUSE");
	//	}
	//	get_parents(a,W,B,WF,BF);
	//	
	//	for(int j=0; j<num_parents; j++)
	//		tmp+=hash->hash(a); //ez nem a parent-oket hash-eli egyelore, mert ahhoz kene a masik sector hash fuggvenyet is inicializalni (persze ez igy a cache-ek miatt nem igazan jol meri a sebesseget)
	//}
	//cout<<tmp<<endl;



	//for(int i=0; i<hash->hash_count; i++){
	//	//cout<<i<<endl;
	//	a=hash->inv_hash(i);
	//	//int c=CHILD_COUNT(a,W,B,0,0);
	//	int c=CHILD_COUNT(a,W,B,1,1);
	//	c+=0;
	//	//cout<<c<<endl;
	//}



	//int h0=0;
	//for(int i=0; i<hash->hash_count; i++){
	//	//cout<<i<<endl;
	//	a=hash->inv_hash(h0);
	//	int h=hash->hash(a);
	//	if(h!=h0){
	//		cerr<<"Baj!!!: "<<h<<" "<<i<<endl;
	//		board a2=hash->inv_hash(h);
	//		cerr<<a2<<" "<<hash->hash(h)<<endl;
	//		system("PAUSE");
	//	}

	//	h0+=10000000; //azert jobb igy tesztelni, mert igy nem fognak egymasra hasonlitani az egymas utani allasok
	//	h0%=603332730;
	//}



	/*W=3; B=3; WF=0; BF=0;
	Hash *hash=new Hash(W,B);
	cout<<"Meres indul, hash_count="<<hash->hash_count<<endl;
	for(int h=0; h<hash->hash_count; h++){
		board a=hash->inv_hash(h);
		get_parents(a,W,B,WF,BF);
		for(int i=0; i<num_parents; i++)
			for(int j=0; j<16; j++)
				if(parents[i].a==sym48(j,a)){
					cout<<"hurokel! a="<<a<<" j="<<j<<endl;
					system("PAUSE");
				}
				
	}*/

	
	/*cout<<".."<<endl;
	Sector s(id(4,4,0,0));
	s.save();
	s.cleanup();*/
}
