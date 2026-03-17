const nodemailer = require('nodemailer');

const MY_EMAIL = "vugorenko2000@gmail.com";
const APP_PASSWORD = "eiub ioil fgjy fhiv";

const transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: MY_EMAIL,
        pass: APP_PASSWORD
    }
});

function send(message) {

    transporter.sendMail({
        from: MY_EMAIL,
        to: MY_EMAIL,
        subject: "Message from m0603 module",
        text: message
    }, (error, info) => {

        if (error) {
            console.log("Ошибка отправки:", error);
        } else {
            console.log("Письмо отправлено:", info.response);
        }

    });

}

module.exports = { send };