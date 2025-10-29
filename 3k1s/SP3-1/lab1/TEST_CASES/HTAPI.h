#pragma once
#include <Windows.h>
#include <iomanip>
#include <iostream>

#define _CRT_SECURE_NO_WARNINGS

namespace HT    
{ 
	// API HT - программный интерфейс для доступа к НТ-хранилищу 
		//          НТ-хранилище предназначено для хранения данных в ОП в формате ключ/значение
		//          сохранность данных обеспечивается с помощью snapshot-механизма 
		//          Create - создать и открыть HT-хранилище для использования   
		//          Open   - открыть HT-хранилище для использования
		//          Insert - создать элемент данных
		//          Delete - удалить элемент данных    
		//          Get    - читать  элемент данных
		//          Update - изменить элемент данных
		//          Snap   - выполнить snapshot
		//          Close  - закрыть HT-хранилище для использования
		//          GetLastError - получить сообщение о последней ошибке  

	struct HTHandle    //блок управления HT 
	{
		HTHandle();
		HTHandle(int Capacity, int SecSnapshotInterval, int MaxKeyLength, int MaxPayloadLength, const wchar_t FileName[512]);
		int     Capacity;				// емкость хранилища в количестве элементов             
		int     SecSnapshotInterval;	// периодичность сохранения в сек
		int     MaxKeyLength;			// максимальная длина ключа         
		int     MaxPayloadLength;		// максимальная длина данных   
		char    FileName[512];			// имя файла        
		HANDLE  File;                   // File HANDLE != 0, если файл открыт
		HANDLE  FileMapping;			// Mapping File HANDLE != 0, если mapping создан
		LPVOID  Addr;                   // Addr != NULL, если mapview выполнен
		char    LastErrorMessage[512];  // сообщение об последней ошибке или 0x00
		time_t  Lastsnaptime;           // дата последнего snap'a (time())
		HANDLE  Mutex;
		int ElementCount;
		HANDLE SnapshotThread; 
	};

	struct Element  // элемент
	{
		Element();
		Element(const void* key, int keylength);                                            // for Get
		Element(const void* key, int keylength, const void* payload, int  payloadlength);   // for Insert
		Element(Element* oldelement, const void* newpayload, int  newpayloadlength);        // for update
		void* key;                        // значение ключа
		int             keylength;		  // рахмер ключа         
		void* payload;					  // данные
		int             payloadlength;    // размер данных   
	};

	HTHandle* Create    // создать HT    
	(
		int	  Capacity,					// емкость хранилища  
		int   SecSnapshotInterval,		// периодичность сохранения в сек
		int   MaxKeyLength,             // максимальный размер ключа
		int   MaxPayloadLength,         // максимальный размер данных 
		const wchar_t FileName[512]     // имя файла    
	);

	HTHandle* Open // открыть НТ
	(
		const wchar_t FileName[512]     // имя файла    
	); 	// = NULL успешное завершение
    

	BOOL Snap   // выполнить snapshot     
	(
		HTHandle* ht     // управление НТ (File, FileMapping)      
	);

	BOOL Close  // закрыть HT хранилище    
	(
		HTHandle* ht          
	);	

	BOOL Insert   // добавить элемент в хранилище
	(
		HTHandle* ht,    // управление HT       
		Element* el      // элемент
	);	// ==TRUE  успешное завершение

	BOOL Delete   // удалить элемент в хранилище
	(
		HTHandle* ht,          
		Element* el            
	);	// ==TRUE  успешное завершение

	Element* Get   // читать элемент в хранилище  
	(
		HTHandle* ht,        
		Element* el            
	);  // != NULL успешное завершение

	BOOL Update    // изменить элемент в хранилище
	(
		HTHandle* ht,            // управление HT
		Element* oldelement,     // старый элемент (ключ, размер ключа)
		const void* newpayload,  // новые данные   
		int  newpayloadlength    // размер новых данных  
	); 	// != NULL успешное завершение

	char* GetHTLastError  // получить сообщение о последней ошибке
	(
		HTHandle* ht                        
	);

	void Print     // распечатать элемент                                 
	(
		const Element* el      
	);

	// Вспомогательные функции
	const int DELETED = -1;

	void SetDeletedFlag(Element* el);

	void SetLastError(HTHandle* ht, const char* message);

	BOOL CheckElementParm(HTHandle* ht, Element* el);

	BOOL CheckElementParm(HTHandle* ht, int payloadLength);

	BOOL CheckEqualElementKeys(Element* el1, Element* el2);

	int HashFunction(const Element* el, int size, int p);

	int NextHash(int hash, int size, int p);

	Element* GetElementFromHT(HTHandle* ht, int hash);

	BOOL SetElementToHT(HTHandle* ht, Element* el, int n);

	DWORD WINAPI SnapshotRoutine(HTHandle* ht);

	BOOL IsDeleted(Element* el);

	void UpdateElement(HTHandle* ht, Element* el, const void* newpayload, int newpayloadlength);
};