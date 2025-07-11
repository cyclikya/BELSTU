﻿// Combi.h  
#pragma once 
namespace combi
{
    struct  xcombination           // генератор  сочетаний (эвристика) 
    {
        short  n,                  // количество элементов исходного множества  
            m,                  // количество элементов в сочетаниях 

            * sset;            	  // массив индексов текущего сочетания  
        xcombination(
            short n = 1, //количество элементов исходного множества  
            short m = 1  // количество элементов в сочетаниях
        );
        void reset();              // сбросить генератор, начать сначала 
        short getfirst();          // сформировать первый массив индексов    
        short getnext();           // сформировать следующий массив индексов  
        short ntx(short i);        // получить i-й элемент массива индексов  
        unsigned __int64 nc;       // номер сочетания  0,..., count()-1   
        unsigned __int64 count() const;  // вычислить количество сочетаний      
    };
};

