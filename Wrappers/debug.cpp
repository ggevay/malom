#include "stdafx.h" 


#include <string>
#include <sstream>



const char* toclp(board b){
	board mask=1;
	vector<int> kit(24,-1);
	for(int i=0; i<24; i++){
		if((mask<<i)&b)
			kit[i]=0;
	}
	for(int i=24; i<48; i++){
		if((mask<<i)&b)
			kit[i-24]=1;
	}

	stringstream ss;
	for(int i=0; i<24; i++)
		ss<<kit[i]<<",";
	ss<<"0,0,0,1,0,0,0,0,False,0,0,malom";

	char *ret=new char[1024];
	strcpy_s(ret,1024,ss.str().c_str());
	return ret;
}
