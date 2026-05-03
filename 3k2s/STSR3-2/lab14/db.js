const Sequelize = require("sequelize");
require('dotenv').config();

const sequelize = new Sequelize(
    process.env.DB,
    process.env.USER,
    process.env.PASSWORD,
    {
        host: process.env.HOST,
        port: process.env.SQL_PORT,
        dialect: process.env.DIALECT,
        dialectOptions: {
            options: {
                encrypt: false,
                trustServerCertificate: false,
                enableArithAbort: false
            },
        },
        pool: {
            max: 10,
            min: 1,
            idle: 20000,
            acquire: 30000
        },
        logging: console.log 
    }
);

module.exports = sequelize;