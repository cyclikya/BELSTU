#pragma once
// - Levenshtein.h  
// -- ���������   �������e��� (������������ ����������������)
int levenshtein(
	int lx,           // ����� ����� x 
	const char x[],   // ����� ������ lx
	int ly,           // ����� ����� y
	const char y[]    // ����� y
);
// -- ���������   �������e��� (��������)
int levenshtein_r(
	int lx,           // ����� ������ x 
	const char x[],   // ������ ������ lx
	int ly,           // ����� ������ y
	const char y[]    // ������ y
);
int levenshtein_iterative(
    int lx,
    const char x[],
    int ly,
    const char y[]
);