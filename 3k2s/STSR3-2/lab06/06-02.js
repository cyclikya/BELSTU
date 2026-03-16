const express = require('express');
const nodemailer = require('nodemailer');
const { send } = require('./m0603');

const app = express();

app.use(express.urlencoded({ extended: true }));

const transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: 'vugorenko2000@gmail.com',
        pass: 'eiub ioil fgjy fhiv'
    }
});

app.get('/', (req, res) => {
    res.send(`
        <h2>Send Email</h2>
        <form method="POST">
            From: <input name="from"><br><br>
            To: <input name="to"><br><br>
            Message:<br>
            <textarea name="message"></textarea><br><br>
            <button type="submit">Send</button>
        </form>
    `);

});

app.post('/', async (req, res) => {

    const { from, to, message } = req.body;

    await transporter.sendMail({
        from: from,
        to: to,
        subject: "Lab06 message",
        text: message
    });

    res.send("Email sent successfully!");

});




// 2
app.get('/m0603', async (req, res) => {
    send('Смелов В.В. ');
});

app.listen(3000, () => {
    console.log("Server started");
    console.log("http://localhost:3000");
    console.log("http://localhost:3000/m0603");
});