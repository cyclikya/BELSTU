const https = require('https');
const fs = require('fs');

const options = {
    key: fs.readFileSync('resource-key.pem'),
    cert: fs.readFileSync('resource-cert.pem')
};

https.createServer(options, (req, res) => {
    console.log('GET request:', req.url);

    res.writeHead(200, {
        'Content-Type': 'text/html; charset=utf-8'
    });

    res.end(`
        <h1>LAB22 HTTPS SERVER</h1>
        <p>Resource: UVR</p>
        <p>CA: DUS</p>
        <p>HTTPS работает через сертификат, подписанный CA.</p>
    `);
}).listen(3000, () => {
    console.log('HTTPS server started: https://LAB22-UVR:3000');
});