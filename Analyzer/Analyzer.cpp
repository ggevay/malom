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

//#define ANALYZER  //ez a build beallitasoknal van mostmar

#include "common.h"
#include "auto_grow_vector.h"
#include "histogram.h"
#include "sec_val.h"
#include "sector.h"
#include "debug.h"
//#include "sector_graph.h"

#include "dirent.h"


//b starts with a
bool starts_with(string a, string b){
	if(a.size()>b.size())
		return false;

	for(unsigned int i=0; i<a.size(); i++)
		if(a[i]!=b[i])
			return false;

	return true;
}
bool ends_with(string a, string b){
	if(a.size()>b.size())
		return false;

	int diff = (int)(b.size()-a.size());
	for(unsigned int i=0; i<a.size(); i++)
		if(a[i]!=b[i+diff])
			return false;

	return true;
}


struct wdl {
	int win,draw,loss;
	double val;
	wdl():win(6),draw(6),loss(6),val(6){}
	wdl(int win,int draw,int loss):win(win),draw(draw),loss(loss),val(6){}
};


int maxval = numeric_limits<int>::min();
board maxval_board;
id maxval_board_id;
pn_vector<long long> dist_dist; //distance distribution
pn_vector<long long> akey1_dist; //akey1 distribution  (range will be forced to virt_loss_val-virt_win_val)
int read_virt_loss_val = 0, read_virt_win_val = 0;
map<id, int> all_maxvals;

map<id, wdl> read_sm(long long &sym_count, long long &win_count, long long &loss_count, long long &draw_count){
	DIR *dir;
	struct dirent *ent;
	if((dir = opendir(".")) != NULL) {
		map<id, wdl> sm;
		sym_count = 0; win_count = 0; loss_count = 0; draw_count = 0;
		while((ent = readdir(dir)) != NULL) {
			string f_name = ent->d_name;
			if(starts_with(VARIANT_NAME, f_name) && ends_with(string(".analyze") + FNAME_SUFFIX, f_name)){
				id s;
				sscanf_s(f_name.c_str(), VARIANT_NAME"_%d_%d_%d_%d", &s.W, &s.B, &s.WF, &s.BF);

				FILE *in;
				errno_t res = fopen_s(&in, f_name.c_str(), "r");
				if(res){
					perror((string("Could not open file ") + f_name).c_str());
				}

				int cur_maxval, cur_sym_count, cur_win_count, cur_draw_count, cur_loss_count;
				board cur_maxval_board;
				fscanf_s(in, "%d\n%lld\n%d\n%d %d %d\n", &cur_maxval, &cur_maxval_board, &cur_sym_count, &cur_win_count, &cur_draw_count, &cur_loss_count);
				//int total = win_count+draw_count+loss_count;
				//secs.push_back(make_pair((double)(win_count + draw_count/2) / total, s));
				sm[s] = wdl(cur_win_count, cur_draw_count, cur_loss_count);

				if(cur_maxval > maxval){
					maxval = cur_maxval;
					maxval_board = cur_maxval_board;
					maxval_board_id = s;
				}
				sym_count += cur_sym_count;  win_count += cur_win_count;  loss_count += cur_loss_count;  draw_count += cur_draw_count;

				all_maxvals[s] = cur_maxval;

				int dist_dist_size;  fscanf_s(in, "%d\n", &dist_dist_size);
				for(int i = 0; i < dist_dist_size; i++){
					int x;  fscanf_s(in, "%d\n", &x);
					assert(x >= 0);
					dist_dist[i - 1] += x; //dist_dist always starts at -1 (which is the draw_count)
				}
#ifdef DD
				int cur_virt_loss_val, cur_virt_win_val;
				fscanf_s(in, "%d %d\n", &cur_virt_loss_val, &cur_virt_win_val);
				REL_ASSERT(read_virt_loss_val == 0 || read_virt_loss_val == cur_virt_loss_val);
				read_virt_loss_val = cur_virt_loss_val;  read_virt_win_val = cur_virt_win_val;
				int akey1_dist_size;  fscanf_s(in, "%d\n", &akey1_dist_size);
				assert(akey1_dist_size == read_virt_win_val - read_virt_loss_val + 1);
				for(int i = 0; i < akey1_dist_size; i++){
					int x;  fscanf_s(in, "%d\n", &x);
					assert(x >= 0);
					akey1_dist[i + read_virt_loss_val] += x;
				}
				assert(akey1_dist[0] >= 0);
				assert((int)akey1_dist.size() == read_virt_win_val - read_virt_loss_val + 1);
#endif

				fclose(in);

				printf(".");
			}
		}
		closedir(dir);
		printf("\n");
		return sm;
	} else {
		perror("could not open directory");
		exit(1);
	}
}


//vector<id> topsorted_sector_list;
//
//set<id> vis;
//void sec_dfs(id u){
//	if(vis.count(u))
//		return;
//
//	vis.insert(u);
//
//	auto chd=graph_func(u);
//	for(auto it=chd.begin(); it!=chd.end(); ++it)
//		sec_dfs(*it);
//
//	topsorted_sector_list.push_back(u);
//}
//
//void rev_topsort(){
//	for(unsigned int i=0; i<sector_list.size(); i++)
//		sec_dfs(sector_list[i]);
//	//reverse(topsorted_sector_list.begin(), topsorted_sector_list.end());
//}


//double sqr(double x){return x*x;}


void calc_sec_vals(map<id, wdl> &sm){
	//init_sector_graph();

	vector<pair<double, id> > secs;
	for(auto it = sm.begin(); it != sm.end(); ++it){
		id s = it->first;

		double val1 = (double)sm[s].win / (sm[s].win + sm[s].draw + sm[s].loss);

		double val;

		if(!s.transient()){
			assert(sm.count(-s));
			//double val2 = (double)sm[-s].win / (sm[-s].win + sm[-s].draw + sm[-s].loss);

			//val = val1-val2;

			val = ((sm[s].win + sm[-s].loss) + ((double)sm[s].draw / 2 + (double)sm[-s].draw / 2)) / (sm[s].win + sm[s].draw + sm[s].loss + sm[-s].win + sm[-s].draw + sm[-s].loss);
		} else{
			//val = 2; //ide kell majd a masik (pl. a gyerekszektorok ertekenek atlagolasa? (kulon ciklusban, mert most meg nem biztos, hogy ki vannak szamolva))

			val = (sm[s].win + (double)sm[s].draw / 2) / (sm[s].win + sm[s].draw + sm[s].loss);
		}

		sm[s].val = val;
		secs.push_back(make_pair(sm[s].val, s));
	}

	//rev_topsort();

	/*for(unsigned int i=0; i<topsorted_sector_list.size(); i++){
	id s=topsorted_sector_list[i];

	if(sm[s].val==2){
	double sum = 0;
	auto chds = graph_func(s);
	for(auto it=chds.begin(); it!=chds.end(); ++it){
	assert(sm[*it].val!=2);
	sum += sm[*it].val;
	}
	sm[s].val = -sum/chds.size();
	}else{
	assert(sm[s].val<1.5);
	}

	secs.push_back(make_pair(sm[s].val, s));
	}*/

	const double eps = 0.00000000001;

	//eddig [0, 1] volt, most [-0.5, 0.5] lesz
	for(unsigned int i = 0; i<secs.size(); i++){
		secs[i].first -= 0.5;
		if(abs(secs[i].first)<eps && secs[i].first != 0){
			assert(false); //igazabol ez nem lenne nagy baj, ha aztan itt 0-ra allitjuk
			secs[i].first = 0;
		}
	}

	auto comp = [](pair<double, id> &a, pair<double, id> &b) -> bool {
		//a<b: itt ket dolog kell:
		//	-a twineknek kozre kell fogniuk a nem tranziens EKS-eket
		//	-a twineket kulonbozo fajta zarojelparokkent elkepzelve helyes zarojelezest kapunk
		//	(felteve a lentebbi elso assertet)
		if(a.first != b.first)
			return a.first<b.first;
		int al = a.second.eks() && !a.second.transient() ? 0 : (a.second<-a.second ? -1 : 1);
		int bl = b.second.eks() && !b.second.transient() ? 0 : (b.second<-b.second ? -1 : 1);
		//a kovetkezo ket return tartozik a masodik gondolatjelhez
		if(al == 1 && bl == 1)
			return -a.second>-b.second; //(itt a - az id-ket negalja) (tulajdonkeppen a wu-kon akarunk rendezest, ugyhogy kanonizalunk)
		if(al == -1 && bl == -1)
			return a.second<b.second;
		return al<bl;
	};
	sort(secs.begin(), secs.end(), comp);

	for(unsigned int i = 0; i<secs.size() - 1; i++){
		//assert(abs(secs[i].first - secs[i+1].first)>eps || secs[i].first==0 && secs[i+1].first==0); //ez nem volt igaz
		assert(abs(secs[i].first - secs[i + 1].first)>eps || secs[i].first == secs[i + 1].first); //ez valojaban a rendezes elofeltetelehez tartozik
		assert(!comp(secs[i + 1], secs[i])); //(ez meg utofeltetel)
	}

	int first_eks;
	for(first_eks = 0; !secs[first_eks].second.eks(); first_eks++);
	int smallest_left, smallest_right; //a legkisebb hagymalevel
	for(smallest_left = first_eks; !secs[smallest_left].second.twine(); smallest_left--);
	for(smallest_right = first_eks; !secs[smallest_right].second.twine(); smallest_right++);

	vector<int> sec_val(secs.size());
	int left = (smallest_left + smallest_right) / 2, right = (smallest_left + smallest_right + 1) / 2;
	if(left == right){
		sec_val[left] = 0;
		left--; right++;
	}
	int oc = 1;
	while(1){
		//elfogyas
		if(left<0){
			while(right<(int)secs.size())
				sec_val[right++] = oc++;
			break;
		}
		if(right >= (int)secs.size()){
			while(left >= 0)
				sec_val[left--] = -oc++;
			break;
		}

		id &l = secs[left].second, &r = secs[right].second;
		if(l.twine() && r.twine()){
			assert(-l == r);
			sec_val[left--] = -(sec_val[right++] = oc++);
		} else if(!l.twine() && r.twine()){
			sec_val[left--] = -oc++;
		} else if(l.twine() && !r.twine()){
			sec_val[right++] = oc++;
		} else{
			sec_val[left--] = -(sec_val[right++] = oc++); //ez a tomorites
		}
	}

	//korrigalni kell a nem-transziens eks-ek erteket 0-ra (zero-osszeguseg, ld. doc)
	for(unsigned int i = 0; i < secs.size(); i++){
		if(secs[i].second.eks() && !secs[i].second.transient())
			sec_val[i] = 0;
	}


	//itt azt akartuk csinalni, hogy csak a negativokra megcsinalni a klaszterezest, es aztan tukrozni, de
	//attol rossz az egesz, hogy itt nem talalunk part a nem EKS tranzienseknek

	//vector<double> sec_val;
	//for(unsigned int i=0; i<secs.size(); i++){
	//	//assert((secs[i].first==0) == (abs(secs[i].first)<eps));
	//	if(abs(secs[i].first)<eps)
	//		secs[i].first=0;
	//	sec_val.push_back(secs[i].first);
	//}

	//
	//int n=0;
	//for(int i=0; sec_val[i]<0; i++)
	//	n++;

	//vector<vector<double> > sse(n,vector<double>(n+1)); //sse[i][j]: az [i,j) sse-je
	//for(int i=0; i<n; i++){
	//	for(int j=i+1; j<n+1; j++){
	//		double sum=0;
	//		for(int k=i; k<j; k++)
	//			sum+=sec_val[k];
	//		double avg=sum/(j-i);
	//		sum=0;
	//		for(int k=i; k<j; k++)
	//			sum+=sqr(sec_val[k]-avg);
	//		sse[i][j]=sum;
	//	}
	//}

	//int num_clus = min(126,n); //ne lehessen tobb klaszter, mint pont
	////dp[i][j]: [0,i) intervallumot j klaszterre bontva mi az optimalis klaszterezes (ertek, az utolso klaszter eleje)
	//vector<vector<pair<double,int> > > dp(n+1, vector<pair<double,int> >(num_clus+1));
	//for(int i=1; i<=n; i++)
	//	dp[i][1]=make_pair(sse[0][i], 0);
	//for(int i=1; i<=n; i++){
	//	for(int j=2; j<=min(i, num_clus); j++){
	//		dp[i][j]=make_pair(numeric_limits<double>::max(), -1);
	//		for(int k=j-1; k<i; k++){
	//			double cur = dp[k][j-1].first + sse[k][i];
	//			if(cur<dp[i][j].first)
	//				dp[i][j]=make_pair(cur, k);
	//		}
	//	}
	//}
	//vector<int> clus_id(n);
	//int k=n;
	//for(int j=num_clus; j>0; j--){
	//	int next=dp[k][j].second;
	//	for(int i=k-1; i>=next; i--)
	//		clus_id[i]=j-1;
	//	k=next;
	//}

	//vector<pair<int,id> > sec_clus(secs.size());
	//for(int i=0; i<n; i++){
	//	id s=secs[i].second;
	//	sec_clus[i]=make_pair(clus_id[i]-num_clus, s);
	//	
	//	int j;
	//	for(j=n; secs[j].second!=-s; j++);
	//	sec_clus[j]=make_pair(-(clus_id[i]-num_clus), -s);
	//}
	//for(int i=n+1; secs[i].first==0; i++)
	//	sec_clus[i]=make_pair(0, secs[i].second);
	//sort(sec_clus.begin(), sec_clus.end());


	FILE *out;
	if(fopen_s(&out, "analyze_total.txt", "w")){
		perror("Could not open output file");
		abort();
	}
	for(unsigned int i = 0; i<secs.size(); i++){
		/*auto sid=secs[i].second;
		fprintf_s(out, "%llf %s %d\n", secs[i].first, secs[i].second.to_string().c_str(), sid.W+sid.WF-(sid.B+sid.BF));*/

		/*auto sid=sec_clus[i].second;
		fprintf_s(out, "%llf  %d  %s %d\n", sec_clus[i].first, sec_clus[i].first, sid.to_string().c_str(), sid.W+sid.WF-(sid.B+sid.BF));*/

		auto sid = secs[i].second;
		fprintf_s(out, "%llf  %s (%d)  %d\n", secs[i].first, secs[i].second.to_string().c_str(), sid.W + sid.WF - (sid.B + sid.BF), sec_val[i]);
	}
	fclose(out);

	int virt_loss_val = (*min_element(sec_val.begin(), sec_val.end())) - 1;
	int virt_win_val = (*max_element(sec_val.begin(), sec_val.end())) + 1;
	//We need to make these zero-sum (because they get negated):
	if(abs(virt_loss_val) < abs(virt_win_val))
		virt_loss_val = -virt_win_val;
	else
		virt_win_val = -virt_loss_val;
#ifdef DD
	assert(2 * virt_loss_val - 5 > sec_val_min_value); //azert kell, mert egyreszt korrigalas, masreszt levonunk belole egyet a gui_eval_elem2-ben a KLE-s szektorok ertekenel (a -5 csak biztonsagi, lehet, hogy eleg lenne -1 is)
#endif

	fopen_s(&out, sec_val_fname.c_str(), "wt");
	fprintf_s(out, "virt_loss_val: %d\nvirt_win_val: %d\n", virt_loss_val, virt_win_val);
	fprintf_s(out, "%u\n", secs.size());
	for(unsigned int i = 0; i<secs.size(); i++){
		auto id = secs[i].second;
		fprintf_s(out, "%d %d %d %d  %d\n", id.W, id.B, id.WF, id.BF, sec_val[i]);
	}
	fclose(out);
}


void print_stats(long long &sym_count, long long &win_count, long long &loss_count, long long &draw_count){
#ifdef DD
	init_sec_vals();

	assert(read_virt_loss_val == virt_loss_val && read_virt_win_val == virt_win_val); //virt_loss_val was initialized by init_sec_vals()
	akey1_dist[virt_loss_val];  akey1_dist[virt_win_val]; //force range
	assert(akey1_dist.size() == virt_win_val - virt_loss_val + 1);
#endif

	FILE *out;
	fopen_s(&out, VARIANT_NAME".statistics", "w");
	fprintf(out, "maxval: %d\n", maxval);
	fprintf(out, "maxval_board: %lld, toclp: %s\n", maxval_board, toclp3(maxval_board, maxval_board_id).c_str());
	long long tot_count = win_count + loss_count + draw_count;
	fprintf(out, "total gamestates: %lld\n", tot_count);
	fprintf(out, "sym: %lld\n", sym_count);
	fprintf(out, "win/draw/loss: %lld, %lld, %lld\n", win_count, draw_count, loss_count);
	fprintf(out, "win/draw/loss ratios: %f, %f, %f\n", (double)win_count / tot_count, (double)draw_count / tot_count, (double)loss_count / tot_count);
	fprintf(out, "\ndistance distribution:\n");
	for(auto it = dist_dist.begin(); it != dist_dist.end(); ++it)
		fprintf(out, "%d %lld\n", it.ind(), *it);

#ifdef DD
	fprintf(out, "\nakey1 distribution:\n");
	for(auto it = akey1_dist.begin(); it != akey1_dist.end(); ++it)
		if(*it)
			fprintf(out, "%s %lld\n", sec_val_to_sec_name(it.ind()).c_str(), *it);
	fprintf(out, "\n");
#endif

	fclose(out);

	auto ddv = dist_dist.to_vector();
	histogram(ddv, -1, VARIANT_NAME"__disthist").eps().gnuplot(false);
	//histogram(vector<long long>(ddv.begin()+36, ddv.end()), -1, VARIANT_NAME"__disthist_above35").eps().gnuplot(false);
	histogram(vector<long long>(ddv.begin() + 81, ddv.end()), -1, VARIANT_NAME"__disthist_above80").eps().gnuplot(false);

#ifdef DD
	histogram(akey1_dist.to_vector(), virt_loss_val, VARIANT_NAME"__akey1hist").gnuplot(false);
#endif
}


void full_table(){
#ifdef DD
	assert(false);
#endif
	FILE *out;
	fopen_s(&out, VARIANT_NAME".fulltable", "w");
	string table[max_ksz+1][max_ksz+1];
	for(int i = 3; i <= max_ksz; i++){
		for(int j = 3; j <= max_ksz; j++){
			id sid(0, 0, i, j);
			printf("Creating sector obj %s\n", sid.to_string().c_str());
			Sector s(sid);
			s.allocate(false);
			const board start_pos(0);
			eval_elem2 r = s.get_eval(start_pos);
			if(r.cas() == eval_elem2::Val){
				table[i][j] = string(r.value().key1==1 ? "W" : "L") + tostring(r.value().key2);
			} else {
				table[i][j] = "D";
			}
			s.release_hash();
			s.release();
		}
	}
	fprintf(out, "%-5s", "");
	for(int i = 3; i <= max_ksz; i++)
		fprintf(out, "%-5d", i);
	fprintf(out, "\n");
	for(int i = 3; i <= max_ksz; i++){
		fprintf(out, "%-5d", i);
		for(int j = 3; j <= max_ksz; j++){
			fprintf(out, "%-5s", table[i][j].c_str());
		}
		fprintf(out, "\n");
	}
	fclose(out);
}

string add_plus_if0(double x){
	double rx = round(x);
	if(rx == 0 && x != 0){
		return R"(0\textsuperscript{+})";
	} else {
		char buf[128];
		sprintf_s(buf, "%.0f", x);
		return string{ buf };
	}
}

void ratio_table(map<id, wdl> &sm){
	FILE *out;
	fopen_s(&out, VARIANT_NAME".ratiotable", "w");
	fprintf(out, R"(\begin{tabular}{)");
	for(int i = 3; i <= max_ksz + 1; i++)
		fprintf(out, "c");
	fprintf(out, "}\n");
	fprintf(out, "~ ");
	for(int i = 3; i <= max_ksz; i++)
		fprintf(out, "& %d ", i);
	fprintf(out, "\\\\\n");
	for(int i = 3; i <= max_ksz; i++){
		fprintf(out, "%d ", i);
		for(int j = 3; j <= max_ksz; j++){
			id sid(i, j, 0, 0);
			assert(sm.count(sid));
			double sum = sm[sid].win + sm[sid].draw + sm[sid].loss;
			sum /= 100;
			//fprintf(out, "& %.0f/%.0f/%.0f ", sm[sid].win / sum, sm[sid].draw / sum, sm[sid].loss / sum);
			fprintf(out, "& %s / %s / %s ", add_plus_if0(sm[sid].win / sum).c_str(), add_plus_if0(sm[sid].draw / sum).c_str(), add_plus_if0(sm[sid].loss / sum).c_str());
		}
		fprintf(out, "\\\\\n");
	}
	fprintf(out, R"(\end{tabular})");
	fclose(out);
}

void maxval_table(){
	FILE *out;
	fopen_s(&out, VARIANT_NAME".maxvaltable", "w");
	fprintf(out, R"(\begin{tabular}{)");
	for(int i = 3; i <= max_ksz + 1; i++)
		fprintf(out, "c");
	fprintf(out, "}\n");
	fprintf(out, "~ ");
	for(int i = 3; i <= max_ksz; i++)
		fprintf(out, "& %d ", i);
	fprintf(out, "\\\\\n");
	for(int i = 3; i <= max_ksz; i++){
		fprintf(out, "%d ", i);
		for(int j = 3; j <= max_ksz; j++){
			id sid(i, j, 0, 0);
			assert(all_maxvals.count(sid));
			fprintf(out, "& %d ", all_maxvals[sid]);
		}
		fprintf(out, "\\\\\n");
	}
	fprintf(out, R"(\end{tabular})");
	fclose(out);
}


int main(int argc, char **argv)
{
	if(argc == 2 && argv[1] == string{ "-full_table" }){
		full_table();
	} else {
		long long sym_count = 0, win_count = 0, loss_count = 0, draw_count = 0;
		auto sm = read_sm(sym_count, win_count, loss_count, draw_count);

		if(argc == 2 && argv[1] == string{ "-sec_val" }){
			calc_sec_vals(sm);
		} else if(argc == 2 && argv[1] == string{ "-ratio_table" }) {
			ratio_table(sm);
		} else if(argc == 2 && argv[1] == string{ "-maxval_table" }) {
			maxval_table();
		} else {
			print_stats(sym_count, win_count, loss_count, draw_count);
		}
	}

	return 0;
}
