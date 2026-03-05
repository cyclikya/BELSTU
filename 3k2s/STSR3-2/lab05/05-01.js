const EventEmitter = require("events");

class DB extends EventEmitter{
    constructor(){
        super();
        this.db_data =[
            {
                id: '1',
                name: 'Violetta',
                bday: '30.01.2006'
            },
            {
                id: '2',
                name: 'Elena',
                bday: '08.05.2005'
            },
            {
                id: '3',
                name: 'Alina',
                bday: '19.11.2005'
            }
        ]
    }

    dbState = [];
    
    async commit(){
        return new Promise((resolve, reject) => {
            try {
                let copy = structuredClone(this.db_data); 
                this.dbState.push(copy); 
                resolve(copy); 
            } catch (error) {
                reject(error); 
            }
        });
    }

    async select(obj){
        return this.db_data;
    }

    async insert(obj) {
        return new Promise((resolve, reject) => {
            
            if (!obj.id || !obj.name || (obj.bdate && new Date(obj.bdate) > new Date())) {
                console.log("Error: invalid input");
                return reject({ error: "ID and Name are required fields." });
            }
    
            let elem = this.db_data.find(item => item.id === obj.id);
            console.log(elem);
    
            if (elem) {
                console.log("Error: ID already used");
                return reject({ error: "ID already used." });
            }
    
            this.db_data.push({ id: obj.id, name: obj.name, bdate: obj.bdate });
            resolve({ success: true });
        });
    }

    async update(obj) {
        return new Promise((resolve, reject) => {
            if (obj.bday && new Date(obj.bday) > new Date()) {
                console.log("Error: invalid bday");
                return reject({ error: "Invalid birth date." });
            }
            
            let elem = this.db_data.find(item => item.id == obj.id);
            
            if (elem) {
                let index = this.db_data.indexOf(elem);
                if (index !== -1) {
                    // Обновляем элемент
                    this.db_data[index] = obj;
                    console.log("Updated data:", this.db_data);
                    return resolve({ 
                        success: true, 
                        message: "Record updated successfully",
                        updatedRecord: obj 
                    });
                }
            }
            
            console.log("Error: ID not found -", obj.id);
            return reject({ error: "ID not found." });
        });
    }

    async delete(obj) {
        return new Promise((resolve, reject) => {
           
            let elem = this.db_data.find(item => item.id == obj.id);
            if (elem) {
                let index = this.db_data.indexOf(elem);
                if (index !== -1) {
                    this.db_data.splice(index, 1); 
                    console.log(this.db_data);
                    return resolve(elem); 
                }
            }
    
            return reject({ error: "ID not found." });
        });
    }
}

exports.DB = DB;