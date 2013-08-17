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
#include "histogram.h"


void call_gnuplot(string plt_file, string data_file, string png_filename="tmp.png", bool open=true, bool del_pltfile=true, bool del_datafile=true){
	string cmd=("gnuplot "+plt_file);
	LOG("Calling Gnuplot:  %s\n",cmd.c_str());
	int r=system(cmd.c_str());
	if(!r){
		if(del_pltfile) remove(plt_file.c_str());
		if(del_datafile) remove(data_file.c_str());
		if(open){
			string open_cmd="start "+png_filename;
			int r=system(open_cmd.c_str());
			if(r){
				LOG("Calling start failed with code %d; command was  %s\n",r,open_cmd.c_str());
			}
		}
	}else{
		LOG("Calling gnuplot failed with code %d; command was  %s\n",r,cmd.c_str());
	}
}


histogram::histogram(vector<double> &v, double min, double max, int n, string filename)
		: distr(n+1), w((max-min)/n), min(min),
		filename(filename), hist_filename(filename + ".hist"), plt_filename(filename + ".plt"), png_filename(filename + ".png"), _eps{ false } {
	for(auto it=v.begin(); it!=v.end(); ++it){
		auto x=*it;
		assert(x>=min && x<=max);
		this->distr[(int)((x-min)/w)]++;
	}
}

histogram::histogram(vector<long long> &distr, int min, string filename)
	: distr(distr), w(1), min(min),
	filename(filename), hist_filename(filename + ".hist"), plt_filename(filename + ".plt"), png_filename(filename + ".png"), _eps{ false }
	{}

histogram::histogram(vector<int> &distr, int min, string filename) : histogram{ vector<long long>(distr.begin(), distr.end()), min, filename } {}


std::ostream& operator<<(std::ostream &out, const histogram &hist){
	double bin=hist.min;
	for(unsigned int i=0; i<hist.distr.size(); i++){
		out<<bin<<" "<<hist.distr[i]<<endl;
		bin+=hist.w;
	}
	return out;
}

void histogram::write_data_to_file(){
	ofstream f(hist_filename);
	f<<(*this);
	f.close();
}


void histogram::write_plt_file(){
	ofstream f(plt_filename);
	if(!_eps){
		f<<"\
		   	set term png size 800, 600 \n\
			set output \""<<png_filename<<"\" \n\
			set key off \n\
			set style fill solid \n\
			set boxwidth "<<w<<" \n\
			set autoscale fix \n\
			plot \""<<hist_filename<<"\" with boxes";
	} else {
		f<<"\
		    #set ylabel \"billion\" 1,0 \n\
		    #set format y \"$%1.0t$\" \n\
		    set term epslatex size 8.89cm,4cm \n\
			set output \"" << filename << ".tex\" \n\
			set key off \n\
			set style fill solid noborder \n\
			set boxwidth " << w << " \n\
			set autoscale fix \n\
			plot \"" << hist_filename << "\" with boxes";
		/*f<<"\
		    set term eps size 8.89cm,6.65cm \n\
			set output \"" << filename << ".eps\" \n\
			set key off \n\
			set style fill solid noborder \n\
			set boxwidth " << w << " \n\
			set autoscale fix \n\
			plot \"" << hist_filename << "\" with boxes";*/
	}
	f.close();
}

void histogram::gnuplot(bool open){
	write_data_to_file();
	write_plt_file();

	call_gnuplot(plt_filename, hist_filename, png_filename, open && !_eps);
}

histogram& histogram::eps(){
	_eps = true;
	return *this;
}