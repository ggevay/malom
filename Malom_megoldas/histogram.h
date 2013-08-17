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

#include "common.h"

//A class for calculating histograms, and visualizing with gnuplot.
class histogram{
public:
	vector<long long> distr;
	double w; //widths of the bins
	double min;

	//v is the vector, for which the histogram should be created
	//min and max give the x axis
	//n gives the number of bins
	//file_name is the name of the .hist (data for gnuplot), .plt and .png files that are generated when calling the appropriate methods
	histogram(vector<double> &v, double min, double max, int n, string filename);

	histogram(vector<long long> &distr, int min, string filename);
	histogram(vector<int> &distr, int min, string filename);

	//writes .hist file
	void write_data_to_file();

	//write plt file (bar width is set to w)
	void write_plt_file();

	//calls the above two methods, then calls gnuplot, and then calls xdg-open if open is set to true
	void gnuplot(bool open=true);

	histogram& eps();
private:
	string filename, hist_filename, plt_filename, png_filename;
	bool _eps;
};