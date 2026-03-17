const http = require('http');
const nodemailer = require('nodemailer');
const { send } = require('./m0603');

const transporter = nodemailer.createTransport({
    service: 'gmail',
    auth: {
        user: 'vugorenko2000@gmail.com',
        pass: 'eiub ioil fgjy fhiv'
    }
});

const server = http.createServer((req, res) => {
    if (req.url === '/' && req.method === 'GET') {
        res.writeHead(200, { 'Content-Type': 'text/html' });
        res.end(`
            <h2>Send Email</h2>
            <form method="POST">
                From: <input name="from"><br><br>
                To: <input name="to"><br><br>
                Message:<br>
                <textarea name="message"></textarea><br><br>
                <button type="submit">Send</button>
            </form>
        `);
    }
    else if (req.url === '/' && req.method === 'POST') {
        let body = '';
        req.on('data', chunk => body += chunk);
        req.on('end', async () => {
            const params = new URLSearchParams(body);
            const from = params.get('from');
            const to = params.get('to');
            const message = params.get('message');
            
            await transporter.sendMail({
                from: from,
                to: to,
                subject: "Lab06 message",
                text: message
            });
            
            res.writeHead(200, { 'Content-Type': 'text/html' });
            res.end("Email sent successfully!");
        });
    }
    else if (req.url === '/m0603' && req.method === 'GET') {
        send('Смелов В.В. ');
        res.writeHead(200, { 'Content-Type': 'text/html' });
        res.end("Message sent to m0603");
    }
    else {
        res.writeHead(404, { 'Content-Type': 'text/html' });
        res.end("404 Not Found");
    }
});

server.listen(3000, () => {
    console.log("Server started");
    console.log("http://localhost:3000");
    console.log("http://localhost:3000/m0603");
});